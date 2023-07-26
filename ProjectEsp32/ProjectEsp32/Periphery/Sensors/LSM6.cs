// Copyright kruglov.valentine@gmail.com KruglovVS.

using System;
using System.Device.I2c;
using System.Diagnostics;

namespace Periphery.Sensors
{
    internal class LSM6 : I2cDevice, II2cSensor
    {
        public II2cSensor.I2cSensorEventArgs Accelation
        {
            get
            {
                Debug.WriteLine("Accelation");
                return new II2cSensor.I2cSensorEventArgs (new double[3] { 0, 0, 0 });
            }
        }
        public II2cSensor.I2cSensorEventArgs Rotation
        {
            get
            {
                Debug.WriteLine("Rotation");
                return new II2cSensor.I2cSensorEventArgs(new double[3] { 0, 0, 0 });
            }
        }
        internal LSM6(int address) : base(new I2cConnectionSettings(Constants.ID.I2cBus, address)) {
            Write(new SpanByte(new byte[] { 0x10, 0b01010000 }));
            Write(new SpanByte(new byte[] { 0x10, 0b01010000 }));
        }
    }
}