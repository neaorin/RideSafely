using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.Devices;
using Newtonsoft.Json;

namespace RideSafely.ProcessNotifications
{
    public class Functions
    {
        static string _iotHubConnectionString = "HostName=RideSafely-Hub.azure-devices.net;SharedAccessKeyName=service;SharedAccessKey=IXvNW2hQ9LsiU1yhzNNKi+JGAYRwM5iOXca7E/CPeuU=";

        // This function will get triggered/executed when a new message is written 
        // on an Azure Queue called ridesafelyqueue.
        public static async void ProcessQueueMessage([QueueTrigger("ridesafelyqueue")] string message, TextWriter log)
        {
            log.WriteLine("{0}: Processing message: {1}", DateTime.Now.ToShortTimeString(), message);

            try
            {
                var deviceMessage = JsonConvert.DeserializeObject<DeviceMessage>(message);
                var receiverDeviceId = (deviceMessage.DeviceId == "ridesafely-leader" ? "ridesafely-follower1" : "ridesafely-leader");
                await SendCommand(receiverDeviceId, deviceMessage);

                log.WriteLine("{0}: Sent command to {1}: {2}", DateTime.Now.ToShortTimeString(), receiverDeviceId, message);
            }
            catch (Exception ex)
            {
                log.WriteLine(ex.ToString());
                //throw;
            }
        }

        /// <summary>
        /// Sends a fire and forget command to the device
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        public static async Task SendCommand(string deviceId, dynamic command)
        {
            ServiceClient serviceClient = ServiceClient.CreateFromConnectionString(_iotHubConnectionString);

            byte[] commandAsBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(command));
            var notificationMessage = new Message(commandAsBytes);

            notificationMessage.Ack = DeliveryAcknowledgement.None;
            notificationMessage.MessageId = command.Message;

            await serviceClient.SendAsync(deviceId, notificationMessage);

            await serviceClient.CloseAsync();
        }
    }

    public class DeviceMessage
    {
        public string DeviceId { get; set; }
        public string Message { get; set; }
    }
}
