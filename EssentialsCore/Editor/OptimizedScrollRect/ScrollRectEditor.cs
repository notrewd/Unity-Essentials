using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Essentials.Core.UI
{
    [CustomEditor(typeof(ScrollRect))]
    public class ScrollRectEditor : UnityEditor.UI.ScrollRectEditor
    {
        public override void OnInspectorGUI()
        {
            ScrollRect scrollRect = (ScrollRect)target;
            if (scrollRect.GetComponent<OptimizedScrollRect>() == null) base.OnInspectorGUI();
            else
            {
                GUILayout.Label("Overriden by Optimized Scroll Rect.", new GUIStyle
                {
                    normal =
                    {
                        textColor = Color.gray
                    }
                });
            }
        }
    }
}