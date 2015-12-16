using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RideSafely.Common
{
    interface IDisplayInterface
    {
        void DisplayMessage(string message);
    }

    interface ISoundInterface
    {
        void PlayNotificationSound();
    }
}
