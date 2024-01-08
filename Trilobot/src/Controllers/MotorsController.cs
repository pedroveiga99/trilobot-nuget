using System.Device.Gpio;
using System.Device.Pwm.Drivers;

namespace Trilobot.Controllers;
public class MotorsController : IDisposable
{
    GpioController gpio;
    readonly Dictionary<int, SoftwarePwmChannel> motorPwmMapping = [];

    public MotorsController(GpioController gpioController)
    {
        gpio = gpioController;
        gpio.OpenPin(TrilobotPins.MOTOR_EN_PIN, PinMode.Output);

        SetupMotorPwm(TrilobotPins.MOTOR_LEFT_P);
        SetupMotorPwm(TrilobotPins.MOTOR_LEFT_N);
        SetupMotorPwm(TrilobotPins.MOTOR_RIGHT_P);
        SetupMotorPwm(TrilobotPins.MOTOR_RIGHT_N);
    }

    public MotorsController() : this(new GpioController()) { }

    private void SetupMotorPwm(int pinNumber)
    {
        SoftwarePwmChannel pwm = new(pinNumber, 100, 0);
        pwm.Start();

        motorPwmMapping.Add(pinNumber, pwm);
    }

    /// <summary>
    /// Sets the speed of the given motor
    /// </summary>
    /// <param name="motor">The ID of the motor to set the state of</param>
    /// <param name="speed">The motor speed, between -1.0 and 1.0</param>
    public void SetMotorSpeed(TrilobotMotor motor, double speed)
    {
        // Limit the speed value rather than throw a value exception
        double actualSpeed = Math.Max(Math.Min(speed, 1), -1);

        gpio.Write(TrilobotPins.MOTOR_EN_PIN, PinValue.High);

        SoftwarePwmChannel pwmP;
        SoftwarePwmChannel pwmN;

        if (motor == TrilobotMotor.MOTOR_LEFT)
        {
            pwmP = motorPwmMapping[TrilobotPins.MOTOR_LEFT_N];
            pwmN = motorPwmMapping[TrilobotPins.MOTOR_LEFT_P];
        }
        else
        {
            pwmP = motorPwmMapping[TrilobotPins.MOTOR_RIGHT_P];
            pwmN = motorPwmMapping[TrilobotPins.MOTOR_RIGHT_N];
        }

        if (actualSpeed > 0)
        {
            pwmP.DutyCycle = 1;
            pwmN.DutyCycle = 1 - actualSpeed;
        }
        else if (actualSpeed < 0)
        {
            pwmP.DutyCycle = 1 + actualSpeed;
            pwmN.DutyCycle = 1;
        }
        else
        {
            pwmP.DutyCycle = 1;
            pwmN.DutyCycle = 0;
        }
    }

    /// <summary>
    /// Sets both motors ate the same speed
    /// </summary>
    /// <param name="speed">The motor speed, between -1.0 and 1.0</param>
    public void SetBothMotorSpeed(double speed)
    {
        SetMotorSpeed(TrilobotMotor.MOTOR_LEFT, speed);
        SetMotorSpeed(TrilobotMotor.MOTOR_RIGHT, speed);
    }

    /// <summary>
    /// Disables both motors, allowing them to spin freely
    /// </summary>
    public void DisableMotors()
    {
        gpio.Write(TrilobotPins.MOTOR_EN_PIN, PinValue.Low);

        motorPwmMapping[TrilobotPins.MOTOR_LEFT_N].DutyCycle = 0;
        motorPwmMapping[TrilobotPins.MOTOR_LEFT_P].DutyCycle = 0;
        motorPwmMapping[TrilobotPins.MOTOR_RIGHT_N].DutyCycle = 0;
        motorPwmMapping[TrilobotPins.MOTOR_RIGHT_P].DutyCycle = 0;
    }

    public async Task MotorTest(TimeSpan interval)
    {
        SetBothMotorSpeed(1);
        await Task.Delay(interval);
        DisableMotors();
    }

    public void Dispose()
    {
        foreach (SoftwarePwmChannel motorPwm in motorPwmMapping.Values)
        {
            motorPwm.Dispose();
        }
    }
}
