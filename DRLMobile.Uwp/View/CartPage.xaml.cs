using DevExpress.UI.Xaml.Editors;

using DRLMobile.Core.Models.UIModels;
using DRLMobile.ExceptionHandler;
using DRLMobile.Uwp.ViewModel;

using MsgKit;

using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Windows.UI.ViewManagement.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DRLMobile.Uwp.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CartPage : Page
    {
        #region properties
        private CartPageViewModel ViewModel;
        #endregion

        public CartPage()
        {
            this.InitializeComponent();
            DataContext = ViewModel = new CartPageViewModel();
            this.Loaded += CartPage_Loaded;
            this.Unloaded += CartPage_Unloaded;
        }

        private async void CartPage_Unloaded(object sender, RoutedEventArgs e)
        {
            if (ViewModel != null)
            {
                if (!ViewModel.isTriggerConfirmOrder)
                    await ViewModel.SaveCartItems();
                ViewModel = null;
            }
        }

        private async void CartPage_Loaded(object sender, RoutedEventArgs e)
        {
            await ViewModel.LoadInitialPageData();
        }

        private void Delete_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            var senderName = (Grid)sender;
            var dataCxtx = senderName.DataContext;
            var dataSource = (OrderDetailUIModel)dataCxtx;
            ViewModel?.DeleteImageCommand.Execute(dataSource);
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

        private void UnitComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var senderName = (ComboBox)sender;
            var dataCxtx = senderName.DataContext;
            var dataSource = (OrderDetailUIModel)dataCxtx;
            if (!string.IsNullOrEmpty(senderName.SelectedItem?.ToString()))
            {
                dataSource.Unit = senderName.SelectedItem?.ToString();
                //ViewModel?.UnitComboboxChangedCommand.Execute(dataSource);
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

        private void quantityImage_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e) => ShowFlyout(sender);
        private void unitPriceImage_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e) => ShowFlyout(sender);
        private void ShowFlyout(object sender)
        {
            Image imgCntrl = sender as Image;
            var dataCxtx = imgCntrl.DataContext;
            var dataSource = (OrderDetailUIModel)dataCxtx;

            switch (imgCntrl.Name.ToLower())
            {
                #region Quantity Image Button
                case "quantityimage":
                case "nontobaccoquantityimage":
                    ViewModel.isOpenedFrom = "quantity";
                    ViewModel.quantityBeforeEdit = dataSource.QuantityDisplay;
                    break;
                case "rtnquantityimage":
                    ViewModel.isOpenedFrom = "quantity";
                    ViewModel.IsOpenedForCRQuantity = "RtnQuantity";
                    ViewModel.quantityBeforeEdit = dataSource.QuantityDisplay;
                    break;
                case "difquantityimage":
                    ViewModel.isOpenedFrom = "quantity";
                    ViewModel.IsOpenedForCRQuantity = "DifQuantity";
                    ViewModel.quantityBeforeEdit = dataSource.QuantityDisplay;
                    break;
                #endregion

                #region Unit Price Image Button
                case "unitpriceimage":
                case "nontobaccounitpriceimage":
                    ViewModel.isOpenedFrom = "price";
                    ViewModel.priceBeforeEdit = dataSource.PriceDisplay;
                    break;
                case "rtnunitpriceimage":
                    ViewModel.isOpenedFrom = "price";
                    ViewModel.IsOpenedForCRUnitPrice = "RtnPrice";
                    ViewModel.priceBeforeEdit = dataSource.PriceDisplay;
                    break;
                case "difunitpriceimage":
                    ViewModel.isOpenedFrom = "price";
                    ViewModel.IsOpenedForCRUnitPrice = "DifPrice";
                    ViewModel.priceBeforeEdit = dataSource.PriceDisplay;
                    break;
                #endregion

                default:
                    break;
            }
            string upperDisplay = ViewModel.CartDetailModel.OrderDetailModel.SubTotalUpperDisplay;
            string lowerDisplay = ViewModel.CartDetailModel.OrderDetailModel.SubTotalLowerDisplay;
            string grandDisplay = ViewModel.CartDetailModel.OrderDetailModel.GrandTotalDisplay;
            ViewModel.CartDetailModel.OrderDetailModel = dataSource;
            ViewModel.CartDetailModel.OrderDetailModel.SubTotalUpperDisplay = upperDisplay;
            ViewModel.CartDetailModel.OrderDetailModel.SubTotalLowerDisplay = lowerDisplay;
            ViewModel.CartDetailModel.OrderDetailModel.GrandTotalDisplay = grandDisplay;
            FlyoutBase.ShowAttachedFlyout(imgCntrl as FrameworkElement);
        }
        private void CustomKeypad_DoneClickEvent(object sender, bool e)
        {
            QuantityCustomKeyPadFlyout.Hide();
        }

        private void QuantityCustomKeyPadFlyout_Closing(FlyoutBase sender, FlyoutBaseClosingEventArgs args)
        {
            if (ViewModel != null)
            {
                ViewModel.quantityString = string.Empty;
                ViewModel.QuantityChangedCommand.Execute(ViewModel?.CartDetailModel.OrderDetailModel);
            }
        }
        private void Flyout_Closing(FlyoutBase sender, FlyoutBaseClosingEventArgs args)
        {
            if (ViewModel != null)
            {
                ViewModel.priceString = string.Empty;
                ViewModel.PriceChangedCommand.Execute(ViewModel.CartDetailModel.OrderDetailModel);
            }
        }

        private void CustomKeypad_DoneClickEvent_1(object sender, bool e)
        {
            PriceKeyPadFlyout.Hide();
        }
    }


}