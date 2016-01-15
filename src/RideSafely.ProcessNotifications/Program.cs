using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.ServiceBus.Messaging;
using System.Configuration;

namespace RideSafely.ProcessNotifications
{

    class Program
    {
        static void Main()
        {
            //var host = new JobHost();

            //host.RunAndBlock();
            Setup();
        }

        static void Setup()
        {
            string iotHubConnectionString = ConfigurationManager.ConnectionStrings["AzureIoTHub"].ConnectionString;
            string hubName = iotHubConnectionString.Substring(iotHubConnectionString.IndexOf('=') + 1, 
                iotHubConnectionString.IndexOf('.') - iotHubConnectionString.IndexOf('=') - 1).ToLower();
            string storageConnectionString = ConfigurationManager.ConnectionStrings["AzureWebJobsStorage"].ConnectionString;

            var eventHubClient = EventHubClient.CreateFromConnectionString(iotHubConnectionString, hubName);
            var eventProcessorHostName = Guid.NewGuid().ToString();
            var eventProcessorHost = new EventProcessorHost(
              eventProcessorHostName,
              hubName,
              EventHubConsumerGroup.DefaultGroupName,
              iotHubConnectionString,
              storageConnectionString);

            var epo = new EventProcessorOptions
            {
                MaxBatchSize = 100,
                PrefetchCount = 10,
                ReceiveTimeOut = TimeSpan.FromSeconds(20),
                InitialOffsetProvider = (name) => DateTime.Now.AddDays(-7),
            };

            epo.ExceptionReceived += OnExceptionReceived;

            eventProcessorHost.RegisterEventProcessorAsync<RideSafelyEventProcessor>(epo).Wait();

            Console.WriteLine("Receiving.  Please enter to stop worker.");
            Console.ReadLine();
            eventProcessorHost.UnregisterEventProcessorAsync().Wait();
        }


        public static void OnExceptionReceived(object sender, ExceptionReceivedEventArgs args)
        {
            Console.WriteLine("Event Hub exception received: {0}", args.Exception.Message);
        }

    }
}
