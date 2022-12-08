using System.Text;
using sais = TextIndexierung.SAIS;

namespace TextIndexierung.Console;

internal class Program
{
    public static void Main(string[] args)
    {
        var filePath = args[0];
        var text = File.ReadAllText(filePath);
        var textBytes = Encoding.ASCII.GetBytes(text);
        textBytes = textBytes.Append((byte)0).ToArray();

        var suffixArrayBuilder = new SAIS.SuffixArrayBuilder();
        var suffixArray = suffixArrayBuilder.BuildSuffixArray(textBytes);

        var checkResult = SuffixArrayChecker.Check(textBytes, suffixArray, textBytes.Length, true);
    }
}