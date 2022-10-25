using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Essentials.Inspector
{
    [CustomPropertyDrawer(typeof(FieldColorAttribute))]
    public class FieldColorPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            FieldColorAttribute fieldColorAttribute = (FieldColorAttribute) attribute;

            GUI.backgroundColor = fieldColorAttribute.color;
                
            EditorGUI.PropertyField(position, property, label);

            GUI.backgroundColor = Color.white;
        }
    }
}