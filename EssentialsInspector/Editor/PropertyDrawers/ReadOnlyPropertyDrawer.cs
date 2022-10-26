using UnityEditor;
using UnityEngine;

namespace Essentials.Inspector
{
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false;

            EditorGUI.PropertyField(position, property, label, true);

            GUI.enabled = true;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => EditorGUI.GetPropertyHeight(property);
    }
}
