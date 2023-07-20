// Copyright kruglov.valentine@gmail.com KruglovVS.

using System;
using System.Device.I2c;
using System.Diagnostics;
using nanoFramework.Hardware.Esp32;

namespace Periphery.Sensors
{
    public abstract class I2cSensor : I2cDevice
    {
        static I2cSensor()
        {
            PeripheryController.OpenPin(Constants.Pins.SDA, Constants.PinModes.SDA);
            PeripheryController.OpenPin(Constants.Pins.CLK, Constants.PinModes.CLK);
            Configuration.SetPinFunction(Constants.Pins.SDA, DeviceFunction.I2C1_DATA);
            Configuration.SetPinFunction(Constants.Pins.SCL, DeviceFunction.I2C1_CLOCK);
        }
        internal I2cSensor(int address) : base(new I2cConnectionSettings(Constants.ID.I2cBus, address)) { }
        public new I2cTransferResult Read(SpanByte readSpanBytes)
        {
            I2cTransferResult result = Read(readSpanBytes);
            if (result.Status != I2cTransferStatus.FullTransfer)
            {
                Debug.WriteLine("can't read data");
            }
            return result;
        }
        public new I2cTransferResult Write(SpanByte writeSpanBytes)
        {
            I2cTransferResult result = Write(writeSpanBytes);
            if (result.Status != I2cTransferStatus.FullTransfer)
            {
                Debug.WriteLine("can't write data");
            }
            return result;
        }
        public new I2cTransferResult WriteRead(SpanByte writeSpanBytes, SpanByte readSpanBytes)
        {
            I2cTransferResult result = WriteRead(writeSpanBytes, readSpanBytes);
            if (result.Status != I2cTransferStatus.FullTransfer)
            {
                Debug.WriteLine("can't write or read data");
            }
            return result;
        }
    }
}