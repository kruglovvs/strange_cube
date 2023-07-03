using System;
using System.Device.Gpio;
using System.Device.I2c;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using DataNS;

namespace PeripheryNS {
    public class PeripheryController {
        static GpioController GpioController;

        private SimpleSensors.Button[] _buttons;
        private SimpleSensors.PhotoSensor _photoSensor;
        private SimpleSensors.VibrationSensor _vibrationSensor;
        private SimpleSensors.GasSensor _gasSensor;
        private I2cSensors.TMP112 _tmp112;
        private I2cSensors.LSM6 _lsm6;
        private Actuators.VibrationMotor _vibrationMtor;
        private Actuators.Door _door;

        public PeripheryController()
        {
            _buttons = new SimpleSensors.Button[Constants.Counts.Buttons];
            _photoSensor = new SimpleSensors.PhotoSensor(Constants.Pins.PhotoSensor);
            _tmp112 = new I2cSensors.TMP112(Constants.Adress.TMP112);
            _lsm6 = new I2cSensors.LSM6(Constants.Adress.LSM6);
            _vibrationSensor = new SimpleSensors.VibrationSensor(Constants.Pins.VibrationSensor);
            _gasSensor = new SimpleSensors.GasSensor(Constants.Pins.GasSensor);
            _vibrationMtor = new Actuators.VibrationMotor(Constants.Pins.VibrationMotor);
            _door = new Actuators.Door(Constants.Pins.Door);
            for (int i = 0; i < Constants.Counts.Buttons; i++)
            {
                _buttons[i] = new Button(Constants.Pins.Buttons[i]);
            }
        }
        public Data send_data() { return new Data(); }
        private Data read_data() { return new Data(); }

        private static class Constants
        {
            public static class Counts
            {
                public const int Buttons = 12;
                public const int PinsTMP112 = 2;
                public const int PinsLSM6 = 4;
            }
            public static class Adress
            {
                public const int TMP112 = 146; // 8'b10010010
                public const int LSM6 = 235; // 8'b11101011
            }
            public static class Pins
            {
                public static readonly int[] Buttons = new int[Counts.Buttons] { 22, 21, 25, 24, 23, 13, 7, 6, 18, 17, 27, 12 };
                public const int Luminoides = 4;
                public const int PhotoSensor = 9;
                public const int VibrationSensor = 5;
                public const int GasSensor = 5;
                public const int SDA = 11;
                public const int SCL = 14;
                public const int INT1 = 10;
                public const int INT2 = 15;
                public const int MOSI = 28;
                public const int CLK = 26;
                public const int A0 = 3;
                public const int VibrationMotor = 20;
                public const int Door = 19;
            }
            public static class ID
            {
                public const int I2cBus = 0;
            }
        }
        private class SimpleSensors
        {
            public class Sensor
            {
                private int _pinNumber { get; set; }
                public int PinNumber { get { return _pinNumber; } }
                public Sensor(int pinNumber)
                {
                    PeripheryController.GpioController.OpenPin(pinNumber, PinMode.Input);
                    _pinNumber = pinNumber;
                }
                public PinValue Value
                {
                    get
                    {
                        return GpioController.Read(_pinNumber);
                    }
                }
            }
            public class Button : Sensor
            {
                public Button(int pinNumber) : base(pinNumber)
                {
                }
            }
            public class PhotoSensor : Sensor
            {
                public PhotoSensor(int pinNumber) : base(pinNumber)
                {
                }
            }
            public class VibrationSensor : Sensor
            {
                public VibrationSensor(int pinNumber) : base(pinNumber)
                {
                }
            }
            public class GasSensor : Sensor
            {
                public GasSensor(int pinNumber) : base(pinNumber)
                {
                }
            }
        }
        private class I2cSensors
        {
            private static int _SCL = Constants.Pins.SCL;
            private static int _SDA = Constants.Pins.SDA;
            private static int _INT1 = Constants.Pins.INT1;
            private static int _INT2 = Constants.Pins.INT2;
            public class I2cSensor : I2cDevice
            {
                public I2cSensor(int address) : base(new I2cConnectionSettings(Constants.ID.I2cBus, address)) { }
            }
            public class TMP112 : I2cSensor
            {
                public TMP112(int address) : base(address) { }
                public int Temperature { get; }
            }
            public class LSM6 : I2cSensor
            {
                internal LSM6(int address) : base(address) { }
                public int Rotation { get; }
                public int Accelation { get; }
            }
        }
        private class Actuators
        {
            public class Actuator
            {
                private int _pinNumber { get; set; }
                public Actuator(int pinNumber)
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
            public class VibrationMotor : Actuator
            {
                public VibrationMotor(int pinNumber) : base(pinNumber)
                {
                }
            }
            public class Door : Actuator
            {
                public Door(int pinNumber) : base(pinNumber)
                {
                }
            }
        }
    }
}
