using Essentials.Serialization;
using UnityEditor;
using UnityEngine;

namespace Essentials.Inspector
{
    /// <summary>
    /// Custom property drawer for the HideIfAttribute. Hides the property if the specified condition is met.
    /// </summary>
    [CustomPropertyDrawer(typeof(HideIfAttribute))]
    public class HideIfPropertyDrawer : PropertyDrawer
    {
        private bool _isHidden;

        /// <summary>
        /// Draws the property field only if the condition specified by the HideIfAttribute is NOT met.
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the property GUI.</param>
        /// <param name="property">The SerializedProperty to make the custom GUI for.</param>
        /// <param name="label">The label of this property.</param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            HideIfAttribute hideIfAttribute = (HideIfAttribute)attribute;
            SerializedProperty condition = property.serializedObject.FindProperty(hideIfAttribute.conditionName);

            if (condition == null)
            {
                Debug.LogError("Essentials Inspector: HideIf attribute has no condition.");
                return;
            }

            if (hideIfAttribute.compareValue == null && hideIfAttribute.compareValues == null)
            {
                Debug.LogError("Essentials Inspector: HideIf attribute has no compare value.");
                return;
            }

            if (hideIfAttribute.compareValues != null)
            {
                Serialization.CompareType compareType = EssentialsSerialization.ConvertInspectorCompareTypeToSerialized(hideIfAttribute.compareType);
                _isHidden = EssentialsSerialization.CompareValues(condition, hideIfAttribute.compareValues, compareType);
            }
            else _isHidden = EssentialsSerialization.CompareValues(condition, hideIfAttribute.compareValue);

            if (!_isHidden) EditorGUI.PropertyField(position, property, label, true);
        }

        /// <summary>
        /// Gets the height required to draw the property.
        /// Returns the standard property height if not hidden, otherwise returns a negative spacing to hide it effectively.
        /// </summary>
        /// <param name="property">The SerializedProperty to make the custom GUI for.</param>
        /// <param name="label">The label of this property.</param>
        /// <returns>The height required for the property GUI.</returns>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!_isHidden) return EditorGUI.GetPropertyHeight(property);
            return -EditorGUIUtility.standardVerticalSpacing;
        }
    }
}