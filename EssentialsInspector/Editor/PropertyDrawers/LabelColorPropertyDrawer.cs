using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(LabelColorAttribute))]
public class LabelColorPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        LabelColorAttribute labelColorAttribute = (LabelColorAttribute)attribute;

        GUI.contentColor = labelColorAttribute.color;

        EditorGUI.PropertyField(position, property, label);

        GUI.contentColor = Color.white;
    }
}