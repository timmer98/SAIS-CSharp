using System.Text;
using TextIndexierung.SAIS;

namespace TextIndexierung.Benchmarks
{
    internal class BenchmarkHelper
    {
        internal static (byte[] inputText, int[] suffixArray) GlobalSetup()
        {
            var text = File.ReadAllText("..\\..\\..\\..\\..\\..\\..\\..\\..\\..\\loremipsumsmall.txt");
            var textBytes = Encoding.ASCII.GetBytes(text);
            textBytes = textBytes.Append((byte)0).ToArray();
            var inputText = textBytes;
            var suffixArrayBuilder = new SuffixArrayBuilder();
            var suffixArray = suffixArrayBuilder.BuildSuffixArray(textBytes);

            return (inputText, suffixArray);
        }
    }
}
