using System.Data;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System;
using CommandLine;
using TextIndexierung.SAIS;
using TextIndexierung.SAIS.LongestCommonPrefix;

namespace TextIndexierung.Console;

internal class Program
{
    public static void Main(string[] args)
    {
        var result = Parser.Default.ParseArguments<Options>(args);

        if (result.Tag == ParserResultType.NotParsed)
        {
            System.Console.WriteLine("Could not parse command line options.");
            Environment.Exit(-1);
        }

        var options = result.Value;

        var textBytes = ReadText(options);

        MeasureAlgorithms(textBytes, options);
    }

    public static byte[] ReadText(Options options)
    {
        var filePath = options.InputFile;
        var text = File.ReadAllText(filePath);
        var textBytes = Encoding.ASCII.GetBytes(text);
        return textBytes;
    }

    private static void MeasureAlgorithms(byte[] textBytes, Options options)
    {
        var memoryManager = new MemoryManager();

        var saisStopwath = Stopwatch.StartNew();

        var suffixArrayBuilder = new SuffixArrayBuilder(memoryManager);
        var suffixArray = suffixArrayBuilder.BuildSuffixArray(textBytes);

        saisStopwath.Stop();

        var saisPeakMemory = (double)(memoryManager.PeakMemoryInBytes / Math.Pow(10, 6));

        var checkResult = SuffixArrayChecker.Check(textBytes, suffixArray, textBytes.Length, true);

        var builder = new StringBuilder($"RESULT name=\"Name\" sa_construction_time={saisStopwath.ElapsedMilliseconds} ms sa_construction_memory={saisPeakMemory:0.00} ");

        if (options.ShouldRunNaiveLcp)
        {
            var naiveStopwatch = MeasureNaiveLcp(textBytes, suffixArray);
            builder.Append($"lcp_naive_construction_time ={naiveStopwatch.ElapsedMilliseconds} ");
        }

        var kasaiStopwatch = MeasureKasaiLcp(textBytes, suffixArray);
        var phiStopwatch = MeasurePhiLcp(textBytes, suffixArray);

        builder.Append(
            $"lcp_kasai_construction_time ={kasaiStopwatch.ElapsedMilliseconds} lcp_phi_construction_time ={phiStopwatch.ElapsedMilliseconds}");

        System.Console.WriteLine(builder.ToString());
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