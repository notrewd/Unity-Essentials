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

    private RectTransform rectTransform;
    private GameObject[] items = new GameObject[0];

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
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
        if (resizeContent) RecalculateContentSize();
    }

    private void OnRectTransformDimensionsChange()
    {
        if (stretchChildren) RecalculateLayout();
    }

    private void OnScroll(Vector2 pos)
    {
        RecalculateVisibility();
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
        if (transform.childCount == 0) return;

        if (stretchChildren)
        {
            RectTransform firstItem = items[0].GetComponent<RectTransform>();
            firstItem.anchorMin = new Vector2(0, 1);
            firstItem.anchorMax = new Vector2(0, 1);
            firstItem.pivot = new Vector2(0, 1);
            firstItem.anchoredPosition = new Vector2(padding.left, -padding.top);
            firstItem.sizeDelta = new Vector2(firstItem.sizeDelta.x, scrollRect.viewport.rect.height / firstItem.localScale.y - padding.top / firstItem.localScale.y - padding.bottom / firstItem.localScale.y);

            for (int i = 1; i < items.Length; i++)
            {
                RectTransform item = items[i].GetComponent<RectTransform>();
                RectTransform prevItem = items[i - 1].GetComponent<RectTransform>();

                item.anchorMin = new Vector2(0, 1);
                item.anchorMax = new Vector2(0, 1);
                item.pivot = new Vector2(0, 1);
                item.anchoredPosition = new Vector2(prevItem.anchoredPosition.x + prevItem.rect.width * prevItem.localScale.x + spacing, -padding.top);
                item.sizeDelta = new Vector2(item.sizeDelta.x, scrollRect.viewport.rect.height / item.localScale.y - padding.top / firstItem.localScale.y - padding.bottom / firstItem.localScale.y);
            }

            return;
        }

        if (horizontalLayoutAlignment == HorizontalLayoutAlignment.Top)
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
                item.anchoredPosition = new Vector2(prevItem.anchoredPosition.x + prevItem.rect.width * prevItem.localScale.x + spacing, -padding.top);
            }
        }
        else if (horizontalLayoutAlignment == HorizontalLayoutAlignment.Center)
        {
            RectTransform firstItem = items[0].GetComponent<RectTransform>();
            firstItem.anchorMin = new Vector2(0, 0.5f);
            firstItem.anchorMax = new Vector2(0, 0.5f);
            firstItem.pivot = new Vector2(0, 0.5f);
            firstItem.anchoredPosition = new Vector2(padding.left, 0);

            for (int i = 1; i < items.Length; i++)
            {
                RectTransform item = items[i].GetComponent<RectTransform>();
                RectTransform prevItem = items[i - 1].GetComponent<RectTransform>();

                item.anchorMin = new Vector2(0, 0.5f);
                item.anchorMax = new Vector2(0, 0.5f);
                item.pivot = new Vector2(0, 0.5f);
                item.anchoredPosition = new Vector2(prevItem.anchoredPosition.x + prevItem.rect.width * prevItem.localScale.x + spacing, 0);
            }
        }
        else if (horizontalLayoutAlignment == HorizontalLayoutAlignment.Bottom)
        {
            RectTransform firstItem = items[0].GetComponent<RectTransform>();
            firstItem.anchorMin = new Vector2(0, 0);
            firstItem.anchorMax = new Vector2(0, 0);
            firstItem.pivot = new Vector2(0, 0);
            firstItem.anchoredPosition = new Vector2(padding.left, padding.bottom);

            for (int i = 1; i < items.Length; i++)
            {
                RectTransform item = items[i].GetComponent<RectTransform>();
                RectTransform prevItem = items[i - 1].GetComponent<RectTransform>();

                item.anchorMin = new Vector2(0, 0);
                item.anchorMax = new Vector2(0, 0);
                item.pivot = new Vector2(0, 0);
                item.anchoredPosition = new Vector2(prevItem.anchoredPosition.x + prevItem.rect.width * prevItem.localScale.x + spacing, padding.bottom);
            }
        }
    }

    private void RecalculateContentSize()
    {
        if (layoutType == LayoutType.Vertical)
        {
            float height = 0;

            for (int i = 0; i < items.Length; i++)
            {
                RectTransform item = items[i].GetComponent<RectTransform>();
                height += item.rect.height * item.localScale.y;
            }

            height += (items.Length - 1) * spacing;
            height += padding.top + padding.bottom;

            rectTransform.anchorMin = new Vector2(0, 1);
            rectTransform.anchorMax = new Vector2(1, 1);

            rectTransform.sizeDelta = new Vector2(0, height);
        }
        else if (layoutType == LayoutType.Horizontal)
        {
            float width = 0;

            for (int i = 0; i < items.Length; i++)
            {
                RectTransform item = items[i].GetComponent<RectTransform>();
                width += item.rect.width * item.localScale.x;
            }

            width += (items.Length - 1) * spacing;
            width += padding.left + padding.right;

            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(0, 1);

            rectTransform.sizeDelta = new Vector2(width, 0);
        }
    }

    private void RecalculateVisibility()
    {
        // calculate the visible area of the scroll rect using inverse transform point
        Vector3[] corners = new Vector3[4];
        scrollRect.viewport.GetWorldCorners(corners);

        for (int i = 0; i < 4; i++) corners[i] = transform.InverseTransformPoint(corners[i]);

        Rect visibleArea = new Rect(corners[0], Vector2.zero);
        visibleArea.xMin = corners[0].x;
        visibleArea.yMin = corners[0].y;
        visibleArea.xMax = corners[2].x;
        visibleArea.yMax = corners[2].y;

        // check if the item is visible and set the appropriate flag
        for (int i = 0; i < items.Length; i++)
        {
            RectTransform item = items[i].GetComponent<RectTransform>();
            Rect itemRect = new Rect(item.anchoredPosition, item.rect.size * item.localScale);

            if (layoutType == LayoutType.Vertical) itemRect.y -= item.rect.height * item.localScale.y;
            else if (layoutType == LayoutType.Horizontal) itemRect.x -= item.rect.width * item.localScale.x;

            items[i].SetActive(visibleArea.Overlaps(itemRect));
        }
    }
}