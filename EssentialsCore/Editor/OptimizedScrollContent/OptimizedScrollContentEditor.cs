using System;
using Essentials.Core.UI;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

[CustomEditor(typeof(OptimizedScrollContent))]
public class OptimizedScrollContentEditor : Editor
{
    private EnumField layoutType;
    private Foldout layoutSettings;
    private EnumField verticalLayoutAlignment;
    private EnumField horizontalLayoutAlignment;
    private FloatField spacing;
    private Toggle resizeContent;
    private Toggle stretchChildren;
    private PropertyField padding;

    public override VisualElement CreateInspectorGUI()
    {
        VisualElement root = new VisualElement();
        VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.notrewd.essentials/EssentialsCore/Editor/OptimizedScrollContent/OptimizedScrollContentDocument.uxml");
        visualTree.CloneTree(root);

        layoutType = root.Q<EnumField>("LayoutType");
        layoutSettings = root.Q<Foldout>("LayoutSettings");
        verticalLayoutAlignment = layoutSettings.Q<EnumField>("VerticalLayoutAlignment");
        horizontalLayoutAlignment = layoutSettings.Q<EnumField>("HorizontalLayoutAlignment");
        spacing = layoutSettings.Q<FloatField>("Spacing");
        resizeContent = layoutSettings.Q<Toggle>("ResizeContent");
        stretchChildren = layoutSettings.Q<Toggle>("StretchChildren");
        padding = root.Q<PropertyField>("Padding");

        layoutType.Init(OptimizedScrollContent.LayoutType.None);
        layoutType.RegisterCallback<ChangeEvent<Enum>>(OnLayoutTypeChanged);

        verticalLayoutAlignment.Init(OptimizedScrollContent.VerticalLayoutAlignment.Left);
        verticalLayoutAlignment.style.display = ((OptimizedScrollContent)target).layoutType == OptimizedScrollContent.LayoutType.Vertical ? DisplayStyle.Flex : DisplayStyle.None;
        verticalLayoutAlignment.SetEnabled(!((OptimizedScrollContent)target).stretchChildren);

        horizontalLayoutAlignment.Init(OptimizedScrollContent.HorizontalLayoutAlignment.Top);
        horizontalLayoutAlignment.style.display = ((OptimizedScrollContent)target).layoutType == OptimizedScrollContent.LayoutType.Horizontal ? DisplayStyle.Flex : DisplayStyle.None;
        horizontalLayoutAlignment.SetEnabled(!((OptimizedScrollContent)target).stretchChildren);

        layoutSettings.style.display = ((OptimizedScrollContent)target).layoutType != OptimizedScrollContent.LayoutType.None ? DisplayStyle.Flex : DisplayStyle.None;

        stretchChildren.RegisterCallback<ChangeEvent<bool>>(OnStretchChildrenChanged);

        padding.style.display = ((OptimizedScrollContent)target).layoutType != OptimizedScrollContent.LayoutType.None ? DisplayStyle.Flex : DisplayStyle.None;

        return root;
    }

    private void OnLayoutTypeChanged(ChangeEvent<Enum> evt)
    {
        OptimizedScrollContent.LayoutType value = (OptimizedScrollContent.LayoutType)evt.newValue;
        layoutSettings.style.display = value != OptimizedScrollContent.LayoutType.None ? DisplayStyle.Flex : DisplayStyle.None;
        verticalLayoutAlignment.style.display = value == OptimizedScrollContent.LayoutType.Vertical ? DisplayStyle.Flex : DisplayStyle.None;
        horizontalLayoutAlignment.style.display = value == OptimizedScrollContent.LayoutType.Horizontal ? DisplayStyle.Flex : DisplayStyle.None;
        padding.style.display = value != OptimizedScrollContent.LayoutType.None ? DisplayStyle.Flex : DisplayStyle.None;
    }

    private void OnStretchChildrenChanged(ChangeEvent<bool> evt)
    {
        verticalLayoutAlignment.SetEnabled(!evt.newValue);
        horizontalLayoutAlignment.SetEnabled(!evt.newValue);
    }
}