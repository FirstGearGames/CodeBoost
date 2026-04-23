#if UNITY_EDITOR || UNITY_2021_3_OR_NEWER
#define UNITY_ENGINE
#endif
#if UNITY_ENGINE
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace CodeBoost.Unity.Types.Canvases
{
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class FloatingImage : FloatingContainer
    {
        /// <summary>
        /// The renderer to apply the sprite on.
        /// </summary>
        [Tooltip("Renderer to apply sprite on.")]
        [TabGroup("Components")]
        // ReSharper disable once UnassignedField.Global
        protected Image Renderer;

        /// <summary>
        /// Sets the sprite to use.
        /// </summary>
        /// <param name = "sprite"> Sprite to use. </param>
        /// <param name = "sizeOverride"> When has value the renderer will be set to this size. Otherwise, the size of the sprite will be used. This value assumes the sprite anchors are set to center. </param>
        public virtual void SetSprite(Sprite sprite, Vector3? sizeOverride)
        {
            Renderer.sprite = sprite;
            Vector3 size = sizeOverride is null ? sprite.bounds.size * sprite.pixelsPerUnit : sizeOverride.Value;

            Renderer.rectTransform.sizeDelta = size;
        }
    }
}

#endif