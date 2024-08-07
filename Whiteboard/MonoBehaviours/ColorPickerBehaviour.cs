using UnityEngine;

namespace com.github.zehsteam.Whiteboard.MonoBehaviours;

public class ColorPickerBehaviour : MonoBehaviour
{
    public static ColorPickerBehaviour Instance;

    public GameObject ColorPickerWindowObject = null;
    public ColorPickerControlBehaviour ColorPickerControlBehaviour = null;

    public bool IsWindowOpen {  get; private set; }

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
        if (!WhiteboardEditorBehaviour.Instance.IsWindowOpen || IsWindowOpen) return;

        IsWindowOpen = true;
        ColorPickerWindowObject.SetActive(true);
        ColorPickerControlBehaviour.SetColor(WhiteboardEditorBehaviour.Instance.TextHexColor);
    }

    public void CloseWindow()
    {
        IsWindowOpen = false;
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
