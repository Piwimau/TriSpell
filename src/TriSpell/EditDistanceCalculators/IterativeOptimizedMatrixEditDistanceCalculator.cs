using System;

namespace TriSpell.EditDistanceCalculators;

/// <summary>
/// Represents an even more advanced, iterative implementation of an
/// <see cref="IEditDistanceCalculator"/>.
/// </summary>
/// <remarks>
/// This implementation builds upon the fundamental algorithm of the
/// <see cref="IterativeFullMatrixEditDistanceCalculator"/>. However, it makes
/// use of an important observation: For calculating the edit distance at a
/// certain position in the matrix, it is sufficient to consider only the
/// current and previous row of edit distances. By reducing the full matrix down
/// to only two rows, the memory footprint and runtime may improve
/// significantly.
/// </remarks>
public sealed class IterativeOptimizedMatrixEditDistanceCalculator :
    IEditDistanceCalculator {

    /// <summary>
    /// Maximum limit for allocating the distances rows on the stack.
    /// </summary>
    private const int MaxStackAllocLimit = 128;

    /// <inheritdoc/>
    public string Description => "Iterative Optimized Matrix (Very Fast)";

    /// <summary>
    /// Initializes a new
    /// <see cref="IterativeOptimizedMatrixEditDistanceCalculator"/>.
    /// </summary>
    public IterativeOptimizedMatrixEditDistanceCalculator() { }

    /// <inheritdoc/>
    public int EditDistance(
        ReadOnlySpan<char> source,
        ReadOnlySpan<char> target
    ) {
        int size = target.Length + 1;
        Span<int> previousDistances = (size <= MaxStackAllocLimit)
            ? stackalloc int[size]
            : new int[size];
        Span<int> currentDistances = (size <= MaxStackAllocLimit)
            ? stackalloc int[size]
            : new int[size];
        // Empty source prefix can only be transformed into target by inserting
        // all characters.
        for (int j = 0; j < previousDistances.Length; j++) {
            previousDistances[j] = j;
        }
        for (int i = 0; i < source.Length; i++) {
            currentDistances[0] = i + 1;
            for (int j = 0; j < target.Length; j++) {
                int insertion = currentDistances[j] + 1;
                int deletion = previousDistances[j + 1] + 1;
                int substitution = previousDistances[j]
                    + ((source[i] == target[j]) ? 0 : 1);
                currentDistances[j + 1] = Math.Min(
                    Math.Min(insertion, deletion),
                    substitution
                );
            }
            Span<int> temp = previousDistances;
            previousDistances = currentDistances;
            currentDistances = temp;
        }
        return previousDistances[target.Length];
    }

}