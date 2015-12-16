using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using GIS = GHIElectronics.UWP.Shields;

namespace RideSafely.DeviceApp
{
    public class BumpDetector
    {
        static double accelerometerUpdateInterval = 1.0 / 60.0;
        
        static double lowPassKernelWidthInSeconds = 1.0;
        
        double shakeDetectionThreshold = 0.5;

        private double lowPassFilterFactor = accelerometerUpdateInterval / lowPassKernelWidthInSeconds;
        private Vector3 lowPassValue = Vector3.Zero;
        private Vector3 acceleration;
        private Vector3 deltaAcceleration;

        private DispatcherTimer timer;
        public GIS.FEZHAT Hat;
        

        public async Task StartAsync(Vector3 _acceleration)
        {
            shakeDetectionThreshold *= shakeDetectionThreshold;
            lowPassValue = _acceleration;

            this.Hat = await GIS.FEZHAT.CreateAsync();

           

            double x, y, z;
            this.Hat.GetAcceleration(out x, out y, out z);
            

            this.timer = new DispatcherTimer();
            this.timer.Interval = TimeSpan.FromMilliseconds(100);
            this.timer.Tick += this.OnTick;
            this.timer.Start();
        }

        private void OnTick(object sender, object e)
        {
            double x, y, z;

            this.Hat.GetAcceleration(out x, out y, out z);
            this.Update(new Vector3((float)x, (float)y, (float)z));
        }

        public void Update(Vector3 _acceleration)
        {
            acceleration = _acceleration;
            lowPassValue = Vector3.Lerp(lowPassValue, acceleration, (float)lowPassFilterFactor);
            deltaAcceleration = acceleration - lowPassValue;
            Debug.WriteLine(deltaAcceleration.LengthSquared());
            if (deltaAcceleration.LengthSquared() >= shakeDetectionThreshold)
            {
                // Perform your "shaking actions" here, with suitable guards in the if check above, if necessary to not, to not fire again if they're already being performed.
                //Debug.Log("Shake event detected at time " + Time.time);
                BumpOccured?.Invoke(this, DateTime.Now);
            }
        }

        public event EventHandler<DateTime> BumpOccured;
    }
}
