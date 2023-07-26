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
        private static St7565WO12864 s_display { get; set; }
        private static Luminodiodes s_luminodiodes { get; set; }
        private static Mechanical s_vibrationMotor { get; set; }
        private static Mechanical s_door{ get; set; }

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
            Debug.WriteLine("I2cController turning on");
            I2cSensorsController.TurnOn();
            I2cSensorsController.RotationListened += (sender, e) => { };//GotAction?.Invoke(Instruction.Rotate); };
            I2cSensorsController.AccelationListened += (sender, e) => { };//GotAction?.Invoke(Instruction.Rotate); };
            I2cSensorsController.TemperatureListened += (sender, e) => {
                if (e.Data[0] > Constants.Temperature.Hot)
                {
                    GotAction?.Invoke(Instruction.HeatUp);
                }
                else if (e.Data[0] < Constants.Temperature.Cold)
                {
                    GotAction?.Invoke(Instruction.CoolDown);
                }
            };

            Debug.WriteLine("ButtonsController turning on");
            ButtonsController.TurnOn();
            ButtonsController.GotButton += (e) => { GotAction?.Invoke(e); };

            Debug.WriteLine("SimpleSensorsController turning on");
            SimpleSensorsController.TurnOn();
            SimpleSensorsController.GotSimpleSensor += (e) => { GotAction?.Invoke(e); };

            DisplaysController.TurnOn();
            // These pins should be removed to other places.
            //s_gpioController.OpenPin(34, PinMode.Output); // Door.
            //IsDoorOpened = false;
            //s_gpioController.OpenPin(35, PinMode.Output); // VibrationMotor.
            //s_gpioController.Wite(35, PinValue.Low);

            // Set Display.
            //s_gpioController.Write(27, PinValue.High); // ACs.
            Debug.WriteLine($"{Configuration.GetFunctionPin(DeviceFunction.SPI1_CLOCK)}");
            Debug.WriteLine($"{Configuration.GetFunctionPin(DeviceFunction.SPI1_MISO)}");
            Debug.WriteLine($"{Configuration.GetFunctionPin(DeviceFunction.SPI1_MOSI)}");
            //s_gpioController.Write(12, PinValue.High); // A0.
            s_gpioController.Write(26, PinValue.High); // ARes.
            Thread.Sleep(10);
            s_gpioController.Write(26, PinValue.Low); // ARes.

            // Create spi and i2c devices.
            Debug.WriteLine($"Create spi and i2c devices.");
            //s_display = new SpiDevice(new SpiConnectionSettings(1, 27));

        }
        public static void SetLuminodiodds(byte[] image)
        {
            //LuminodiodesInterop.LuminodiodesInterop.OneWireSendLuminodiodes(image, 27);
        }
        public static void SetDisplay(byte[] image)
        {
            //s_display.Write(new SpanByte(image));
        }
        public static void Vibrate(int timeMillisecons)
        {
            // s_gpioController.Write(35, PinValue.High); // VibrationMotor.
            //Thread.Sleep(timeMillisecons);
            // s_gpioController.Write(35, PinValue.Low); // VibrationMotor.
        }
        public static void OpenDoor() {

        }

        public static event PeripheryGotActionEventHandler GotAction;
        public delegate void PeripheryGotActionEventHandler(Instruction instruction);
    }
}
