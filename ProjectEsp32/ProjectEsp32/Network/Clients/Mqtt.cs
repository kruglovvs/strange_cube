// Copyright kruglov.valentine@gmail.com KruglovVS.

using nanoFramework.M2Mqtt;
using nanoFramework.M2Mqtt.Messages;
using Network.Constants;
using System;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Network.Clients
{
    public class Mqtt : MqttClient, IClient, IConnectable
    {
        private string _clientID { get; init; }
        private string _username { get; init; }
        private string _password { get; init; }

        internal Mqtt(string brokerHostName, int brokerPort, bool secure, X509Certificate caCert,
            X509Certificate clientCert, MqttSslProtocols sslProtocol,
            string clientID, string username, string password) :
            base(brokerHostName, brokerPort, secure, caCert, clientCert, sslProtocol)
        {
            _clientID = clientID;
            _username = username;
            _password = password;
            MqttMsgPublishReceived += (sender, e) => { Got?.Invoke(sender, new IClient.GotMessageEventArgs(e.Topic, e.Message)); };
        }

        public bool Publish(string topic, string message)
        {
            Debug.WriteLine("Publishing");
            Debug.WriteLine($"id publish: {Publish(topic, Encoding.UTF8.GetBytes(message), null, null, MqttQoSLevel.ExactlyOnce, false)}"); // should be changed
            Debug.WriteLine("Published string");
            return true;
        }
        public new bool Publish(string topic, byte[] message)
        {
            Debug.WriteLine("Publishing");
            Debug.WriteLine($"id publish: {Publish(topic, message, null, null, MqttQoSLevel.ExactlyOnce, false)}"); // should be changed
            Debug.WriteLine("Published bytes");
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
        public bool Connect()
        {
            try
            {
                Debug.WriteLine("Connecting MQTT");
                Debug.WriteLine($"connected: {Connect(_clientID, _username, _password) == MqttReasonCode.Success}");
                Debug.WriteLine("Gotcha!");
            } catch
            {
                Debug.WriteLine("Badcha!");
                return false;
            }
            return IsConnected;
        }
        public bool Reconnect()
        {
            if (IsConnected)
            {
                Disconnect();
            }
            try
            {
                var connected = Connect(_clientID, _username, _password);
                if (connected == MqttReasonCode.Success)
                {
                    Subscribe("/Instructions");
                    Subscribe("/BootData");
                }
                return connected == MqttReasonCode.Success;
            } catch 
            { 
                return false; 
            }
        }

        public event IClient.GotMessageEventHandler Got;
    }
}