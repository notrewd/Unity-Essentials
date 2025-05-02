using UnityEditor;
using UnityEngine;

namespace Essentials.Inspector
{
    [CustomPropertyDrawer(typeof(LabelColorAttribute))]
    public class LabelColorPropertyDrawer : PropertyDrawer
    {
        /// <summary>
        /// Draws the property field with the label colored according to the LabelColorAttribute.
        /// Resets the GUI content color afterwards.
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the property GUI.</param>
        /// <param name="property">The SerializedProperty to make the custom GUI for.</param>
        /// <param name="label">The label of this property.</param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            LabelColorAttribute labelColorAttribute = (LabelColorAttribute)attribute;

            GUI.contentColor = labelColorAttribute.color;

            EditorGUI.PropertyField(position, property, label, true);

            GUI.contentColor = Color.white;
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