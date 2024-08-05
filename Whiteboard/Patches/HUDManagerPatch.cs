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
}
