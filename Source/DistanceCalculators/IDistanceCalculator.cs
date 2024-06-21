using System;

namespace TriSpell.Source.DistanceCalculators;

/// <summary>Represents a calculator for determining the edit distance between two strings.</summary>
/// <remarks>
/// <para>
/// The edit distance, also known as Levenshtein distance after Soviet mathematician Vladimir
/// Levenshtein, is a measure of the similarity between two strings that may be used for basic
/// spellchecking.
/// </para>
/// <para>
/// More specifically, it is defined as the minimum number of single character edits (i. e.
/// insertions, deletions and substitutions) required to transform a source string into a target.
/// See <see href="https://en.wikipedia.org/wiki/Levenshtein_distance">Wikipedia</see> for more
/// information.
/// </para>
/// </remarks>
internal interface IDistanceCalculator {

    /// <summary>Determines the edit distance between two given strings.</summary>
    /// <remarks>
    /// <para>
    /// The edit distance is defined as the minimum number of single character edits (i. e.
    /// insertions, deletions and substitutions) required to transform a source string into a target.
    /// See <see href="https://en.wikipedia.org/wiki/Levenshtein_distance">Wikipedia</see> for more
    /// information.
    /// </para>
    /// <para>
    /// <example>
    /// An example for using this method might be the following:
    /// <code>
    /// IDistanceCalculator distanceCalculator = ...; 
    /// Console.WriteLine(distanceCalculator.Distance("Cat", "Car")); // Output: 1
    /// </code>
    /// </example>
    /// </para>
    /// </remarks>
    /// <param name="source">Source string to transform to <paramref name="target"/>.</param>
    /// <param name="target">Target string to transform <paramref name="source"/> to.</param>
    /// <returns>
    /// The edit distance between <paramref name="source"/> and <paramref name="target"/>.
    /// </returns>
    int Distance(ReadOnlySpan<char> source, ReadOnlySpan<char> target);

}