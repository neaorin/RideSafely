using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RideSafely.DeviceApp.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private AppSettingsViewModel _appSettings;
        private int _distanceToLeader;
        private string _message;
        private bool _alarm;

        public AppSettingsViewModel AppSettings { get { return _appSettings; } set { _appSettings = value; OnChanged("AppSettings"); } }
        public int DistanceToLeader { get { return _distanceToLeader; } set { _distanceToLeader = value; OnChanged("DistanceToLeader"); } }
        public string Message { get { return _message; } set { _message = value; OnChanged("Message"); } }
        public bool Alarm { get { return _alarm; } set { _alarm = value; OnChanged("Alarm"); } }


        public event PropertyChangedEventHandler PropertyChanged;

        private void OnChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
