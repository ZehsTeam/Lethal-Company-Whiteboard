using Unity.Netcode;

namespace com.github.zehsteam.Whiteboard.MonoBehaviours;

internal class PluginNetworkBehaviour : NetworkBehaviour
{
    public static PluginNetworkBehaviour Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    [ClientRpc]
    public void SetWhiteboardUnlockablePriceClientRpc(int price, ClientRpcParams clientRpcParams = default)
    {
        UnlockableHelper.UpdateUnlockablePrice(Content.WhiteboardUnlockableItemDef, price);
    }
}
