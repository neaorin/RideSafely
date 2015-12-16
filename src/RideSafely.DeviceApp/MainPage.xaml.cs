//#define BUMPDETECTOR
#define OTHERSTUFF

using GHIElectronics.UWP.Shields;
using RideSafely.GrovePi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409



namespace RideSafely.DeviceApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
#if BUMPDETECTOR
            this.SetupBumpDetectorAsync();
#elif OTHERSTUFF
            (new GroveManager()).RunAsync();
#endif
        }

#if BUMPDETECTOR
        BumpDetector bumpDetector;
        private async Task SetupBumpDetectorAsync()
        {
            bumpDetector = new BumpDetector();

            await bumpDetector.StartAsync(Vector3.Zero);
            bumpDetector.BumpOccured += bumpDetector_BumpOccured;
        }


        DispatcherTimer bumpTimer = null;
        private void bumpDetector_BumpOccured(object sender, DateTime e)
        {
            BumpTextBox.Text = $"Bump occured at {e}";

            if (bumpTimer != null)
            {
                bumpDetector.Hat.D2.Color = bumpDetector.Hat.D3.Color = FEZHAT.Color.Black;
                bumpTimer.Stop();
                bumpTimer = null;
            }

            bumpTimer = new DispatcherTimer();
            this.bumpTimer.Interval = TimeSpan.FromMilliseconds(500);
            bumpTimer.Start();
            int i = 0;
            this.bumpTimer.Tick += (sender2, obj) =>
            {
                if (i % 2 == 0)
                    bumpDetector.Hat.D2.Color = bumpDetector.Hat.D3.Color = FEZHAT.Color.White;
                else
                    bumpDetector.Hat.D2.Color = bumpDetector.Hat.D3.Color = FEZHAT.Color.Black;
                if (i == 19)
                {
                    bumpTimer.Stop();
                    bumpTimer = null;
                }
                i++;
            };
        }
#endif


    }
}
