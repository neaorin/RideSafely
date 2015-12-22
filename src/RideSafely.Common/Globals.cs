using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RideSafely.Common
{
    public static class Globals
    {
        public static string LeaderDeviceId = "ridesafely-leader";
        public static string LeaderDeviceConnectionString = "";
            

        public static string FollowerDeviceId = "ridesafely-follower1";
        public static string FollowerDeviceConnectionString = "";
            


        public static string BumpOccuredMessage = "BumpOccured";
        public static string LostLeaderMessage = "LostLeader";
    }
}
