namespace CodeBoost.Logging;

/// <summary>
/// Defines the contract used by CodeBoost loggers.
/// </summary>
public interface ILogger
{
    /// <summary>
    /// Returns the settings that should be used for this logger.
    /// </summary>
    /// <returns>The settings that govern this logger's behavior.</returns>
    public LoggerSetting GetLoggerSetting();

    /// <summary>
    /// Returns whether the logger disables the unconditional inclusion of a stacktrace in development environments.
    /// </summary>
    /// <remarks>
    /// Even when unconditional inclusion is disabled, the stacktrace will still be included for higher-level log calls.
    /// </remarks>
    /// <returns>True when unconditional development stacktraces should be suppressed.</returns>
    public bool DisableUnconditionalDevelopmentStacktrace();

    /// <summary>
    /// Logs the supplied message at the information level.
    /// </summary>
    /// <param name="message">Message to log.</param>
    public void LogInformation(string message);

    /// <summary>
    /// Logs the supplied message at the warning level.
    /// </summary>
    /// <param name="message">Message to log.</param>
    public void LogWarning(string message);

    /// <summary>
    /// Logs the supplied message at the error level.
    /// </summary>
    /// <param name="message">Message to log.</param>
    public void LogError(string message);
}