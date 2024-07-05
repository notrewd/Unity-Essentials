using Essentials.Serialization;
using UnityEngine;
using UnityEditor;

namespace Essentials.Inspector.Editor
{
    [CustomPropertyDrawer(typeof(EnableIfAttribute))]
    public class EnableIfPropertyDrawer : PropertyDrawer
    {
        private bool _isEnabled;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EnableIfAttribute enableIfAttribute = (EnableIfAttribute)attribute;
            SerializedProperty condition = property.serializedObject.FindProperty(enableIfAttribute.conditionName);

            if (condition == null)
            {
                Debug.LogError("Essentials Inspector: EnableIf attribute has no condition.");
                return;
            }

            if (enableIfAttribute.compareValue == null && enableIfAttribute.compareValues == null)
            {
                Debug.LogError("Essentials Inspector: EnableIf attribute has no compare value.");
                return;
            }

            if (enableIfAttribute.compareValues != null)
            {
                Serialization.CompareType compareType = EssentialsSerialization.ConvertInspectorCompareTypeToSerialized(enableIfAttribute.compareType);
                _isEnabled = EssentialsSerialization.CompareValues(condition, enableIfAttribute.compareValues, compareType);
            }
            else _isEnabled = EssentialsSerialization.CompareValues(condition, enableIfAttribute.compareValue);

            if (!_isEnabled) GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;
        }
    }
}