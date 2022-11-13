using UnityEditor;
using UnityEngine;


namespace Essentials.Inspector
{
    [CustomPropertyDrawer(typeof(SetIndentLevelAttribute))]
    public class SetIndentLevelPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SetIndentLevelAttribute setIndentLevelAttribute = (SetIndentLevelAttribute) attribute;

            EditorGUI.indentLevel += setIndentLevelAttribute.level;
            EditorGUI.PropertyField(position, property, label);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => EditorGUI.GetPropertyHeight(property);
    }
}
