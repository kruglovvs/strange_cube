// Copyright kruglov.valentine@gmail.com KruglovVS.

using System.Device.Gpio;

namespace Periphery.Constants
{
    public static class Pins
    {
        public static readonly int[] Buttons = { 35, 34 };
        public static readonly int[] ButtonsIn = { 5, 17, 16 };
        public static readonly int[] ButtonsOut = { 15, 2, 4 };
        public static readonly int Luminoides = 18;
        public static readonly int PhotoSensor = 25;
        public static readonly int VibrationSensor = 39;
        public static readonly int GasSensor = 36;
        public static readonly int SDA = 21;
        public static readonly int SCL = 22;
        public static readonly int INT1 = 19;
        public static readonly int INT2 = 23;
        public static readonly int MOSI = 13;
        public static readonly int MISO = 404;
        public static readonly int CLK = 14;
        public static readonly int A0 = 12;
        public static readonly int ARes = 26;
        public static readonly int ACs = 27;
        public static readonly int VibrationMotor = 33;
        public static readonly int Door = 32;
    }
}

