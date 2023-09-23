// Copyright kruglov.valentine@gmail.com KruglovVS.

using System.Device.Gpio;

namespace Periphery.Constants
{
    public static class PinModes
    {
        public const PinMode Button = PinMode.Input;
        public const PinMode ButtonIn = PinMode.Input;
        public const PinMode ButtonOut = PinMode.OutputOpenDrainPullUp;
        public const PinMode SimpleSensor = PinMode.Input;
        public const PinMode Mechanical = PinMode.Output;
        public const PinMode SDA = PinMode.Output;
        public const PinMode CLK = PinMode.Output;
        public const PinMode Luminodiodes = PinMode.OutputOpenSourcePullDown;
        public const PinMode A0 = PinMode.OutputOpenDrainPullUp;
        public const PinMode ARes = PinMode.OutputOpenDrainPullUp;
        public const PinMode ACs = PinMode.OutputOpenSourcePullDown;
    }
}

