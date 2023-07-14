// Copyright kruglov.valentine@gmail.com KruglovVS.

using nanoFramework.M2Mqtt;
using nanoFramework.M2Mqtt.Messages;
using nanoFramework.Networking;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace NetworkNS
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
                Constants.Mqtt.clientID, Constants.Mqtt.Username, Constants.Mqtt.Password,
                Topics.Subscribed);
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
        public static bool Send(string topic, string message)
        {
            if (!IsConnected)
            {
                if (!Reconnect()) { return false; }
            }
            return s_mqtt.Publish(topic, message);
        }

        public class GotEventArgs : EventArgs
        {
            public string Topic { get; set; }
            public string Message { get; set; }
            public GotEventArgs(string topic, string message)
            {
                Topic = topic;
                Message = message;
            }
        }
        public static event EventHandler<GotEventArgs> Got;

        public static class Topics
        {
            public static readonly string[] Subscribed = { "/Instructions", "/Boot" };
            public static readonly string[] Publishing = { "/GameData" };
        }
        private static class Constants
        {
            public static class Wifi
            {
                public static readonly string SSID = "Guest_WiFi";
                public static readonly string Password = "NeRm25:)KmwQ";
            }
            public static class Mqtt
            {
                public static readonly string Ip = "127.0.0.1";
                public const int Port = 8080;
                public const bool UsingSecure = true;
                public static readonly X509Certificate CaCert = null;
                public static readonly X509Certificate ClientCert = null;
                public static readonly MqttSslProtocols MqttSslProtocol = MqttSslProtocols.SSLv3;

                public static readonly string clientID = "client";
                public static readonly string Username = "username";
                public static readonly string Password = "password";
            }
        }
        private class Clients
        {
            public interface IClient
            {
                public bool Publish(string topic, string message);
                public void Subscribe(string topic);
                public void Unsubscribe(string topic);
                public bool IsConnected { get; }
                public void Disconnect();
                public bool Reconnect();
            }
            public class Mqtt : MqttClient, IClient
            {
                private string _clientID { get; init; }
                private string _username { get; init; }
                private string _password { get; init; }
                public Mqtt(string brokerHostName, int brokerPort, bool secure, X509Certificate caCert,
                    X509Certificate clientCert, MqttSslProtocols sslProtocol,
                    string clientID, string username, string password, string[] topicsSubscribe) :
                    base(brokerHostName, brokerPort, secure, caCert, clientCert, sslProtocol)
                {
                    _clientID = clientID;
                    _username = username;
                    _password = password;
                    ProtocolVersion = MqttProtocolVersion.Version_5;
                    MqttMsgPublishReceived += (sender, e) => { Got.Invoke(s_mqtt, new GotEventArgs(e.Topic, Encoding.UTF8.GetString(e.Message, 0, e.Message.Length))); };
                    Connect(clientID, username, password);
                    foreach (string topic in topicsSubscribe)
                    {
                        Subscribe(topic);
                    }
                }
                public bool Publish(string topic, string message)
                {
                    Publish(topic, Encoding.UTF8.GetBytes(message), null, null, MqttQoSLevel.ExactlyOnce, false); // should be changed
                    return true;
                }
                public void Subscribe(string topic)
                {
                    Subscribe(new string[] { topic }, new MqttQoSLevel[] { MqttQoSLevel.ExactlyOnce });
                }
                public void Unsubscribe(string topic)
                {
                    Unsubscribe(new string[] { topic });
                }
                public bool Reconnect()
                {
                    return Connect(_clientID, _username, _password) == MqttReasonCode.Success;
                }
            }
        }
        private class Connections
        {
            public interface IConnection
            {
                public bool IsConnected { get; }
                public void Disconnect();
                public bool Reconnect();
            }
            public class Wifi : IConnection
            {
                public Wifi(string ssid, string password)
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
    }
}