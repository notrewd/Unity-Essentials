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

        private ToolbarPopupSearchField _searchField;
        private Button _orderButton;

        private ToolbarButton _newItemButton;
        private ToolbarButton _deleteItemButton;

        private VisualElement _content;

        private ScrollView _itemsView;
        private InspectorElement _itemInspector;

        private SearchType _searchType = SearchType.Name;

        private bool _orderAscending = true;

        private enum SearchType { Name, ID }

        public static void CreateWindow(DatabaseObject databaseObject)
        {
            if (_openDatabases.ContainsKey(databaseObject))
            {
                _openDatabases[databaseObject].Focus();
                return;
            }

            DatabaseEditor window = CreateInstance<DatabaseEditor>();
            window.titleContent = new GUIContent(databaseObject.name, IconDatabase.GetIcon("Database@32"));
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

            _searchField = rootVisualElement.Q<ToolbarPopupSearchField>("SearchField");
            _orderButton = rootVisualElement.Q<Button>("OrderButton");

            _orderButton.style.backgroundImage = _orderAscending ? IconDatabase.GetIcon("Ascending@32") : IconDatabase.GetIcon("Descending@32");
            _orderButton.clicked += SwitchOrder;

            _searchField.menu.AppendAction("Name",
                action =>
                {
                    _searchType = SearchType.Name;
                    RefreshItemList();
                },
                actionStatus => _searchType == SearchType.Name ? DropdownMenuAction.Status.Checked : DropdownMenuAction.Status.Normal);

            _searchField.menu.AppendAction("ID",
                action =>
                {
                    _searchType = SearchType.ID;
                    RefreshItemList();
                },
                actionStatus => _searchType == SearchType.ID ? DropdownMenuAction.Status.Checked : DropdownMenuAction.Status.Normal);

            _searchField.RegisterValueChangedCallback(evt => RefreshItemList());

            _newItemButton = rootVisualElement.Q<ToolbarButton>("NewItemButton");
            _deleteItemButton = rootVisualElement.Q<ToolbarButton>("DeleteItemButton");

            _newItemButton.text = $"New {_itemLabel}";
            _newItemButton.clicked += ShowNewItemPrompt;

            _deleteItemButton.text = $"Delete {_itemLabel}";
            _deleteItemButton.clicked += DeleteCurrentItem;

            _content = rootVisualElement.Q<VisualElement>("Content");

            _itemsView = _content.Q<ScrollView>("ItemsView");

            _itemInspector = new InspectorElement();
            _content.Add(_itemInspector);

            RefreshDeleteButton();
            RefreshItemList();
        }

        private void ShowNewItemPrompt()
        {
            InputPrompt.ShowWindow(this, $"New {_itemLabel}", $"Enter Name of the New {_itemLabel}", $"New {_itemLabel}", "Create", name => CreateNewItem(name));
        }

        private void ShowRenameItemPrompt(DatabaseItem databaseItem)
        {
            InputPrompt.ShowWindow(this, $"Rename {_itemLabel}", $"Enter New Name for '{databaseItem.name}'", databaseItem.name, "Rename", name => RenameItem(databaseItem, name));
        }

        private void CreateNewItem(string itemName)
        {
            ScriptableObject scriptableObject = CreateInstance(_databaseType);
            scriptableObject.name = itemName;

            GenerateIdForItem(scriptableObject as DatabaseItem, _databaseObject);

            AssetDatabase.AddObjectToAsset(scriptableObject, _databasePath);
            AssetDatabase.SaveAssets();

            RefreshItemList();
            RefreshDatabaseItems();
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

            object[] items = AssetDatabase.LoadAllAssetRepresentationsAtPath(_databasePath);
            object[] filteredItems = _searchType == SearchType.Name ? items.Cast<DatabaseItem>().Where(item => item.name.ToLower().Contains(_searchField.value.ToLower())).ToArray() : items.Cast<DatabaseItem>().Where(item => item.id.ToString().Contains(_searchField.value)).ToArray();

            if (!_orderAscending) Array.Reverse(filteredItems);

            foreach (object item in filteredItems)
            {
                DatabaseItem databaseItem = item as DatabaseItem;
                if (databaseItem != null) CreateItemOption(databaseItem, databaseItem == _currentItem);
            }
        }

        private void SwitchOrder()
        {
            _orderAscending = !_orderAscending;

            Texture2D icon = _orderAscending ? IconDatabase.GetIcon("Ascending@32") : IconDatabase.GetIcon("Descending@32");
            _orderButton.style.backgroundImage = icon;

            RefreshItemList();
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
            RefreshDatabaseItems();
        }

        private void RefreshDatabaseItems()
        {
            _databaseObject.items.Clear();

            foreach (DatabaseItem databaseItem in AssetDatabase.LoadAllAssetRepresentationsAtPath(_databasePath).Cast<DatabaseItem>())
            {
                _databaseObject.items.Add(databaseItem);
            }

            EditorUtility.SetDirty(_databaseObject);
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
                _itemInspector.Unbind();
                _itemInspector.Clear();
                return;
            }

            SerializedObject serializedObject = new SerializedObject(_currentItem);
            _itemInspector.Bind(serializedObject);
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

        private static string GenerateIdFromName(string name)
        {
            string[] words = name.Split(' ');
            string id = "";

            for (int i = 0; i < words.Length; i++)
            {
                string word = words[i];
                id += char.ToUpper(word[0]) + word.Substring(1);
            }

            id = char.ToLower(id[0]) + id[1..];
            return id;
        }

        public static void GenerateIdForItem(DatabaseItem databaseItem, DatabaseObject databaseObject)
        {
            SerializedObject serializedObject = new SerializedObject(databaseItem);
            SerializedProperty idProperty = serializedObject.FindProperty("id");

            string id;
            int interations = 0;

            do
            {
                id = GenerateIdFromName(databaseItem.name) + (interations > 0 ? interations.ToString() : "");
                idProperty.stringValue = id;
                interations++;
            }
            while (IdExists(id, databaseObject));

            serializedObject.ApplyModifiedProperties();
        }

        private static bool IdExists(string id, DatabaseObject databaseObject) => databaseObject.items.Exists(item => item.id == id);

        private void OnDestroy()
        {
            InputPrompt.CleanUp(this);
            if (_databaseObject != null && _openDatabases.ContainsKey(_databaseObject)) _openDatabases.Remove(_databaseObject);
        }
    }
}
