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

using nanoFramework.Networking;

namespace NetworkNS
{
    public class NetworkController
    {
        private static ClientTcp clientTcp = new ClientTcp();
        private static Wifi wifi = new Wifi();
        public static class Constants
        {
            public static readonly string SSID = "Guest_WiFi";
            public static readonly string Password = "NeRm25:)KmwQ";
            public static class TCP
            {
                public static readonly string Ip = "127.0.0.1";
                public const int Port = 8080;
            }
        }
        static NetworkController() {
        }
        public NetworkController()
        {

        }
        public int Send(string message)
        {
            return clientTcp.Send(Encoding.UTF8.GetBytes(message));
        }
        public string Read()
        {
            byte[] buffer = new byte[256];
            int size;
            string answer = "";

            do
            {

                size = clientTcp.Receive(buffer);
                answer += Encoding.UTF8.GetString(buffer, 0, size);
            }
            while (clientTcp.Available > 0);
            return answer;
        }
        private class ClientTcp : Socket
        {
            private IPEndPoint _tcpEndPoint;
            public ClientTcp() : base(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            {
                _tcpEndPoint = new IPEndPoint(IPAddress.Parse(Constants.TCP.Ip), Constants.TCP.Port);
                Connect(_tcpEndPoint);
            }
            ~ClientTcp()
            {
                Close();
            }
        }
        private class Wifi
        {
            static Wifi()
            {
                CancellationTokenSource cs = new(6000);
                var success = WifiNetworkHelper.ConnectDhcp(Constants.SSID, Constants.Password, requiresDateTime: true, token: cs.Token);
                if (!success)
                {
                    // Something went wrong, you can get details with the ConnectionError property:
                    Debug.WriteLine($"Can't connect to the network, error: {WifiNetworkHelper.Status}");
                    if (WifiNetworkHelper.HelperException != null)
                    {
                        Debug.WriteLine($"ex: {WifiNetworkHelper.HelperException}");
                    }
                }
            }
        }
    } 
}

