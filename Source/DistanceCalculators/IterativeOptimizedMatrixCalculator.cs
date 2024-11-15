using System;

namespace TriSpell.Source.DistanceCalculators;

/// <summary>
/// Represents an even more advanced implementation of an <see cref="IDistanceCalculator"/>.
/// </summary>
/// <remarks>
/// This implementation builds upon the fundamental algorithm of the
/// <see cref="IterativeFullMatrixCalculator"/>. However, it makes use of an important observation:
/// For the purpose of calculating the edit distance at a certain position in the matrix, it is
/// sufficient to consider only the current and previous row of edit distances. By therefore
/// reducing the full matrix down to only two rows, the memory footprint and runtime may improve
/// significantly.
/// <para/>
/// Note that this class is implemented as a singleton, as it does not feature any meaningful state
/// that would justify having more than one instance at runtime.
/// </remarks>
internal sealed class IterativeOptimizedMatrixCalculator : IDistanceCalculator {

    /// <summary>Maximum limit for allocating the distances rows on the stack.</summary>
    private const int MaxStackLimit = 128;

    /// <summary>
    /// Gets the instance of this <see cref="IterativeOptimizedMatrixCalculator"/>.
    /// </summary>
    public static IterativeOptimizedMatrixCalculator Instance { get; } = new();

    /// <summary>Initializes a new <see cref="IterativeOptimizedMatrixCalculator"/>.</summary>
    /// <remarks>
    /// Note that this constructor is marked <see langword="private"/>,
    /// as <see cref="IterativeOptimizedMatrixCalculator"/> is implemented as a singleton.
    /// </remarks>
    private IterativeOptimizedMatrixCalculator() { }

    public int Distance(ReadOnlySpan<char> source, ReadOnlySpan<char> target) {
        int size = target.Length + 1;
        Span<int> previousDistances = (size <= MaxStackLimit) ? stackalloc int[size] : new int[size];
        Span<int> currentDistances = (size <= MaxStackLimit) ? stackalloc int[size] : new int[size];
        // Empty source prefix can only be transformed into target by inserting all characters.
        for (int j = 0; j < previousDistances.Length; j++) {
            previousDistances[j] = j;
        }
        for (int i = 0; i < source.Length; i++) {
            currentDistances[0] = i + 1;
            for (int j = 0; j < target.Length; j++) {
                int insertion = currentDistances[j] + 1;
                int deletion = previousDistances[j + 1] + 1;
                int substitution = previousDistances[j] + ((source[i] == target[j]) ? 0 : 1);
                currentDistances[j + 1] = Math.Min(Math.Min(insertion, deletion), substitution);
            }
            // Swap the spans for the next iteration. Sadly, we can't use the new swap using tuples
            // syntax (i. e. (a, b) = (b, a)), since Span<T> can't be used as type argument.
            Span<int> temp = previousDistances;
            previousDistances = currentDistances;
            currentDistances = temp;
        }
        return previousDistances[target.Length];
    }

}