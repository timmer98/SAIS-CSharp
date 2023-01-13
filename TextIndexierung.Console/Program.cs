using System.Diagnostics;
using System.Runtime.CompilerServices;
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
        var memoryManager = new MemoryManager();

        var saisStopwath = Stopwatch.StartNew();

        var suffixArrayBuilder = new SuffixArrayBuilder(memoryManager);
        var suffixArray = suffixArrayBuilder.BuildSuffixArray(textBytes);

        saisStopwath.Stop();

        var saisPeakMemory = (double)(memoryManager.PeakMemoryInBytes / Math.Pow(10, 6));

        var checkResult = SuffixArrayChecker.Check(textBytes, suffixArray, textBytes.Length, true);
        
        var naiveStopwatch = MeasureNaiveLcp(textBytes, suffixArray);
        var kasaiStopwatch = MeasureKasaiLcp(textBytes, suffixArray);
        var phiStopwatch = MeasurePhiLcp(textBytes, suffixArray);

        

        System.Console.WriteLine($"RESULT name=\"Name\" sa_construction_time={saisStopwath.ElapsedMilliseconds} ms sa_construction_memory={saisPeakMemory.ToString("0.00")} lcp_naive_construction_time={naiveStopwatch.ElapsedMilliseconds} lcp_kasai_construction_time={kasaiStopwatch.ElapsedMilliseconds} lcp_phi_construction_time={phiStopwatch.ElapsedMilliseconds}");
    }

    private static Stopwatch MeasureNaiveLcp(byte[] textBytes, ArraySegment<int> suffixArray)
    {
        var naiveLcp = new NaiveLcpStrategy();
        var naiveStopwatch = Stopwatch.StartNew();
        naiveLcp.ComputeLcpArrayParallel(textBytes, suffixArray);
        naiveStopwatch.Stop();

        return naiveStopwatch;
    }

    private static Stopwatch MeasureKasaiLcp(byte[] textBytes, ArraySegment<int> suffixArray)
    {
        var kasaiLcp = new KasaiLinearTimeLcpStrategy();
        var kasaiStopwatch = Stopwatch.StartNew();
        kasaiLcp.ComputeLcpArray(textBytes, suffixArray);
        kasaiStopwatch.Stop();
        return kasaiStopwatch;
    }

    private static Stopwatch MeasurePhiLcp(byte[] textBytes, ArraySegment<int> suffixArray)
    {
        var phiLcp = new PhiLinearTimeLcpStrategy();
        var phiStopwatch = Stopwatch.StartNew();
        phiLcp.ComputeLcpArray(textBytes, suffixArray);
        phiStopwatch.Stop();
        return phiStopwatch;
    }
}