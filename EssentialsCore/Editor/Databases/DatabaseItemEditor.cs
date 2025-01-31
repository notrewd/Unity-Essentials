using Essentials.Core.Databases;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Essentials.Internal.Databases
{
    [CustomEditor(typeof(DatabaseItem), true)]
    public class DatabaseItemEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();

            InspectorElement.FillDefaultInspector(root, serializedObject, this);

            root.Q<VisualElement>("PropertyField:m_Script").style.display = DisplayStyle.None;

            PropertyField idField = root.Q<PropertyField>("PropertyField:id");
            idField.label = "ID";
            idField.style.paddingBottom = 10;

            return root;
        }
    }
}