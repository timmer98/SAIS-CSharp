using System.Collections;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Exporters.Csv;

namespace TextIndexierung.Benchmarks
{
    [SimpleJob]
    [MarkdownExporter]
    [CsvExporter(CsvSeparator.Comma)]
    public class BitArrayBenchmarks
    {
        [Params(100_000, 1_000_000)] public int N;

        private const int SEED = 0;

        [Benchmark]
        public void BitArray()
        {
            var random = new Random(SEED);
            var array = new BitArray(N);

            for (var i = 0; i < N; i++)
            {
                var randomNumber = random.Next(N);
                array[randomNumber] = !array[randomNumber];
            }
        }

        [Benchmark(Baseline = true)]
        public void BoolArray()
        {
            var random = new Random(SEED);
            var array = new bool[N];

            for (var i = 0; i < N; i++)
            {
                var randomNumber = random.Next(N);
                array[randomNumber] = !array[randomNumber];
            }
        }

        [Benchmark]
        public void MyBitArray()
        {
            var random = new Random(SEED);
            var array = new MyBitArray(N);

            for (var i = 0; i < N; i++)
            {
                var randomNumber = random.Next(N);
                array[randomNumber] = !array[randomNumber];
            }
        }
    }
}