
using RideSafely.Common;
using RideSafely.DeviceApp.Managers;
using RideSafely.DeviceApp.ViewModels;
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
        private ProgramManager ProgramManager { get; set; }
        private MainViewModel ViewModel { get; set; }
        public MainPage()
        {

            this.InitializeComponent();

            this.ViewModel = new MainViewModel();

            // test data
            //var appSettings = new AppSettingsViewModel();
            //appSettings.IotHubName = "<enter hub name>";
            //appSettings.DeviceId = "<enter device id>";
            //appSettings.DeviceKey = "<enter key>";
            //appSettings.IsLeader = false;
            //appSettings.SaveToSettings(Windows.Storage.ApplicationData.Current.LocalSettings.Values);
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var settingsDialog = new SettingsDialog();
            var result = await settingsDialog.ShowAsync();

            if (result != ContentDialogResult.Primary)
                Application.Current.Exit();

            ViewModel.AppSettings = new AppSettingsViewModel();
            ViewModel.AppSettings.LoadFromSettings(Windows.Storage.ApplicationData.Current.LocalSettings.Values);


            // device manager
            IDeviceManager deviceManager = null;
            // if ARM, we deploy this on Raspberry PI with GrovePi+
            // if not, we run on a simulated device
#if ARM
            deviceManager = new GrovePi.GroveManager();
#else
            deviceManager = new SimulatedDeviceManager();
#endif
            // Program
            this.ProgramManager = new ProgramManager(
                ViewModel,
                new AzureConnectManager(ViewModel.AppSettings.ConnectionString),
                deviceManager
                );           

            this.ProgramManager.Run();
        }
    }
}
