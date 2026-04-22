using DevExpress.UI.Xaml.Editors;
using DRLMobile.Core.Models.UIModels;
using DRLMobile.ViewModels;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DRLMobile.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CartPage : Page
    {
        #region properties
        private CartPageViewModel ViewModel = new CartPageViewModel();
        #endregion

        public CartPage()
        {
            this.InitializeComponent();
            DataContext = ViewModel;

        }
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            await ViewModel.LoadInitialPageData();
        }

        private void Delete_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            var senderName = (Grid)sender;
            var dataCxtx = senderName.DataContext;
            var dataSource = (OrderDetailUIModel)dataCxtx;
            ViewModel?.DeleteImageCommand.Execute(dataSource);
        }

        private void QuantityTextBlock_BeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
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

        private void headerCheckbox_Checked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ViewModel.IsAllUpperGridSelectedCommand.Execute(null);
        }

        private void headerCheckbox_Unchecked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ViewModel.IsAllUpperGridUnselectedCommand.Execute(null);
        }

        private void LowerGridheaderCheckbox_Checked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ViewModel.IsAllLowerGridSelectedCommand.Execute(null);
        }

        private void LowerGridheaderCheckbox_Unchecked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ViewModel.IsAllLowerGridUnselectedCommand.Execute(null);
        }

        private void QuantityTextBlock_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var senderName = (TextBox)sender;
            var dataCxtx = senderName.DataContext;
            var dataSource = (OrderDetailUIModel)dataCxtx;
           
            if (!string.IsNullOrEmpty(senderName.Text))
            {
                ///var quantityBefore = dataSource.Quantity;
                dataSource.Quantity = Convert.ToInt32(senderName.Text);
                ViewModel?.QuantityChangedCommand.Execute(dataSource);
            }
        }

        private void UnitComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var senderName = (ComboBox)sender;
            var dataCxtx = senderName.DataContext;
            var dataSource = (OrderDetailUIModel)dataCxtx;
            if (!string.IsNullOrEmpty(senderName.SelectedItem?.ToString()))
            {
                dataSource.Unit = senderName.SelectedItem?.ToString();
                ViewModel?.UnitComboboxChangedCommand.Execute(dataSource);
            }
        }

        private void RtnGridCreditRequestComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var senderName = (ComboBox)sender;
            var dataCxtx = senderName.DataContext;
            var dataSource = (OrderDetailUIModel)dataCxtx;

            if (!string.IsNullOrEmpty(senderName.SelectedItem?.ToString()))
            {
                dataSource.CreditRequest = senderName.SelectedItem?.ToString();
                ViewModel?.CreditRequestComboboxChangedCommand.Execute(dataSource);
            }
        }

        private void DIFCreditRequestComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var senderName = (ComboBox)sender;
            var dataCxtx = senderName.DataContext;
            var dataSource = (OrderDetailUIModel)dataCxtx;

            if (!string.IsNullOrEmpty(senderName.SelectedItem?.ToString()))
            {
                dataSource.CreditRequest = senderName.SelectedItem?.ToString();
                ViewModel?.CreditRequestComboboxChangedCommand.Execute(dataSource);
            }
        }

        private void UnitPriceTextEdit_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var senderName = (TextEdit)sender;
            var dataCxtx = senderName.DataContext;
            var dataSource = (OrderDetailUIModel)dataCxtx;
            if (!string.IsNullOrEmpty(senderName.Text))
            {
                var priceEntered = senderName.Text.Replace(@"$", "");
                var finalstring = Regex.Replace(priceEntered, @"\s+", "");
                double temp;
                bool isOk = double.TryParse(finalstring, out temp);
                int value = isOk ? (int)temp : 0;
                if (isOk)
                {
                    dataSource.Price = value;
                    ViewModel?.PriceChangedCommand.Execute(dataSource);
                }
            }
        }

        private void CustomKeypad_DoneClickEvent(object sender, bool e)
        {
            QuantityCustomKeyPadFlyout.Hide();
        }

        private void QuantityCustomKeyPadFlyout_Closing(FlyoutBase sender, FlyoutBaseClosingEventArgs args)
        {
            if (string.IsNullOrEmpty(ViewModel?.CartDetailModel.OrderDetailModel?.QuantityDisplay))
            {
                ViewModel.CartDetailModel.OrderDetailModel.QuantityDisplay = ViewModel?.quantityBeforeEdit;
                if (string.IsNullOrEmpty(ViewModel?.CartDetailModel.OrderDetailModel?.QuantityDisplay))
                {
                    ViewModel.CartDetailModel.OrderDetailModel.Quantity = Convert.ToInt32(ViewModel?.CartDetailModel.OrderDetailModel?.QuantityDisplay);
                }

            }
            ViewModel?.QuantityChangedCommand.Execute(ViewModel?.CartDetailModel.OrderDetailModel);
        }

        private void Flyout_Closing(FlyoutBase sender, FlyoutBaseClosingEventArgs args)
        {
            ViewModel.priceString = string.Empty;
            ViewModel?.PriceChangedCommand.Execute(ViewModel.CartDetailModel.OrderDetailModel);
        }

        private void CustomKeypad_DoneClickEvent_1(object sender, bool e)
        {
            PriceKeyPadFlyout.Hide();
        }

        private void quantityTextBlock_GotFocus(object sender, RoutedEventArgs e)
        {
            var senderName = (TextBox)sender;
            var dataCxtx = senderName.DataContext;
            var dataSource = (OrderDetailUIModel)dataCxtx;
            ViewModel.isOpenedFrom = "quantity";
            ViewModel.CartDetailModel.OrderDetailModel = dataSource;
            ViewModel.quantityBeforeEdit = dataSource.QuantityDisplay;
            FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
        }

        private void UnitPriceTextEdit_GotFocus(object sender, RoutedEventArgs e)
        {
            var senderName = (TextEdit)sender;
            var dataCxtx = senderName.DataContext;
            var dataSource = (OrderDetailUIModel)dataCxtx;
            ViewModel.isOpenedFrom = "price";
            ViewModel.CartDetailModel.OrderDetailModel = dataSource;
            FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
        }

        private void rtnquantityTextBlock_BeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            if (args.NewText.Trim().Length == 1 && args.NewText.Trim().Equals("0"))
            {
                args.Cancel = true;
            }           
        }

        private void RtnQuantityTextBlock_GotFocus(object sender, RoutedEventArgs e)
        {
            var senderName = (TextBox)sender;
            var dataCxtx = senderName.DataContext;
            var dataSource = (OrderDetailUIModel)dataCxtx;
            ViewModel.isOpenedFrom = "quantity";
            ViewModel.IsOpenedForCRQuantity = "RtnQuantity";
            ViewModel.CartDetailModel.OrderDetailModel = dataSource;
            ViewModel.quantityBeforeEdit = dataSource.QuantityDisplay;
            FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
        }

        private void DIFQuantityTextBlock_GotFocus(object sender, RoutedEventArgs e)
        {
            var senderName = (TextBox)sender;
            var dataCxtx = senderName.DataContext;
            var dataSource = (OrderDetailUIModel)dataCxtx;
            ViewModel.isOpenedFrom = "quantity";
            ViewModel.IsOpenedForCRQuantity = "DifQuantity";
            ViewModel.CartDetailModel.OrderDetailModel = dataSource;
            ViewModel.quantityBeforeEdit = dataSource.QuantityDisplay;
            FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
        }

        private void DifUnitPrice_GotFocus(object sender, RoutedEventArgs e)
        {
            var senderName = (TextEdit)sender;
            var dataCxtx = senderName.DataContext;
            var dataSource = (OrderDetailUIModel)dataCxtx;
            ViewModel.isOpenedFrom = "price";
            ViewModel.IsOpenedForCRUnitPrice = "DifPrice";
            ViewModel.CartDetailModel.OrderDetailModel = dataSource;
            FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
        }

        private void RtnUnitPrice_GotFocus(object sender, RoutedEventArgs e)
        {
            var senderName = (TextEdit)sender;
            var dataCxtx = senderName.DataContext;
            var dataSource = (OrderDetailUIModel)dataCxtx;
            ViewModel.isOpenedFrom = "price";
            ViewModel.IsOpenedForCRUnitPrice = "RtnPrice";
            ViewModel.CartDetailModel.OrderDetailModel = dataSource;
            FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
        }
    }
}
