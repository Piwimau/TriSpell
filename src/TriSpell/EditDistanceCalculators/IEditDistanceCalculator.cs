using System;

namespace TriSpell.EditDistanceCalculators;

/// <summary>
/// Represents a calculator for determining the edit distance between two
/// strings.
/// </summary>
/// <remarks>
/// The edit distance, also known as "Levenshtein distance" after the Soviet
/// mathematician Vladimir Levenshtein, is a measure of the similarity between
/// two strings often used for basic spell checking. It is defined as the
/// minimum number of single character edits (i.e., insertions, deletions and
/// substitutions) required to transform a source into a target string. See
/// <see href="https://en.wikipedia.org/wiki/Levenshtein_distance">this article
/// </see>for more information.
/// </remarks>
internal interface IEditDistanceCalculator {

    /// <summary>Calculates the edit distance between two strings.</summary>
    /// <remarks>
    /// The edit distance is defined as the minimum number of single character
    /// edits (i. e., insertions, deletions and substitutions) required to
    /// transform a source string into a target string. See
    /// <see href="https://en.wikipedia.org/wiki/Levenshtein_distance">
    /// this article</see> for more information.
    /// </remarks>
    /// <param name="source">The source string.</param>
    /// <param name="target">The target string.</param>
    /// <returns>The edit distance between the two strings.</returns>
    int EditDistance(ReadOnlySpan<char> source, ReadOnlySpan<char> target);

}