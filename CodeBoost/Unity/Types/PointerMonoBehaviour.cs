#if UNITY_EDITOR || UNITY_2021_3_OR_NEWER
#define UNITY_ENGINE
using UnityEngine;
using UnityEngine.EventSystems;
#endif

#if UNITY_ENGINE

namespace CodeBoost.Unity.Types
{
    public abstract class PointerMonoBehaviour : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        /// <summary>
        /// Called when the pointer enters this object's RectTransform.
        /// </summary>
        public void OnPointerEnter(PointerEventData eventData) => OnHovered(true, eventData);

        /// <summary>
        /// Called when the pointer exits this object's RectTransform.
        /// </summary>
        public void OnPointerExit(PointerEventData eventData) => OnHovered(false, eventData);

        /// <summary>
        /// Called when the pointer presses this object's RectTransform.
        /// </summary>
        public void OnPointerDown(PointerEventData eventData) => OnPressed(true, eventData);

        /// <summary>
        /// Called when the pointer releases this object's RectTransform.
        /// </summary>
        public void OnPointerUp(PointerEventData eventData) => OnPressed(false, eventData);

        public virtual void OnHovered(bool hovered, PointerEventData eventData) { }
        public virtual void OnPressed(bool pressed, PointerEventData eventData) { }
    }
}

#endif