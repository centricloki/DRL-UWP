using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace DRLMobile.Uwp.Converters
{
    public class BoolToFavoriteDetailImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (bool)value ? new BitmapImage(new Uri("ms-appx:///Assets/ProductDetail/favorite_selected.png")) : new BitmapImage(new Uri("ms-appx:///Assets/ProductDetail/favorite_normal.png"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
