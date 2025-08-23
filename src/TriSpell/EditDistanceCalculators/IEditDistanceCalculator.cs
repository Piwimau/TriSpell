using System;

namespace TriSpell.EditDistanceCalculators;

/// <summary>Represents a calculator for determining the edit distance of two strings.</summary>
/// <remarks>
/// The edit distance, also known as "Levenshtein distance" after Soviet mathematician Vladimir
/// Levenshtein, is a measure of the similarity between two strings that may be used for basic spell
/// checking. More specifically, it is defined as the minimum number of single character edits
/// (i.e., insertions, deletions and substitutions) required to transform a source into a target
/// string. See <see href="https://en.wikipedia.org/wiki/Levenshtein_distance">this article</see>
/// for more information.
/// </remarks>
public interface IEditDistanceCalculator {

    /// <summary>Gets a short description of this <see cref="IEditDistanceCalculator"/>.</summary>
    string Description { get; }

    /// <summary>Calculates the edit distance of two specified strings.</summary>
    /// <remarks>
    /// The edit distance is defined as the minimum number of single character edits (i. e.,
    /// insertions, deletions and substitutions) required to transform a source into a target
    /// string. See
    /// <see href="https://en.wikipedia.org/wiki/Levenshtein_distance">this article</see> for more
    /// information.
    /// </remarks>
    /// <param name="source">The source string to transform to <paramref name="target"/>.</param>
    /// <param name="target">The target string to transform <paramref name="source"/> to.</param>
    /// <returns>The edit distance of the two specified strings.</returns>
    int EditDistance(ReadOnlySpan<char> source, ReadOnlySpan<char> target);

}