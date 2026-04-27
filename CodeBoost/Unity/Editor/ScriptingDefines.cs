#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;

namespace CodeBoost.Unity.Editor
{
    /// <summary>
    /// Provides editor-time helpers for managing Unity scripting define symbols.
    /// </summary>
    internal static class ScriptingDefines
    {
        [InitializeOnLoadMethod]
        private static void AddDefineSymbols()
        {
            AddDefineSymbolToCurrentBuildTarget("UNITY_ENGINE");
        }

        /// <summary>
        /// Adds a scripting define symbol to the supplied build target.
        /// </summary>
        /// <param name="symbol">Symbol to add.</param>
        /// <param name="buildTarget">Build target to update.</param>
        /// <returns>True when the symbol was added, or false when it already existed.</returns>
        public static bool AddDefineSymbol(string symbol, NamedBuildTarget buildTarget)
        {
            HashSet<string> currentDefines = GetDefineSymbols(buildTarget);

            //Already added.
            if (!currentDefines.Add(symbol))
                return false;

            string newDefines = string.Join(";", currentDefines);
            PlayerSettings.SetScriptingDefineSymbols(buildTarget, newDefines);

            return true;
        }

        /// <summary>
        /// Adds a scripting define symbol to the current build target.
        /// </summary>
        /// <param name="symbol">Symbol to add.</param>
        /// <returns>True when the symbol was added, or false when it already existed.</returns>
        public static bool AddDefineSymbolToCurrentBuildTarget(string symbol) => AddDefineSymbol(symbol, GetCurrentBuildTarget());

        /// <summary>
        /// Removes a scripting define symbol from the supplied build target.
        /// </summary>
        /// <param name="symbol">Symbol to remove.</param>
        /// <param name="buildTarget">Build target to update.</param>
        /// <returns>True when the symbol was removed, or false when it did not exist.</returns>
        public static bool RemoveDefineSymbol(string symbol, NamedBuildTarget buildTarget)
        {
            HashSet<string> currentDefines = GetDefineSymbols(buildTarget);

            //Does not exist.
            if (!currentDefines.Remove(symbol))
                return false;

            string newDefines = string.Join(";", currentDefines);
            PlayerSettings.SetScriptingDefineSymbols(buildTarget, newDefines);

            return true;
        }

        /// <summary>
        /// Removes a scripting define symbol from the current build target.
        /// </summary>
        /// <param name="symbol">Symbol to remove.</param>
        /// <returns>True when the symbol was removed, or false when it did not exist.</returns>
        public static bool RemoveDefineSymbolToCurrentBuildTarget(string symbol) => RemoveDefineSymbol(symbol, GetCurrentBuildTarget());

        /// <summary>
        /// Returns the <see cref="NamedBuildTarget"/> that represents the current editor build target.
        /// </summary>
        /// <returns>The current <see cref="NamedBuildTarget"/>.</returns>
        public static NamedBuildTarget GetCurrentBuildTarget()
        {
            bool standaloneAndServer = false;
            BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;
            BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(buildTarget);
            if (buildTargetGroup == BuildTargetGroup.Standalone)
            {
                StandaloneBuildSubtarget standaloneSubTarget = EditorUserBuildSettings.standaloneBuildSubtarget;
                if (standaloneSubTarget == StandaloneBuildSubtarget.Server)
                    standaloneAndServer = true;
            }

            if (standaloneAndServer)
                return NamedBuildTarget.Server;

            return NamedBuildTarget.FromBuildTargetGroup(buildTargetGroup);
        }

        /// <summary>
        /// Returns the scripting define symbols configured for the supplied build target.
        /// </summary>
        /// <param name="namedBuildTarget">Build target to inspect.</param>
        /// <returns>The scripting define symbols configured for the supplied build target.</returns>
        public static HashSet<string> GetDefineSymbols(NamedBuildTarget namedBuildTarget)
        {
            string currentDefines = PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget);
            List<string> defines = currentDefines.Split(';').ToList();

            for (int i = 0; i < defines.Count; i++)
                defines[i] = defines[i].Trim();

            return defines.ToHashSet();
        }

        /// <summary>
        /// Returns the scripting define symbols configured for the current build target.
        /// </summary>
        /// <returns>The scripting define symbols configured for the current build target.</returns>
        public static HashSet<string> GetDefineSymbolsForCurrentBuildTarget() => GetDefineSymbols(GetCurrentBuildTarget());
    }
}
#endif