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
    public class KasaiLcpStrategyTest
    {
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
            TestHelper.AssertLectureLcpArray(lcpArray);
        }

        [TestMethod]
        public void KasaiLinearTimeStrategy_WithLargerFile_ShouldJustNotThrow()
        {
            // Arrange
            var text = File.ReadAllText("..\\..\\..\\..\\..\\loremipsumsmall.txt");
            var textBytes = Encoding.ASCII.GetBytes(text);
            textBytes = textBytes.Append((byte)0).ToArray();
            var suffixArrayBuilder = new SuffixArrayBuilder();
            var lcpStrategy = new KasaiLinearTimeLcpStrategy();
            var suffixArray = suffixArrayBuilder.BuildSuffixArray(textBytes);

            // Act
            var lcp = lcpStrategy.ComputeLcpArray(textBytes, suffixArray);

            // Assert
            lcp.Should().Equal(new NaiveLcpStrategy().ComputeLcpArrayParallel(textBytes, suffixArray));
        }
    }
}
