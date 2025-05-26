using System;

namespace TriSpell;

/// <summary>Provides useful extensions for the <see cref="Accuracy"/> enumeration.</summary>
public static class AccuracyExtensions {

    /// <summary>Returns a short description of this <see cref="Accuracy"/>.</summary>
    /// <param name="accuracy">The <see cref="Accuracy"/> to describe.</param>
    /// <returns>A short description of this <see cref="Accuracy"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="accuracy"/> is not a valid <see cref="Accuracy"/>.
    /// </exception>
    public static string Description(this Accuracy accuracy)
        => accuracy switch {
            Accuracy.Low => $"Low (Maximum Edit Distance = {accuracy.MaxEditDistance()})",
            Accuracy.Medium => $"Medium (Maximum Edit Distance = {accuracy.MaxEditDistance()})",
            Accuracy.High => $"High (Maximum Edit Distance = {accuracy.MaxEditDistance()})",
            _ => throw new ArgumentOutOfRangeException(
                nameof(accuracy),
                $"'{accuracy}' is not a valid accuracy."
            )
        };

    /// <summary>Returns the maximum edit distance for this <see cref="Accuracy"/>.</summary>
    /// <param name="accuracy">
    /// The <see cref="Accuracy"/> to get the maximum edit distance for.
    /// </param>
    /// <returns>The maximum edit distance for this <see cref="Accuracy"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="accuracy"/> is not a valid <see cref="Accuracy"/>.
    /// </exception>
    public static int MaxEditDistance(this Accuracy accuracy)
        => accuracy switch {
            Accuracy.Low => 3,
            Accuracy.Medium => 2,
            Accuracy.High => 1,
            _ => throw new ArgumentOutOfRangeException(
                nameof(accuracy),
                $"'{accuracy}' is not a valid accuracy."
            )
        };

}