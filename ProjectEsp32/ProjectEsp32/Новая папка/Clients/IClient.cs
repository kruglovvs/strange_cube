// Copyright kruglov.valentine@gmail.com KruglovVS.

using System;

namespace Network.Clients
{
    public interface IClient
    {
        public bool Publish(string topic, string message);
        public void Subscribe(string topic);
        public void Unsubscribe(string topic);
        public class GotMessageEventArgs
        {
            public string Topic;
            public string Message;
            public GotMessageEventArgs(string topic, string message)
            {
                Topic = topic;
                Message = message;
            }
        }
        public event EventHandler<GotMessageEventArgs> Got;
    }
}
