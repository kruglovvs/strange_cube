// copyright kruglovvs kruglov.valentine@gmail.com

using System;

namespace ProjectESP32.Periphery.Sensors
{
    public interface ISimpleSensor
    {
        public event EventHandler Sensored;
    }
}
