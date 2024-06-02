using System;

namespace TriSpell.Source.DistanceCalculators;

/// <summary>
/// Represents a slightly more advanced implementation of an <see cref="IDistanceCalculator"/>.
/// </summary>
/// <remarks>
/// <para>
/// This implementation employs techniques of dynamic programming: In comparison to the
/// <see cref="RecursiveCalculator"/>, it avoids redundant calculations by storing the edit
/// distances between all prefixes of the source and target string in a two-dimensional matrix.
/// The final value calculated in the lower right-hand corner is the actual edit distance between
/// the full strings.
/// See <see href="https://en.wikipedia.org/wiki/Levenshtein_distance">Wikipedia</see> for more
/// information.
/// </para>
/// <para>
/// Note that this class is implemented as a singleton, as it does not feature any meaningful state
/// that would justify having more than one instance at runtime.
/// </para>
/// </remarks>
internal sealed class IterativeFullMatrixCalculator : IDistanceCalculator {

    /// <summary>Maximum size for allocating the distances matrix on the stack.</summary>
    private const int MaxStackAllocSize = 256;

    /// <summary>Gets the instance of this <see cref="IterativeFullMatrixCalculator"/>.</summary>
    public static IterativeFullMatrixCalculator Instance { get; } = new();

    /// <summary>Initializes a new <see cref="IterativeFullMatrixCalculator"/>.</summary>
    /// <remarks>
    /// Note that this constructor is marked <see langword="private"/>,
    /// as the <see cref="IterativeFullMatrixCalculator"/> is implemented as a singleton.
    /// </remarks>
    private IterativeFullMatrixCalculator() { }

    public int Distance(ReadOnlySpan<char> source, ReadOnlySpan<char> target) {
        int rows = source.Length + 1;
        int columns = target.Length + 1;
        int size = rows * columns;
        // The matrix of edit distances is conceptually two-dimensional, but we use a
        // one-dimensional buffer for improved locality and performance. This also means that we
        // have to do indexing on our own, therefore the edit distance at (i, j) is found at
        // distances[(i * columns) + j].
        Span<int> distances = (size <= MaxStackAllocSize) ? stackalloc int[size] : new int[size];
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
                distances[(i * columns) + j] = Math.Min(Math.Min(insertion, deletion),
                    substitution);
            }
        }
        return distances[((rows - 1) * columns) + (columns - 1)];
    }

}