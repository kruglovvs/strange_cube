// Copyright kruglov.valentine@gmail.com KruglovVS.

using nanoFramework.M2Mqtt;
using System.Diagnostics;

namespace Network
{
    public static class NetworkController
    {
        private static Connections.Wifi s_wifi = new Connections.Wifi(Constants.Wifi.SSID, Constants.Wifi.Password);
        private static Clients.Mqtt s_mqtt = new Clients.Mqtt(Constants.Mqtt.Ip, Constants.Mqtt.Port,
                        Constants.Mqtt.UsingSecure, Constants.Mqtt.CaCert, Constants.Mqtt.ClientCert, Constants.Mqtt.MqttSslProtocol,
                        Constants.Mqtt.ClientID, Constants.Mqtt.Username, Constants.Mqtt.Password);

        public static bool IsConnected
        {
            get
            {
                return s_mqtt.IsConnected && s_wifi.IsConnected;
            }
        }
        public static bool Connect()
        {
            s_wifi.Connect();
            if (s_wifi.IsConnected)
            {
                s_mqtt.Connect();
                s_mqtt.Got += (sender, e) => { Got.Invoke(sender, e); };
            }
            return IsConnected;
        }
        public static void Disconnect()
        {
            s_wifi.Disconnect();
            s_mqtt.Disconnect();
        }
        public static bool Reconnect()
        {
            if (s_wifi == null || s_mqtt == null)
            {
                Connect();
            }
            if (!s_wifi.IsConnected)
            {
                s_wifi.Reconnect();
            }
            if (!s_mqtt.IsConnected)
            {
                s_mqtt.Reconnect();
            }
            return IsConnected;
        }
        public static bool Publish(string topic, string message)
        {
            if (!IsConnected)
            {
                if (!Reconnect()) { return false; }
            }
            return s_mqtt.Publish(topic, message);
        }
        public static void Subscribe(string topic)
        {
            if (!IsConnected)
            {
                if (!Reconnect()) { return; }
            }
            s_mqtt?.Subscribe(topic);
        }
        public static void Unsubscribe(string topic)
        {
            s_mqtt?.Unsubscribe(topic);
        }

       
        public static event Clients.IClient.GotMessageEventHandler Got;
    }
}