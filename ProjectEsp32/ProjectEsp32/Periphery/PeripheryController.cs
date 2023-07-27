using Iot.Device.KeyMatrix;
using nanoFramework.Hardware.Esp32;
using Periphery.Actuators;
using Periphery.Displays;
using Periphery.Sensors;
using System;
using System.Device.Gpio;
using System.Device.I2c;
using System.Device.Spi;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using Game;
//using LuminodiodesInterop;

namespace Periphery
{
    public static class PeripheryController
    {
        internal static GpioController s_gpioController { get; set; } = new GpioController();

        public static Instruction ListeningAction
        {
            set
            {
                ButtonsController.IsListeningMatrix = false;
                I2cSensorsController.IsCheckingRotation = false;
                I2cSensorsController.IsCheckingTemperature = false;
                I2cSensorsController.IsCheckingAccelation = false;
                SimpleSensorsController.IsListeningVibrationSensor = false;
                SimpleSensorsController.IsListeningGasSensor = false;
                SimpleSensorsController.IsListeningPhotoSensor = false;
                switch (value)
                {
                    case Instruction.Rotate:
                        I2cSensorsController.IsCheckingRotation = true;
                        break;
                    case Instruction.Accelate:
                        I2cSensorsController.IsCheckingAccelation = true;
                        break;
                    case Instruction.HeatUp:
                    case Instruction.CoolDown:
                        I2cSensorsController.IsCheckingTemperature = true;
                        break;
                    case Instruction.ButtonPress0:
                    case Instruction.ButtonPress1:
                    case Instruction.ButtonPress2:
                    case Instruction.ButtonPress3:
                    case Instruction.ButtonPress4:
                    case Instruction.ButtonPress5:
                    case Instruction.ButtonPress6:
                    case Instruction.ButtonPress7:
                    case Instruction.ButtonPress8:
                        ButtonsController.IsListeningMatrix = true;
                        break;
                    case Instruction.Illuminate:
                        SimpleSensorsController.IsListeningPhotoSensor = true;
                        break;
                    case Instruction.Smoke:
                        SimpleSensorsController.IsListeningGasSensor = true;
                        break;
                    case Instruction.Vibrate:
                        SimpleSensorsController.IsListeningVibrationSensor = true;
                        break;
                }
            }
        }
        public static void TurnOn()
        {
            Sleep.EnableWakeupByPin(Sleep.WakeupGpioPin.Pin34, 2);
            Sleep.EnableWakeupByPin(Sleep.WakeupGpioPin.Pin35, 2);

            Debug.WriteLine("I2cController turning on");
            I2cSensorsController.TurnOn();
            I2cSensorsController.GotAction += (e) => { GotAction?.Invoke(e); };

            Debug.WriteLine("ButtonsController turning on");
            ButtonsController.TurnOn();
            ButtonsController.GotButton += (e) => { GotAction?.Invoke(e); };

            Debug.WriteLine("SimpleSensorsController turning on");
            SimpleSensorsController.TurnOn();
            SimpleSensorsController.GotSimpleSensor += (e) => { GotAction?.Invoke(e); };

            DisplaysController.TurnOn();
            ActuatorsController.TurnOn();
        }
        public static void SetLuminodiodds(byte[] image)
        {
            DisplaysController.SetLuminodiodes(image);
        }
        public static void SetDisplay(byte[] image)
        {
            DisplaysController.SetDisplay(image);
        }
        public static void Vibrate()
        {
            ActuatorsController.Vibrate();
        }
        public static void OpenDoor() {
            ActuatorsController.OpenDoor();
        }

        public static event PeripheryGotActionEventHandler GotAction;
        public delegate void PeripheryGotActionEventHandler(Instruction instruction);
    }
}
