# Trilobot

This is a nuget package for controlling the [Trilobot mid-level robot learning platform](https://shop.pimoroni.com/products/trilobot?variant=39594077093971). This is a C# implementation based on the [official python library](https://github.com/pimoroni/trilobot-python).

## Pre-requisites

You must enable:

* i2c: `sudo raspi-config nonint do_i2c 0`

You can optionally run `sudo raspi-config` or the graphical Raspberry Pi Configuration UI to enable interfaces.

## Getting Started

**ON PROGRESS**

To start using your Trilobot, you need to create a `TrilobotController` object. 

```csharp
using Trilobot;

TrilobotController trilobot = new();

// Turns ON all button LEDs, waits the interval given, turns OFF all button LEDs
await trilobot.Buttons.BlinkButtonLeds(TimeSpan.FromSeconds(1));

await trilobot.Motors.MotorTest(TimeSpan.FromSeconds(1));
```

## Examples

There are many examples to get you started with your Trilobot. Go to `./Trilobot/examples` and run the `TrilobotExamples` project. This is a simple Console App that let's you run multiple examples. In this project you can also chech the multiple examples code.
