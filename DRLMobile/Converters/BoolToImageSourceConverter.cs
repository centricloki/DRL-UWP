using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace DRLMobile.Converters
{
    public class BoolToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var isEditMode = (bool)value;
            var parms = (string)parameter;
            string returnVal = "";
            switch (parms)
            {
                case "Hover":
                    returnVal = isEditMode == true ? (string)Application.Current.Resources["SaveIconHover"] : (string)Application.Current.Resources["EditIconHover"];
                    return returnVal;
                case "Normal":
                    returnVal = isEditMode == true ?(string) Application.Current.Resources["SaveIconNormal"] : (string) Application.Current.Resources["EditIconNormal"];
                    return returnVal;
                default:
                    return string.Empty;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return false;
        }
    }
}
