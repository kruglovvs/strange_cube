// Copyright kruglov.valentine@gmail.com KruglovVS.

using System;

namespace Periphery.Sensors
{
    public interface ISimpleSensor
    {
        public delegate void SimpleEventHandler();
        public event SimpleEventHandler Sensored;
    }
}
