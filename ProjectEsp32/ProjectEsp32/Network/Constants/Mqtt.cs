// Copyright kruglov.valentine@gmail.com KruglovVS.

using nanoFramework.M2Mqtt;
using System.Security.Cryptography.X509Certificates;

namespace Network.Constants
{
    public static class Mqtt
    {
        public const string Ip = "10.0.41.59";
        public const int Port = 1883;
        public const bool UsingSecure = false;
        public const X509Certificate CaCert = null;
        public const X509Certificate ClientCert = null;
        public const MqttSslProtocols MqttSslProtocol = MqttSslProtocols.None;

        public const string ClientID = "Esp32";
        public const string Username = "Kyala";
        public const string Password = "123";
    }
}