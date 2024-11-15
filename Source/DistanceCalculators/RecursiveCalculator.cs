using System;

namespace TriSpell.Source.DistanceCalculators;

/// <summary>
/// Represents a naive, recursive implementation of an <see cref="IDistanceCalculator"/>.
/// </summary>
/// <remarks>
/// This implementation is based on the original mathematical definition by Vladimir Levenshtein
/// from 1965. Although the algorithm is relatively straightforward and easy to understand,
/// it performs rather poorly (especially on larger inputs), as a lot of the edit distances between
/// the prefixes of the strings are redundantly calculated.
/// See <see href="https://en.wikipedia.org/wiki/Levenshtein_distance">this article</see> for more
/// information.
/// <para/>
/// Note that this class is implemented as a singleton, as it does not feature any meaningful state
/// that would justify having more than one instance at runtime.
/// </remarks>
internal sealed class RecursiveCalculator : IDistanceCalculator {

    /// <summary>Gets the instance of this <see cref="RecursiveCalculator"/>.</summary>
    public static RecursiveCalculator Instance { get; } = new();

    /// <summary>Initializes a new <see cref="RecursiveCalculator"/>.</summary>
    /// <remarks>
    /// Note that this constructor is marked <see langword="private"/>,
    /// as <see cref="RecursiveCalculator"/> is implemented as a singleton.
    /// </remarks>
    private RecursiveCalculator() { }

    public int Distance(ReadOnlySpan<char> source, ReadOnlySpan<char> target) {
        // If either of the strings is empty, the other can only be transformed into the same form
        // by inserting or deleting all characters.
        if (source.IsEmpty) {
            return target.Length;
        }
        if (target.IsEmpty) {
            return source.Length;
        }
        // First characters of source and target match, so the edit distance only depends on the
        // remaining characters.
        if (source[0] == target[0]) {
            return Distance(source[1..], target[1..]);
        }
        // By now we know that at least the first character is different and we can calculate
        // the edit distance by trying out all three possible actions.
        int insertion = Distance(source, target[1..]);
        int deletion = Distance(source[1..], target);
        int substitution = Distance(source[1..], target[1..]);
        return 1 + Math.Min(Math.Min(insertion, deletion), substitution);
    }

}