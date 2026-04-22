using DRLMobile.Core.Models.DataModels;
using DRLMobile.ViewModels;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DRLMobile.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddCustomerPage : Page
    {
        private AddCustomerPageViewModel ViewModel = new AddCustomerPageViewModel();
        public AddCustomerPage()
        {
            this.InitializeComponent();
            this.DataContext = ViewModel;
        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ViewModel.NavigatedToCommand.Execute(null);
            ViewModel.FlyoutEvent += ViewModel_FlyoutEventHandler;
            ViewModel.GoBackEvent += ViewModel_GoBackEvent;
        }

        private void ViewModel_GoBackEvent(object sender, bool e)
        {
            try
            {
                var frame = ((Window.Current.Content as Frame).Content as ShellPage).FindName("shellFrame") as Frame;
                frame.GoBack();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            ViewModel.FlyoutEvent -= ViewModel_FlyoutEventHandler;
            ViewModel.GoBackEvent -= ViewModel_GoBackEvent;
        }

        private void ViewModel_FlyoutEventHandler(object sender, Core.Enums.CustomerDetailFlyoutType e)
        {
            switch (e)
            {
                case Core.Enums.CustomerDetailFlyoutType.Close:
                    DistributorFlyout.Hide();
                    FlyoutTextBox.Text = string.Empty;
                    break;
                case Core.Enums.CustomerDetailFlyoutType.CustomerName:
                    CustomerNameTextBox.Focus(FocusState.Programmatic);
                    AlertFlyoutTextBlock.Text = "Customer name is mandatory";
                    AlertFlyout.ShowAt(HeaderControl);
                    break;
                case Core.Enums.CustomerDetailFlyoutType.AccountClassification:
                    AccountClassificationComboBox.Focus(FocusState.Programmatic);
                    AlertFlyoutTextBlock.Text = "Classification is mandatory";
                    AlertFlyout.ShowAt(HeaderControl);
                    break;
                case Core.Enums.CustomerDetailFlyoutType.Rank:
                    RankComboBox.Focus(FocusState.Programmatic);
                    AlertFlyoutTextBlock.Text = "Rank is mandatory";
                    AlertFlyout.ShowAt(HeaderControl);
                    break;
                case Core.Enums.CustomerDetailFlyoutType.Address:
                    PhysicalAddressTextBox.Focus(FocusState.Programmatic);
                    AlertFlyoutTextBlock.Text = "Address is mandatory";
                    AlertFlyout.ShowAt(HeaderControl);
                    break;
                case Core.Enums.CustomerDetailFlyoutType.City:
                    PhysicalCityComboBox.Focus(FocusState.Programmatic);
                    AlertFlyoutTextBlock.Text = "City is mandatory";
                    AlertFlyout.ShowAt(HeaderControl);
                    break;
                case Core.Enums.CustomerDetailFlyoutType.State:
                    PhysicalStateComboBox.Focus(FocusState.Programmatic);
                    AlertFlyoutTextBlock.Text = "State is mandatory";
                    AlertFlyout.ShowAt(HeaderControl);
                    break;
                case Core.Enums.CustomerDetailFlyoutType.Zip:
                    PhysicalZipTextBox.Focus(FocusState.Programmatic);
                    AlertFlyoutTextBlock.Text = "Zip is mandatory";
                    AlertFlyout.ShowAt(HeaderControl);
                    break;
                case Core.Enums.CustomerDetailFlyoutType.InvalidEmail:
                    AlertFlyoutTextBlock.Text = "Enter a valid Email address";
                    AlertFlyout.ShowAt(HeaderControl);
                    break;
                case Core.Enums.CustomerDetailFlyoutType.InvalidFax:
                    AlertFlyoutTextBlock.Text = "Enter a valid fax number";
                    AlertFlyout.ShowAt(HeaderControl);
                    break;
                case Core.Enums.CustomerDetailFlyoutType.InvalidPhone:
                    AlertFlyoutTextBlock.Text = "Enter a valid phone number";
                    AlertFlyout.ShowAt(HeaderControl);
                    break;
            }
        }

        private void NumericTextBox_BeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            if (!string.IsNullOrWhiteSpace(args.NewText))
            {
                var isDigit = int.TryParse(args.NewText, out int returnVal);
                if (!isDigit)
                    args.Cancel = true;

                if (sender.SelectionLength == 0 && !string.IsNullOrWhiteSpace(args.NewText))
                {
                    sender.Select(args.NewText.Length, 0);
                }
            }
            else
                args.Cancel = false;
        }

        private void DistributorDeletedIconTapped(object sender, TappedRoutedEventArgs e)
        {
            ViewModel.DeleteDistributorButtonCommand.Execute((sender as Grid).DataContext as DistributorMaster);
        }

        private void DistributorTemplateGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ViewModel.DistributionSelectionCommand.Execute((sender as Grid).DataContext);
        }

        private void OpenDistributorFlyout(object sender, TappedRoutedEventArgs e)
        {
            DistributorFlyout.ShowAt(DistributorGrid as FrameworkElement);
        }
    }
}
