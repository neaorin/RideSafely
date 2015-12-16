using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RideSafely.Common
{
    public interface IDistanceSensor
    {
        int GetDistanceInCentimeters();
    }

    public interface IHumiditySensor
    {
        double GetHumidityPercent();
    }

    public interface ITemperatureSensor
    {
        double GetTemperatureCelsius();
    }
}
