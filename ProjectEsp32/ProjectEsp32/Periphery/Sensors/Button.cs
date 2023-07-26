// Copyright kruglov.valentine@gmail.com KruglovVS.

using Iot.Device.Button;
using System;
using System.Device.Gpio;

namespace Periphery.Sensors
{
    public class Button : GpioButton, ISimpleSensor
    {
        internal Button(int pinNumber, GpioController gpioController) : base(pinNumber, gpioController, false, Constants.PinModes.Button, new TimeSpan(Constants.Time.Debounce))
        {
            IsDoublePressEnabled = false;
            IsHoldingEnabled = false;
            IsPressed = true;
            Press += (sender, e) =>
            {
                Sensored?.Invoke();
            };
        }
        public event ISimpleSensor.SimpleEventHandler Sensored;
    }

}