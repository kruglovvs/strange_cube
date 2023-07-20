using nanoFramework.M2Mqtt;
using nanoFramework.M2Mqtt.Messages;
using nanoFramework.Networking;
using System.IO;
using System.Text;

namespace ProjectESP32 {
    public static class Network {
        private static MqttClient s_mqtt { get; set; }

        public static DelegateGotInstructions GotInstructions { get; set; }

        public static void TurnOn() 
        {
            WifiNetworkHelper.ConnectDhcp("Guest_WiFi", "NeRm25:)KmwQ");
            s_mqtt = new MqttClient("10.0.41.59", 1883, false, null, null, MqttSslProtocols.None);
            s_mqtt.Connect("Esp32");
            s_mqtt.Subscribe(new string[] { "/Instructions", "/BootData" }, new MqttQoSLevel[] { MqttQoSLevel.ExactlyOnce, MqttQoSLevel.ExactlyOnce });
            s_mqtt.MqttMsgPublishReceived += (sender, e) => {
                switch (e.Topic) {
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
        }
        public static void Publish (string topic, string message) {
            s_mqtt.Publish(topic, Encoding.UTF8.GetBytes(message), null, null, MqttQoSLevel.ExactlyOnce, false);
        }

        public delegate void DelegateGotInstructions(byte[] message);
    }
}
