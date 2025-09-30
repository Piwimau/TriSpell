using System;
using CommunityToolkit.Diagnostics;

namespace TriSpell;

/// <summary>
/// Represents a class providing useful extensions for an
/// <see cref="Accuracy"/>.
/// </summary>
public static class AccuracyExtensions {

    /// <summary>
    /// Returns the maximum edit distance for this <see cref="Accuracy"/> and a
    /// word with a given length.
    /// </summary>
    /// <param name="accuracy">The <see cref="Accuracy"/> level.</param>
    /// <param name="wordLength">The length of the word.</param>
    /// <returns>
    /// The maximum edit distance for this <see cref="Accuracy"/> and a word
    /// with a given length.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="accuracy"/> is not a valid
    /// <see cref="Accuracy"/> or <paramref name="wordLength"/> is negative.
    /// </exception>
    public static int MaxEditDistance(this Accuracy accuracy, int wordLength) {
        Guard.IsGreaterThanOrEqualTo(wordLength, 0);
        (double scale, int minDistance, int maxDistance) = accuracy switch {
            Accuracy.Low => (0.45, 2, 5),
            Accuracy.Medium => (0.35, 1, 3),
            Accuracy.High => (0.25, 0, 2),
            _ => throw new ArgumentOutOfRangeException(
                nameof(accuracy),
                $"'{accuracy}' is not a valid accuracy."
            )
        };
        return Math.Clamp(
            (int) Math.Ceiling(wordLength * scale),
            minDistance,
            maxDistance
        );
    }

}