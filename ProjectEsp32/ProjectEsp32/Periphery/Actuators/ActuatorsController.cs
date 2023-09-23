// Copyright kruglov.valentine@gmail.com KruglovVS.

using System.Device.Gpio;
using System.Threading;
using static Periphery.PeripheryController;

namespace Periphery.Actuators
{
    public static class ActuatorsController
    {
        private static Mechanical s_vibrationMotor { get; set; }
        private static Mechanical s_door { get; set; }
        public static void TurnOn() {
            s_door = new Mechanical(Constants.Pins.Door, s_gpioController);
            s_vibrationMotor = new Mechanical(Constants.Pins.VibrationMotor, s_gpioController);
        }
        public static void Vibrate() {
            s_vibrationMotor.Actuate();
        }
        public static void OpenDoor() {
            s_door.Actuate();
        }
    }

}
