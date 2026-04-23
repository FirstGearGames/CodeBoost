#if UNITY_EDITOR || UNITY_2021_3_OR_NEWER
#define UNITY_ENGINE
#endif
#if UNITY_ENGINE
using System.Collections.Generic;
using CodeBoost.Performance;

namespace CodeBoost.Unity.Types.Canvases
{
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class FloatingOptions : CanvasGroupFader
    {
        /// <summary>
        /// The current buttons.
        /// </summary>
        protected List<ButtonData> Buttons = new();

        /// <summary>
        /// Adds the buttons.
        /// </summary>
        /// <param name = "clearExisting"> True to clear existing buttons first. </param>
        /// <param name = "buttonDatas"> Buttons to add. </param>
        protected virtual void AddButtons(bool clearExisting, IEnumerable<ButtonData> buttonDatas)
        {
            if (clearExisting)
                RemoveButtons();
            foreach (ButtonData item in buttonDatas)
                Buttons.Add(item);
        }

        /// <summary>
        /// Removes all buttons.
        /// </summary>
        protected virtual void RemoveButtons()
        {
            foreach (ButtonData item in Buttons)
                ResettableObjectPool<ButtonData>.Return(item);
            Buttons.Clear();
        }
    }
}

#endif