// copyright kruglovvs kruglov.valentine@gmail.com

using System;
using System.Device.Gpio;
using System.Device.I2c;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace DataNS {
    public class Data {
        public bool[] Buttons = new bool[12];
        public double[] Rotation = new double[3];
        public double[] Accelation = new double[3];
        public double Temperature = new double();
        public bool PhotoSensor = new bool();
        public bool GasSensor = new bool();
        public bool VibrationSensor = new bool();
    }
}
