﻿using com.github.zehsteam.Whiteboard.MonoBehaviours;
using System;
using Unity.Netcode;

namespace com.github.zehsteam.Whiteboard;

[Serializable]
public class WhiteboardData : INetworkSerializable
{
    public string DisplayText;
    public string TextHexColor;
    public int FontSizeIndex;
    public int FontStyleIndex;
    public int FontFamilyIndex;
    public int HorizontalAlignmentIndex;
    public int VerticalAlignmentIndex;

    public WhiteboardData()
    {
        try
        {
            DisplayText = Plugin.ConfigManager.DefaultDisplayText.Value;
        }
        catch { }

        TextHexColor = WhiteboardEditorBehaviour.DefaultTextHexColor;
        FontSizeIndex = WhiteboardEditorBehaviour.DefaultFontSizeIndex;
    }

    public WhiteboardData(string displayText)
    {
        DisplayText = displayText;
        TextHexColor = WhiteboardEditorBehaviour.DefaultTextHexColor;
        FontSizeIndex = WhiteboardEditorBehaviour.DefaultFontSizeIndex;
    }

    public WhiteboardData(string displayText, string textHexColor, int fontSizeIndex, int fontStyleIndex, int fontFamilyIndex, int horizontalAlignmentIndex, int verticalAlignmentIndex) : this(displayText)
    {
        TextHexColor = textHexColor;
        FontSizeIndex = fontSizeIndex;
        FontStyleIndex = fontStyleIndex;
        FontFamilyIndex = fontFamilyIndex;
        HorizontalAlignmentIndex = horizontalAlignmentIndex;
        VerticalAlignmentIndex = verticalAlignmentIndex;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref DisplayText);
        serializer.SerializeValue(ref TextHexColor);
        serializer.SerializeValue(ref FontSizeIndex);
        serializer.SerializeValue(ref FontStyleIndex);
        serializer.SerializeValue(ref FontFamilyIndex);
        serializer.SerializeValue(ref HorizontalAlignmentIndex);
        serializer.SerializeValue(ref VerticalAlignmentIndex);
    }
}
