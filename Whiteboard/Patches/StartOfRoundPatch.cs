using com.github.zehsteam.Whiteboard.MonoBehaviours;
using GameNetcodeStuff;
using HarmonyLib;
using Unity.Netcode;
using UnityEngine;

namespace com.github.zehsteam.Whiteboard.Patches;

[HarmonyPatch(typeof(StartOfRound))]
internal class StartOfRoundPatch
{
    [HarmonyPatch(nameof(StartOfRound.Awake))]
    [HarmonyPostfix]
    private static void AwakePatch()
    {
        SpawnNetworkHandler();
    }

    private static void SpawnNetworkHandler()
    {
        if (!Plugin.IsHostOrServer) return;

        var networkHandlerHost = Object.Instantiate(Content.NetworkHandlerPrefab, Vector3.zero, Quaternion.identity);
        networkHandlerHost.GetComponent<NetworkObject>().Spawn();
    }

    [HarmonyPatch(nameof(StartOfRound.Start))]
    [HarmonyPostfix]
    private static void StartPatch()
    {
        UnlockableHelper.UpdateUnlockablePrice(Content.WhiteboardUnlockableItemDef, Plugin.ConfigManager.Price.Value);
    }

    [HarmonyPatch(nameof(StartOfRound.OnClientConnect))]
    [HarmonyPrefix]
    private static void OnClientConnectPatch(ref ulong clientId)
    {
        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = [clientId]
            }
        };

        PluginNetworkBehaviour.Instance.SetWhiteboardUnlockablePriceClientRpc(Plugin.ConfigManager.Price.Value, clientRpcParams);
    }

    [HarmonyPatch(nameof(StartOfRound.ReviveDeadPlayers))]
    [HarmonyPostfix]
    private static void ReviveDeadPlayersPatch()
    {
        if (WhiteboardEditorBehaviour.Instance == null) return;

        if (WhiteboardEditorBehaviour.Instance.IsOpen)
        {
            PlayerControllerB playerScript = PlayerUtils.GetLocalPlayerScript();
            if (playerScript == null) return;

            playerScript.disableMoveInput = true;
        }
    }
}
