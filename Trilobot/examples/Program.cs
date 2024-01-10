using Trilobot;

namespace TrilobotExamples;

internal class Program
{
    static async Task Main(string[] args)
    {
        TrilobotController trilobot = new();
        bool running = true;

        string help = """"
            mb => Buttons lister
            fb => Flash buttons LEDs

            exit => Exit Program
            """";

        while (running)
        {
            Console.WriteLine(help);

            string? example = Console.ReadLine();

            switch (example)
            {
                case "exit":
                    running = false;
                    break;
                case "mb":
                    MultipleButtons.Run(trilobot);
                    break;
                case "fb":
                    await FlashButtons.Run(trilobot);
                    break;
            }
        }
    }
}
