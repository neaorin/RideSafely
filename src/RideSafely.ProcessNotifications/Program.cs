using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.ServiceBus.Messaging;

namespace RideSafely.ProcessNotifications
{
    // To learn more about Microsoft Azure WebJobs SDK, please see http://go.microsoft.com/fwlink/?LinkID=320976
    class Program
    {
        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage
        static void Main()
        {
            //var host = new JobHost();
            //// The following code ensures that the WebJob will be running continuously
            //host.RunAndBlock();

            var queueProc = new QueueProcessing();

            while (true)
            {

            }
        }
    }

    class QueueProcessing
    {
        string _serviceBusConnectionString = "Endpoint=sb://ridesafely.servicebus.windows.net/;SharedAccessKeyName=Owner;SharedAccessKey=a5W7GFhUOoDxCbmEu7y3iB5ds8gBNtspqtRWM4vEiZs=";
        string _queueName = "ridesafelyqueue";
         
        public QueueProcessing()
        {
            QueueClient Client =
              QueueClient.CreateFromConnectionString(_serviceBusConnectionString, _queueName);

            // Configure the callback options.
            OnMessageOptions options = new OnMessageOptions();
            options.AutoComplete = false;
            options.AutoRenewTimeout = TimeSpan.FromMinutes(1);

            // Callback to handle received messages.
            Client.OnMessage((message) =>
            {
                try
                {
                    Functions.ProcessQueueMessage(message.GetBody<string>(), Console.Out);
                    message.Complete();
                }
                catch (Exception)
                {
                    // Indicates a problem, unlock message in queue.
                    message.Abandon();
                }
            }, options);
        }
    }
}
