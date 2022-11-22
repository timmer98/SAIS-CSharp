using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TextIndexierung.LongestCommonPrefix;

namespace TextIndexierung.Test;

[TestClass]
public class LcpStrategyTest
{
    [TestMethod]
    public void NaiveStrategy_WithLectureText_ShouldReturnCorrectLcp()
    {
        // Arrange
        var inputText = SuffixArrayBuilderTest.LECTURE_TEST_STRING;
        var suffixArrayBuilder = new SuffixArrayBuilder();
        var lcpStrategy = new NaiveLcpStrategy();
        var suffixArray = suffixArrayBuilder.BuildSuffixArray(inputText);

        // Act
        var lcpArray = lcpStrategy.ComputeLcpArray(inputText, suffixArray);

        // Assert
        AssertLectureLcpArray(lcpArray);
    }

    [TestMethod]
    public void KasaiLinearStrategy_WithLectureText_ShouldReturnCorrectLcp()
    {
        // Arrange
        var inputText = SuffixArrayBuilderTest.LECTURE_TEST_STRING;
        var suffixArrayBuilder = new SuffixArrayBuilder();
        var lcpStrategy = new KasaiLinearTimeLcpStrategy();
        var suffixArray = suffixArrayBuilder.BuildSuffixArray(inputText);

        // Act
        var lcpArray = lcpStrategy.ComputeLcpArray(inputText, suffixArray);

        // Assert
        AssertLectureLcpArray(lcpArray);
    }

    private void AssertLectureLcpArray(int[] lcpArray)
    {
        lcpArray[0].Should().Be(0);
        lcpArray[1].Should().Be(0);
        lcpArray[2].Should().Be(1);
        lcpArray[3].Should().Be(2);
        lcpArray[4].Should().Be(2);
        lcpArray[5].Should().Be(5);
        lcpArray[6].Should().Be(0);
        lcpArray[7].Should().Be(2);
        lcpArray[8].Should().Be(1);
        lcpArray[9].Should().Be(1);
        lcpArray[10].Should().Be(4);
        lcpArray[11].Should().Be(0);
        lcpArray[12].Should().Be(3);
    }
}