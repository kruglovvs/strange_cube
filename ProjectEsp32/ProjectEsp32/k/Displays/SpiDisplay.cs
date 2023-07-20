// Copyright kruglov.valentine@gmail.com KruglovVS.

using nanoFramework.Hardware.Esp32;
using System.Device.Spi;

namespace Periphery.Displays
{
    public abstract class SpiDisplay : SpiDevice
    {
        static SpiDisplay()
        {
            Configuration.SetPinFunction(Constants.Pins.MOSI, DeviceFunction.SPI1_MOSI);
            Configuration.SetPinFunction(Constants.Pins.CLK, DeviceFunction.SPI1_CLOCK);
        }
        public SpiDisplay() : base (new SpiConnectionSettings(Constants.ID.SpiBus))
        {
        }
    }
}
