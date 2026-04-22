using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace DRLMobile.Uwp.Converters
{
    public class BoolToStyleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var parm = (string)parameter;
            var isInEditMode = (bool)value;

            switch (parm)
            {
                case "TextBox":
                    return isInEditMode ? (Style)Application.Current.Resources["EditTextBoxStyle"] : (Style)Application.Current.Resources["ViewOnlyTextBoxStyle"];
                case "ComboBox":
                    return isInEditMode ? (Style)Application.Current.Resources["ComboBoxWhiteBackground"] : (Style)Application.Current.Resources["ComboBoxViewOnlyStyle"];
                case "Grid":
                    return isInEditMode ? (Style)Application.Current.Resources["EditBackgroundGridStyle"] : (Style)Application.Current.Resources["ViewOnlyBackgroundGridStyle"];
                case "ListViewDistributor":
                    return isInEditMode ? (Style)Application.Current.Resources["ReorderListStyle"] : (Style)Application.Current.Resources["NoReorderListStyle"];
                case "DevExpressEditor":
                    return isInEditMode ? (Style)Application.Current.Resources["EditableDevExpressEditor"] : (Style)Application.Current.Resources["ViewOnlyDevExpressEditor"];
                case "Date":
                    return isInEditMode ? (Style)Application.Current.Resources["EditCalendar"] : (Style)Application.Current.Resources["ViewOnlyCalendar"];
                case "ZoneFilterTextBlock":
                    return isInEditMode ? (Style)Application.Current.Resources["ZoneFilterLeftLabelStyle"] : (Style)Application.Current.Resources["ZoneFilterLeftLabelDisabledStyle"];
                case "ZoneFilterFontIcon":
                    return isInEditMode ? (Style)Application.Current.Resources["ZoneFontIconStyle"] : (Style)Application.Current.Resources["ZoneFontIconDisabledStyle"];
                case "RouteCustomerCheckBox":
                    return isInEditMode ? (Style)Application.Current.Resources["RouteCustomerCheckBoxChecked"] : (Style)Application.Current.Resources["RouteCustomerCheckBoxUnchecked"];

                    
                        
                default:
                    return null;
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
