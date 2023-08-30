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
            Debug.WriteLine($"SimspleAction: {Instruction.Illuminate}");
            GotSimpleSensor?.Invoke(Instruction.Illuminate);
        };
        private static PinChangeEventHandler s_listeningVibrating { get; set; } = (sender, e) =>
        {
            Debug.WriteLine($"SimspleAction: {Instruction.Vibrate}");
            GotSimpleSensor?.Invoke(Instruction.Vibrate);
        };
        private static PinChangeEventHandler s_listeningSmoking { get; set; } = (sender, e) =>
        {
            Debug.WriteLine($"SimspleAction: {Instruction.Smoke}");
            GotSimpleSensor?.Invoke(Instruction.Smoke);
        };
        public static void TurnOn()
        {
            s_gpioController?.OpenPin(Constants.Pins.PhotoSensor, PinMode.InputPullUp);
            s_gpioController?.OpenPin(Constants.Pins.GasSensor, PinMode.InputPullUp);
            s_gpioController?.OpenPin(Constants.Pins.VibrationSensor, PinMode.InputPullUp);
        }

        private static bool s_isListeningPhotoSensor = false;
        private static bool s_isListeningVibrationSensor = false;
        private static bool s_isListeningGasSensor = false;
        public static bool IsListeningPhotoSensor
        {
            get
            {
                return s_isListeningPhotoSensor;
            }
            set
            {
                if (value)
                {
                    if (!IsListeningPhotoSensor)
                    s_gpioController?.RegisterCallbackForPinValueChangedEvent(Constants.Pins.PhotoSensor, PinEventTypes.Falling, s_listeningIllumination);
                }
                else
                {
                    if (IsListeningPhotoSensor)
                    s_gpioController?.UnregisterCallbackForPinValueChangedEvent(Constants.Pins.PhotoSensor, s_listeningIllumination);
                }
                s_isListeningPhotoSensor = value;
            }
        }
        public static bool IsListeningVibrationSensor
        {
            get
            {
                return s_isListeningVibrationSensor;
            }
            set
            {
                if (value)
                {
                    if (!IsListeningVibrationSensor)
                        s_gpioController?.RegisterCallbackForPinValueChangedEvent(Constants.Pins.VibrationSensor, PinEventTypes.Falling, s_listeningVibrating);
                }
                else
                {
                    if (IsListeningVibrationSensor)
                        s_gpioController?.UnregisterCallbackForPinValueChangedEvent(Constants.Pins.VibrationSensor, s_listeningVibrating);
                }
                s_isListeningVibrationSensor = value;
            }
        }
        public static bool IsListeningGasSensor
        {
            get
            {
                return s_isListeningGasSensor;
            }
            set
            {
                if (value)
                {
                    if (!IsListeningGasSensor)
                        s_gpioController?.RegisterCallbackForPinValueChangedEvent(Constants.Pins.GasSensor, PinEventTypes.Falling, s_listeningSmoking);
                }
                else
                {
                    if (IsListeningGasSensor)
                        s_gpioController?.UnregisterCallbackForPinValueChangedEvent(Constants.Pins.GasSensor, s_listeningSmoking);
                }
                s_isListeningGasSensor = value;
            }
        }
        public static event PeripheryGotActionEventHandler GotSimpleSensor;
    }
}