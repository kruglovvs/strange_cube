// copyright kruglovvs kruglov.valentine@gmail.com

using Iot.Device.Button;
using System;

namespace ProjectESP32.Periphery.Sensors
{
    public class Button : GpioButton, ISimpleSensor
    {
        internal Button(int pinNumber) : base(pinNumber, PeripheryController.GpioController, false, Constants.PinModes.Button, Constants.Time.Debounce)
        {
            IsDoublePressEnabled = false;
            IsHoldingEnabled = false;
            IsPressed = true;
            Press += (sender, e) =>
            {
                Sensored.Invoke(this, new EventArgs());
            };
        }
        public event EventHandler Sensored;
    }

}