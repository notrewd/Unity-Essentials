using UnityEditor;
using UnityEngine;

namespace Essentials.Inspector
{
    [CustomPropertyDrawer(typeof(LabelColorAttribute))]
    public class LabelColorPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            LabelColorAttribute labelColorAttribute = (LabelColorAttribute) attribute;

            GUI.contentColor = labelColorAttribute.color;

            EditorGUI.PropertyField(position, property, label, true);

            GUI.contentColor = Color.white;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => EditorGUI.GetPropertyHeight(property);
    }

}