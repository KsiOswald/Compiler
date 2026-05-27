using System;
using System.IO;

internal class Program
{
    private static void Main(string[] args)
    {
        const string inputPath = "text1.txt";
        const string outputPath = "tokens.txt";

        InputOutput.Init(inputPath);

        using (StreamWriter writer = new StreamWriter(outputPath))
        {
            while (!InputOutput.IsEof)
            {
                Sym sym = LexicalAnalyzer.NextSym();
                if (sym == Sym.Unknown) continue;

                writer.Write((byte)sym);
                writer.Write(' ');
            }
        }

        Console.WriteLine();
        Console.WriteLine($"Коды лексем записаны в {outputPath}");
        Console.ReadKey();
    }
}