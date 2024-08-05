using com.github.zehsteam.Whiteboard.MonoBehaviours;
using GameNetcodeStuff;
using HarmonyLib;

namespace com.github.zehsteam.Whiteboard.Patches;

[HarmonyPatch(typeof(PlayerControllerB))]
internal class PlayerControllerBPatch
{
    [HarmonyPatch(nameof(PlayerControllerB.Start))]
    [HarmonyPostfix]
    private static void StartPatch(ref PlayerControllerB __instance)
    {
        if (__instance != PlayerUtils.GetLocalPlayerScript()) return;

        if (WhiteboardBehaviour.Instance != null)
        {
            WhiteboardBehaviour.Instance.SetWorldCanvasCamera();
        }
    }

    [HarmonyPatch(nameof(PlayerControllerB.KillPlayer))]
    [HarmonyPostfix]
    private static void KillPlayerPatch()
    {
        if (WhiteboardEditorBehaviour.Instance == null) return;

        if (WhiteboardEditorBehaviour.Instance.IsOpen)
        {
            WhiteboardEditorBehaviour.Instance.CloseEditorWindow();
        }
    }
}
