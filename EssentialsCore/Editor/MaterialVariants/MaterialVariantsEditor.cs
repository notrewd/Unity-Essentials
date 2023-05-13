using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Essentials.Internal.MaterialVariants
{
    public class MaterialVariantsEditor : EditorWindow
    {
        private VisualElement topBar;
        private Label selectedMaterialLabel;
        private Button backButton;
        private VisualElement noMaterialScreen;
        private VisualElement materialScreen;
        private VisualElement materialList;
        private VisualElement materialProperties;
        private ColorField colorField;

        private GameObject selectedObject;

        [MenuItem("Essentials/Material Variants")]
        public static void ShowWindow()
        {
            MaterialVariantsEditor window = GetWindow<MaterialVariantsEditor>();
            window.titleContent = new GUIContent("Material Variants");
            window.minSize = new Vector2(350, 300);
        }

        public void CreateGUI()
        {
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.notrewd.essentials/EssentialsCore/Editor/MaterialVariants/MaterialVariantsEditorDocument.uxml");
            visualTree.CloneTree(rootVisualElement);

            topBar = rootVisualElement.Q<VisualElement>("TopBar");
            selectedMaterialLabel = topBar.Q<Label>("SelectedMaterialLabel");
            backButton = topBar.Q<Button>("BackButton");
            noMaterialScreen = rootVisualElement.Q<VisualElement>("NoMaterialScreen");
            materialScreen = rootVisualElement.Q<VisualElement>("MaterialScreen");
            materialList = materialScreen.Q<VisualElement>("MaterialList");
            materialProperties = rootVisualElement.Q<VisualElement>("MaterialProperties");
            colorField = materialProperties.Q<ColorField>("ColorField");

            backButton.clicked += () =>
            {
                backButton.style.display = DisplayStyle.None;
                materialScreen.style.display = DisplayStyle.Flex;
                materialProperties.style.display = DisplayStyle.None;
                selectedMaterialLabel.text = "None";
            };
        }

        private void OnEnable()
        {
            EditorApplication.update += Update;
        }

        private void OnDisable()
        {
            EditorApplication.update -= Update;
        }

        private void Update()
        {
            if (selectedObject != Selection.activeGameObject)
            {
                selectedObject = Selection.activeGameObject;
                UpdateMaterialList();

                if (selectedObject == null)
                {
                    noMaterialScreen.style.display = DisplayStyle.Flex;
                    materialScreen.style.display = DisplayStyle.None;
                    materialProperties.style.display = DisplayStyle.None;
                    backButton.style.display = DisplayStyle.None;
                    selectedMaterialLabel.text = "None";
                }
                else
                {
                    noMaterialScreen.style.display = DisplayStyle.None;
                    materialScreen.style.display = DisplayStyle.Flex;
                    materialProperties.style.display = DisplayStyle.None;
                    backButton.style.display = DisplayStyle.None;
                    selectedMaterialLabel.text = "None";
                }
            }
        }

        private void UpdateMaterialList()
        {
            materialList.Clear();

            if (selectedObject == null)
            {
                return;
            }

            MeshRenderer[] meshRenderers = selectedObject.GetComponentsInChildren<MeshRenderer>();

            foreach (MeshRenderer meshRenderer in meshRenderers)
            {
                foreach (Material material in meshRenderer.sharedMaterials)
                {
                    if (material == null)
                    {
                        continue;
                    }

                    VisualElement materialElement = new VisualElement();
                    materialElement.style.alignItems = Align.Stretch;
                    materialElement.style.flexDirection = FlexDirection.Row;

                    Label materialName = new Label(material.name);
                    materialName.style.flexGrow = 1;

                    Button materialButton = new Button();
                    materialButton.text = "Select";
                    materialButton.clicked += () => SelectedMaterial(material);

                    materialElement.Add(materialName);
                    materialElement.Add(materialButton);

                    materialList.Add(materialElement);
                }
            }
        }

        private void SelectedMaterial(Material material)
        {
            if (material == null) return;

            selectedMaterialLabel.text = material.name;
            backButton.style.display = DisplayStyle.Flex;
            materialScreen.style.display = DisplayStyle.None;
            materialProperties.style.display = DisplayStyle.Flex;

            colorField.value = material.color;
            colorField.RegisterValueChangedCallback((evt) => material.color = evt.newValue);
        }
    }
}