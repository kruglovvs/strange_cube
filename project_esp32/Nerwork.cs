using System.Device.Gpio;
using System.Device.Wifi;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Security;
using System.Net.Sockets;
using System;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Net.WebSockets;
using nanoFramework.M2Mqtt;

using nanoFramework.Networking;
using System.Security.Cryptography.X509Certificates;
using static NetworkNS.NetworkController.Constants;
using nanoFramework.M2Mqtt.Messages;


//read https://github.com/nanoframework/nanoFramework.m2mqtt
// https://github.com/nanoframework/Samples/blob/main/samples/MQTT
// https://github.com/nanoframework/Samples/tree/main/samples/Wifi

namespace NetworkNS
{
    public static class NetworkController
    {
        private static Connections.Wifi Wifi = new Connections.Wifi(Constants.Wifi.SSID, Constants.Wifi.Password, Constants.Wifi.TimeWait);

        public static class Constants
        {
            public static class Wifi
            {
                public static readonly string SSID = "Guest_WiFi";
                public static readonly string Password = "NeRm25:)KmwQ";
                public const int TimeWait = 60000; // milliseconds
            }
            public static class Mqtt
            {
                public static readonly string Ip = "127.0.0.1";
                public const int Port = 8080;
                public const bool UsingSecure = false;
                public static readonly X509Certificate CaCert = null;
                public static readonly X509Certificate ClientCert = null;
                public static readonly MqttSslProtocols MqttSslProtocol = MqttSslProtocols.None;

                public static readonly string clientID = "client";
                public static readonly string Username = "username";
                public static readonly string Password = "password";
            }
        }
        private class Clients
        {
            public interface IClient
            {
                public bool IsConnected { get; }
                public bool Send(string message);
                public string Read(); 
            }
            public class Mqtt : IClient
            {
                private MqttClient _mqttClient;
                public Mqtt(string brokerHostName, int brokerPort, bool secure, X509Certificate caCert, X509Certificate clientCert, MqttSslProtocols sslProtocol, 
                    string clientID, string username, string password)
                {
                    _mqttClient = new MqttClient(brokerHostName, brokerPort, secure, caCert, clientCert, sslProtocol);
                    if (_mqttClient.Connect(clientID, username, password) != MqttReasonCode.Success) // connecting
                    {
                        Debug.WriteLine("ERROR connecting MQTT");
                        _mqttClient.Disconnect();
                        return;
                    }
                    client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;

                }
                public bool IsConnected
                {
                    get
                    {
                        return _mqttClient.IsConnected;
                    }
                }
                public bool Send(string message)
                {

                }
                public string Read()
                {

                }

            }
        }
        private class Connections
        {
            public interface IConnection
            {
                public bool IsConnected { get; set; }
                public abstract void Disconnect();
                public abstract bool Reconnect(int timeWait);
            }
            public class Wifi : IConnection
            {
                public Wifi(string ssid, string password, int timeWait)
                {
                    bool IsConnected = WifiNetworkHelper.ConnectDhcp(ssid, password, requiresDateTime: true, token: new CancellationTokenSource(timeWait).Token);
                    if (!IsConnected)
                    {
                        // Something went wrong, you can get details with the ConnectionError property:
                        Debug.WriteLine($"Can't connect to the network, error: {WifiNetworkHelper.Status}");
                        if (WifiNetworkHelper.HelperException != null)
                        {
                            Debug.WriteLine($"ex: {WifiNetworkHelper.HelperException}");
                        }
                    }
                }
                public bool IsConnected { get; set; }
                public void Disconnect()
                {
                    WifiNetworkHelper.Disconnect();
                    IsConnected = false;
                }
                public bool Reconnect(int timeWait)
                {
                    IsConnected = WifiNetworkHelper.Reconnect(token: new CancellationTokenSource(timeWait).Token);
                    return IsConnected;
                }
            }
        }   
    } 
}