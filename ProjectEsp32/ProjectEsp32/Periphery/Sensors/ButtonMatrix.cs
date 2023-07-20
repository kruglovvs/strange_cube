// Copyright kruglov.valentine@gmail.com KruglovVS.

using Iot.Device.KeyMatrix;
using System;

namespace Periphery.Sensors
{
    public class ButtonMatrix : KeyMatrix, IButtonMatrix
    {
        internal ButtonMatrix(int[] pinNumbersIn, int[] pinNumbersOut) : base(pinNumbersOut, pinNumbersIn, new TimeSpan(10), PeripheryController.GpioController, false)
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