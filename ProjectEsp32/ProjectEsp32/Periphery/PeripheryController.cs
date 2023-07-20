// Copyright kruglov.valentine@gmail.com KruglovVS.

using nanoFramework.Hardware.Esp32;
using System;
using System.Device.Gpio;
using System.Diagnostics;

namespace Periphery
{
    public static class PeripheryController
    {
        public static GpioController GpioController = new GpioController();

        private static Sensors.Button[] s_buttons;
        private static Sensors.ButtonMatrix s_matrix;
        private static Sensors.SimpleSensor s_photoSensor;
        private static Sensors.SimpleSensor s_vibrationSensor;
        private static Sensors.SimpleSensor s_gasSensor;
        private static Sensors.TMP112 s_tmp112;
        private static Sensors.LSM6 s_lsm6;
        private static Actuators.Mechanical s_vibrationMotor;
        private static Actuators.Mechanical s_door;
        private static Displays.St7565WO12864 s_display;
        private static Displays.Luminodiodes s_luminodiodes;

        internal static GpioPin OpenPin(int pinNumber, PinMode pinMode)
        {
            if (GpioController is null) { return null; }
            if (!GpioController.IsPinOpen(pinNumber))
            {
                return GpioController?.OpenPin(pinNumber, pinMode);
            }
            else
            {
                throw new Exception($"pin {pinNumber} is already opened");
            }
        }

        public static bool IsTurnedOn
        {
            get
            {
                return IsTurnedOn;
            }
            set
            {
                s_display.IsTurnedOn = !value;
                s_matrix.IsListening = value;
                //IsDisplaying = value;
               // IsOpened = false;
               // IsVibrating = false;
                IsTurnedOn = value;
            }
        }
        public static void TurnOn()
        {
            OpenPin(Constants.Pins.SDA, PinMode.Output);
            OpenPin(Constants.Pins.SCL, PinMode.Output);

            Configuration.SetPinFunction(21, DeviceFunction.I2C1_DATA);
            Configuration.SetPinFunction(22, DeviceFunction.I2C1_CLOCK);

            OpenPin(Constants.Pins.MOSI, PinMode.Output);
            OpenPin(Constants.Pins.CLK, PinMode.Output);

            Configuration.SetPinFunction(Constants.Pins.MOSI, DeviceFunction.SPI1_MOSI);
            Configuration.SetPinFunction(Constants.Pins.CLK, DeviceFunction.SPI1_CLOCK);

            s_buttons = new Sensors.Button[Constants.Counts.Buttons];
            Debug.WriteLine("6");
            s_matrix = new Sensors.ButtonMatrix(Constants.Pins.ButtonsOut, Constants.Pins.ButtonsIn);
            Debug.WriteLine("6");
            s_photoSensor = new Sensors.SimpleSensor(Constants.Pins.PhotoSensor);
            Debug.WriteLine("6");
            s_vibrationSensor = new Sensors.SimpleSensor(Constants.Pins.VibrationSensor);
            Debug.WriteLine("6");
            s_gasSensor = new Sensors.SimpleSensor(Constants.Pins.GasSensor);
            Debug.WriteLine("6");
            s_tmp112 = new Sensors.TMP112(Constants.Addresses.TMP112);
            Debug.WriteLine("6");
            s_lsm6 = new Sensors.LSM6(Constants.Addresses.LSM6);
            Debug.WriteLine("6");
            //s_vibrationMotor = new Actuators.Mechanical(Constants.Pins.VibrationMotor);
            Debug.WriteLine("6");
            //s_door = new Actuators.Mechanical(Constants.Pins.Door);
            Debug.WriteLine("444");
            //s_display = new Displays.St7565WO12864(Constants.Pins.A0, Constants.Pins.ARes, Constants.Pins.ACs);
            //s_luminodiodes = new Displays.Luminodiodes(Constants.Pins.Luminoides, Constants.Counts.Luminodiodes);

            Debug.WriteLine("6");
            for (int i = 0; i < Constants.Counts.Buttons; i++)
            {
                s_buttons[i] = new Sensors.Button(Constants.Pins.Buttons[i]);
                s_buttons[i].Sensored += () => { ButtonPressed?.Invoke(i + Constants.ButtonIndexes.Single); };

            }
            Debug.WriteLine("7");
            s_matrix.ButtonMatrixPressed += (sender, e) => { ButtonPressed?.Invoke(e + Constants.ButtonIndexes.Matrix); };
            s_photoSensor.Sensored += () => { PhotoSensored?.Invoke(); };
            s_gasSensor.Sensored += () => { GasSensored?.Invoke(); };
            s_vibrationSensor.Sensored += () => { VibrationSensored?.Invoke(); };
            Debug.WriteLine("8");

            //IsTurnedOn = true;
            Debug.WriteLine("9");
        }
       /* public static bool IsOpened
        {
            get { return s_door.IsActing; }
            set
            {
                s_door.IsActing = value;
            }
        }
        public static bool IsVibrating
        {
            get { return s_vibrationMotor.IsActing; }
            set
            {
                s_vibrationMotor.IsActing = value;
            }
        }
        public static bool IsDisplaying
        {
            get { return s_display.IsTurnedOn; }
            set
            {
                s_display.IsTurnedOn = value;
                s_luminodiodes.IsTurnedOn = value;
            }
        }
        public static double Temperature
        {
            get
            {
                return s_tmp112.Read((byte)Sensors.TMP112.Registers.Temperature)[0];
            }
        }
        public static double[] Accelation
        {
            get
            {
                return s_lsm6.Read((byte)Sensors.LSM6.Registers.Accelation);
            }
        }
        public static double[] Rotation
        {
            get
            {
                return s_lsm6.Read((byte)Sensors.LSM6.Registers.Rotation);
            }
        }*/

        public delegate void EmptyHandler();
        public delegate void ButtonHandler(int buttonNumber);
        public static event ButtonHandler ButtonPressed;
        public static event EmptyHandler PhotoSensored;
        public static event EmptyHandler VibrationSensored;
        public static event EmptyHandler GasSensored;
    }
}