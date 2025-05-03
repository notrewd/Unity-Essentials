using Essentials.Serialization;
using UnityEditor;
using UnityEngine;

namespace Essentials.Inspector
{
    [CustomPropertyDrawer(typeof(ShowIfAttribute))]
    public class ShowIfPropertyDrawer : PropertyDrawer
    {
        private bool _isShown;

        /// <summary>
        /// Draws the property field only if the condition specified by the ShowIfAttribute is met.
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the property GUI.</param>
        /// <param name="property">The SerializedProperty to make the custom GUI for.</param>
        /// <param name="label">The label of this property.</param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ShowIfAttribute showIfAttribute = (ShowIfAttribute)attribute;
            SerializedProperty condition = property.serializedObject.FindProperty(showIfAttribute.conditionName);

            if (condition == null)
            {
                Debug.LogError("Essentials Inspector: ShowIf attribute has no condition.");
                return;
            }

            if (showIfAttribute.compareValue == null && showIfAttribute.compareValues == null)
            {
                Debug.LogError("Essentials Inspector: ShowIf attribute has no compare value.");
                return;
            }

            if (showIfAttribute.compareValues != null)
            {
                Serialization.CompareType compareType = EssentialsSerialization.ConvertInspectorCompareTypeToSerialized(showIfAttribute.compareType);
                _isShown = EssentialsSerialization.CompareValues(condition, showIfAttribute.compareValues, compareType);
            }
            else _isShown = EssentialsSerialization.CompareValues(condition, showIfAttribute.compareValue);

            if (_isShown) EditorGUI.PropertyField(position, property, label, true);
        }

        /// <summary>
        /// Gets the height required to draw the property.
        /// Returns the standard property height if shown, otherwise returns a negative spacing to hide it effectively.
        /// </summary>
        /// <param name="property">The SerializedProperty to make the custom GUI for.</param>
        /// <param name="label">The label of this property.</param>
        /// <returns>The height required for the property GUI.</returns>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (_isShown) return EditorGUI.GetPropertyHeight(property);
            return -EditorGUIUtility.standardVerticalSpacing;
        }
    }
}