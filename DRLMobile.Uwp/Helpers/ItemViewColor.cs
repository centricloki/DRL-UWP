using System;

using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using DevExpress.Pdf;
using System.Collections.ObjectModel;
using DevExpress.Core.Collections;
using DevExpress.Data.Extensions;
using System.Collections.Generic;

namespace DRLMobile.Uwp.Helpers
{
    public class ItemViewColor
    {
        private static Dictionary<string, Color> colorCollection = new Dictionary<string, Color>(capacity:1);
        // Set your desired colors for even and odd rows
        private static readonly Color evenColor = Color.FromArgb(255, 255, 255, 255);//Grey background
        private static readonly Color oddColor = Color.FromArgb(223, 223, 223, 223);//White background
        public static SolidColorBrush Get(string _key)
        {
            if (colorCollection.ContainsKey(_key))
            {
                if (Color.Equals(evenColor, colorCollection[_key]))
                    colorCollection[_key] = oddColor;
                else
                    colorCollection[_key] = evenColor;
            }
            else
            {
                colorCollection.Clear();
                colorCollection.Add(_key, oddColor);
            }
            return new SolidColorBrush(colorCollection[_key]);
        }
    }
}
