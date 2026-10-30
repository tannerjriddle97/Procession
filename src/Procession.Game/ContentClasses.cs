using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.API.Config;
using Vintagestory.GameContent;

namespace Procession.Game;

public sealed class ProcessionChanter(Entity entity) : EntityBehavior(entity)
{
    public override string PropertyName() => "ProcessionChanter";
    public override void OnEntityReceiveDamage(DamageSource source, ref float damage)
    {
        if (source.Type == EnumDamageType.Heal || damage <= 0) return;
        // Bow/thrown impacts have a projectile as their immediate source, with the shooter as CauseEntity.
        string sourceClass = source.SourceEntity?.GetType().Name ?? "";
        if (sourceClass.Contains("Projectile", StringComparison.OrdinalIgnoreCase)
            || sourceClass.Contains("Thrown", StringComparison.OrdinalIgnoreCase)
            || source.SourceEntity != null && source.CauseEntity != null && source.SourceEntity != source.CauseEntity)
            damage *= .5f;
    }
}

/// <summary>Stock movement/melee plus a clearly delayed radial attack. No world/block damage.</summary>
public sealed class ProcessionBoss(Entity entity) : EntityBehavior(entity)
{
    private float cooldown = 8;
    private float charge;
    private float ringTimer;
    private int phase;
    private float radius;
    private Vec3d? home;
    public override string PropertyName() => "ProcessionBoss";

    public override void OnGameTick(float dt)
    {
        if (entity.World.Side != EnumAppSide.Server || !entity.Alive) return;
        if (home == null)
        {
            if (!entity.WatchedAttributes.HasAttribute("processionHomeX"))
            {
                entity.WatchedAttributes.SetDouble("processionHomeX", entity.Pos.X);
                entity.WatchedAttributes.SetDouble("processionHomeY", entity.Pos.Y);
                entity.WatchedAttributes.SetDouble("processionHomeZ", entity.Pos.Z);
            }
            home = new(entity.WatchedAttributes.GetDouble("processionHomeX"), entity.WatchedAttributes.GetDouble("processionHomeY"), entity.WatchedAttributes.GetDouble("processionHomeZ"));
        }
        var health = entity.GetBehavior<EntityBehaviorHealth>();
        if (health == null) return;
        if (entity.Pos.XYZ.SquareDistanceTo(home) > 48 * 48)
        {
            entity.TeleportTo(home); health.Health = health.MaxHealth; health.MarkDirty(); charge = 0; cooldown = 8;
            entity.WatchedAttributes.SetBool("processionPulseCharging", false);
            return;
        }
        float fraction = health.Health / health.MaxHealth;
        int newPhase = fraction > 2f / 3 ? 1 : fraction > 1f / 3 ? 2 : 3;
        if (newPhase != phase)
        {
            phase = newPhase; entity.WatchedAttributes.SetInt("processionPhase", phase);
            entity.World.PlaySoundAt(new AssetLocation("game:sounds/creature/bell/alarm"), entity, null, false, 40, .6f);
        }
        if (charge > 0)
        {
            ringTimer -= dt;
            if (ringTimer <= 0) { DrawRing(); ringTimer = .2f; }
            charge -= dt;
            if (charge <= 0)
            {
                entity.WatchedAttributes.SetBool("processionPulseCharging", false);
                foreach (var player in entity.World.AllOnlinePlayers)
                {
                    if (!player.Entity.Alive || player.Entity.Pos.Dimension != entity.Pos.Dimension
                        || player.Entity.Pos.XYZ.SquareDistanceTo(entity.Pos.XYZ) > radius * radius
                        || !HasSight(player.Entity.Pos.XYZ.AddCopy(0, 1, 0))) continue;
                    player.Entity.ReceiveDamage(new DamageSource { Source = EnumDamageSource.Entity, Type = EnumDamageType.BluntAttack,
                        SourceEntity = entity, CauseEntity = entity, DamageTier = 2, KnockbackStrength = .4f }, 3 + phase * 2);
                }
                cooldown = phase == 3 ? 7 : 10;
            }
            return;
        }
        cooldown -= dt;
        if (cooldown <= 0 && entity.World.AllOnlinePlayers.Any(p => p.Entity.Alive && p.Entity.Pos.XYZ.SquareDistanceTo(entity.Pos.XYZ) < 20 * 20))
        {
            radius = 4 + phase; charge = 1.8f; ringTimer = 0;
            entity.WatchedAttributes.SetBool("processionPulseCharging", true);
            entity.WatchedAttributes.SetFloat("processionPulseRadius", radius);
            entity.World.PlaySoundAt(new AssetLocation("game:sounds/creature/bell/listen"), entity, null, false, 30, 1);
        }
    }

    private bool HasSight(Vec3d to)
    {
        Vec3d from = entity.Pos.XYZ.AddCopy(0, 1, 0);
        double length = from.DistanceTo(to);
        for (double distance = .5; distance < length; distance += .5)
        {
            double fraction = distance / length;
            var p = new BlockPos((int)(from.X + (to.X - from.X) * fraction), (int)(from.Y + (to.Y - from.Y) * fraction), (int)(from.Z + (to.Z - from.Z) * fraction));
            if (entity.World.BlockAccessor.GetBlock(p).Replaceable < 6000) return false;
        }
        return true;
    }

    private void DrawRing()
    {
        for (int i = 0; i < 24; i++)
        {
            double angle = i * Math.PI * 2 / 24;
            var p = entity.Pos.XYZ.AddCopy(Math.Cos(angle) * radius, .15, Math.Sin(angle) * radius);
            entity.World.SpawnParticles(1, ColorUtil.ToRgba(230, 75, 205, 185), p, p.AddCopy(.04, .04, .04),
                new Vec3f(0, .1f, 0), new Vec3f(0, .2f, 0), .35f, 0, .12f, EnumParticleModel.Cube);
        }
    }

    public override void OnEntityDeath(DamageSource source)
    {
        if (entity.World.Side == EnumAppSide.Server)
            entity.Api.ModLoader.GetModSystem<ProcessionSystem>().Sites?.BossDefeated(entity);
    }
}

public sealed class ProcessionRouteChart : Item
{
    public override void OnHeldInteractStart(ItemSlot itemslot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, bool firstEvent, ref EnumHandHandling handling)
    {
        handling = EnumHandHandling.PreventDefault;
        if (!firstEvent || byEntity.World.Side != EnumAppSide.Server || byEntity is not EntityPlayer ep) return;
        var player = byEntity.World.PlayerByUid(ep.PlayerUID);
        var sites = api.ModLoader.GetModSystem<ProcessionSystem>().Sites;
        if (byEntity.Controls.Sneak) { sites?.RecoverControl(player); return; }
        var text = sites?.DescribeRoute(player) ?? "The route is still settling.";
        ((ICoreServerAPI)api).SendMessage(player, GlobalConstants.GeneralChatGroup, text, EnumChatType.Notification);
    }
}

public sealed class ProcessionClosingBell : Block
{
    public override ItemStack[] GetDrops(IWorldAccessor world, BlockPos pos, IPlayer byPlayer, float dropQuantityMultiplier = 1)
    {
        var stack = new ItemStack(this);
        int generation = world.Side == EnumAppSide.Server ? api.ModLoader.GetModSystem<ProcessionSystem>().Sites?.ControllerGenerationAt(pos) ?? 0 : 0;
        stack.Attributes.SetInt("processionControllerGeneration", generation);
        return new[] { stack };
    }

    public override ItemStack OnPickBlock(IWorldAccessor world, BlockPos pos)
    {
        var stack = new ItemStack(this);
        stack.Attributes.SetInt("processionControllerGeneration", 0);
        return stack;
    }

    public override bool DoPlaceBlock(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel, ItemStack byItemStack)
    {
        if (world.Side == EnumAppSide.Server && api.ModLoader.GetModSystem<ProcessionSystem>().Sites?.ControlEarned != true) return false;
        bool placed = base.DoPlaceBlock(world, byPlayer, blockSel, byItemStack);
        if (placed && world.Side == EnumAppSide.Server)
        {
            var sites = api.ModLoader.GetModSystem<ProcessionSystem>().Sites;
            if (sites?.InstallController(blockSel.Position, byPlayer, byItemStack) != true)
                ((ICoreServerAPI)api).SendMessage(byPlayer, GlobalConstants.GeneralChatGroup, "Another Closing Bell already controls this world. This copy is inactive.", EnumChatType.Notification);
        }
        return placed;
    }

    public override bool OnBlockInteractStart(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel)
    {
        if (world.Side == EnumAppSide.Server)
        {
            var sites = api.ModLoader.GetModSystem<ProcessionSystem>().Sites;
            if (sites != null)
            {
                bool changed = sites.TrySetSuppressed(!sites.Suppressed, byPlayer, blockSel.Position);
                string text = !changed ? "This bell is inactive, unearned, or belongs to another keeper."
                    : sites.Suppressed ? "Future Processions suppressed world-wide. Ordinary storms continue." : "Future Processions allowed world-wide.";
                ((ICoreServerAPI)api).SendMessage(byPlayer, GlobalConstants.GeneralChatGroup, text, EnumChatType.Notification);
            }
        }
        return true;
    }

    public override void OnBlockBroken(IWorldAccessor world, BlockPos pos, IPlayer byPlayer, float dropQuantityMultiplier = 1)
    {
        if (world.Side == EnumAppSide.Server)
        {
            var sites = api.ModLoader.GetModSystem<ProcessionSystem>().Sites;
            if (byPlayer == null) sites?.ControllerRemoved(pos);
            else if (sites?.RemoveController(pos, byPlayer) == false) return;
        }
        base.OnBlockBroken(world, pos, byPlayer, dropQuantityMultiplier);
    }

    public override void OnBlockRemoved(IWorldAccessor world, BlockPos pos)
    {
        if (world.Side == EnumAppSide.Server) api.ModLoader.GetModSystem<ProcessionSystem>().Sites?.ControllerRemoved(pos);
        base.OnBlockRemoved(world, pos);
    }
}
