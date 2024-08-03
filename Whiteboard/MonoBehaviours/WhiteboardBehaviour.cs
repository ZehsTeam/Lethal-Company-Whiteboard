using GameNetcodeStuff;
using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace com.github.zehsteam.Whiteboard.MonoBehaviours;

public class WhiteboardBehaviour : NetworkBehaviour
{
    public static WhiteboardBehaviour Instance;

    public Canvas WorldCanvas;
    public TextMeshProUGUI WhiteboardText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        SetWorldCanvasCamera();

        if (Plugin.IsHostOrServer)
        {
            SetDisplayTextToConfigValue();
        }
        else
        {
            RequestDisplayTextServerRpc(NetworkUtils.GetLocalClientId());
        }
    }

    public void SetWorldCanvasCamera()
    {
        PlayerControllerB playerScript = PlayerUtils.GetLocalPlayerScript();

        if (playerScript == null)
        {
            Plugin.logger.LogWarning("Failed to set whiteboard world canvas camera. Could not find the local player script or the local player is not spawned yet.");
            return;
        }
        
        WorldCanvas.worldCamera = playerScript.gameplayCamera;

        Plugin.Instance.LogInfoExtended("Set whiteboard world canvas camera.");
    }

    public void SetDisplayTextToConfigValue()
    {
        Plugin.Instance.LogInfoExtended("Set whiteboard display text to display text config value.");

        SetDisplayText(Plugin.ConfigManager.DisplayText.Value);
    }
    
    public void SetDisplayText(string displayText)
    {
        SetDisplayTextServerRpc(displayText, PlayerUtils.GetLocalPlayerId());
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetDisplayTextServerRpc(string displayText, int fromPlayerId)
    {
        PlayerControllerB playerScript = PlayerUtils.GetPlayerScript(fromPlayerId);
        if (playerScript == null) return;

        if (!playerScript.isHostPlayerObject)
        {
            if (Plugin.ConfigManager.HostOnly.Value)
            {
                Plugin.logger.LogWarning($"Player \"{playerScript.playerUsername}\" tried to edit the whiteboard while HostOnly mode is enabled.");
                return;
            }

            Plugin.Instance.LogInfoExtended($"Player \"{playerScript.playerUsername}\" set the whiteboard display text to \"{displayText}\".");
        }
        else
        {
            Plugin.Instance.LogInfoExtended($"Set the whiteboard display text to \"{displayText}\".");
        }
        
        SetDisplayTextClientRpc(displayText);
        SetDisplayTextOnLocalClient(displayText);
    }
    
    [ClientRpc]
    private void SetDisplayTextClientRpc(string displayText)
    {
        if (Plugin.IsHostOrServer) return;

        SetDisplayTextOnLocalClient(displayText);
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestDisplayTextServerRpc(int toClientId)
    {
        Plugin.Instance.LogInfoExtended($"Recieved request for whiteboard display text from client: {toClientId}");

        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = [(ulong)toClientId]
            }
        };

        RequestDisplayTextClientRpc(WhiteboardText.text, clientRpcParams);
    }

    [ClientRpc]
    private void RequestDisplayTextClientRpc(string displayText, ClientRpcParams clientRpcParams = default)
    {
        Plugin.Instance.LogInfoExtended("Recieved requested display text for whiteboard.");

        SetDisplayTextOnLocalClient(displayText);
    }

    public void SetDisplayTextOnLocalClient(string displayText)
    {
        WhiteboardText.text = displayText;

        Plugin.Instance.LogInfoExtended("Set whiteboard display text on local client.");
    }
}
