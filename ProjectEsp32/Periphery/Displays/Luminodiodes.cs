using System.Device.Gpio;

namespace ProjectESP32.Periphery.Displays
{
    public class Luminodiodes : IDisplay<Luminodiodes.Image>, ITurningOn
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
        public void SetImage (Image image)
        {

        }
        public bool IsTurnedOn
        {
            get { return IsTurnedOn; }
            set
            {
                if (!value)
                {
                    SetImage(new Image());
                }
            }
        }
    }
}
