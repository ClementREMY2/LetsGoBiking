using System;

namespace RoutingServer
{
    internal class NoPlaceFoundException : Exception
    {
        public NoPlaceFoundException(string message) : base(message)
        {
        }
    }
}
