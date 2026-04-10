#if UNITY_EDITOR || UNITY_2021_3_OR_NEWER
#define UNITY_ENGINE
#endif
#if UNITY_ENGINE
using UnityEngine;

namespace CodeBoost.Extensions
{
    public static class MaterialExtensions
    {
        /// <summary>
        /// Returns the color or tint color property for a material.
        /// </summary>
        /// <param name = "material"> </param>
        /// <returns> </returns>
        public static Color GetColor(this Material material)
        {
            if (material.HasProperty("_Color"))
                return material.color;
            if (material.HasProperty("_TintColor"))
                return material.GetColor("_TintColor");

            return Color.white;
        }

        /// <summary>
        /// Sets the color or tint color property for a material.
        /// </summary>
        /// <param name = "material"> </param>
        public static void SetColor(this Material material, Color color)
        {
            if (material.HasProperty("_Color"))
                material.color = color;
            else if (material.HasProperty("_TintColor"))
                material.SetColor("_TintColor", color);
        }
    }
}

#endif