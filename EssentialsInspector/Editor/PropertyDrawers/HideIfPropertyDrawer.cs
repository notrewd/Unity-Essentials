using Essentials.Serialization;
using UnityEditor;
using UnityEngine;

namespace Essentials.Inspector.Editor
{
    [CustomPropertyDrawer(typeof(HideIfAttribute))]
    public class HideIfPropertyDrawer : PropertyDrawer
    {
        private bool _isHidden;

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

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!_isHidden) return EditorGUI.GetPropertyHeight(property);
            return -EditorGUIUtility.standardVerticalSpacing;
        }
    }
}