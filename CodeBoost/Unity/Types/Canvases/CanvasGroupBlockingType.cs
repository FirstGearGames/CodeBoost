#if UNITY_EDITOR || UNITY_2021_3_OR_NEWER
#define UNITY_ENGINE
#endif
#if UNITY_ENGINE

namespace CodeBoost.Unity.Types.Canvases
{
    /// <summary>
    /// Describes the ways a CanvasGroup can have its blocking properties modified.
    /// </summary>
    public enum CanvasGroupBlockingType
    {
        Unchanged = 0,
        DoNotBlock = 1,
        Block = 2
    }
}

#endif