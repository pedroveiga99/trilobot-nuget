using Trilobot;

namespace TrilobotExamples;
public static class MultipleButtons
{
    public static void Run(TrilobotController trilobot)
    {
        string help = """"
            Shows how to read all of Trilobots buttons.
            It will print if the button is pressed, double pressed and hold.
            """";
        Console.WriteLine(help);

        string exitHelp = "Press \"e\" to exit this example.";
        Console.WriteLine(exitHelp);

        trilobot.buttons.OnButtonPressed += Trilobot_OnButtonPressed;
        trilobot.buttons.OnButtonDoublePressed += Trilobot_OnButtonDoublePressed;
        trilobot.buttons.OnButtonHolding += Trilobot_OnButtonHolding;

        char? k = null;
        while (k != 'e')
        {
            k = Console.ReadKey().KeyChar;
            Console.WriteLine(exitHelp);
        }

        trilobot.buttons.OnButtonPressed -= Trilobot_OnButtonPressed;
        trilobot.buttons.OnButtonDoublePressed -= Trilobot_OnButtonDoublePressed;
        trilobot.buttons.OnButtonHolding -= Trilobot_OnButtonHolding;
    }

    private static void Trilobot_OnButtonHolding(TrilobotButton button)
    {
        Console.WriteLine($"Holding button {button}");
    }

    private static void Trilobot_OnButtonDoublePressed(TrilobotButton button)
    {
        Console.WriteLine($"Double press button {button}");
    }

    private static void Trilobot_OnButtonPressed(TrilobotButton button)
    {
        Console.WriteLine($"Pressed button {button}");
    }
}
