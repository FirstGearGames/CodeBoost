#if UNITY_EDITOR || UNITY_2021_3_OR_NEWER
#define UNITY_ENGINE
using UnityEngine;
using UnityEngine.UI;
#endif

#if UNITY_ENGINE

namespace CodeBoost.Unity.Extensions
{
    public static class LayoutGroupExtensions
    {
        /// <summary>
        /// Returns how many entries can fit into a GridLayoutGroup
        /// </summary>
        public static int EntriesPerWidth(this GridLayoutGroup lg)
        {
            RectTransform rectTransform = lg.GetComponent<RectTransform>();
            return Mathf.CeilToInt(rectTransform.rect.width / lg.cellSize.x);
        }
    }
}

#endif