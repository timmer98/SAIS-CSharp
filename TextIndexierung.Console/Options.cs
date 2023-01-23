using CommandLine;

namespace TextIndexierung.Console
{
    internal class Options
    {
        [Value(0, Required = true, MetaName = "File path",
            HelpText = "File path to the file to build the suffix array for.")]
        public string InputFile { get; set; } = "";

        [Option('n', "no-naive", HelpText = "Prevents running the naive algorithm.")]
        public bool ShouldNotRunNaiveLcp { get; set; }
    }
}
