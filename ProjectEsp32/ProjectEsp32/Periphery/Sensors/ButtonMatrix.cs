// Copyright kruglov.valentine@gmail.com KruglovVS.

using Iot.Device.KeyMatrix;
using System;
using System.Device.Gpio;

namespace Periphery.Sensors
{
    public class ButtonMatrix : KeyMatrix, IButtonMatrix
    {
        internal ButtonMatrix(int[] pinNumbersIn, int[] pinNumbersOut, GpioController gpioController) : base(pinNumbersOut, pinNumbersIn, new TimeSpan(10), gpioController, false)
        {
            KeyEvent += (sender, e) =>
            {
                ButtonMatrixPressed?.Invoke(this, e.Output + (e.Input * pinNumbersOut.Length));
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
                IsListening = value;
            }
        }
        public event IButtonMatrix.ButtonMatrixEventHandler ButtonMatrixPressed;
    }
}