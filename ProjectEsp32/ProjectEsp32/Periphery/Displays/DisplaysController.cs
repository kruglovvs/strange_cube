// Copyright kruglov.valentine@gmail.com KruglovVS.

using nanoFramework.Hardware.Esp32;
using System.Device.Gpio;
using static Periphery.PeripheryController;

namespace Periphery.Displays
{
    public static class DisplaysController 
    {
        private static St7565WO12864 s_display { get; set; }
        private static Luminodiodes s_luminodiodes { get; set; }
        public static void TurnOn()
        {
            Configuration.SetPinFunction(Constants.Pins.MOSI, DeviceFunction.SPI1_MOSI);
            Configuration.SetPinFunction(Constants.Pins.MISO, DeviceFunction.SPI1_MISO);
            Configuration.SetPinFunction(Constants.Pins.CLK, DeviceFunction.SPI1_CLOCK);

            s_display = new St7565WO12864(Constants.Pins.A0, Constants.Pins.ARes, Constants.Pins.ACs, s_gpioController);
            s_luminodiodes = new Luminodiodes(Constants.Pins.Luminoides, Constants.Counts.Luminodiodes, s_gpioController);

        }
    }
}
