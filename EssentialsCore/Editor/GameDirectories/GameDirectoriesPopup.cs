using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class GameDirectoriesPopup : PopupWindowContent
{
    private string title;
    private string inputFieldText;
    private string confirmButtonText;
    private Action<string> onConfirm;

    private Label titleLabel;
    private TextField inputField;
    private Button confirmButton;

    /// <summary>
    /// Initializes a new instance of the GameDirectoriesPopup class.
    /// </summary>
    /// <param name="title">The title displayed at the top of the popup.</param>
    /// <param name="inputFieldText">The initial text for the input field.</param>
    /// <param name="confirmButtonText">The text displayed on the confirm button.</param>
    /// <param name="onConfirm">The action to execute when the confirm button is clicked, passing the input field's value.</param>
    public GameDirectoriesPopup(string title, string inputFieldText, string confirmButtonText, Action<string> onConfirm)
    {
        this.title = title;
        this.inputFieldText = inputFieldText;
        this.confirmButtonText = confirmButtonText;
        this.onConfirm = onConfirm;
    }

    /// <summary>
    /// Gets the desired size of the popup window.
    /// </summary>
    /// <returns>The size of the window as a Vector2.</returns>
    public override Vector2 GetWindowSize()
    {
        return new Vector2(200, 100);
    }

    /// <summary>
    /// Called for rendering and handling GUI events using the immediate mode GUI system. (Currently empty)
    /// </summary>
    /// <param name="rect">The area the popup window covers.</param>
    public override void OnGUI(Rect rect) { }

    /// <summary>
    /// Called when the popup window is opened. Sets up the UI elements and event handlers.
    /// </summary>
    public override void OnOpen()
    {
        VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.notrewd.essentials/EssentialsCore/Editor/GameDirectories/GameDirectoriesPopupDocument.uxml");
        visualTree.CloneTree(editorWindow.rootVisualElement);

        titleLabel = editorWindow.rootVisualElement.Q<Label>("TitleLabel");
        inputField = editorWindow.rootVisualElement.Q<TextField>("InputField");
        confirmButton = editorWindow.rootVisualElement.Q<Button>("ConfirmButton");

        titleLabel.text = title;
        inputField.value = inputFieldText;
        confirmButton.text = confirmButtonText;

        inputField.RegisterCallback<KeyDownEvent>(evt =>
        {
            if (evt.keyCode == KeyCode.Return)
            {
                onConfirm(inputField.value);
                editorWindow.Close();
            }
        });

        confirmButton.clickable.clicked += () =>
        {
            onConfirm(inputField.value);
            editorWindow.Close();
        };
    }
}