// Copyright kruglov.valentine@gmail.com KruglovVS.

using nanoFramework.Networking;

namespace Network.Connections
{
    public class Wifi : IConnectable
    {
        internal Wifi(string ssid, string password)
        {
            WifiNetworkHelper.ConnectDhcp(ssid, password);
        }
        public bool IsConnected
        {
            get
            {
                return WifiNetworkHelper.Status == NetworkHelperStatus.NetworkIsReady;
            }
        }
        public void Disconnect()
        {
            //WifiNetworkHelper.Disconnect();
        }
        public bool Reconnect()
        {
            return WifiNetworkHelper.Reconnect();
        }
    }
}