#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;

namespace CodeBoost.Unity.Editor
{
    internal static class ScriptingDefines
    {
        [InitializeOnLoadMethod]
        private static void AddDefineSymbols()
        {
            AddDefineSymbolToCurrentBuildTarget("UNITY_ENGINE");
        }

        /// <summary>
        /// Adds a define symbol for the build target.
        /// </summary>
        /// <returns>True if the symbol was added, false if it already existed.</returns>
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
        /// Adds a define symbol for the current build target.
        /// </summary>
        /// <returns>True if the symbol was added, false if it already existed.</returns>

        public static bool AddDefineSymbolToCurrentBuildTarget(string symbol) => AddDefineSymbol(symbol, GetCurrentBuildTarget());

        /// <summary>
        /// Removes a define symbol for the build target.
        /// </summary>
        /// <returns>True if the symbol was removed, false if it did not exist.</returns>
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
        /// Removes a define symbol for the build target.
        /// </summary>
        /// <returns>True if the symbol was removed, false if it did not exist.</returns>
        public static bool RemoveDefineSymbolToCurrentBuildTarget(string symbol) => RemoveDefineSymbol(symbol, GetCurrentBuildTarget());

        /// <summary>
        /// Returns the current NamedBuildTarget.
        /// </summary>
        /// <returns></returns>
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
        /// Gets defines for a specific build target.
        /// </summary>
        public static HashSet<string> GetDefineSymbols(NamedBuildTarget namedBuildTarget)
        {
            string currentDefines = PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget);
            List<string> defines = currentDefines.Split(';').ToList();

            for (int i = 0; i < defines.Count; i++)
                defines[i] = defines[i].Trim();

            return defines.ToHashSet();
        }

        /// <summary>
        /// Gets defines for the current build target.
        /// </summary>
        public static HashSet<string> GetDefineSymbolsForCurrentBuildTarget() => GetDefineSymbols(GetCurrentBuildTarget());
    }
}
#endif