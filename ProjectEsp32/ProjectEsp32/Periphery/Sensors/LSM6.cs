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
                byte[] rawAccelation = new byte[6];
                WriteRead(new SpanByte(new byte[1] { 0x28 }), new SpanByte(rawAccelation));
                double[] accelation = new double[3];
                for (int i = 0, j = 0; i < 3; ++i, j += 2) {
                    accelation[i] = (rawAccelation[j] + rawAccelation[j + 1] * 256) * 4 / 32768;
                }
                Debug.WriteLine("Accelation");
                return new II2cSensor.I2cSensorEventArgs (accelation);
            }
        }
        public II2cSensor.I2cSensorEventArgs Rotation
        {
            get
            {
                byte[] rawRotation = new byte[6];
                WriteRead(new SpanByte(new byte[1] { 0x28 }), new SpanByte(rawRotation));
                double[] rotation = new double[3];
                for (int i = 0, j = 0; i < 3; ++i, j += 2) {
                    rotation[i] = (rawRotation[j] + rawRotation[j + 1] * 256) * 2000 / 32768;
                }
                Debug.WriteLine("Rotation");
                return new II2cSensor.I2cSensorEventArgs(rotation);
            }
        }
        internal LSM6(int address) : base(new I2cConnectionSettings(Constants.ID.I2cBus, address)) {
            //set the gyroscope control register to work at 104 Hz, 2000 dps and in bypass mode
            Write(new SpanByte(new byte[2] { 0x11, 0b01001100 }));

            // Set the Accelerometer control register to work at 104 Hz, 4 g,and in bypass mode and enable ODR/4
            // low pass filter (check figure9 of LSM6DS3's datasheet)
            Write(new SpanByte(new byte[2] { 0x10, 0b01001010 }));

            // set gyroscope power mode to high performance and bandwidth to 16 MHz
            Write(new SpanByte(new byte[2] { 0x16, 0b00000000 }));

            // Set the ODR config register to ODR/4
            Write(new SpanByte(new byte[2] { 0x17, 0b00001001 }));

        }
    }
}