using System;
using System.Diagnostics;
using System.Threading;
using NetworkNS;
using PeripheryNS;

namespace project_esp32
{
    public class Program
    {
        private PeripheryController _peripheryController = new PeripheryController();
        private NetworkController _networkController = new NetworkController();
        public static void Main()
        {

            Debug.WriteLine("Hello from nanoFramework!");

            Thread.Sleep(Timeout.Infinite);

            // Browse our samples repository: https://github.com/nanoframework/samples
            // Check our documentation online: https://docs.nanoframework.net/
            // Join our lively Discord community: https://discord.gg/gCyBu8T
        }
    }
}