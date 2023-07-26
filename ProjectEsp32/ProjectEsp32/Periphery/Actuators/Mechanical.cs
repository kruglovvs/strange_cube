// Copyright kruglov.valentine@gmail.com KruglovVS.

using System.Device.Gpio;

namespace Periphery.Actuators
{
    public class Mechanical : IActuator
    {
        private GpioPin Pin { get; init; }
        internal Mechanical(int pinNumber, GpioController gpioController)
        {
            Pin = gpioController.OpenPin(pinNumber, Constants.PinModes.Mechanical);
            IsActing = (bool)Constants.PinStartValues.Mechanical;
        }
        public bool IsActing
        {
            get
            {
                return IsActing;
            }
            set
            {
                Pin.Write((PinValue)value);
                IsActing = value;
            }
        }
    }

}
