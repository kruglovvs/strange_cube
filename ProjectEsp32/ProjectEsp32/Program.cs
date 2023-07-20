using Network;
using Periphery;
using System.Diagnostics;
using System.Threading;
using System.Device.Gpio;

namespace ProjectESP32
{
    public class Program
    {
        public static void Main()
        {
            //Debug.WriteLine($"net connected: {NetworkController.Connect()}");
            PeripheryController.TurnOn();
            PeripheryController.GasSensored += () => { Debug.WriteLine("Vib sensored"); };
            Debug.WriteLine($"is vn sensor's pin can pe input: {PeripheryController.GpioController.IsPinModeSupported(39, PinMode.Input)}");
            Debug.WriteLine($"is vn sensor's pin opened: {PeripheryController.GpioController.IsPinOpen(39)}");

            //NetworkController.Subscribe("/GameData");
            //Debug.WriteLine($"is connected{ NetworkController.IsConnected}");
            /*NetworkController.Got += (sender, e) => { Debug.WriteLine($" topic: {e.Topic} message: {e.Message}"); };
            for (int i = 0; i < 10; i++)
            {
                NetworkController.Publish("/GameData", "kek");
            }*/


            Thread.Sleep(Timeout.Infinite);

            // Browse our samples repository: https://github.com/nanoframework/samples
            // Check our documentation online: https://docs.nanoframework.net/
            // Join our lively Discord community: https://discord.gg/gCyBu8T
        }
    }
}
