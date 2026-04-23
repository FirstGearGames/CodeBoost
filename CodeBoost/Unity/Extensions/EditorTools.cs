#if UNITY_EDITOR || UNITY_2021_3_OR_NEWER
#define UNITY_ENGINE
#endif
#if UNITY_ENGINE
#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace CodeBoost.Extensions
{
    public enum EditorLayoutEnableType
    {
        Enabled = 0,
        Disabled = 1,
        DisabledWhilePlaying = 2
    }

    public static class EditorGuiLayoutTools
    {
        /// <summary>
        /// Adds a help box field.
        /// </summary>
        public static void AddHelpBox(string text, MessageType messageType = MessageType.Info)
        {
            EditorGUILayout.HelpBox(text, messageType);
        }

        /// <summary>
        /// Adds a property field.
        /// </summary>
        public static void AddPropertyField(SerializedProperty sp, string fieldName, string tooltip = "")
        {
            if (tooltip == "")
                tooltip = sp.tooltip;

            EditorGUILayout.PropertyField(sp, new GUIContent(fieldName, tooltip));
        }

        /// <summary>
        /// Adds a property field.
        /// </summary>
        public static void AddPropertyField(SerializedProperty sp, GUIContent guiContent)
        {
            EditorGUILayout.PropertyField(sp, guiContent);
        }

        /// <summary>
        /// Adds a property field.
        /// </summary>
        public static void AddPropertyField(SerializedProperty sp, GUIContent guiContent = null, EditorLayoutEnableType enableType = EditorLayoutEnableType.Enabled, params GUILayoutOption[] options)
        {
            bool disable = IsDisableLayoutType(enableType);
            if (disable)
                GUI.enabled = false;

            EditorGUILayout.PropertyField(sp, guiContent, options);

            if (disable)
                GUI.enabled = true;
        }

        /// <summary>
        /// Adds an object field.
        /// </summary>
        public static void AddObjectField(string label, MonoScript ms, Type type, bool allowSceneObjects, EditorLayoutEnableType enableType = EditorLayoutEnableType.Enabled, params GUILayoutOption[] options)
        {
            bool disable = IsDisableLayoutType(enableType);
            if (disable)
                GUI.enabled = false;

            EditorGUILayout.ObjectField("Script:", ms, type, allowSceneObjects, options);

            if (disable)
                GUI.enabled = true;
        }

        /// <summary>
        /// Disables the GUI if the application is playing.
        /// </summary>
        public static void DisableGUIIfPlaying()
        {
            if (Application.isPlaying)
                GUI.enabled = false;
        }

        /// <summary>
        /// Enables the GUI if the application is playing.
        /// </summary>
        public static void EnableGUIIfPlaying()
        {
            if (Application.isPlaying)
                GUI.enabled = true;
        }

        /// <summary>
        /// Returns whether a layout field should be disabled.
        /// </summary>
        /// <param name = "enableType"> </param>
        /// <returns>Returns true if the layout field should be disabled; otherwise, false.</returns>
        private static bool IsDisableLayoutType(EditorLayoutEnableType enableType)
        {
            return enableType == EditorLayoutEnableType.Disabled || (enableType == EditorLayoutEnableType.DisabledWhilePlaying && Application.isPlaying);
        }
    }

    public static class PropertyDrawerToolExtensions
    {
        /// <summary>
        /// Returns the GetPropertyHeight value based on the drawerTool properties.
        /// </summary>
        public static float GetPropertyHeight(this PropertyDrawerTool drawerTool)
        {
            if (drawerTool is null)
                return EditorGUIUtility.singleLineHeight;

            return EditorGUIUtility.singleLineHeight * drawerTool.LineSpacingMultiplier * drawerTool.PropertiesDrawn;
        }
    }

    /// <summary>
    /// Provides various utility classes relating to Float.
    /// </summary>
    public class PropertyDrawerTool
    {
        public PropertyDrawerTool()
        {
            Debug.LogError("This initializer is not supported. Use the initializer with arguments.");
        }

        public PropertyDrawerTool(Rect position, float lineSpacingMultiplier = 1f)
        {
            Position = position;
            LineSpacingMultiplier = lineSpacingMultiplier;
            Position = position;
            _startingIndent = EditorGUI.indentLevel;
        }

        /// <summary>
        /// The starting position as indicated by the OnGUI method.
        /// </summary>
        /// <remarks> This value may be modified by user code. </remarks>
        public Rect Position;
        /// <summary>
        /// The preferred spacing between each draw.
        /// </summary>
        public float LineSpacingMultiplier;
        /// <summary>
        /// The number of entries drawn by this object.
        /// </summary>
        public int PropertiesDrawn;
        /// <summary>
        /// The additional position Y of the next draw.
        /// </summary>
        private float _additionalPositionY;
        /// <summary>
        /// The indent level during initialization.
        /// </summary>
        private readonly int _startingIndent;

        /// <summary>
        /// Sets EditorGUI.Indent to the level it was when initializing this class.
        /// </summary>
        public void SetIndentToStarting() => EditorGUI.indentLevel = _startingIndent;

        /// <summary>
        /// Draws a label.
        /// </summary>
        public void DrawLabel(GUIContent lLabel) => DrawLabel(lLabel, EditorStyles.label.fontStyle, indent: 0);

        /// <summary>
        /// Draws a label.
        /// </summary>
        public void DrawLabel(GUIContent lLabel, FontStyle styleOverride) => DrawLabel(lLabel, styleOverride, indent: 0);

        /// <summary>
        /// Draws a label.
        /// </summary>
        public void DrawLabel(GUIContent lLabel, FontStyle styleOverride, int indent)
        {
            PropertiesDrawn++;

            if (indent != 0)
                EditorGUI.indentLevel += indent;

            // Set style.
            FontStyle startingStyle = EditorStyles.label.fontStyle;
            EditorStyles.label.fontStyle = styleOverride;

            EditorGUI.PrefixLabel(GetRect(), GUIUtility.GetControlID(FocusType.Passive), lLabel);

            EditorStyles.label.fontStyle = startingStyle;

            if (indent != 0)
                EditorGUI.indentLevel -= indent;
        }

        /// <summary>
        /// Draws a property.
        /// </summary>
        public void DrawProperty(SerializedProperty prop) => DrawProperty(prop, lLabel: "", indent: 0);

        /// <summary>
        /// Draws a property.
        /// </summary>
        public void DrawProperty(SerializedProperty prop, string label) => DrawProperty(prop, new GUIContent(label), EditorStyles.label.fontStyle, indent: 0);

        /// <summary>
        /// Draws a property.
        /// </summary>
        public void DrawProperty(SerializedProperty prop, GUIContent content) => DrawProperty(prop, content, EditorStyles.label.fontStyle, indent: 0);

        /// <summary>
        /// Draws a property.
        /// </summary>
        public void DrawProperty(SerializedProperty prop, int indent) => DrawProperty(prop, lLabel: "", indent);

        /// <summary>
        /// Draws a property.
        /// </summary>
        public void DrawProperty(SerializedProperty prop, string lLabel, int indent) => DrawProperty(prop, lLabel, EditorStyles.label.fontStyle, indent);

        /// <summary>
        /// Draws a property.
        /// </summary>
        public void DrawProperty(SerializedProperty prop, GUIContent content, int indent) => DrawProperty(prop, content, EditorStyles.label.fontStyle, indent);

        /// <summary>
        /// Draws a property.
        /// </summary>
        public void DrawProperty(SerializedProperty prop, GUIContent content, FontStyle labelStyle) => DrawProperty(prop, content, labelStyle, indent: 0);

        /// <summary>
        /// Draws a property.
        /// </summary>
        public void DrawProperty(SerializedProperty prop, string lLabel, FontStyle labelStyle, int indent)
        {
            GUIContent content = lLabel == "" ? default : new GUIContent(lLabel);

            DrawProperty(prop, content, labelStyle, indent);
        }

        /// <summary>
        /// Draws a property.
        /// </summary>
        public void DrawProperty(SerializedProperty prop, GUIContent content, FontStyle labelStyle, int indent)
        {
            PropertiesDrawn++;

            EditorGUI.indentLevel += indent;

            FontStyle startingStyle = EditorStyles.label.fontStyle;
            EditorStyles.label.fontStyle = labelStyle;

            EditorGUI.PropertyField(GetRect(), prop, content);

            EditorStyles.label.fontStyle = startingStyle;

            EditorGUI.indentLevel -= indent;
        }

        /// <summary>
        /// Gets the next Rect to draw at.
        /// </summary>
        /// <returns>Returns the next Rect to draw at.</returns>
        public Rect GetRect(floaT lineSpacingMultiplierOverride = null)
        {
            float multiplier = lineSpacingMultiplierOverride ?? LineSpacingMultiplier;

            Rect result = new(Position.x, Position.y + _additionalPositionY, Position.width, EditorGUIUtility.singleLineHeight * multiplier);

            _additionalPositionY += EditorGUIUtility.singleLineHeight * multiplier;

            return result;
        }
    }
}
#endif
#endif