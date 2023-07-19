// copyright kruglovvs kruglov.valentine@gmail.com

using Iot.Device.Button;
using Iot.Device.KeyMatrix;
using nanoFramework.Hardware.Esp32;
using System;
using System.Device.Gpio;
using System.Device.I2c;
using System.Diagnostics;
using System.Threading;

namespace ProjectESP32.Periphery.Constants
{
    public static class PinModes
    {
        public static readonly PinMode Button = PinMode.Input;
        public static readonly PinMode ButtonIn = PinMode.Input;
        public static readonly PinMode ButtonOut = PinMode.OutputOpenDrainPullUp;
        public static readonly PinMode SimpleSensor = PinMode.Input;
        public static readonly PinMode Mechanical = PinMode.Output;
        public static readonly PinMode SDA = PinMode.Output;
        public static readonly PinMode CLK = PinMode.Output;
        public static readonly PinMode Luminodiodes = PinMode.OutputOpenSourcePullDown;
        public static readonly PinMode A0 = PinMode.OutputOpenDrainPullUp;
        public static readonly PinMode ARes = PinMode.OutputOpenDrainPullUp;
        public static readonly PinMode ACs = PinMode.OutputOpenSourcePullDown;
    }
}

