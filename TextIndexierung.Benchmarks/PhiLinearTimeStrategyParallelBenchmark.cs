using BenchmarkDotNet.Attributes;
using TextIndexierung.SAIS.LongestCommonPrefix;

namespace TextIndexierung.Benchmarks
{
    [SimpleJob]
    public class PhiLinearTimeStrategyParallelBenchmark
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
            var lcpStrategy = new PhiLinearTimeLcpStrategy();
            lcpStrategy.ComputeParallelLcpArray(inputText, suffixArray);
        }

        [Benchmark]
        public void Sequential()
        {
            var lcpStrategy = new PhiLinearTimeLcpStrategy();
            lcpStrategy.ComputeLcpArray(inputText, suffixArray);
        }
    }
}
