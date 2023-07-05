using System;
using System.Device.Gpio;
using System.Device.I2c;
using System.Device.Spi;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using Iot.Device.Button;
using nanoFramework.Hardware.Esp32;

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
                public const int Luminodiodes = 9;
            }
            public static class Pins
            {
                public static readonly int[] Buttons = new int[Counts.Buttons] { 15, 2, 4, 404, 404, 5, 18, 404, 404, 33, 25, 26 }; //исправить
                public const int Luminoides = 34;
                public const int PhotoSensor = 27;
                public const int VibrationSensor = 35;
                public const int GasSensor = 404;
                public const int SDA = 21;
                public const int SCL = 22;
                public const int INT1 = 19;
                public const int INT2 = 23;
                public const int MOSI = 13;
                public const int CLK = 14;
                public const int A0 = 12;
                public const int VibrationMotor = 35;
                public const int Door = 32;
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
                public int PinNumber { get; private set; }
                public Sensor(int pinNumber)
                {
                    PeripheryController.GpioController.OpenPin(pinNumber, PinMode.Input);
                    PinNumber = pinNumber;
                }
                public PinValue Value
                {
                    get
                    {
                        return GpioController.Read(PinNumber);
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
                public new bool IsPressed { 
                    get 
                    {
                        if (IsPressed)
                        {
                            IsPressed = false;
                            return true;
                        }
                        return false;
                    }
                    private set 
                    { 
                        IsPressed = value; 
                    }
                }
                public Button(int pinNumber) : base(pinNumber, GpioController, true, PinMode.InputPullDown)
                {
                    this.IsDoublePressEnabled = false; // it can be changed
                    this.IsHoldingEnabled = false; // it can be changed
                    this.ButtonDown += (sender, e) => { IsPressed = true; };
                }
            }
        }
        private class I2cSensors
        { 
            public class I2cSensor : I2cDevice
            {
                static I2cSensor() {
                    Configuration.SetPinFunction(Constants.Pins.SDA, DeviceFunction.I2C1_DATA);
                    Configuration.SetPinFunction(Constants.Pins.SCL, DeviceFunction.I2C1_CLOCK);
                }
                public I2cSensor(int address) : base(new I2cConnectionSettings(Constants.ID.I2cBus, address)) { }
                public new I2cTransferResult Read(SpanByte readSpanBytes)
                {
                    I2cTransferResult result = Read(readSpanBytes);
                    if (result.Status != I2cTransferStatus.FullTransfer)  {
                        Debug.WriteLine("can't read data");
                        throw new Exception("can't read data");
                    }
                    return result;
                }
                public new I2cTransferResult Write(SpanByte writeSpanBytes)
                {
                    I2cTransferResult result = Write(writeSpanBytes);
                    if (result.Status != I2cTransferStatus.FullTransfer)
                    {
                        Debug.WriteLine("can't write data");
                        throw new Exception("can't write data");
                    }
                    return result;
                }
                public new I2cTransferResult WriteRead(SpanByte writeSpanBytes, SpanByte readSpanBytes)
                {
                    I2cTransferResult result = WriteRead(writeSpanBytes, readSpanBytes);
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
                     public const byte Temperature = 0x0;
                     public const byte Configuration = 0x1;
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
                /*public enum Registers 
                {
                    Temperature = 0x20,
                    Rotation = 0x22, 
                    Accelation = 0x28,
                }*/
                public static class Registers
                {
                    public const byte Temperature = 0x20; // { 0x20, 0x21 }
                    public const byte Rotation = 0x22; // { 0x22, 0x23, 0x24, 0x25, 0x26, 0x27 }
                    public const byte Accelation = 0x28; // { 0x28, 0x29, 0x2A, 0x2B, 0x2C, 0x2D }
                    //public static readonly byte[] Tmperature = new byte[2] { 0x20, 0x21 };
                    //public static readonly byte[] Rotation = new byte[6] { 0x22, 0x23, 0x24, 0x25, 0x26, 0x27 };
                    //public static readonly byte[] Accelation = new byte[6] { 0x28, 0x29, 0x2A, 0x2B, 0x2C, 0x2D };
                }
                public double[] ReadValue(byte register) {
                    double[] value = new double[3];
                    byte[] rawValue = new byte[6];
                    WriteRead(new byte[] { register }, new SpanByte(rawValue));
                    value[0] = rawValue[0] * 256 + rawValue[1];
                    value[1] = rawValue[2] * 256 + rawValue[3];
                    value[2] = rawValue[4] * 256 + rawValue[5];
                    return value;
                }
                /*public double[] ReadValue(Registers register) {
                    return ReadValue((byte)register);
                }*/
                public double[] Rotation 
                {
                    get 
                        {
                        return ReadValue(Registers.Rotation);
                    }
                }
                public double[] Accelation 
                {
                    get {
                        return ReadValue(Registers.Accelation);
                    }
                }
            }
        }
        private class Actuators
        {
            public class Mechanical
            {
                public int PinNumber { get; private set; }
                public Mechanical(int pinNumber)
                {
                    PeripheryController.GpioController.OpenPin(pinNumber, PinMode.Output);
                    PinNumber = pinNumber;
                }
                public PinValue IsActing
                {
                    get 
                    { 
                        return IsActing; 
                    }
                    set
                    {
                        GpioController.Write(PinNumber, value);
                        IsActing = value;
                    }
                }
            }
            public class VibrationMotor : Mechanical {
                public VibrationMotor(int pinNumber) : base(pinNumber)
                {
                }
            }
            public class Door : Mechanical {
                public Door(int pinNumber) : base(pinNumber)
                {
                }
            }
            public class Luminodiodes {
                public int PinNumber { get; private set; }
                public Luminodiodes(int pinNumber) {
                    PeripheryController.GpioController.OpenPin(pinNumber, PinMode.Output);
                    PinNumber = pinNumber;
                }
                public byte[] Colors { set { } }
            }
        }
    }
}
