// Copyright kruglov.valentine@gmail.com KruglovVS.

using System.Device.Gpio;
using System.Threading;

namespace Periphery.Displays
{
    public class St7565WO12864 : SpiDisplay, IDisplay, ITurningOn
    {
        public bool IsTurnedOn
        {
            get { return IsTurnedOn; }
            set
            {
                PinACs.Write(value);
                if (!value)
                {
                    PinA0.Write((byte)StateA0.Data);
                    PinARes.Write(PinValue.Low);
                } else
                {
                    PinARes.Write(PinValue.High);
                    Thread.Sleep(Constants.Time.SetARes);
                }
                IsTurnedOn = value;
            }
        }
        private enum StateA0 : byte
        {
            Data = 1,
            Control = 0,
        }
        private GpioPin PinA0 { get; init; }
        private GpioPin PinARes { get; init; }
        private GpioPin PinACs { get; init; }
        internal St7565WO12864(int pinNumberA0, int pinNumberARes, int pinNumberACs)
        {
            PinA0 = PeripheryController.OpenPin(pinNumberA0, Constants.PinModes.A0);
            PinARes = PeripheryController.OpenPin(pinNumberARes, Constants.PinModes.ARes);
            PinACs = PeripheryController.OpenPin(pinNumberACs, Constants.PinModes.ACs);
            IsTurnedOn = (bool)Constants.PinStartValues.ACs;
        }
        public class Image
        {

        }
        public void SetImage(byte[] image)
        {

        }
    }
}
