using GrovePi;
using GrovePi.Sensors;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RideSafely.GrovePi
{
    public class GroveManager
    {
        public string DeviceId = "dxberry1";
        public string DeviceConnectionString = "";


        private int distanceThreshold = 10000;
        private TimeSpan timeThreshold = TimeSpan.FromMinutes(2);
        private bool haveLostLeader = false;
        private DateTime timeLostLeader;

        public event EventHandler LostLeader;

        public async Task RunAsync()
        {
            var thermostat = new Thermostat() { DeviceId = DeviceId };
            var random = new Random();

            DeviceClient deviceClient = DeviceClient.CreateFromConnectionString(DeviceConnectionString, TransportType.Http1);

            ReceiveCommands(deviceClient).Forget();


            var deviceFactory = DeviceFactory.Build;
            var tempHumiditySensor = deviceFactory.TemperatureAndHumiditySensor(Pin.DigitalPin4, TemperatureAndHumiditySensorModel.DHT11);
            var distanceSensor = deviceFactory.UltraSonicSensor(Pin.DigitalPin2);
            var lcdDisplay = deviceFactory.RgbLcdDisplay();
            lcdDisplay.SetBacklightRgb(0, 20, 0);

            while (true)
            {
                try
                {

                    // READ SENSOR DATA
                    var readValue = tempHumiditySensor.TemperatureAndHumidity();
                    thermostat.Temperature = readValue.Temperature;
                    thermostat.Humidity = readValue.Humidity;
                    lcdDisplay.SetText(String.Format("  {0}      {1}\n  `C       %",
                        thermostat.Temperature, thermostat.Humidity));

                    CheckIfWehaveLostleader(distanceSensor);

                    // SEND DATA TO IOT HUB

                    var serializedMessage = JsonConvert.SerializeObject(thermostat);
                    //Debug.WriteLine("Sending message " + serializedMessage);
                    var message = new Message(Encoding.UTF8.GetBytes(serializedMessage));
                    await deviceClient.SendEventAsync(message);
                    //Debug.WriteLine("Message sent.");

                    lcdDisplay.SetBacklightRgb(0, 20, 0);



                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);

                    lcdDisplay.SetBacklightRgb(20, 0, 0);

                }

                await Task.Delay(1000);
            }

        }

        private void CheckIfWehaveLostleader(IUltrasonicRangerSensor distanceSensor)
        {
            //read distance data
            int currentDistance = distanceSensor.MeasureInCentimeters();

            //if we haven't lost the leader
            if (!haveLostLeader)
            {
                //check if we've lost him
                if (currentDistance > distanceThreshold)
                {
                    haveLostLeader = true;
                    timeLostLeader = DateTime.Now;
                }
                else //we've found him again
                {
                    haveLostLeader = false;
                }
            }
            else //we've lost the leader
            {
                //if we have lost him for over two minutes
                if (DateTime.Now - timeLostLeader > timeThreshold)
                {
                    //raise the lost event and begin the search again
                    LostLeader?.Invoke(this, EventArgs.Empty);
                    haveLostLeader = false;
                }
            }
        }

        static async Task ReceiveCommands(DeviceClient deviceClient)
        {
            Debug.WriteLine("\nDevice waiting for commands from IoTHub...\n");
            Message receivedMessage;
            string messageData;

            while (true)
            {
                receivedMessage = await deviceClient.ReceiveAsync();

                if (receivedMessage != null)
                {
                    messageData = Encoding.ASCII.GetString(receivedMessage.GetBytes());
                    Debug.WriteLine("\t{0}> Received message: {1}", DateTime.Now.ToLocalTime(), messageData);

                    await deviceClient.CompleteAsync(receivedMessage);
                }

                await Task.Delay(500);
            }
        }
    }



    public class Thermostat
    {
        public double Temperature { get; set; }
        public double Humidity { get; set; }
        public string DeviceId { get; set; }
    }


    static class TaskExtensions
    {
        public static void Forget(this Task task)
        {
        }
    }

}
