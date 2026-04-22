using System;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace DRLMobile.Converters
{
    public class BoolToCartImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (bool)value ? new BitmapImage(new Uri("ms-appx:///Assets/SRCProduct/cart_selected.png")) : new BitmapImage(new Uri("ms-appx:///Assets/SRCProduct/cart_normal.png"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
