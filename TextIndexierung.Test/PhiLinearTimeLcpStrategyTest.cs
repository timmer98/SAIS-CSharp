using System.IO;
using System.Linq;
using System.Text;
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
            TestHelper.AssertLectureLcpArray(lcpArray);
        }

        [TestMethod]
        public void PhiLinearTimeStrategy_WithLargerFile_ShouldJustNotThrow()
        {
            // Arrange
            var text = File.ReadAllText("..\\..\\..\\..\\..\\loremipsum.txt");
            var textBytes = Encoding.ASCII.GetBytes(text);
            textBytes = textBytes.Append((byte)0).ToArray();
            var suffixArrayBuilder = new SuffixArrayBuilder();
            var lcpStrategy = new PhiLinearTimeLcpStrategy();
            var suffixArray = suffixArrayBuilder.BuildSuffixArray(textBytes);

            // Act
            var lcp = lcpStrategy.ComputeLcpArray(textBytes, suffixArray);
        }
    }
}
