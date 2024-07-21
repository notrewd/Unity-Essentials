using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Essentials.Internal.Databases
{
    public class NewItemPromptEditor : EditorWindow
    {
        private DatabaseEditor _editorInstance;

        private Type _databaseType;
        private string _databasePath;
        private string _itemLabel;

        private Label _titleLabel;
        private TextField _nameField;
        private Button _createButton;

        public static void ShowWindow(DatabaseEditor editorInstance, Type databaseType, string databasePath, string itemLabel)
        {
            NewItemPromptEditor window = GetWindow<NewItemPromptEditor>(true);
            window.titleContent = new GUIContent($"New {itemLabel}", EditorGUIUtility.IconContent("d_SettingsIcon").image);
            window.minSize = new Vector2(300, 100);
            window.maxSize = new Vector2(300, 100);

            window._editorInstance = editorInstance;

            window.ConfigureWindow(databaseType, databasePath, itemLabel);
            window.Show();
        }

        private void CreateGUI()
        {
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.notrewd.essentials/EssentialsCore/Editor/Databases/NewItemPromptDocument.uxml");
            visualTree.CloneTree(rootVisualElement);

            _titleLabel = rootVisualElement.Q<Label>("TitleLabel");
            _titleLabel.text = $"Enter Name of the New {_itemLabel}";

            _nameField = rootVisualElement.Q<TextField>("NameField");
            _createButton = rootVisualElement.Q<Button>("CreateButton");

            _createButton.clicked += CreateItem;
        }

        private void ConfigureWindow(Type databaseType, string databasePath, string itemName)
        {
            _databaseType = databaseType;
            _databasePath = databasePath;
            _itemLabel = itemName;

            if (_titleLabel != null) _titleLabel.text = $"Enter Name of the New {_itemLabel}";
        }

        private bool CheckItemName(string name)
        {
            if (string.IsNullOrEmpty(name)) return false;
            return name.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) < 0;
        }

        private void CreateItem()
        {
            if (!CheckItemName(_nameField.value)) EditorUtility.DisplayDialog("Invalid Name", "Name contains invalid characters", "OK");

            ScriptableObject scriptableObject = CreateInstance(_databaseType);
            scriptableObject.name = _nameField.value;

            AssetDatabase.AddObjectToAsset(scriptableObject, _databasePath);
            AssetDatabase.SaveAssets();

            _editorInstance.RefreshItemList();

            Close();
        }
    }
}
