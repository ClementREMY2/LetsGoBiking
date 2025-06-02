using System;
using System.ServiceModel;

namespace RoutingServer
{
    internal class Launcher
    {
        public static void Main(string[] args)
        {
            var serviceHost = new ServiceHost(typeof(RS_Service));
            serviceHost.Open();
            Console.WriteLine("RoutingServer started");
            Console.ReadLine();
        }
    }
}
