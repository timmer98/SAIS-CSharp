using CommandLine;

namespace TextIndexierung.Console
{
    internal class Options
    {
        [Value(0, Required = true, MetaName = "File path",
            HelpText = "File path to the file to build the suffix array for.")]
        public string InputFile { get; set; } = "";

        [Option('n', "naive", Default = true, HelpText = "set false to prevent running the slow naive lcp algorithm.")]
        public bool ShouldRunNaiveLcp { get; set; }
    }
}
