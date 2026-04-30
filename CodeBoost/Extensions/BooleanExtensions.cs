using System.Runtime.CompilerServices;

namespace CodeBoost.Extensions;

public static class BooleanExtensions
{
    /// <summary>
    /// Converts the supplied boolean to an integer.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    /// <returns><c>1</c> when the value is <c>true</c>, otherwise <c>0</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ToInt(this bool value) => value ? 1 : 0;
}
