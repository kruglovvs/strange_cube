using Network;
using Periphery;
using System.Diagnostics;
using System.Threading;
using System.Device.Gpio;
using Iot.Device.Button;

namespace ProjectESP32
{
    public class Program
    {
        internal static GpioController GpioController = new GpioController();
        public static void Main()
        {
            //  Open all pins.

            GpioController.OpenPin(12, PinMode.Output); // A0.
            GpioController.OpenPin(13, PinMode.Output); // MOSI.
            GpioController.OpenPin(14, PinMode.Output); // CLK.
            GpioController.OpenPin(15, PinMode.Output); // BOut1.
            GpioController.OpenPin(16, PinMode.Input);  // BIn3.
            GpioController.OpenPin(17, PinMode.Input);  // BIn2.
            GpioController.OpenPin(19, PinMode.Input);  // Int1.
            GpioController.OpenPin(21, PinMode.Output); // SDA.
            GpioController.OpenPin(22, PinMode.Output); // SCL.
            GpioController.OpenPin(23, PinMode.Input);  // Int2.
            GpioController.OpenPin(26, PinMode.Output); // ARes.
            GpioController.OpenPin(27, PinMode.Output); // ACs.

            GpioController.OpenPin(34, PinMode.Output); // Door.
            GpioController.OpenPin(35, PinMode.Output); // VibrationMotor.

            GpioController.OpenPin(25, PinMode.Input).ValueChanged += (sender, e) => { Debug.WriteLine("1"); };  // PhotoSensor.
            GpioController.OpenPin(36, PinMode.Input).ValueChanged += (sender, e) => { Debug.WriteLine("1"); };  // GasSensor.
            GpioController.OpenPin(39, PinMode.Input).ValueChanged += (sender, e) => { Debug.WriteLine("1"); };  // VibrationSensor.

            new GpioButton(33, GpioController, false).Press += (sender, e) => { Debug.WriteLine("1"); }; // B1
            new GpioButton(32, GpioController, false).Press += (sender, e) => { Debug.WriteLine("1"); }; // B2
        }
    }
}
