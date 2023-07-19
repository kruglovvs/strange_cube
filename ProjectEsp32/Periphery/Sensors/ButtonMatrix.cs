// copyright kruglovvs kruglov.valentine@gmail.com

using Iot.Device.KeyMatrix;
using System;

namespace ProjectESP32.Periphery.Sensors
{
    public class ButtonMatrix : KeyMatrix, IButtonMatrix
    {
        internal ButtonMatrix(int[] pinNumbersIn, int[] pinNumbersOut) : base(pinNumbersOut, pinNumbersIn, Constants.Time.Debounce, s_gpioController, false)
        {
            KeyEvent += (sender, e) =>
            {
                ButtonMatrixPressed.Invoke(this, e.Output + (e.Input * pinNumbersOut.Length));
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
        public event EventHandler<int> ButtonMatrixPressed;
    }
}