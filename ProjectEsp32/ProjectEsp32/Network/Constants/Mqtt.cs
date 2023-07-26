// Copyright kruglov.valentine@gmail.com KruglovVS.

using nanoFramework.M2Mqtt;
using System.Security.Cryptography.X509Certificates;

namespace Network.Constants
{
    internal static class Mqtt
    {
        internal static readonly string Ip = "10.0.41.64";
        internal static readonly int Port = 1883;
        internal static readonly bool UsingSecure = false;
        internal static readonly X509Certificate CaCert = null;
        internal static readonly X509Certificate ClientCert = null;
        internal static readonly MqttSslProtocols MqttSslProtocol = MqttSslProtocols.None;

        internal static readonly string ClientID = "Esp32";
        internal static readonly string Username = "Esp32";
        internal static readonly string Password = "JKnni9393((@Jjc992kLDMJwjjfoJ@JC))(@jncookJP@#RJIvdhbjnIIJCBJJ!@O((CNNEo0icjk2bfjjfbd9U@(9chne0ujndiJBSJKCp[qjnfpkH*@__!*($NCBOJUGhw";
    }
}