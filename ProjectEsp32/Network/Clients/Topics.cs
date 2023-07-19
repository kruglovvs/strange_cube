// Copyright kruglov.valentine@gmail.com KruglovVS.

using nanoFramework.M2Mqtt;
using nanoFramework.M2Mqtt.Messages;
using nanoFramework.Networking;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Network.Clients
{
    public static class Topics
    {
        public static readonly string[] Subscribed = { "/Instructions", "/Boot" };
        public static readonly string[] Publishing = { "/GameData" };
    }
}