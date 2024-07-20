using System;
using Essentials.Core.Databases;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Essentials.Internal.Databases
{
    public class DatabaseEditor : EditorWindow
    {
        private DatabaseObject _databaseObject;

        private ToolbarButton _newItemButton;
        private ToolbarButton _deleteItemButton;

        public static void CreateWindow(DatabaseObject databaseObject)
        {
            DatabaseEditor window = CreateInstance<DatabaseEditor>();
            window.titleContent = new GUIContent(databaseObject.name);
            window.minSize = new Vector2(300, 300);

            window.SetDatabaseObject(databaseObject);
            window.Show();
        }

        [OnOpenAsset]
        private static bool OnOpenAsset(int instanceID, int line)
        {
            DatabaseObject databaseObject = EditorUtility.InstanceIDToObject(instanceID) as DatabaseObject;

            if (databaseObject != null)
            {
                CreateWindow(databaseObject);
                return true;
            }

            return false;
        }

        private void CreateGUI()
        {
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.notrewd.essentials/EssentialsCore/Editor/Databases/DatabaseEditorDocument.uxml");
            visualTree.CloneTree(rootVisualElement);

            _newItemButton = rootVisualElement.Q<ToolbarButton>("NewItemButton");
            _deleteItemButton = rootVisualElement.Q<ToolbarButton>("DeleteItemButton");

            ConfigureDatabase();
        }

        private void ConfigureDatabase()
        {
            Attribute[] attributes = Attribute.GetCustomAttributes(_databaseObject.GetType(), true);

            for (int i = 0; i < attributes.Length; i++)
            {
                if (attributes[i] is DatabaseAttribute databaseAttribute)
                {
                    _newItemButton.text = databaseAttribute.NewItemButtonLabel;
                    _deleteItemButton.text = databaseAttribute.DeleteItemButtonLabel;
                }
            }
        }

        public void SetDatabaseObject(DatabaseObject databaseObject) => _databaseObject = databaseObject;
    }
}
