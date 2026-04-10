#if UNITY_ENGINE
using System.Numerics;

namespace CodeBoost.Unity.Extensions
{
    public static class VectorExtensions
    {
        public static Vector4 ToNative(this UnityEngine.Vector4 value) => new(value.x, value.y, value.z, value.w);
        
        public static UnityEngine.Vector4 ToUnity(this Vector4 value) =>  new(value.X, value.Y, value.Z, value.W);
        
        public static Vector3 ToNative(this UnityEngine.Vector3 value) => new(value.x, value.y, value.z);
        
        public static UnityEngine.Vector3 ToUnity(this Vector3 value) => new(value.X, value.Y, value.Z);
        
        public static Vector2 ToNative(this UnityEngine.Vector2 value) => new(value.x, value.y);
        
        public static UnityEngine.Vector2 ToUnity(this Vector2 value) => new(value.X, value.Y);
    }
}
#endif