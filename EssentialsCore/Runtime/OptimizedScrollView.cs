using System;
using System.Collections.Generic;
using System.Linq;
using Essentials.Inspector;
using UnityEngine;
using UnityEngine.UI;

namespace Essentials.Core.UI
{
    [RequireComponent(typeof(ScrollRect))]
    [DisallowMultipleComponent]
    public class OptimizedScrollView : MonoBehaviour
    {
        public LayoutType layoutType = LayoutType.None;

        [ShowIf(nameof(layoutType), LayoutType.VerticalLayout)]
        public VerticalAlignment verticalAlignment;

        [ShowIf(nameof(layoutType), LayoutType.HorizontalLayout)]
        public HorizontalAlignment horizontalAlignment;

        [ShowIf(nameof(layoutType), LayoutType.None)]
        public ScrollDirection scrollDirection;

        public Vector2 marginOffset = Vector2.zero;

        [ShowIf(nameof(layoutType), new object[] {LayoutType.HorizontalLayout, LayoutType.VerticalLayout})]
        public float spacing;

        [ShowIf(nameof(layoutType), new object[] {LayoutType.HorizontalLayout, LayoutType.VerticalLayout})]
        public bool fitContent;

        [ShowIf(nameof(layoutType), new object[] {LayoutType.HorizontalLayout, LayoutType.VerticalLayout})]
        public bool controlChildSize;

        [ShowIf(nameof(layoutType), new object[] { LayoutType.HorizontalLayout, LayoutType.VerticalLayout })]
        public ChildSizePadding childSizePadding;

        [ShowIf(nameof(layoutType), new object[] {LayoutType.HorizontalLayout, LayoutType.VerticalLayout})]
        public Padding padding;

        [HideInInspector] public List<RectTransform> elements = new List<RectTransform>();

        private ScrollRect scrollRect;
        private GameObject content;

        private float disableMarginX;
        private float disableMarginY;


        private void Awake()
        {
            scrollRect = GetComponent<ScrollRect>();
            content = scrollRect.content.gameObject;
            
            scrollRect.onValueChanged.AddListener(OnScroll);

            if (content.GetComponent<ContentSizeFitter>() != null || content.GetComponent<VerticalLayoutGroup>() != null || content.GetComponent<HorizontalLayoutGroup>() != null || content.GetComponent<GridLayoutGroup>() != null)
            {
                if (content.TryGetComponent(out ContentSizeFitter contentSizeFitter)) contentSizeFitter.enabled = false;
                if (content.TryGetComponent(out VerticalLayoutGroup verticalLayoutGroup)) verticalLayoutGroup.enabled = false;
                if (content.TryGetComponent(out HorizontalLayoutGroup horizontalLayoutGroup)) horizontalLayoutGroup.enabled = false;
                if (content.TryGetComponent(out GridLayoutGroup gridLayoutGroup)) gridLayoutGroup.enabled = false;
            
                Debug.LogError("Essentials Core UI: Optimized Scroll View does not need any layout groups or content size fitter.");
            }

            if (content.transform.childCount >= 1)
            {
                foreach (Transform t in content.transform)
                {
                    if (t.TryGetComponent(out RectTransform rectTransform))
                    {
                        elements.Add(rectTransform);
                    
                        UpdateMarginSize(t.gameObject);
                    }
                }
                
                RecalculateContentSize();
                RecalculateLayout();
                CalculateApproxVisibility();
            }
        }

        public void Instatiate(GameObject element)
        {
            GameObject instance = Instantiate(element, content.transform, false);
            elements.Add(instance.GetComponent<RectTransform>());
            
            UpdateMarginSize(element);

            RecalculateContentSize();
            RecalculateLayout();
            RecalculateVisibility();
        }

        public void Add(GameObject element)
        {
            element.transform.SetParent(content.transform, false);
            elements.Add(element.GetComponent<RectTransform>());

            UpdateMarginSize(element);

            RecalculateContentSize();
            RecalculateLayout();
            RecalculateVisibility();
        }

        public void Remove(GameObject element)
        {
            if (elements.Contains(element.GetComponent<RectTransform>()))
            {
                elements.Remove(element.GetComponent<RectTransform>());
                Destroy(element);

                if (elements.Count >= 1)
                {
                    RecalculateContentSize();
                    RecalculateLayout();
                    RecalculateVisibility();
                }
            }
        }

        public void MoveToFront(GameObject element)
        {
            RectTransform rectTransform = element.GetComponent<RectTransform>();
            
            if (rectTransform == null || elements.Count <= 1 || !elements.Contains(rectTransform)) return;

            elements.Remove(rectTransform);
            elements.Insert(0, rectTransform);
            
            RecalculateLayout();
        }

        public void MoveToBack(GameObject element)
        {
            RectTransform rectTransform = element.GetComponent<RectTransform>();
            
            if (rectTransform == null || elements.Count <= 1 || !elements.Contains(rectTransform)) return;

            elements.Remove(rectTransform);
            elements.Add(rectTransform);
            
            RecalculateLayout();
        }

        public void Clear()
        {
            if (elements.Count == 0) return;

            foreach (RectTransform element in elements) Destroy(element.gameObject);
            
            elements.Clear();

            scrollRect.content.sizeDelta = new Vector2(0, 0);
        }

        public void Recalculate()
        {
            RecalculateContentSize();
            RecalculateLayout();
            RecalculateVisibility();
        }

        public GameObject[] GetAll()
        {
            return elements.Count == 0 ? default : elements.Select(x => x.gameObject).ToArray();
        }

        private void OnScroll(Vector2 pos)
        {
            if (elements.Count == 0) return;
            
            RecalculateVisibility();
        }

        private void UpdateMarginSize(GameObject element)
        {
            if (scrollRect.GetComponent<RectTransform>().rect.width * 0.5f + element.GetComponent<RectTransform>().sizeDelta.x + marginOffset.x > disableMarginX)
                disableMarginX = scrollRect.GetComponent<RectTransform>().rect.width * 0.5f + element.GetComponent<RectTransform>().sizeDelta.x + marginOffset.x;
            if (scrollRect.GetComponent<RectTransform>().rect.height * 0.5f + element.GetComponent<RectTransform>().sizeDelta.y + marginOffset.y > disableMarginY)
                disableMarginY = scrollRect.GetComponent<RectTransform>().rect.height * 0.5f + element.GetComponent<RectTransform>().sizeDelta.y + marginOffset.y;
        }

        private void CalculateApproxVisibility()
        {
            if (elements.Count == 0) return;

            if (IsHorizontal())
            {
                float maxSize = scrollRect.GetComponent<RectTransform>().sizeDelta.x;
                float size = 0f;

                if (elements.Count >= 2)
                {
                    foreach (RectTransform element in elements)
                    {
                        element.gameObject.SetActive(size < maxSize);

                        size += element.sizeDelta.x;
                    }
                }
            }
            
            else if (IsVertical())
            {
                float maxSize = scrollRect.GetComponent<RectTransform>().sizeDelta.y;
                float size = 0f;

                if (elements.Count >= 2)
                {
                    foreach (RectTransform element in elements)
                    {
                        element.gameObject.SetActive(size < maxSize);

                        size += element.sizeDelta.y;
                    }
                }
            }
        }

        private void RecalculateVisibility()
        {
            if (IsHorizontal())
            {
                foreach (RectTransform element in elements)
                {
                    if (IsOutsideOfMargin(element.position, Margin.Left) || IsOutsideOfMargin(element.position, Margin.Right))
                    {
                        element.gameObject.SetActive(false);
                    }
                    else element.gameObject.SetActive(true);
                }
            }
            
            else if (IsVertical())
            {
                foreach (RectTransform element in elements)
                {
                    if (IsOutsideOfMargin(element.position, Margin.Bottom) || IsOutsideOfMargin(element.position, Margin.Top))
                    {
                        element.gameObject.SetActive(false);
                    }
                    else element.gameObject.SetActive(true);
                }
            }
        }

        private void RecalculateLayout()
        {
            switch (layoutType)
            {
                case LayoutType.None:
                    return;
                case LayoutType.VerticalLayout:
                    
                    AlignRectTransform(elements[0]);

                    if (elements.Count >= 2)
                    {
                        Vector2 anchorMin = elements[0].anchorMin;
                        Vector2 anchorMax = elements[0].anchorMax;
                        float positionX = elements[0].anchoredPosition.x;
                        
                        for (int i = 1; i < elements.Count; i++)
                        {
                            elements[i].anchorMin = anchorMin;
                            elements[i].anchorMax = anchorMax;

                            elements[i].anchoredPosition = new Vector2(positionX, elements[i - 1].anchoredPosition.y - elements[i - 1].sizeDelta.y * 0.5f - elements[i].sizeDelta.y * 0.5f - spacing);

                            if (controlChildSize && verticalAlignment == VerticalAlignment.Center) elements[i].sizeDelta = new Vector2(scrollRect.content.rect.width - childSizePadding.leftAndRight, elements[i].sizeDelta.y);
                        }
                    }
                    
                    break;
                case LayoutType.HorizontalLayout:
                    
                    AlignRectTransform(elements[0]);

                    if (elements.Count >= 2)
                    {
                        Vector2 anchorMin = elements[0].anchorMin;
                        Vector2 anchorMax = elements[0].anchorMax;
                        float positionY = elements[0].anchoredPosition.y;

                        for (int i = 1; i < elements.Count; i++)
                        {
                            elements[i].anchorMin = anchorMin;
                            elements[i].anchorMax = anchorMax;

                            elements[i].anchoredPosition = new Vector2( elements[i - 1].anchoredPosition.x + elements[i - 1].sizeDelta.x * 0.5f + elements[i].sizeDelta.x * 0.5f + spacing, positionY);
                            
                            if (controlChildSize && horizontalAlignment == HorizontalAlignment.Center) elements[i].sizeDelta = new Vector2(elements[i].sizeDelta.x, scrollRect.content.rect.height - childSizePadding.topAndBottom);
                        }
                    }
                    
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void RecalculateContentSize()
        {
            if (elements.Count == 0 || !fitContent) return;

            switch (layoutType)
            {
                case LayoutType.None:
                    break;
                case LayoutType.VerticalLayout:

                    float newSizeY = elements[0].sizeDelta.y + padding.top + padding.bottom;

                    if (elements.Count >= 2)
                    {
                        for (int i = 1; i < elements.Count; i++)
                        {
                            newSizeY += elements[i].sizeDelta.y + spacing;
                        }
                    }

                    scrollRect.content.anchorMin = new Vector2(0, 1);
                    scrollRect.content.anchorMax = new Vector2(1, 1);

                    scrollRect.content.sizeDelta = new Vector2(0, newSizeY);
                    
                    break;
                case LayoutType.HorizontalLayout:

                    float newSizeX = elements[0].rect.width + padding.right + padding.left;

                    if (elements.Count >= 2)
                    {
                        for (int i = 1; i < elements.Count; i++)
                        {
                            newSizeX += elements[i].rect.width + spacing;
                        }
                    }

                    scrollRect.content.anchorMin = new Vector2(0, 0);
                    scrollRect.content.anchorMax = new Vector2(0, 1);
                    
                    scrollRect.content.sizeDelta = new Vector2(newSizeX, 0);

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void AlignRectTransform(RectTransform rectTransform)
        {
            if (layoutType == LayoutType.VerticalLayout)
            {
                switch (verticalAlignment)
                {
                    case VerticalAlignment.Left:
                        rectTransform.anchorMin = new Vector2(0, 1);
                        rectTransform.anchorMax = new Vector2(0, 1);
                        rectTransform.anchoredPosition = new Vector2(rectTransform.sizeDelta.x * 0.5f + padding.left, -rectTransform.sizeDelta.y * 0.5f - padding.top);
                        break;
                    case VerticalAlignment.Center:
                        rectTransform.anchorMin = new Vector2(0.5f, 1);
                        rectTransform.anchorMax = new Vector2(0.5f, 1);
                        rectTransform.anchoredPosition = new Vector2(0, -rectTransform.sizeDelta.y * 0.5f - padding.top);
                        break;
                    case VerticalAlignment.Right:
                        rectTransform.anchorMin = new Vector2(1, 1);
                        rectTransform.anchorMax = new Vector2(1, 1);
                        rectTransform.anchoredPosition = new Vector2(-rectTransform.sizeDelta.x * 0.5f - padding.right, -rectTransform.sizeDelta.y * 0.5f - padding.top);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (controlChildSize && verticalAlignment == VerticalAlignment.Center) rectTransform.sizeDelta = new Vector2(scrollRect.content.rect.width - childSizePadding.leftAndRight, rectTransform.sizeDelta.y);
            }
            else if (layoutType == LayoutType.HorizontalLayout)
            {
                switch (horizontalAlignment)
                {
                    case HorizontalAlignment.Top:
                        rectTransform.anchorMin = new Vector2(0, 1);
                        rectTransform.anchorMax = new Vector2(0, 1);
                        rectTransform.anchoredPosition = new Vector2(rectTransform.sizeDelta.x * 0.5f + padding.left, -rectTransform.sizeDelta.y * 0.5f - padding.top);
                        break;
                    case HorizontalAlignment.Center:
                        rectTransform.anchorMin = new Vector2(0, 0.5f);
                        rectTransform.anchorMax = new Vector2(0, 0.5f);
                        rectTransform.anchoredPosition = new Vector2(rectTransform.sizeDelta.x * 0.5f + padding.left, 0);
                        break;
                    case HorizontalAlignment.Bottom:
                        rectTransform.anchorMin = new Vector2(0, 0);
                        rectTransform.anchorMax = new Vector2(0, 0);
                        rectTransform.anchoredPosition = new Vector2(rectTransform.sizeDelta.x * 0.5f + padding.left, rectTransform.sizeDelta.y * 0.5f + padding.bottom);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (controlChildSize && horizontalAlignment == HorizontalAlignment.Center) rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, scrollRect.content.rect.height - childSizePadding.topAndBottom);
            }
        }

        public bool IsOutsideOfMargin(Vector3 position, Margin margin)
        {
            return margin switch
            {
                Margin.Left => scrollRect.transform.InverseTransformPoint(position).x < -disableMarginX,
                Margin.Right => scrollRect.transform.InverseTransformPoint(position).x > disableMarginX,
                Margin.Top => scrollRect.transform.InverseTransformPoint(position).y > disableMarginY,
                Margin.Bottom => scrollRect.transform.InverseTransformPoint(position).y < -disableMarginY,
                _ => throw new ArgumentOutOfRangeException(nameof(margin), margin, null)
            };
        }

        private bool IsHorizontal()
        {
            if (layoutType == LayoutType.None) return scrollDirection == ScrollDirection.Horizontal;
            return layoutType == LayoutType.HorizontalLayout;
        }

        private bool IsVertical()
        {
            if (layoutType == LayoutType.None) return scrollDirection == ScrollDirection.Vertical;
            return layoutType == LayoutType.VerticalLayout;
        }

        public enum Margin {Left, Right, Top, Bottom}
    
        public enum LayoutType {None, VerticalLayout, HorizontalLayout}
        
        public enum VerticalAlignment {Left, Center, Right}
        
        public enum HorizontalAlignment {Top, Center, Bottom}
        
        public enum ScrollDirection {Vertical, Horizontal}

        [Serializable]
        public struct Padding
        {
            public float top;
            public float bottom;
            public float left;
            public float right;
        }

        [Serializable]
        public struct ChildSizePadding
        {
            public float leftAndRight;
            public float topAndBottom;
        }
    }
}