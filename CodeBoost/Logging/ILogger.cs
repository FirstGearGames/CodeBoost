namespace CodeBoost.Logging;

public interface ILogger
{
    /// <summary>
    /// Returns the settings to use for this logger.
    /// </summary>
    /// <returns>The settings to use for the logger.</returns>
    public LoggerSetting GetLoggerSetting();

    /// <summary>
    /// Disables always including the stacktrace in development environments.
    /// </summary>
    /// <returns>True to disable always including the stacktrace in development environments.</returns>
    /// <remarks>Even with unconditional inclusions disabled, the stacktrace will still be included for higher level log calls.</remarks>
    public bool DisableUnconditionalDevelopmentStacktrace();

    /// <summary>
    /// Logs a message as information.
    /// </summary>
    public void LogInformation(string message);

    /// <summary>
    /// Logs a message as a warning.
    /// </summary>
    public void LogWarning(string message);

    /// <summary>
    /// Logs a message as an error.
    /// </summary>
    public void LogError(string message);
}