using System;
using CommunityToolkit.Diagnostics;

namespace TriSpell.Extensions;

/// <summary>Contains useful extensions for an <see cref="Accuracy"/>.</summary>
internal static class AccuracyExtensions {

    extension(Accuracy accuracy) {

        /// <summary>
        /// Gets a short description for this <see cref="Accuracy"/>.
        /// </summary>
        public string Description => accuracy switch {
            Accuracy.Low => "Low (Higher Recall, Lower Precision)",
            Accuracy.Medium => "Medium (Balanced Recall and Precision)",
            Accuracy.High => "High (Lower Recall, Higher Precision)",
            _ => throw new InvalidOperationException("Unreachable.")
        };

        /// <summary>
        /// Returns the maximum edit distance for this <see cref="Accuracy"/>
        /// and a word with a specified length.
        /// </summary>
        /// <param name="length">The length of the word.</param>
        /// <returns>
        /// The maximum edit distance for this <see cref="Accuracy"/> and a word
        /// with the specified length.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="length"/> is negative.
        /// </exception>
        public int MaxEditDistance(int length) {
            Guard.IsGreaterThanOrEqualTo(length, 0);
            (double scale, int minDistance, int maxDistance) = accuracy switch {
                Accuracy.Low => (0.45, 2, 5),
                Accuracy.Medium => (0.35, 1, 3),
                Accuracy.High => (0.25, 0, 2),
                _ => throw new InvalidOperationException("Unreachable.")
            };
            return Math.Clamp(
                (int) Math.Ceiling(length * scale),
                minDistance,
                maxDistance
            );
        }

    }

}