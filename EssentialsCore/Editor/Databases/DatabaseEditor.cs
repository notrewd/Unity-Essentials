using System;
using System.Linq;
using Essentials.Core.Databases;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Essentials.Inspector.Utilities;
using System.Collections.Generic;

namespace Essentials.Internal.Databases
{
    public class DatabaseEditor : EditorWindow
    {
        private static Dictionary<DatabaseObject, DatabaseEditor> _openDatabases = new Dictionary<DatabaseObject, DatabaseEditor>();

        private DatabaseObject _databaseObject;
        private Type _databaseType;
        private string _databasePath;
        private string _itemLabel;

        private DatabaseItem _currentItem;

        private ToolbarButton _newItemButton;
        private ToolbarButton _deleteItemButton;

        private ScrollView _itemsView;
        private IMGUIContainer _itemContent;

        public static void CreateWindow(DatabaseObject databaseObject)
        {
            if (_openDatabases.ContainsKey(databaseObject))
            {
                _openDatabases[databaseObject].Focus();
                return;
            }

            DatabaseEditor window = CreateInstance<DatabaseEditor>();
            window.titleContent = new GUIContent(databaseObject.name);
            window.minSize = new Vector2(300, 300);

            window.ConfigureWindow(databaseObject);
            window.Show();

            _openDatabases.Add(databaseObject, window);
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
            if (_databaseObject == null)
            {
                Close();
                return;
            }

            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.notrewd.essentials/EssentialsCore/Editor/Databases/DatabaseEditorDocument.uxml");
            visualTree.CloneTree(rootVisualElement);

            _newItemButton = rootVisualElement.Q<ToolbarButton>("NewItemButton");
            _deleteItemButton = rootVisualElement.Q<ToolbarButton>("DeleteItemButton");

            _newItemButton.text = $"New {_itemLabel}";
            _newItemButton.clicked += ShowNewItemPrompt;

            _deleteItemButton.text = $"Delete {_itemLabel}";
            _deleteItemButton.clicked += DeleteCurrentItem;

            _itemsView = rootVisualElement.Q<ScrollView>("ItemsView");
            _itemContent = rootVisualElement.Q<IMGUIContainer>("ItemContent");

            RefreshDeleteButton();
            RefreshItemList();
        }

        private void ShowNewItemPrompt()
        {
            InputPrompt.ShowWindow($"New {_itemLabel}", $"Enter the Name of the New {_itemLabel}", $"New {_itemLabel}", "Create", name => CreateNewItem(name));
        }

        private void ShowRenameItemPrompt(DatabaseItem databaseItem)
        {
            InputPrompt.ShowWindow($"Rename {_itemLabel}", $"Enter the New Name for '{databaseItem.name}'", databaseItem.name, "Rename", name => RenameItem(databaseItem, name));
        }

        private void CreateNewItem(string itemName)
        {
            ScriptableObject scriptableObject = CreateInstance(_databaseType);
            scriptableObject.name = itemName;

            AssetDatabase.AddObjectToAsset(scriptableObject, _databasePath);
            AssetDatabase.SaveAssets();

            RefreshItemList();
        }

        private void RenameItem(DatabaseItem databaseItem, string newName)
        {
            databaseItem.name = newName;

            AssetDatabase.SaveAssets();
            RefreshItemList();
        }

        private void RefreshItemList()
        {
            _itemsView.Clear();

            foreach (object item in AssetDatabase.LoadAllAssetRepresentationsAtPath(_databasePath))
            {
                DatabaseItem databaseItem = item as DatabaseItem;
                if (databaseItem != null) CreateItemOption(databaseItem, databaseItem == _currentItem);
            }
        }

        private void CreateItemOption(DatabaseItem databaseItem, bool selected)
        {
            Button option = new Button
            {
                text = databaseItem.name
            };

            option.AddToClassList("item-entry");
            if (selected) option.AddToClassList("selected");

            option.clicked += () => SelectItem(option, databaseItem);

            option.RegisterCallback<MouseUpEvent>(e =>
            {
                if (e.button == 1)
                {
                    ShowItemContextMenu(databaseItem);
                    e.StopPropagation();
                }
            });

            _itemsView.Add(option);
        }


        private void DeleteItem(DatabaseItem databaseItem)
        {
            if (databaseItem == null) return;
            if (!EditorUtility.DisplayDialog($"Delete {databaseItem.name}", $"Are you sure you want to delete '{databaseItem.name}'?", "Yes", "No")) return;

            if (_currentItem == databaseItem) DeselectCurrentItem();

            AssetDatabase.RemoveObjectFromAsset(databaseItem);
            AssetDatabase.SaveAssets();

            RefreshItemList();
        }

        private void ShowItemContextMenu(DatabaseItem databaseItem)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent($"Rename {databaseItem.name}"), false, () => ShowRenameItemPrompt(databaseItem));
            menu.AddItem(new GUIContent($"Delete {databaseItem.name}"), false, () => DeleteItem(databaseItem));
            menu.ShowAsContext();
        }

        private void SelectItem(Button button, DatabaseItem databaseItem)
        {
            _currentItem = databaseItem;

            foreach (Button child in _itemsView.Children().Cast<Button>()) child.RemoveFromClassList("selected");
            button.AddToClassList("selected");

            RefreshDeleteButton();
            ShowCurrentItem();
        }

        private void ShowCurrentItem()
        {
            if (_currentItem == null)
            {
                _itemContent.onGUIHandler = null;
                return;
            }

            _itemContent.onGUIHandler = () => Editor.CreateEditor(_currentItem).OnInspectorGUI();
        }

        private void DeselectCurrentItem()
        {
            _currentItem = null;

            foreach (Button child in _itemsView.Children().Cast<Button>()) child.RemoveFromClassList("selected");

            RefreshDeleteButton();
            ShowCurrentItem();
        }

        private void DeleteCurrentItem() => DeleteItem(_currentItem);

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

        private void OnDestroy()
        {
            if (_openDatabases.ContainsKey(_databaseObject)) _openDatabases.Remove(_databaseObject);
        }
    }
}
