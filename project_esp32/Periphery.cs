using System;
using System.Device.Gpio;
using System.Diagnostics;
using System.Threading;

namespace Periphery
{
    static class Constants
    {
        public const int count_buttons = 10;
        public const int count_phototransistor = 2;
    }
    public class Button
    {
        public bool is_pressed { get; }
    }
    public class Phototransistor
    {
        public bool is_illuminated { get; }
    }
    public class TMP112
    {
        public int temperature { get; }
    }
    public class LSM6
    {
        public int gyroscope { get; }
        public int accelerometer { get; }
    }
    public class Vibration_sensor
    {
        public bool is_vibrating { get; }
    }
    public class Gas_sensor
    {
        public bool is_gased { get; }
    }
    public class Vibration_motor 
    { 
        public bool is_vibrating { get;  set; }
    }
    public class Door
    {
        public bool is_open { get; set; }
    }
    public class Data
    {
        public string data_string { get; set; }
    }
    public class Periphery_controller
    {
        static GpioController s_GpioController;
        static int s_GreenPinNumber;
        static int s_RedPinNumber;
        static int s_UserButtonPinNumber;

        private Button[] buttons = new Button[Constants.count_buttons];
        private Phototransistor[] phototransistors = new Phototransistor[Constants.count_phototransistor];
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
