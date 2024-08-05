using com.github.zehsteam.Whiteboard.MonoBehaviours;
using HarmonyLib;

namespace com.github.zehsteam.Whiteboard.Patches;

[HarmonyPatch(typeof(QuickMenuManager))]
internal class QuickMenuManagerPatch
{
    [HarmonyPatch(nameof(QuickMenuManager.OpenQuickMenu))]
    [HarmonyPrefix]
    private static bool OpenQuickMenuPatch()
    {
        if (WhiteboardEditorBehaviour.Instance == null) return false;

        if (WhiteboardEditorBehaviour.Instance.IsOpen)
        {
            WhiteboardEditorBehaviour.Instance.CloseEditorWindow();
            return false;
        }

        return true;
    }

    [HarmonyPatch(nameof(QuickMenuManager.CloseQuickMenu))]
    [HarmonyPostfix]
    private static void CloseQuickMenuPatch()
    {
        if (WhiteboardEditorBehaviour.Instance == null) return;

        if (WhiteboardEditorBehaviour.Instance.IsOpen)
        {
            Utils.SetCursorLockState(false);
        }
    }
}
