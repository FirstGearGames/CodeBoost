#if UNITY_EDITOR || UNITY_2021_3_OR_NEWER
#define UNITY_ENGINE
#endif
#if UNITY_ENGINE
using UnityEngine;

/* This uses Sirenix namespace intentionally to mimic the needed classes
 * when Sirenix is not installed. */

namespace Sirenix.OdinInspector
{
    #if !ODIN_INSPECTOR

    public class TabGroupAttribute : PropertyAttribute
    {
        public string name;
        public bool foldEverything;

        public TabGroupAttribute(string name, bool foldEverything = false)
        {
            this.foldEverything = foldEverything;
            this.name = name;
        }
    }

    public class ShowIfAttribute : PropertyAttribute
    {
        public string comparedPropertyName { get; private set; }
        public object comparedValue { get; private set; }
        public DisablingType disablingType { get; private set; }

        public enum DisablingType : byte
        {
            ReadOnly = 2,
            DontDraw = 3
        }

        public ShowIfAttribute(string comparedPropertyName, object comparedValue, DisablingType disablingType = DisablingType.DontDraw)
        {
            this.comparedPropertyName = comparedPropertyName;
            this.comparedValue = comparedValue;
            this.disablingType = disablingType;
        }
    }

    #endif
}

#endif