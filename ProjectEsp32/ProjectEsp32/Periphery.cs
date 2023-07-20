using Iot.Device.KeyMatrix;
using nanoFramework.Hardware.Esp32;
using System;
using System.Device.Gpio;
using System.Device.I2c;
using System.Device.Spi;
using System.Diagnostics;
using System.Threading;
using static ProjectESP32.Program;

namespace ProjectESP32 {
    public static class Periphery {

        // A0 
        // ARes
        //
        //
        //

        private static GpioController s_gpioController { get; set; } = new GpioController();
        private static KeyMatrix s_keyMatrix { get; set; }
        private static SpiDevice s_display { get; set; }
        private static I2cDevice s_tmp112 { get; set; }
        private static I2cDevice s_lsm6 { get; set; }
        private static Thread s_checkingTemperature { get; set; }
        private static Thread s_checkingRotation { get; set; }
        private static Thread s_checkingAccelation { get; set; }

        public static DelegateAction Action { get; set; }
        public static DelegateButtonPressed ActionButtonPressed {
            get {
                return ActionButtonPressed;
            }
            set {
                ActionButtonPressed = value;
                if (value == null) {
                    s_keyMatrix?.StopListeningKeyEvent();
                } else {
                    s_keyMatrix?.StartListeningKeyEvent();
                }
            }
        }

        public static void TurnOn() {

            //  Open all pins.
            s_gpioController.OpenPin(12, PinMode.Output); // A0.
            s_gpioController.OpenPin(26, PinMode.Output); // ARes.
            s_gpioController.OpenPin(27, PinMode.Output); // ACs.
            s_gpioController.OpenPin(13, PinMode.Output); // MOSI.
            Configuration.SetPinFunction(13, DeviceFunction.SPI1_MOSI);
            s_gpioController.OpenPin(14, PinMode.Output); // CLK.
            Configuration.SetPinFunction(14, DeviceFunction.SPI1_CLOCK);
            s_gpioController.OpenPin(19, PinMode.Input);  // Int1.
            s_gpioController.OpenPin(23, PinMode.Input);  // Int2.
            s_gpioController.OpenPin(21, PinMode.Output); // SDA.
            Configuration.SetPinFunction(21, DeviceFunction.I2C1_DATA);
            s_gpioController.OpenPin(22, PinMode.Output); // SCL.
            Configuration.SetPinFunction(13, DeviceFunction.I2C1_CLOCK);

            // These pins should be removed to other places.
            //s_gpioController.OpenPin(34, PinMode.Output); // Door.
            //IsDoorOpened = false;
            //s_gpioController.OpenPin(35, PinMode.Output); // VibrationMotor.
            //s_gpioController.Wite(35, PinValue.Low);

            // Create simple events.
            s_gpioController.RegisterCallbackForPinValueChangedEvent(25, PinEventTypes.Rising, (sender, e) => { Action?.Invoke(Instruction.Illuminate); });
            s_gpioController.RegisterCallbackForPinValueChangedEvent(25, PinEventTypes.Falling, (sender, e) => { Action?.Invoke(Instruction.Darken); });
            s_gpioController.RegisterCallbackForPinValueChangedEvent(36, PinEventTypes.Rising, (sender, e) => { Action?.Invoke(Instruction.Smoke); });
            s_gpioController.RegisterCallbackForPinValueChangedEvent(39, PinEventTypes.Rising, (sender, e) => { Action?.Invoke(Instruction.Vibrate); });

            // Create KeyMatrix.
            s_keyMatrix = new KeyMatrix(new int[] { 15, 2, 4 }, new int[] { 5, 17, 16 }, new TimeSpan(1), s_gpioController, false);
            s_keyMatrix.KeyEvent += (sender, e) => { ActionButtonPressed?.Invoke(e.Input + e.Output * 3); };
            s_keyMatrix.StopListeningKeyEvent();

            // Set Display.
            s_gpioController.Write(27, PinValue.High); // ACs.
            s_gpioController.Write(12, PinValue.High); // A0.
            s_gpioController.Write(26, PinValue.High); // ARes.
            Thread.Sleep(10);
            s_gpioController.Write(26, PinValue.Low); // ARes.

            // Create spi and i2c devices.
            s_display = new SpiDevice(new SpiConnectionSettings(1, 27));
            s_tmp112 = new I2cDevice(new I2cConnectionSettings(1, 0x49));
            s_lsm6 = new I2cDevice(new I2cConnectionSettings(1, 0x75));

            // Set power-on modes.
            //s_tmp112.Write(new SpanByte(new byte[] { 0x13, 0x01}));
            s_lsm6.Write(new SpanByte(new byte[] { 0x10, 0b01010000 }));
            s_lsm6.Write(new SpanByte(new byte[] { 0x10, 0b01010000 }));

            // Create threads that will check parameters for sensors.
            s_checkingTemperature = new Thread(() => {
                while (true) {
                    byte[] rawTemperature = new byte[2];
                    s_tmp112.WriteRead(new SpanByte(new byte[] { 0x0 }), new SpanByte(rawTemperature));
                    double temperature = (rawTemperature[0] * 256 + rawTemperature[1]) / 16 * 0.0625;
                    if (temperature > 35.0) {
                        Action?.Invoke(Instruction.HeatUp);
                    } else if (temperature < 15.0) {
                        Action?.Invoke(Instruction.CoolDown);
                    }
                    Thread.Sleep(50);
                }
            });
            s_checkingRotation = new Thread(() => {
                while (true) {
                    byte[] rawRotation = new byte[6];
                    s_lsm6.WriteRead(new SpanByte(new byte[] { 0x22 }), new SpanByte(rawRotation));
                    Debug.WriteLine($"Rotation read raw data: {rawRotation}");
                    Thread.Sleep(50);
                }
            });
            s_checkingAccelation = new Thread(() => {
                while (true) {
                    byte[] rawAccelation = new byte[6];
                    s_lsm6.WriteRead(new SpanByte(new byte[] { 0x28 }), new SpanByte(rawAccelation));
                    Debug.WriteLine($"Accelation read raw data: {rawAccelation}");
                    Thread.Sleep(50);
                }
            });
        }
        public static void SetLuminodiodds(byte[] image) {

        }
        public static void SetDisplay(byte[] image) {
            s_display.Write(new SpanByte(image));
        }
        public static void Vibrate(int timeMillisecons) {
            // s_gpioController.Write(35, PinValue.High); // VibrationMotor.
            Thread.Sleep(timeMillisecons);
            // s_gpioController.Write(35, PinValue.Low); // VibrationMotor.
        }
        public static void OpenDoor() {
            // s_gpioController.Write(34, PinValue.High); // Door
            Thread.Sleep(100000);
            // s_gpioController.Write(34, PinValue.Low); // Door
        }

        public delegate void DelegateAction(Instruction instruction);
        public delegate void DelegateButtonPressed(int buttonNumber);
    }
}
