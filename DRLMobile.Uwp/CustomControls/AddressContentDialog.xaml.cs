using DRLMobile.Core.Models.FedExAddressValidationModels;

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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DRLMobile.Uwp.CustomControls
{
    public sealed partial class AddressContentDialog : ContentDialog
    {
        public FedExAddressContentDialog EnteredAddress { get; set; }
        public FedExAddressContentDialog SuggestedAddress { get; set; }


        public AddressContentDialog(FedExAddressContentDialog enteredAddress, FedExAddressContentDialog suggestedAddress)
        {
            this.InitializeComponent();
            EnteredAddress = enteredAddress;
            SuggestedAddress = suggestedAddress;
            DataContext = this;
        }

        public SolidColorBrush IsTextNotEqual(string arg1, string arg2)
        {
            if (string.IsNullOrWhiteSpace(arg1)||(!string.IsNullOrWhiteSpace(arg1) && !arg1.Equals(arg2, StringComparison.OrdinalIgnoreCase)))
                return (SolidColorBrush)Application.Current.Resources["R255_G38_B0"];
            else return (SolidColorBrush)Application.Current.Resources["TextFillColorPrimaryBrush"];
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
    }
}
