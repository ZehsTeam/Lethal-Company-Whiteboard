using GameNetcodeStuff;
using UnityEngine;

namespace com.github.zehsteam.Whiteboard;

internal class Utils
{
    public static void SetCursorLockState(bool value)
    {
        // If the pause menu is open and you try to lock the cursor, return.
        if (IsQuickMenuOpen()) return;

        Cursor.lockState = value ? CursorLockMode.Locked : CursorLockMode.None;

        if (value)
        {
            Cursor.visible = false;
        }
        else
        {
            if (!StartOfRound.Instance.localPlayerUsingController)
            {
                Cursor.visible = true;
            }
        }
    }

    public static string GetCurrentSaveFileName()
    {
        return GameNetworkManager.Instance.currentSaveFileName;
    }

    public static void SaveToCurrentSaveFile<T>(string key, T value)
    {
        ES3.Save($"{MyPluginInfo.PLUGIN_GUID}.{key}", value, GetCurrentSaveFileName());
    }

    public static T LoadFromCurrentSaveFile<T>(string key, T defaultValue = default)
    {
        return ES3.Load($"{MyPluginInfo.PLUGIN_GUID}.{key}", GetCurrentSaveFileName(), defaultValue);
    }

    public static bool KeyExistsInCurrentSaveFile(string key)
    {
        return ES3.KeyExists($"{MyPluginInfo.PLUGIN_GUID}.{key}", GetCurrentSaveFileName());
    }

    public static bool ArrayContains(string[] array, string value)
    {
        foreach (string item in array)
        {
            if (item.Equals(value, System.StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    public static bool IsQuickMenuOpen()
    {
        PlayerControllerB playerScript = PlayerUtils.GetLocalPlayerScript();
        if (playerScript == null) return false;

        return playerScript.quickMenuManager.isMenuOpen;
    }
}
