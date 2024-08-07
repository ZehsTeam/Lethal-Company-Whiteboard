using com.github.zehsteam.Whiteboard.MonoBehaviours;
using HarmonyLib;

namespace com.github.zehsteam.Whiteboard.Patches;

[HarmonyPatch(typeof(HUDManager))]
internal class HUDManagerPatch
{
    [HarmonyPatch(nameof(HUDManager.Start))]
    [HarmonyPostfix]
    private static void StartPatch()
    {
        Plugin.Instance.SpawnWhiteboardEditorCanvas();
    }

    [HarmonyPatch(nameof(HUDManager.OpenMenu_performed))]
    [HarmonyPrefix]
    private static void OpenMenu_performedPatch()
    {
        if (WhiteboardEditorBehaviour.Instance == null) return;

        if (WhiteboardEditorBehaviour.Instance.IsWindowOpen)
        {
            WhiteboardEditorBehaviour.Instance.CloseWindow();
        }
    }
}
