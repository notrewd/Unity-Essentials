using System;
using UnityEngine;


public class LabelColorAttribute : PropertyAttribute
{
    public readonly Color color;

    public LabelColorAttribute(int r, int g, int b)
    {
        color = new Color32(Convert.ToByte(r), Convert.ToByte(g), Convert.ToByte(b), 255);
    }
}
