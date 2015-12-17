using GrovePi;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using RideSafely.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RideSafely.GrovePi
{
    public static class AzureConnectManagerFollower
    {

        
        public static DeviceClient AzureClient { get; set; }

        public static void Setup()
        {
            
            var random = new Random();

            AzureClient = DeviceClient.CreateFromConnectionString(Globals.FollowerDeviceConnectionString, 
                TransportType.Http1);

            ReceiveCommands(AzureClient).Forget();
        }

        public static async Task ReceiveCommands(DeviceClient deviceClient)
        {
            Debug.WriteLine("\nDevice waiting for commands from IoTHub...\n");
            Message receivedMessage;
            string messageData;

            while (true)
            {
                receivedMessage = await deviceClient.ReceiveAsync();

                if (receivedMessage != null)
                {
                    messageData = Encoding.UTF8.GetString(receivedMessage.GetBytes());

                    DeviceMessage dm = JsonConvert.DeserializeObject<DeviceMessage>(messageData);

                    if(dm.Message.Contains(Globals.BumpOccuredMessage))
                    {
                        DeviceFactory.Build.RgbLcdDisplay().SetText(Globals.BumpOccuredMessage);
                    }

                    await deviceClient.CompleteAsync(receivedMessage);
                }

                await Task.Delay(500);
            }
        }

    }
}
