// copyright kruglovvs kruglov.valentine@gmail.com

using System;
using System.Device.Gpio;
using System.Device.I2c;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace DataNS {
    public class Data {
        public bool[] Buttons { get; set; } = new bool[12];
        public double[] Rotation { get; set; } = new double[3];
        public double[] Accelation { get; set; } = new double[3];
        public double Temperature { get; set; } = new double();
        public bool PhotoSensor { get; set; } = new bool();
        public bool GasSensor { get; set; } = new bool();
        public bool VibrationSensor { get; set; } = new bool();
    }
}
