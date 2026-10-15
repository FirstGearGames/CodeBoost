namespace CodeBoost.Logging;

/// <summary>
/// A message and the level to log it at, carried as one value so a log line can be decided in one place and written in another.
/// </summary>
/// <remarks>
/// The default value is not set and logs nothing, so a field of this type reads as "no log" until something assigns it. Pass a
/// constant message so building an entry never allocates; only writing it through <see cref="Logger.Log(LogEntry, string)"/> does.
/// </remarks>
public readonly struct LogEntry
{
    /// <summary>
    /// The level to log <see cref="Message"/> at.
    /// </summary>
    public readonly LoggerLevel Level;
    /// <summary>
    /// The message to log, or null when the entry is not set.
    /// </summary>
    public readonly string Message;
    /// <summary>
    /// Returns whether this entry holds a message to log.
    /// </summary>
    public bool IsSet => Message is not null && Level is not LoggerLevel.Disabled;

    /// <summary>
    /// Creates an entry that logs <paramref name="message"/> at <paramref name="level"/>.
    /// </summary>
    /// <param name="level">The level to log the message at.</param>
    /// <param name="message">The message to log.</param>
    public LogEntry(LoggerLevel level, string message)
    {
        Level = level;
        Message = message;
    }

    /// <summary>
    /// Creates an entry that logs <paramref name="message"/> at the information level.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <returns>The entry.</returns>
    public static LogEntry Information(string message) => new(LoggerLevel.Information, message);
    /// <summary>
    /// Creates an entry that logs <paramref name="message"/> at the warning level.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <returns>The entry.</returns>
    public static LogEntry Warning(string message) => new(LoggerLevel.Warning, message);
    /// <summary>
    /// Creates an entry that logs <paramref name="message"/> at the error level.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <returns>The entry.</returns>
    public static LogEntry Error(string message) => new(LoggerLevel.Error, message);
}
