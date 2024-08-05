using HarmonyLib;

namespace com.github.zehsteam.Whiteboard.Patches;

[HarmonyPatch(typeof(Terminal))]
internal class TerminalPatch
{
    [HarmonyPatch(nameof(Terminal.Start))]
    [HarmonyPostfix]
    private static void StartPatch()
    {
        UnlockableHelper.UpdateUnlockablePrice(Content.WhiteboardUnlockableItemDef, Plugin.ConfigManager.Price.Value);
    }
}
