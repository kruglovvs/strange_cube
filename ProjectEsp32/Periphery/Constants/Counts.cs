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
    public static class Counts
    {
        public const int Buttons = 2;
        public const int ButtonMatrix = 3;
        public const int Luminodiodes = 9;
        public static class PixelBytes
        {
            public const int Luminodiodes = 12;
            public const int St7565WO12864 = 404;
        }
        public static class CountPixels
        {
            public const int Luminodiodes = 9;
            public const int St7565WO12864 = 128 * 64;
        }
    }
}

