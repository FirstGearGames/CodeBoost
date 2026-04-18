#if UNITY_EDITOR || UNITY_2021_3_OR_NEWER
#define UNITY_ENGINE
#endif
#if UNITY_ENGINE
using System.Collections.Generic;
using CodeBoost.Performance;
using UnityEngine;

namespace CodeBoost.Unity.Types.Canvases
{
    /// <summary>
    /// Gameplay canvases register to this manager.
    /// </summary>
    public class RectTransformResizer : MonoBehaviour
    {
        public class ResizeData : IPoolResettable
        {
            public byte Remaining;
            public ResizeHandler Delegate;

            public ResizeData()
            {
                Remaining = 2;
            }

            public void OnRent() { }

            public void OnReturn()
            {
                Remaining = 2;
                Delegate = null;
            }
        }

        /// <summary>
        /// Delegate for resizing RectTransforms.
        /// </summary>
        /// <param name = "complete"> True if the resize iterations are complete. Typically show your visuals when true. </param>
        public delegate void ResizeHandler(bool complete);
        /// <summary>
        /// Elements to resize.
        /// </summary>
        private readonly List<ResizeData> _resizeDatas = new();
        /// <summary>
        /// Singleton instance of this class.
        /// </summary>
        private static RectTransformResizer _instance;

        private void OnDestroy()
        {
            foreach (ResizeData item in _resizeDatas)
                ResettableObjectPool<ResizeData>.Return(item);
        }

        private void Update()
        {
            Resize();
        }

        /// <summary>
        /// Calls pending resizeDatas.
        /// </summary>
        private void Resize()
        {
            for (int i = 0; i < _resizeDatas.Count; i++)
            {
                _resizeDatas[i].Remaining--;
                bool complete = _resizeDatas[i].Remaining == 0;
                _resizeDatas[i].Delegate?.Invoke(complete);
                if (complete)
                {
                    ResettableObjectPool<ResizeData>.Return(_resizeDatas[i]);
                    _resizeDatas.RemoveAt(i);
                    i--;
                }
            }
        }

        /// <summary>
        /// Used to call a delegate twice, over two frames.
        /// This is an easy way to resize RectTransforms multiple times as they will often fail after the first resize due to Unity limitations.
        /// Note: this work-around may not be required for newer Unity versions.
        /// </summary>
        /// <param name = "del"> Delegate to invoke when resizing completes. </param>
        public static void Resize(ResizeHandler del)
        {
            // Check to make a singleton instance.
            if (_instance is null)
            {
                GameObject go = new(typeof(RectTransformResizer).Name);
                _instance = go.AddComponent<RectTransformResizer>();
                DontDestroyOnLoad(go);
            }

            _instance.Resize_Internal(del);
        }

        private void Resize_Internal(ResizeHandler del)
        {
            ResizeData rd = ResettableObjectPool<ResizeData>.Rent();
            rd.Delegate = del;
            _instance._resizeDatas.Add(rd);
        }
    }
}

#endif