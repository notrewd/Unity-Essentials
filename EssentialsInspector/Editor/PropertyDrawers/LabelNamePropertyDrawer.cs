using UnityEditor;
using UnityEngine;

namespace Essentials.Inspector
{
    [CustomPropertyDrawer(typeof(LabelNameAttribute))]
    public class LabelNamePropertyDrawer : PropertyDrawer
    {
        /// <summary>
        /// Draws the property field using the custom label specified by the LabelNameAttribute.
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the property GUI.</param>
        /// <param name="property">The SerializedProperty to make the custom GUI for.</param>
        /// <param name="label">The original label of this property (ignored).</param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            LabelNameAttribute labelNameAttribute = (LabelNameAttribute)attribute;

            GUIContent newLabel = new GUIContent(labelNameAttribute.label);

            EditorGUI.PropertyField(position, property, newLabel, true);
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