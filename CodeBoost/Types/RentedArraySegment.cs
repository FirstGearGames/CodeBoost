// using System;
// using System.Buffers;
// using System.Collections;
//
// namespace CodeBoost.Types
// {
//     /// <summary>
//     /// Copies an array into an array belonging to an ArrayPool, creating an ArraySegment with the copy.
//     /// </summary>
//     public readonly struct RentedArraySegment<T0> : IDisposable
//     {
//         /// <summary>
//         /// ArraySegment created 
//         /// </summary>
//         public readonly ArraySegment<T0> ArraySegment;
//         /// <summary>
//         /// ArrayPool which the array was rented from.
//         /// </summary>
//         private readonly ArrayPool<T0> _pool;
//
//         /// <summary>
//         /// Creates an ArraySegment using a specified pool.
//         /// </summary>
//         /// <param name="pool">Pool to use.</param>
//         /// <param name="array">Array to copy.</param>
//         public RentedArraySegment(ArrayPool<T0> pool, T0[] array)
//         {
//             _pool = pool;
//
//             if (array is null or { Length: 0 })
//             {
//                 ArraySegment = new(Array.Empty<T0>());
//                 return;
//             }
//
//             // Array has content, copy contents.
//             T0[] arrayCopy = pool.Rent(array.Length);
//             Array.Copy(array, 0, arrayCopy, 0, array.Length);
//
//             ArraySegment = new(arrayCopy);
//         }
//
//         /// <summary>
//         /// Creates an ArraySegment using a specified pool.
//         /// </summary>
//         /// <param name="pool">Pool to use.</param>
//         /// <param name="array">Array to copy.</param>
//         public RentedArraySegment(T0[] array) : this(ArrayPool<T0>.Shared, array) { }
//
//         public void Dispose()
//         {
//             if (ArraySegment.Array is { Length: > 0 })
//                 _pool.Return(ArraySegment.Array);
//         }
//     }
// }