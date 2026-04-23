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
        /// <param name = "interval"> Interval frequency to allow operations. </param>
        /// <param name = "scaledTime"> True to compare against scaled time. </param>
        public TimedOperation(float interval, bool scaledTime = false)
        {
            _interval = interval;
            _scaledTime = scaledTime;
        }

        /// <summary>
        /// The amount of time that must pass between each operation.
        /// </summary>
        private readonly float _interval;
        /// <summary>
        /// True to use scaled time.
        /// </summary>
        private readonly bool _scaledTime;
        /// <summary>
        /// The last times specific key operations were performed.
        /// </summary>
        private readonly Dictionary<string, float> _operationTimes = new();
        /// <summary>
        /// The last time a global operation was performed.
        /// </summary>
        private float _lastGlobalTime;

        /// <summary>
        /// Returns whether the operation can be performed at the configured interval.
        /// </summary>
        /// <returns> True if the operation can be performed; otherwise, false. </returns>
        public bool TryUseOperation()
        {
            float time = _scaledTime ? Time.time : Time.unscaledTime;

            // If enough time has passed.
            if (time - _lastGlobalTime >= _interval)
            {
                _lastGlobalTime = time + _interval;
                return true;
            }
            // Not enough time passed.

            return false;
        }

        /// <summary>
        /// Returns whether the operation can be performed at the configured interval for the specified key.
        /// </summary>
        /// <param name = "key"> </param>
        /// <returns> True if the operation can be performed for the specified key; otherwise, false. </returns>
        public bool TryUseOperation(string key)
        {
            float time = _scaledTime ? Time.time : Time.unscaledTime;

            float result;
            // Key already exist.
            if (_operationTimes.TryGetValue(key, out result))
            {
                // If enough time has passed.
                if (time - result >= _interval)
                {
                    _operationTimes[key] = time + _interval;
                    return true;
                }
                // Not enough time passed.

                return false;
            }
            // Key not yet added.

            _operationTimes[key] = time + _interval;
            return true;
        }
    }
}

#endif