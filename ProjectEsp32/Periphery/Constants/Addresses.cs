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
    public static class Addresses
    {
        public const byte TMP112 = 0b1001001;
        public const byte LSM6 = 0b1001001;
    }
}

