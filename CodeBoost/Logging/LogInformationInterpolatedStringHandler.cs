using System.Runtime.CompilerServices;
using System.Text;

namespace CodeBoost.Logging;

/// <summary>
/// Builds the interpolated message of an information-level log call only when information messages are written, so a level that is off never formats it.
/// </summary>
/// <remarks>The compiler builds one wherever an interpolated string is passed to a logger's information method, so a caller writes <c>$"..."</c> as before and pays nothing while information messages are not written: neither the formatting nor the values in its holes are evaluated.</remarks>
[InterpolatedStringHandler]
public struct LogInformationInterpolatedStringHandler
{
    /// <summary>
    /// True when information messages are written, so the message is being built.
    /// </summary>
    public bool IsEnabled => _stringBuilder is not null;
    /// <summary>
    /// The message built so far, or null while information messages are not written.
    /// </summary>
    private StringBuilder? _stringBuilder;

    /// <summary>
    /// Creates the handler, which builds the message only when information messages are written.
    /// </summary>
    /// <param name="literalLength">The number of literal characters in the message, supplied by the compiler.</param>
    /// <param name="formattedCount">The number of values formatted into the message, supplied by the compiler.</param>
    /// <param name="isEnabled">False when information messages are not written, which tells the compiler to skip building the message.</param>
    public LogInformationInterpolatedStringHandler(int literalLength, int formattedCount, out bool isEnabled)
    {
        isEnabled = LoggingService.IsInformationEnabled;
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
