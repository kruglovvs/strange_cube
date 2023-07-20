// Copyright kruglov.valentine@gmail.com KruglovVS.

using nanoFramework.Networking;
using System.Diagnostics;

namespace Network.Connections
{
    public class Wifi : IConnectable
    {
        public string _ssid { get; set; }
        public string _password { get; set; }
        internal Wifi(string ssid, string password)
        {
            _ssid = ssid;
            _password = password;
        }
        public bool Connect ()
        {
            WifiNetworkHelper.ConnectDhcp(_ssid, _password);
            return IsConnected;
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
            WifiNetworkHelper.Disconnect();
        }
        public bool Reconnect()
        {
            return WifiNetworkHelper.Reconnect();
        }
    }
}