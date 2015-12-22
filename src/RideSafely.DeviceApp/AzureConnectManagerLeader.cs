using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using RideSafely.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RideSafely.DeviceApp
{
    public static class AzureConnectManagerLeader
    {
        public static DeviceClient AzureClient { get; set; }

        public static void Setup()
        {

            var random = new Random();

            AzureClient = DeviceClient.CreateFromConnectionString(Globals.LeaderDeviceConnectionString,
                TransportType.Http1);

            
        }

        public static async Task SendBumpMessageAsync()
        {
            DeviceMessage dm = new DeviceMessage()
            {
                DeviceId = Globals.LeaderDeviceId,
                Message = Globals.BumpOccuredMessage + " on " + DateTime.Now 
            };
            var serializedMessage = JsonConvert.SerializeObject(dm);
            //Debug.WriteLine("Sending message " + serializedMessage);
            var message = new Message(Encoding.UTF8.GetBytes(serializedMessage));
            await AzureClient.SendEventAsync(message);
            Debug.WriteLine("Message sent.");
        }
    }
}
