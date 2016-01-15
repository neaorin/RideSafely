using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RideSafely.DeviceApp.ViewModels
{
    public class AppSettingsViewModel : INotifyPropertyChanged
    {
        private static string _iotHubConnectionStringFormat = "HostName={0}.azure-devices.net;DeviceId={1};SharedAccessKey={2}";

        private string _iotHubName;
        private string _deviceId;
        private string _deviceKey;
        private bool? _isLeader;

        public string IotHubName { get { return _iotHubName; } set { _iotHubName = value; OnChanged("IotHubName"); } }
        public string DeviceId { get { return _deviceId; } set { _deviceId = value; OnChanged("DeviceId"); CheckIsLeader(); } }
        public string DeviceKey { get { return _deviceKey; } set { _deviceKey = value; OnChanged("DeviceKey"); } }
        public bool? IsLeader { get { return _isLeader; } set { _isLeader = value; OnChanged("IsLeader"); } }

        public string ConnectionString { get {
                return string.Format(_iotHubConnectionStringFormat, IotHubName, DeviceId, DeviceKey);
            } }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void LoadFromSettings(IDictionary<string, object> settings)
        {
            this.IotHubName = (settings["IotHubName"] == null ? null : settings["IotHubName"].ToString());
            this.DeviceId = (settings["DeviceId"] == null ? null : settings["DeviceId"].ToString());
            this.DeviceKey = (settings["DeviceKey"] == null ? null : settings["DeviceKey"].ToString());
            this.IsLeader = (settings["IsLeader"] == null ? false : (bool)settings["IsLeader"]);
        }

        public void SaveToSettings(IDictionary<string, object> settings)
        {
            settings["IotHubName"] = this.IotHubName;
            settings["DeviceId"] = this.DeviceId;
            settings["DeviceKey"] = this.DeviceKey;
            settings["IsLeader"] = this.IsLeader;
        }

        private void CheckIsLeader()
        {
            this.IsLeader = this.DeviceId.ToLower().Contains("leader");                
        }
    }
}
