using Unity.Netcode;

namespace com.github.zehsteam.Whiteboard;

internal class NetworkUtils
{
    public static int GetLocalClientId()
    {
        return (int)NetworkManager.Singleton.LocalClientId;
    }

    public static bool IsLocalClientId(int clientId)
    {
        return clientId == GetLocalClientId();
    }
}
