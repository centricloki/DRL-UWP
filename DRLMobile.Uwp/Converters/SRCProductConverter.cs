using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DevExpress.Mvvm.Native;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace DRLMobile.Uwp.Converters
{
    public class SRCProductColumnWidthConverter : IValueConverter
    {
        public object Convert(object _value, Type _targetType, object _parameter, string _language)
        {
            Visibility _isSaleDocCategory = (Visibility)_value;
            int _colIndex = System.Convert.ToInt32(_parameter);

            if (_isSaleDocCategory == Visibility.Visible)
            {
                if (_colIndex < 4)
                    return new GridLength(3.4d, Windows.UI.Xaml.GridUnitType.Star);
                else
                    return new GridLength(0d);
            }
            else
            {
                if ((new int[] { 0, 1, 5 }).Contains(_colIndex))
                    return new GridLength(3.4d, Windows.UI.Xaml.GridUnitType.Star);
                else
                    return new GridLength(2.4d, Windows.UI.Xaml.GridUnitType.Star);
            }
        }

        public object ConvertBack(object _value, Type _targetType, object _parameter, string _language) => throw new NotImplementedException();

    }


    public class SRCProductSelectedItemConverter : IValueConverter
    {
        public object Convert(object _value, Type _targetType, object _parameter, string _language) => new Windows.UI.Xaml.Media.SolidColorBrush(((bool)_value ? Windows.UI.Colors.Red : Windows.UI.Colors.Transparent));

        public object ConvertBack(object _value, Type _targetType, object _parameter, string _language) => throw new NotImplementedException();

    }
}
