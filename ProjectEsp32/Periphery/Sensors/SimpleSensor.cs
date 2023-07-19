// copyright kruglovvs kruglov.valentine@gmail.com

using System;
using System.Device.Gpio;

namespace ProjectESP32.Periphery.Sensors
{
    public class SimpleSensor : ISimpleSensor
    {
        private GpioPin Pin { get; init; }
        internal SimpleSensor(int pinNumber)
        {
            Pin = PeripheryController.OpenPin(pinNumber, Constants.PinModes.SimpleSensor);
            Pin.ValueChanged += (sender, e) =>
            {
                if ((bool)Pin.Read())
                {
                    Sensored.Invoke(this, new EventArgs());
                }
            };
        }
        public event EventHandler Sensored;
    }
}