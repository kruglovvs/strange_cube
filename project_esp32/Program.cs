using System;
using System.Diagnostics;
using System.Threading;
using Network;
using Periphery;

namespace project_esp32
{
    public class Program
    {
        private Periphery_controller periphery_controller = new Periphery_controller();
        private Network_controller network_controller = new Network_controller();
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