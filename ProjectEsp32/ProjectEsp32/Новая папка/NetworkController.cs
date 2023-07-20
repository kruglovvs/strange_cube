// Copyright kruglov.valentine@gmail.com KruglovVS.

using System;

namespace Network
{
    public static class NetworkController
    {
        private static Connections.Wifi s_wifi;
        private static Clients.Mqtt s_mqtt;

        static NetworkController()
        {
            s_wifi = new Connections.Wifi(Constants.Wifi.SSID, Constants.Wifi.Password);
            s_mqtt = new Clients.Mqtt(Constants.Mqtt.Ip, Constants.Mqtt.Port, Constants.Mqtt.UsingSecure,
            Constants.Mqtt.CaCert, Constants.Mqtt.ClientCert, Constants.Mqtt.MqttSslProtocol,
            Constants.Mqtt.clientID, Constants.Mqtt.Username, Constants.Mqtt.Password);
            foreach (string topic in Clients.Topics.Subscribed)
            {
                s_mqtt.Subscribe(topic);
            }
            s_mqtt.Got += (sender, e) => { Got.Invoke(sender, e); };
        }

        public static bool IsConnected
        {
            get
            {
                return s_mqtt.IsConnected && s_wifi.IsConnected;
            }
            set
            {
                if (value)
                {
                    Reconnect();
                }
                else
                {
                    Disconnect();
                }
            }
        }
        public static void Disconnect()
        {
            s_mqtt.Disconnect();
            s_wifi.Disconnect();
        }
        public static bool Reconnect()
        {
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
            s_mqtt.Subscribe(topic);
        }

        public static event EventHandler<Clients.IClient.GotMessageEventArgs> Got;
    }
}