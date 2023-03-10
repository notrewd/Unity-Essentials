using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Essentials.Core.UI
{
    [ExecuteAlways]
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
        private List<GameObject> items = new List<GameObject>();

        private bool changeChildrenOnTransformChildrenChanged = true;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            if (scrollRect != null) scrollRect.onValueChanged.AddListener(OnScroll);
            else if (Application.isPlaying) Debug.LogError("Essentials UI: ScrollRect is not assigned to Optimized Scroll Content");
        }

        public void AddElement(GameObject element)
        {
            changeChildrenOnTransformChildrenChanged = false;

            element.transform.SetParent(transform, false);
            items.Add(element);

            RecalculateLayout();
            if (resizeContent) RecalculateContentSize();
            RecalculateVisibility();

            changeChildrenOnTransformChildrenChanged = true;
        }

        public void AddElements(GameObject[] elements)
        {
            changeChildrenOnTransformChildrenChanged = false;

            for (int i = 0; i < elements.Length; i++)
            {
                elements[i].transform.SetParent(transform, false);
                items.Add(elements[i]);
            }

            RecalculateLayout();
            if (resizeContent) RecalculateContentSize();
            RecalculateVisibility();

            changeChildrenOnTransformChildrenChanged = true;
        }

        public void RemoveElement(GameObject element)
        {
            changeChildrenOnTransformChildrenChanged = false;

            items.Remove(element);
            DestroyImmediate(element);

            RecalculateLayout();
            if (resizeContent) RecalculateContentSize();
            RecalculateVisibility();

            changeChildrenOnTransformChildrenChanged = true;
        }

        public void ClearElements()
        {
            changeChildrenOnTransformChildrenChanged = false;

            for (int i = 0; i < items.Count; i++) Destroy(items[i]);
            items.Clear();

            if (resizeContent) RecalculateContentSize();

            changeChildrenOnTransformChildrenChanged = true;
        }

        private void Update()
        {
            if (Application.isPlaying || layoutType == LayoutType.None || scrollRect == null) return;
            OnTransformChildrenChanged();
        }

        private void OnTransformChildrenChanged()
        {
            if (!changeChildrenOnTransformChildrenChanged || !Application.isPlaying) return;

            items.Clear();
            for (int i = 0; i < transform.childCount; i++) items.Add(transform.GetChild(i).gameObject);

            RecalculateLayout();
            if (resizeContent) RecalculateContentSize();
            RecalculateVisibility();
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

                for (int i = 1; i < items.Count; i++)
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

                for (int i = 1; i < items.Count; i++)
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

                for (int i = 1; i < items.Count; i++)
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

                for (int i = 1; i < items.Count; i++)
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

                for (int i = 1; i < items.Count; i++)
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

                for (int i = 1; i < items.Count; i++)
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

                for (int i = 1; i < items.Count; i++)
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

                for (int i = 1; i < items.Count; i++)
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

                for (int i = 0; i < items.Count; i++)
                {
                    RectTransform item = items[i].GetComponent<RectTransform>();
                    height += item.rect.height * item.localScale.y;
                }

                height += (items.Count - 1) * spacing;
                height += padding.top + padding.bottom;

                rectTransform.anchorMin = new Vector2(0, 1);
                rectTransform.anchorMax = new Vector2(1, 1);

                rectTransform.sizeDelta = new Vector2(0, height);
            }
            else if (layoutType == LayoutType.Horizontal)
            {
                float width = 0;

                for (int i = 0; i < items.Count; i++)
                {
                    RectTransform item = items[i].GetComponent<RectTransform>();
                    width += item.rect.width * item.localScale.x;
                }

                width += (items.Count - 1) * spacing;
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
            for (int i = 0; i < items.Count; i++)
            {
                RectTransform item = items[i].GetComponent<RectTransform>();

                // get the world corners of the item
                item.GetWorldCorners(corners);

                // convert the world corners to local space
                for (int j = 0; j < 4; j++) corners[j] = transform.InverseTransformPoint(corners[j]);

                // create a rect from the corners
                Rect itemRect = new Rect(corners[0], Vector2.zero);
                itemRect.xMin = corners[0].x;
                itemRect.yMin = corners[0].y;
                itemRect.xMax = corners[2].x;
                itemRect.yMax = corners[2].y;

                // check if the item is visible
                items[i].SetActive(visibleArea.Overlaps(itemRect));
            }
        }
    }
}