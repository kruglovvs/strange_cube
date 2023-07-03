using System;
using System.Device.Gpio;
using System.Device.I2c;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using DataNS;

namespace PeripheryNS {
    public class PeripheryController {
        static class Constants {
            public static class Count {
                public const int Buttons = 10;
            }
            public static class Adress {
                public const int TMP112 = 213; // 8'b10010010
                public const int LSM6 = 234; // 8'b11101011
            }
            public static class Pin {
                public static readonly int[] Buttons = new int[Count.Buttons] { 22, 21, 25, 24, 23, 15, 7, 6, 18, 17 };
                public const int PhotoSensor = 9;
                public const int Luminoides = 4;
                public const int VibrationMotor = 5;
            }
        }
        private class Sensor {
            private int _pinNumber { get; set; }
            public int PinNumber { get { return _pinNumber; } }
            public Sensor (int pinNumber) {
                PeripheryController.GpioController.OpenPin(pinNumber, PinMode.Input);
                _pinNumber = pinNumber;
            }
            public PinValue Value {
                get {
                    return GpioController.Read(_pinNumber);
                }
            }
        }
        private class Button : Sensor {
            public Button(int pinNumber) : base(pinNumber) {
            }
        }
        private class PhotoSensor : Sensor {
            public PhotoSensor(int pinNumber) : base(pinNumber) {
            }
        }
        private class VibrationSensor : Sensor {
            public VibrationSensor(int pinNumber) : base(pinNumber) {
            }
        }
        private class GasSensor : Sensor {
            public GasSensor(int pinNumber) : base(pinNumber) {
            }
        }
        private class TMP112 {
            public int temperature { get; }
        }
        private class LSM6 {
            public int gyroscope { get; }
            public int accelerometer { get; }
        }
        private class Actuator {
            private int _pinNumber { get; set; }
            public Actuator (int pinNumber) 
            {
                PeripheryController.GpioController.OpenPin(pinNumber, PinMode.Output);
                _pinNumber = pinNumber;
            }
            public PinValue IsActing
            {
                get 
                {
                    return GpioController.Read(_pinNumber);
                }
                set
                {
                    GpioController.Write(_pinNumber, value);
                }
            }
        }
        private class VibrationMotor : Actuator
        {
            public VibrationMotor(int pinNumber) : base(pinNumber)
            {
            }
        }
        private class Door : Actuator
        {
            public Door(int pinNumber) : base(pinNumber)
            {
            }
        }

        static GpioController GpioController;

        private Button[] buttons = new Button[Constants.Count.buttons];
        private PhotoSensor phototransistors = new PhotoSensor();
        private TMP112 tmp112 = new TMP112();
        private LSM6 lsm6 = new LSM6();
        private VibrationSensor vibration_sensor = new VibrationSensor();
        private GasSensor gas_sensor = new GasSensor();
        private VibrationMotor vibration_motor = new VibrationMotor();
        private Door door = new Door();
        private Data last_data = new Data();

        public PeripheryController() {
            last_data = read_data();
        }

        public Data send_data() { return new Data(); }
        private Data read_data() { return new Data(); }
    }
}
