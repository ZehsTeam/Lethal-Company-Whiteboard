using System.Collections.Generic;
using UnityEngine;

namespace com.github.zehsteam.Whiteboard;

[CreateAssetMenu(menuName = "Whiteboard/SpriteSheetData")]
public class SpriteSheetData : ScriptableObject
{
    public List<SpriteSheetItem> Sprites = [];

    public string GetAllSpritesText()
    {
        string text = string.Empty;

        foreach (var item in Sprites)
        {
            text += $"{item.GetText()} ";
        }

        return text.Trim();
    }

    public string GetParsedText(string text, bool matchCase = false)
    {
        string result = text;

        if (matchCase && result.Contains("<all>"))
        {
            result = result.Replace("<all>", GetAllSpritesText());
        }

        if (!matchCase && result.Contains("<all>", System.StringComparison.OrdinalIgnoreCase))
        {
            result = result.Replace("<all>", GetAllSpritesText(), System.StringComparison.OrdinalIgnoreCase);
        }

        foreach (var item in Sprites)
        {
            if (matchCase && result.Contains(item.Name))
            {
                result = result.Replace(item.Name, item.GetText());
            }

            if (!matchCase && result.Contains(item.Name, System.StringComparison.OrdinalIgnoreCase))
            {
                result = result.Replace(item.Name, item.GetText(), System.StringComparison.OrdinalIgnoreCase);
            }
        }

        return result;
    }
}

[System.Serializable]
public class SpriteSheetItem
{
    public string Name;
    public int Index;
    public int EndIndex;
    public int AnimationSpeed;

    public string GetText()
    {
        if (EndIndex > Index)
        {
            return $"<sprite anim=\"{Index},{EndIndex},{AnimationSpeed}\">";
        }

        return $"<sprite={Index}>";
    }
}
