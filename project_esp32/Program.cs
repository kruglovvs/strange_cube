// copyright kruglovvs kruglov.valentine@gmail.com
// this code is made by hardware team of the Vulcan company
using System;
using System.Diagnostics;
using System.Threading;
using DataNS;
using NetworkNS;
using PeripheryNS;

namespace ProjectESP32
{
    public class Program
    {
        private static PeripheryController _peripheryController = new PeripheryController();
        private static NetworkController _networkController = new NetworkController();
        private static Data _data = new Data();
        public static void Main()
        {
            Debug.WriteLine(_peripheryController.ToString());
            Debug.WriteLine(_networkController.ToString());
            Debug.WriteLine(_data.ToString());
            while (true)
            {
                Debug.WriteLine((_data = _peripheryController.Data).ToString());
                Debug.WriteLine(_networkController.Send(_data).ToString());
            }

            // Browse our samples repository: https://github.com/nanoframework/samples
            // Check our documentation online: https://docs.nanoframework.net/
            // Join our lively Discord community: https://discord.gg/gCyBu8T
        }
    }
}