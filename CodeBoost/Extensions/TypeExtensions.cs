using System;

namespace CodeBoost.Extensions;

public static partial class TypeExtensions
{
    /// <summary>
    /// Gets the FullName of the type.
    /// </summary>
    /// <param name="type">Type to get the FullName of.</param>
    /// <param name="fullName">The found FullName. Empty will be returned if the FullName is null or has no length.</param>
    /// <returns>True if the FullName is not null; otherwise, false.</returns>
    public static bool TryGetFullName(this Type type, out string fullName)
    {
        string? lFulLName = type.FullName;

        if (lFulLName is null)
        {
            fullName = string.Empty;
            return false;
        }

        fullName = lFulLName;
            
        return true;
    }

}