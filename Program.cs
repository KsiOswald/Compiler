internal class Program
{
    private static void Main(string[] args)
    {
        InputOutput.Init("text1.txt");
        while (!InputOutput.IsEof)
        {
            if (InputOutput.Ch =='$')
            {
                InputOutput.Error(InputOutput.PositionNow, 5);
            }
            if (InputOutput.Ch == '#')
            {
                InputOutput.Error(InputOutput.PositionNow, 5);
            }
            if (InputOutput.Ch == '@')
            {
                InputOutput.Error(InputOutput.PositionNow, 5);
            }
            InputOutput.NextCh();
        }
        Console.ReadKey();
    }
}