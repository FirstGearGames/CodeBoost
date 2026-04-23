#if UNITY_EDITOR || UNITY_2021_3_OR_NEWER
#define UNITY_ENGINE
#endif

#if UNITY_ENGINE
using UnityEngine;
using Quaternion = System.Numerics.Quaternion;

namespace CodeBoost.Unity.Extensions
{
    public static class QuaternionExtensions
    {
        public static Quaternion ToNative(this UnityEngine.Quaternion value) => new(value.x, value.y, value.z, value.w);
        public static UnityEngine.Quaternion ToUnity(this Quaternion value) => new(value.X, value.Y, value.Z, value.W);

        /// <summary>
        /// Returns how fast an object must rotate over the duration to reach the goal.
        /// </summary>
        /// <param name = "goal"> Quaternion to measure distance against. </param>
        /// <param name = "duration"> How long it should take to move to goal. </param>
        /// <param name = "interval"> A multiplier applied towards interval. Typically this is used for ticks passed. </param>
        /// <returns>Returns the rate at which the object must rotate to reach the goal.</returns>
        public static float GetRate(this UnityEngine.Quaternion a, UnityEngine.Quaternion goal, float duration, out float angle, uint interval = 1, float tolerance = 0f)
        {
            angle = a.Angle(goal, true);
            return angle / (duration * interval);
        }

        /// <summary>
        /// Subtracts quaternion b from quaternion a.
        /// </summary>
        public static UnityEngine.Quaternion Subtract(this UnityEngine.Quaternion a, UnityEngine.Quaternion b) => UnityEngine.Quaternion.Inverse(b) * a;

        /// <summary>
        /// Adds quaternion b onto quaternion a.
        /// </summary>
        public static UnityEngine.Quaternion Add(this UnityEngine.Quaternion a, UnityEngine.Quaternion b) => a * b;

        /// <summary>
        /// Returns whether two quaternions match.
        /// </summary>
        /// <param name = "precise"> True to use a custom implementation with no error tolerance. False to use Unity's implementation which may return a match even when not true due to error tolerance. </param>
        /// <returns>Returns true if the two quaternions match; otherwise, false.</returns>
        public static bool Matches(this UnityEngine.Quaternion a, UnityEngine.Quaternion b, bool precise = false)
        {
            if (precise)
                return a.w == b.w && a.x == b.x && a.y == b.y && a.z == b.z;
            return a == b;
        }

        /// <summary>
        /// Returns the angle between two quaternions.
        /// </summary>
        /// <param name = "precise"> True to use a custom implementation with no error tolerance. False to use Unity's implementation which may return 0f due to error tolerance, even while there is a difference. </param>
        /// <returns>Returns the angle between the two quaternions.</returns>
        public static float Angle(this UnityEngine.Quaternion a, UnityEngine.Quaternion b, bool precise = false)
        {
            if (precise)
            {
                // This is run Unitys implementation without the error tolerance.
                float dot = a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
                return Mathf.Acos(Mathf.Min(Mathf.Abs(dot), 1f)) * 2f * 57.29578f;
            }

            return UnityEngine.Quaternion.Angle(a, b);
        }
    }
}

#endif