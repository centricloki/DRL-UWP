using DRLMobile.Core.Models.UIModels;
using DRLMobile.ViewModels;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Globalization;
using System.Linq;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DRLMobile.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RetailTransactionPage : Page
    {
        private RetailTransactionPageViewModel retailTransactionPageViewModel = new RetailTransactionPageViewModel();

        public RetailTransactionPage()
        {
            this.InitializeComponent();

            DataContext = retailTransactionPageViewModel;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            await retailTransactionPageViewModel.InitializeDataOnPageLoad();

            prebookDatePicker.Date = DateTime.Now.Date;

            retailTransactionPageViewModel.RetailTransacUiModel.PreBookShipDate = prebookDatePicker.Date.ToString();

            DateTime datetime = Convert.ToDateTime(retailTransactionPageViewModel.RetailTransacUiModel.PreBookShipDate);

            retailTransactionPageViewModel.RetailTransacUiModel.PreBookShipDate = datetime.ToString("MM/dd/yyyy hh:mm:ss", CultureInfo.InvariantCulture);

            if (retailTransactionPageViewModel.RetailTransacUiModel.IsSampleOrder)
            {
                SalesTypeStackPanel.Visibility = Visibility.Collapsed;
                rackPosOrderStackPanel.Visibility = Visibility.Collapsed;
                sampleOrderStackPanel.Visibility = Visibility.Visible;
            }
            else if (retailTransactionPageViewModel.RetailTransacUiModel.IsRackOrder)
            {
                SalesTypeStackPanel.Visibility = Visibility.Collapsed;
                rackPosOrderStackPanel.Visibility = Visibility.Visible;
                sampleOrderStackPanel.Visibility = Visibility.Collapsed;
            }
            else if (retailTransactionPageViewModel.RetailTransacUiModel.IsPopOrder || retailTransactionPageViewModel.RetailTransacUiModel.IsDirectCustomer)
            {
                salesTypeLabel.Visibility = Visibility.Collapsed;
                SalesTypeStackPanel.Visibility = Visibility.Collapsed;
                rackPosOrderStackPanel.Visibility = Visibility.Collapsed;
                sampleOrderStackPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                SalesTypeStackPanel.Visibility = Visibility.Visible;
                rackPosOrderStackPanel.Visibility = Visibility.Collapsed;
                sampleOrderStackPanel.Visibility = Visibility.Collapsed;
                retailTransactionPageViewModel.RetailTransacUiModel.SelectedSalesType = "1";
            }
        }

        private void SalesTypeOption_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedSalesType = (sender as RadioButtons).SelectedItem as string;

            retailTransactionPageViewModel.RetailTransacUiModel.ActivityType = selectedSalesType;

            switch (selectedSalesType)
            {
                case "Cash Sale":
                    {
                        retailTransactionPageViewModel.RetailTransacUiModel.SelectedSalesType = "1";

                        distributorStackPanel.Visibility = Visibility.Collapsed;

                        retailTransactionPageViewModel.ClearEmailToField();
                    }
                    break;
                case "Prebook":
                    {
                        retailTransactionPageViewModel.RetailTransacUiModel.SelectedSalesType = "2";

                        if (retailTransactionPageViewModel.RetailTransacUiModel.IsDirectCustomer)
                        {
                            distributorStackPanel.Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            distributorStackPanel.Visibility = Visibility.Visible;

                            retailTransactionPageViewModel.RetailTransacUiModel.CreateDistributorsCollection();
                            retailTransactionPageViewModel.RetailTransacUiModel.PopulateStateNameForDistributorList();
                        }

                        retailTransactionPageViewModel.ClearEmailToField();
                    }
                    break;
                case "Bill Through":
                    {
                        retailTransactionPageViewModel.RetailTransacUiModel.SelectedSalesType = "3";

                        distributorStackPanel.Visibility = Visibility.Collapsed;

                        retailTransactionPageViewModel.ClearEmailToField();
                    }
                    break;
                case "Suggested Order":
                    {
                        retailTransactionPageViewModel.RetailTransacUiModel.SelectedSalesType = "4";

                        distributorStackPanel.Visibility = Visibility.Collapsed;

                        retailTransactionPageViewModel.ClearEmailToField();
                    }
                    break;
                case "Rack/POS":
                    {
                        retailTransactionPageViewModel.RetailTransacUiModel.SelectedSalesType = "5";

                        distributorStackPanel.Visibility = Visibility.Collapsed;

                        retailTransactionPageViewModel.ClearEmailToField();
                    }
                    break;
                case "Cash Sales Initiative":
                    {
                        retailTransactionPageViewModel.RetailTransacUiModel.SelectedSalesType = "9";

                        distributorStackPanel.Visibility = Visibility.Collapsed;

                        retailTransactionPageViewModel.ClearEmailToField();
                    }
                    break;
                case "Chain Distribution":
                    {
                        retailTransactionPageViewModel.RetailTransacUiModel.SelectedSalesType = "12";

                        retailTransactionPageViewModel.RetailTransacUiModel.EmailTo = retailTransactionPageViewModel.RetailTransacUiModel.UserEmailId;

                        distributorStackPanel.Visibility = Visibility.Collapsed;
                    }
                    break;
                case "Credit Card Sales":
                    {
                        retailTransactionPageViewModel.RetailTransacUiModel.SelectedSalesType = "13";

                        distributorStackPanel.Visibility = Visibility.Collapsed;

                        retailTransactionPageViewModel.ClearEmailToField();
                    }
                    break;
                default:
                    {
                        retailTransactionPageViewModel.RetailTransacUiModel.SelectedSalesType = "7";

                        distributorStackPanel.Visibility = Visibility.Collapsed;

                        retailTransactionPageViewModel.ClearEmailToField();
                    }
                    break;
            }
        }

        private async void DistributorGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (retailTransactionPageViewModel.RetailTransacUiModel.DistributorsList == null ||
                retailTransactionPageViewModel.RetailTransacUiModel.DistributorsList?.Count == 0)
            {
                ContentDialog noDistributorAddedDialog = new ContentDialog
                {
                    Content = ResourceLoader.GetForCurrentView().GetString("NoDistributorAddedMessage"),
                    CloseButtonText = ResourceLoader.GetForCurrentView().GetString("OK")
                };

                await noDistributorAddedDialog.ShowAsync();
            }
            else
            {
                DistributorFlyout.ShowAt(DistributorGrid as FrameworkElement);
            }
        }

        private void DistributorTemplateGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            retailTransactionPageViewModel.DistributionSelectionCommand.Execute((sender as Grid).DataContext);

            selectedDistributor.Text = retailTransactionPageViewModel.RetailTransacUiModel.SelectedDistributorName;

            DistributorFlyout.Hide();
        }

        private void prebookDatePicker_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            retailTransactionPageViewModel.RetailTransacUiModel.PreBookShipDate = prebookDatePicker.Date?.ToString();

            DateTime datetime = Convert.ToDateTime(retailTransactionPageViewModel.RetailTransacUiModel.PreBookShipDate);

            retailTransactionPageViewModel.RetailTransacUiModel.PreBookShipDate = datetime.ToString("MM/dd/yyyy hh:mm:ss", CultureInfo.InvariantCulture);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DistributorFlyout.Hide();
        }

        private void SignatureButton_Click(object sender, RoutedEventArgs e)
        {
            retailTransactionPageViewModel.RetailTransacUiModel.SignaturePanelVisibility = Visibility.Visible;
        }

        private void quantityTextBlock_BeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            if (args.NewText.Trim().Length == 1 && args.NewText.Trim().Equals("0"))
            {
                args.Cancel = true;
            }
            else
            {
                args.Cancel = args.NewText.Any(c => !char.IsDigit(c));
            }
        }

        private void quantityTextBlock_GotFocus(object sender, RoutedEventArgs e)
        {
            var senderName = (TextBox)sender;
            var dataCxtx = senderName.DataContext;
            var dataSource = (OrderDetailUIModel)dataCxtx;
            retailTransactionPageViewModel.RetailTransacUiModel.OrderDetailModel = dataSource;
            retailTransactionPageViewModel.quantityBeforeEdit = dataSource.QuantityDisplay;
            FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
        }

        private void QuantityTextBlock_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            var senderName = (TextBox)sender;
            var dataCxtx = senderName.DataContext;
            var dataSource = (OrderDetailUIModel)dataCxtx;

            if (!string.IsNullOrEmpty(senderName.Text))
            {
                ///var quantityBefore = dataSource.Quantity;
                dataSource.Quantity = Convert.ToInt32(senderName.Text);
                retailTransactionPageViewModel?.QuantityChangedCommand.Execute(dataSource);
            }
        }

        private void QuantityCustomKeyPadFlyout_Closing(FlyoutBase sender,FlyoutBaseClosingEventArgs args)
        {
            if (string.IsNullOrEmpty(retailTransactionPageViewModel?.RetailTransacUiModel.OrderDetailModel?.QuantityDisplay))
            {
                retailTransactionPageViewModel.RetailTransacUiModel.OrderDetailModel.QuantityDisplay = retailTransactionPageViewModel?.quantityBeforeEdit;
                if (string.IsNullOrEmpty(retailTransactionPageViewModel?.RetailTransacUiModel.OrderDetailModel?.QuantityDisplay))
                {
                    retailTransactionPageViewModel.RetailTransacUiModel.OrderDetailModel.Quantity = Convert.ToInt32(retailTransactionPageViewModel?.RetailTransacUiModel.OrderDetailModel?.QuantityDisplay);
                }

            }
            retailTransactionPageViewModel?.QuantityChangedCommand.Execute(retailTransactionPageViewModel?.RetailTransacUiModel.OrderDetailModel);
        }

        private void CustomKeypad_DoneClickEvent1(object sender, bool e)
        {
            QuantityGridCustomKeyPadFlyout.Hide();
        }

        private void Delete_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var senderName = (Grid)sender;
            var dataCxtx = senderName.DataContext;
            var dataSource = (OrderDetailUIModel)dataCxtx;
            retailTransactionPageViewModel?.DeleteImageCommand.Execute(dataSource);
        }
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
        }
    }
}
