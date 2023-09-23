using nanoFramework.Hardware.Esp32;
using nanoFramework.M2Mqtt;
using nanoFramework.M2Mqtt.Messages;
using nanoFramework.Networking;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using Network;
using Network.Clients;

namespace Network
{
    public static class NetworkController
    {
        private static Connections.Wifi s_wifi { get; set; } = new Connections.Wifi(Constants.Wifi.SSID, Constants.Wifi.Password);
        private static Clients.Mqtt s_mqtt { get; set; } = new Clients.Mqtt(Constants.Mqtt.Ip, Constants.Mqtt.Port, Constants.Mqtt.UsingSecure,
            Constants.Mqtt.CaCert, Constants.Mqtt.ClientCert, Constants.Mqtt.MqttSslProtocol,
            Constants.Mqtt.ClientID, Constants.Mqtt.Username, Constants.Mqtt.Password);
        public static void TurnOn()
        {
            Debug.WriteLine("connecting wifi");
            s_wifi.Connect();
            Debug.WriteLine("connecting mqtt");
            s_mqtt.Connect();
            Debug.WriteLine("subscribing");
            s_mqtt.Subscribe("/Instructions");
            s_mqtt.Subscribe("/BootData");
            Debug.WriteLine("making events");
            s_mqtt.Got += (sender, e) => { GotMessage?.Invoke(sender, e); };
            Debug.WriteLine("network turned on");
            s_mqtt.ConnectionClosed += (sender, e) =>
            {
                while (!s_mqtt.IsConnected)
                {
                    try
                    {
                        s_mqtt.Reconnect();
                        Debug.WriteLine("Reconnecting");
                    }
                    catch
                    {
                        Debug.WriteLine("Can't reconnect");
                    }
                }
                Debug.WriteLine("Reconnected MQTT");
            };
        }

        public static void Subscribe(string topic)
        {
            s_mqtt.Subscribe(topic);
        }
        public static void Unsubscribe(string topic)
        {
            s_mqtt.Unsubscribe(topic);
        }
        public static void Publish(string topic, string message)
        {
            try
            {
                Debug.WriteLine($"Mqtt publish string: {topic} - {message}");
                s_mqtt.Publish(topic, message);
            }
            catch
            {
                Debug.WriteLine("Can't publish message");
            }
        }
        public static void Publish(string topic, byte[] message)
        {
            try
            {
                Debug.WriteLine("Mqtt publish byte");
                s_mqtt.Publish(topic, message);
            }
            catch
            {
                Debug.WriteLine("Can't publish message");
            }
        }

        public static event IClient.GotMessageEventHandler GotMessage;
    }
}
