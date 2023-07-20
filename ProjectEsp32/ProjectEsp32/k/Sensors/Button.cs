// Copyright kruglov.valentine@gmail.com KruglovVS.

using Iot.Device.Button;
using System;

namespace Periphery.Sensors
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
                Sensored.Invoke(this);
            };
        }
        public event ISimpleSensor.SimpleEventHandler Sensored;
    }

}