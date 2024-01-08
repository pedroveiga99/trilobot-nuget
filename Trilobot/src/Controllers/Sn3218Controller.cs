// https://github.com/jcoliz/Iot-Frosting
// https://github.com/pimoroni/sn3218/blob/master/library/sn3218.py

using System.Device.I2c;

namespace Trilobot.Controllers;
public abstract class Sn3218Controller : IDisposable
{
    private const int BUS_ID = 1;
    private const byte I2C_ADDRESS = 0x54;
    private const byte CMD_ENABLE_OUTPUT = 0x00;
    private const byte CMD_SET_PWM_VALUES = 0x01;
    private const byte CMD_ENABLE_LEDS = 0x13;
    private const byte CMD_UPDATE = 0x16;
    private const byte CMD_RESET = 0x17;

    /// <summary>
    /// How many total lights are there in an SN3218 bank
    /// </summary>
    private const int NumberOfLights = 18;

    private readonly I2cDevice Device;

    public Sn3218Controller() 
    {
        I2cConnectionSettings i2cSettings = new(BUS_ID, I2C_ADDRESS);
        Device = I2cDevice.Create(i2cSettings);
    }

    /// <summary>
    /// Enables output
    /// </summary>
    public void Enable()
    {
        Device.Write(new byte[] { CMD_ENABLE_OUTPUT, 0x1 });
    }

    /// <summary>
    /// Disables output
    /// </summary>
    public void Disable()
    {
        Device.Write(new byte[] { CMD_ENABLE_OUTPUT, 0x0 });
    }

    /// <summary>
    /// Resets all internal registers
    /// </summary>
    public void Reset()
    {
        Device.Write(new byte[] { CMD_RESET, 0xff });
    }

    private void Update()
    {
        Device.Write(new byte[] { CMD_UPDATE, 0xff });
    }

    /// <summary>
    /// Enables or disables each LED channel. The first 18 bit values are
    /// used to determine the state of each channel (1=on, 0=off) if fewer
    /// than 18 bits are provided the remaining channels are turned off.
    /// </summary>
    /// <param name="enable_mask"></param>
    protected void EnableLeds(UInt32 enable_mask = 0x3ffff)
    {
        Device.Write(new byte[] { CMD_ENABLE_LEDS, (byte)(enable_mask & 0x3F), (byte)((enable_mask >> 6) & 0x3F), (byte)((enable_mask >> 12) & 0X3F) });
        Update();
    }

    /// <summary>
    /// Outputs a new set of values to the driver
    /// </summary>
    /// <param name="values">Channel numbers</param>
    /// <exception cref="InvalidOperationException">If values is not a list of 18 values</exception>
    protected void Output(IEnumerable<double> values)
    {
        if (values.Count() != NumberOfLights)
            throw new InvalidOperationException("Values must be a list of 18 values");

        byte[] output_buffer = new byte[18];
        int i = 0;

        // Write gamma-corrected values
        foreach (double value in values)
            output_buffer[i++] = (byte)(Math.Round(Math.Pow(256, value)) - 1);

        Output(output_buffer);
    }

    /// <summary>
    /// Outputs a new set of values to the driver directly
    /// </summary>
    /// <param name="output_buffer">Channel numbers</param>
    /// <exception cref="InvalidOperationException">If output buffer is not a list of 18 values</exception>
    protected void Output(byte[] output_buffer)
    {
        if (output_buffer.Length != NumberOfLights)
            throw new InvalidOperationException("Output_buffer must be a list of 18 values");

        Device.Write([ CMD_SET_PWM_VALUES, ..output_buffer ]);
        Update();
    }

    public void Dispose()
    {
        Device.Dispose();
    }
}
