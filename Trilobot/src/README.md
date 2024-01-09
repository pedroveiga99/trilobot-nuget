# Trilobot

This is a nuget package for controlling the [Trilobot mid-level robot learning platform](https://shop.pimoroni.com/products/trilobot?variant=39594077093971). This is a C# implementation based on the [official python library](https://github.com/pimoroni/trilobot-python).

## Pre-requisites

You must enable:

* i2c: `sudo raspi-config nonint do_i2c 0`

You can optionally run `sudo raspi-config` or the graphical Raspberry Pi Configuration UI to enable interfaces.

## Getting Started

To start using your Trilobot, you need to create a `TrilobotController` object. This object has multiple properties to control the different Trilobot components.

* Motors => Control the Trilobot movement
* Buttons => Get button pressed and control the buttons LEDs
* Leds => Control the underlight LEDs
* Ultrasound => Measure ultrasound distance

## Examples

There are many examples to get you started with your Trilobot. Go to the [GitHub Repository](https://github.com/pedroveiga99/trilobot-nuget/tree/main/Trilobot/examples/README.md) to check them out.

Simple use case:
```csharp
using Trilobot;

TrilobotController trilobot = new();

// Turns ON all button LEDs, waits the interval given, turns OFF all button LEDs
await trilobot.Buttons.BlinkButtonLeds(TimeSpan.FromSeconds(1));
```