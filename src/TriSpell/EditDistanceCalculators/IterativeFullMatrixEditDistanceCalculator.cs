using System;

namespace TriSpell.EditDistanceCalculators;

/// <summary>
/// Represents a slightly more advanced, iterative implementation of an
/// <see cref="IEditDistanceCalculator"/>.
/// </summary>
/// <remarks>
/// This implementation employs techniques of dynamic programming: In comparison to the
/// <see cref="RecursiveEditDistanceCalculator"/>, it avoids redundant calculations by storing the
/// edit distances between all prefixes of the source and target string in a two-dimensional matrix.
/// The final value calculated in the lower right-hand corner is the actual edit distance between
/// the full strings. See
/// <see href="https://en.wikipedia.org/wiki/Levenshtein_distance">this article</see> for more
/// information.
/// </remarks>
public sealed class IterativeFullMatrixEditDistanceCalculator : IEditDistanceCalculator {

    /// <summary>Maximum limit for allocating the distances matrix on the stack.</summary>
    private const int MaxStackAllocLimit = 256;

    /// <inheritdoc/>
    public string Description => "Iterative Full Matrix (Fast)";

    /// <summary>
    /// Initializes a new <see cref="IterativeFullMatrixEditDistanceCalculator"/>.
    /// </summary>
    public IterativeFullMatrixEditDistanceCalculator() { }

    /// <inheritdoc/>
    public int EditDistance(ReadOnlySpan<char> source, ReadOnlySpan<char> target) {
        int rows = source.Length + 1;
        int columns = target.Length + 1;
        int size = rows * columns;
        // The matrix of edit distances is conceptually two-dimensional, but we use a
        // one-dimensional buffer for improved locality and performance. This also means that we
        // have to do the indexing on our own. The edit distance at position (i, j) is found at
        // distances[(i * columns) + j].
        Span<int> distances = (size <= MaxStackAllocLimit) ? stackalloc int[size] : new int[size];
        distances[0] = 0;
        // Source prefixes can only be transformed into an empty string by deleting all characters.
        for (int i = 1; i < rows; i++) {
            distances[i * columns] = i;
        }
        // Empty source prefix can only be transformed into target by inserting all characters.
        for (int j = 1; j < columns; j++) {
            distances[j] = j;
        }
        for (int i = 1; i < rows; i++) {
            for (int j = 1; j < columns; j++) {
                int insertion = distances[(i * columns) + (j - 1)] + 1;
                int deletion = distances[((i - 1) * columns) + j] + 1;
                // If the characters match, there is no additional cost for substitution.
                int substitution = distances[((i - 1) * columns) + (j - 1)]
                    + ((source[i - 1] == target[j - 1]) ? 0 : 1);
                distances[(i * columns) + j] = Math.Min(
                    Math.Min(insertion, deletion),
                    substitution
                );
            }
        }
        return distances[((rows - 1) * columns) + (columns - 1)];
    }

}