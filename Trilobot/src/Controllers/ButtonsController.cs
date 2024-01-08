using Iot.Device.Button;
using System.Device.Gpio;
using System.Device.Pwm.Drivers;
using UnitsNet;

namespace Trilobot.Controllers;
public class ButtonsController
{
    GpioController gpio;

    static readonly TimeSpan doublePressMaxTime = TimeSpan.FromSeconds(1);
    static readonly TimeSpan holdingMinTime = TimeSpan.FromSeconds(1);

    readonly Dictionary<TrilobotButton, SoftwarePwmChannel> ledPwmMapping = [];
    readonly Dictionary<TrilobotButton, GpioButton> buttons = [];
    readonly Dictionary<TrilobotButton, bool> buttonHoldingState = [];

    public event ButtonPressed? OnButtonPressed;
    public event ButtonPressed? OnButtonDoublePressed;
    public event ButtonPressed? OnButtonHolding;

    public ButtonsController(GpioController gpioController)
    {
        gpio = gpioController;

        // Setup user buttons
        SetupButton(TrilobotButton.BUTTON_A);
        SetupButton(TrilobotButton.BUTTON_B);
        SetupButton(TrilobotButton.BUTTON_X);
        SetupButton(TrilobotButton.BUTTON_Y);

        // Setup user LEDs
        SetupButtonLeds(TrilobotButton.BUTTON_A);
        SetupButtonLeds(TrilobotButton.BUTTON_B);
        SetupButtonLeds(TrilobotButton.BUTTON_X);
        SetupButtonLeds(TrilobotButton.BUTTON_Y);
    }

    public ButtonsController() : this(new GpioController()) { }

    #region Button
    private void SetupButton(TrilobotButton button)
    {
        int pinNumber = button switch
        {
            TrilobotButton.BUTTON_A => TrilobotPins.BUTTON_A_PIN,
            TrilobotButton.BUTTON_B => TrilobotPins.BUTTON_B_PIN,
            TrilobotButton.BUTTON_X => TrilobotPins.BUTTON_X_PIN,
            TrilobotButton.BUTTON_Y => TrilobotPins.BUTTON_Y_PIN,
            _ => throw new NotImplementedException(),
        };

        GpioButton gpioButton = new(
            pinNumber,
            isPullUp: true,
            doublePress: doublePressMaxTime,
            holding: holdingMinTime);

        gpioButton.IsDoublePressEnabled = true;
        gpioButton.IsHoldingEnabled = true;

        // Setup events
        gpioButton.Press += (sender, e) => Button_Press(button);
        gpioButton.DoublePress += (sender, e) => Button_DoublePress(button);
        gpioButton.Holding += (sender, e) => Button_Holding(e, button);

        // Add to dict
        buttons.Add(button, gpioButton);
        buttonHoldingState.Add(button, false);
    }

    private void Button_DoublePress(TrilobotButton button) =>
        OnButtonDoublePressed?.Invoke(button);

    private void Button_Press(TrilobotButton button) =>
        OnButtonPressed?.Invoke(button);

    private void Button_Holding(ButtonHoldingEventArgs e, TrilobotButton button)
    {
        switch (e.HoldingState)
        {
            case ButtonHoldingState.Started:
                buttonHoldingState[button] = true;
                OnButtonHolding?.Invoke(button);
                break;
            case ButtonHoldingState.Completed:
                buttonHoldingState[button] = false;
                break;
            case ButtonHoldingState.Canceled:
                buttonHoldingState[button] = false;
                break;
        }
    }

    /// <summary>
    /// Checks if the user is holding the button
    /// </summary>
    /// <param name="button"></param>
    /// <returns>Button holding state</returns>
    public bool GetHoldingState(TrilobotButton button) => buttonHoldingState[button];

    #endregion

    #region Button LEDs
    private void SetupButtonLeds(TrilobotButton button)
    {
        int pinNumber = button switch
        {
            TrilobotButton.BUTTON_A => TrilobotPins.LED_A_PIN,
            TrilobotButton.BUTTON_B => TrilobotPins.LED_B_PIN,
            TrilobotButton.BUTTON_X => TrilobotPins.LED_X_PIN,
            TrilobotButton.BUTTON_Y => TrilobotPins.LED_Y_PIN,
            _ => throw new NotImplementedException(),
        };

        SoftwarePwmChannel pwn = new(pinNumber, 2000, 0);
        pwn.Start();

        ledPwmMapping.Add(button, pwn);
    }

    /// <summary>
    /// Turns the given button LED to a brighness value
    /// </summary>
    /// <param name="button">ID of the button to set the state to</param>
    /// <param name="brightness">Brightness value between 0.0 and 1.0</param>
    public void SetButtonLed(TrilobotButton button, double brightness)
    {
        // Limit the brightness value rather than throw a value exception
        double actualBrightness = Math.Max(Math.Min(brightness, 1), 0);

        ledPwmMapping[button].DutyCycle = actualBrightness;
    }

    /// <summary>
    /// Turns the given button LED to ON or OFF at full brighness
    /// </summary>
    /// <param name="button">ID of the button to set the state to</param>
    /// <param name="state">Turns ON or OFF at full brighness</param>
    public void SetButtonLed(TrilobotButton button, bool state) =>
        SetButtonLed(button, state ? 1 : 0);

    /// <summary>
    /// Turns the all button LEDs to ON or OFF at full brighness
    /// </summary>
    /// <param name="state">Turns ON or OFF at full brighness</param>
    public void SetAllButtonLeds(bool state)
    {
        foreach (TrilobotButton pin in Enum.GetValues(typeof(TrilobotButton)))
        {
            SetButtonLed(pin, state);
        }
    }

    /// <summary>
    /// Turns the all button LEDs to a brighness value
    /// </summary>
    /// <param name="brightness">Brightness value between 0.0 and 1.0</param>
    public void SetAllButtonLeds(double brightness)
    {
        foreach (TrilobotButton pin in Enum.GetValues(typeof(TrilobotButton)))
        {
            SetButtonLed(pin, brightness);
        }
    }

    /// <summary>
    /// Turns ON all button LEDs, waits the interval given, turns OFF all button LEDs
    /// </summary>
    /// <param name="interval">Interval that the LEDs are turned ON</param>
    public async Task BlinkButtonLeds(TimeSpan interval)
    {
        SetAllButtonLeds(true);
        await Task.Delay(interval);
        SetAllButtonLeds(false);
    }

    #endregion

    public void Dispose()
    {
        foreach (SoftwarePwmChannel ledPwm in ledPwmMapping.Values)
        {
            ledPwm.Dispose();
        }

        foreach (KeyValuePair<TrilobotButton, GpioButton> buttonPair in buttons)
        {
            TrilobotButton buttonName = buttonPair.Key;
            GpioButton buttonValue = buttonPair.Value;

            buttonValue.Press -= (sender, e) => Button_Press(buttonName);
            buttonValue.DoublePress -= (sender, e) => Button_DoublePress(buttonName);
            buttonValue.Holding -= (sender, e) => Button_Holding(e, buttonName);

            buttonValue.Dispose();
        }
    }
}
