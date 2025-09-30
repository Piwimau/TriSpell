using TriSpell.EditDistanceCalculators;
using Xunit;

namespace TriSpell.Tests.EditDistanceCalculators;

public sealed class IterativeFullMatrixEditDistanceCalculatorTests {

    [Theory]
    [InlineData("", "", 0)]
    [InlineData("a", "a", 0)]
    [InlineData("abc", "abc", 0)]
    [InlineData("", "a", 1)]
    [InlineData("a", "", 1)]
    [InlineData("", "test", 4)]
    [InlineData("test", "", 4)]
    [InlineData("bc", "abc", 1)]
    [InlineData("ac", "abc", 1)]
    [InlineData("ab", "abc", 1)]
    [InlineData("abc", "bc", 1)]
    [InlineData("abc", "ac", 1)]
    [InlineData("abc", "ab", 1)]
    [InlineData("_bc", "abc", 1)]
    [InlineData("a_c", "abc", 1)]
    [InlineData("ab_", "abc", 1)]
    [InlineData("kitten", "sitting", 3)]
    [InlineData("saturday", "sunday", 3)]
    public void EditDistance_ReturnsExpectedResult_WhenGivenVariousInputs(
        string source,
        string target,
        int expectedEditDistance
    ) {
        IterativeFullMatrixEditDistanceCalculator calculator = new();
        Assert.Equal(
            expectedEditDistance,
            calculator.EditDistance(source, target)
        );
    }

}