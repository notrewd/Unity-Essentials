using UnityEngine;

namespace Essentials.Inspector
{
    public class HideIfAttribute : PropertyAttribute
    {
        public readonly string conditionName;
        public readonly object compareValue;
        public readonly object[] compareValues;
        public readonly CompareType compareType = CompareType.All;

        public HideIfAttribute(string conditionName, object compareValue)
        {
            this.conditionName = conditionName;
            this.compareValue = compareValue;
        }

        public HideIfAttribute(string conditionName, params object[] compareValues)
        {
            this.conditionName = conditionName;
            this.compareValues = compareValues;
        }

        public HideIfAttribute(string conditionName, CompareType compareType, params object[] compareValues)
        {
            this.conditionName = conditionName;
            this.compareType = compareType;
            this.compareValues = compareValues;
        }
    }
}