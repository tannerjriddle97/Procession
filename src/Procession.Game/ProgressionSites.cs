using System.Text;
using Newtonsoft.Json;
using Procession.Core;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

namespace Procession.Game;

/// <summary>Server-owned, finite expedition landmarks and the earned world-wide controller.</summary>
public sealed class ProgressionSites : IDisposable
{
    private const string SaveKey = "procession-progression-sites-v1";
    private readonly ICoreServerAPI api;
    private SiteSave state = new();
    private WorldProgression worldProgression = new();
    private readonly HashSet<string> announced = new();
    private float elapsed;
    private bool initialized;
    private float missingBossSeconds;

    public bool ControlEarned => worldProgression.State.CustodianDefeated;
    public bool Suppressed => worldProgression.State.Suppressed;

    public ProgressionSites(ICoreServerAPI api) => this.api = api;

    public void Initialize()
    {
        var bytes = api.WorldManager.SaveGame.GetData(SaveKey);
        if (bytes is not null)
        {
            state = JsonConvert.DeserializeObject<SiteSave>(Encoding.UTF8.GetString(bytes))
                ?? throw new InvalidOperationException("Procession progression save is empty.");
            if (state.SchemaVersion != 1) throw new InvalidOperationException("Unsupported Procession site save schema.");
        }
        worldProgression = new WorldProgression(state.Control);
        initialized = true;
        Save();
    }

    private bool PlanSites()
    {
        // Fresh worlds announce SaveGameLoaded before their spawn is chosen. Do not substitute (0,0)
        // or let that ordering exception abort the mod; the normal server tick retries after WorldReady.
        EntityPos? spawn;
        try { spawn = api.World.DefaultSpawnPosition; }
        catch (NullReferenceException) { return false; }
        if (spawn is null) return false;
        state.OriginX = (int)spawn.X;
        state.OriginZ = (int)spawn.Z;
        var random = new Random(api.World.Seed);
        var bearing = random.NextDouble() * Math.PI * 2;
        AddSite("relief-shelter", "Relief Shelter", random.Next(400, 651), bearing, 4);
        AddSite("lower-relay", "Lower Relay", random.Next(1600, 2401), bearing + .16, 6);
        AddSite("far-relay", "Far Relay", random.Next(6000, 8001), bearing + .24, 9);
        Save();
        return true;
    }

    private void AddSite(string id, string name, int distance, double bearing, int radius)
    {
        // Small test maps keep landmarks in bounds; the route reports their actual saved distances.
        int x = Math.Clamp(state.OriginX + (int)(Math.Cos(bearing) * distance), 32, Math.Max(32, api.World.BlockAccessor.MapSizeX - 33));
        int z = Math.Clamp(state.OriginZ + (int)(Math.Sin(bearing) * distance), 32, Math.Max(32, api.World.BlockAccessor.MapSizeZ - 33));
        state.Sites.Add(new SiteState { Id = id, Name = name, TargetX = x, TargetZ = z, X = x, Z = z, Radius = radius });
    }

    public void Tick(float dt)
    {
        if (!initialized) return;
        if (state.Sites.Count == 0 && !PlanSites()) return;
        elapsed += dt;
        if (elapsed < 2) return;
        elapsed = 0;
        bool farRelayVisited = false;
        foreach (var player in api.World.AllOnlinePlayers)
        {
            if (player.Entity is null || !player.Entity.Alive || player.Entity.Pos.Dimension != 0) continue;
            foreach (var site in state.Sites)
            {
                if (HorizontalDistance(player, site.X, site.Z) > 100) continue;
                if (!site.Generated)
                {
                    TryBuild(site);
                    if (!site.Generated && announced.Add(player.PlayerUID + ":pending:" + site.Id))
                        Tell(player, $"{site.Name}: the survey is waiting for safe, open terrain near this marker. No blocks were changed. Find an open patch within 512 blocks and use /procession survey to validate it; the route chart will retain this same destination.");
                }
                if (!site.Generated || HorizontalDistance(player, site.X, site.Z) > site.Radius + 14) continue;
                var key = player.PlayerUID + ":" + site.Id;
                if (announced.Add(key)) Tell(player, DescribeSite(site));
                if (site.Id == "relief-shelter" && !site.ChartIssued) IssueChart(site, player);
                if (site.Id == "far-relay")
                {
                    farRelayVisited = true;
                    if (ControlEarned && !state.ControlRewardIssued) GiveControl(player, false);
                }
            }
        }
        if (farRelayVisited && !ControlEarned) MaintainBoss(state.Sites.First(s => s.Id == "far-relay"));
        if (ControlEarned && !string.IsNullOrEmpty(state.BossIdentity))
            foreach (var stale in api.World.LoadedEntities.Values.Where(e => e.Alive &&
                         e.Code?.ToString() == "procession:lastcustodian" &&
                         e.WatchedAttributes.GetString("processionBossIdentity") == state.BossIdentity).ToArray())
                api.World.DespawnEntity(stale, new EntityDespawnData { Reason = EnumDespawnReason.Removed });
    }

    private void IssueChart(SiteState site, IPlayer player)
    {
        var item = api.World.GetItem(new AssetLocation("procession:routechart"));
        if (item is null) return;
        // The saved finite claim precedes exposure. A chart is also craftable if this item is lost.
        site.ChartIssued = true;
        Save();
        Deliver(player, new ItemStack(item));
    }

    private bool TryBuild(SiteState site)
    {
        // Only generate close to an actual visitor, on loaded terrain, and without modifying existing blocks.
        // Search offsets remain near the chart's original survey; unvalidated destinations stay labelled pending.
        var offsets = Enumerable.Range(-3, 7).SelectMany(dx => Enumerable.Range(-3, 7).Select(dz => (X: dx * 12, Z: dz * 12)))
            .OrderBy(p => p.X * p.X + p.Z * p.Z);
        foreach (var offset in offsets)
        {
            int x = site.TargetX + offset.X, z = site.TargetZ + offset.Z;
            if (!TryValidateFootprint(x, z, site.Radius, out int y)) continue;
            var writes = PlanRuin(site.Id, x, y, z, site.Radius);
            // Validate the exact write set again before the first mutation.
            // Leave low vegetation exactly as it is; gaps in a broken wall are preferable to modifying it.
            writes.RemoveAll(w => IsLowVegetation(api.World.BlockAccessor.GetBlock(w.Position)));
            if (writes.Any(w => api.World.BlockAccessor.GetBlock(w.Position).Id != 0)) continue;
            foreach (var write in writes) api.World.BlockAccessor.SetBlock(write.BlockId, write.Position);
            site.X = x; site.Y = y; site.Z = z; site.Generated = true;
            Save();
            api.Logger.Notification("[Procession] Generated {0} at {1}, {2}, {3}.", site.Name, x, y, z);
            return true;
        }
        return false;
    }

    private bool TryValidateFootprint(int x, int z, int radius, out int y)
    {
        y = 0;
        var blocks = api.World.BlockAccessor;
        if (x - radius < 1 || z - radius < 1 || x + radius >= blocks.MapSizeX - 1 || z + radius >= blocks.MapSizeZ - 1) return false;
        int minHeight = int.MaxValue, maxHeight = 0;
        for (int dx = -radius; dx <= radius; dx++)
        for (int dz = -radius; dz <= radius; dz++)
        {
            var column = new BlockPos(x + dx, 0, z + dz);
            if (blocks.GetMapChunkAtBlockPos(column) is null) return false;
            int height = blocks.GetTerrainMapheightAt(column);
            if (height < 1 || height + 9 >= blocks.MapSizeY) return false;
            var ground = new BlockPos(column.X, height, column.Z);
            if (blocks.GetChunkAtBlockPos(ground) is null) return false;
            var baseBlock = blocks.GetBlock(ground);
            string code = baseBlock.Code?.Path ?? "";
            if (baseBlock.Code?.Domain != "game" ||
                !(code.StartsWith("soil-") || code.StartsWith("forestfloor-") || code.StartsWith("sand-") ||
                  code.StartsWith("gravel-") || code.StartsWith("rock-") || code.StartsWith("snowblock"))) return false;
            minHeight = Math.Min(minHeight, height); maxHeight = Math.Max(maxHeight, height);
            if (maxHeight - minHeight > 2) return false;
            for (int dy = 1; dy <= 9; dy++)
            {
                var pos = new BlockPos(column.X, height + dy, column.Z);
                var existing = blocks.GetBlock(pos);
                if (blocks.GetChunkAtBlockPos(pos) is null || existing.Id != 0 && !IsLowVegetation(existing) ||
                    blocks.GetBlock(pos, BlockLayersAccess.Fluid).Id != 0 || api.World.Claims.Get(pos).Any()) return false;
            }
        }
        y = maxHeight + 1;
        return true;
    }

    private static bool IsLowVegetation(Block block)
    {
        string code = block.Code?.Path ?? "";
        return block.Code?.Domain == "game" && block.Replaceable >= 6000 &&
            (code.StartsWith("tallgrass-") || code.StartsWith("flower-") || code.StartsWith("fern-") ||
             code.StartsWith("mushroom-") || code.StartsWith("snowlayer-"));
    }

    private List<BlockWrite> PlanRuin(string id, int x, int y, int z, int radius)
    {
        var stone = api.World.GetBlock(new AssetLocation("game:cobblestone-granite"));
        var wood = api.World.GetBlock(new AssetLocation("game:planks-oak"));
        if (stone is null || wood is null) throw new InvalidOperationException("Procession ruin palette is unavailable.");
        var result = new Dictionary<(int, int, int), BlockWrite>();
        void Put(int dx, int dy, int dz, Block block)
        {
            var pos = new BlockPos(x + dx, y + dy, z + dz);
            result[(pos.X, pos.Y, pos.Z)] = new BlockWrite(pos, block.Id);
        }
        // Ruined perimeter leaves a three-block approach through every side. The natural floor is retained.
        for (int a = -radius; a <= radius; a++)
        {
            if (Math.Abs(a) < 2) continue;
            Put(a, 0, -radius, stone); Put(a, 0, radius, stone);
            Put(-radius, 0, a, stone); Put(radius, 0, a, stone);
            if ((a + radius) % 3 == 0)
            {
                Put(a, 1, -radius, stone); Put(-radius, 1, a, stone);
            }
        }
        foreach (int dx in new[] { -radius, radius })
        foreach (int dz in new[] { -radius, radius })
            for (int h = 1; h <= (id == "far-relay" ? 5 : 3); h++) Put(dx, h, dz, wood);
        if (id == "relief-shelter")
        {
            for (int dx = -radius; dx <= 0; dx++)
            for (int dz = -radius; dz <= radius; dz++) Put(dx, 4, dz, wood);
            Put(-2, 0, 0, wood); Put(-2, 1, 0, wood);
        }
        else
        {
            // Four inert stations are scenery, never additional chanters or storm anchors.
            foreach (int dx in new[] { -3, 3 })
            foreach (int dz in new[] { -3, 3 })
            {
                Put(dx, 0, dz, stone); Put(dx, 1, dz, wood);
            }
            if (id == "far-relay")
                for (int dx = -radius; dx <= -radius + 5; dx++) Put(dx, 5, -radius, wood);
        }
        return result.Values.ToList();
    }

    private void SpawnBoss(SiteState site, bool recover = false)
    {
        var type = api.World.GetEntityType(new AssetLocation("procession:lastcustodian"));
        if (type is null) { api.Logger.Error("[Procession] Last Custodian entity asset is missing."); return; }
        var boss = api.ClassRegistry.CreateEntity(type);
        int height = api.World.BlockAccessor.GetTerrainMapheightAt(new BlockPos(site.X, 0, site.Z)) + 1;
        if (string.IsNullOrEmpty(state.BossIdentity))
        {
            state.BossIdentity = Guid.NewGuid().ToString("N");
            Save();
        }
        boss.Pos.SetPos(site.X + .5, height, site.Z + .5);
        boss.WatchedAttributes.SetBool("processionFinalBoss", true);
        boss.WatchedAttributes.SetString("processionBossIdentity", state.BossIdentity);
        boss.WatchedAttributes.SetDouble("processionHomeX", boss.Pos.X);
        boss.WatchedAttributes.SetDouble("processionHomeY", boss.Pos.Y);
        boss.WatchedAttributes.SetDouble("processionHomeZ", boss.Pos.Z);
        api.World.SpawnEntity(boss);
        state.BossEntityId = boss.EntityId;
        if (recover && state.BossHealth > 0)
        {
            var health = boss.GetBehavior<EntityBehaviorHealth>();
            if (health is not null) { health.Health = Math.Min(health.MaxHealth, state.BossHealth); health.MarkDirty(); }
        }
        state.BossLastX = boss.Pos.X; state.BossLastY = boss.Pos.Y; state.BossLastZ = boss.Pos.Z;
        missingBossSeconds = 0;
        Save();
    }

    private void MaintainBoss(SiteState site)
    {
        if (state.BossEntityId == 0 && string.IsNullOrEmpty(state.BossIdentity)) { SpawnBoss(site); return; }
        var committed = api.World.GetEntityById(state.BossEntityId);
        if (committed?.Code?.ToString() == "procession:lastcustodian" &&
            string.IsNullOrEmpty(committed.WatchedAttributes.GetString("processionBossIdentity")))
        {
            if (string.IsNullOrEmpty(state.BossIdentity)) state.BossIdentity = Guid.NewGuid().ToString("N");
            committed.WatchedAttributes.SetString("processionBossIdentity", state.BossIdentity);
        }
        var matching = api.World.LoadedEntities.Values.Where(e => e.Code?.ToString() == "procession:lastcustodian" &&
            e.WatchedAttributes.GetString("processionBossIdentity") == state.BossIdentity).ToArray();
        var boss = matching.FirstOrDefault(e => e.EntityId == state.BossEntityId) ?? matching.FirstOrDefault();
        if (boss is not null)
        {
            state.BossEntityId = boss.EntityId;
            // A crash between entity/world saves can leave an older duplicate; keep one saved identity.
            foreach (var duplicate in matching.Where(e => e.EntityId != boss.EntityId))
                api.World.DespawnEntity(duplicate, new EntityDespawnData { Reason = EnumDespawnReason.Removed });
            if (!boss.Alive) { BossDefeated(boss); return; }
            missingBossSeconds = 0;
            state.BossLastX = boss.Pos.X; state.BossLastY = boss.Pos.Y; state.BossLastZ = boss.Pos.Z;
            state.BossHealth = boss.GetBehavior<EntityBehaviorHealth>()?.Health ?? state.BossHealth;
            if (SquaredDistance(boss.Pos.X, boss.Pos.Z, site.X, site.Z) > 48 * 48)
                boss.TeleportTo(new Vec3d(site.X + .5, boss.WatchedAttributes.GetDouble("processionHomeY", site.Y), site.Z + .5));
            Save();
            return;
        }
        // Null alone is never a death. The boss is confined by its own behavior to a 48-block home leash.
        // A full larger cube must be loaded continuously, not merely the anchor column, before recovery.
        if (!BossLeashVolumeLoaded(site)) { missingBossSeconds = 0; return; }
        missingBossSeconds += 2;
        if (missingBossSeconds < 30) return;
        SpawnBoss(site, true);
        api.Logger.Warning("[Procession] Recovered missing Last Custodian at saved health after its full leash volume loaded for 30 seconds.");
    }

    private bool BossLeashVolumeLoaded(SiteState site)
    {
        int homeY = api.World.BlockAccessor.GetTerrainMapheightAt(new BlockPos(site.X, 0, site.Z)) + 1;
        for (int x = site.X - 64; x <= site.X + 64; x += 16)
        for (int z = site.Z - 64; z <= site.Z + 64; z += 16)
        for (int y = Math.Max(1, homeY - 64); y <= Math.Min(api.World.BlockAccessor.MapSizeY - 1, homeY + 64); y += 16)
            if (api.World.BlockAccessor.GetChunkAtBlockPos(new BlockPos(x, y, z)) is null) return false;
        return true;
    }

    public void OnBossDeath(Entity boss) => BossDefeated(boss);

    public void BossDefeated(Entity boss)
    {
        if (!initialized || ControlEarned || boss.EntityId != state.BossEntityId ||
            boss.Code?.ToString() != "procession:lastcustodian") return;
        worldProgression.EarnControl();
        Save();
        foreach (var player in api.World.AllOnlinePlayers)
        {
            Tell(player, "The Last Custodian has fallen. World-wide Procession control is earned. Return to the Far Relay to collect the Closing Bell.");
            var site = state.Sites.First(s => s.Id == "far-relay");
            if (!state.ControlRewardIssued && HorizontalDistance(player, site.X, site.Z) < 40) GiveControl(player, false);
        }
    }

    private bool GiveControl(IPlayer player, bool replace)
    {
        var block = api.World.GetBlock(new AssetLocation("procession:closingbell"));
        if (block is null || block.Id == 0) { Tell(player, "The Closing Bell asset is unavailable; your earned unlock is saved."); return false; }
        if (replace) worldProgression.RecoverControl();
        var stack = new ItemStack(block);
        stack.Attributes.SetInt("processionControllerGeneration", worldProgression.State.ControllerGeneration);
        state.ControlRewardIssued = true;
        Save();
        Deliver(player, stack);
        Tell(player, "Closing Bell recovered. Place it at home, then use it to choose future Processions. Ordinary temporal storms continue.");
        return true;
    }

    /// <summary>Explicit chart interaction at the cleared site; never requires another boss kill.</summary>
    public bool RecoverControl(IPlayer player)
    {
        if (!ControlEarned) { Tell(player, "Defeat the Last Custodian before recovering the isolation instrument."); return false; }
        var site = state.Sites.First(s => s.Id == "far-relay");
        if (HorizontalDistance(player, site.X, site.Z) > site.Radius + 14)
        { Tell(player, "Return to the cleared Far Relay and use the route chart to recover a lost Closing Bell."); return false; }
        if (worldProgression.State.InstalledController is not null &&
            worldProgression.State.ControllerOwner != player.PlayerUID && !IsAdmin(player))
        { Tell(player, "The installed Closing Bell belongs to another keeper. Its owner or a server administrator must authorize replacement."); return false; }
        if (player.Entity.Pos.Dimension != 0) return false;
        return GiveControl(player, state.ControlRewardIssued);
    }

    public bool InstallController(BlockPos pos, IPlayer player) => InstallController(pos, player, null);

    public bool InstallController(BlockPos pos, IPlayer player, ItemStack? stack)
    {
        if (!ControlEarned) { Tell(player, "The Last Custodian still guards the isolation instrument."); return false; }
        int generation = stack?.Attributes.GetInt("processionControllerGeneration", 0) ?? 0;
        bool installed = worldProgression.Install(generation, PositionKey(pos), player.PlayerUID);
        if (installed)
        {
            state.PlacementGenerations[PositionKey(pos)] = generation;
            Save(); Tell(player, "Closing Bell installed: Procession allowed world-wide. Changes apply to future heavy storms.");
        }
        else
        {
            // An inactive copy placed on a retired coordinate must not inherit that coordinate's old drop token.
            if (worldProgression.State.InstalledController != PositionKey(pos))
            { state.PlacementGenerations[PositionKey(pos)] = 0; Save(); }
            Tell(player, "Another Closing Bell is installed, or this recovered copy is obsolete. Remove the active Bell or recover a replacement at the Far Relay.");
        }
        return installed;
    }

    public int ControllerGenerationAt(BlockPos pos) => state.PlacementGenerations.GetValueOrDefault(PositionKey(pos));

    /// <summary>Survival-accessible safe replan of a pending local survey; never moves an existing ruin.</summary>
    public string SurveyNearby(IPlayer player)
    {
        if (!initialized || player.Entity.Pos.Dimension != 0) return "The relief survey is unavailable here.";
        var site = state.Sites.Where(s => !s.Generated && HorizontalDistance(player, s.TargetX, s.TargetZ) <= 512)
            .OrderBy(s => HorizontalDistance(player, s.TargetX, s.TargetZ)).FirstOrDefault();
        if (site is null) return "No pending relief site lies within 512 blocks. Read the route chart or /procession guide for the saved destinations.";
        int x = (int)player.Entity.Pos.X, z = (int)player.Entity.Pos.Z;
        if (!TryValidateFootprint(x, z, site.Radius, out _))
            return $"{site.Name}: this patch is not safe for a ruin. Find open, nearly level natural ground away from trees, water and claims. Nothing was changed.";
        int oldX = site.TargetX, oldZ = site.TargetZ;
        site.TargetX = x; site.TargetZ = z;
        if (TryBuild(site)) return $"{site.Name}: the local survey is complete. " + StaticRoute(site.Id);
        site.TargetX = oldX; site.TargetZ = oldZ;
        return $"{site.Name}: no safe write set was found. The original survey remains pending; nothing was changed.";
    }

    public bool TrySetSuppressed(bool value, IPlayer player, BlockPos pos)
    {
        if (!ControlEarned || !worldProgression.SetSuppressed(PositionKey(pos), player.PlayerUID, IsAdmin(player), value))
        { Tell(player, "Only the installed Closing Bell's owner or a server administrator may change this world setting."); return false; }
        Save();
        Tell(player, value
            ? "Procession suppressed world-wide. Ordinary temporal storms continue. Applies to the next heavy storm."
            : "Procession allowed world-wide. Ordinary temporal storms continue. Applies to the next heavy storm.");
        return true;
    }

    public bool TrySetSuppressed(bool value, IPlayer player)
    {
        string? position = worldProgression.State.InstalledController;
        if (position is null) { Tell(player, "Place the earned Closing Bell before changing Procession availability."); return false; }
        if (!worldProgression.SetSuppressed(position, player.PlayerUID, IsAdmin(player), value)) return false;
        Save(); return true;
    }

    public bool RemoveController(BlockPos pos, IPlayer player)
    {
        if (worldProgression.State.InstalledController != PositionKey(pos)) return true;
        if (!worldProgression.Remove(PositionKey(pos), player.PlayerUID, IsAdmin(player)))
        { Tell(player, "Only this Closing Bell's owner or a server administrator may remove it."); return false; }
        Save(); return true;
    }

    public void ControllerRemoved(BlockPos pos)
    {
        if (worldProgression.State.InstalledController != PositionKey(pos)) return;
        worldProgression.Remove(PositionKey(pos), "system", true);
        Save();
    }

    public string DescribeRoute(IPlayer player)
    {
        if (!initialized || state.Sites.Count == 0) return "The relief survey is waiting for the world spawn to be established.";
        var descriptions = state.Sites.Select(s => RouteTo(player, s));
        var result = "Relief survey (coordinates relative to the original world spawn): " + string.Join(" | ", descriptions);
        if (ControlEarned) result += " | Custodian defeated. Use this chart at the cleared Far Relay to recover a lost Closing Bell; a replacement revokes the old installation.";
        result += " | No relic, manuscript or key is required to enter either relay.";
        return result;
    }

    private string DescribeSite(SiteState site) => site.Id switch
    {
        "relief-shelter" => "Relief Shelter: the old survey directs travelers onward. " + StaticRoute("lower-relay"),
        "lower-relay" => "Lower Relay: four deserted stations face the return gallery. The route board points to the isolation station. " + StaticRoute("far-relay"),
        _ => ControlEarned
            ? "Far Relay: the custodian is silent. Use a route chart here to recover a lost Closing Bell."
            : "Far Relay: the Last Custodian guards the isolation instrument. Prepare outside the court before approaching. No key is required."
    };

    private string StaticRoute(string id)
    {
        var site = state.Sites.First(s => s.Id == id);
        return $"{site.Name}: X {site.X - state.OriginX}, Z {site.Z - state.OriginZ}, relative to world spawn.";
    }

    private string RouteTo(IPlayer player, SiteState site)
    {
        double dx = site.X - player.Entity.Pos.X, dz = site.Z - player.Entity.Pos.Z;
        var bearings = new[] { "east", "southeast", "south", "southwest", "west", "northwest", "north", "northeast" };
        int octant = (int)Math.Floor((Math.Atan2(dz, dx) + Math.PI * 2 + Math.PI / 8) / (Math.PI / 4)) % 8;
        return $"{site.Name}: approximately {Math.Round(Math.Sqrt(dx * dx + dz * dz))} blocks {bearings[octant]}, X {site.X - state.OriginX}, Z {site.Z - state.OriginZ}{(site.Generated ? "" : " (terrain survey pending within 50 blocks)")}";
    }

    private void Deliver(IPlayer player, ItemStack stack)
    {
        if (!player.InventoryManager.TryGiveItemstack(stack, true)) api.World.SpawnItemEntity(stack, player.Entity.Pos.XYZ);
    }

    private void Tell(IPlayer player, string message) => api.SendMessage(player, GlobalConstants.GeneralChatGroup, message, EnumChatType.Notification);
    private static bool IsAdmin(IPlayer player) => player.HasPrivilege("controlserver");
    private static string PositionKey(BlockPos pos) => $"{pos.dimension}:{pos.X}:{pos.Y}:{pos.Z}";
    private static double HorizontalDistance(IPlayer player, int x, int z) => Math.Sqrt(SquaredDistance(player.Entity.Pos.X, player.Entity.Pos.Z, x, z));
    private static double SquaredDistance(double x, double z, double tx, double tz) => (x - tx) * (x - tx) + (z - tz) * (z - tz);

    public void Save()
    {
        if (!initialized) return;
        state.Control = worldProgression.State;
        api.WorldManager.SaveGame.StoreData(SaveKey, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(state)));
    }

    public void Dispose() => Save();

    private sealed record BlockWrite(BlockPos Position, int BlockId);
    public sealed class SiteSave
    {
        public int SchemaVersion { get; set; } = 1;
        public int OriginX { get; set; }
        public int OriginZ { get; set; }
        public List<SiteState> Sites { get; set; } = new();
        public WorldProgressionState Control { get; set; } = new();
        public Dictionary<string, int> PlacementGenerations { get; set; } = new();
        public long BossEntityId { get; set; }
        public string BossIdentity { get; set; } = "";
        public float BossHealth { get; set; }
        public double BossLastX { get; set; }
        public double BossLastY { get; set; }
        public double BossLastZ { get; set; }
        public bool ControlRewardIssued { get; set; }
    }
    public sealed class SiteState
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public int TargetX { get; set; }
        public int TargetZ { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public int Radius { get; set; }
        public bool Generated { get; set; }
        public bool ChartIssued { get; set; }
    }
}
