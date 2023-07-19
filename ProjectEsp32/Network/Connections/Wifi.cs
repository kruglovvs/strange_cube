// Copyright kruglov.valentine@gmail.com KruglovVS.

using nanoFramework.M2Mqtt;
using nanoFramework.M2Mqtt.Messages;
using nanoFramework.Networking;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Text;

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
            WifiNetworkHelper.Disconnect();
        }
        public bool Reconnect()
        {
            return WifiNetworkHelper.Reconnect();
        }
    }
}