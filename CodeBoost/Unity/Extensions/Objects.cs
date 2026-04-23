#if UNITY_EDITOR || UNITY_2021_3_OR_NEWER
#define UNITY_ENGINE
#endif
#if UNITY_ENGINE
using UnityEngine;
using System.Collections.Generic;
using CodeBoost.Unity.Types;
using UnityEngine.SceneManagement;

namespace CodeBoost.Unity.Extensions
{
    public static class GameObjectExtensions
    {
        /// <summary>
        /// Returns whether an object has been destroyed from memory.
        /// </summary>
        /// <param name = "gameObject"> </param>
        /// <returns>Returns true if the object has been destroyed; otherwise, false.</returns>
        public static bool IsDestroyed(this GameObject gameObject)
        {
            // UnityEngine overloads the == operator for the GameObject type
            // and returns null when the object has been destroyed, but 
            // actually the object is still there but has not been cleaned up yet
            // if we test both we can determine if the object has been destroyed.
            return gameObject is null && !ReferenceEquals(gameObject, null);
        }

        /// <summary>
        /// Finds all objects in the scene of the specified type. This method is very expensive.
        /// </summary>
        /// <typeparam name = "T"> </typeparam>
        /// <param name = "requireSceneLoaded"> True if the scene must be fully loaded before trying to seek objects. </param>
        /// <returns>Returns a list containing all objects of the specified type found in the scene.</returns>
        public static List<T0> FindAllObjectsOfType<T0>(bool activeSceneOnly = true, bool requireSceneLoaded = false, bool includeDDOL = true, bool includeInactive = true)
        {
            List<T0> results = new();
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                // If to include only current scene.
                if (activeSceneOnly)
                {
                    if (SceneManager.GetActiveScene() != scene)
                        continue;
                }

                // If the scene must be fully loaded to seek objects within.
                if (!scene.isLoaded && requireSceneLoaded)
                    continue;

                GameObject[] allGameObjects = scene.GetRootGameObjects();
                for (int j = 0; j < allGameObjects.Length; j++)
                {
                    results.AddRange(allGameObjects[j].GetComponentsInChildren<T0>(includeInactive));
                }
            }

            // If to also include DDOL.
            if (includeDDOL)
            {
                GameObject ddolGo = DDOL.GetDDOL().gameObject;
                results.AddRange(ddolGo.GetComponentsInChildren<T0>(includeInactive));
            }

            return results;
        }
    }
}

#endif