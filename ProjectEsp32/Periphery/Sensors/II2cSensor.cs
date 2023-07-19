// copyright kruglovvs kruglov.valentine@gmail.com

namespace ProjectESP32.Periphery.Sensors
{
    public interface II2cSensor <Registers>
    {
        public double[] Read(Registers register);
    }
}
