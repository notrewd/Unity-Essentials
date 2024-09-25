using UnityEditor;
using UnityEngine.UIElements;

[CustomEditor(typeof(EssentialsSensors))]
[CanEditMultipleObjects]
public class EssentialsSensorsEditor : Editor
{
    public override VisualElement CreateInspectorGUI()
    {
        VisualElement root = new VisualElement();

        VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.notrewd.essentials/EssentialsCore/Editor/Sensors/EssentialsSensorsEditorDocument.uxml");
        visualTree.CloneTree(root);

        return root;
    }
}
