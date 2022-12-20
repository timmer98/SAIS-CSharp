using System.Diagnostics;
using System.Text;
using TextIndexierung.SAIS;
using TextIndexierung.SAIS.LongestCommonPrefix;

namespace TextIndexierung.Console;

internal class Program
{
    public static void Main(string[] args)
    {
        var filePath = args[0];
        var text = File.ReadAllText(filePath);
        var textBytes = Encoding.ASCII.GetBytes(text);
        textBytes = textBytes.Append((byte)0).ToArray();
        var memoryManager = new MemoryManager();

        var saisStopwath = Stopwatch.StartNew();

        var suffixArrayBuilder = new SuffixArrayBuilder(memoryManager);
        var suffixArray = suffixArrayBuilder.BuildSuffixArray(textBytes);

        saisStopwath.Stop();

        var saisPeakMemory = (double)(memoryManager.PeakMemoryInBytes / Math.Pow(10, 6));

        var checkResult = SuffixArrayChecker.Check(textBytes, suffixArray, textBytes.Length, true);

        var naiveLcp = new NaiveLcpStrategy();
        var kasaiLcp = new KasaiLinearTimeLcpStrategy();
        var phiLcp = new PhiLinearTimeLcpStrategy();

        var naiveStopwatch = Stopwatch.StartNew();
        naiveLcp.ComputeLcpArrayParallel(textBytes, suffixArray);
        naiveStopwatch.Stop();

        var kasaiStopwatch = Stopwatch.StartNew();
        kasaiLcp.ComputeLcpArray(textBytes, suffixArray);
        kasaiStopwatch.Stop();

        var phiStopwatch = Stopwatch.StartNew();
        phiLcp.ComputeLcpArray(textBytes, suffixArray);
        phiStopwatch.Stop();
        

        System.Console.WriteLine($"RESULT name=\"Name\" sa_construction_time={saisStopwath.ElapsedMilliseconds} ms sa_construction_memory={saisPeakMemory.ToString("0.00")} lcp_naive_construction_time={naiveStopwatch.ElapsedMilliseconds} lcp_kasai_construction_time={kasaiStopwatch.ElapsedMilliseconds} lcp_phi_construction_time={phiStopwatch.ElapsedMilliseconds}");
    }
}