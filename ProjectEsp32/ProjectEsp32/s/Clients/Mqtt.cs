// Copyright kruglov.valentine@gmail.com KruglovVS.

using nanoFramework.M2Mqtt;
using nanoFramework.M2Mqtt.Messages;
using System;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Network.Clients
{
    public class Mqtt : MqttClient, IClient, IConnectable
    {
        private string _clientID { get; set; }
        private string _username { get; set; }
        private string _password { get; set; }
        internal Mqtt(string brokerHostName, int brokerPort, bool secure, X509Certificate caCert,
            X509Certificate clientCert, MqttSslProtocols sslProtocol,
            string clientID, string username, string password) :
            base(brokerHostName, brokerPort, secure, caCert, clientCert, sslProtocol)
        {
            _clientID = clientID;
            _username = username;
            _password = password;
            //ProtocolVersion = MqttProtocolVersion.Version_5;
            MqttMsgPublishReceived += (sender, e) => { Got.Invoke(sender, new IClient.GotMessageEventArgs(e.Topic, Encoding.UTF8.GetString(e.Message, 0, e.Message.Length))); };
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

        public event IClient.GotMessageEventHandler Got;
    }
}