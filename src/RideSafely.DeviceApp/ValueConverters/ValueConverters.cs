using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace RideSafely.DeviceApp.ValueConverters
{
    public class LedBrushValueConverter : IValueConverter
    {
        static SolidColorBrush brushOn = new SolidColorBrush(Colors.Red);
        static SolidColorBrush brushOff = new SolidColorBrush(Colors.Magenta);
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool led = (bool)value;
            return led ? brushOn : brushOff;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
