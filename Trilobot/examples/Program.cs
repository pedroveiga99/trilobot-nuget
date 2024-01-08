using Trilobot;

namespace TrilobotExamples;

internal class Program
{
    static void Main(string[] args)
    {
        TrilobotController trilobot = new();

        string help = """"
            mb => Buttons lister example
            """";
        Console.WriteLine(help);

        string? a = Console.ReadLine();

        switch (a)
        {
            case "mb":
                MultipleButtons.Run(trilobot);
                break;
        }
    }
}
