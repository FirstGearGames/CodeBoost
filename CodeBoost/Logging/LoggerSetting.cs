namespace CodeBoost.Logging;

/// <summary>
/// Specifies what type of messages to log.
/// </summary>
public class LoggerSetting
{
    /// <summary>
    /// The logging level for the editor.
    /// </summary>
    // Do not make readonly so that the field may be serialized.
    // ReSharper disable once FieldCanBeMadeReadOnly.Global
    public LoggerLevel Editor = LoggerLevel.Information;
    /// <summary>
    /// The logging level for development builds.
    /// </summary>
    // Do not make readonly so that the field may be serialized.
    // ReSharper disable once FieldCanBeMadeReadOnly.Global
    public LoggerLevel DevelopmentBuilds = LoggerLevel.Warning;
    /// <summary>
    /// The logging level for release builds.
    /// </summary>
    // Do not make readonly so that the field may be serialized.
    // ReSharper disable once FieldCanBeMadeReadOnly.Global
    public LoggerLevel ReleaseBuilds = LoggerLevel.Error;

    /// <summary>
    /// The default settings to use.
    /// </summary>
    public static readonly LoggerSetting LoggerServiceSetting = new()
    {
        DevelopmentBuilds = LoggerLevel.Information,
        Editor = LoggerLevel.Information,
        ReleaseBuilds = LoggerLevel.Error,
    };
}