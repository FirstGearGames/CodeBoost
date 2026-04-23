#if UNITY_EDITOR || UNITY_2021_3_OR_NEWER
#define UNITY_ENGINE
using UnityEngine;
using UnityEngine.UI;
#endif

#if UNITY_ENGINE

namespace CodeBoost.Unity.Types
{
    /// <summary>
    /// Forces a scrollbar to a value over multiple frames. Often scrollbars will not stay at the right value when a RectTransform is redrawn; this solves that problem.
    /// </summary>
    public class ScrollbarValueSetter
    {
        /// <summary>
        /// The scrollbar to fix.
        /// </summary>
        private Scrollbar _scrollBar;
        /// <summary>
        /// The value to set the scrollbar at.
        /// </summary>
        private float _value;
        /// <summary>
        /// The frame when the value was updated.
        /// </summary>
        private int _updatedFrame = -1;
        /// <summary>
        /// The number of frames to wait before fixing.
        /// </summary>
        private readonly int _fixFrames;

        public ScrollbarValueSetter(Scrollbar sb, int fixFrames = 2)
        {
            _scrollBar = sb;
            _fixFrames = fixFrames;
        }

        /// <summary>
        /// Sets the value of the scrollbar.
        /// </summary>
        /// <param name = "value"> </param>
        public void SetValue(float value)
        {
            _scrollBar.value = value;
            _value = value;
            _updatedFrame = Time.frameCount;
        }

        /// <summary>
        /// Checks to fix the scrollbar value. Should be called every frame.
        /// </summary>
        public void LateUpdate()
        {
            if (_updatedFrame == -1)
                return;
            if (Time.frameCount - _updatedFrame < _fixFrames)
                return;

            _updatedFrame = -1;
            _scrollBar.value = _value;
        }
    }
}

#endif