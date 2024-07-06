using Essentials.Serialization;
using UnityEditor;
using UnityEngine;

namespace Essentials.Inspector
{
    [CustomPropertyDrawer(typeof(DisableIfAttribute))]
    public class DisableIfPropertyDrawer : PropertyDrawer
    {
        private bool _isDisabled;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            DisableIfAttribute disableIfAttribute = (DisableIfAttribute)attribute;
            SerializedProperty condition = property.serializedObject.FindProperty(disableIfAttribute.conditionName);

            if (condition == null)
            {
                Debug.LogError("Essentials Inspector: DisableIf attribute has no condition.");
                return;
            }

            if (disableIfAttribute.compareValue == null && disableIfAttribute.compareValues == null)
            {
                Debug.LogError("Essentials Inspector: DisableIf attribute has no compare value.");
                return;
            }

            if (disableIfAttribute.compareValues != null)
            {
                Serialization.CompareType compareType = EssentialsSerialization.ConvertInspectorCompareTypeToSerialized(disableIfAttribute.compareType);
                _isDisabled = EssentialsSerialization.CompareValues(condition, disableIfAttribute.compareValues, compareType);
            }
            else _isDisabled = EssentialsSerialization.CompareValues(condition, disableIfAttribute.compareValue);

            if (_isDisabled) GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;
        }
    }
}