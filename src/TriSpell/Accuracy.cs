namespace TriSpell;

/// <summary>Represents a level of accuracy used for spell checking.</summary>
public enum Accuracy {

    /// <summary>
    /// The lowest level of accuracy, which finds many similar candidates. Use this when you want
    /// recall over precision.
    /// </summary>
    Low,

    /// <summary>
    /// The medium level of accuracy, which is a compromise between recall and precision.
    /// </summary>
    Medium,

    /// <summary>
    /// The highest level of accuracy, which finds only few similar candidates. Use this when you
    /// want high precision over recall.
    /// </summary>
    High

}