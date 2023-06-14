using Essentials.Core;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public class MaterialVariantInitializer
{
    static MaterialVariantInitializer()
    {
        EditorSceneManager.sceneOpened += OnSceneOpened;
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        Initialize();
    }

    private static void OnSceneOpened(Scene scene, OpenSceneMode mode) => Initialize();

    private static void OnPlayModeStateChanged(PlayModeStateChange stateChange)
    {
        if (stateChange == PlayModeStateChange.EnteredEditMode)
        {
            Initialize();
        }
    }

    private static void Initialize()
    {
        MaterialVariant[] materialVariants = Object.FindObjectsOfType<MaterialVariant>();

        foreach (MaterialVariant materialVariant in materialVariants)
        {
            Renderer renderer = materialVariant.GetComponent<Renderer>();
            MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();

            renderer.GetPropertyBlock(materialPropertyBlock);
            materialPropertyBlock.SetColor("_Color", materialVariant.color);
            renderer.SetPropertyBlock(materialPropertyBlock, materialVariant.materialIndex);
        }
    }
}