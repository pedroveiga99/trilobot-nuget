using Trilobot;

namespace TrilobotExamples;
public static class FlashButtons
{
    public static async Task Run(TrilobotController trilobot)
    {
        string help = """"
            Shows how to turn the button LEDs on and off, by having them flash.
            It will print if the button is pressed, double pressed and hold.
            """";
        Console.WriteLine(help);

        string exitHelp = "Press \"e\" to exit this example.";
        Console.WriteLine(exitHelp);

        int flashes = 10;  // How many times to flash the LEDs
        TimeSpan interval = TimeSpan.FromSeconds(0.3);  // Control the speed of the LED animation

        for (int i = 0; i < flashes; i++)
        {
            Console.WriteLine($"Flash {i}");

            trilobot.Buttons.SetAllButtonLeds(true);

            await Task.Delay(interval);

            trilobot.Buttons.SetAllButtonLeds(false);
        }
    }
}
