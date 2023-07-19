using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectESP32.Network
{
    public interface IConnectable
    {
        public bool IsConnected { get; }
        public void Disconnect();
        public bool Reconnect();
    }
}
