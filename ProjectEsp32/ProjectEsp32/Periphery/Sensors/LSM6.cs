// Copyright kruglov.valentine@gmail.com KruglovVS.

using System;

namespace Periphery.Sensors
{
    public class LSM6 : I2cSensor, II2cSensor
    {
        internal LSM6(int address) : base(address) { }
        public enum Registers : byte
        {
            Temperature = 0x20, // { 0x20, 0x21 }
            Rotation = 0x22, // { 0x22, 0x23, 0x24, 0x25, 0x26, 0x27 }
            Accelation = 0x28, // { 0x28, 0x29, 0x2A, 0x2B, 0x2C, 0x2D }
        }
        public double[] Read(byte register)
        {
            switch (register)
            {
                case (byte)Registers.Accelation: return ReadArray(register);
                case (byte)Registers.Rotation: return ReadArray(register);
                case (byte)Registers.Temperature: return new double[] { Temperature };
                default: throw new Exception("wrong register");
            }
        }
        private double[] ReadArray(byte register)
        {
            double[] value = new double[3];
            byte[] rawValue = new byte[6];
            WriteRead(new byte[] { (byte)register }, new SpanByte(rawValue));
            value[0] = rawValue[0] * 256 + rawValue[1];
            value[1] = rawValue[2] * 256 + rawValue[3];
            value[2] = rawValue[4] * 256 + rawValue[5];
            return value;
        }
        public double[] Rotation
        {
            get
            {
                return ReadArray((byte)Registers.Rotation);
            }
        }
        public double[] Accelation
        {
            get
            {
                return ReadArray((byte)Registers.Accelation);
            }
        }
        public double Temperature
        {
            get
            {
                byte[] rawTemperature = new byte[2];
                Read(new SpanByte(rawTemperature));
                return (rawTemperature[0] * 256 + rawTemperature[1]) / 16 * 0.0625; // idk it should be changed
            }
        }
    }
}