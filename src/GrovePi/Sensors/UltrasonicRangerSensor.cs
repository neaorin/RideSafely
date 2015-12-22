using System.Threading.Tasks;

namespace GrovePi.Sensors
{
    public interface IUltrasonicRangerSensor
    {
        Task<int> MeasureInCentimetersAsync();
    }

    internal class UltrasonicRangerSensor : IUltrasonicRangerSensor
    {
        private const byte CommandAddress = 7;
        private readonly GrovePi _device;
        private readonly Pin _pin;

        internal UltrasonicRangerSensor(GrovePi device, Pin pin)
        {
            _device = device;
            _pin = pin;
        }

        public async Task<int> MeasureInCentimetersAsync()
        {
            var buffer = new[] {CommandAddress, (byte) _pin, Constants.Unused, Constants.Unused};
            _device.DirectAccess.Write(buffer);
            await Task.Delay(500);
            _device.DirectAccess.Read(buffer);
            return buffer[1]*256 + buffer[2];
        }
    }
}