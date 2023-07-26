// Copyright kruglov.valentine@gmail.com KruglovVS.

namespace Periphery.Sensors
{
    public interface II2cSensor
    {
        public delegate void I2cSensorEventHandler(object sender, I2cSensorEventArgs e);
        public class I2cSensorEventArgs {
            public readonly double[] Data;
            public I2cSensorEventArgs(double[] data) { 
                Data = data;
            }
        }
    }
}
