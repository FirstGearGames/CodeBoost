#if UNITY_EDITOR || UNITY_2021_3_OR_NEWER
#define UNITY_ENGINE
#endif
#if UNITY_ENGINE
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace CodeBoost.Unity.Types.Editors
{
    /* Source https://forum.unity.com/threads/how-to-link-scenes-in-the-inspector.383140/ */

    [CustomPropertyDrawer(typeof(SceneAttribute))]
    public class SceneDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.String)
            {
                SceneAsset sceneObject = AssetDatabase.LoadAssetAtPath<SceneAsset>(property.stringValue);

                if (sceneObject is null && !string.IsNullOrEmpty(property.stringValue))
                {
                    // try to load it from the build settings for legacy compatibility
                    sceneObject = GetBuildSettingsSceneObject(property.stringValue);
                }

                if (sceneObject is null && !string.IsNullOrEmpty(property.stringValue))
                {
                    Debug.Log($"Could not find scene {property.stringValue} in {property.propertyPath}, assign the proper scenes in your coreManager");
                }

                SceneAsset scene = (SceneAsset)EditorGUI.ObjectField(position, label, sceneObject, typeof(SceneAsset), true);

                property.stringValue = AssetDatabase.GetAssetPath(scene);
            }
            else
            {
                EditorGUI.LabelField(position, label.text, "Use [Scene] with strings.");
            }
        }

        protected SceneAsset GetBuildSettingsSceneObject(string sceneName)
        {
            foreach (EditorBuildSettingsScene buildScene in EditorBuildSettings.scenes)
            {
                SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(buildScene.path);
                if (sceneAsset is not null && sceneAsset.name == sceneName)
                {
                    return sceneAsset;
                }
            }

            return null;
        }
    }
}
#endif
#endif