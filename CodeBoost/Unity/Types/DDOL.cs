#if UNITY_EDITOR || UNITY_2021_3_OR_NEWER
#define UNITY_ENGINE
#endif
#if UNITY_ENGINE
using UnityEngine;

namespace CodeBoost.Unity.Types
{
    public class DDOL : MonoBehaviour
    {
        /// <summary>
        /// The created instance of the DDOL.
        /// </summary>
        private static DDOL _instance;

        /// <summary>
        /// Returns the current DDOL, or creates one if not yet created.
        /// </summary>
        public static DDOL GetDDOL()
        {
            // Not yet made.
            if (_instance is null)
            {
                GameObject obj = new();
                obj.name = "DDOL Instance";
                DDOL ddol = obj.AddComponent<DDOL>();
                DontDestroyOnLoad(ddol);
                _instance = ddol;
                return ddol;
            }
            // Already  made.

            return _instance;
        }
    }
}

#endif