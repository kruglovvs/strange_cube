// Copyright kruglov.valentine@gmail.com KruglovVS.

using System.Device.Gpio;

namespace Periphery.Displays
{
    public class Luminodiodes : IDisplay
    {
        private GpioPin Pin { get; init; }
        public int CountPixels { get; init; }
        internal Luminodiodes(int pinNumber, int countPixels, GpioController gpioController)
        {
            Pin = gpioController.OpenPin(pinNumber, Constants.PinModes.Luminodiodes);
            Pin.Write(PinValue.Low);
            CountPixels = countPixels;
        }
        public void SetImage (byte[] image)
        {

        }
    }
}
