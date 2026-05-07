using System.Diagnostics;
using System.Runtime.CompilerServices;
using CodeBoost.Environment;
using CodeBoost.Extensions;
using System;

#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
#pragma warning disable CS0067 // Event is never used

namespace CodeBoost.Logging;

/// <summary>
/// A static logger that delegates to the currently registered <see cref="ILogger"/> and prefixes messages with the supplied outer and inner type names.
/// </summary>
/// <typeparam name="TOuter">Outer type used in the log prefix.</typeparam>
/// <typeparam name="TInner0">Inner type used in the log prefix.</typeparam>
public static class Logger<TOuter, TInner0>
{
    /// <summary>
    /// Returns whether information-level logging is currently enabled. Inspect this before formatting an interpolated argument so the formatting can be skipped when logging is disabled.
    /// </summary>
    public static bool IsInformationEnabled
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => LoggingService.IsInformationEnabled;
    }
    /// <summary>
    /// Returns whether warning-level logging is currently enabled. Inspect this before formatting an interpolated argument so the formatting can be skipped when logging is disabled.
    /// </summary>
    public static bool IsWarningEnabled
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => LoggingService.IsWarningEnabled;
    }
    /// <summary>
    /// Returns whether error-level logging is currently enabled. Inspect this before formatting an interpolated argument so the formatting can be skipped when logging is disabled.
    /// </summary>
    public static bool IsErrorEnabled
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => LoggingService.IsErrorEnabled;
    }

    /// <summary>
    /// Logs the supplied message at the information level prefixed with the outer and inner type names.
    /// </summary>
    /// <param name="message">Message to log.</param>
    /// <param name="methodName">Name of the calling method, supplied automatically by the compiler.</param>
    public static void LogInformation(string message, [CallerMemberName] string methodName = "")
    {
        if (!LoggingService.IsInformationEnabled)
            return;

        LoggingService.LogInformationUnchecked($"{Logger.GetLogMessagePrefix(typeof(TOuter), typeof(TInner0), methodName)}{message}");
    }
    /// <summary>
    /// Logs the supplied message at the warning level prefixed with the outer and inner type names.
    /// </summary>
    /// <param name="message">Message to log.</param>
    /// <param name="methodName">Name of the calling method, supplied automatically by the compiler.</param>
    public static void LogWarning(string message, [CallerMemberName] string methodName = "")
    {
        if (!LoggingService.IsWarningEnabled)
            return;

        LoggingService.LogWarningUnchecked($"{Logger.GetLogMessagePrefix(typeof(TOuter), typeof(TInner0), methodName)}{message}");
    }
    /// <summary>
    /// Logs the supplied message at the error level prefixed with the outer and inner type names.
    /// </summary>
    /// <param name="message">Message to log.</param>
    /// <param name="methodName">Name of the calling method, supplied automatically by the compiler.</param>
    public static void LogError(string message, [CallerMemberName] string methodName = "")
    {
        if (!LoggingService.IsErrorEnabled)
            return;

        LoggingService.LogErrorUnchecked($"{Logger.GetLogMessagePrefix(typeof(TOuter), typeof(TInner0), methodName)}{message}");
    }
}

/// <summary>
/// A static logger that delegates to the currently registered <see cref="ILogger"/> and prefixes messages with the supplied type name.
/// </summary>
/// <typeparam name="T0">Type used in the log prefix.</typeparam>
public static class Logger<T0>
{
    /// <summary>
    /// Returns whether information-level logging is currently enabled. Inspect this before formatting an interpolated argument so the formatting can be skipped when logging is disabled.
    /// </summary>
    public static bool IsInformationEnabled
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => LoggingService.IsInformationEnabled;
    }
    /// <summary>
    /// Returns whether warning-level logging is currently enabled. Inspect this before formatting an interpolated argument so the formatting can be skipped when logging is disabled.
    /// </summary>
    public static bool IsWarningEnabled
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => LoggingService.IsWarningEnabled;
    }
    /// <summary>
    /// Returns whether error-level logging is currently enabled. Inspect this before formatting an interpolated argument so the formatting can be skipped when logging is disabled.
    /// </summary>
    public static bool IsErrorEnabled
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => LoggingService.IsErrorEnabled;
    }

    /// <summary>
    /// Logs the supplied message at the information level prefixed with the type name.
    /// </summary>
    /// <param name="message">Message to log.</param>
    /// <param name="methodName">Name of the calling method, supplied automatically by the compiler.</param>
    public static void LogInformation(string message, [CallerMemberName] string methodName = "")
    {
        if (!LoggingService.IsInformationEnabled)
            return;

        LoggingService.LogInformationUnchecked($"{Logger.GetLogMessagePrefix(typeof(T0), methodName)}{message}");
    }
    /// <summary>
    /// Logs the supplied message at the warning level prefixed with the type name.
    /// </summary>
    /// <param name="message">Message to log.</param>
    /// <param name="methodName">Name of the calling method, supplied automatically by the compiler.</param>
    public static void LogWarning(string message, [CallerMemberName] string methodName = "")
    {
        if (!LoggingService.IsWarningEnabled)
            return;

        LoggingService.LogWarningUnchecked($"{Logger.GetLogMessagePrefix(typeof(T0), methodName)}{message}");
    }
    /// <summary>
    /// Logs the supplied message at the error level prefixed with the type name.
    /// </summary>
    /// <param name="message">Message to log.</param>
    /// <param name="methodName">Name of the calling method, supplied automatically by the compiler.</param>
    public static void LogError(string message, [CallerMemberName] string methodName = "")
    {
        if (!LoggingService.IsErrorEnabled)
            return;

        LoggingService.LogErrorUnchecked($"{Logger.GetLogMessagePrefix(typeof(T0), methodName)}{message}");
    }
}

/// <summary>
/// A static logger that delegates to the currently registered <see cref="ILogger"/>.
/// </summary>
public static class Logger
{
    /// <summary>
    /// Returns whether information-level logging is currently enabled. Inspect this before formatting an interpolated argument so the formatting can be skipped when logging is disabled.
    /// </summary>
    public static bool IsInformationEnabled
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => LoggingService.IsInformationEnabled;
    }
    /// <summary>
    /// Returns whether warning-level logging is currently enabled. Inspect this before formatting an interpolated argument so the formatting can be skipped when logging is disabled.
    /// </summary>
    public static bool IsWarningEnabled
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => LoggingService.IsWarningEnabled;
    }
    /// <summary>
    /// Returns whether error-level logging is currently enabled. Inspect this before formatting an interpolated argument so the formatting can be skipped when logging is disabled.
    /// </summary>
    public static bool IsErrorEnabled
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => LoggingService.IsErrorEnabled;
    }

    /// <summary>
    /// Returns whether the active logger disables the unconditional inclusion of a stacktrace in development environments.
    /// </summary>
    /// <remarks>
    /// Even when unconditional inclusion is disabled, the stacktrace will still be included for higher-level log calls.
    /// </remarks>
    /// <returns>True when unconditional development stacktraces are suppressed.</returns>
    public static bool DisableUnconditionalDevelopmentStacktrace() => LoggingService.DisableUnconditionalDevelopmentStacktrace();

    /// <summary>
    /// Logs the supplied message at the information level.
    /// </summary>
    /// <param name="message">Message to log.</param>
    /// <param name="methodName">Name of the calling method, supplied automatically by the compiler.</param>
    public static void LogInformation(string message, [CallerMemberName] string methodName = "")
    {
        if (!LoggingService.IsInformationEnabled)
            return;

        LoggingService.LogInformationUnchecked($"{GetLogMessagePrefix(methodName)}{message}");
    }
    /// <summary>
    /// Logs the supplied message at the warning level.
    /// </summary>
    /// <param name="message">Message to log.</param>
    /// <param name="methodName">Name of the calling method, supplied automatically by the compiler.</param>
    public static void LogWarning(string message, [CallerMemberName] string methodName = "")
    {
        if (!LoggingService.IsWarningEnabled)
            return;

        LoggingService.LogWarningUnchecked($"{GetLogMessagePrefix(methodName)}{message}");
    }
    /// <summary>
    /// Logs the supplied message at the error level.
    /// </summary>
    /// <param name="message">Message to log.</param>
    /// <param name="methodName">Name of the calling method, supplied automatically by the compiler.</param>
    public static void LogError(string message, [CallerMemberName] string methodName = "")
    {
        if (!LoggingService.IsErrorEnabled)
            return;

        LoggingService.LogErrorUnchecked($"{GetLogMessagePrefix(methodName)}{message}");
    }
    /// <summary>
    /// Logs the supplied message at the information level prefixed with the supplied type name.
    /// </summary>
    /// <param name="type">Type used in the log prefix.</param>
    /// <param name="message">Message to log.</param>
    /// <param name="methodName">Name of the calling method, supplied automatically by the compiler.</param>
    public static void LogInformation(Type type, string message, [CallerMemberName] string methodName = "")
    {
        if (!LoggingService.IsInformationEnabled)
            return;

        LoggingService.LogInformationUnchecked($"{GetLogMessagePrefix(type, methodName)}{message}");
    }
    /// <summary>
    /// Logs the supplied message at the warning level prefixed with the supplied type name.
    /// </summary>
    /// <param name="type">Type used in the log prefix.</param>
    /// <param name="message">Message to log.</param>
    /// <param name="methodName">Name of the calling method, supplied automatically by the compiler.</param>
    public static void LogWarning(Type type, string message, [CallerMemberName] string methodName = "")
    {
        if (!LoggingService.IsWarningEnabled)
            return;

        LoggingService.LogWarningUnchecked($"{GetLogMessagePrefix(type, methodName)}{message}");
    }
    /// <summary>
    /// Logs the supplied message at the error level prefixed with the supplied type name.
    /// </summary>
    /// <param name="type">Type used in the log prefix.</param>
    /// <param name="message">Message to log.</param>
    /// <param name="methodName">Name of the calling method, supplied automatically by the compiler.</param>
    public static void LogError(Type type, string message, [CallerMemberName] string methodName = "")
    {
        if (!LoggingService.IsErrorEnabled)
            return;

        LoggingService.LogErrorUnchecked($"{GetLogMessagePrefix(type, methodName)}{message}");
    }

    /// <summary>
    /// Returns the prefix used to annotate a log message with both the outer and inner types and the originating method name.
    /// </summary>
    /// <param name="outerType">Type that contains the method logging the message.</param>
    /// <param name="innerType">Inner type that further qualifies the log prefix.</param>
    /// <param name="methodName">Name of the method logging the message.</param>
    /// <returns>The prefix string used to annotate the log message.</returns>
    public static string GetLogMessagePrefix(Type outerType, Type innerType, string methodName) => $"[{outerType.Name}<{innerType.Name}>::{methodName}]: ";

    /// <summary>
    /// Returns the prefix used to annotate a log message with the outer type and the originating method name.
    /// </summary>
    /// <param name="outerType">Type that contains the method logging the message.</param>
    /// <param name="methodName">Name of the method logging the message.</param>
    /// <returns>The prefix string used to annotate the log message.</returns>
    public static string GetLogMessagePrefix(Type outerType, string methodName) => $"[{outerType.Name}::{methodName}]: ";

    /// <summary>
    /// Returns the prefix used to annotate a log message with the originating method name.
    /// </summary>
    /// <param name="methodName">Name of the method logging the message.</param>
    /// <returns>The prefix string used to annotate the log message.</returns>
    public static string GetLogMessagePrefix(string methodName) => $"[{methodName}]: ";

    /// <summary>
    /// Appends a stacktrace to the supplied message when the application is running as a development build.
    /// </summary>
    /// <param name="message">Message to annotate.</param>
    /// <returns>The original or annotated message, depending on the build configuration.</returns>
    public static string AddStackTraceIfDevelopment(string message)
    {
        if (!ApplicationState.IsDevelopmentBuild())
            return message;

        if (DisableUnconditionalDevelopmentStacktrace())
            return message;

        return AddStackTrace(message);
    }

    /// <summary>
    /// Appends a stacktrace to the supplied message.
    /// </summary>
    /// <param name="message">Message to annotate.</param>
    /// <returns>The annotated message containing the stacktrace.</returns>
    public static string AddStackTrace(string message) => $"{message}: {new StackTrace(fNeedFileInfo: true)}";
}