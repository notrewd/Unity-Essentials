using System;
using UnityEngine;

namespace Essentials.Inspector
{
    /// <summary>
    /// Attribute to change the background color of a field in the inspector.
    /// </summary>
    public class FieldColorAttribute : PropertyAttribute
    {
        public readonly Color color;

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldColorAttribute"/> class.
        /// </summary>
        /// <param name="r">The red component of the color (0-255).</param>
        /// <param name="g">The green component of the color (0-255).</param>
        /// <param name="b">The blue component of the color (0-255).</param>
        public FieldColorAttribute(int r, int g, int b)
        {
            color = new Color32(Convert.ToByte(r), Convert.ToByte(g), Convert.ToByte(b), 255);
        }
    }
}