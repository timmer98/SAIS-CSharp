using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using TextIndexierung.SAIS;
using TextIndexierung.SAIS.LongestCommonPrefix;

namespace TextIndexierung.Benchmarks
{
    [SimpleJob]
    public class NaiveLcpConstructionBenchmark
    {
        private byte[] inputText = null!;

        private int[] suffixArray = null!;

        [GlobalSetup]
        public void Setup()
        {
            (inputText, suffixArray) = BenchmarkHelper.GlobalSetup();
        }

        [Benchmark]
        public void Parallel()
        {
            var lcpStrategy = new NaiveLcpStrategy();
            lcpStrategy.ComputeLcpArrayParallel(inputText, suffixArray);
        }

        [Benchmark]
        public void Sequential()
        {
            var lcpStrategy = new NaiveLcpStrategy();
            lcpStrategy.ComputeLcpArray(inputText, suffixArray);
        }
    }
}
