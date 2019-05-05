using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;

namespace IoT.Hub
{
    public static class IoTHub
    {
        private static DeviceClient _deviceClient;
        private static string DeviceId = "test002";
        private static string Conn = "HostName=hubtest-iot.azure-devices.net;DeviceId=test002;SharedAccessKey=dS+/ixFZbgGgrOk2iWi/4EdcmD2M6CiS5kGvnu7fA9c=";

        public static async Task ConnectToHub()
        {
            _deviceClient = DeviceClient.CreateFromConnectionString(Conn, TransportType.Amqp_WebSocket_Only);
            await Task.CompletedTask;
        }

        public static async Task SendDeviceToCloudMessage(double temperature)
        {
            // create new telemetry message
            var telemetryDataPoint = new
            {
                time = DateTime.Now.ToString(),
                deviceId = DeviceId,
                temperature = temperature
            };

            var messageString = JsonConvert.SerializeObject(telemetryDataPoint);

            // format JSON string into IoT Hub message
            var message = new Message(Encoding.ASCII.GetBytes(messageString));

            // push message to IoT Hub
            await _deviceClient.SendEventAsync(message);
        }
    }
}