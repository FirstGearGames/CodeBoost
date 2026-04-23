namespace CodeBoost.Logging;

/// <summary>
/// Specifies what type of messages to log.
/// </summary>
public enum LoggerLevel : byte
{
    /// <summary>
    /// Logs errors and higher.
    /// </summary>
    Error = 1,
    /// <summary>
    /// Logs warnings and higher.
    /// </summary>
    Warning = 2,
    /// <summary>
    /// Logs information and higher.
    /// </summary>
    Information = 3,
    /// <summary>
    /// All logging is disabled.
    /// </summary>
    Disabled = 4
}