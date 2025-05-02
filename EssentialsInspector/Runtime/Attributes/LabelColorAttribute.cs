using System;
using UnityEngine;

namespace Essentials.Inspector
{
    /// <summary>
    /// Attribute to change the color of a field's label in the inspector.
    /// </summary>
    public class LabelColorAttribute : PropertyAttribute
    {
        public readonly Color color;

        /// <summary>
        /// Initializes a new instance of the <see cref="LabelColorAttribute"/> class.
        /// </summary>
        /// <param name="r">The red component of the color (0-255).</param>
        /// <param name="g">The green component of the color (0-255).</param>
        /// <param name="b">The blue component of the color (0-255).</param>
        public LabelColorAttribute(int r, int g, int b)
        {
            color = new Color32(Convert.ToByte(r), Convert.ToByte(g), Convert.ToByte(b), 255);
        }
    }
}
