internal class Program
{
    private static void Main(string[] args)
    {
        InputOutput.Init("text3.txt");
        while (!InputOutput.IsEof)
        {
            InputOutput.NextCh();
        }
        Console.ReadKey();
    }
}