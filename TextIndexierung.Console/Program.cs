﻿namespace TextIndexierung.Console;

internal class Program
{
    public static void Main(string[] args)
    {
        var filePath = args[1];
        var textBytes = File.ReadAllBytes(filePath);

        var suffixArrayBuilder = new SuffixArrayBuilder();
        var suffixArray = suffixArrayBuilder.BuildSuffixArray(textBytes);

    }
}