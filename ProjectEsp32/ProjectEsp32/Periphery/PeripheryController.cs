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
using static ProjectESP32.Program;
//using LuminodiodesInterop;

namespace Periphery
{
    public static class PeripheryController
    {
        internal static GpioController s_gpioController { get; set; } = new GpioController();
        private static ButtonMatrix s_buttonMatrix { get; set; }
        private static Button[] s_buttons { get; set; }
        private static SimpleSensor s_photoSensor { get; set; }
        private static SimpleSensor s_vibrationSensor { get; set; }
        private static SimpleSensor s_gasSensor { get; set; }
        private static TMP112 s_tmp112 { get; set; }
        private static LSM6 s_lsm6 { get; set; }
        private static St7565WO12864 s_display { get; set; }
        private static Luminodiodes s_luminodiodes { get; set; }
        private static Mechanical s_vibrationMotor { get; set; }
        private static Mechanical s_door{ get; set; }

        private static KeyMatrix s_keyMatrix { get; set; }
        private static SpiDevice s_display { get; set; }
        private static I2cDevice s_tmp112 { get; set; }
        private static I2cDevice s_lsm6 { get; set; }
        private static Thread s_checkingTemperature { get; set; } = new Thread(() =>
        {
            Thread.CurrentThread.Suspend();
            while (true)
            {
                byte[] rawTemperature = new byte[2];
                s_tmp112.WriteRead(new SpanByte(new byte[] { 0x0 }), new SpanByte(rawTemperature));
                double temperature = (rawTemperature[0] * 256 + rawTemperature[1]) / 16 * 0.0625;
                if (temperature > 35.0)
                {
                    GotAction?.Invoke(Instruction.HeatUp);

                }
                else if (temperature < 15.0)
                {
                    GotAction?.Invoke(Instruction.CoolDown);
                }
                Debug.WriteLine($"Temperature read raw data: {rawTemperature}");
                Thread.Sleep(50);
            }
        });
        private static Thread s_checkingRotation { get; set; } = new Thread(() =>
        {
            Thread.CurrentThread.Suspend();
            while (true)
            {
                byte[] rawRotation = new byte[6];
                s_lsm6.WriteRead(new SpanByte(new byte[] { 0x22 }), new SpanByte(rawRotation));
                Debug.WriteLine($"Rotation read raw data: {rawRotation}");
                Thread.Sleep(50);
            }
        });
        private static Thread s_checkingAccelation { get; set; } = new Thread(() =>
        {
            Thread.CurrentThread.Suspend();
            while (true)
            {
                byte[] rawAccelation = new byte[6];
                s_lsm6.WriteRead(new SpanByte(new byte[] { 0x28 }), new SpanByte(rawAccelation));
                Debug.WriteLine($"Accelation read raw data: {rawAccelation}");
                Thread.Sleep(50);
            }
        });
        private static PinChangeEventHandler s_listeningIllumination { get; set; } = (sender, e) =>
        {
            GotAction?.Invoke(Instruction.Illuminate);
        };
        private static PinChangeEventHandler s_listeningVibrating { get; set; } = (sender, e) =>
        {

            GotAction?.Invoke(Instruction.Vibrate);

        };
        private static PinChangeEventHandler s_listeningSmoking { get; set; } = (sender, e) =>
        {

            GotAction?.Invoke(Instruction.Smoke);

        };

        public static Instruction CurrentInstruction { get; set; }
        public static Instruction ListeningAction
        {
            get
            {
                return CurrentInstruction;
            }
            set
            {
                CurrentInstruction = value;
                ListeningButtons = false;
                CheckingRotation = false;
                CheckingTemperature = false;
                CheckingAccelation = false;
                s_gpioController.UnregisterCallbackForPinValueChangedEvent(25, s_listeningIllumination);
                s_gpioController.UnregisterCallbackForPinValueChangedEvent(36, s_listeningSmoking);
                s_gpioController.UnregisterCallbackForPinValueChangedEvent(39, s_listeningVibrating);
                switch (value)
                {
                    case Instruction.Rotate:
                        CheckingRotation = true;
                        break;
                    case Instruction.Accelate:
                        CheckingRotation = true;
                        break;
                    case Instruction.HeatUp:
                    case Instruction.CoolDown:
                        CheckingRotation = true;
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
                        ListeningButtons = true;
                        break;
                    case Instruction.Illuminate:
                        s_gpioController.RegisterCallbackForPinValueChangedEvent(25, PinEventTypes.Falling, s_listeningIllumination);
                        break;
                    case Instruction.Smoke:
                        s_gpioController.RegisterCallbackForPinValueChangedEvent(36, PinEventTypes.Falling, s_listeningSmoking);
                        break;
                    case Instruction.Vibrate:
                        s_gpioController.RegisterCallbackForPinValueChangedEvent(39, PinEventTypes.Falling, s_listeningVibrating);
                        break;
                }
            }
        }

        private static bool CheckingTemperature
        {
            set
            {
                Debug.WriteLine("CheckingTemperature!");
                if (value)
                {
                    s_checkingTemperature?.Resume();
                }
                else
                {
                    s_checkingTemperature?.Suspend();
                }
            }
        }
        private static bool CheckingRotation
        {
            set
            {
                Debug.WriteLine("CheckingRotation!");
                if (value)
                {
                    s_checkingRotation?.Resume();
                }
                else
                {
                    s_checkingRotation?.Suspend();
                }
            }
        }
        private static bool CheckingAccelation
        {
            set
            {
                Debug.WriteLine("CheckingAccelation!");
                if (value)
                {
                    s_checkingAccelation?.Resume();
                }
                else
                {
                    s_checkingAccelation?.Suspend();
                }
            }
        }
        private static bool ListeningButtons
        {
            set
            {
                Debug.WriteLine("ListeningButtons!");
                if (value)
                {
                    s_keyMatrix?.StartListeningKeyEvent();
                }
                else
                {
                    s_keyMatrix?.StopListeningKeyEvent();
                }
            }
        }

        public static void TurnOn()
        {

            //  Open all pins.
            s_gpioController.OpenPin(12, PinMode.Output); // A0.
            s_gpioController.OpenPin(26, PinMode.Output); // ARes.
            //s_gpioController.OpenPin(27, PinMode.Output); // ACs.
            s_gpioController.OpenPin(13, PinMode.Output); // MOSI.
            Configuration.SetPinFunction(13, DeviceFunction.SPI1_MOSI);
            //Configuration.SetPinFunction(404, DeviceFunction.SPI1_MISO);
            //s_gpioController.OpenPin(14, PinMode.Output); // CLK.
            Configuration.SetPinFunction(14, DeviceFunction.SPI1_CLOCK);
            s_gpioController.OpenPin(19, PinMode.Input);  // Int1.
            s_gpioController.OpenPin(23, PinMode.Input);  // Int2.
            //s_gpioController.OpenPin(21, PinMode.Output); // SDA.
            Configuration.SetPinFunction(21, DeviceFunction.I2C1_DATA);
            //s_gpioController.OpenPin(22, PinMode.Output); // SCL.
            Configuration.SetPinFunction(13, DeviceFunction.I2C1_CLOCK);

            s_gpioController.OpenPin(25, PinMode.InputPullUp);  // PhotoSensor.
            s_gpioController.OpenPin(36, PinMode.Input);  // GasSensor.
            s_gpioController.OpenPin(39, PinMode.InputPullUp);  // VibrationSensor.

            // These pins should be removed to other places.
            //s_gpioController.OpenPin(34, PinMode.Output); // Door.
            //IsDoorOpened = false;
            //s_gpioController.OpenPin(35, PinMode.Output); // VibrationMotor.
            //s_gpioController.Wite(35, PinValue.Low);

            // Create KeyMatrix.
            s_keyMatrix = new KeyMatrix(new int[] { 15, 2, 4 }, new int[] { 5, 17, 16 }, new TimeSpan(1), s_gpioController, false);
            s_keyMatrix.KeyEvent += (sender, e) =>
            {
                if (e.EventType == PinEventTypes.Rising)
                {
                    GotAction.Invoke((Instruction)((int)Instruction.ButtonPress0 + e.Input * 3 + e.Output));
                }
            };
            s_keyMatrix.StopListeningKeyEvent();
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
            s_tmp112 = new I2cDevice(new I2cConnectionSettings(1, 0x49));
            s_lsm6 = new I2cDevice(new I2cConnectionSettings(1, 0x75));

            // Set power-on modes.
            //s_tmp112.Write(new SpanByte(new byte[] { 0x13, 0x01}));
            Debug.WriteLine($"Set power-on modes");
            //s_lsm6.Write(new SpanByte(new byte[] { 0x10, 0b01010000 }));
            //s_lsm6.Write(new SpanByte(new byte[] { 0x10, 0b01010000 }));

            // Start threads that will check parameters for sensors.
            s_checkingTemperature.Start();
            s_checkingAccelation.Start();
            s_checkingRotation.Start();

            Debug.WriteLine($"End turn on periphery");
            //SetLuminodiodds(new byte[] { 0 });
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
        public static void OpenDoor() { }

        public static event PeripheryGotActionEventHandler GotAction;
        public delegate void PeripheryGotActionEventHandler(Instruction instruction);
    }
}
