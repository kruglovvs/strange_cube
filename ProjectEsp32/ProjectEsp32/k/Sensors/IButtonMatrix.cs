// Copyright kruglov.valentine@gmail.com KruglovVS.

using System;

namespace Periphery.Sensors
{
    public interface IButtonMatrix
    {
        public bool IsListening { get; set; }
        public delegate void ButtonMatrixEventHandler(object sender, int buttonNumber);
        public event ButtonMatrixEventHandler ButtonMatrixPressed;
    }
}
