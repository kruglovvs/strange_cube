// Copyright kruglov.valentine@gmail.com KruglovVS.

namespace Periphery.Sensors
{
    public interface II2cSensor
    {
        public double[] Read(byte register);
    }
}
