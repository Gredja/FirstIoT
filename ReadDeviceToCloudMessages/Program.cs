using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace ReadDeviceToCloudMessages
{
    internal class Program
    {
        private const string ConnectionString = "HostName=GredjaIoT.azure-devices.net;SharedAccessKeyName=GredjaIoT.azure-devices.net;SharedAccessKey=UlU/IIEXHNyk0Spz9bCw2ESyss2CjfR+ZhjGHEv4+78=";

        private static string _iotHubD2CEndpoint = "messages/events";
        private static EventHubClient _eventHubClient;

        private static void Main(string[] args)
        {
            Console.WriteLine("Receive messages. Ctrl-C to exit.\n");
            _eventHubClient = EventHubClient.CreateFromConnectionString(ConnectionString, _iotHubD2CEndpoint);

            var d2CPartitions = _eventHubClient.GetRuntimeInformation().PartitionIds;

            var cts = new CancellationTokenSource();

            Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
                Console.WriteLine("Exiting...");
            };

            var tasks = new List<Task>();

            foreach (var partition in d2CPartitions)
            {
                tasks.Add(ReceiveMessagesFromDeviceAsync(partition, cts.Token));
            }

            Task.WaitAll(tasks.ToArray());
        }

        private static async Task ReceiveMessagesFromDeviceAsync(string partition, CancellationToken ct)
        {
            var eventHubReceiver = _eventHubClient.GetDefaultConsumerGroup().CreateReceiver(partition, DateTime.UtcNow);

            while (true)
            {
                if (ct.IsCancellationRequested) break;
                EventData eventData = await eventHubReceiver.ReceiveAsync();
                if (eventData == null) continue;

                string data = Encoding.UTF8.GetString(eventData.GetBytes());
                Console.WriteLine("Message received. Partition: {0} Data: '{1}'", partition, data);
            }
        }

    }
}
