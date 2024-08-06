using UnityEngine;

namespace com.github.zehsteam.Whiteboard.MonoBehaviours;

public class ColorPickerBehaviour : MonoBehaviour
{
    public static ColorPickerBehaviour Instance;

    public GameObject ColorPickerWindowObject = null;
    public ColorPickerControlBehaviour ColorPickerControlBehaviour = null;

    public bool IsOpen {  get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        CloseWindow();
    }

    public void OpenWindow()
    {
        if (!WhiteboardEditorBehaviour.Instance.IsOpen || IsOpen) return;

        IsOpen = true;
        ColorPickerControlBehaviour.SetHexColorInputField(WhiteboardEditorBehaviour.Instance.TextHexColor, updateColorPicker: true);
        ColorPickerWindowObject.SetActive(true);
    }

    public void CloseWindow()
    {
        IsOpen = false;
        ColorPickerWindowObject.SetActive(false);
    }

    public void OnConfirmButtonClicked()
    {
        WhiteboardEditorBehaviour.Instance.SetTextHexColor(ColorPickerControlBehaviour.GetHexColor());

        CloseWindow();
    }

    public void OnCancelButtonClicked()
    {
        CloseWindow();
    }
}
