// Copyright kruglov.valentine@gmail.com KruglovVS.

using System;

namespace Periphery.Sensors
{
    public class TMP112 : I2cSensor, II2cSensor
    {
        internal TMP112(int address) : base(address)
        {
        }
        public enum Registers : byte 
        {
            Temperature = 0x0,
            //Configuration = 0x1,
        }
        public double[] Read(byte register)
        {
            switch (register)
            {
                case (byte)Registers.Temperature: return new double[] { Temperature };
                default: throw new Exception("wrong register");
            }
        }
        public double Temperature
        {
            get
            {
                byte[] rawTemperature = new byte[2];
                Read(new SpanByte(rawTemperature));
                return (rawTemperature[0] * 256 + rawTemperature[1]) / 16 * 0.0625;
            }
        }
    }
}