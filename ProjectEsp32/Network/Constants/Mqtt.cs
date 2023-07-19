// Copyright kruglov.valentine@gmail.com KruglovVS.

using nanoFramework.M2Mqtt;
using nanoFramework.M2Mqtt.Messages;
using nanoFramework.Networking;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Network.Clients
{
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