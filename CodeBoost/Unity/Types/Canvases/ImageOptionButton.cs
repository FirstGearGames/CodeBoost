#if UNITY_EDITOR || UNITY_2021_3_OR_NEWER
#define UNITY_ENGINE
using UnityEngine;
using UnityEngine.UI;
#endif

#if UNITY_ENGINE

namespace CodeBoost.Unity.Types.Canvases
{
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class OptionMenuImageButton : OptionMenuButton
    {
        /// <summary>
        /// Image component to show image on.
        /// </summary>
        [Tooltip("Image component to show image on.")]
        private Image _image;

        public virtual void Initialize(ImageButtonData buttonData)
        {
            base.Initialize(buttonData);
            _image.sprite = buttonData.DisplayImage;
        }
    }
}
#endif