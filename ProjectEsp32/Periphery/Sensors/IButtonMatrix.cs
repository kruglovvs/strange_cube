// copyright kruglovvs kruglov.valentine@gmail.com

using System;

namespace ProjectESP32.Periphery.Sensors
{
    public interface IButtonMatrix
    {
        public bool IsListening { get; set; }
        public event EventHandler<int> ButtonMatrixPressed;
    }
}
