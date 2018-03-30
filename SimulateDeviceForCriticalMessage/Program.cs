using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;

namespace SimulateDeviceForCriticalMessage
{
    internal static class Program
    {
        private static DeviceClient _deviceClient;
        private const string IotHubUri = "GredjaIoT.azure-devices.net";
        private const string DeviceKey = "amZG8GutOaDyT7PlqTrD1zdMipAf16h00a2gDsuM0Yc=";

        private static void Main(string[] args)
        {
            Console.WriteLine("Simulated device\n");
            _deviceClient = DeviceClient.Create(IotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey("myFirstDevice", DeviceKey), TransportType.Mqtt);
            _deviceClient.ProductInfo = "HappyPath_Simulated-CSharp";
            SendDeviceToCloudMessagesAsync();
            Console.ReadLine();
        }

        private static async void SendDeviceToCloudMessagesAsync()
        {
            const double minTemperature = 20;
            const double minHumidity = 60;

            var rand = new Random();

            while (true)
            {
                var currentTemperature = minTemperature + rand.NextDouble() * 15;
                var currentHumidity = minHumidity + rand.NextDouble() * 20;

                var telemetryDataPoint = new
                {
                    deviceId = "myFirstDevice",
                    temperature = currentTemperature,
                    humidity = currentHumidity
                };

                var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
                string levelValue;

                if (rand.NextDouble() > 0.7)
                {
                    if (rand.NextDouble() > 0.5)
                    {
                        messageString = "This is a critical message";
                        levelValue = "critical";
                    }
                    else
                    {
                        messageString = "This is a storage message";
                        levelValue = "storage";
                    }
                }
                else
                {
                    levelValue = "normal";
                }

                var message = new Message(Encoding.ASCII.GetBytes(messageString));
                message.Properties.Add("level", levelValue);

                await _deviceClient.SendEventAsync(message);
                Console.WriteLine("{0} > Sent message: {1}", DateTime.Now, messageString);

                await Task.Delay(1000);
            }
        }
    }
}
