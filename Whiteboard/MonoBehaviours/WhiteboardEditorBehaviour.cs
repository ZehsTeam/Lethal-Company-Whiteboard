using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace com.github.zehsteam.Whiteboard.MonoBehaviours;

public class WhiteboardEditorBehaviour : MonoBehaviour
{
    public static WhiteboardEditorBehaviour Instance;

    public GameObject EditorWindowObject = null;
    public TMP_InputField DisplayTextInputField = null;

    public GameObject HostOnlyObject = null;
    public Button HostOnlyButton = null;
    public GameObject HostOnlyCheckedObject = null;
    
    public TMP_InputField TextColorInputField = null;
    public TMP_Dropdown FontSizeDropdown = null;
    public TMP_Dropdown FontStyleDropdown = null;
    public TMP_Dropdown FontFamilyDropdown = null;
    public TMP_Dropdown HorizontalAlignmentDropdown = null;
    public TMP_Dropdown VerticalAlignmentDropdown = null;

    public const int DefaultFontSizeIndex = 7; // 0.12

    public bool IsOpen { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        CloseEditorWindow();
    }

    public void OpenEditorWindow()
    {
        if (WhiteboardBehaviour.Instance == null)
        {
            Plugin.logger.LogError("Failed to open whiteboard editor window. Whiteboard instance was not found.");
            return;
        }

        if (Utils.IsQuickMenuOpen() || IsOpen) return;

        HostOnlyObject.SetActive(Plugin.IsHostOrServer);

        if (Plugin.IsHostOrServer)
        {
            UpdateHostOnlyCheckbox();
        }

        IsOpen = true;
        EditorWindowObject.SetActive(true);
        SetDataToUI(WhiteboardBehaviour.Instance.Data);
        Utils.SetCursorLockState(false);
        PlayerUtils.SetControlsEnabled(false);
    }

    public void CloseEditorWindow()
    {
        IsOpen = false;
        EditorWindowObject.SetActive(false);
        Utils.SetCursorLockState(true);
        PlayerUtils.SetControlsEnabled(true);
    }

    public void OnConfirm()
    {
        if (WhiteboardBehaviour.Instance == null)
        {
            Plugin.logger.LogError("Failed to confirm whiteboard changes. Whiteboard instance was not found.");
            return;
        }

        WhiteboardBehaviour.Instance.SetData(GetDataFromUI());

        CloseEditorWindow();
    }

    public void OnCancel()
    {
        CloseEditorWindow();
    }

    public void OnReset()
    {
        SetDataToUI(new WhiteboardData());
    }

    public void OnHostOnly()
    {
        if (!Plugin.IsHostOrServer) return;

        Plugin.ConfigManager.HostOnly.Value = !Plugin.ConfigManager.HostOnly.Value;
        UpdateHostOnlyCheckbox();
    }

    private void UpdateHostOnlyCheckbox()
    {
        HostOnlyCheckedObject.SetActive(Plugin.ConfigManager.HostOnly.Value);
    }

    private WhiteboardData GetDataFromUI()
    {
        string displayText = DisplayTextInputField.text;
        string textColor = TextColorInputField.text;
        int fontSizeIndex = FontSizeDropdown.value;
        int fontStyleIndex = FontStyleDropdown.value;
        int fontFamilyIndex = FontFamilyDropdown.value;
        int horizontalAlignmentIndex = HorizontalAlignmentDropdown.value;
        int verticalAlignmentIndex = VerticalAlignmentDropdown.value;

        return new WhiteboardData(displayText, textColor, fontSizeIndex, fontStyleIndex, fontFamilyIndex, horizontalAlignmentIndex, verticalAlignmentIndex);
    }

    private void SetDataToUI(WhiteboardData data)
    {
        DisplayTextInputField.text = data.DisplayText;
        TextColorInputField.text = data.TextColor;
        FontSizeDropdown.value = data.FontSizeIndex;
        FontStyleDropdown.value = data.FontStyleIndex;
        FontFamilyDropdown.value = data.FontFamilyIndex;
        HorizontalAlignmentDropdown.value = data.HorizontalAlignmentIndex;
        VerticalAlignmentDropdown.value = data.VerticalAlignmentIndex;
    }
}
