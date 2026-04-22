using DRLMobile.Uwp.ViewModel;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DRLMobile.Uwp.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HeadQuarterPage : Page
    {
        public HeadQuarterPage()
        {
            this.InitializeComponent();
        }

        //// Handles the Click event on the Button inside the Popup control and 
        //// closes the Popup. 
        //private void ClosePopupClicked(object sender, RoutedEventArgs e)
        //{
        //    // if the Popup is open, then close it 
        //    if (StandardPopup.IsOpen) { StandardPopup.IsOpen = false; }
        //}

        //// Handles the Click event on the Button on the page and opens the Popup. 
        //private void ShowPopupOffsetClicked(object sender, RoutedEventArgs e)
        //{
        //    // open the Popup if it isn't open already 
        //    if (!StandardPopup.IsOpen) { StandardPopup.IsOpen = true; }
        //}
    }
}
