#if UNITY_EDITOR || UNITY_2021_3_OR_NEWER
#define UNITY_ENGINE
#endif
#if UNITY_ENGINE
using UnityEngine;

namespace CodeBoost.Unity.Types
{
    public class DontDestroyOnLoadRoot : MonoBehaviour
    {
        /// <summary>
        /// The created instance of the DontDestroyOnLoadRoot.
        /// </summary>
        private static DontDestroyOnLoadRoot _instance;

        /// <summary>
        /// Returns the current DontDestroyOnLoadRoot, or creates one if not yet created.
        /// </summary>
        public static DontDestroyOnLoadRoot GetDontDestroyOnLoadRoot()
        {
            // Not yet made. Use Unity's == so a destroyed instance carried across a domain-reload-disabled session re-creates.
            if (_instance == null)
            {
                GameObject dontDestroyOnLoadObject = new();
                dontDestroyOnLoadObject.name = "DontDestroyOnLoad Root";
                DontDestroyOnLoadRoot dontDestroyOnLoadRoot = dontDestroyOnLoadObject.AddComponent<DontDestroyOnLoadRoot>();
                DontDestroyOnLoad(dontDestroyOnLoadRoot);
                _instance = dontDestroyOnLoadRoot;
                return dontDestroyOnLoadRoot;
            }
            // Already  made.

            return _instance;
        }
    }
}

#endif