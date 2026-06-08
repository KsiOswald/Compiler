using System;
using System.IO;

internal class Program
{
    private static void Main(string[] args)
    {
        const string inputPath = "text4.txt";
        const string outputPath = "tokens.txt";

        InputOutput.Init(inputPath);

        using (StreamWriter writer = new StreamWriter(outputPath))
        {
            Parser parser = new Parser(writer);
            parser.Parse();
        }

        Console.WriteLine();
        Console.WriteLine($"Коды лексем записаны в {outputPath}");
        Console.ReadKey();
    }
}