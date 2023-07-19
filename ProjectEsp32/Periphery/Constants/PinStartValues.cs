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
    public static class PinStartValues
    {
        public static readonly PinValue ButtonOut = PinValue.High;
        public static readonly PinValue Mechanical = PinValue.Low;
        public static readonly PinValue Luminodiodes = PinValue.Low;
        public static readonly PinValue A0 = PinValue.High;
        public static readonly PinValue ARes = PinValue.High;
        public static readonly PinValue ACs = PinValue.High;
    }
}

