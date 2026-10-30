using System.Globalization;
using System.Reflection;
using System.Text.Json;
using Procession.Core;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

namespace Procession.Game;

public sealed class GameSave
{
    public EncounterState Encounter { get; set; } = new();
    public string? LastStorm { get; set; }
    public string? EncounterId { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }
    public HashSet<long> HealOnLoad { get; set; } = new();
    public HashSet<string> AbandonedEvents { get; set; } = new();
    public int EligibleChanterDeaths { get; set; }
    public int? PortalId { get; set; }
}

public sealed class ProcessionConfig
{
    public double EventChance { get; set; } = 1;
    public double ArrivalMinimumSeconds { get; set; } = 15;
    public double ArrivalMaximumSeconds { get; set; } = 30;
}

/// <summary>1.22.7 adapter. Owns only tagged Procession entities and the active vanilla storm lifetime.</summary>
public sealed class ProcessionSystem : ModSystem
{
    internal const string Ownership = "processionEvent";
    private ICoreServerAPI? server;
    private ICoreClientAPI? client;
    private SystemTemporalStability? storms;
    private ModSystemRifts? rifts;
    private GameSave save = new();
    private ProcessionConfig config = new();
    private EncounterDirector director = new();
    private long tickId;
    private bool initialized;
    private bool baselineQuery;
    private int? portalId { get => save.PortalId; set => save.PortalId = value; }
    private bool portalRecoveryPending;
    private float soundTimer;
    private float recoveryTimer;
    private float loadedSeconds;
    private bool cleaning;
    private readonly PropertyInfo? entityProperties = typeof(Entity).GetProperty("Properties");
    private readonly MethodInfo? stormTick = typeof(SystemTemporalStability).GetMethod("onTempStormTick", BindingFlags.Instance | BindingFlags.NonPublic);
    private readonly MethodInfo? riftPlacement = typeof(ModSystemRifts).GetMethod("canSpawnRiftAt", BindingFlags.Instance | BindingFlags.NonPublic);
    private readonly MethodInfo? broadcastRifts = typeof(ModSystemRifts).GetMethod("BroadCastRifts", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
    public ProgressionSites? Sites { get; private set; }

    public override void Start(ICoreAPI api)
    {
        api.RegisterEntityBehaviorClass("ProcessionChanter", typeof(ProcessionChanter));
        api.RegisterEntityBehaviorClass("ProcessionBoss", typeof(ProcessionBoss));
        api.RegisterBlockClass("ProcessionClosingBell", typeof(ProcessionClosingBell));
        api.RegisterItemClass("ProcessionRouteChart", typeof(ProcessionRouteChart));
    }

    public override void StartServerSide(ICoreServerAPI api)
    {
        server = api;
        storms = api.ModLoader.GetModSystem<SystemTemporalStability>();
        rifts = api.ModLoader.GetModSystem<ModSystemRifts>();
        config = api.LoadModConfig<ProcessionConfig>("procession.json") ?? new();
        config.EventChance = Math.Clamp(double.IsFinite(config.EventChance) ? config.EventChance : 1, 0, 1);
        config.ArrivalMinimumSeconds = Math.Clamp(double.IsFinite(config.ArrivalMinimumSeconds) ? config.ArrivalMinimumSeconds : 15, 15, 30);
        config.ArrivalMaximumSeconds = Math.Clamp(double.IsFinite(config.ArrivalMaximumSeconds) ? config.ArrivalMaximumSeconds : 30, config.ArrivalMinimumSeconds, 30);
        api.StoreModConfig(config, "procession.json");
        Sites = new ProgressionSites(api);
        api.Event.SaveGameLoaded += Load;
        api.Event.GameWorldSave += Save;
        api.Event.OnEntityDeath += OnDeath;
        api.Event.OnEntityLoaded += OnLoaded;
        api.Event.OnEntityDespawn += OnDespawn;
        storms.OnGetTemporalStability += ModifyStability;
        tickId = api.Event.RegisterGameTickListener(Tick, 250);
        api.ChatCommands.Create("processionaudit").WithDescription("Read-only validation of registered Procession assets and runtime hooks.")
            .RequiresPrivilege(Privilege.controlserver).HandleWith(_ => Audit());
        api.ChatCommands.Create("procession").WithDescription("Show the Procession investigation and current event state.")
            .RequiresPrivilege(Privilege.chat).RequiresPlayer().HandleWith(args => TextCommandResult.Success(Describe(args.Caller.Player)))
            .BeginSubCommand("guide").WithDescription("Read your guaranteed investigation route.")
            .HandleWith(args => TextCommandResult.Success(Sites.DescribeRoute(args.Caller.Player))).EndSubCommand()
            .BeginSubCommand("survey").WithDescription("Safely resurvey a nearby pending expedition landmark.")
            .HandleWith(args => TextCommandResult.Success(Sites.SurveyNearby(args.Caller.Player))).EndSubCommand()
            .BeginSubCommand("test").RequiresPrivilege(Privilege.controlserver).WithDescription("Begin a Procession beside the caller for isolated-world testing.")
            .HandleWith(args => TextCommandResult.Success(BeginTest(args.Caller.Player))).EndSubCommand();
    }

    public override void StartClientSide(ICoreClientAPI api)
    {
        client = api;
        storms = api.ModLoader.GetModSystem<SystemTemporalStability>();
        storms.OnGetTemporalStability += ModifyStability;
    }

    private void Load()
    {
        if (server == null) return;
        try
        {
            var bytes = server.WorldManager.SaveGame.GetData("procession-encounter-v1");
            save = bytes == null ? new() : JsonSerializer.Deserialize<GameSave>(bytes) ?? new();
            director = new(save.Encounter, random: server.World.Rand.NextDouble);
        }
        catch (Exception ex)
        {
            // Never lock a storm with unreadable ownership. Preserve corrupt bytes for diagnosis.
            server.Logger.Error("[Procession] Encounter save could not be restored; storm lock disabled: {0}", ex);
            save = new(); director = new();
        }
        Sites!.Initialize();
        initialized = true;
        // Reconcile on the first tick, after all vanilla SaveGameLoaded handlers restore their rifts.
        portalRecoveryPending = true;
        foreach (var entity in server.World.LoadedEntities.Values.ToArray()) OnLoaded(entity);
    }

    private void Save()
    {
        if (server == null || !initialized) return;
        save.Encounter = director.State;
        server.WorldManager.SaveGame.StoreData("procession-encounter-v1", JsonSerializer.SerializeToUtf8Bytes(save));
        Sites!.Save();
    }

    private Vec3d Center => new(save.X, save.Y, save.Z);
    private bool IsOwned(Entity e) => save.EncounterId != null && e.WatchedAttributes.GetString(Ownership) == save.EncounterId;

    private void Tick(float dt)
    {
        if (!initialized || server == null || storms == null) return;
        loadedSeconds += dt;
        Sites!.Tick(dt);
        var data = storms.StormData;
        if (portalRecoveryPending)
        {
            portalRecoveryPending = false;
            if (director.State.Phase == EncounterPhase.Arriving && data.nowStormActive)
            {
                if (rifts != null && !rifts.ServerRifts.Any(r => r.RiftId == portalId)) AddPortal();
            }
            else RemovePortal();
            Save();
        }
        if (director.State.Phase == EncounterPhase.Arriving && !data.nowStormActive)
        {
            director.Abandon(); CleanupOwned(); Save();
        }
        if (director.LocksStorm && !data.nowStormActive)
        {
            director.Abandon(); CleanupOwned(); Save();
        }
        if (data.nowStormActive && data.nextStormStrength == EnumTempStormStrength.Heavy)
        {
            string key = data.nextStormTotalDays.ToString("R", CultureInfo.InvariantCulture);
            if (save.LastStorm != key && director.State.Phase is not (EncounterPhase.Arriving or EncounterPhase.Active))
            {
                save.LastStorm = key; // Commit before attempting placement: never duplicate a storm's event.
                if (!Sites.Suppressed && server.World.Rand.NextDouble() < config.EventChance) TryBegin();
                Save();
            }
        }
        if (director.LocksStorm)
        {
            // Vanilla tick still runs and retains spawning/bookkeeping; only its expiry is postponed.
            data.stormActiveTotalDays = Math.Max(data.stormActiveTotalDays, server.World.Calendar.TotalDays + .02);
        }
        var livingPositions = director.State.LivingChanters.Keys.Select(server.World.GetEntityById)
            .Where(e => e?.Alive == true).Select(e => e!.Pos.XYZ).ToArray();
        bool near = server.World.AllOnlinePlayers.Any(p => p.Entity.Alive && p.Entity.Pos.Dimension == 0
            && livingPositions.Any(position => p.Entity.Pos.XYZ.SquareDistanceTo(position) <= 30 * 30));
        var signal = director.Tick(dt, near);
        switch (signal)
        {
            case EncounterSignal.ArrivalReady: CompleteArrival(); break;
            case EncounterSignal.ReinforcementDue: SpawnReplacement(); break;
            case EncounterSignal.Abandoned: CleanupOwned(); EndStorm(); Save(); break;
        }
        soundTimer += dt;
        if (director.State.Phase is EncounterPhase.Arriving or EncounterPhase.Active && soundTimer >= 5)
        {
            soundTimer = 0;
            server.World.PlaySoundAt(new AssetLocation(director.State.Phase == EncounterPhase.Active && director.State.OpeningBellId.HasValue
                ? "game:sounds/creature/bell/alarm" : "game:sounds/creature/bell/listen"), save.X, save.Y, save.Z, null, false, 64, .6f);
        }
        foreach (var player in server.World.AllOnlinePlayers)
        {
            double multiplier = director.DrainMultiplier;
            if (player.Entity.WatchedAttributes.GetDouble("processionDrainMultiplier", 1) != multiplier)
                player.Entity.WatchedAttributes.SetDouble("processionDrainMultiplier", multiplier);
        }
        recoveryTimer += dt;
        if (recoveryTimer >= 5) { recoveryTimer = 0; RecoverMissing(); }
    }

    private void TryBegin()
    {
        if (server == null || riftPlacement == null || rifts == null || stormTick == null)
        {
            server?.Logger.Error("[Procession] Required 1.22.7 storm/rift hooks unavailable; encounter skipped safely."); return;
        }
        var player = server.World.AllOnlinePlayers.FirstOrDefault(p => p.Entity.Alive && p.Entity.Pos.Dimension == 0);
        if (player == null) return;
        var spot = FindGround(player.Entity.Pos.XYZ, 18, 36, true);
        if (spot == null) { server.Logger.Notification("[Procession] No accessible rift position found; this heavy storm remains ordinary."); return; }
        save.X = spot.X; save.Y = spot.Y; save.Z = spot.Z;
        save.EncounterId = Guid.NewGuid().ToString("N");
        save.EligibleChanterDeaths = 0;
        director.StartArrival(config.ArrivalMinimumSeconds + server.World.Rand.NextDouble() * (config.ArrivalMaximumSeconds - config.ArrivalMinimumSeconds));
        AddPortal();
        server.BroadcastMessageToAllGroups("A broken rescue call threads through the storm. Follow the answering bell.", EnumChatType.Notification);
    }

    private string BeginTest(IPlayer player)
    {
        if (server == null || storms == null) return "The world is not ready.";
        if (director.State.Phase is EncounterPhase.Active or EncounterPhase.Arriving) return "An encounter already exists.";
        var data = storms.StormData;
        data.nextStormStrength = EnumTempStormStrength.Heavy;
        data.nextStormTotalDays = server.World.Calendar.TotalDays - .001;
        data.nowStormActive = true;
        data.stormGlitchStrength = .95f;
        data.stormActiveTotalDays = server.World.Calendar.TotalDays + .2;
        TryBegin(); Save();
        return director.State.Phase == EncounterPhase.Arriving ? "Test Procession arriving. Use only in a disposable test world." : "No valid rift position; move to a dark open area.";
    }

    private Vec3d? FindGround(Vec3d origin, int minRange, int maxRange, bool requireRiftRules = false)
    {
        if (server == null) return null;
        for (int tries = 0; tries < 80; tries++)
        {
            double angle = server.World.Rand.NextDouble() * Math.PI * 2;
            int radius = server.World.Rand.Next(minRange, maxRange + 1);
            var pos = new BlockPos((int)origin.X + (int)(Math.Cos(angle) * radius), (int)origin.Y, (int)origin.Z + (int)(Math.Sin(angle) * radius));
            if (server.World.BlockAccessor.GetChunkAtBlockPos(pos) == null) continue;
            pos.Y = server.World.BlockAccessor.GetTerrainMapheightAt(pos) + 1;
            if (Math.Abs(pos.Y - origin.Y) > 12 || !ClearArea(pos, requireRiftRules ? 5 : 1, 4)) continue;
            if (requireRiftRules && riftPlacement!.Invoke(rifts, new object[] { pos }) is not true) continue;
            return new Vec3d(pos.X + .5, pos.Y, pos.Z + .5);
        }
        return null;
    }

    private bool ClearArea(BlockPos center, int radius, int height)
    {
        if (server == null) return false;
        var ba = server.World.BlockAccessor;
        for (int x = -radius; x <= radius; x++) for (int z = -radius; z <= radius; z++)
        {
            var floor = center.AddCopy(x, -1, z);
            if (ba.GetBlock(floor).Replaceable >= 6000 || ba.GetBlock(floor, BlockLayersAccess.Fluid).IsLiquid()) return false;
            for (int y = 0; y < height; y++)
            {
                var p = center.AddCopy(x, y, z);
                if (ba.GetChunkAtBlockPos(p) == null || ba.GetBlock(p).Replaceable < 6000 || ba.GetBlock(p, BlockLayersAccess.Fluid).IsLiquid()) return false;
            }
        }
        return true;
    }

    private Entity? Spawn(string code, Vec3d pos, bool defender)
    {
        if (server == null) return null;
        var type = server.World.GetEntityType(new AssetLocation(code));
        if (type == null) { server.Logger.Warning("[Procession] Entity unavailable: {0}", code); return null; }
        var entity = server.ClassRegistry.CreateEntity(type);
        if (entity == null) return null;
        entity.Pos.SetPos(pos);
        entity.WatchedAttributes.SetString(Ownership, save.EncounterId);
        entity.WatchedAttributes.SetBool("processionGuard", defender);
        entity.Attributes.SetBool("ignoreDaylightFlee", true);
        server.World.SpawnEntity(entity);
        if (defender) ProtectGuard(entity);
        return entity;
    }

    private void ProtectGuard(Entity entity)
    {
        if (server == null || !entity.WatchedAttributes.GetBool("processionGuard")) return;
        // EntityProperties.Clone intentionally clears runtime behavior instances. Restore this entity's
        // existing instance list on its isolated cloned server properties before replacing Properties.
        var liveBehaviors = entity.SidedProperties.Behaviors.ToArray();
        var properties = entity.Properties.Clone();
        properties.Drops = Array.Empty<BlockDropItemStack>();
        properties.Attributes = Vintagestory.API.Datastructures.JsonObject.FromJson(properties.Attributes?.Token?.ToString() ?? "{}");
        properties.Attributes.Token!["onDeathStabilityRecovery"] = 0;
        properties.Attributes.Token!["catchable"] = false;
        properties.Attributes.Token!["hackedEntity"] = null;
        properties.Attributes.Token!["hackedEntityByType"] = null;
        // Disable corpse harvesting and capture on this entity only, including after reload.
        static bool Restricted(EntityBehavior behavior) => behavior.PropertyName().Contains("harvest", StringComparison.OrdinalIgnoreCase)
            || behavior.PropertyName().Contains("catch", StringComparison.OrdinalIgnoreCase)
            || behavior.PropertyName().Contains("trap", StringComparison.OrdinalIgnoreCase);
        properties.Server.Behaviors.AddRange(liveBehaviors.Where(b => !Restricted(b)));
        entityProperties?.SetValue(entity, properties);
        entity.ServerBehaviorsMainThread = entity.ServerBehaviorsMainThread.Where(b => !Restricted(b)).ToArray();
        entity.ServerBehaviorsThreadsafe = entity.ServerBehaviorsThreadsafe.Where(b => !Restricted(b)).ToArray();
    }

    private void CompleteArrival()
    {
        var created = new Dictionary<long, ChanterKind>();
        foreach (var kind in Enum.GetValues<ChanterKind>())
        {
            int index = (int)kind;
            var pos = Center.AddCopy(index % 2 == 0 ? -3 : 3, 0, index < 2 ? -3 : 3);
            var e = Spawn("procession:chanter-" + kind.ToString().ToLowerInvariant(), pos, false);
            if (e == null) { director.Abandon(); CleanupOwned(); RemovePortal(); return; }
            created[e.EntityId] = kind;
        }
        var bell = Spawn("procession:openingbell", Center, true);
        director.Activate(created, bell?.EntityId);
        foreach (var family in new[] { "shiver", "bowtorn", "locust", "drifter" }) SpawnFamily(family);
        RemovePortal(); Save();
    }

    private void SpawnReplacement()
    {
        if (server == null) return;
        var families = new List<string> { "drifter" };
        if (director.FamilyAllowed(ChanterKind.Bound)) families.Add("shiver");
        if (director.FamilyAllowed(ChanterKind.Veiled)) families.Add("bowtorn");
        if (director.FamilyAllowed(ChanterKind.Resonant)) families.Add("locust");
        // The one-time bell has its own drifter source, but uses this very same cap and clock.
        if (director.State.OpeningBellId.HasValue) families.Add("drifter");
        SpawnFamily(families[server.World.Rand.Next(families.Count)]);
        director.RetryReinforcementLater();
    }

    private void SpawnFamily(string family)
    {
        if (server == null || director.State.Defenders.Count >= 8) return;
        var variants = server.World.EntityTypes.Where(t => t.Code.Domain == "game" && t.Code.Path.StartsWith(family + "-", StringComparison.Ordinal)
            && !t.Code.Path.Contains("hacked", StringComparison.OrdinalIgnoreCase)).ToArray();
        if (variants.Length == 0) return;
        var spot = FindGround(Center, 5, 12);
        if (spot == null) return;
        var e = Spawn(variants[server.World.Rand.Next(variants.Length)].Code.ToString(), spot, true);
        if (e != null && !director.RegisterDefender(e.EntityId, e.GetBehavior<EntityBehaviorHealth>()?.MaxHealth ?? 12))
            server.World.DespawnEntity(e, new() { Reason = EnumDespawnReason.Removed });
    }

    private void OnDeath(Entity entity, DamageSource source)
    {
        if (!initialized || !IsOwned(entity)) return;
        if (director.State.LivingChanters.TryGetValue(entity.EntityId, out var kind))
        {
            bool victory = director.ChanterDied(entity.EntityId);
            if (source?.GetCauseEntity() is EntityPlayer) save.EligibleChanterDeaths++;
            ProcessionRewards.Chanter(server!, entity, kind, victory && save.EligibleChanterDeaths == 4, source!);
            foreach (long id in director.State.LivingChanters.Keys)
            {
                var survivor = server!.World.GetEntityById(id);
                if (survivor?.Alive == true) Heal(survivor);
                else save.HealOnLoad.Add(id);
            }
            if (victory) { EndStorm(); server!.BroadcastMessageToAllGroups("The last rescue call falls silent. The storm releases its hold.", EnumChatType.Notification); }
            Save();
        }
        else
        {
            bool bell = director.State.OpeningBellId == entity.EntityId;
            if (director.DefenderDied(entity.EntityId))
            {
                if (bell) ProcessionRewards.OpeningBell(server!, entity, source);
                Save();
            }
        }
    }

    private static void Heal(Entity entity)
    {
        var health = entity.GetBehavior<EntityBehaviorHealth>();
        if (health == null || !entity.Alive) return;
        health.Health = health.MaxHealth; health.MarkDirty();
    }

    private void OnLoaded(Entity entity)
    {
        if (!initialized) return;
        if (!string.IsNullOrEmpty(entity.WatchedAttributes.GetString(Ownership))) ProtectGuard(entity);
        if (save.AbandonedEvents.Contains(entity.WatchedAttributes.GetString(Ownership)))
        {
            server!.World.DespawnEntity(entity, new() { Reason = EnumDespawnReason.Removed }); return;
        }
        if (!IsOwned(entity)) return;
        if (director.State.Phase == EncounterPhase.Abandoned)
        {
            server!.World.DespawnEntity(entity, new() { Reason = EnumDespawnReason.Removed }); return;
        }
        if (save.HealOnLoad.Remove(entity.EntityId)) Heal(entity);
    }

    private void OnDespawn(Entity entity, EntityDespawnData reason)
    {
        if (cleaning || !initialized || !IsOwned(entity) || reason.Reason is EnumDespawnReason.Unload or EnumDespawnReason.Disconnect or EnumDespawnReason.Death) return;
        if (director.State.LivingChanters.ContainsKey(entity.EntityId))
        {
            director.Abandon(); CleanupOwned(); EndStorm(); Save();
        }
        else director.DefenderDied(entity.EntityId);
    }

    private void RecoverMissing()
    {
        if (server == null || !director.LocksStorm || loadedSeconds < 45) return;
        // Missing unloaded IDs remain counted. A loaded arena with missing chanters abandons safely.
        if (server.World.BlockAccessor.GetChunkAtBlockPos(Center.AsBlockPos) == null) return;
        bool arenaNear = server.World.AllOnlinePlayers.Any(p => p.Entity.Pos.XYZ.SquareDistanceTo(Center) < 20 * 20);
        if (!arenaNear) return;
        if (director.State.LivingChanters.Keys.Any(id => server.World.GetEntityById(id) == null))
        {
            // A missing entity may have been pushed into an unloaded chunk; retain ownership and let
            // the real-time absence clock release the lock rather than inventing a death/reward.
            return;
        }
    }

    private void CleanupOwned()
    {
        if (server == null || cleaning) return;
        cleaning = true;
        try
        {
            if (save.EncounterId != null) save.AbandonedEvents.Add(save.EncounterId);
            foreach (var e in server.World.LoadedEntities.Values.Where(IsOwned).ToArray())
                server.World.DespawnEntity(e, new() { Reason = EnumDespawnReason.Removed });
            RemovePortal();
        }
        finally { cleaning = false; }
    }

    private void EndStorm()
    {
        RemovePortal();
        if (server == null || storms == null || !storms.StormData.nowStormActive) return;
        storms.StormData.stormActiveTotalDays = server.World.Calendar.TotalDays - .0001;
        stormTick?.Invoke(storms, new object[] { .01f });
    }

    private void AddPortal()
    {
        if (server == null || rifts == null) return;
        int id = rifts.NextRiftId;
        rifts.ServerRifts.Add(new Rift { RiftId = id, Position = Center, Size = 5, SpawnedTotalHours = server.World.Calendar.TotalHours,
            DieAtTotalHours = server.World.Calendar.TotalHours + 1 });
        portalId = id; broadcastRifts?.Invoke(rifts, new object?[] { null });
    }

    private void RemovePortal()
    {
        if (rifts == null || portalId == null) return;
        rifts.ServerRifts.RemoveAll(r => r.RiftId == portalId); portalId = null;
        broadcastRifts?.Invoke(rifts, new object?[] { null });
    }

    private float ModifyStability(float current, double x, double y, double z)
    {
        if (baselineQuery || storms == null || !storms.StormData.nowStormActive) return current;
        double multiplier = server != null ? director.DrainMultiplier : client?.World.Player?.Entity.WatchedAttributes.GetDouble("processionDrainMultiplier", 1) ?? 1;
        if (multiplier <= 1) return current;
        // Isolate the vanilla storm contribution while preserving local geography and other modifiers.
        bool active = storms.StormData.nowStormActive;
        try
        {
            baselineQuery = true; storms.StormData.nowStormActive = false;
            float baseline = storms.GetTemporalStability(x, y, z);
            return (float)StabilityMath.ApplyStormDrain(current, baseline, multiplier);
        }
        finally { storms.StormData.nowStormActive = active; baselineQuery = false; }
    }

    public string Describe(IPlayer player) => $"Procession: {director.State.Phase}; {director.State.LivingChanters.Count}/4 chanters, {director.State.Defenders.Count}/8 defenders. "
        + (Sites?.Suppressed == true ? "Future Processions suppressed. " : "Future Processions allowed. ") + Sites?.DescribeRoute(player);

    private TextCommandResult Audit()
    {
        if (server == null) return TextCommandResult.Error("Server unavailable.");
        var missing = new List<string>();
        foreach (string code in new[] { "chanter-bound", "chanter-veiled", "chanter-resonant", "chanter-hollow", "openingbell", "lastcustodian" })
            if (server.World.GetEntityType(new AssetLocation("procession", code)) == null) missing.Add(code);
        foreach (string code in new[] { "routechart", "relayfragment" })
            if (server.World.GetItem(new AssetLocation("procession", code)) == null) missing.Add(code);
        if (server.World.GetBlock(new AssetLocation("procession:closingbell"))?.Id is not > 0) missing.Add("closingbell");
        if (stormTick == null || riftPlacement == null || broadcastRifts == null || entityProperties?.GetSetMethod(true) == null) missing.Add("required 1.22.7 hooks");
        return missing.Count > 0 ? TextCommandResult.Error("Procession audit failed: " + string.Join(", ", missing))
            : TextCommandResult.Success($"Procession audit passed: 6 entity types, 2 items, Closing Bell and 4 installed-runtime hooks present. World loaded: {initialized}. This validates registration, not combat or rendering.");
    }

    public override void Dispose()
    {
        if (storms != null) storms.OnGetTemporalStability -= ModifyStability;
        if (server != null)
        {
            server.Event.UnregisterGameTickListener(tickId);
            server.Event.SaveGameLoaded -= Load; server.Event.GameWorldSave -= Save;
            server.Event.OnEntityDeath -= OnDeath; server.Event.OnEntityLoaded -= OnLoaded; server.Event.OnEntityDespawn -= OnDespawn;
        }
        Sites?.Dispose();
        base.Dispose();
    }
}
