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
    public sealed partial class SettingsPage : Page
    {
        private SettingPageViewModel ViewModel = new SettingPageViewModel();

        public SettingsPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            ViewModel?.OnNavigatingFrom.Execute(null);
        }

        private void ChangePinButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ViewModel.OldPinText = string.Empty;
            ViewModel.NewPinText = string.Empty;
            ViewModel.ConfirmNewPinText = string.Empty;
            ViewModel.ErrorForConfirmNewPinText = string.Empty;
            ViewModel.ErrorForNewPinText = string.Empty;
            ViewModel.ErrorForOldPinText = string.Empty;
            ChangePinFlyout.ShowAt(outerPanel);
        }

        private void SaveButtonClicked(object sender, RoutedEventArgs e)
        {
            //ChangePinFlyout.Hide();
            ViewModel.ChangePinSaveButtonCommand.Execute(ChangePinFlyout);
        }

        private void TextBox_BeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            if(!string.IsNullOrWhiteSpace(args.NewText))
            {
                var isDigit = int.TryParse(args.NewText, out int returnVal);
                if (!isDigit)
                    args.Cancel = true;
            }
        }

    }
}
