using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace DRLMobile.Converters
{
    public class StringToPhoneNumberConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var databasePhoneNo = (string)value;
            string justDigits = new string(databasePhoneNo?.Where(c => char.IsDigit(c)).ToArray());
            var isDouble = double.TryParse(justDigits, out double val);
            var returnValue = isDouble ? string.Format(Constants.Constants.PHONE_FORMAT, val) : string.Empty;
            return returnValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var str = (string)value;
            string justDigits = new string(str.Where(c => char.IsDigit(c)).ToArray());
            return justDigits;
        }
    }
}
