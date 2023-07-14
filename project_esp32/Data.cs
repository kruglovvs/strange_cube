// copyright kruglovvs kruglov.valentine@gmail.com

using System;

namespace DataNS
{
    public class Data
    {
        public interface IData
        {

        }
        public class Periphery : IData
        {
            public class SensorEventArgs : EventArgs
            {
                public SensorTypes SensorType { get; set; }
                public object Value { get; set; }
            }
            public enum SensorTypes
            {
                Button,
                Photo,
                Vibration,
                Gas,
                Temperature,
                Accelation,
                Rotation,
            }
            public enum DisplayType
            {
                
            }
            public class Sensors : IData
            {
                public bool[] Buttons { get; set; } = new bool[12];
                public double[] Rotation { get; set; } = new double[3];
                public double[] Accelation { get; set; } = new double[3];
                public double Temperature { get; set; } = new double();
                public bool PhotoSensor { get; set; } = new bool();
                public bool GasSensor { get; set; } = new bool();
                public bool VibrationSensor { get; set; } = new bool();
            }
            public class Image : IData
            {

            }
        }
    }
}
