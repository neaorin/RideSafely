using GrovePi;
using GrovePi.Sensors;
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
    public class GroveManager
    {






        public int DistanceFromLeader { get; set; }
        public double Temperature { get; set; }
        public double Humidity { get; set; }

        


        public async Task RunAsync()
        {
            AzureConnectManagerFollower.Setup();

            var thermostat = new Thermostat() { DeviceId = Globals.LeaderDeviceId };

            var tempHumiditySensor = DeviceFactory.Build.TemperatureAndHumiditySensor(Pin.DigitalPin3, TemperatureAndHumiditySensorModel.DHT11);
            var lcdDisplay = DeviceFactory.Build.RgbLcdDisplay();
            lcdDisplay.SetBacklightRgb(0, 20, 0);

            CheckTooCloseToLeaderManager tooCloseToLeaderManager = new CheckTooCloseToLeaderManager();
            CheckFarAwayFromLeaderManager farAwayFromLeaderManager = new CheckFarAwayFromLeaderManager(this);

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

                    Temperature = thermostat.Temperature;
                    Humidity = thermostat.Humidity;

                    DistanceFromLeader = await farAwayFromLeaderManager.CheckIfWehaveLostLeaderAsync();
                    DistanceFromLeader = await tooCloseToLeaderManager.CheckIfWeAreTooCloseToLeaderAsync();

                    thermostat.Distance = DistanceFromLeader;

                    // SEND DATA TO IOT HUB

                    var serializedMessage = JsonConvert.SerializeObject(thermostat);
                    //Debug.WriteLine("Sending message " + serializedMessage);
                    var message = new Message(Encoding.UTF8.GetBytes(serializedMessage));
                    await AzureConnectManagerFollower.AzureClient.SendEventAsync(message);
                    Debug.WriteLine("Message sent.");

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






    }



    public class Thermostat
    {
        public double Temperature { get; set; }
        public double Humidity { get; set; }
        public string DeviceId { get; set; }
        public int Distance { get; set; }
    }


    static class TaskExtensions
    {
        public static void Forget(this Task task)
        {
        }
    }

}
