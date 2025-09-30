using System;

namespace TriSpell.EditDistanceCalculators;

/// <summary>
/// Represents a naive, recursive implementation of an
/// <see cref="IEditDistanceCalculator"/>.
/// </summary>
/// <remarks>
/// This implementation is based on the original mathematical definition by
/// Vladimir Levenshtein from 1965. Although the algorithm is relatively
/// straightforward and easy to understand, it performs rather poorly
/// (especially on larger inputs), as a lot of the edit distances between the
/// prefixes of the strings are calculated redundantly. See <see href="https://en.wikipedia.org/wiki/Levenshtein_distance">this article</see>
/// for more information.
/// </remarks>
public sealed class RecursiveEditDistanceCalculator : IEditDistanceCalculator {

    /// <inheritdoc/>
    public string Description => "Recursive (Slow)";

    /// <summary>
    /// Initializes a new <see cref="RecursiveEditDistanceCalculator"/>.
    /// </summary>
    public RecursiveEditDistanceCalculator() { }

    /// <inheritdoc/>
    public int EditDistance(
        ReadOnlySpan<char> source,
        ReadOnlySpan<char> target
    ) {
        // If either of the strings is empty, the other can only be transformed
        // into the same form by inserting or deleting all characters.
        if (source.IsEmpty) {
            return target.Length;
        }
        if (target.IsEmpty) {
            return source.Length;
        }
        // First characters of source and target match, so the edit distance
        // only depends on the remaining characters.
        if (source[0] == target[0]) {
            return EditDistance(source[1..], target[1..]);
        }
        // By now we know that at least the first character is different and we
        // can calculate the edit distance by trying out all three possible
        // actions.
        int insertion = EditDistance(source, target[1..]);
        int deletion = EditDistance(source[1..], target);
        int substitution = EditDistance(source[1..], target[1..]);
        return 1 + Math.Min(Math.Min(insertion, deletion), substitution);
    }

}