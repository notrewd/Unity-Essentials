using System;
using System.Linq;
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
        private Type _databaseType;
        private string _databasePath;
        private string _itemLabel;

        private DatabaseItem _currentItem;

        private ToolbarButton _newItemButton;
        private ToolbarButton _deleteItemButton;

        private ScrollView _itemsView;

        public static void CreateWindow(DatabaseObject databaseObject)
        {
            DatabaseEditor window = CreateInstance<DatabaseEditor>();
            window.titleContent = new GUIContent(databaseObject.name);
            window.minSize = new Vector2(300, 300);

            window.ConfigureWindow(databaseObject);
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

            _newItemButton.text = $"New {_itemLabel}";
            _newItemButton.clicked += CreateNewItem;

            _deleteItemButton.text = $"Delete {_itemLabel}";
            _deleteItemButton.clicked += DeleteCurrentItem;

            _itemsView = rootVisualElement.Q<ScrollView>("ItemsView");

            RefreshDeleteButton();
            RefreshItemList();
        }

        private void CreateNewItem() => NewItemPromptEditor.ShowWindow(this, _databaseType, _databasePath, _itemLabel);

        public void RefreshItemList()
        {
            _itemsView.Clear();

            foreach (object item in AssetDatabase.LoadAllAssetRepresentationsAtPath(_databasePath))
            {
                DatabaseItem databaseItem = item as DatabaseItem;
                if (databaseItem != null) CreateItemOption(databaseItem);
            }
        }

        private void CreateItemOption(DatabaseItem databaseItem)
        {
            Button option = new Button
            {
                text = databaseItem.name
            };

            option.AddToClassList("item-entry");
            option.clicked += () => SelectItem(option, databaseItem);

            _itemsView.Add(option);
        }

        private void SelectItem(Button button, DatabaseItem databaseItem)
        {
            _currentItem = databaseItem;

            foreach (Button child in _itemsView.Children().Cast<Button>()) child.RemoveFromClassList("selected");
            button.AddToClassList("selected");

            RefreshDeleteButton();
        }

        private void DeselectItem()
        {
            _currentItem = null;

            foreach (Button child in _itemsView.Children().Cast<Button>()) child.RemoveFromClassList("selected");

            RefreshDeleteButton();
        }

        private void DeleteCurrentItem()
        {
            if (_currentItem == null) return;
            if (!EditorUtility.DisplayDialog($"Delete {_itemLabel}", $"Are you sure you want to delete '{_currentItem.name}'?", "Yes", "No")) return;

            AssetDatabase.RemoveObjectFromAsset(_currentItem);
            AssetDatabase.SaveAssets();

            DeselectItem();
            RefreshItemList();
        }

        private void RefreshDeleteButton()
        {
            if (_currentItem == null) _deleteItemButton.SetEnabled(false);
            else _deleteItemButton.SetEnabled(true);
        }

        private void ConfigureWindow(DatabaseObject databaseObject)
        {
            _databaseObject = databaseObject;
            _databasePath = AssetDatabase.GetAssetPath(_databaseObject);

            Attribute[] attributes = Attribute.GetCustomAttributes(_databaseObject.GetType(), true);

            for (int i = 0; i < attributes.Length; i++)
            {
                if (attributes[i] is DatabaseAttribute databaseAttribute)
                {
                    _databaseType = databaseAttribute.databaseType;
                    _itemLabel = databaseAttribute.itemLabel;
                }
            }
        }
    }
}
