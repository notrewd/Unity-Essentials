using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Essentials.Inspector.Utilities
{
    public class InputPrompt : EditorWindow
    {
        private string _message;
        private string _defaultValue;
        private string _buttonLabel;
        private Action<string> _onSubmit;

        private Label _messageLabel;
        private TextField _inputField;
        private Button _submitButton;

        public static void ShowWindow(string title, string message, string defaultValue, string buttonLabel, Action<string> onSubmit)
        {
            InputPrompt window = GetWindow<InputPrompt>(true);

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
    }
}