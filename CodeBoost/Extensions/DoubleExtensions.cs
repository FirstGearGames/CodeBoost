
namespace CodeBoost.Extensions;

public static class DoubleExtensions
{
    /// <summary>
    /// Converts a Double to an Int64.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    /// <param name="accuracy">Accuracy to use for decimals. This value is typically less than 1f.</param>
    public static long ToInt64Unsafe(this double value, float accuracy)
    {
        long wholeValue = (long)(value * (1d / accuracy));
        return wholeValue;
    }
}