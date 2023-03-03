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
        // Parses the command line arguments into options object.
        var result = Parser.Default.ParseArguments<Options>(args); 

        if (result.Tag == ParserResultType.NotParsed)
        {
            System.Console.WriteLine("Could not parse command line options.");
            Environment.Exit(-1);
        }

        var options = result.Value;

        var textBytes = ReadText(options);

        Runner.MeasureAlgorithms(textBytes, options);
    }

    /// <summary>
    /// Reads the text from the command line argument.
    /// </summary>
    /// <param name="options">Command line options.</param>
    /// <returns>Input file read as byte array.</returns>
    public static byte[] ReadText(Options options)
    {
        var filePath = options.InputFile;
        var text = File.ReadAllText(filePath);
        var textBytes = Encoding.ASCII.GetBytes(text);
        return textBytes;
    }

    
}