using GrovePi;
using GrovePi.I2CDevices;
using GrovePi.Sensors;
using RideSafely.Common;

namespace RideSafely.GrovePi
{
    public class GroveManager : IDeviceManager
    {
        // GrovePi Pin Configuration - configure these to match the sensors you plugged into GrovePi+
        // you can connect the LCD display to any I2C port.
        private static Pin LedPin = Pin.DigitalPin8;
        private static Pin UltraSonicSensorPin = Pin.DigitalPin4;

        private IRgbLcdDisplay LCD { get; set; }
        private IUltrasonicRangerSensor UltrasonicRangerSensor { get; set; }
        private ILed Led { get; set; }

        public GroveManager()
        {
            this.LCD = DeviceFactory.Build.RgbLcdDisplay();
            this.Led = DeviceFactory.Build.Led(LedPin);
            this.UltrasonicRangerSensor = DeviceFactory.Build.UltraSonicSensor(UltraSonicSensorPin);
        }

        public void ChangeAlarmState(bool alarmOn)
        {
            this.Led.ChangeState(alarmOn ? SensorStatus.On : SensorStatus.Off);
        }

        public void DisplayMessage(string message)
        {
            this.LCD.SetText(message);
        }

        public int GetDistanceFromLeader()
        {
            return UltrasonicRangerSensor.MeasureInCentimeters();
        }
    }

}
