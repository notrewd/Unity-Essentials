using Essentials.Serialization;
using UnityEditor;
using UnityEngine;

namespace Essentials.Inspector.Editor
{
    [CustomPropertyDrawer(typeof(ShowIfAttribute))]
    public class ShowIfPropertyDrawer : PropertyDrawer
    {
        private bool _isShown;

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

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (_isShown) return EditorGUI.GetPropertyHeight(property);
            return -EditorGUIUtility.standardVerticalSpacing;
        }
    }
}