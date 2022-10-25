using System;
using UnityEngine;

namespace Essentials.Inspector
{
    public class FieldColorAttribute : PropertyAttribute
    {
        public readonly Color color;

        public FieldColorAttribute(int r, int g, int b)
        {
            color = new Color32(Convert.ToByte(r), Convert.ToByte(g), Convert.ToByte(b), 255);
        }
    }
}