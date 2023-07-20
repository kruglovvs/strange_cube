// Copyright kruglov.valentine@gmail.com KruglovVS.

using System.Device.Gpio;

namespace Periphery.Constants
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

