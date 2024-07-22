using System;
using Essentials.Internal.Databases;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Essentials.Inspector.Utilities
{
    public class InputPrompt : EditorWindow
    {
        private static EditorWindow _parentWindow;
        private static InputPrompt _currentInstance;

        private string _message;
        private string _defaultValue;
        private string _buttonLabel;
        private Action<string> _onSubmit;

        private Label _messageLabel;
        private TextField _inputField;
        private Button _submitButton;

        public static void ShowWindow(EditorWindow parentWindow, string title, string message, string defaultValue, string buttonLabel, Action<string> onSubmit)
        {
            InputPrompt window = GetWindow<InputPrompt>(true);

            _parentWindow = parentWindow;
            _currentInstance = window;

            window.titleContent = new GUIContent(title);
            window.minSize = new Vector2(300, 100);
            window.maxSize = new Vector2(300, 100);

            window._onSubmit = onSubmit;
            window._message = message;
            window._buttonLabel = buttonLabel;
            window._defaultValue = defaultValue;

            window.ConfigureValues();

            window.Show();
        }

        public static void CleanUp(EditorWindow parentWindow)
        {
            if (_currentInstance != null && _parentWindow == parentWindow) _currentInstance.Close();
        }

        private void CreateGUI()
        {
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.notrewd.essentials/EssentialsInspector/Editor/Utilities/InputPromptDocument.uxml");
            visualTree.CloneTree(rootVisualElement);

            _messageLabel = rootVisualElement.Q<Label>("MessageLabel");
            _inputField = rootVisualElement.Q<TextField>("InputField");
            _submitButton = rootVisualElement.Q<Button>("SubmitButton");

            _submitButton.clicked += Submit;
        }

        private void ConfigureValues()
        {
            _messageLabel.text = _message;
            _inputField.value = _defaultValue;
            _submitButton.text = _buttonLabel;
        }

        private bool CheckInput()
        {
            if (string.IsNullOrEmpty(_inputField.text)) return false;
            return _inputField.text.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) < 0;
        }

        private void Submit()
        {
            if (!CheckInput())
            {
                EditorUtility.DisplayDialog("Error", "Invalid input", "Ok");
                return;
            }

            _onSubmit?.Invoke(_inputField.text);
            Close();
        }

        private void OnDestroy()
        {
            if (_currentInstance == this) _currentInstance = null;
        }
    }
}