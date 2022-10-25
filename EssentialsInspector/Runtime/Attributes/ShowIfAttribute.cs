using UnityEngine;

public class ShowIfAttribute : PropertyAttribute
{
    public readonly string conditionName;
    public readonly object compareValue;
    public readonly object[] compareValues;

    public ShowIfAttribute(string conditionName, object compareValue)
    {
        this.conditionName = conditionName;
        this.compareValue = compareValue;
    }

    public ShowIfAttribute(string conditionName, object[] compareValues)
    {
        this.conditionName = conditionName;
        this.compareValues = compareValues;
    }
}