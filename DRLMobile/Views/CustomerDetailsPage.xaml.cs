using DRLMobile.Converters;
using DRLMobile.Core.Models.DataModels;
using DRLMobile.ViewModels;
using System;
using System.Threading.Tasks;
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
    public sealed partial class CustomerDetailsPage : Page
    {
        private CustomerDetailsPageViewModel ViewModel = new CustomerDetailsPageViewModel();
        public CustomerDetailsPage()
        {
            this.InitializeComponent();
            DataContext = ViewModel;
        }


        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await FocusManager.TryFocusAsync(CustomerNameTextBox, FocusState.Programmatic);
            ViewModel?.OnNavigatedToCommand.Execute(e.Parameter);
            if (ViewModel != null)
            {
                ViewModel.FlyoutEvent += FlyoutOpen;
            }
        }


        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            ViewModel.FlyoutEvent -= FlyoutOpen;
        }

        private void FlyoutOpen(object sender, Core.Enums.CustomerDetailFlyoutType e)
        {
            switch (e)
            {
                case Core.Enums.CustomerDetailFlyoutType.DistributorFlyout:
                    SalesTaxCertificateTextBox.Focus(FocusState.Programmatic);
                    DistributorFlyout.ShowAt(DistributorGrid as FrameworkElement);
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
                case Core.Enums.CustomerDetailFlyoutType.Close:
                    DistributorFlyout.Hide();
                    FlyoutTextBox.Text = string.Empty;
                    break;
                case Core.Enums.CustomerDetailFlyoutType.ContactName:
                    AlertFlyoutTextBlock.Text = "Contact Name is Mandatory";
                    AlertFlyout.ShowAt(HeaderControl);
                    break;
                case Core.Enums.CustomerDetailFlyoutType.ContactEmail:
                    AlertFlyoutTextBlock.Text = "Contact Email is Mandatory";
                    AlertFlyout.ShowAt(HeaderControl);
                    break;
                case Core.Enums.CustomerDetailFlyoutType.ContactRank:
                    AlertFlyoutTextBlock.Text = "Contact Rank is Mandatory";
                    AlertFlyout.ShowAt(HeaderControl);
                    break;
                case Core.Enums.CustomerDetailFlyoutType.ContactPhone:
                    AlertFlyoutTextBlock.Text = "Contact Phone is Mandatory";
                    AlertFlyout.ShowAt(HeaderControl);
                    break;
                case Core.Enums.CustomerDetailFlyoutType.ContactAllMandatoryFeilds:
                    AlertFlyoutTextBlock.Text = "Contact Name ,Email, Rank is Mandatory";
                    AlertFlyout.ShowAt(HeaderControl);
                    break;
                case Core.Enums.CustomerDetailFlyoutType.InValidContactEmail:
                    AlertFlyoutTextBlock.Text = "Enter a valid contact Email address";
                    AlertFlyout.ShowAt(HeaderControl);
                    break;
                case Core.Enums.CustomerDetailFlyoutType.InvalidContactFax:
                    AlertFlyoutTextBlock.Text = "Enter a Valid contact Fax";
                    AlertFlyout.ShowAt(HeaderControl);
                    break;
                case Core.Enums.CustomerDetailFlyoutType.InvalidContactPhone:
                    AlertFlyoutTextBlock.Text = "Enter a valid contact phone number";
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



        private void DistributorTemplateGrid_Tapped(object sender, TappedRoutedEventArgs e) => ViewModel.UiModel.DistributionSelectionCommand.Execute((sender as Grid).DataContext);




        private void DistributorDeletedIconTapped(object sender, TappedRoutedEventArgs e)
        {
            ViewModel.UiModel.DeleteDistributorButtonCommand.Execute((sender as Grid).DataContext as DistributorMaster);
        }

        private async void DeleteContactGridTapped(object sender, TappedRoutedEventArgs e)
        {
            var result = await ShowConfirmationAlert(Helpers.ResourceExtensions.GetLocalized("Confirmation"), Helpers.ResourceExtensions.GetLocalized("ContactDeleteConfirmationMsg"));
            if (result) ViewModel.UiModel.RemoveContactCommand.Execute((sender as Grid).DataContext);
        }


        private async Task<bool> ShowConfirmationAlert(string title, string msg)
        {
            ContentDialog userLogoutDialog = new ContentDialog
            {
                Title = title,
                Content = msg,
                PrimaryButtonText = "Yes",
                SecondaryButtonText = "No"
            };

            var result = await userLogoutDialog.ShowAsync();
            userLogoutDialog.Hide();
            if (result == ContentDialogResult.Primary)
            {
                return true;
            }
            else return false;

        }

    }
}
