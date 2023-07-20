// Copyright kruglov.valentine@gmail.com KruglovVS.

using nanoFramework.M2Mqtt;
using System.Security.Cryptography.X509Certificates;

namespace Network.Constants
{
    public static class Mqtt
    {
        public static readonly string Ip = "10.0.41.59";
        public const int Port = 1883;
        public const bool UsingSecure = false;
        public static readonly X509Certificate CaCert = null;
        public static readonly X509Certificate ClientCert = null;
        public static readonly MqttSslProtocols MqttSslProtocol = MqttSslProtocols.SSLv3;

        public static readonly string clientID = "client";
        public static readonly string Username = "username";
        public static readonly string Password = "password";
    }

}