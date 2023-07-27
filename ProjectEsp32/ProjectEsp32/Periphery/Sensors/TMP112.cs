// Copyright kruglov.valentine@gmail.com KruglovVS.

using System;
using System.Device.Gpio;
using System.Device.I2c;
using System.Diagnostics;
using System.Threading;
using static Periphery.PeripheryController;

namespace Periphery.Sensors
{
    internal class TMP112 : I2cDevice, II2cSensor
    {
        public II2cSensor.I2cSensorEventArgs Temperature
        {
            get
            {
                byte[] rawTemperature = new byte[2];
                WriteRead(new SpanByte(new byte[1] { 0x00 }), new SpanByte(rawTemperature));
                int readTemp = (rawTemperature[0] * 256 + rawTemperature[1]) / 16;
                if (readTemp > 2047) {
                    readTemp -= 4096;
                }
                double temperature = readTemp * 0.0625;
                return (new II2cSensor.I2cSensorEventArgs(new double[] { temperature }));
            }
        }
        internal TMP112(int address) : base(new I2cConnectionSettings(Constants.ID.I2cBus, address)) {
            s_gpioController.OpenPin(Constants.Pins.INT1, PinMode.Input);  // Int1.
            s_gpioController.OpenPin(Constants.Pins.INT2, PinMode.Input);  // Int2.
            Write(new SpanByte(new byte[] { 0x13, 0b00000001 }));
        }
    }
}