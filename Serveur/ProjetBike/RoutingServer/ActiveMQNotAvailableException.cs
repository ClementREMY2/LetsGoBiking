using System;

namespace RoutingServer
{
    internal class ActiveMQNotAvailableException : Exception
    {
        public ActiveMQNotAvailableException() : base("ActiveMQ is not available, verify that you launch it correctly on your laptop. We will just return the itineraries built in the method then.")
        {
        }
    }
}
