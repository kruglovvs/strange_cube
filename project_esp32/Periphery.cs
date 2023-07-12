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
using static DataNS.Data.Periphery;

namespace PeripheryNS
{
    public class PeripheryController
    {

        private static GpioController GpioController = new GpioController();

        private static Sensors.Button[] _buttons;
        private static Sensors.SimpleSensor _photoSensor;
        private static Sensors.SimpleSensor _vibrationSensor;
        private static Sensors.SimpleSensor _gasSensor;
        private static Sensors.TMP112 _tmp112;
        private static Sensors.LSM6 _lsm6;
        private static Actuators.Mechanical _vibrationMotor;
        private static Actuators.Mechanical _door;
        private static Displays.St7565WO12864 _display;
        private static Displays.Luminodiodes _luminodiodes;

        static PeripheryController()
        {
            _buttons = new Sensors.Button[Constants.Counts.Buttons];
            _photoSensor = new Sensors.SimpleSensor(Constants.Pins.PhotoSensor);
            _vibrationSensor = new Sensors.SimpleSensor(Constants.Pins.VibrationSensor);
            _gasSensor = new Sensors.SimpleSensor(Constants.Pins.GasSensor);
            _tmp112 = new Sensors.TMP112(Constants.Adresses.TMP112);
            _lsm6 = new Sensors.LSM6(Constants.Adresses.LSM6);
            _vibrationMotor = new Actuators.Mechanical(Constants.Pins.VibrationMotor);
            _door = new Actuators.Mechanical(Constants.Pins.Door);
            _display = new Displays.St7565WO12864(Constants.Pins.A0, Constants.Pins.ARes, Constants.Pins.ACs);
            _luminodiodes = new Displays.Luminodiodes(Constants.Pins.Luminoides, Constants.Counts.Luminodiodes);
            for (int i = 0; i < Constants.Counts.Buttons; i++)
            {
                _buttons[i] = new Sensors.Button(Constants.Pins.Buttons[i]);
            }
        }
        public bool IsTurnedOn
        {
            get
            {

            }
            set 
            { 

            }
        }
        public Data.Periphery.Image Image
        {
            set
            {
            }
        }

        public class ButtonPressedEventArgs : EventArgs
        {
            public int Number;
        }
        public event EventHandler<ButtonPressedEventArgs> ButtonPressed;
        public event EventHandler GasSensored;
        public event EventHandler TemperatureSensored;
        public event EventHandler PhotoSensored;

        private static class Constants
        {
            public static class Counts
            {
                public const int Buttons = 12;
                public const int Luminodiodes = 9;
                public static class PixelBytes
                {
                    public const int Luminodiodes = 12;
                    public const int St7565WO12864 = 404;
                }
                public static class CountPixels
                {
                    public const int Luminodiodes = 9;
                    public const int St7565WO12864 = 128 * 64;
                }
            }
            public static class Pins
            {
                public static readonly int[] Buttons = new int[Counts.Buttons] { 15, 2, 4, 404, 404, 5, 18, 404, 404, 33, 25, 26 }; //исправить
                public const int Luminoides = 34;
                public const int PhotoSensor = 27;
                public const int VibrationSensor = 35;
                public const int GasSensor = 404; // 
                public const int SDA = 21;
                public const int SCL = 22;
                public const int INT1 = 19;
                public const int INT2 = 23;
                public const int MOSI = 13;
                public const int CLK = 14;
                public const int A0 = 12;
                public const int ARes = 404;
                public const int ACs = 404;
                public const int VibrationMotor = 35;
                public const int Door = 32;
            }
            public static class PinModes
            {
                public static readonly PinMode Button = PinMode.Input;
                public static readonly PinMode SimpleSensor = PinMode.Input;
                public static readonly PinMode Mechanical = PinMode.Output;
                public static readonly PinMode Luminodiodes = PinMode.OutputOpenSourcePullDown;
                public static readonly PinMode A0 = PinMode.OutputOpenDrainPullUp;
                public static readonly PinMode ARes = PinMode.OutputOpenDrainPullUp;
                public static readonly PinMode ACs = PinMode.OutputOpenSourcePullDown;
            }
            public static class PinStartValues
            {
                public static readonly PinValue Mechanical = PinValue.Low;
                public static readonly PinValue Luminodiodes = PinValue.Low;
                public static readonly PinValue A0 = PinValue.High;
                public static readonly PinValue ARes = PinValue.High;
                public static readonly PinValue ACs = PinValue.High;
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
        private class Sensors
        {
            public interface ISimpleSensor
            {
                public GpioPin Pin { get; init; }
                public event EventHandler Sensored;
            }
            public class SimpleSensor : ISimpleSensor
            {
                public GpioPin Pin { get; init; }
                public SimpleSensor(int pinNumber)
                {
                    Pin = GpioController.OpenPin(pinNumber, Constants.PinModes.SimpleSensor);
                    Pin.ValueChanged += (s, e) =>
                    {
                        if((bool)Pin.Read()) { 
                            this.Sensored.Invoke(this, new EventArgs());
                        }
                    };
                }
                public event EventHandler Sensored;
                public event EventHandler Unsensored;
            }
            public class Button : GpioButton, ISimpleSensor
            {
                public int Pin { get; init; }
                public bool Value
                {
                    get
                    {
                        if (Value)
                        {
                            Value = false;
                            return true;
                        }
                        return false;
                    }
                    private set
                    {
                        Value = value;
                    }
                }
                public Button(int pinNumber) : base(pinNumber, GpioController, true, Constants.PinModes.Button)
                {
                    this.IsDoublePressEnabled = false; // it can be changed
                    this.IsHoldingEnabled = false; // it can be changed
                    this.ButtonDown += (sender, e) => { Value = true; };
                }
            }
            public interface II2cSensor
            {
                public double[] Read(byte register);
                public static class Registers { }
            }
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

            }
            public class TMP112 : I2cSensor, II2cSensor
            {
                public TMP112(int address) : base(address) { }
                public static class Registers
                {
                    public const byte Temperature = 0x0;
                    public const byte Configuration = 0x1;
                }
                public double[] Read(byte register)
                {
                    switch (register)
                    {
                        case Registers.Temperature: return new double[] { Temperature };
                        default: throw new Exception("wrong register");
                    }
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
            public class LSM6 : I2cSensor, II2cSensor
            {
                public LSM6(int address) : base(address) { }
                public static class Registers
                {
                    public const byte Temperature = 0x20; // { 0x20, 0x21 }
                    public const byte Rotation = 0x22; // { 0x22, 0x23, 0x24, 0x25, 0x26, 0x27 }
                    public const byte Accelation = 0x28; // { 0x28, 0x29, 0x2A, 0x2B, 0x2C, 0x2D }
                }
                public double[] Read(byte register)
                {
                    switch (register)
                    {
                        case Registers.Accelation: return ReadArray(register);
                        case Registers.Rotation: return ReadArray(register);
                        case Registers.Temperature: return new double[] { Temperature };
                        default: throw new Exception("wrong register");
                    }
                }
                private double[] ReadArray(byte register)
                {
                    double[] value = new double[3];
                    byte[] rawValue = new byte[6];
                    WriteRead(new byte[] { register }, new SpanByte(rawValue));
                    value[0] = rawValue[0] * 256 + rawValue[1];
                    value[1] = rawValue[2] * 256 + rawValue[3];
                    value[2] = rawValue[4] * 256 + rawValue[5];
                    return value;
                }
                public double[] Rotation
                {
                    get
                    {
                        return ReadArray(Registers.Rotation);
                    }
                }
                public double[] Accelation
                {
                    get
                    {
                        return ReadArray(Registers.Accelation);
                    }
                }
                public double Temperature
                {
                    get
                    {
                        byte[] rawTemperature = new byte[2];
                        Read(new SpanByte(rawTemperature));
                        return (rawTemperature[0] * 256 + rawTemperature[1]) / 16 * 0.0625; // idk it should be changed
                    }
                }
            }
        }
        private class Actuators
        {
            public interface IActuator
            {
                public int PinNumber { get; init; }
                public bool IsActing { get; set; }
            }
            public class Mechanical : IActuator
            {
                public int PinNumber { get; init; }
                public Mechanical(int pinNumber)
                {
                    GpioController.OpenPin(pinNumber, Constants.PinModes.Mechanical);
                    PinNumber = pinNumber;
                }
                public bool IsActing
                {
                    get
                    {
                        return IsActing;
                    }
                    set
                    {
                        GpioController.Write(PinNumber, (PinValue)value);
                        IsActing = value;
                    }
                }
            }
        }
        private class Displays
        {
            public interface IDisplay
            {
                public static int CountPixelBytes { get; }
                public int CountPixels { get; }
                public byte[] Image { set; }
            }
            public class Luminodiodes : IDisplay
            {
                public static int CountPixelBytes { get; } = Constants.Counts.PixelBytes.Luminodiodes;
                public int CountPixels { get; init; }
                public int PinNumber { get; init; }
                public Luminodiodes(int pinNumber, int countPixels)
                {
                    GpioController.OpenPin(pinNumber, Constants.PinModes.Luminodiodes);
                    PinNumber = pinNumber;
                    CountPixels = countPixels;

                }
                public byte[] Image
                {
                    set
                    {
                        // I should create dll driver for 1-wire luminodiodes
                    }
                }
            }
            public abstract class SpiDisplay
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
            }
            public class St7565WO12864 : SpiDisplay, IDisplay
            {
                public static int CountPixelBytes { get; } = Constants.Counts.PixelBytes.St7565WO12864;
                public int CountPixels { get; } = Constants.Counts.CountPixels.St7565WO12864;
                public static class StateA0
                {
                    public static readonly PinValue Data = PinValue.High;
                    public static readonly PinValue Control = PinValue.Low;
                }
                public int PinNumberA0 { get; init; }
                public int PinNumberARes { get; init; }
                public int PinNumberACs { get; init; }
                public St7565WO12864(int pinNumberA0, int pinNumberARes, int pinNumberACs) : base(new SpiConfiguration(1, -1, -1, -1, -1), new ScreenConfiguration(0, 0, Parameters.Width, Parameters.Height, GraphicDriver), 128 * 64)
                {
                    PinNumberA0 = pinNumberA0;
                    PinNumberARes = pinNumberARes;
                    PinNumberACs = pinNumberACs;
                    OpenPin(pinNumberA0,  )
                    GpioController.OpenPin(pinNumberA0, Constants.PinModes.A0);
                    GpioController.Write(pinNumberA0, PinValue.High);
                    GpioController.OpenPin(pinNumberARes, Constants.PinModes.ARes);
                    GpioController.Write(pinNumberARes, PinValue.High);
                    GpioController.OpenPin(pinNumberACs, Constants.PinModes.ACs);
                    GpioController.Write(pinNumberACs, PinValue.Low);
                }
                public void WritePoint(ushort x, ushort y)
                {
                    GpioController.Write(PinNumberA0, StateA0.Data);
                    DisplayControl.WritePoint(x, y, Color.Black);
                }
                public static class Registers
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
                public static class Orientation
                {
                    public const int
                    MADCTL_MH = 0x04, // sets the Horizontal Refresh, 0=Left-Right and 1=Right-Left
                    MADCTL_ML = 0x10, // sets the Vertical Refresh, 0=Top-Bottom and 1=Bottom-Top
                    MADCTL_MV = 0x20, // sets the Row/Column Swap, 0=Normal and 1=Swapped
                    MADCTL_MX = 0x40, // sets the Column Order, 0=Left-Right and 1=Right-Left
                    MADCTL_MY = 0x80, // sets the Row Order, 0=Top-Bottom and 1=Bottom-Top
                    MADCTL_BGR = 0x08; // Blue-Green-Red pixel order
                }
                public static class Parameters
                {
                    public const int Width = 128;
                    public const int Height = 64;
                }
                public byte[] Image { set { } }
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
