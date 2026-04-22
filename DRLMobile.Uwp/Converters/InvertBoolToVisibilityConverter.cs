using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace DRLMobile.Uwp.Converters
{
    public class InvertBoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var boolValue = (bool)value;
            if (boolValue)
                return Visibility.Collapsed;
            else
                return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class InvertVisibilityConverter : IValueConverter
    {
        public object Convert(object _value, Type _targetType, object _parameter, string _language)
        {          
            Visibility value = (Visibility)_value;
            if (value == Visibility.Visible)
                return Visibility.Collapsed;
            else
                return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)=> throw new NotImplementedException();
        
    }

}
