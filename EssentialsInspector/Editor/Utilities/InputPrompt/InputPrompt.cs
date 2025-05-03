using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Essentials.Inspector.Utilities
{
    /// <summary>
    /// An editor window that prompts the user for a string input.
    /// </summary>
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

        /// <summary>
        /// Shows the input prompt window.
        /// </summary>
        /// <param name="parentWindow">The parent editor window.</param>
        /// <param name="title">The title of the prompt window.</param>
        /// <param name="message">The message displayed to the user.</param>
        /// <param name="defaultValue">The default value for the input field.</param>
        /// <param name="buttonLabel">The label for the submit button.</param>
        /// <param name="onSubmit">The action to execute when the user submits the input.</param>
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

        /// <summary>
        /// Closes the current input prompt instance if it belongs to the specified parent window.
        /// </summary>
        /// <param name="parentWindow">The parent window to check against.</param>
        public static void CleanUp(EditorWindow parentWindow)
        {
            if (_currentInstance != null && _parentWindow == parentWindow) _currentInstance.Close();
        }

        /// <summary>
        /// Creates the GUI elements for the input prompt window.
        /// </summary>
        private void CreateGUI()
        {
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.notrewd.essentials/EssentialsInspector/Editor/Utilities/InputPrompt/InputPromptDocument.uxml");
            visualTree.CloneTree(rootVisualElement);

            _messageLabel = rootVisualElement.Q<Label>("MessageLabel");
            _inputField = rootVisualElement.Q<TextField>("InputField");
            _submitButton = rootVisualElement.Q<Button>("SubmitButton");

            _submitButton.clicked += Submit;
        }

        /// <summary>
        /// Configures the initial values of the GUI elements (message, input field, button label).
        /// </summary>
        private void ConfigureValues()
        {
            _messageLabel.text = _message;
            _inputField.value = _defaultValue;
            _submitButton.text = _buttonLabel;
        }

        /// <summary>
        /// Validates the user's input, checking if it's empty or contains invalid file name characters.
        /// </summary>
        /// <returns>True if the input is valid, false otherwise.</returns>
        private bool CheckInput()
        {
            if (string.IsNullOrEmpty(_inputField.text)) return false;
            return _inputField.text.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) < 0;
        }

        /// <summary>
        /// Handles the submission of the input. Validates the input and invokes the onSubmit action if valid.
        /// </summary>
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

        /// <summary>
        /// Cleans up the static instance reference when the window is destroyed.
        /// </summary>
        private void OnDestroy()
        {
            if (_currentInstance == this) _currentInstance = null;
        }
    }
}