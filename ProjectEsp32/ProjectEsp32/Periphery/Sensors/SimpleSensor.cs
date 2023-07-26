// Copyright kruglov.valentine@gmail.com KruglovVS.

using System;
using System.Device.Gpio;
using System.Diagnostics;

namespace Periphery.Sensors
{
    public class SimpleSensor : ISimpleSensor
    {
        private GpioPin Pin { get; init; }
        internal SimpleSensor(int pinNumber, GpioController gpioController)
        {
            Pin = gpioController.OpenPin(pinNumber, Constants.PinModes.SimpleSensor);
            Pin.ValueChanged += (sender, e) =>
            {

                if (!(bool)Pin.Read())
                {
                    Sensored?.Invoke();
                }
            };
        }
        public event ISimpleSensor.SimpleEventHandler Sensored;
    }
}