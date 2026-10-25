using System.Runtime.CompilerServices;
using System.Text;

namespace CodeBoost.Logging;

/// <summary>
/// Builds the interpolated message of a warning-level log call only when warnings are written, so a level that is off never formats it.
/// </summary>
/// <remarks>The compiler builds one wherever an interpolated string is passed to a logger's warning method, so a caller writes <c>$"..."</c> as before and pays nothing while warnings are not written: neither the formatting nor the values in its holes are evaluated.</remarks>
[InterpolatedStringHandler]
public struct LogWarningInterpolatedStringHandler
{
    /// <summary>
    /// True when warnings are written, so the message is being built.
    /// </summary>
    public bool IsEnabled => _stringBuilder is not null;
    /// <summary>
    /// The message built so far, or null while warnings are not written.
    /// </summary>
    private StringBuilder? _stringBuilder;

    /// <summary>
    /// Creates the handler, which builds the message only when warnings are written.
    /// </summary>
    /// <param name="literalLength">The number of literal characters in the message, supplied by the compiler.</param>
    /// <param name="formattedCount">The number of values formatted into the message, supplied by the compiler.</param>
    /// <param name="isEnabled">False when warnings are not written, which tells the compiler to skip building the message.</param>
    public LogWarningInterpolatedStringHandler(int literalLength, int formattedCount, out bool isEnabled)
    {
        isEnabled = LoggingService.IsWarningEnabled;
        _stringBuilder = LogInterpolation.CreateStringBuilder(isEnabled, literalLength, formattedCount);
    }

    /// <summary>
    /// Appends a literal part of the message.
    /// </summary>
    /// <param name="value">The literal text.</param>
    public void AppendLiteral(string value) => _stringBuilder!.Append(value);

    /// <summary>
    /// Appends a value formatted into the message.
    /// </summary>
    /// <typeparam name="T0">The type of the value.</typeparam>
    /// <param name="value">The value to append.</param>
    public void AppendFormatted<T0>(T0 value) => LogInterpolation.AppendFormatted(_stringBuilder!, value, format: null);

    /// <summary>
    /// Appends a value formatted into the message in the format its hole states.
    /// </summary>
    /// <typeparam name="T0">The type of the value.</typeparam>
    /// <param name="value">The value to append.</param>
    /// <param name="format">The format the hole states.</param>
    public void AppendFormatted<T0>(T0 value, string? format) => LogInterpolation.AppendFormatted(_stringBuilder!, value, format);

    /// <summary>
    /// Returns the built message and lets go of the builder.
    /// </summary>
    /// <returns>The message.</returns>
    internal string ToStringAndClear()
    {
        string message = _stringBuilder!.ToString();
        _stringBuilder = null;

        return message;
    }
}
