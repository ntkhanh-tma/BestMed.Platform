using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace BestMed.Common.Helpers;

/// <summary>
/// Lightweight guard clauses for argument validation.
/// </summary>
public static class Guard
{
    /// <summary>
    /// Throws <see cref="ArgumentNullException"/> if <paramref name="value"/> is null.
    /// </summary>
    public static T NotNull<T>([NotNull] T? value, [CallerArgumentExpression(nameof(value))] string? name = null)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(value, name);
        return value;
    }

    /// <summary>
    /// Throws <see cref="ArgumentException"/> if <paramref name="value"/> is null or whitespace.
    /// </summary>
    public static string NotNullOrWhiteSpace([NotNull] string? value, [CallerArgumentExpression(nameof(value))] string? name = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, name);
        return value;
    }

    /// <summary>
    /// Throws <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is <see cref="Guid.Empty"/>.
    /// </summary>
    public static Guid NotEmpty(Guid value, [CallerArgumentExpression(nameof(value))] string? name = null)
    {
        if (value == Guid.Empty)
            throw new ArgumentOutOfRangeException(name, "GUID must not be empty.");
        return value;
    }
}
