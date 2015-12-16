using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RideSafely.Common
{
    interface IWeatherService
    {
        // TODO
        double GetChanceOfRainPercent(double latitude, double longitude);
    }
}
