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

            var lcdDisplay = DeviceFactory.Build.RgbLcdDisplay();
            lcdDisplay.SetBacklightRgb(0, 20, 0);

            CheckTooCloseToLeaderManager tooCloseToLeaderManager = new CheckTooCloseToLeaderManager();
            CheckFarAwayFromLeaderManager farAwayFromLeaderManager = new CheckFarAwayFromLeaderManager(this);

            while (true)
            {
                try
                {

                    //read distance data
                    int currentDistance = DeviceFactory.Build.
                        UltraSonicSensor(Pin.DigitalPin4).MeasureInCentimeters();

                    DistanceFromLeader = await farAwayFromLeaderManager.CheckIfWehaveLostLeaderAsync(currentDistance);
                    DistanceFromLeader = await tooCloseToLeaderManager.CheckIfWeAreTooCloseToLeaderAsync(currentDistance);

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


    static class TaskExtensions
    {
        public static void Forget(this Task task)
        {
        }
    }

}
