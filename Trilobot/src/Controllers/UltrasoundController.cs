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
            CancellationToken cs = new CancellationTokenSource(100).Token;
            CancellationToken combined = CancellationTokenSource.CreateLinkedTokenSource(cs, cancellationToken).Token;

            // Trigger
            gpio.Write(TrilobotPins.ULTRA_TRIG_PIN, PinValue.High);
            await Task.Delay(TimeSpan.FromMicroseconds(10), cancellationToken);
            gpio.Write(TrilobotPins.ULTRA_TRIG_PIN, PinValue.Low);

            // Wait for the ECHO pin to go high
            // wait for the pulse rise
            await gpio.WaitForEventAsync(TrilobotPins.ULTRA_ECHO_PIN, PinEventTypes.Rising, combined);
            timer.Start();

            // And wait for it to fall
            await gpio.WaitForEventAsync(TrilobotPins.ULTRA_ECHO_PIN, PinEventTypes.Falling, combined);
            timer.Stop();

            double elapsedTime = timer.Elapsed.TotalMilliseconds - offset.TotalMilliseconds;
            elapsedTime = Math.Max(elapsedTime, 0); // Prevent negative readings when offset was too high
        
            totalTime += TimeSpan.FromMilliseconds(elapsedTime);
        }

        return GetDistance(samples, totalTime);
    }

    public async Task<double> ReadDistance(int timeout = 50, int samples = 3, long offset = 190000)
    {
        Stopwatch stopwatch = new ();
        int count = 0;
        double totalPulseDurations = 0;
        double distance = -999;

        gpio.OpenPin(TrilobotPins.ULTRA_TRIG_PIN, PinMode.Output);
        gpio.OpenPin(TrilobotPins.ULTRA_ECHO_PIN, PinMode.Input);

        while (count < samples && stopwatch.Elapsed.TotalMilliseconds < timeout)
        {
            CancellationTokenSource tokenSource = new (TimeSpan.FromMilliseconds(timeout));
            // Trigger the sensor
            gpio.Write(TrilobotPins.ULTRA_TRIG_PIN, PinValue.High);
            await Task.Delay(TimeSpan.FromMicroseconds(10)); // 10 microseconds
            gpio.Write(TrilobotPins.ULTRA_TRIG_PIN, PinValue.Low);

            stopwatch.Restart();

            // Wait for the ECHO pin to go high
            await gpio.WaitForEventAsync(TrilobotPins.ULTRA_ECHO_PIN, PinEventTypes.Rising, tokenSource.Token);

            double pulseStart = stopwatch.Elapsed.TotalMilliseconds;

            // And wait for it to go low
            await gpio.WaitForEventAsync(TrilobotPins.ULTRA_ECHO_PIN, PinEventTypes.Falling, tokenSource.Token);

            double pulseEnd = stopwatch.Elapsed.TotalMilliseconds;

            double pulseDuration = pulseEnd - pulseStart - offset * 1000;
            if (pulseDuration < 0)
            {
                pulseDuration = 0; // Prevent negative readings when offset was too high
            }

            if (pulseDuration < timeout * 10000) // Convert timeout to ticks
            {
                totalPulseDurations += pulseDuration;
                count++;
            }
        }

        // Calculate average distance in cm if any successful readings were made
        if (count > 0)
        {
            distance = totalPulseDurations * TrilobotConst.SPEED_OF_SOUND_CM_NS / (2.0 * count);
        }

        gpio.ClosePin(TrilobotPins.ULTRA_TRIG_PIN);
        gpio.ClosePin(TrilobotPins.ULTRA_ECHO_PIN);

        return distance;
    }

    public static double GetDistance(int samples, TimeSpan time) =>
        (time.TotalMilliseconds * TrilobotConst.SPEED_OF_SOUND_CM_MS) / (2 * samples);

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}
