#if UNITY_EDITOR || UNITY_2021_3_OR_NEWER
#define UNITY_ENGINE
#endif
#if UNITY_ENGINE
using UnityEngine;

namespace CodeBoost.Unity.Types
{
    /// <summary>
    /// Creates a singleton instance of a scriptable object.
    /// </summary>
    public abstract class SingletonScriptableObject<T0> : ScriptableObject where T0 : ScriptableObject
    {
        private static T0 _instance;
        public static T0 Instance
        {
            get
            {
                if (_instance is null)
                {
                    T0[] results = Resources.FindObjectsOfTypeAll<T0>();
                    if (results.Length == 0)
                    {
                        Debug.LogError("SingletonScriptableObject: results length is 0 of " + typeof(T0));
                        return null;
                    }

                    if (results.Length > 1)
                    {
                        Debug.LogError("SingletonScriptableObject: results length is greater than 1 of " + typeof(T0));
                        return null;
                    }

                    _instance = results[0];
                    _instance.hideFlags = HideFlags.DontUnloadUnusedAsset;
                }

                return _instance;
            }
        }
    }
}

#endif