using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace DRLMobile.Uwp.Converters
{
    public class BooleanToGridLengthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool isVisible && isVisible)
            {
                // Parse the desired width from parameter, e.g., "4.6"
                if (parameter is string param && double.TryParse(param, out double stars))
                {
                    return new GridLength(stars, GridUnitType.Star);
                }
            }
            return new GridLength(0); // Hidden
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
