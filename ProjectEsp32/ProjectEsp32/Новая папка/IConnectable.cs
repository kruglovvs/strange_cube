// Copyright kruglov.valentine@gmail.com KruglovVS.

namespace Network
{
    public interface IConnectable
    {
        public bool IsConnected { get; }
        public void Disconnect();
        public bool Reconnect();
    }
}
