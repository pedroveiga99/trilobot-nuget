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

    #region Basic commands

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

    #endregion

    #region Helpers

    /// <summary>
    /// Drives Trilobot forward
    /// </summary>
    /// <param name="speed">Speed to drive at, between 0.0 and 1.0</param>
    public void Forward(double speed)
    {
        SetBothMotorSpeed(speed);
    }

    /// <summary>
    /// Drives Trilobot backward
    /// </summary>
    /// <param name="speed">Speed to drive at, between 0.0 and 1.0</param>
    public void Backward(double speed)
    {
        SetBothMotorSpeed(-speed);
    }

    /// <summary>
    /// Turns Trilobot left
    /// </summary>
    /// <param name="speed">Speed to drive at, between 0.0 and 1.0</param>
    public void TurnLeft(double speed)
    {
        SetMotorSpeed(TrilobotMotor.MOTOR_LEFT, -speed);
        SetMotorSpeed(TrilobotMotor.MOTOR_RIGHT, speed);
    }

    /// <summary>
    /// Turns Trilobot right
    /// </summary>
    /// <param name="speed">Speed to drive at, between 0.0 and 1.0</param>
    public void TurnRight(double speed)
    {
        SetMotorSpeed(TrilobotMotor.MOTOR_LEFT, speed);
        SetMotorSpeed(TrilobotMotor.MOTOR_RIGHT, -speed);
    }

    /// <summary>
    /// Drives Trilobot forward and to the left
    /// </summary>
    /// <param name="speed">Speed to drive at, between 0.0 and 1.0</param>
    public void CurveForwardLeft(double speed)
    {
        SetMotorSpeed(TrilobotMotor.MOTOR_LEFT, 0);
        SetMotorSpeed(TrilobotMotor.MOTOR_RIGHT, speed);
    }

    /// <summary>
    /// Drives Trilobot forward and to the right
    /// </summary>
    /// <param name="speed">Speed to drive at, between 0.0 and 1.0</param>
    public void CurveForwardRight(double speed)
    {
        SetMotorSpeed(TrilobotMotor.MOTOR_LEFT, speed);
        SetMotorSpeed(TrilobotMotor.MOTOR_RIGHT, 0);
    }

    /// <summary>
    /// Drives Trilobot backward and to the left
    /// </summary>
    /// <param name="speed">Speed to drive at, between 0.0 and 1.0</param>
    public void CurveBackwardLeft(double speed)
    {
        SetMotorSpeed(TrilobotMotor.MOTOR_LEFT, 0);
        SetMotorSpeed(TrilobotMotor.MOTOR_RIGHT, -speed);
    }

    /// <summary>
    /// Drives Trilobot backward and to the right
    /// </summary>
    /// <param name="speed">Speed to drive at, between 0.0 and 1.0</param>
    public void CurveBackwardRight(double speed)
    {
        SetMotorSpeed(TrilobotMotor.MOTOR_LEFT, -speed);
        SetMotorSpeed(TrilobotMotor.MOTOR_RIGHT, 0);
    }

    /// <summary>
    /// Stops Trilobot from driving, sharply
    /// </summary>
    public void Stop()
    {
        SetBothMotorSpeed(0);
    }

    /// <summary>
    /// Stops Trilobot from driving, slowly
    /// </summary>
    public void Coast()
    {
        DisableMotors();
    }

    #endregion

    public async Task MotorTest(TimeSpan interval)
    {
        SetBothMotorSpeed(1);
        await Task.Delay(interval);
        DisableMotors();
    }

    public void Dispose()
    {
        DisableMotors();
        foreach (SoftwarePwmChannel motorPwm in motorPwmMapping.Values)
        {
            motorPwm.Dispose();
        }
    }
}
