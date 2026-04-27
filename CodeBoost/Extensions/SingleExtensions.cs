using CodeBoost.Mathematics;

namespace CodeBoost.Extensions;

/// <summary>
/// Extension methods for converting <see cref="float"/> values into other numeric forms.
/// </summary>
public static class SingleExtensions
{
    /// <summary>
    /// Converts the supplied <see cref="float"/> value into a clamped <see cref="int"/> using the requested accuracy.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    /// <param name="accuracy">Accuracy to use for decimals. This value is typically less than <c>1f</c>.</param>
    /// <returns>The converted integer value, clamped to the range of <see cref="int"/>.</returns>
    public static int ToInt32Unsafe(this float value, float accuracy)
    {
        int wholeValue = (int)MathCb.Clamp((long)(value * (1f / accuracy)), int.MinValue, int.MaxValue);
        return wholeValue;
    }
}