#if UNITY_EDITOR || UNITY_2021_3_OR_NEWER
#define UNITY_ENGINE
#endif
#if UNITY_ENGINE
using UnityCanvasGroup = UnityEngine.CanvasGroup;

namespace CodeBoost.Unity.Types.Canvases
{
    public static class CanvasGroupExtensions
    {
        public static void SetBlockingType(this UnityCanvasGroup group, CanvasGroupBlockingType blockingType)
        {
            if (blockingType == CanvasGroupBlockingType.Unchanged)
                return;

            bool block = blockingType == CanvasGroupBlockingType.Block;
            group.blocksRaycasts = block;
            group.interactable = block;
        }

        /// <summary>
        /// Sets the blocking type and alpha of a CanvasGroup.
        /// </summary>
        /// <param name = "blockingType"> How to handle interactions. </param>
        /// <param name = "alpha"> Alpha for CanvasGroup. </param>
        public static void SetActive(this UnityCanvasGroup group, CanvasGroupBlockingType blockingType, float alpha)
        {
            group.SetBlockingType(blockingType);
            group.alpha = alpha;
        }

        /// <summary>
        /// Sets a CanvasGroup active with the specified alpha.
        /// </summary>
        public static void SetActive(this UnityCanvasGroup group, float alpha)
        {
            group.SetActive(true, false);
            group.alpha = alpha;
        }

        /// <summary>
        /// Sets a CanvasGroup inactive with the specified alpha.
        /// </summary>
        public static void SetInactive(this UnityCanvasGroup group, float alpha)
        {
            group.SetActive(false, false);
            group.alpha = alpha;
        }

        /// <summary>
        /// Sets the active state of a group by changing the alpha and interaction toggles.
        /// </summary>
        public static void SetActive(this UnityCanvasGroup group, bool active, bool setAlpha)
        {
            if (group is null)
                return;

            if (setAlpha)
            {
                if (active)
                    group.alpha = 1f;
                else
                    group.alpha = 0f;
            }

            group.interactable = active;
            group.blocksRaycasts = active;
        }

        /// <summary>
        /// Sets the active state of a group by changing the alpha and interaction toggles with a custom alpha.
        /// </summary>
        public static void SetActive(this UnityCanvasGroup group, bool active, float alpha)
        {
            if (group is null)
                return;

            group.alpha = alpha;

            group.interactable = active;
            group.blocksRaycasts = active;
        }
    }
}

#endif