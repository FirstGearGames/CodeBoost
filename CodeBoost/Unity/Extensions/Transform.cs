#if UNITY_EDITOR || UNITY_2021_3_OR_NEWER
#define UNITY_ENGINE
#endif
#if UNITY_ENGINE
using UnityEngine;
using System.Collections.Generic;
using CodeBoost.Performance;

namespace CodeBoost.Unity.Extensions
{
    public static class TransformExtensions
    {
        /// <summary>
        /// Returns the sizeDelta halved.
        /// </summary>
        /// <param name = "considerScale"> True to multiple values by UnityRectTransform scale. </param>
        public static Vector2 HalfSizeDelta(this RectTransform rectTransform, bool useScale = false)
        {
            Vector2 sizeDelta = useScale ? rectTransform.SizeDeltaScaled() : rectTransform.sizeDelta;
            return sizeDelta / 2f;
        }

        /// <summary>
        /// Returns the sizeDelta multiplied by scale.
        /// </summary>
        public static Vector2 SizeDeltaScaled(this RectTransform rectTransform)
        {
            return rectTransform.sizeDelta * rectTransform.localScale;
        }

        /// <summary>
        /// Returns a position for the UnityRectTransform, ensuring it is fully on the screen.
        /// </summary>
        /// <param name = "desiredPosition"> Preferred position for the UnityRectTransform. </param>
        /// <param name = "padding"> How much padding the Transform must be from the screen edges. </param>
        public static Vector3 GetOnScreenPosition(this RectTransform rectTransform, Vector3 desiredPosition, Vector2 padding)
        {
            if (rectTransform is null)
                return Vector3.zero;

            RectTransform canvasUnityRectTransform = rectTransform.GetComponentInParent<Canvas>().transform as RectTransform;
            if (canvasUnityRectTransform is null)
                return Vector3.zero;

            Vector2 clampedPos = desiredPosition;
            Vector2 localScale = canvasUnityRectTransform.localScale;
            Vector2 oneMinusPivot = Vector2.one - rectTransform.pivot;

            // The size has to be scaled to account for the size and scale of the Canvas it is childed to
            Vector2 scaledSize = rectTransform.sizeDelta * localScale;

            // Calculate the minimum and maximum bounds of the canvas our object can occupy
            Vector2 minClamp = scaledSize * rectTransform.pivot + padding;
            Vector2 maxClamp = (canvasUnityRectTransform.rect.size - (rectTransform.sizeDelta * oneMinusPivot + padding)) * localScale;

            float clampX = Mathf.Clamp(clampedPos.x, minClamp.x, maxClamp.x);
            float clampY = Mathf.Clamp(clampedPos.y, minClamp.y, maxClamp.y);

            return new Vector2(clampX, clampY);
        }

        /// <summary>
        /// Sets a parent for src while maintaining the position, rotation, and scale of src.
        /// </summary>
        /// <param name = "parent"> Transform to become a child of. </param>
        public static void SetParentAndKeepTransform(this Transform src, Transform parent)
        {
            Vector3 pos = src.position;
            Quaternion rot = src.rotation;
            Vector3 scale = src.localScale;

            src.SetParent(parent);
            src.position = pos;
            src.rotation = rot;
            src.localScale = scale;
        }

        /// <summary>
        /// Destroys all children under the specified Transform.
        /// </summary>
        public static void DestroyChildren(this Transform t, bool destroyImmediately = false)
        {
            // If destroying immediately then the iteration needs to occur only on the top-most children.
            if (destroyImmediately)
            {
                List<Transform> children = CollectionPool<Transform>.RentList();
                int childCount = t.childCount;

                for (int i = 0; i < childCount; i++)
                    children.Add(t.GetChild(i));

                foreach (Transform child in children)
                    Object.DestroyImmediate(child);

                CollectionPool<Transform>.Return(children);
            }
            // Iterate using Unitys enumerator.
            else
            {
                foreach (Transform child in t)
                    Object.Destroy(child.gameObject);
            }
        }

        /// <summary>
        /// Destroys all children of the specified type under the specified Transform.
        /// </summary>
        public static void DestroyChildren<T0>(this Transform t, bool destroyImmediately = false) where T0 : MonoBehaviour
        {
            T0[] children = t.GetComponentsInChildren<T0>();
            foreach (T0 child in children)
            {
                if (destroyImmediately)
                    Object.DestroyImmediate(child.gameObject);
                else
                    Object.Destroy(child.gameObject);
            }
        }

        /// <summary>
        /// Gets the components in children and optionally the parent.
        /// </summary>
        /// <typeparam name = "T"> </typeparam>
        /// <param name = "results"> </param>
        /// <param name = "parent"> </param>
        /// <param name = "includeParent"> </param>
        /// <param name = "includeInactive"> </param>
        public static void GetComponentsInChildren<T0>(this Transform parent, List<T0> results, bool includeParent = true, bool includeInactive = false) where T0 : Component
        {
            if (!includeParent)
            {
                List<T0> current = CollectionPool<T0>.RentList();
                for (int i = 0; i < parent.childCount; i++)
                {
                    parent.GetChild(i).GetComponentsInChildren(includeInactive, current);
                    results.AddRange(current);
                }

                CollectionPool<T0>.Return(current);
            }
            else
            {
                parent.GetComponentsInChildren(includeInactive, results);
            }
        }

        /// <summary>
        /// Returns the position of this Transform.
        /// </summary>
        public static Vector3 GetPosition(this Transform t, bool localSpace)
        {
            return localSpace ? t.localPosition : t.position;
        }

        /// <summary>
        /// Returns the rotation of this Transform.
        /// </summary>
        public static Quaternion GetRotation(this Transform t, bool localSpace)
        {
            return localSpace ? t.localRotation : t.rotation;
        }

        /// <summary>
        /// Returns the scale of this Transform.
        /// </summary>
        public static Vector3 GetScale(this Transform t)
        {
            return t.localScale;
        }

        /// <summary>
        /// Sets the position of this Transform.
        /// </summary>
        /// <param name = "t"> </param>
        /// <param name = "localSpace"> </param>
        public static void SetPosition(this Transform t, bool localSpace, Vector3 pos)
        {
            if (localSpace)
                t.localPosition = pos;
            else
                t.position = pos;
        }

        /// <summary>
        /// Sets the position of this Transform.
        /// </summary>
        /// <param name = "t"> </param>
        /// <param name = "localSpace"> </param>
        public static void SetRotation(this Transform t, bool localSpace, Quaternion rot)
        {
            if (localSpace)
                t.localRotation = rot;
            else
                t.rotation = rot;
        }

        /// <summary>
        /// Sets the position of this Transform.
        /// </summary>
        /// <param name = "t"> </param>
        /// <param name = "localSpace"> </param>
        public static void SetScale(this Transform t, Vector3 scale)
        {
            t.localScale = scale;
        }
    }
}

#endif