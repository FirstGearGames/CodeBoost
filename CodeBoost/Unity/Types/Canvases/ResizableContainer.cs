#if UNITY_EDITOR || UNITY_2021_3_OR_NEWER
#define UNITY_ENGINE
#endif
#if UNITY_ENGINE
using System.Runtime.CompilerServices;
using CodeBoost.Types;
using UnityEngine;

namespace CodeBoost.Unity.Types.Canvases
{
    public class ResizableContainer : FloatingContainer
    {
        /// <summary>
        /// Minimum and maximum range for widwth and height of the RectTransform.
        /// </summary>
        [Tooltip("Minimum and maximum range for width and height of the RectTransform.")]
        // [Foldout("Sizing")]
        public Vector2Range SizeLimits = new()
        {
            X = new(0f, 999999f),
            Y = new(0f, 999999f)
        };

        /// <summary>
        /// Sets a size, and resizes if needed.
        /// Other transform values must be set separately using inherited methods.
        /// </summary>
        /// <param name = "size"> New size to use. </param>
        /// <param name = "ignoreSizeLimits"> True to ignore serialized Size limits. </param>
        public void SetSizeAndShow(Vector2 size, bool ignoreSizeLimits = false)
        {
            ResizeAndShow(size, ignoreSizeLimits);
        }

        /// <summary>
        /// Resizes this canvas.
        /// </summary>
        protected virtual void ResizeAndShow(Vector2 desiredSize, bool ignoreSizeLimits)
        {
            float widthRequired = desiredSize.x;
            float heightRequired = desiredSize.y;
            // Clamp width and height.
            widthRequired = Mathf.Clamp(widthRequired, SizeLimits.X.Minimum, SizeLimits.X.Maximum);
            heightRequired = Mathf.Clamp(heightRequired, SizeLimits.Y.Minimum, SizeLimits.Y.Maximum);
            RectTransform.sizeDelta = new(widthRequired, heightRequired);
            base.Move();
            base.Show();
        }
    }
}

#endif