using DevExpress.UI.Xaml.Editors;

using DRLMobile.Core.Models.UIModels;
using DRLMobile.ExceptionHandler;
using DRLMobile.Uwp.ViewModel;

using System;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DRLMobile.Uwp.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class OrderHistoryDetailsPage : Page
    {
        private OrderHistoryDetailsPageViewModel ViewModel = null;

        public OrderHistoryDetailsPage()
        {
            this.InitializeComponent();

            DataContext = ViewModel;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                base.OnNavigatedTo(e);

                if (e.NavigationMode == NavigationMode.New || e.NavigationMode == NavigationMode.Back)
                {
                    ViewModel = new OrderHistoryDetailsPageViewModel();
                    ViewModel.OnNavigatedToCommand.Execute(e.Parameter);
                    ViewModel.PrintHelper = new StandardOptionsPrintHelper(this);
                    ViewModel.PrintHelper.RegisterForPrinting("OrderHistorDetails");
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(OrderHistoryDetailsPage), "OnNavigatedTo", ex.Message);
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            try
            {
                base.OnNavigatedFrom(e);
                if (ViewModel != null)
                {
                    if (ViewModel.PrintHelper != null)
                        ViewModel.PrintHelper.UnregisterForPrinting();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(OrderHistoryDetailsPage), "OnNavigatedFrom", ex.Message);
            }
        }

        private void NumericTextBox_BeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            try
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
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(OrderHistoryDetailsPage), "NumericTextBox_BeforeTextChanging", ex.Message);
            }
        }

        private async void QuantityCustomKeyPadFlyout_Closing(FlyoutBase sender, FlyoutBaseClosingEventArgs args)
        {
            try
            {
                ViewModel.quantityString = string.Empty;
                if (string.IsNullOrEmpty(ViewModel.AddEditUIModel.Quantity))
                {
                    if (ViewModel.quantityBeforeEdit == "-" || string.IsNullOrEmpty(ViewModel.quantityBeforeEdit))
                    {
                        ViewModel.quantityBeforeEdit = Convert.ToString(ViewModel.AddEditUIModel.EditedOrderDetail.Quantity);
                    }
                    ViewModel.AddEditUIModel.Quantity = ViewModel.quantityBeforeEdit;
                }

                if (ViewModel.AddEditUIModel.IsCreditRequest)
                {
                    if (!string.IsNullOrEmpty(ViewModel.AddEditUIModel.Quantity) && !ViewModel.AddEditUIModel.Quantity.Contains("-"))
                    {
                        ViewModel.AddEditUIModel.Quantity = "-" + ViewModel.AddEditUIModel.Quantity;
                    }
                }

                await ViewModel?.QuantityChangedCommand.ExecuteAsync(ViewModel.AddEditUIModel.Quantity);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(OrderHistoryDetailsPage), "QuantityCustomKeyPadFlyout_Closing", ex.Message);
            }
        }

        private void CustomKeypad_DoneClickEvent(object sender, bool e)
        {
            QuantityCustomKeyPadFlyout.Hide();
        }

        private void CustomKeypad_DoneClickEvent_1(object sender, bool e)
        {
            PriceKeyPadFlyout.Hide();
        }

        private async void Flyout_Closing(FlyoutBase sender, FlyoutBaseClosingEventArgs args)
        {
            try
            {
                ViewModel.PriceString = string.Empty;
                string currentPrice = ViewModel.AddEditUIModel.Price;
                currentPrice = currentPrice.Replace("$", "");
                if (string.IsNullOrEmpty(currentPrice) || currentPrice == ".")
                {
                    await ViewModel?.PriceChangedCommand.ExecuteAsync(string.Format("{0:0.00}", Convert.ToDecimal(ViewModel.AddEditUIModel.PriceToSave)));
                }
                else
                {
                    await ViewModel?.PriceChangedCommand.ExecuteAsync(string.Format("{0:0.00}", Convert.ToDecimal(currentPrice)));
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(OrderHistoryDetailsPage), "Flyout_Closing", ex.Message);
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var senderName = (ComboBox)sender;

                if (!string.IsNullOrEmpty(senderName.SelectedItem?.ToString()))
                {
                    ViewModel.AddEditUIModel.SelectedCreditRequest = senderName.SelectedItem?.ToString();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(OrderHistoryDetailsPage), "ComboBox_SelectionChanged", ex.Message);
            }
        }

        private void OnItemGridTapped(object sender, TappedRoutedEventArgs e)
        {
            ViewModel.ItemClickCommand.Execute((sender as Grid)?.DataContext);
        }

        private void UnitPriceImage_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ViewModel.IsOpenedFrom = "price";
            ShowFlyout(sender);
        }
        private void quantityImage_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ViewModel.IsOpenedFrom = "quantity";
            ShowFlyout(sender);
        }
        private void ShowFlyout(object sender) => FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);

        private void DataGrid_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            if (args.ItemIndex % 2 != 0)
            {
                //Grey background
                args.ItemContainer.Background = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(223, 223, 223, 223));
            }
            else
            {
                //White background
                args.ItemContainer.Background = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 255, 255));
            }
        }

    }
}
