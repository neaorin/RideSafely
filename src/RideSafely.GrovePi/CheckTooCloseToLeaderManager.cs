using GrovePi;
using GrovePi.Sensors;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RideSafely.GrovePi
{
    public class CheckTooCloseToLeaderManager
    {
        private int distanceThresholdForTooCloseToLeader = 3; //centimeters
        private TimeSpan timeThresholdForTooCloseToLeader = TimeSpan.FromSeconds(5);
        private bool tooCloseToLeader = false;
        private DateTime timeTooCloseToLeader;

        public async Task<int> CheckIfWeAreTooCloseToLeaderAsync(int currentDistance)
        {
            
            
            Debug.WriteLine($"distance is {currentDistance}");
            //if we aren't too close to the leader
            if (!tooCloseToLeader)
            {
                //check if are too close to him
                if (currentDistance < distanceThresholdForTooCloseToLeader)
                {
                    tooCloseToLeader = true;
                    timeTooCloseToLeader = DateTime.Now;
                }
                else //we are not too close to him
                {
                    tooCloseToLeader = false;
                    DeviceFactory.Build.Buzzer(Pin.DigitalPin7).ChangeState(SensorStatus.Off);
                }
            }
            else //we are too close to leader
            {
                //if we are too close to him for over two minutes
                if (DateTime.Now - timeTooCloseToLeader > timeThresholdForTooCloseToLeader)
                {
                    //play sound
                    DeviceFactory.Build.Buzzer(Pin.DigitalPin7).ChangeState(SensorStatus.On);
                    tooCloseToLeader = false;
                }
            }

            return currentDistance;
        }
    }
}
