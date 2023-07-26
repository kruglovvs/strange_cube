// Copyright kruglov.valentine@gmail.com KruglovVS.

using System;
using System.Device.Gpio;
using System.Diagnostics;
using static Periphery.PeripheryController;
using Periphery;
using Game;

namespace Periphery.Sensors
{
    public static class SimpleSensorsController 
    {
        private static PinChangeEventHandler s_listeningIllumination { get; set; } = (sender, e) =>
        {
            GotSimpleSensor?.Invoke(Instruction.Illuminate);
        };
        private static PinChangeEventHandler s_listeningVibrating { get; set; } = (sender, e) =>
        {

            GotSimpleSensor?.Invoke(Instruction.Vibrate);

        };
        private static PinChangeEventHandler s_listeningSmoking { get; set; } = (sender, e) =>
        {

            GotSimpleSensor?.Invoke(Instruction.Smoke);

        };
        public static void TurnOn()
        {
            s_gpioController?.OpenPin(Constants.Pins.PhotoSensor, PinMode.InputPullUp);
            s_gpioController?.OpenPin(Constants.Pins.GasSensor, PinMode.InputPullUp);
            s_gpioController?.OpenPin(Constants.Pins.VibrationSensor, PinMode.InputPullUp);
        }
        public static bool IsListeningPhotoSensor
        {
            set
            {
                if (value)
                {
                    s_gpioController?.RegisterCallbackForPinValueChangedEvent(Constants.Pins.PhotoSensor, PinEventTypes.Falling, s_listeningIllumination);
                }
                else
                {
                    s_gpioController?.UnregisterCallbackForPinValueChangedEvent(Constants.Pins.PhotoSensor, s_listeningIllumination);
                }
            }
        }
        public static bool IsListeningVibrationSensor
        {
            set
            {
                if (value)
                {
                    s_gpioController?.RegisterCallbackForPinValueChangedEvent(Constants.Pins.PhotoSensor, PinEventTypes.Falling, s_listeningVibrating);
                }
                else
                {
                    s_gpioController?.UnregisterCallbackForPinValueChangedEvent(Constants.Pins.PhotoSensor, s_listeningVibrating);
                }
            }
        }
        public static bool IsListeningGasSensor
        {
            set
            {
                if (value)
                {
                    s_gpioController?.RegisterCallbackForPinValueChangedEvent(Constants.Pins.PhotoSensor, PinEventTypes.Falling, s_listeningSmoking);
                }
                else
                {
                    s_gpioController?.UnregisterCallbackForPinValueChangedEvent(Constants.Pins.PhotoSensor, s_listeningSmoking);
                }
            }
        }
        public static event PeripheryGotActionEventHandler GotSimpleSensor;
    }
}