using GameNetcodeStuff;
using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace com.github.zehsteam.Whiteboard.MonoBehaviours;

public class WhiteboardBehaviour : NetworkBehaviour
{
    public static WhiteboardBehaviour Instance;

    public InteractTrigger InteractTrigger;
    public Canvas WorldCanvas = null;
    public TextMeshProUGUI WhiteboardText = null;
    public float[] FontSizeArray = [];
    public FontStyles[] FontStyleArray = [];
    public TMP_FontAsset[] FontAssetArray = [];

    [HideInInspector]
    public NetworkVariable<bool> IsHostOnly = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public WhiteboardData Data {  get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        SetWorldCanvasCamera();

        if (Plugin.IsHostOrServer)
        {
            LoadData();
        }
        else
        {
            RequestDataServerRpc(NetworkUtils.GetLocalClientId());
        }
    }

    public override void OnNetworkSpawn()
    {
        IsHostOnly.OnValueChanged += OnIsHostOnlyChanged;

        if (Plugin.IsHostOrServer)
        {
            IsHostOnly.Value = Plugin.ConfigManager.HostOnly.Value;
        }
        else if (IsHostOnly.Value)
        {
            InteractTrigger.interactable = false;
        }
    }

    public override void OnNetworkDespawn()
    {
        IsHostOnly.OnValueChanged -= OnIsHostOnlyChanged;

        if (WhiteboardEditorBehaviour.Instance == null) return;

        if (WhiteboardEditorBehaviour.Instance.IsOpen)
        {
            WhiteboardEditorBehaviour.Instance.CloseEditorWindow();
        }
    }

    private void OnIsHostOnlyChanged(bool previous, bool current)
    {
        if (Plugin.IsHostOrServer) return;

        InteractTrigger.interactable = !current;
    }

    public void OnInteract()
    {
        if (WhiteboardEditorBehaviour.Instance == null)
        {
            Plugin.logger.LogError("Failed to open whiteboard editor window. WhiteboardEditorBehaviour instance was not found.");
            return;
        }

        WhiteboardEditorBehaviour.Instance.OpenEditorWindow();
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

    private void LoadData()
    {
        if (!Plugin.IsHostOrServer) return;

        string displayText = Utils.LoadFromCurrentSaveFile("Whiteboard_DisplayText", defaultValue: Plugin.ConfigManager.DefaultDisplayText.Value);
        string textColor = Utils.LoadFromCurrentSaveFile("Whiteboard_TextColor", defaultValue: "");
        int fontSizeIndex = Utils.LoadFromCurrentSaveFile("Whiteboard_FontSizeIndex", defaultValue: WhiteboardEditorBehaviour.DefaultFontSizeIndex);
        int fontStyleIndex = Utils.LoadFromCurrentSaveFile("Whiteboard_FontStyleIndex", defaultValue: 0);
        int fontFamilyIndex = Utils.LoadFromCurrentSaveFile("Whiteboard_FontFamilyIndex", defaultValue: 0);
        int horizontalAlignmentIndex = Utils.LoadFromCurrentSaveFile("Whiteboard_HorizontalAlignmentIndex", defaultValue: 0);
        int verticalAlignmentIndex = Utils.LoadFromCurrentSaveFile("Whiteboard_VerticalAlignmentIndex", defaultValue: 0);

        SetData(new WhiteboardData(displayText, textColor, fontSizeIndex, fontStyleIndex, fontFamilyIndex, horizontalAlignmentIndex, verticalAlignmentIndex));
    }

    private void SaveData()
    {
        if (!Plugin.IsHostOrServer) return;

        Utils.SaveToCurrentSaveFile("Whiteboard_DisplayText", Data.DisplayText);
        Utils.SaveToCurrentSaveFile("Whiteboard_TextColor", Data.TextColor);
        Utils.SaveToCurrentSaveFile("Whiteboard_FontSizeIndex", Data.FontSizeIndex);
        Utils.SaveToCurrentSaveFile("Whiteboard_FontStyleIndex", Data.FontStyleIndex);
        Utils.SaveToCurrentSaveFile("Whiteboard_FontFamilyIndex", Data.FontFamilyIndex);
        Utils.SaveToCurrentSaveFile("Whiteboard_HorizontalAlignmentIndex", Data.HorizontalAlignmentIndex);
        Utils.SaveToCurrentSaveFile("Whiteboard_VerticalAlignmentIndex", Data.VerticalAlignmentIndex);
    }

    public void SetData(WhiteboardData data)
    {
        SetDataServerRpc(data, PlayerUtils.GetLocalPlayerId());
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetDataServerRpc(WhiteboardData data, int fromPlayerId)
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

            Plugin.Instance.LogInfoExtended($"Player \"{playerScript.playerUsername}\" set the whiteboard data. Display text: \"{data.DisplayText}\".");
        }
        else
        {
            Plugin.Instance.LogInfoExtended($"Set the whiteboard data. Display text: \"{data.DisplayText}\".");
        }
        
        SetDataClientRpc(data);
        SetDataOnLocalClient(data);
    }
    
    [ClientRpc]
    private void SetDataClientRpc(WhiteboardData data)
    {
        if (Plugin.IsHostOrServer) return;

        SetDataOnLocalClient(data);
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestDataServerRpc(int toClientId)
    {
        Plugin.Instance.LogInfoExtended($"Recieved request for whiteboard data from client: {toClientId}");

        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = [(ulong)toClientId]
            }
        };

        RequestDataClientRpc(Data, clientRpcParams);
    }

    [ClientRpc]
    private void RequestDataClientRpc(WhiteboardData data, ClientRpcParams clientRpcParams = default)
    {
        Plugin.Instance.LogInfoExtended("Recieved whiteboard data.");

        SetDataOnLocalClient(data);
    }

    public void SetDataOnLocalClient(WhiteboardData data)
    {
        Data = data;
        SaveData();
        UpdateWorldCanvas();
        LogData();
    }

    #region WorldCanvas
    private void UpdateWorldCanvas()
    {
        UpdateWhiteboardText();
    }

    private void UpdateWhiteboardText()
    {
        string displayText = string.Empty;

        string[] colorNames = ["black", "blue", "green", "orange", "purple", "red", "white", "yellow"];

        if (!string.IsNullOrWhiteSpace(Data.TextColor))
        {
            if (Utils.ArrayContains(colorNames, Data.TextColor))
            {
                displayText += $"<color=\"{Data.TextColor}\">";
            }
            else
            {
                if (Data.TextColor.StartsWith("#"))
                {
                    displayText += $"<color={Data.TextColor}>";
                }
                else
                {
                    displayText += $"<color=#{Data.TextColor}>";
                }
            }
        }

        displayText += Data.DisplayText;

        // If using the Signal Translator font, make all the text lowercase.
        if (Data.FontFamilyIndex == 2)
        {
            displayText = displayText.ToLower();
        }

        WhiteboardText.text = displayText;
        WhiteboardText.fontSize = FontSizeArray[Data.FontSizeIndex];
        WhiteboardText.fontStyle = FontStyleArray[Data.FontStyleIndex];
        WhiteboardText.font = FontAssetArray[Data.FontFamilyIndex];

        switch (Data.HorizontalAlignmentIndex)
        {
            case 0:
                WhiteboardText.horizontalAlignment = HorizontalAlignmentOptions.Left;
                break;
            case 1:
                WhiteboardText.horizontalAlignment = HorizontalAlignmentOptions.Center;
                break;
            case 2:
                WhiteboardText.horizontalAlignment = HorizontalAlignmentOptions.Right;
                break;
        }

        switch (Data.VerticalAlignmentIndex)
        {
            case 0:
                WhiteboardText.verticalAlignment = VerticalAlignmentOptions.Top;
                break;
            case 1:
                WhiteboardText.verticalAlignment = VerticalAlignmentOptions.Middle;
                break;
            case 2:
                WhiteboardText.verticalAlignment = VerticalAlignmentOptions.Bottom;
                break;
        }
    }
    #endregion

    private void LogData()
    {
        string message = string.Empty;

        message += $"DisplayText: \n\"{Data.DisplayText}\"\n\n";
        message += $"TextColor: \"{Data.TextColor}\"\n";
        message += $"FontSizeIndex: {Data.FontSizeIndex}\n";
        message += $"FontStyleIndex: {Data.FontStyleIndex}\n";
        message += $"FontFamilyIndex: {Data.FontFamilyIndex}\n";
        message += $"HorizontalAlignmentIndex: {Data.HorizontalAlignmentIndex}\n";
        message += $"VerticalAlignmentIndex: {Data.VerticalAlignmentIndex}\n";

        Plugin.Instance.LogInfoExtended($"\n{message.Trim()}\n\n");
    }
}
