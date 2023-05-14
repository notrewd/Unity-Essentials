using UnityEditor;
using Essentials.Core;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEngine;

namespace Essentials.Internal
{
    [CustomEditor(typeof(MaterialVariant))]
    public class MaterialVariantEditor : Editor
    {
        private MaterialVariant materialVariant;
        private Renderer renderer;
        private MaterialPropertyBlock materialPropertyBlock;

        private VisualElement materialSelection;
        private Button previousButton;
        private Label materialLabel;
        private Button nextButton;
        private ColorField colorField;

        public override VisualElement CreateInspectorGUI()
        {
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.notrewd.essentials/EssentialsCore/Editor/MaterialVariant/MaterialVariantEditorDocument.uxml");
            VisualElement root = visualTree.CloneTree();

            materialVariant = (MaterialVariant)target;
            renderer = materialVariant.GetComponent<Renderer>();
            materialPropertyBlock = new MaterialPropertyBlock();

            materialSelection = root.Q<VisualElement>("MaterialSelection");
            previousButton = materialSelection.Q<Button>("PreviousButton");
            materialLabel = materialSelection.Q<Label>("MaterialLabel");
            nextButton = materialSelection.Q<Button>("NextButton");
            colorField = root.Q<ColorField>("ColorField");

            previousButton.clickable.clicked += OnPreviousButtonClicked;
            nextButton.clickable.clicked += OnNextButtonClicked;

            if (renderer.sharedMaterials.Length == 1) materialSelection.style.display = DisplayStyle.None;
            else UpdateMaterialLabel();

            colorField.RegisterValueChangedCallback(OnColorChanged);

            return root;
        }

        private void OnPreviousButtonClicked()
        {
            SerializedProperty materialIndexProperty = serializedObject.FindProperty("materialIndex");
            materialIndexProperty.intValue = Mathf.Clamp(materialIndexProperty.intValue - 1, 0, renderer.sharedMaterials.Length - 1);
            serializedObject.ApplyModifiedProperties();

            UpdateMaterialLabel();
        }

        private void OnNextButtonClicked()
        {
            SerializedProperty materialIndexProperty = serializedObject.FindProperty("materialIndex");
            materialIndexProperty.intValue = Mathf.Clamp(materialIndexProperty.intValue + 1, 0, renderer.sharedMaterials.Length - 1);
            serializedObject.ApplyModifiedProperties();

            UpdateMaterialLabel();
        }

        private void UpdateMaterialLabel() => materialLabel.text = renderer.sharedMaterials[materialVariant.materialIndex].name;

        private void OnColorChanged(ChangeEvent<Color> evt)
        {
            renderer.GetPropertyBlock(materialPropertyBlock, materialVariant.materialIndex);
            materialPropertyBlock.SetColor("_Color", evt.newValue);
            renderer.SetPropertyBlock(materialPropertyBlock, materialVariant.materialIndex);
        }
    }
}