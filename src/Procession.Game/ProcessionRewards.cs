using Procession.Core;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Server;

namespace Procession.Game;

/// <summary>Finite source rewards. Called only once after coordinator ownership is consumed.</summary>
internal static class ProcessionRewards
{
    internal static void Chanter(ICoreServerAPI api, Entity entity, ChanterKind kind, bool victory, DamageSource source)
    {
        // A legitimate partial kill pays immediately; environmental/admin cleanup cannot farm rewards.
        if (source is null || source.CauseEntity is not EntityPlayer && source.SourceEntity is not EntityPlayer) return;
        Drop(api, entity, "gear-rusty", 2);
        if (api.World.Rand.NextDouble() < .25) Drop(api, entity, "gear-rusty", api.World.Rand.Next(1, 3));
        switch (kind)
        {
            case ChanterKind.Veiled: Drop(api, entity, "cloth-plain", 1); break;
            case ChanterKind.Bound:
                Drop(api, entity, "bandage-clean", 2); Drop(api, entity, "flaxtwine", 2); break;
            case ChanterKind.Resonant:
                Drop(api, entity, "metal-parts", 1, true); Drop(api, entity, "metal-scraps", 2, true); break;
            case ChanterKind.Hollow:
                Drop(api, entity, "flaxtwine", 2); Drop(api, entity, "fat", 1); break;
        }
        if (victory)
        {
            Drop(api, entity, "gear-rusty", 4); Drop(api, entity, "gear-temporal", 1);
            // Vanilla recovery supplies stand on their own; V5's bespoke dressing remains unimplemented.
            Drop(api, entity, "bandage-clean", 2);
        }
    }

    internal static void OpeningBell(ICoreServerAPI api, Entity entity, DamageSource source)
    {
        if (source is null || source.CauseEntity is not EntityPlayer && source.SourceEntity is not EntityPlayer) return;
        Drop(api, entity, "gear-rusty", 1); Drop(api, entity, "metal-parts", 1, true); Drop(api, entity, "metal-scraps", 2, true);
    }

    private static void Drop(ICoreServerAPI api, Entity entity, string path, int quantity, bool block = false)
    {
        var code = new AssetLocation("game", path);
        CollectibleObject? collectible = block ? api.World.GetBlock(code) : api.World.GetItem(code);
        if (collectible == null)
        {
            api.Logger.Warning("[Procession] Missing finite reward asset {0}; reward skipped.", code); return;
        }
        var stack = collectible is Block b ? new ItemStack(b, quantity) : new ItemStack((Item)collectible, quantity);
        api.World.SpawnItemEntity(stack, entity.Pos.XYZ.AddCopy(0, .5, 0));
    }
}
