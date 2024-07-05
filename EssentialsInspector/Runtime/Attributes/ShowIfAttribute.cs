using UnityEngine;

namespace Essentials.Inspector
{
    public class ShowIfAttribute : PropertyAttribute
    {
        public readonly string conditionName;
        public readonly object compareValue;
        public readonly object[] compareValues;
        public readonly CompareType compareType = CompareType.All;

        public ShowIfAttribute(string conditionName, object compareValue)
        {
            this.conditionName = conditionName;
            this.compareValue = compareValue;
        }

        public ShowIfAttribute(string conditionName, params object[] compareValues)
        {
            this.conditionName = conditionName;
            this.compareValues = compareValues;
        }

        public ShowIfAttribute(string conditionName, CompareType compareType, params object[] compareValues)
        {
            this.conditionName = conditionName;
            this.compareType = compareType;
            this.compareValues = compareValues;
        }
    }

    public enum CompareType
    {
        All,
        Any
    }
}