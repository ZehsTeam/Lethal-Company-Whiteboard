using HarmonyLib;
using Unity.Netcode;
using UnityEngine;

namespace com.github.zehsteam.Whiteboard.Patches;

[HarmonyPatch(typeof(GameNetworkManager))]
internal class GameNetworkManagerPatch
{
    [HarmonyPatch(nameof(GameNetworkManager.Start))]
    [HarmonyPostfix]
    static void StartPatch()
    {
        AddNetworkPrefabs();
    }

    private static void AddNetworkPrefabs()
    {
        AddNetworkPrefab(Content.NetworkHandlerPrefab);
    }

    private static void AddNetworkPrefab(GameObject prefab)
    {
        if (prefab == null)
        {
            Plugin.logger.LogError($"Failed to add network prefab. GameObject is null.");
            return;
        }

        NetworkManager.Singleton.AddNetworkPrefab(prefab);

        Plugin.logger.LogInfo($"Registered \"{prefab.name}\" network prefab.");
    }
}
