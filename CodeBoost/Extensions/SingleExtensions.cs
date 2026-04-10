using CodeBoost.Mathematics;

namespace CodeBoost.Extensions;

public static class SingleExtensions
{
    /// <summary>
    /// Converts a Single to an Int32.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    /// <param name="accuracy">Accuracy to use for decimals. This value is typically less than 1f.</param>
    public static int ToInt32Unsafe(this float value, float accuracy)
    {
        int wholeValue = (int)MathCb.Clamp((long)(value * (1f / accuracy)), int.MinValue, int.MaxValue);
        return wholeValue;
    }
}