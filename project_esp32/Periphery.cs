using System;
using System.Device.Gpio;
using System.Device.I2c;
using System.Device.Spi;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

using Iot.Device.Button;

using DataNS;
using nanoFramework.UI.Input;
using System.Reflection;

namespace PeripheryNS {
    public class PeripheryController {

        static GpioController GpioController;

        private SimpleSensors.Button[] _buttons;
        private SimpleSensors.PhotoSensor _photoSensor;
        private SimpleSensors.VibrationSensor _vibrationSensor;
        private SimpleSensors.GasSensor _gasSensor;
        private I2cSensors.TMP112 _tmp112;
        private I2cSensors.LSM6 _lsm6;
        private Actuators.VibrationMotor _vibrationMotor;
        private Actuators.Door _door;

        public PeripheryController()
        {
            _buttons = new SimpleSensors.Button[Constants.Counts.Buttons];
            _photoSensor = new SimpleSensors.PhotoSensor(Constants.Pins.PhotoSensor);
            _tmp112 = new I2cSensors.TMP112(Constants.Adresses.TMP112);
            _lsm6 = new I2cSensors.LSM6(Constants.Adresses.LSM6);
            _vibrationSensor = new SimpleSensors.VibrationSensor(Constants.Pins.VibrationSensor);
            _gasSensor = new SimpleSensors.GasSensor(Constants.Pins.GasSensor);
            _vibrationMotor = new Actuators.VibrationMotor(Constants.Pins.VibrationMotor);
            _door = new Actuators.Door(Constants.Pins.Door);
            for (int i = 0; i < Constants.Counts.Buttons; i++)
            {
                _buttons[i] = new SimpleSensors.Button(Constants.Pins.Buttons[i], GpioController);
            }
        }
        public Data send_data() { return new Data(); }
        private Data read_data() { return new Data(); }

        private static class Constants
        {
            public static class Counts
            {
                public const int Buttons = 12;
            }
            public static class Pins
            {
                public static readonly int[] Buttons = new int[Counts.Buttons] { 33, 32, 27, 26, 25, 1, 7, 6, 18, 17, 27, 12 }; //исправить
                public const int Luminoides = 2;
                public const int PhotoSensor = 18;
                public const int VibrationSensor = 4;
                public const int GasSensor = 5;
                public const int SDA = 21;
                public const int SCL = 22;
                public const int INT1 = 19;
                public const int INT2 = 23;
                public const int MOSI = 13;
                public const int CLK = 14;
                public const int A0 = 15;
                public const int VibrationMotor = 35;
                public const int Door = 34;
            }
            public static class ID
            {
                public const int I2cBus = 1;
            }
            public static class Adresses
            {
                public const byte TMP112 = 0b1001001;
                public const byte LSM6 = 0b1001001;
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
            public class Button : GpioButton
            {
                private bool _isPressed;
                public new bool IsPressed { 
                    get 
                    {
                        if (_isPressed)
                        {
                            _isPressed = false;
                            return true;
                        }
                        return false;
                    } 
                }
                public Button(int pinNumber, GpioController gpioController) : base(pinNumber, gpioController, true, PinMode.InputPullDown)
                {
                    this.IsDoublePressEnabled = true;
                    this.IsHoldingEnabled = true;
                    this.ButtonDown += (sender, e) => { _isPressed = true; };
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
                public string sa;
                public I2cSensor(int address) : base(new I2cConnectionSettings(Constants.ID.I2cBus, address)) { }
                public I2cTransferResult Read(byte[] readBytes)
                {
                    I2cTransferResult result = Read(readBytes);
                    if (result.Status != I2cTransferStatus.FullTransfer)  {
                        Debug.WriteLine("can't read data");
                        throw new Exception("can't read data");
                    }
                    return result;
                }
                public I2cTransferResult Write(byte[] writeBytes)
                {
                    I2cTransferResult result = Write(writeBytes);
                    if (result.Status != I2cTransferStatus.FullTransfer)
                    {
                        Debug.WriteLine("can't write data");
                        throw new Exception("can't write data");
                    }
                    return result;
                }
                public I2cTransferResult WriteRead(byte[] writeBytes, byte[] readBytes)
                {
                    I2cTransferResult result = WriteRead(writeBytes, readBytes);
                    if (result.Status != I2cTransferStatus.FullTransfer)
                    {
                        Debug.WriteLine("can't write or read data");
                        throw new Exception("can't write or read data");
                    }
                    return result;
                }
            }
            public class TMP112 : I2cSensor
            {
                public TMP112(int address) : base(address) { }
                public static class Registers
                {
                     public static readonly byte Temperature = 0x0;
                     public static readonly byte Configuration = 0x1;
                }
                public double Temperature { 
                    get 
                    {
                        byte[] rawTemperature = new byte[2];
                        Read(new SpanByte(rawTemperature));
                        return (rawTemperature[0] * 256 + rawTemperature[1]) / 16 * 0.0625;
                    } 
                }
            }
            public class LSM6 : I2cSensor
            {
                public LSM6(int address) : base(address) { }
                public static class Registers
                {
                    public static readonly byte[] Temperature = new byte[2] { 0x20, 0x21 };
                    public static readonly byte[] Rotation = new byte[6] { 0x22, 0x23, 0x24, 0x25, 0x26, 0x27 };
                    public static readonly byte[] Accelation = new byte[6] { 0x28, 0x29, 0x2A, 0x2B, 0x2C, 0x2D };
                }
                public double[] Rotation 
                {
                    get
                    {
                        double[] rotation = new double[3];
                        byte[] rawRotation = new byte[6];
                        WriteRead(Registers.Rotation[0], rawRotation);
                        rotation[0] = rawRotation[0] * 256 + rawRotation[1];
                        return rotation;
                    }
                }
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
