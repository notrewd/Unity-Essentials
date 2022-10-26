using UnityEngine;

namespace Essentials.Inspector
{
    public class LabelNameAttribute : PropertyAttribute
    {
        public readonly string label;

        public LabelNameAttribute(string label) => this.label = label;
    }
}