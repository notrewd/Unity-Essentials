using UnityEditor;
using UnityEngine;

namespace Essentials.Inspector
{
    [CustomPropertyDrawer(typeof(LabelNameAttribute))]
    public class LabelNamePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            LabelNameAttribute labelNameAttribute = (LabelNameAttribute) attribute;

            GUIContent newLabel = new GUIContent(labelNameAttribute.label);
            
            EditorGUI.PropertyField(position, property, newLabel, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => EditorGUI.GetPropertyHeight(property);
    }
}