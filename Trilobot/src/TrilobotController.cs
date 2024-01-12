using System.Device.Gpio;
using Trilobot.Controllers;

namespace Trilobot;

public delegate void ButtonPressed(TrilobotButton button);

public class TrilobotController : IDisposable
{
    GpioController gpio;

    public MotorsController Motors { get; }
    public ButtonsController Buttons { get; }
    public UnderlightController Leds { get; }
    public UltrasoundController Ultrasound { get; }

    public TrilobotController()
    {
        gpio = new();

        // Setup buttons and leds
        Buttons = new(gpio);

        // Setup motor
        Motors = new(gpio);

        // Setup underlight
        Leds = new();

        // Setup ultrasound
        Ultrasound = new(gpio);
    }

    public void Close()
    {
        gpio.Dispose();

        Buttons.Dispose();
        Motors.Dispose();
        Leds.Dispose();
        Ultrasound.Dispose();
    }

    public void Dispose() =>
        Close();
}
