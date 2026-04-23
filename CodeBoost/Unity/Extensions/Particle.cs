#if UNITY_EDITOR || UNITY_2021_3_OR_NEWER
#define UNITY_ENGINE
#endif
#if UNITY_ENGINE
using UnityEngine;

namespace CodeBoost.Extensions
{
    public static class ParticleExtensions
    {
        /// <summary>
        /// Calls stop on the specified particle systems.
        /// </summary>
        /// <param name = "systems"> </param>
        /// <returns>Returns the time required for the particle systems to play out.</returns>
        public static float StopParticleSystem(ParticleSystem[] systems, bool stopLoopingOnly)
        {
            return StopParticleSystem(systems, stopLoopingOnly, ParticleSystemStopBehavior.StopEmitting);
        }

        /// <summary>
        /// Calls stop on the specified particle systems while returning the time required to play out.
        /// </summary>
        /// <param name = "systems"> </param>
        /// <returns>Returns the time required for the particle systems to play out.</returns>
        public static float StopParticleSystem(ParticleSystem[] systems, ParticleSystemStopBehavior stopBehavior = ParticleSystemStopBehavior.StopEmitting)
        {
            return StopParticleSystem(systems, false, stopBehavior);
        }

        /// <summary>
        /// Calls stop on the specified particle systems while returning the time required to play out.
        /// </summary>
        /// <param name = "systems"> </param>
        /// <returns>Returns the time required for the particle systems to play out.</returns>
        public static float StopParticleSystem(ParticleSystem[] systems, bool stopLoopingOnly, ParticleSystemStopBehavior stopBehavior = ParticleSystemStopBehavior.StopEmitting)
        {
            if (systems is null)
                return 0f;

            float playOutDuration = 0f;
            for (int i = 0; i < systems.Length; i++)
                playOutDuration = Mathf.Max(playOutDuration, StopParticleSystem(systems[i], stopLoopingOnly, stopBehavior));

            return playOutDuration;
        }

        /// <summary>
        /// Calls stop on the specified particle system.
        /// </summary>
        /// <param name = "systems"> </param>
        /// <returns>Returns the time required for the particle system to play out.</returns>
        public static float StopParticleSystem(ParticleSystem system, bool stopLoopingOnly, bool stopChildren = false)
        {
            return StopParticleSystem(system, stopLoopingOnly, ParticleSystemStopBehavior.StopEmitting, stopChildren);
        }

        /// <summary>
        /// Calls stop on the specified particle system while returning the time required to play out.
        /// </summary>
        /// <param name = "systems"> </param>
        /// <returns>Returns the time required for the particle system to play out.</returns>
        public static float StopParticleSystem(ParticleSystem system, ParticleSystemStopBehavior stopBehavior = ParticleSystemStopBehavior.StopEmitting, bool stopChildren = false)
        {
            return StopParticleSystem(system, false, stopBehavior, stopChildren);
        }

        /// <summary>
        /// Calls stop on the specified particle system while returning the time required to play out.
        /// </summary>
        public static float StopParticleSystem(ParticleSystem system, bool stopLoopingOnly, ParticleSystemStopBehavior stopBehavior = ParticleSystemStopBehavior.StopEmitting, bool stopChildren = false)
        {
            if (system is null)
                return 0f;
            if (stopChildren)
            {
                ParticleSystem[] all = system.GetComponentsInChildren<ParticleSystem>();
                StopParticleSystem(all, stopLoopingOnly, stopBehavior);
            }

            float playOutDuration = 0f;
            float timeLeft = system.main.duration - system.time;
            playOutDuration = Mathf.Max(playOutDuration, timeLeft);

            if (stopLoopingOnly)
            {
                if (system.main.loop)
                    system.Stop(false, stopBehavior);
            }
            else
            {
                system.Stop(false, stopBehavior);
            }

            return playOutDuration;
        }

        /// <summary>
        /// Returns the longest time required for all systems to stop.
        /// </summary>
        /// <param name = "systems"> </param>
        /// <returns>Returns the longest time required for all systems to stop.</returns>
        public static float ReturnLongestCycle(ParticleSystem[] systems)
        {
            float longestPlayTime = 0f;
            for (int i = 0; i < systems.Length; i++)
            {
                float timeLeft = systems[i].main.duration - systems[i].time;
                longestPlayTime = Mathf.Max(longestPlayTime, timeLeft);
            }

            return longestPlayTime;
        }
    }
}

#endif