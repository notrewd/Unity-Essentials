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

    public GameDirectoriesPopup(string title, string inputFieldText, string confirmButtonText, Action<string> onConfirm)
    {
        this.title = title;
        this.inputFieldText = inputFieldText;
        this.confirmButtonText = confirmButtonText;
        this.onConfirm = onConfirm;
    }

    public override Vector2 GetWindowSize()
    {
        return new Vector2(200, 100);
    }

    public override void OnGUI(Rect rect) { }

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