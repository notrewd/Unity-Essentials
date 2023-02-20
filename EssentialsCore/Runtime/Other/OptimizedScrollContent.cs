using System;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class OptimizedScrollContent : MonoBehaviour
{
    public enum LayoutType
    {
        None,
        Vertical,
        Horizontal
    }

    public enum VerticalLayoutAlignment
    {
        Left,
        Center,
        Right
    }

    public enum HorizontalLayoutAlignment
    {
        Top,
        Center,
        Bottom
    }

    [Serializable]
    public struct Padding
    {
        public float left;
        public float right;
        public float top;
        public float bottom;
    }

    public ScrollRect scrollRect;

    public LayoutType layoutType;
    public VerticalLayoutAlignment verticalLayoutAlignment;
    public HorizontalLayoutAlignment horizontalLayoutAlignment;
    public float spacing;
    public bool resizeContent;
    public bool stretchChildren;
    public Padding padding;

    private GameObject[] items;

    private void Awake()
    {
        scrollRect.onValueChanged.AddListener(OnScroll);
    }

    private void OnTransformChildrenChanged()
    {
        items = new GameObject[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            items[i] = transform.GetChild(i).gameObject;
            //items[i].SetActive(false);
        }

        RecalculateLayout();
    }

    private void OnRectTransformDimensionsChange()
    {
        if (stretchChildren) RecalculateLayout();
    }

    private void OnScroll(Vector2 pos)
    {

    }

    private void RecalculateLayout()
    {
        switch (layoutType)
        {
            case LayoutType.None:
                break;
            case LayoutType.Vertical:
                RecalculateLayoutVertical();
                break;
            case LayoutType.Horizontal:
                RecalculateLayoutHorizontal();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void RecalculateLayoutVertical()
    {
        if (transform.childCount == 0) return;

        if (stretchChildren)
        {
            RectTransform firstItem = items[0].GetComponent<RectTransform>();
            firstItem.anchorMin = new Vector2(0, 1);
            firstItem.anchorMax = new Vector2(0, 1);
            firstItem.pivot = new Vector2(0, 1);
            firstItem.anchoredPosition = new Vector2(padding.left, -padding.top);
            firstItem.sizeDelta = new Vector2(scrollRect.viewport.rect.width / firstItem.localScale.x - padding.left / firstItem.localScale.x - padding.right / firstItem.localScale.x, firstItem.sizeDelta.y);

            for (int i = 1; i < items.Length; i++)
            {
                RectTransform item = items[i].GetComponent<RectTransform>();
                RectTransform prevItem = items[i - 1].GetComponent<RectTransform>();

                item.anchorMin = new Vector2(0, 1);
                item.anchorMax = new Vector2(0, 1);
                item.pivot = new Vector2(0, 1);
                item.anchoredPosition = new Vector2(padding.left, prevItem.anchoredPosition.y - prevItem.rect.height * prevItem.localScale.y - spacing);
                item.sizeDelta = new Vector2(scrollRect.viewport.rect.width / item.localScale.x - padding.left / firstItem.localScale.x - padding.right / firstItem.localScale.x, item.sizeDelta.y);
            }

            return;
        }

        if (verticalLayoutAlignment == VerticalLayoutAlignment.Left)
        {
            RectTransform firstItem = items[0].GetComponent<RectTransform>();
            firstItem.anchorMin = new Vector2(0, 1);
            firstItem.anchorMax = new Vector2(0, 1);
            firstItem.pivot = new Vector2(0, 1);
            firstItem.anchoredPosition = new Vector2(padding.left, -padding.top);

            for (int i = 1; i < items.Length; i++)
            {
                RectTransform item = items[i].GetComponent<RectTransform>();
                RectTransform prevItem = items[i - 1].GetComponent<RectTransform>();

                item.anchorMin = new Vector2(0, 1);
                item.anchorMax = new Vector2(0, 1);
                item.pivot = new Vector2(0, 1);
                item.anchoredPosition = new Vector2(padding.left, prevItem.anchoredPosition.y - prevItem.rect.height * prevItem.localScale.y - spacing);
            }
        }
        else if (verticalLayoutAlignment == VerticalLayoutAlignment.Center)
        {
            RectTransform firstItem = items[0].GetComponent<RectTransform>();
            firstItem.anchorMin = new Vector2(0.5f, 1);
            firstItem.anchorMax = new Vector2(0.5f, 1);
            firstItem.pivot = new Vector2(0.5f, 1);
            firstItem.anchoredPosition = new Vector2(0, -padding.top);

            for (int i = 1; i < items.Length; i++)
            {
                RectTransform item = items[i].GetComponent<RectTransform>();
                RectTransform prevItem = items[i - 1].GetComponent<RectTransform>();

                item.anchorMin = new Vector2(0.5f, 1);
                item.anchorMax = new Vector2(0.5f, 1);
                item.pivot = new Vector2(0.5f, 1);
                item.anchoredPosition = new Vector2(0, prevItem.anchoredPosition.y - prevItem.rect.height * prevItem.localScale.y - spacing);
            }
        }
        else if (verticalLayoutAlignment == VerticalLayoutAlignment.Right)
        {
            RectTransform firstItem = items[0].GetComponent<RectTransform>();
            firstItem.anchorMin = new Vector2(1, 1);
            firstItem.anchorMax = new Vector2(1, 1);
            firstItem.pivot = new Vector2(1, 1);
            firstItem.anchoredPosition = new Vector2(-padding.right, -padding.top);

            for (int i = 1; i < items.Length; i++)
            {
                RectTransform item = items[i].GetComponent<RectTransform>();
                RectTransform prevItem = items[i - 1].GetComponent<RectTransform>();

                item.anchorMin = new Vector2(1, 1);
                item.anchorMax = new Vector2(1, 1);
                item.pivot = new Vector2(1, 1);
                item.anchoredPosition = new Vector2(-padding.right, prevItem.anchoredPosition.y - prevItem.rect.height * prevItem.localScale.y - spacing);
            }
        }
    }

    private void RecalculateLayoutHorizontal()
    {

    }
}