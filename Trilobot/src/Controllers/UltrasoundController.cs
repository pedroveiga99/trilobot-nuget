using System.Device.Gpio;
using System.Diagnostics;

namespace Trilobot.Controllers;

public class UltrasoundController : IDisposable
{
    GpioController gpio;

    static readonly TimeSpan offset = TimeSpan.FromMilliseconds(190);

    public UltrasoundController(GpioController gpioController)
    {
        gpio = gpioController;

        gpio.OpenPin(TrilobotPins.ULTRA_TRIG_PIN, PinMode.Output);
        gpio.OpenPin(TrilobotPins.ULTRA_ECHO_PIN, PinMode.Input);
    }

    /// <summary>
    /// Distance in cm from the ultrasound sensor.
    /// To give more stable readings, this method will attempt to take several
    /// readings and return the average distance.
    /// </summary>
    /// <param name="samples">Determines how many readings to average</param>
    public async Task<double> ReadDistance(CancellationToken cancellationToken, int samples = 3)
    {
        Stopwatch timer = new ();
        TimeSpan totalTime = TimeSpan.Zero;

        for(int i  = 0; i < samples; i++)
        {
            timer.Reset();

            // Trigger
            gpio.Write(TrilobotPins.ULTRA_TRIG_PIN, PinValue.High);
            await Task.Delay(TimeSpan.FromMicroseconds(10), cancellationToken);
            gpio.Write(TrilobotPins.ULTRA_TRIG_PIN, PinValue.Low);

            // Wait for the ECHO pin to go high
            // wait for the pulse rise
            await gpio.WaitForEventAsync(TrilobotPins.ULTRA_ECHO_PIN, PinEventTypes.Rising, cancellationToken);
            timer.Start();

            // And wait for it to fall
            await gpio.WaitForEventAsync(TrilobotPins.ULTRA_ECHO_PIN, PinEventTypes.Falling, cancellationToken);
            timer.Stop();

            double elapsedTime = timer.ElapsedMilliseconds - offset.TotalMilliseconds;
            elapsedTime = Math.Max(elapsedTime, 0); // Prevent negative readings when offset was too high
        
            totalTime += TimeSpan.FromMilliseconds(elapsedTime);
        }

        return GetDistance(samples, totalTime);
    }

    public static double GetDistance(int samples, TimeSpan time) =>
        time.TotalMilliseconds * TrilobotConst.SPEED_OF_SOUND_CM_MS / (2 * samples);

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}
