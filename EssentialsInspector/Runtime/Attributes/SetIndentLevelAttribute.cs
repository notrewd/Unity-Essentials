using UnityEngine;

namespace Essentials.Inspector
{
    /// <summary>
    /// Attribute to set the indentation level for a field in the inspector.
    /// </summary>
    public class SetIndentLevelAttribute : PropertyAttribute
    {
        public readonly int level;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetIndentLevelAttribute"/> class.
        /// </summary>
        /// <param name="level">The indentation level to set. Positive values increase indentation, negative values decrease it.</param>
        public SetIndentLevelAttribute(int level)
        {
            this.level = level;
        }
    }
}