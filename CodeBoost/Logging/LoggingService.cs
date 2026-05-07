using System.Diagnostics;
using System.Runtime.CompilerServices;
using CodeBoost.Environment;
using CodeBoost.Extensions;
using System;

#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
#pragma warning disable CS0067 // Event is never used

namespace CodeBoost.Logging;

/// <summary>
/// Provides a static logging service that delegates to the registered <see cref="ILogger"/> implementation.
/// </summary>
public static class LoggingService
{
    /// <summary>
    /// Invoked when the active logger is set.
    /// </summary>
    public static event LoggerSetEventHandler? LoggerSet;

    /// <summary>
    /// Represents the method that handles the <see cref="LoggerSet"/> event.
    /// </summary>
    /// <param name="logger">The logger that was registered.</param>
    public delegate void LoggerSetEventHandler(ILogger logger);

    /// <summary>
    /// The active <see cref="ILogger"/> instance.
    /// </summary>
    public static ILogger? Logger;
    /// <summary>
    /// The cached value of the logging level for performance.
    /// </summary>
    private static byte _loggerLevelAsUnderlyingType;
    /// <summary>
    /// The message used when trying to access the logger when there is no instance created.
    /// </summary>
    private static readonly string LoggerIsNullMessage = $"[{nameof(LoggingService)}] is null. Use [{nameof(LoggingService)}] to set a service.";

    static LoggingService()
    {
        UseLogger(new ConsoleLogger());
    }

    /// <summary>
    /// Sets the <see cref="ILogger"/> instance that the service will delegate to.
    /// </summary>
    /// <param name="logger">Logger to register.</param>
    public static void UseLogger(ILogger logger)
    {
        Logger = logger;
        CacheLoggerLevelAsUnderlyingType();

        LoggerSet?.Invoke(logger);
    }

    /// <summary>
    /// Returns whether the active logger disables the unconditional inclusion of a stacktrace in development environments.
    /// </summary>
    /// <returns>True when unconditional development stacktraces are suppressed.</returns>
    public static bool DisableUnconditionalDevelopmentStacktrace()
    {
        if (Logger is not null)
            return Logger.DisableUnconditionalDevelopmentStacktrace();

        throw new(LoggerIsNullMessage);
    }

    /// <summary>
    /// Returns whether the active logger and current logging level allow information messages to be emitted. Inspect this before formatting an interpolated message to skip the cost when logging is disabled.
    /// </summary>
    public static bool IsInformationEnabled
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Logger is not null && _loggerLevelAsUnderlyingType >= (byte)LoggerLevel.Information;
    }
    /// <summary>
    /// Returns whether the active logger and current logging level allow warning messages to be emitted. Inspect this before formatting an interpolated message to skip the cost when logging is disabled.
    /// </summary>
    public static bool IsWarningEnabled
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Logger is not null && _loggerLevelAsUnderlyingType >= (byte)LoggerLevel.Warning;
    }
    /// <summary>
    /// Returns whether the active logger and current logging level allow error messages to be emitted. Inspect this before formatting an interpolated message to skip the cost when logging is disabled.
    /// </summary>
    public static bool IsErrorEnabled
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Logger is not null && _loggerLevelAsUnderlyingType >= (byte)LoggerLevel.Error;
    }

    /// <summary>
    /// Logs the supplied message at the information level.
    /// </summary>
    /// <param name="message">Message to log.</param>
    public static void LogInformation(string message)
    {
        if (Logger is not null)
        {
            if (_loggerLevelAsUnderlyingType < (byte)LoggerLevel.Information)
                return;

            Logger.LogInformation(message);
        }
        else
        {
            throw new(LoggerIsNullMessage);
        }
    }

    /// <summary>
    /// Logs the supplied message at the warning level.
    /// </summary>
    /// <param name="message">Message to log.</param>
    public static void LogWarning(string message)
    {
        if (Logger is not null)
        {
            if (_loggerLevelAsUnderlyingType < (byte)LoggerLevel.Warning)
                return;

            Logger.LogWarning(message);
        }
        else
        {
            throw new(LoggerIsNullMessage);
        }
    }

    /// <summary>
    /// Logs the supplied message at the error level.
    /// </summary>
    /// <param name="message">Message to log.</param>
    public static void LogError(string message)
    {
        if (Logger is not null)
        {
            if (_loggerLevelAsUnderlyingType < (byte)LoggerLevel.Error)
                return;

            Logger.LogError(message);
        }
        else
        {
            throw new(LoggerIsNullMessage);
        }
    }

    /// <summary>
    /// Logs the supplied message at the information level without checking whether the logger is set or whether the level allows the message. Callers must verify both via <see cref="IsInformationEnabled"/> beforehand.
    /// </summary>
    /// <param name="message">Message to log.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void LogInformationUnchecked(string message) => Logger!.LogInformation(message);

    /// <summary>
    /// Logs the supplied message at the warning level without checking whether the logger is set or whether the level allows the message. Callers must verify both via <see cref="IsWarningEnabled"/> beforehand.
    /// </summary>
    /// <param name="message">Message to log.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void LogWarningUnchecked(string message) => Logger!.LogWarning(message);

    /// <summary>
    /// Logs the supplied message at the error level without checking whether the logger is set or whether the level allows the message. Callers must verify both via <see cref="IsErrorEnabled"/> beforehand.
    /// </summary>
    /// <param name="message">Message to log.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void LogErrorUnchecked(string message) => Logger!.LogError(message);

    /// <summary>
    /// Gets the logger level for the current environment.
    /// </summary>
    /// <returns>The logger level for the current environment.</returns>
    private static LoggerLevel GetLoggerLevel()
    {
        IApplicationState? applicationState = ApplicationStateService.ApplicationState;
        ILogger? logger = Logger;

        // Missing dependencies.
        if (logger is null || applicationState is null)
            return LoggerLevel.Disabled;

        LoggerSetting loggingSetting = logger.GetLoggerSetting();
        bool isEditor = applicationState.IsEditor();
        if (isEditor)
            return loggingSetting.Editor;

        bool isDevelopment = applicationState.IsDevelopment();
        if (isDevelopment)
            return loggingSetting.DevelopmentBuilds;

        // If here then is release builds.
        return loggingSetting.ReleaseBuilds;
    }

    /// <summary>
    /// Gets the logger level as the underlying type.
    /// </summary>
    /// <returns>The logger level cast to the underlying byte type.</returns>
    private static byte GetLoggerLevelAsUnderlyingType() => (byte)GetLoggerLevel();

    /// <summary>
    /// Caches the logger level to use.
    /// </summary>
    private static void CacheLoggerLevelAsUnderlyingType() => _loggerLevelAsUnderlyingType = GetLoggerLevelAsUnderlyingType();
}