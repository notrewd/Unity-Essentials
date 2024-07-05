using UnityEngine;

namespace Essentials.Inspector
{
    public class ShowIfAttribute : PropertyAttribute
    {
        public readonly string conditionName;
        public readonly object compareValue;
        // public readonly object[] compareValues;
        // public readonly CompareType compareType;

        public ShowIfAttribute(string conditionName, object compareValue)
        {
            this.conditionName = conditionName;
            this.compareValue = compareValue;
        }

        // public ShowIfAttribute(string conditionName, object[] compareValues, CompareType compareType = CompareType.All)
        // {
        //     this.conditionName = conditionName;
        //     this.compareValues = compareValues;
        // }
    }

    // public enum CompareType
    // {
    //     All,
    //     Any
    // }
}