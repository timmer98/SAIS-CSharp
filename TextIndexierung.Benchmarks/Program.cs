using System.Collections;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using TextIndexierung.Benchmarks;

public class Program
{
#if DEBUG
    static void Main(string[] args) => BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, new DebugInProcessConfig());
#else
    static void Main(string[] args) => BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
#endif

}