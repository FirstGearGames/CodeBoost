#if UNITY_EDITOR || UNITY_2021_3_OR_NEWER
#define UNITY_ENGINE
#endif
#if UNITY_ENGINE
using TMPro;
using UnityEngine;

namespace CodeBoost.Unity.Types.Canvases
{
    public class OptionMenuButton : MonoBehaviour
    {
        /// <summary>
        /// The ButtonData for this button.
        /// </summary>
        public ButtonData ButtonData { get; protected set; }

        /// <summary>
        /// The text component to show the button text.
        /// </summary>
        [Tooltip("Text component to show button text.")]
        private TextMeshProUGUI _text;

        public virtual void Initialize(ButtonData buttonData)
        {
            ButtonData = buttonData;
            _text.text = buttonData.Text;
        }
    }
}
#endif