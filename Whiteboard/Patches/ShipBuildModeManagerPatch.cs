using com.github.zehsteam.Whiteboard.MonoBehaviours;
using HarmonyLib;

namespace com.github.zehsteam.Whiteboard.Patches;

[HarmonyPatch(typeof(ShipBuildModeManager))]
internal class ShipBuildModeManagerPatch
{
    [HarmonyPatch(nameof(ShipBuildModeManager.PlayerMeetsConditionsToBuild))]
    [HarmonyPostfix]
    private static void PlayerMeetsConditionsToBuild(ref bool __result)
    {
        if (WhiteboardEditorBehaviour.Instance == null) return;

        if (WhiteboardEditorBehaviour.Instance.IsOpen)
        {
            __result = false;
        }
    }
}
