// Copyright kruglov.valentine@gmail.com KruglovVS.

using System;
using System.Device.Gpio;
using System.Diagnostics;
using Periphery.Constants;
using Periphery.Sensors;

namespace Periphery
{
    public static partial class PeripheryController
    {
        internal static GpioController GpioController = new GpioController(PinNumberingScheme.Board);

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

        static PeripheryController()
        {
            s_buttons = new Sensors.Button[Constants.Counts.Buttons];

            foreach (int i in new int[3] { 5, 17, 16 })
            {
                Debug.WriteLine($"fuck {i.ToString()}");
                Debug.WriteLine($"hell {i} is open:{GpioController.IsPinOpen(i).ToString()}, can be opened out: {GpioController.IsPinModeSupported(i, PinMode.Output).ToString()}");
            }

            s_matrix = new Sensors.ButtonMatrix(Constants.Pins.ButtonsIn, Constants.Pins.ButtonsOut);
            s_photoSensor = new Sensors.SimpleSensor(Constants.Pins.PhotoSensor);
            s_vibrationSensor = new Sensors.SimpleSensor(Constants.Pins.VibrationSensor);
            s_gasSensor = new Sensors.SimpleSensor(Constants.Pins.GasSensor);
            s_tmp112 = new Sensors.TMP112(Constants.Addresses.TMP112);
            s_lsm6 = new Sensors.LSM6(Constants.Addresses.LSM6);
            s_vibrationMotor = new Actuators.Mechanical(Constants.Pins.VibrationMotor);
            s_door = new Actuators.Mechanical(Constants.Pins.Door);
            s_display = new Displays.St7565WO12864(Constants.Pins.A0, Constants.Pins.ARes, Constants.Pins.ACs);
            s_luminodiodes = new Displays.Luminodiodes(Constants.Pins.Luminoides, Constants.Counts.Luminodiodes);

            for (int i = 0; i < Constants.Counts.Buttons; i++)
            {
                s_buttons[i] = new Sensors.Button(Constants.Pins.Buttons[i]);
                s_buttons[i].Sensored += (sender) => { ButtonPressed.Invoke(); };
            }

            IsTurnedOn = true;

            //s_matrix.ButtonMatrixPressed += (sender, e) => { ButtonPressed.Invoke(sender, e + Constants.ButtonIndexes.Matrix); };
            //s_photoSensor.Sensored += (sender, e) => { PhotoSensored.Invoke(sender, new EventArgs()); };
            //s_photoSensor.Sensored += (sender, e) => { PhotoSensored.Invoke(sender, new EventArgs()); };
            //s_gasSensor.Sensored += (sender, e) => { GasSensored.Invoke(sender, new EventArgs()); };
            //s_vibrationSensor.Sensored += (sender, e) => { VibrationSensored.Invoke(sender, new EventArgs()); };
        }
        internal static GpioPin OpenPin(int pinNumber, PinMode pinMode)
        {
            if (!GpioController.IsPinOpen(pinNumber))
            {
                return GpioController.OpenPin(pinNumber, pinMode);
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
                IsDisplaying = value;
                IsOpened = false;
                IsVibrating = false;
                IsTurnedOn = value;
            }
        }
        public static bool IsOpened
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
        }

        public delegate void EmptyHandler();
        public static event EmptyHandler ButtonPressed;
        public static event EmptyHandler PhotoSensored;
        public static event EmptyHandler VibrationSensored;
        public static event EmptyHandler GasSensored;
    }
}