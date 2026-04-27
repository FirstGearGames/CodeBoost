#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
using System;

namespace CodeBoost.Logging;

/// <summary>
/// A logger that writes messages to the console.
/// </summary>
public class ConsoleLogger : ILogger
{
    /// <inheritdoc/>
    public LoggerSetting GetLoggerSetting() => LoggerSetting.LoggerServiceSetting;

    /// <inheritdoc/>
    public bool DisableUnconditionalDevelopmentStacktrace() => true;

    /// <inheritdoc/>
    public void LogInformation(string message) => Console.WriteLine($"Information :: {Logger.AddStackTraceIfDevelopment(message)}");

    /// <inheritdoc/>
    public void LogWarning(string message) => Console.WriteLine($"Warning :: {Logger.AddStackTraceIfDevelopment(message)}");

    /// <inheritdoc/>
    public void LogError(string message) => Console.WriteLine($"Error :: {Logger.AddStackTrace(message)}");
}