using System;
using System.IO;
using System.Text;
using Microsoft.ServiceBus.Messaging;

namespace ReadCriticalQueue
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            const string connectionString = "Endpoint=sb://gredjaservicebus.servicebus.windows.net/;SharedAccessKeyName=gredja;SharedAccessKey=R9jFhWqUCAKX/4G+uuPZxI9BlcXcYoZX2ngFFXENzXQ=";
            const string queueName = "gredja";

            Console.WriteLine("Receive critical messages. Ctrl-C to exit.\n");
            var client = QueueClient.CreateFromConnectionString(connectionString, queueName);

            client.OnMessage(message =>
            {
                var stream = message.GetBody<Stream>();
                var reader = new StreamReader(stream, Encoding.ASCII);
                var s = reader.ReadToEnd();
                Console.WriteLine("Message body: {0}", s);
            });

            Console.ReadLine();
        }
    }
}
