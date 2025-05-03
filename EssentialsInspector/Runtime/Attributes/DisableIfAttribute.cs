using UnityEngine;

namespace Essentials.Inspector
{
    /// <summary>
    /// Attribute to conditionally disable a field in the inspector based on the value of another field.
    /// </summary>
    public class DisableIfAttribute : PropertyAttribute
    {
        public readonly string conditionName;
        public readonly object compareValue;
        public readonly object[] compareValues;
        public readonly CompareType compareType = CompareType.All;

        /// <summary>
        /// Initializes a new instance of the <see cref="DisableIfAttribute"/> class.
        /// Disables the field if the condition field's value equals the compare value.
        /// </summary>
        /// <param name="conditionName">The name of the field to check the condition against.</param>
        /// <param name="compareValue">The value to compare the condition field against.</param>
        public DisableIfAttribute(string conditionName, object compareValue)
        {
            this.conditionName = conditionName;
            this.compareValue = compareValue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DisableIfAttribute"/> class.
        /// Disables the field if the condition field's value equals any of the compare values (using CompareType.All by default).
        /// </summary>
        /// <param name="conditionName">The name of the field to check the condition against.</param>
        /// <param name="compareValues">An array of values to compare the condition field against.</param>
        public DisableIfAttribute(string conditionName, params object[] compareValues)
        {
            this.conditionName = conditionName;
            this.compareValues = compareValues;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DisableIfAttribute"/> class.
        /// Disables the field based on the comparison type (All or Any) against the compare values.
        /// </summary>
        /// <param name="conditionName">The name of the field to check the condition against.</param>
        /// <param name="compareType">The comparison logic (All or Any).</param>
        /// <param name="compareValues">An array of values to compare the condition field against.</param>
        public DisableIfAttribute(string conditionName, CompareType compareType, params object[] compareValues)
        {
            this.conditionName = conditionName;
            this.compareType = compareType;
            this.compareValues = compareValues;
        }
    }
}