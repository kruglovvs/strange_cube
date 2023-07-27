// Copyright kruglov.valentine@gmail.com KruglovVS.

using System.Device.Gpio;
using System.Device.Spi;
using System.Threading;
using System;

namespace Periphery.Displays
{
    public class St7565WO12864 : SpiDevice, IDisplay
    {
        private enum StateA0 : byte
        {
            Data = 1,
            Control = 0,
        }
        private GpioPin PinA0 { get; init; }
        private GpioPin PinARes { get; init; }
        internal St7565WO12864(int pinNumberA0, int pinNumberARes, int pinNumberACs, GpioController gpioController) : base(new SpiConnectionSettings(Constants.ID.SpiBus, pinNumberACs))
        {
            ConnectionSettings.ChipSelectLineActiveState = PinValue.Low;
            PinA0 = gpioController.OpenPin(pinNumberA0,Constants.PinModes.A0);
            PinARes = gpioController.OpenPin(pinNumberARes, Constants.PinModes.ARes);
            PinA0.Write(Constants.PinStartValues.A0);
            PinARes.Write(PinValue.Low);
            Thread.Sleep(10);
            PinARes.Write(PinValue.High);
        }
        public void SetImage(byte[] image)
        {
            PinARes.Write(PinValue.High);
            PinA0.Write((PinValue)(byte)StateA0.Data);
            Thread.Sleep(10);
            this.Write(new SpanByte(image));
        }
    }
}
