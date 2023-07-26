// Copyright kruglov.valentine@gmail.com KruglovVS.

using System.Device.Gpio;
using System.Threading;

namespace Periphery.Actuators
{
    public class Mechanical : IActuator
    {
        private GpioPin Pin { get; init; }
        internal Mechanical(int pinNumber, GpioController gpioController)
        {
            Pin = gpioController.OpenPin(pinNumber, Constants.PinModes.Mechanical);
            Pin.Write(Constants.PinStartValues.Mechanical);
        }
        public void Actuate()
        {
            Pin.Write(PinValue.High);
            new Timer((e) => { Pin.Write(PinValue.Low); }, null, Constants.Time.Actuate.Milliseconds, Timeout.Infinite);
        }
    }

}
