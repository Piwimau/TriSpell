namespace TriSpell;

/// <summary>Represents a level of accuracy used for spell checking.</summary>
public enum Accuracy {

    /// <summary>
    /// The lowest level of accuracy, which may result in more false positives and negatives.
    /// </summary>
    Low,

    /// <summary>
    /// The medium level of accuracy, which balances performance and correctness.
    /// </summary>
    Medium,

    /// <summary>
    /// The highest level of accuracy, which may be slower but provides the most reliable
    /// results.
    /// </summary>
    High

}