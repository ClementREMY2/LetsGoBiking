using System;
using System.ServiceModel;

namespace ProxyCacheServeur
{
    internal class Launcher
    {
        public static void Main(string[] args)
        {
            var serviceHost = new ServiceHost(typeof(ProxyCacheServeurService));
            serviceHost.Open();
            Console.WriteLine("ProxyCache started");
            Console.ReadLine(); 
        }
    }
}
