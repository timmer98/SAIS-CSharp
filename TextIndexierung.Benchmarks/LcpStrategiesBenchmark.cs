using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using TextIndexierung.SAIS.LongestCommonPrefix;

namespace TextIndexierung.Benchmarks
{
    [SimpleJob(RunStrategy.ColdStart)]
    [RPlotExporter]
    public class LcpStrategiesBenchmark
    {
        private byte[] inputText = null!;

        private int[] suffixArray = null!;

        [GlobalSetup]
        public void Setup()
        {
            (inputText, suffixArray) = BenchmarkHelper.GlobalSetup();
        }

        [Benchmark]
        public void NaiveSequential()
        {
            var lcpStrategy = new NaiveLcpStrategy();
            lcpStrategy.ComputeLcpArray(inputText, suffixArray);
        }

        [Benchmark]
        public void Kasai()
        {
            var lcpStrategy = new KasaiLinearTimeLcpStrategy();
            lcpStrategy.ComputeLcpArray(inputText, suffixArray);
        }

        [Benchmark(Baseline = true)]
        public void Phi()
        {
            var lcpStrategy = new PhiLinearTimeLcpStrategy();
            lcpStrategy.ComputeLcpArray(inputText, suffixArray);
        }
    }
}
