// Copyright kruglov.valentine@gmail.com KruglovVS.

using System.Device.Gpio;

namespace Periphery.Displays
{
    public class Luminodiodes : IDisplay, ITurningOn
    {
        private GpioPin Pin { get; init; }
        public int CountPixels { get; init; }
        internal Luminodiodes(int pinNumber, int countPixels)
        {
            Pin = PeripheryController.OpenPin(pinNumber, Constants.PinModes.Luminodiodes);
            CountPixels = countPixels;
            IsTurnedOn = (bool)Constants.PinStartValues.Luminodiodes;
        }
        public class Image
        {
            internal byte[] bytes;
        }
        public void SetImage (byte[] image)
        {

        }
        public bool IsTurnedOn
        {
            get { return IsTurnedOn; }
            set
            {
                if (!value)
                {
                    SetImage(new byte[CountPixels]);
                }
            }
        }
    }
}
