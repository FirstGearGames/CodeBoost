using System.Diagnostics;
using System.Runtime.CompilerServices;
using CodeBoost.Environment;
using CodeBoost.Extensions;
using System;

#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
#pragma warning disable CS0067 // Event is never used

namespace CodeBoost.Logging;

/// <summary>
/// A static logger which uses the currently registered ILogger.
/// </summary>
public static class Logger<TOuter, TInner0>
{
    public static void LogInformation(string message, [CallerMemberName] string methodName = "") => LoggingService.LogInformation($"{Logger.GetLogMessagePrefix(typeof(TOuter), typeof(TInner0), methodName)}{message}");
    public static void LogWarning(string message, [CallerMemberName] string methodName = "") => LoggingService.LogWarning($"{Logger.GetLogMessagePrefix(typeof(TOuter), typeof(TInner0), methodName)}{message}");
    public static void LogError(string message, [CallerMemberName] string methodName = "") => LoggingService.LogError($"{Logger.GetLogMessagePrefix(typeof(TOuter), typeof(TInner0), methodName)}{message}");
}

/// <summary>
/// A static logger which uses the currently registered ILogger.
/// </summary>
public static class Logger<T0>
{
    public static void LogInformation(string message, [CallerMemberName] string methodName = "") => LoggingService.LogInformation($"{Logger.GetLogMessagePrefix(typeof(T0), methodName)}{message}");
    public static void LogWarning(string message, [CallerMemberName] string methodName = "") => LoggingService.LogWarning($"{Logger.GetLogMessagePrefix(typeof(T0), methodName)}{message}");
    public static void LogError(string message, [CallerMemberName] string methodName = "") => LoggingService.LogError($"{Logger.GetLogMessagePrefix(typeof(T0), methodName)}{message}");
}

/// <summary>
/// A static logger which uses the currently registered ILogger.
/// </summary>
public static class Logger
{
    /// <summary>
    /// Disables always including the stacktrace in development environments.
    /// </summary>
    /// <returns>True to disable always including the stacktrace in development environments.</returns>
    /// <remarks>Even with unconditional inclusions disabled, the stacktrace will still be included for higher level log calls.</remarks>
    public static bool DisableUnconditionalDevelopmentStacktrace() => LoggingService.DisableUnconditionalDevelopmentStacktrace();

    public static void LogInformation(string message, [CallerMemberName] string methodName = "") => LoggingService.LogInformation($"{Logger.GetLogMessagePrefix(methodName)}{message}");
    public static void LogWarning(string message, [CallerMemberName] string methodName = "") => LoggingService.LogWarning($"{Logger.GetLogMessagePrefix(methodName)}{message}");
    public static void LogError(string message, [CallerMemberName] string methodName = "") => LoggingService.LogError($"{Logger.GetLogMessagePrefix(methodName)}{message}");
    public static void LogInformation(Type type, string message, [CallerMemberName] string methodName = "") => Logger.LogInformation($"{Logger.GetLogMessagePrefix(type, methodName)}{message}");
    public static void LogWarning(Type type, string message, [CallerMemberName] string methodName = "") => Logger.LogWarning($"{Logger.GetLogMessagePrefix(type, methodName)}{message}");
    public static void LogError(Type type, string message, [CallerMemberName] string methodName = "") => Logger.LogError($"{Logger.GetLogMessagePrefix(type, methodName)}{message}");

    /// <summary>
    /// Returns the prefix to use for a method under an invoking type.
    /// </summary>
    /// <param name="outerType">Type which contains the method logging the message.</param>
    /// <param name="methodName">Name of the method logging the message.</param>
    /// <returns>The prefix string used to annotate the log message.</returns>
    public static string GetLogMessagePrefix(Type outerType, Type innerType, string methodName) => $"[{outerType.Name}<{innerType.Name}>::{methodName}]: ";

    /// <summary>
    /// Returns the prefix to use for a method under an invoking type.
    /// </summary>
    /// <param name="outerType">Type which contains the method logging the message.</param>
    /// <param name="methodName">Name of the method logging the message.</param>
    /// <returns>The prefix string used to annotate the log message.</returns>
    public static string GetLogMessagePrefix(Type outerType, string methodName) => $"[{outerType.Name}::{methodName}]: ";

    /// <summary>
    /// Returns the prefix to use for a method under an invoking type.
    /// </summary>
    /// <param name="methodName">Name of the method logging the message.</param>
    /// <returns>The prefix string used to annotate the log message.</returns>
    public static string GetLogMessagePrefix(string methodName) => $"[{methodName}]: ";

    /// <summary>
    /// Adds a stacktrace onto the message if the application is a development build.
    /// </summary>
    public static string AddStackTraceIfDevelopment(string message)
    {
        if (!ApplicationState.IsDevelopmentBuild())
            return message;

        if (DisableUnconditionalDevelopmentStacktrace())
            return message;

        return AddStackTrace(message);
    }

    /// <summary>
    /// Adds a stacktrace to a message.
    /// </summary>
    public static string AddStackTrace(string message) => $"{message}: {new StackTrace(fNeedFileInfo: true)}";
}