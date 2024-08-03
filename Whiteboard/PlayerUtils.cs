using GameNetcodeStuff;

namespace com.github.zehsteam.Whiteboard;

internal class PlayerUtils
{
    public static int GetLocalPlayerId()
    {
        return (int)GetLocalPlayerScript().playerClientId;
    }

    public static bool IsLocalPlayerId(int playerId)
    {
        return playerId == GetLocalPlayerId();
    }

    public static PlayerControllerB GetPlayerScript(int playerId)
    {
        try
        {
            return StartOfRound.Instance.allPlayerScripts[playerId];
        }
        catch
        {
            return null;
        }
    }

    public static PlayerControllerB GetLocalPlayerScript()
    {
        return GameNetworkManager.Instance.localPlayerController;
    }
}
