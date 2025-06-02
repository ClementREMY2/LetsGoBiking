using Apache.NMS;
using Apache.NMS.ActiveMQ;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RoutingServer
{
    //this class is used to send messages via ActiveMQ
    public class ActiveMQ
    {
        private Uri connecturi = new Uri("activemq:tcp://localhost:61616");
        private ConnectionFactory factory;
        private IConnection connection;

        public ActiveMQ()
        {
            factory = new ConnectionFactory(connecturi);
            Console.WriteLine("About to connect to " + connecturi);
            try
            {
                connection = factory.CreateConnection();
                connection.Start();
            }
            catch (Exception)
            {
                throw new ActiveMQNotAvailableException();
            }
            
        }

        public void Send(string queueName, List<string> messages)
        {
            ISession session = connection.CreateSession();

            IDestination destination = session.GetQueue(queueName);

            IMessageProducer producer = session.CreateProducer(destination);

            producer.DeliveryMode = MsgDeliveryMode.NonPersistent;

            foreach (var m in messages.Select(message => session.CreateTextMessage(message)))
            {
                producer.Send(m);
            }
            session.Close();
            connection.Close();
        }

    }
}