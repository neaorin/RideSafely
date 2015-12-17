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
    public class CheckFarAwayFromLeaderManager
    {
        private int distanceThresholdForLosingLeader = 50;
        private TimeSpan timeThresholdForLosingLeader = TimeSpan.FromSeconds(5);
        private bool haveLostLeader = false;
        private DateTime timeLostLeader;

        public CheckFarAwayFromLeaderManager(GroveManager gm)
        {
            groveManager = gm;
        }

        private GroveManager groveManager { get; set; }

        public async Task<int> CheckIfWehaveLostLeaderAsync()
        {
            //read distance data
            int currentDistance = await DeviceFactory.Build.
                UltraSonicSensor(Pin.DigitalPin4).MeasureInCentimetersAsync();
            
            Debug.WriteLine($"distance is {currentDistance}");
            //if we haven't lost the leader
            if (!haveLostLeader)
            {
                //check if we've lost him
                if (currentDistance > distanceThresholdForLosingLeader)
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
                //if we have lost him for over the threshold
                if (DateTime.Now - timeLostLeader > timeThresholdForLosingLeader)
                {
                    //raise the lost event and begin the search again
                    Debug.WriteLine("Lost leader");

                    DeviceMessage dm = new DeviceMessage()
                    {
                        DeviceId = "Follower",
                        Message = Globals.LostLeaderMessage
                    };

                    var serializedMessage = JsonConvert.SerializeObject(dm);
                    //Debug.WriteLine("Sending message " + serializedMessage);
                    var message = new Message(Encoding.UTF8.GetBytes(serializedMessage));
                    await AzureConnectManagerFollower.AzureClient.SendEventAsync(message);
                    haveLostLeader = false;
                }
            }
            return currentDistance;
        }
    }
}
