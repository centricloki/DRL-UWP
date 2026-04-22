using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace DRLMobile.Uwp.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var isInEditMode = (bool)value;
            if (isInEditMode)
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var s = (Visibility)value;

            if (s == Visibility.Visible)
            {
                return true;
            }
            return false;
        }
    }


    public class VisibilityToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var s = (Visibility)value;

            if (parameter == null)
            {
                return (s == Visibility.Visible);               
            }
            else //reverse the visibility
            {
                return (s == Visibility.Collapsed);               
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var isInEditMode = (bool)value;
            if (isInEditMode)
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }
    }
}
