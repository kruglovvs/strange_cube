// Copyright kruglov.valentine@gmail.com KruglovVS.

using nanoFramework.M2Mqtt;
using System;

namespace Network
{
    public class NetworkController
    {
        private static Connections.Wifi s_wifi = new Connections.Wifi("Guest_WiFi", "NeRm25:)KmwQ");
        private static Clients.Mqtt s_mqtt = new Clients.Mqtt("10.0.41.59", 1883, false,
            null, null, MqttSslProtocols.None,
            "esp32", "123", "123");


        public NetworkController() { }
        static NetworkController()
        {
            foreach (string topic in Clients.Topics.Subscribed)
            {
                s_mqtt.Subscribe(topic);
            }
            s_mqtt.Got += (sender, e) => { Got.Invoke(sender, new Clients.IClient.GotMessageEventArgs(e.Topic, e.Message)); };
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

       
        public static event Clients.IClient.GotMessageEventHandler Got;
    }
}