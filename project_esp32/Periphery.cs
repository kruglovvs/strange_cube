using System;
using System.Device.Gpio;
using System.Diagnostics;
using System.Threading;

namespace Periphery
{
    public class Periphery_controller
    {
        static class Constants
        {
            public const int count_buttons = 10;
            public const int count_phototransistors = 2;
            public static readonly int[] pins_buttons = new int[count_buttons] { 22, 21, 25, 24, 23, 15, 7, 6, 18, 17 };
            public static readonly int[] pins_phototransistors = new int [count_phototransistors] { 28, 27 };
        }
        private class Button
        {
            private int pin_number;
            public Button(int _pin_number)
            {
                Periphery_controller.gpio_controller.OpenPin(_pin_number, PinMode.Input);
                pin_number = _pin_number;
            }
            public bool is_pressed
            {
                get
                {
                    return is_pressed;
                }
            }
        }
        private class Phototransistor
        {
            public bool is_illuminated { get; }
        }
        private class TMP112
        {
            public int temperature { get; }
        }
        private class LSM6
        {
            public int gyroscope { get; }
            public int accelerometer { get; }
        }
        private class Vibration_sensor
        {
            public bool is_vibrating { get; }
        }
        private class Gas_sensor
        {
            public bool is_gased { get; }
        }
        private class Vibration_motor
        {
            public bool is_vibrating { get; set; }
        }
        private class Door
        {
            public bool is_open { get; set; }
        }
        public class Data
        {
            public string data_string { get; }
        }

        static GpioController gpio_controller;

        private Button[] buttons = new Button[Constants.count_buttons];
        private Phototransistor[] phototransistors = new Phototransistor[Constants.count_phototransistors];
        private TMP112 tmp112 = new TMP112();
        private LSM6 lsm6 = new LSM6();
        private Vibration_sensor vibration_sensor = new Vibration_sensor();
        private Gas_sensor gas_sensor = new Gas_sensor();
        private Vibration_motor vibration_motor = new Vibration_motor();
        private Door door = new Door();
        private Data last_data = new Data();

        public Periphery_controller()
        {
            last_data = read_data();
        }

        public Data send_data() { return new Data(); }
        private Data read_data() { return new Data(); }
    }
}
