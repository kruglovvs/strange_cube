// Copyright kruglov.valentine@gmail.com KruglovVS.

using Iot.Device.Button;
using Iot.Device.KeyMatrix;
using System;
using System.Device.Gpio;
using System.Diagnostics;
using static Periphery.PeripheryController;
using Game;

namespace Periphery.Sensors
{
    public static class ButtonsController 
    {
        private static KeyMatrix s_keyMatrix { get; set; }
        private static GpioButton[] s_buttons { get; set; }

        public static void TurnOn()
        {
            s_keyMatrix = new KeyMatrix(Constants.Pins.ButtonsOut, Constants.Pins.ButtonsIn, Constants.Time.Debounce, s_gpioController, false);
            s_keyMatrix.KeyEvent += (sender, e) =>
            {
                if (e.EventType == PinEventTypes.Rising)
                {
                    GotButton?.Invoke((Instruction)((int)Instruction.ButtonPress0 + e.Input * 3 + e.Output));
                }
            };
            s_keyMatrix.StopListeningKeyEvent();

            s_buttons = new GpioButton[Constants.Pins.Buttons.Length];
            for (int i = 0; i < Constants.Counts.Buttons; ++i) 
            {
                s_buttons[i] = new GpioButton(Constants.Pins.Buttons[i], s_gpioController, false, Constants.PinModes.Button, Constants.Time.Debounce);
                s_buttons[i].Press += (sender, e) => { GotButton?.Invoke((Instruction)((int)Instruction.ButtonDisplayPress0+ i)); };
            }
        }
        public static bool IsListeningMatrix
        {
            set
            {
                if (value)
                {
                    s_keyMatrix?.StartListeningKeyEvent();
                }
                else
                {
                    s_keyMatrix?.StopListeningKeyEvent();
                }
            }
        }

        public static event PeripheryGotActionEventHandler GotButton;
    }
}