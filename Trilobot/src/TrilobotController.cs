using System.Device.Gpio;
using Trilobot.Controllers;

namespace Trilobot;

public delegate void ButtonPressed(TrilobotButton button);

public class TrilobotController : IDisposable
{
    GpioController gpio;

    public readonly MotorsController motors;
    public readonly ButtonsController buttons;
    public readonly UnderlightController leds;

    public TrilobotController()
    {
        gpio = new();

        // Setup buttons and leds
        buttons = new(gpio);

        // Setup motor
        motors = new(gpio);

        // Setup underlight
        leds = new();
    }

    public void Close()
    {
        gpio.Dispose();

        buttons.Dispose();
        motors.Dispose();
        leds.Dispose();
    }

    public void Dispose() =>
        Close();
}
