using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RideSafely.Common
{
    public interface IDeviceManager
    {
        void ChangeAlarmState(bool alarmOn);
        void DisplayMessage(string message);
        int GetDistanceFromLeader();
    }
}
