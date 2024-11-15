using System;

namespace TriSpell.Source.DistanceCalculators;

/// <summary>Represents a calculator for determining the edit distance of two strings.</summary>
/// <remarks>
/// The edit distance, also known as "Levenshtein distance" after Soviet mathematician Vladimir
/// Levenshtein, is a measure of the similarity between two strings that may be used for basic
/// spellchecking. More specifically, it is defined as the minimum number of single character edits
/// (i. e. insertions, deletions and substitutions) required to transform a source into a target
/// string. See <see href="https://en.wikipedia.org/wiki/Levenshtein_distance">this article</see>
/// for more information.
/// </remarks>
internal interface IDistanceCalculator {

    /// <summary>Determines the edit distance of two given strings.</summary>
    /// <remarks>
    /// The edit distance is defined as the minimum number of single character edits (i. e.
    /// insertions, deletions and substitutions) required to transform a source into a target
    /// string. See
    /// <see href="https://en.wikipedia.org/wiki/Levenshtein_distance">this article</see> for more
    /// information.
    /// </remarks>
    /// <param name="source">Source string to transform to <paramref name="target"/>.</param>
    /// <param name="target">Target string to transform <paramref name="source"/> to.</param>
    /// <returns>
    /// The edit distance of the given <paramref name="source"/> and <paramref name="target"/>
    /// strings.
    /// </returns>
    int Distance(ReadOnlySpan<char> source, ReadOnlySpan<char> target);

}