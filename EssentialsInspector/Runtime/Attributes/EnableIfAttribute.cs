using UnityEngine;

namespace Essentials.Inspector
{
    public class EnableIfAttribute : PropertyAttribute
    {
        public readonly string conditionName;
        public readonly object compareValue;
        public readonly object[] compareValues;
        public readonly CompareType compareType = CompareType.All;

        public EnableIfAttribute(string conditionName, object compareValue)
        {
            this.conditionName = conditionName;
            this.compareValue = compareValue;
        }

        public EnableIfAttribute(string conditionName, params object[] compareValues)
        {
            this.conditionName = conditionName;
            this.compareValues = compareValues;
        }

        public EnableIfAttribute(string conditionName, CompareType compareType, params object[] compareValues)
        {
            this.conditionName = conditionName;
            this.compareType = compareType;
            this.compareValues = compareValues;
        }
    }
}