using nanoFramework.M2Mqtt.Messages;
using System;
using System.Collections;
using System.Device.Gpio;
using System.Device.I2c;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace DataNS {
    public class Data {
        public interface IData
        {

        }
        public class Network : IData
        {
            private ArrayList TopicMessagesArrayList { get; set; }
            public DictionaryEntry[] TopicMessages
            {
                get
                {
                    return (DictionaryEntry[])TopicMessagesArrayList.ToArray();
                }
            }
            public void Add(string topic, string message)
            {
                TopicMessagesArrayList.Add(new DictionaryEntry(topic, message));
            }
            // DictionaryEntry[]: topic and message strings.
        }
        public class Periphery : IData
        {

        }
    }
}
