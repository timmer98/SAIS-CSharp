using System.IO;
using System.Linq;
using System.Text;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TextIndexierung.SAIS;
using TextIndexierung.SAIS.LongestCommonPrefix;

namespace TextIndexierung.Test
{
    [TestClass]
    public class PhiLinearTimeLcpStrategyTest
    {
        [TestMethod]
        public void PhiLinearTimeLcpStrategy_WithLectureText_ShouldReturnCorrectLcp()
        {
            // Arrange
            var inputText = SuffixArrayBuilderTest.LECTURE_TEST_STRING;
            var suffixArrayBuilder = new SuffixArrayBuilder();
            var lcpStrategy = new PhiLinearTimeLcpStrategy();
            var suffixArray = suffixArrayBuilder.BuildSuffixArray(inputText);

            // Act
            var lcpArray = lcpStrategy.ComputeLcpArray(inputText, suffixArray);

            // Assert
            lcpArray.Should().Equal(new NaiveLcpStrategy().ComputeLcpArrayParallel(inputText, suffixArray));
        }

        [TestMethod]
        public void PhiLinearTimeStrategy_WithLargerFile_ShouldBeSameAsNaive()
        {
            // Arrange
            var text = File.ReadAllText("..\\..\\..\\..\\..\\loremipsumsmall.txt");
            var textBytes = Encoding.ASCII.GetBytes(text);
            textBytes = textBytes.Append((byte)0).ToArray();
            var suffixArrayBuilder = new SuffixArrayBuilder();
            var lcpStrategy = new PhiLinearTimeLcpStrategy();
            var naiveStrategy = new NaiveLcpStrategy();
            var suffixArray = suffixArrayBuilder.BuildSuffixArray(textBytes);

            // Act
            var lcp = lcpStrategy.ComputeLcpArray(textBytes, suffixArray);

            // Assert
            var naiveLcp = naiveStrategy.ComputeLcpArrayParallel(textBytes, suffixArray);

            lcp.Should().Equal(naiveLcp);
        }

        [TestMethod]
        public void PhiLinearStrategy_WithRepeatingText_ShouldReturnCorrectLcp()
        {
            // Arrange
            var inputText = Encoding.ASCII.GetBytes("abcabc");
            var suffixArrayBuilder = new SuffixArrayBuilder();
            var lcpStrategy = new PhiLinearTimeLcpStrategy();
            var suffixArray = suffixArrayBuilder.BuildSuffixArray(inputText);

            // Act
            var lcpArray = lcpStrategy.ComputeLcpArray(inputText, suffixArray);

            // Assert
            lcpArray.Should().Equal(0, 3, 0, 2, 0, 1);
        }
    }
}
