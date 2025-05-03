using UnityEngine;

namespace Essentials.Inspector
{
    /// <summary>
    /// Attribute to override the display name of a field in the inspector.
    /// </summary>
    public class LabelNameAttribute : PropertyAttribute
    {
        public readonly string label;

        /// <summary>
        /// Initializes a new instance of the <see cref="LabelNameAttribute"/> class.
        /// </summary>
        /// <param name="label">The custom label to display for the field.</param>
        public LabelNameAttribute(string label) => this.label = label;
    }
}