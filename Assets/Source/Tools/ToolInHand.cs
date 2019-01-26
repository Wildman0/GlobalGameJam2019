using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ToolType
{
    None,
    Wrench,
    Paintbrush
}

public class ToolInHand : MonoBehaviour
{
    public ToolType toolType;
    Color paintbrushColor = Color.white;
    
    public void SetPaintbrushColor(Color color)
    {
        paintbrushColor = color;
    }

    public Color GetPaintbrushColor()
    {
        return paintbrushColor;
    }
}
