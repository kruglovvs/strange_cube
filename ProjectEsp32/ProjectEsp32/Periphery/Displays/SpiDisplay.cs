// Copyright kruglov.valentine@gmail.com KruglovVS.

using nanoFramework.Hardware.Esp32;
using System.Device.Spi;

namespace Periphery.Displays
{
    public abstract class SpiDisplay : SpiDevice
    {
        public SpiDisplay() : base (new SpiConnectionSettings(Constants.ID.SpiBus))
        {
        }
    }
}
