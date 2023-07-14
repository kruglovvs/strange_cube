// copyright kruglovvs kruglov.valentine@gmail.com

using DataNS;
using Iot.Device.Button;
using Iot.Device.KeyMatrix;
using nanoFramework.Hardware.Esp32;
using nanoFramework.UI;
using System;
using System.Device.Gpio;
using System.Device.I2c;
using System.Diagnostics;
using System.Drawing;
using System.Threading;

namespace PeripheryNS
{
    public static class PeripheryController
    {

        private static GpioController s_gpioController = new GpioController();

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
            s_gpioController = new GpioController();
            s_buttons = new Sensors.Button[Constants.Counts.Buttons];
            s_matrix = new Sensors.ButtonMatrix(Constants.Pins.ButtonsIn, Constants.Pins.ButtonsOut);
            s_photoSensor = new Sensors.PhotoSensor(Constants.Pins.PhotoSensor);
            s_vibrationSensor = new Sensors.VibrationSensor(Constants.Pins.VibrationSensor);
            s_gasSensor = new Sensors.GasSensor(Constants.Pins.GasSensor);
            s_tmp112 = new Sensors.TMP112(Constants.Adresses.TMP112);
            s_lsm6 = new Sensors.LSM6(Constants.Adresses.LSM6);
            s_vibrationMotor = new Actuators.Mechanical(Constants.Pins.VibrationMotor);
            s_door = new Actuators.Mechanical(Constants.Pins.Door);
            s_display = new Displays.St7565WO12864(Constants.Pins.A0, Constants.Pins.ARes, Constants.Pins.ACs);
            s_luminodiodes = new Displays.Luminodiodes(Constants.Pins.Luminoides, Constants.Counts.Luminodiodes);
            for (int i = 0; i < Constants.Counts.Buttons; i++)
            {
                s_buttons[i] = new Sensors.Button(Constants.Pins.Buttons[i]);
            }
            IsTurnedOn = true;
            IsOpened = false;
            IsVibrating = false;
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
                if (!value)
                {
                    IsOpened = false;
                    IsVibrating = false;
                }
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
        public static Data.Periphery.Image Image
        {
            set
            {
            }
        }

        public class ButtonPressedEventArgs : EventArgs
        {
            public int PinNumber { get; set; }
            public ButtonPressedEventArgs(int pinNumber) 
            {
                PinNumber = pinNumber;
            }
        }
        public static event EventHandler<ButtonPressedEventArgs> ButtonPressed;
        public static event EventHandler PhotoSensored;
        public static event EventHandler VibrationSensored;
        public static event EventHandler GasSensored;
        public static event EventHandler TemperatureAlertSensored;

        private static class Constants
        {
            public static class Counts
            {
                public const int Buttons = 2;
                public const int ButtonMatrix = 3;
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
                public static readonly int[] Buttons = new int[Counts.Buttons] { 15, 2 }; //исправить
                public static readonly int[] ButtonsIn = new int[Counts.ButtonMatrix] { 404, 404, 404 };
                public static readonly int[] ButtonsOut = new int[Counts.ButtonMatrix] { 404, 404, 404 };
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
                public static readonly PinMode ButtonIn = PinMode.Input;
                public static readonly PinMode ButtonOut = PinMode.OutputOpenDrainPullUp;
                public static readonly PinMode SimpleSensor = PinMode.Input;
                public static readonly PinMode Mechanical = PinMode.Output;
                public static readonly PinMode Luminodiodes = PinMode.OutputOpenSourcePullDown;
                public static readonly PinMode A0 = PinMode.OutputOpenDrainPullUp;
                public static readonly PinMode ARes = PinMode.OutputOpenDrainPullUp;
                public static readonly PinMode ACs = PinMode.OutputOpenSourcePullDown;
            }
            public static class PinStartValues
            {
                public static readonly PinValue ButtonOut = PinValue.High;
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
            public static class Time
            {
                public static readonly TimeSpan Debounce = new TimeSpan(10);
                public static readonly TimeSpan SetARes = new TimeSpan(10);
            }
        }
        private class Sensors
        {
            public interface ISimpleSensor
            {
                public GpioPin Pin { get; init; }
                public bool Sensor { get; }
            }
            public abstract class SimpleSensor : ISimpleSensor
            {
                public GpioPin Pin { get; init; }
                public SimpleSensor(int pinNumber)
                {
                    Pin = s_gpioController.OpenPin(pinNumber, Constants.PinModes.SimpleSensor);
                    Pin.ValueChanged += (sender, e) =>
                    {
                        if((bool)Pin.Read()) 
                        { 
                            this.Sensored.Invoke(this, new EventArgs());
                        } 
                    };
                }
                public event EventHandler Sensored;
                public bool Sensor
                {
                    get
                    {
                        return (bool)Pin.Read();
                    }
                }
            }
            public class PhotoSensor : SimpleSensor
            {
                public PhotoSensor (int pinNumber) : base(pinNumber)
                {
                    Sensored += (sender, e) => PhotoSensored.Invoke(this, new EventArgs());
                }
            }
            public class VibrationSensor : SimpleSensor
            {
                public VibrationSensor(int pinNumber) : base(pinNumber)
                {
                    Sensored += (sender, e) => VibrationSensored.Invoke(this, new EventArgs());
                }
            }
            public class GasSensor : SimpleSensor
            {
                public GasSensor(int pinNumber) : base(pinNumber)
                {
                    Sensored += (sender, e) => GasSensored.Invoke(this, new EventArgs());
                }
            }
            public class Button : GpioButton, ISimpleSensor
            {
                public GpioPin Pin { get; init; }
                public Button(int pinNumber) : base(pinNumber, s_gpioController, true, Constants.PinModes.Button, Constants.Time.Debounce)
                {
                    IsDoublePressEnabled = false; 
                    IsHoldingEnabled = false; 
                    IsPressed = true;
                    Press += (sender, e) => { ButtonPressed.Invoke(this,  new ButtonPressedEventArgs(Pin.PinNumber + 100)); } ;
                }
                public bool Sensor
                {
                    get
                    {
                        return (bool)Pin.Read();
                    }
                }
            }
            public interface IButtonMatrix
            {
                public bool IsListening { get; set; }
            }
            public class ButtonMatrix : KeyMatrix, IButtonMatrix
            {
                public ButtonMatrix(int[] pinNumbersIn, int[] pinNumbersOut) : base (pinNumbersOut, pinNumbersIn, Constants.Time.Debounce, s_gpioController, true)
                {
                    if (pinNumbersIn.Length != pinNumbersOut.Length)
                    {
                        throw new Exception("Matrix should be a square");
                    }
                    KeyEvent += (sender, e) => 
                    {
                        ButtonPressed.Invoke(this, new ButtonPressedEventArgs(e.Input + e.Output * pinNumbersIn.Length));
                    };
                    IsListening = true;
                }
                public bool IsListening 
                { 
                    get 
                    { 
                        return IsListening; 
                    } 
                    set 
                    {
                        if (value)
                        {
                            StartListeningKeyEvent();
                        }
                        else
                        {
                            StopListeningKeyEvent();
                        }
                    }
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
                GpioPin Alert { get; init; }
                public TMP112(int address) : base(address)
                {
                    Alert.ValueChanged += (sender, e) =>
                    {
                        if ((bool)Alert.Read())
                        {
                            TemperatureAlertSensored.Invoke(this, new EventArgs());
                        }
                    };
                }
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
                public GpioPin Pin { get; init; }
                public bool IsActing { get; set; }
            }
            public class Mechanical : IActuator
            {
                public GpioPin Pin { get; init; }
                public Mechanical(int pinNumber)
                {
                    Pin = s_gpioController.OpenPin(pinNumber, Constants.PinModes.Mechanical);
                    Pin.Write(Constants.PinStartValues.Mechanical);
                    IsActing = (bool)Constants.PinStartValues.Mechanical;
                }
                public bool IsActing
                {
                    get
                    {
                        return IsActing;
                    }
                    set
                    {
                        Pin.Write((PinValue)value);
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
                public GpioPin Pin { get; init; }
                public Luminodiodes(int pinNumber, int countPixels)
                {
                    Pin = s_gpioController.OpenPin(pinNumber, Constants.PinModes.Luminodiodes);
                    Pin.Write(Constants.PinStartValues.Luminodiodes);
                    CountPixels = countPixels;
                    IsTurnedOn = (bool)Constants.PinStartValues.Luminodiodes;
                }
                public byte[] Image
                {
                    set
                    {
                        // I should create dll driver for 1-wire luminodiodes
                    }
                }
                public bool IsTurnedOn
                {
                    get { return IsTurnedOn; }
                    set
                    {
                        if (!value)
                        {
                            Image = new byte[CountPixels];
                        }
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
                public bool IsTurnedOn
                {
                    get { return IsTurnedOn; }
                    set
                    {
                        PinACs.Write(value);
                        if (!IsTurnedOn && value) 
                        {
                            PinARes.Write(PinValue.Low);
                            Thread.Sleep(Constants.Time.SetARes);
                        }
                    }
                }
                public static int CountPixelBytes { get; } = Constants.Counts.PixelBytes.St7565WO12864;
                public int CountPixels { get; } = Constants.Counts.CountPixels.St7565WO12864;
                private static class StateA0
                {
                    public static readonly PinValue Data = PinValue.High;
                    public static readonly PinValue Control = PinValue.Low;
                }
                public GpioPin PinA0 { get; init; }
                public GpioPin PinARes { get; init; }
                public GpioPin PinACs { get; init; }
                private bool A0
                {
                    get
                    {
                        return A0;
                    }
                    set
                    {
                        PinA0.Write(value);
                    }
                }
                private bool ARes
                {
                    get
                    {
                        return ARes;
                    }
                    set
                    {
                        PinARes.Write(value);
                    }
                }
                public bool ACs
                {
                    get
                    {
                        return ACs;
                    }
                    set
                    {
                        PinACs.Write(value);
                    }
                }
                public St7565WO12864(int pinNumberA0, int pinNumberARes, int pinNumberACs) : base(new SpiConfiguration(1, -1, -1, -1, -1), new ScreenConfiguration(0, 0, Parameters.Width, Parameters.Height, GraphicDriver), 128 * 64)
                {
                    PinA0 = s_gpioController.OpenPin(pinNumberA0, Constants.PinModes.A0);
                    s_gpioController.Write(pinNumberA0, Constants.PinStartValues.A0);
                    PinARes = s_gpioController.OpenPin(pinNumberARes, Constants.PinModes.ARes);
                    s_gpioController.Write(pinNumberARes, Constants.PinStartValues.ARes);
                    PinACs = s_gpioController.OpenPin(pinNumberACs, Constants.PinModes.ACs);
                    s_gpioController.Write(pinNumberACs, Constants.PinStartValues.ACs);
                    IsTurnedOn = (bool)Constants.PinStartValues.ACs;
                }
                public void WritePoint(ushort x, ushort y)
                {
                    PinA0.Write(StateA0.Data);
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
                    Writes_display_Brightness = 0x51,
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
                    Brightness = (byte)Registers.Writes_display_Brightness,
                };*/
            }
        }

    }
}
