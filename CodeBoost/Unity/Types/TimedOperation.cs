#if UNITY_EDITOR || UNITY_2021_3_OR_NEWER
#define UNITY_ENGINE
#endif
#if UNITY_ENGINE
using System.Collections.Generic;
using UnityEngine;

namespace CodeBoost.Unity.Types
{
    /// <summary>
    /// Limits how often an operation can be performed through TryUseOperation.
    /// </summary>
    public class TimedOperation
    {
        /// <summary>
        /// </summary>
        /// <param name = "intervalSeconds"> Interval frequency to allow operations. </param>
        /// <param name = "isTimeScaled"> True to compare against scaled time. </param>
        public TimedOperation(float intervalSeconds, bool isTimeScaled = false)
        {
            _intervalSeconds = intervalSeconds;
            _isTimeScaled = isTimeScaled;
        }

        /// <summary>
        /// The amount of time that must pass between each operation.
        /// </summary>
        private readonly float _intervalSeconds;
        /// <summary>
        /// True to use scaled time.
        /// </summary>
        private readonly bool _isTimeScaled;
        /// <summary>
        /// The last times specific key operations were performed.
        /// </summary>
        private readonly Dictionary<string, float> _nextOperationTimesByKey = new();
        /// <summary>
        /// The last time a global operation was performed.
        /// </summary>
        private float _lastGlobalTimeSeconds;

        /// <summary>
        /// Returns whether the operation can be performed at the configured intervalSeconds.
        /// </summary>
        /// <returns> True if the operation can be performed; otherwise, false. </returns>
        public bool TryUseOperation()
        {
            float time = _isTimeScaled ? Time.time : Time.unscaledTime;

            // If enough time has passed.
            if (time - _lastGlobalTimeSeconds >= _intervalSeconds)
            {
                _lastGlobalTimeSeconds = time + _intervalSeconds;
                return true;
            }
            // Not enough time passed.

            return false;
        }

        /// <summary>
        /// Returns whether the operation can be performed at the configured intervalSeconds for the specified key.
        /// </summary>
        /// <param name = "key"> </param>
        /// <returns> True if the operation can be performed for the specified key; otherwise, false. </returns>
        public bool TryUseOperation(string key)
        {
            float time = _isTimeScaled ? Time.time : Time.unscaledTime;

            float result;
            // Key already exist.
            if (_nextOperationTimesByKey.TryGetValue(key, out result))
            {
                // If enough time has passed.
                if (time - result >= _intervalSeconds)
                {
                    _nextOperationTimesByKey[key] = time + _intervalSeconds;
                    return true;
                }
                // Not enough time passed.

                return false;
            }
            // Key not yet added.

            _nextOperationTimesByKey[key] = time + _intervalSeconds;
            return true;
        }
    }
}

#endif