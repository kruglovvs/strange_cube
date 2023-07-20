// Copyright kruglov.valentine@gmail.com KruglovVS.

namespace Periphery.Constants
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

