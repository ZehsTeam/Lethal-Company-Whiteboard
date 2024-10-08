﻿using LethalLib.Extras;

namespace com.github.zehsteam.Whiteboard;

internal class UnlockableHelper
{
    public static void RegisterUnlockable(UnlockableItemDef unlockableItemDef, LethalLib.Modules.StoreType storeType, int price, TerminalNode terminalNode)
    {
        try
        {
            unlockableItemDef.unlockable.shopSelectionNode.itemCost = price;
            unlockableItemDef.unlockable.shopSelectionNode.terminalOptions[0].result.itemCost = price;
        }
        catch { }

        LethalLib.Modules.Utilities.FixMixerGroups(unlockableItemDef.unlockable.prefabObject);
        LethalLib.Modules.NetworkPrefabs.RegisterNetworkPrefab(unlockableItemDef.unlockable.prefabObject);
        LethalLib.Modules.Unlockables.RegisterUnlockable(unlockableItemDef, storeType, null, null, terminalNode, price);

        Plugin.logger.LogInfo($"Registered \"{unlockableItemDef.unlockable.unlockableName}\" unlockable shop item with a price of ${price}.");
    }

    public static void UpdateUnlockablePrice(UnlockableItemDef unlockableItemDef, int price)
    {
        try
        {
            unlockableItemDef.unlockable.shopSelectionNode.itemCost = price;
            unlockableItemDef.unlockable.shopSelectionNode.terminalOptions[0].result.itemCost = price;
        }
        catch { }

        LethalLib.Modules.Unlockables.UpdateUnlockablePrice(unlockableItemDef.unlockable, price);

        Plugin.logger.LogInfo($"Updated \"{unlockableItemDef.unlockable.unlockableName}\" unlockable shop item price to ${price}.");
    }
}
