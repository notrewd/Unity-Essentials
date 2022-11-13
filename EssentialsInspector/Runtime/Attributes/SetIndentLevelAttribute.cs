using UnityEngine;

namespace Essentials.Inspector
{
    public class SetIndentLevelAttribute : PropertyAttribute
    {
        public readonly int level;

        public SetIndentLevelAttribute(int level)
        {
            this.level = level;
        }
    }
}