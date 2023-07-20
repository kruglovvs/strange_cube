// Copyright kruglov.valentine@gmail.com KruglovVS.

namespace Periphery.Constants
{
    public static class Counts
    {
        public static readonly int Buttons = 2;
        public static readonly int ButtonMatrix = 3;
        public static readonly int Luminodiodes = 9;
        public static class PixelBytes
        {
            public static readonly int Luminodiodes = 12;
            public static readonly int St7565WO12864 = 404;
        }
        public static class CountPixels
        {
            public static readonly int Luminodiodes = 9;
            public static readonly int St7565WO12864 = 128 * 64;
        }
    }
}

