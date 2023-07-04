using System.Device.Gpio;
using System.Device.Wifi;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Security;
using System.Net.Sockets;
using System;
using System.Text;

namespace NetworkNS
{
    public class ServerTCP
    {
        static void ServerTcp(string[] args)
        {
            const string ip = "127.0.0.1";
            const int port = 8080;

            var tcpEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            /*            var probe = System.Net.IPEndPoint.ReferenceEquals(IPAddress.Parse(ip), port);
            */
            var tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            tcpSocket.Bind(tcpEndPoint);
            tcpSocket.Listen(2);

            while (true)
            {
                var listener = tcpSocket.Accept();
                var buffer = new byte[256];
                var size = 0;
                var data = new StringBuilder();

                do
                {
                    size = listener.Receive(buffer);
                    data.Append(Encoding.UTF8.GetString(buffer, 0, size));
                }
                while (listener.Available > 0);


                listener.Send(Encoding.UTF8.GetBytes("Cool!"));
                //shutdown???
                listener.Close();
            }
        }
    }
}
