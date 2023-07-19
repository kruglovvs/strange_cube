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
    public static class Pins
    {
        public static readonly int[] Buttons = new int[Counts.Buttons] { 33, 32 }; //исправить
        public static readonly int[] ButtonsIn = new int[Counts.ButtonMatrix] { 5, 17, 16 };
        public static readonly int[] ButtonsOut = new int[Counts.ButtonMatrix] { 15, 2, 4 };
        public const int Luminoides = 404;
        public const int PhotoSensor = 25;
        public const int VibrationSensor = 39;
        public const int GasSensor = 36; // 
        public const int Alert = 404;
        public const int SDA = 21;
        public const int SCL = 22;
        public const int INT1 = 19;
        public const int INT2 = 23;
        public const int MOSI = 13;
        public const int CLK = 14;
        public const int A0 = 12;
        public const int ARes = 26;
        public const int ACs = 27;
        public const int VibrationMotor = 35;
        public const int Door = 34;
    }
}

