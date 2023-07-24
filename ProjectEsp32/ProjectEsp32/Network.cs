using nanoFramework.Hardware.Esp32;
using nanoFramework.M2Mqtt;
using nanoFramework.M2Mqtt.Messages;
using nanoFramework.Networking;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace ProjectESP32
{
    public static class Network
    {
        private static MqttClient s_mqtt { get; set; }
        public static DelegateGotInstructions GotInstructions { get; set; }
        public static void TurnOn()
        {
            Debug.WriteLine("Start Network");
            WifiNetworkHelper.ConnectDhcp("Guest_WiFi", "NeRm25:)KmwQ");
            Debug.WriteLine("Connected wifi");
            s_mqtt = new MqttClient("10.0.41.64", 1883, false, null, null, MqttSslProtocols.None);
            Debug.WriteLine("opened mqtt");
            try
            {
                s_mqtt.Connect("Esp32");
            }
            catch
            {
                Debug.WriteLine("Can't Connect to the mqtt broker");
            }
            Debug.WriteLine("connected mqtt");
            s_mqtt.Subscribe(new string[] { "/Instructions", "/BootData" }, new MqttQoSLevel[] { MqttQoSLevel.ExactlyOnce, MqttQoSLevel.ExactlyOnce });
            Debug.WriteLine("subscribed mqtt");
            s_mqtt.MqttMsgPublishReceived += (sender, e) =>
            {
                Debug.WriteLine($"Got data: {e.Topic} : {Encoding.UTF8.GetString(e.Message, 0, e.Message.Length)}");
                switch (e.Topic)
                {
                    case "/BootData":
                        MemoryStream stream = new MemoryStream(e.Message);
                        stream.Flush();
                        stream.Close();
                        break;
                    case "/Instructions":
                        GotInstructions?.Invoke(e.Message);
                        break;
                }
            };
            Debug.WriteLine("Made event mqtt");
            s_mqtt.ConnectionClosed += (sender, e) => { 
                while (!s_mqtt.IsConnected) { 
                    try { 
                        s_mqtt.Connect("Esp32"); 
                    } 
                    catch { 
                        Thread.Sleep(5000); 
                    } 
                } 
            };
        }
        public static void Publish(string topic, string message)
        {
            try
            {
                s_mqtt.Publish(topic, Encoding.UTF8.GetBytes(message), null, null, MqttQoSLevel.ExactlyOnce, false);
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
                s_mqtt.Publish(topic, message, null, null, MqttQoSLevel.ExactlyOnce, false);
            }
            catch
            {
                Debug.WriteLine("Can't publish message");
            }
        }
        public delegate void DelegateGotInstructions(byte[] message);
    }
}
