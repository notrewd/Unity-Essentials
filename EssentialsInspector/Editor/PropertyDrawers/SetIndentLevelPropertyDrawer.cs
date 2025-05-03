using UnityEditor;
using UnityEngine;


namespace Essentials.Inspector
{
    [CustomPropertyDrawer(typeof(SetIndentLevelAttribute))]
    public class SetIndentLevelPropertyDrawer : PropertyDrawer
    {
        /// <summary>
        /// Draws the property field with an adjusted indentation level based on the SetIndentLevelAttribute.
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the property GUI.</param>
        /// <param name="property">The SerializedProperty to make the custom GUI for.</param>
        /// <param name="label">The label of this property.</param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SetIndentLevelAttribute setIndentLevelAttribute = (SetIndentLevelAttribute)attribute;

            EditorGUI.indentLevel += setIndentLevelAttribute.level;
            EditorGUI.PropertyField(position, property, label);
        }

        /// <summary>
        /// Gets the standard height required to draw the property.
        /// </summary>
        /// <param name="property">The SerializedProperty to make the custom GUI for.</param>
        /// <param name="label">The label of this property.</param>
        /// <returns>The height required for the property GUI.</returns>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => EditorGUI.GetPropertyHeight(property);
    }
}
