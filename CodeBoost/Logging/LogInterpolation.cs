using System;
using System.Text;

namespace CodeBoost.Logging;

/// <summary>
/// The formatting the log interpolated string handlers share, so each handler decides only whether its level is written.
/// </summary>
internal static class LogInterpolation
{
    /// <summary>
    /// A rough number of characters each formatted value adds, used to size the builder so most messages never grow it.
    /// </summary>
    private const int EstimatedFormattedLength = 16;

    /// <summary>
    /// Returns a builder sized for a message whose level is written, or null when it is not, so nothing is formatted.
    /// </summary>
    /// <param name="isEnabled">Whether the message's level is written.</param>
    /// <param name="literalLength">The number of literal characters in the message.</param>
    /// <param name="formattedCount">The number of values formatted into the message.</param>
    /// <returns>A builder for the message, or null when the level is not written.</returns>
    internal static StringBuilder? CreateStringBuilder(bool isEnabled, int literalLength, int formattedCount) => isEnabled ? new StringBuilder(literalLength + formattedCount * EstimatedFormattedLength) : null;

    /// <summary>
    /// Appends a value to a message, in the format its hole states when the value can take one.
    /// </summary>
    /// <typeparam name="T0">The type of the value.</typeparam>
    /// <param name="stringBuilder">The message being built.</param>
    /// <param name="value">The value to append.</param>
    /// <param name="format">The format the hole states, or null when it states none.</param>
    internal static void AppendFormatted<T0>(StringBuilder stringBuilder, T0 value, string? format)
    {
        if (format is not null && value is IFormattable formattable)
            stringBuilder.Append(formattable.ToString(format, formatProvider: null));
        else
            stringBuilder.Append(value?.ToString());
    }
}
