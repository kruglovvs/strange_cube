// copyright kruglovvs kruglov.valentine@gmail.com
// this code is made by hardware team of the Vulcan company

using NetworkNS;
using PeripheryNS;
using System.Diagnostics;
using System.Threading;

namespace ProjectESP32
{
    public class Program
    {
        public static void Main()
        {
            NetworkController.Got += (sender, e) => { Debug.WriteLine($"Topic: {e.Topic}, Message: {e.Message}"); };
            while (true)
            {
                NetworkController.Send("/GameData", "Lolkek");
                Thread.Sleep(10000);
            }
        }
    }
}