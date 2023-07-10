// copyright kruglovvs kruglov.valentine@gmail.com

using System;
using System.Device.Gpio;
using System.Device.I2c;
using System.Device.Spi;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using Iot.Device.Button;
using nanoFramework.Hardware.Esp32;
using nanoFramework.UI;

using DataNS;
using System.Runtime.CompilerServices;
using System.Drawing;
using nanoFramework.Presentation.Controls;
using nanoFramework.Presentation.Media;

namespace PeripheryNS
{
    public class PeripheryController
    {

        static GpioController GpioController = new GpioController();

        private SimpleSensors.Button[] _buttons;
        private SimpleSensors.PhotoSensor _photoSensor;
        private SimpleSensors.VibrationSensor _vibrationSensor;
        private SimpleSensors.GasSensor _gasSensor;
        private I2cSensors.TMP112 _tmp112;
        private I2cSensors.LSM6 _lsm6;
        private Actuators.VibrationMotor _vibrationMotor;
        private Actuators.Door _door;
        private SpiDisplays.St7565WO12864 _display;

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
                _buttons[i] = new SimpleSensors.Button(Constants.Pins.Buttons[i]);
            }
        }
        public Data Data 
        { 
            get 
            {
                Data data = new Data();
                for (int i = 0; i < Constants.Counts.Buttons; ++i) {
                    data.Buttons[i] = _buttons[i].IsPressed;
                }
                data.PhotoSensor = _photoSensor.Value;
                data.VibrationSensor = _vibrationSensor.Value;
                data.GasSensor = _gasSensor.Value;
                data.Temperature = _tmp112.Temperature;
                data.Accelation = _lsm6.Accelation;
                data.Rotation = _lsm6.Rotation;
                return data;
            } 
        }
        public bool IsTurnedOn { get; set; }
        public SpanByte Image 
        { 
            set 
            {
                _display.Image = value;
            } 
        }
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
            public abstract class Sensor
            {
                public int PinNumber { get; init; }
                public Sensor(int pinNumber)
                {
                    PeripheryController.GpioController.OpenPin(pinNumber, PinMode.Input);
                    PinNumber = pinNumber;
                }
                public bool Value
                {
                    get
                    {
                        return (bool)GpioController.Read(PinNumber);
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
                public new bool IsPressed
                {
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
            public abstract class I2cSensor : I2cDevice
            {
                static I2cSensor()
                {
                    Configuration.SetPinFunction(Constants.Pins.SDA, DeviceFunction.I2C1_DATA);
                    Configuration.SetPinFunction(Constants.Pins.SCL, DeviceFunction.I2C1_CLOCK);
                }
                public I2cSensor(int address) : base(new I2cConnectionSettings(Constants.ID.I2cBus, address)) { }
                public new I2cTransferResult Read(SpanByte readSpanBytes)
                {
                    I2cTransferResult result = Read(readSpanBytes);
                    if (result.Status != I2cTransferStatus.FullTransfer)
                    {
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
                public abstract class Registers { }
            }
            public class TMP112 : I2cSensor
            {
                public TMP112(int address) : base(address) { }
                public static new class Registers
                {
                    public const byte Temperature = 0x0;
                    public const byte Configuration = 0x1;
                }
                public double Temperature
                {
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
                public static new class Registers
                {
                    public const byte Temperature = 0x20; // { 0x20, 0x21 }
                    public const byte Rotation = 0x22; // { 0x22, 0x23, 0x24, 0x25, 0x26, 0x27 }
                    public const byte Accelation = 0x28; // { 0x28, 0x29, 0x2A, 0x2B, 0x2C, 0x2D }
                    //public static readonly byte[] Tmperature = new byte[2] { 0x20, 0x21 };
                    //public static readonly byte[] Rotation = new byte[6] { 0x22, 0x23, 0x24, 0x25, 0x26, 0x27 };
                    //public static readonly byte[] Accelation = new byte[6] { 0x28, 0x29, 0x2A, 0x2B, 0x2C, 0x2D };
                }
                public double[] ReadValue(byte register)
                {
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
                    get
                    {
                        return ReadValue(Registers.Accelation);
                    }
                }
            }
        }
        private class Actuators
        {
            public class Mechanical
            {
                public int PinNumber { get; init; }
                public Mechanical(int pinNumber)
                {
                    GpioController.OpenPin(pinNumber, PinMode.Output);
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
            public class VibrationMotor : Mechanical
            {
                public VibrationMotor(int pinNumber) : base(pinNumber)
                {
                }
            }
            public class Door : Mechanical
            {
                public Door(int pinNumber) : base(pinNumber)
                {
                }
            }
            public class Luminodiodes
            {
                public int PinNumber { get; private set; }
                public Luminodiodes(int pinNumber)
                {
                    GpioController.OpenPin(pinNumber, PinMode.Output);
                    PinNumber = pinNumber;
                }
                /*#region Stubs

                [MethodImpl(MethodImplOptions.InternalCall)]
                private static extern void NativeGetHardwareSerial(byte[] data);

                #endregion stubs*/
                public byte[] Colors
                {
                    set
                    {
                    }
                }
            }
        }
        private class SpiDisplays
        {
            public abstract class SpiDisplay // singleton
            {
                static SpiDisplay()
                {
                    Configuration.SetPinFunction(Constants.Pins.MOSI, DeviceFunction.SPI1_MOSI);
                    Configuration.SetPinFunction(Constants.Pins.CLK, DeviceFunction.SPI1_CLOCK);
                }
                public SpiDisplay(SpiConfiguration spiConfiguration, ScreenConfiguration screenConfiguration, uint bufferSize)
                {
                    DisplayControl.Initialize(spiConfiguration, screenConfiguration, bufferSize);
                }
                public abstract void WritePoint(ushort x, ushort y);
                public abstract class Registers { }
                public abstract class Orientation { }
                public abstract class Parameters { }
                public abstract SpanByte Image { set; }
            }
            public class St7565WO12864 : SpiDisplay
            {
                public static class StateA0
                {
                    public static readonly PinValue Data = PinValue.High;
                    public static readonly PinValue Control = PinValue.Low;
                }
                public int PinNumberA0 { get; init; }
                public St7565WO12864(int pinNumberA0) : base(new SpiConfiguration(1, -1, -1, -1, -1), new ScreenConfiguration(0, 0, Parameters.Width, Parameters.Height, GraphicDriver), 128 * 64)
                {
                    PinNumberA0 = pinNumberA0;
                    GpioController.OpenPin(pinNumberA0, PinMode.Output);
                }
                public override void WritePoint(ushort x, ushort y)
                {
                    GpioController.Write(PinNumberA0, StateA0.Data);
                    DisplayControl.WritePoint(x, y, Color.Black);
                }
                public static new class Registers
                {
                    public const int
                    NOP = 0x00,
                    SOFTWARE_RESET = 0x01,
                    POWER_STATE = 0x10,
                    Sleep_Out = 0x11,
                    Invertion_Off = 0x20,
                    Invertion_On = 0x21,
                    Gamma_Set = 0x26,
                    Display_OFF = 0x28,
                    Display_ON = 0x29,
                    Column_Address_Set = 0x2A,
                    Page_Address_Set = 0x2B,
                    Memory_Write = 0x2C,
                    Colour_Set = 0x2D,
                    Memory_Read = 0x2E,
                    Partial_Area = 0x30,
                    Memory_Access_Control = 0x36,
                    Pixel_Format_Set = 0x3A,
                    Memory_Write_Continue = 0x3C,
                    Write_Display_Brightness = 0x51,
                    Frame_Rate_Control_Normal = 0xB1,
                    Frame_Rate_Control_2 = 0xB2,
                    Frame_Rate_Control_3 = 0xB3,
                    Invert_On = 0xB4,
                    Display_Function_Control = 0xB6,
                    Entry_Mode_Set = 0xB7,
                    Power_Control_1 = 0xC0,
                    Power_Control_2 = 0xC1,
                    Power_Control_3 = 0xC2,
                    Power_Control_4 = 0xC3,
                    Power_Control_5 = 0xC4,
                    VCOM_Control_1 = 0xC5,
                    VCOM_Control_2 = 0xC7,
                    Power_Control_A = 0xCB,
                    Power_Control_B = 0xCF,
                    Positive_Gamma_Correction = 0xE0,
                    Negative_Gamma_Correction = 0XE1,
                    Driver_Timing_Control_A = 0xE8,
                    Driver_Timing_Control_B = 0xEA,
                    Power_On_Sequence = 0xED,
                    Enable_3G = 0xF2,
                    Pump_Ratio_Control = 0xF7,
                    Power_Control_6 = 0xFC;
                }
                public static new class Orientation
                {
                    public const int
                    MADCTL_MH = 0x04, // sets the Horizontal Refresh, 0=Left-Right and 1=Right-Left
                    MADCTL_ML = 0x10, // sets the Vertical Refresh, 0=Top-Bottom and 1=Bottom-Top
                    MADCTL_MV = 0x20, // sets the Row/Column Swap, 0=Normal and 1=Swapped
                    MADCTL_MX = 0x40, // sets the Column Order, 0=Left-Right and 1=Right-Left
                    MADCTL_MY = 0x80, // sets the Row Order, 0=Top-Bottom and 1=Bottom-Top
                    MADCTL_BGR = 0x08; // Blue-Green-Red pixel order
                }
                public static new class Parameters
                {
                    public const int Width = 128;
                    public const int Height = 64;
                }
                public override SpanByte Image { set { } }

                public static GraphicDriver GraphicDriver = new GraphicDriver();
                /*{
                    MemoryWrite = 0x2C,
                    SetColumnAddress = 0x2A,
                    SetRowAddress = 0x2B,
                    InitializationSequence = new byte[]
                    {
                                                        (byte)GraphicDriverCommandType.Command, 1, (byte)Registers.SOFTWARE_RESET,
                                                        // Sleep for 50 ms
                                                        (byte)GraphicDriverCommandType.Sleep, 5,
                                                        (byte)GraphicDriverCommandType.Command, 1, (byte)Registers.Sleep_Out,
                                                        // Sleep for 500 ms
                                                        (byte)GraphicDriverCommandType.Sleep, 50,
                                                        (byte)GraphicDriverCommandType.Command, 4, (byte)Registers.Frame_Rate_Control_Normal, 0x01, 0x2C, 0x2D,
                                                        (byte)GraphicDriverCommandType.Command, 4, (byte)Registers.Frame_Rate_Control_2, 0x01, 0x2C, 0x2D,
                                                        (byte)GraphicDriverCommandType.Command, 7, (byte)Registers.Frame_Rate_Control_3, 0x01, 0x2C, 0x2D, 0x01, 0x2C, 0x2D,
                                                        (byte)GraphicDriverCommandType.Command, 2, (byte)Registers.Invert_On, 0x07,
                                                        (byte)GraphicDriverCommandType.Command, 1, (byte)Registers.Invertion_On,
                                                        // 0x55 -> 16 bit
                                                        (byte)GraphicDriverCommandType.Command, 2, (byte)Registers.Pixel_Format_Set, 0x55,
                                                        (byte)GraphicDriverCommandType.Command, 4, (byte)Registers.Power_Control_1, 0xA2, 0x02, 0x84,
                                                        (byte)GraphicDriverCommandType.Command, 2, (byte)Registers.Power_Control_2, 0xC5,
                                                        (byte)GraphicDriverCommandType.Command, 3, (byte)Registers.Power_Control_3, 0x0A, 0x00,
                                                        (byte)GraphicDriverCommandType.Command, 3, (byte)Registers.Power_Control_4, 0x8A, 0x2A,
                                                        (byte)GraphicDriverCommandType.Command, 3, (byte)Registers.Power_Control_5, 0x8A, 0xEE,
                                                        (byte)GraphicDriverCommandType.Command, 4, (byte)Registers.VCOM_Control_1, 0x0E, 0xFF, 0xFF,
                                                        (byte)GraphicDriverCommandType.Command, 17, (byte)Registers.Positive_Gamma_Correction, 0x02, 0x1c, 0x7, 0x12, 0x37, 0x32, 0x29, 0x2d, 0x29, 0x25, 0x2B, 0x39, 0x00, 0x01, 0x03, 0x10,
                                                        (byte)GraphicDriverCommandType.Command, 17, (byte)Registers.Negative_Gamma_Correction, 0x03, 0x1d, 0x07, 0x06, 0x2E, 0x2C, 0x29, 0x2D, 0x2E, 0x2E, 0x37, 0x3F, 0x00, 0x00, 0x02, 0x1,
                                                        (byte)GraphicDriverCommandType.Command, 1, (byte)Registers.Sleep_Out,
                                                        (byte)GraphicDriverCommandType.Command, 1, (byte)Registers.Display_ON,
                                                        // Sleep 100 ms
                                                        (byte)GraphicDriverCommandType.Sleep, 10,
                                                        (byte)GraphicDriverCommandType.Command, 1, (byte)Registers.NOP,
                                                        // Sleep 20 ms
                                                        (byte)GraphicDriverCommandType.Sleep, 2,
                                                    },
                    OrientationLandscape = new byte[]
                                                    {
                                                        (byte)GraphicDriverCommandType.Command, 2, (byte)Registers.Memory_Access_Control, (byte)(Orientation.MADCTL_MY | Orientation.MADCTL_MX | Orientation.MADCTL_BGR),
                                                    },
                    PowerModeNormal = new byte[]
                                                    {
                                                        (byte)GraphicDriverCommandType.Command, 3, (byte)Registers.POWER_STATE, 0x00, 0x00,
                                                    },
                    PowerModeSleep = new byte[]
                                                    {
                                                        (byte)GraphicDriverCommandType.Command, 3, (byte)Registers.POWER_STATE, 0x00, 0x01,
                                                    },
                    DefaultOrientation = DisplayOrientation.Landscape,
                    Brightness = (byte)Registers.Write_Display_Brightness,
                };*/
            }
        }
    }
}
