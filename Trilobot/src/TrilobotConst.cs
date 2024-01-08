namespace Trilobot;

public enum TrilobotButton
{
    BUTTON_A = 0,
    BUTTON_B = 1,
    BUTTON_X = 2,
    BUTTON_Y = 3
}

public enum TrilobotLed
{
    LIGHT_FRONT_RIGHT = 0,
    LIGHT_FRONT_LEFT = 1,
    LIGHT_MIDDLE_LEFT = 2,
    LIGHT_REAR_LEFT = 3,
    LIGHT_REAR_RIGHT = 4,
    LIGHT_MIDDLE_RIGHT = 5
}

public enum TrilobotMotor
{
    MOTOR_LEFT = 0,
    MOTOR_RIGHT = 1
}

public static class TrilobotConst{

    // Speed of sound is 343m/s which we need in cm/ns for our distance measure
    public const double SPEED_OF_SOUND_M_S = 343;
    public const double SPEED_OF_SOUND_CM_MS = 343 * 100 / 1E6;

    public static readonly TrilobotLed[] LIGHTS_LEFT =
        [TrilobotLed.LIGHT_FRONT_LEFT, TrilobotLed.LIGHT_MIDDLE_LEFT, TrilobotLed.LIGHT_REAR_LEFT];

    public static readonly TrilobotLed[] LIGHTS_RIGHT =
        [TrilobotLed.LIGHT_FRONT_RIGHT, TrilobotLed.LIGHT_MIDDLE_RIGHT, TrilobotLed.LIGHT_REAR_RIGHT];

    public static readonly TrilobotLed[] LIGHTS_FRONT =
        [TrilobotLed.LIGHT_FRONT_RIGHT, TrilobotLed.LIGHT_FRONT_LEFT];

    public static readonly TrilobotLed[] LIGHTS_MIDDLE =
        [TrilobotLed.LIGHT_MIDDLE_RIGHT, TrilobotLed.LIGHT_MIDDLE_LEFT];

    public static readonly TrilobotLed[] LIGHTS_REAR =
        [TrilobotLed.LIGHT_REAR_RIGHT, TrilobotLed.LIGHT_REAR_LEFT];

    public static readonly TrilobotLed[] LIGHTS_LEFT_DIAGONAL =
    [TrilobotLed.LIGHT_FRONT_LEFT, TrilobotLed.LIGHT_REAR_RIGHT];

    public static readonly TrilobotLed[] LIGHTS_RIGHT_DIAGONAL =
        [TrilobotLed.LIGHT_FRONT_RIGHT, TrilobotLed.LIGHT_REAR_LEFT];

    public const int NUM_BUTTONS = 4;
    public const int NUM_UNDERLIGHTS = 6;
    public const int NUM_MOTORS = 2;
}

public static class TrilobotPins
{
    // User button pins
    public const int BUTTON_A_PIN = 5;
    public const int BUTTON_B_PIN = 6;
    public const int BUTTON_X_PIN = 16;
    public const int BUTTON_Y_PIN = 24;
    public static readonly int[] ButtonPins = [BUTTON_A_PIN, BUTTON_B_PIN, BUTTON_X_PIN, BUTTON_Y_PIN];

    // Onboard LEDs pins (next to each button)
    public const int LED_A_PIN = 23;
    public const int LED_B_PIN = 22;
    public const int LED_X_PIN = 17;
    public const int LED_Y_PIN = 27;
    public static readonly int[] ButtonLedsPins = [LED_A_PIN, LED_B_PIN, LED_X_PIN, LED_Y_PIN];

    // Motor driver pins, via DRV8833PWP Dual H-Bridge
    public const int MOTOR_EN_PIN = 26;
    public const int MOTOR_LEFT_P = 8;
    public const int MOTOR_LEFT_N = 11;
    public const int MOTOR_RIGHT_P = 10;
    public const int MOTOR_RIGHT_N = 9;

    // HC-SR04 Ultrasound pins
    public const int ULTRA_TRIG_PIN = 13;
    public const int ULTRA_ECHO_PIN = 25;

    // Servo / WS2812 pin
    public const int SERVO_PIN = 12;

    // SN3218 LED Driver pin
    public const int UNDERLIGHTING_EN_PIN = 7;
}
