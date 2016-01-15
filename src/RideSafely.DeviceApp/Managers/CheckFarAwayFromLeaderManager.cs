using RideSafely.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RideSafely.DeviceApp.Managers
{
    public class CheckFarAwayFromLeaderManager
    {
        private int distanceThresholdForLosingLeader = 10;
        private TimeSpan timeThresholdForLosingLeader = TimeSpan.FromSeconds(5);
        private bool haveLostLeader = false;
        private DateTime timeLostLeader;

        public CheckFarAwayFromLeaderManager()
        {
        }

        public FarAwayFromLeaderStatus CheckIfWeHaveLostLeader(int currentDistance)
        {
            var status = new FarAwayFromLeaderStatus() { NewAlarmState = null, SendMessage = false };
            
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
                    status.NewAlarmState = false;
                }
            }
            else //we've lost the leader
            {
                //if we have lost him for over the threshold
                if (DateTime.Now - timeLostLeader > timeThresholdForLosingLeader)
                {
                    //raise the lost event and begin the search again
                    Debug.WriteLine("Lost leader");
                    status.SendMessage = true;
                    status.NewAlarmState = true;
                    haveLostLeader = false;

                }
            }
            return status;
        }
    }

    public class FarAwayFromLeaderStatus
    {
        public bool? NewAlarmState { get; set; }
        public bool SendMessage { get; set; }
    }
}
