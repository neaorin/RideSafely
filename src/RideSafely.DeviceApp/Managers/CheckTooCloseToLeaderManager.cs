﻿using RideSafely.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RideSafely.DeviceApp.Managers
{
    public class CheckTooCloseToLeaderManager
    {

        private int distanceThresholdForTooCloseToLeader = 3; //centimeters
        private TimeSpan timeThresholdForTooCloseToLeader = TimeSpan.FromSeconds(5);
        private bool tooCloseToLeader = false;
        private DateTime timeTooCloseToLeader;

        public CheckTooCloseToLeaderManager()
        {
        }

        public TooCloseToLeaderStatus CheckIfWeAreTooCloseToLeader(int currentDistance)
        {
            var status = new TooCloseToLeaderStatus() { NewAlarmState = null };
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
                    status.NewAlarmState = false;
                }
            }
            else //we are too close to leader
            {
                //if we are too close to him for over two minutes
                if (DateTime.Now - timeTooCloseToLeader > timeThresholdForTooCloseToLeader)
                {
                    status.NewAlarmState = true;
                    tooCloseToLeader = false;
                }
            }

            return status;
        }
    }

    public class TooCloseToLeaderStatus
    {
        public bool? NewAlarmState { get; set; }
    }
}
