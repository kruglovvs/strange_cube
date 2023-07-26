// Copyright kruglov.valentine@gmail.com KruglovVS.

using nanoFramework.Hardware.Esp32;
using System;
using System.Device.Gpio;
using System.Diagnostics;
using System.Threading;
using static Periphery.PeripheryController;

namespace Periphery.Sensors
{
    public static class I2cSensorsController
    {
        private static LSM6 s_lsm6 { get; set; }
        private static TMP112 s_tmp112 { get; set; }
        private static Timer s_checkingTemperature { get; set; }
        private static Timer s_checkingRotation { get; set; }
        private static Timer s_checkingAccelation { get; set; }
        
        public static void TurnOn()
        {
            Debug.WriteLine("LSM6 creating");
            s_lsm6 = new LSM6(Constants.Addresses.LSM6);
            Debug.WriteLine("TMP112 creating");
            s_tmp112 = new TMP112(Constants.Addresses.TMP112);
        }
        public static bool IsCheckingTemperature
        {
            set
            {
                if (value)
                {
                    s_checkingTemperature = new Timer((state) =>
                    {
                        TemperatureListened?.Invoke(null, s_tmp112.Temperature);
                    }, null, 0, 100);
                } else
                {
                    s_checkingTemperature?.Dispose();
                }
            }
        }
        public static bool IsCheckingRotation
        {
            set
            {
                if (value)
                {
                    s_checkingRotation = new Timer((state) =>
                    {
                        RotationListened?.Invoke(null, s_lsm6.Rotation);
                    }, null, 0, 100);
                }
                else
                {
                    s_checkingRotation?.Dispose();
                }
            }
        }
        public static bool IsCheckingAccelation
        {
            set
            {
                if (value)
                {
                    s_checkingAccelation = new Timer((state) =>
                    {
                        AccelationListened?.Invoke(null, s_lsm6.Accelation);
                    }, null, 0, 100);
                }
                else
                {
                    s_checkingAccelation?.Dispose();
                }
            }
        }


        public static event II2cSensor.I2cSensorEventHandler TemperatureListened;
        public static event II2cSensor.I2cSensorEventHandler RotationListened;
        public static event II2cSensor.I2cSensorEventHandler AccelationListened;
    }
}