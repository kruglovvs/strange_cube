// Copyright kruglov.valentine@gmail.com KruglovVS.

using Game;
using nanoFramework.Hardware.Esp32;
using System;
using System.Device.Gpio;
using System.Device.Spi;
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
                        if (s_tmp112.Temperature.Data[0] > Constants.Temperature.Hot) {
                            GotAction?.Invoke(Instruction.HeatUp);
                        } else if (s_tmp112.Temperature.Data[0] < Constants.Temperature.Cold) {
                            GotAction?.Invoke(Instruction.CoolDown);
                        }
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
                        double[] firstData = s_lsm6.Rotation.Data;
                        Thread.Sleep(100);
                        double[] secondData = s_lsm6.Rotation.Data;
                        for (int i = 0; i < 3; ++i) {
                            if (Math.Abs(firstData[i] - secondData[i]) > 20) {
                                GotAction?.Invoke(Instruction.Rotate);
                                return;
                            }
                        }
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
                        double[] data = s_lsm6.Accelation.Data;
                        for (int i = 0; i < 3; ++i) {
                            if (Math.Abs(data[i]) > 20) {
                                GotAction?.Invoke(Instruction.Accelate);
                                return;
                            }
                        }
                    }, null, 0, 100);
                }
                else
                {
                    s_checkingAccelation?.Dispose();
                }
            }
        }


        public static event PeripheryGotActionEventHandler GotAction;
    }
}