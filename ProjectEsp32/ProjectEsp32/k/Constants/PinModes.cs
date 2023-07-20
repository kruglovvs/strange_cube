// Copyright kruglov.valentine@gmail.com KruglovVS.

using System.Device.Gpio;

namespace Periphery.Constants
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

