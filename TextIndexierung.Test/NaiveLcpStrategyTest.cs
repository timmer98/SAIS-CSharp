using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TextIndexierung.SAIS;
using TextIndexierung.SAIS.LongestCommonPrefix;

namespace TextIndexierung.Test
{
    [TestClass]
    public class NaiveLcpStrategyTest
    {
        [TestMethod]
        public void NaiveLcpStrategy_WithLectureString_ShouldReturnCorrectLcp()
        {
            // Arrange
            var inputText = SuffixArrayBuilderTest.LECTURE_TEST_STRING;
            var suffixArrayBuilder = new SuffixArrayBuilder();
            var naiveStrategy = new NaiveLcpStrategy();
            var suffixArray = suffixArrayBuilder.BuildSuffixArray(inputText);

            // Act
            var lcpArray = naiveStrategy.ComputeLcpArray(inputText, suffixArray);

            // Assert
            TestHelper.AssertLectureLcpArray(lcpArray);
        }

        [TestMethod]
        public void NaiveLcpStrategy_WithLargerFile_ShouldJustNotThrow()
        {
            // Arrange
            var text = File.ReadAllText("..\\..\\..\\..\\..\\loremipsumsmall.txt");
            var textBytes = Encoding.ASCII.GetBytes(text);
            textBytes = textBytes.Append((byte)0).ToArray();
            var suffixArrayBuilder = new SuffixArrayBuilder();
            var lcpStrategy = new NaiveLcpStrategy();
            var suffixArray = suffixArrayBuilder.BuildSuffixArray(textBytes);

            // Act
            var lcp = lcpStrategy.ComputeLcpArrayParallel(textBytes, suffixArray);
        }
    }
}