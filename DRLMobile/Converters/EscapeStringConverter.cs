using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace DRLMobile.Converters
{
    public class EscapeStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is null)
                return string.Empty;

            var strVal = (string)value;
            var replacedString =  strVal.Replace("\\'", "'");
            return replacedString;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is null)
                return string.Empty;
            else
            {
                var strVal = (string)value;
                var replacedString = strVal.Replace("'", "\\'");
                return replacedString;
            }
        }
    }
}
