using RideSafely.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RideSafely.DeviceApp.Managers
{
    public class SimulatedDeviceManager : IDeviceManager
    {
        private Random random;
        public SimulatedDeviceManager()
        {
            this.random = new Random();
        }
        public void ChangeAlarmState(bool alarmOn)
        {            
        }

        public void DisplayMessage(string message)
        {           
        }

        public int GetDistanceFromLeader()
        {
            return random.Next(1000);
        }
    }
}
