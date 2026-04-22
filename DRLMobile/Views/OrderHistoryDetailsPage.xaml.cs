using DevExpress.UI.Xaml.Editors;
using DRLMobile.ViewModels;
using System;
using System.Linq;
using System.Text.RegularExpressions;
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
    public sealed partial class OrderHistoryDetailsPage : Page
    {

        private readonly OrderHistoryDetailsPageViewModel ViewModel = new OrderHistoryDetailsPageViewModel();

        public OrderHistoryDetailsPage()
        {
            this.InitializeComponent();
            DataContext = ViewModel;
        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.NavigationMode == NavigationMode.New)
            {
                ViewModel.OnNavigatedToCommand.Execute(e.Parameter);
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

        private void quantityTextBlock_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            var senderName = (TextBox)sender;
            if (!string.IsNullOrEmpty(senderName.Text))
            {
                var qty = Convert.ToInt32(senderName.Text);
                ViewModel?.QuantityChangedCommand.Execute(qty);
            }
        }

        private void quantityTextBlock_GotFocus(object sender, RoutedEventArgs e)
        {
            ViewModel.IsOpenedFrom = "quantity";
            FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
        }

        private void QuantityCustomKeyPadFlyout_Closing(FlyoutBase sender, FlyoutBaseClosingEventArgs args)
        {
            //if (string.IsNullOrEmpty(ViewModel?.OrderDetailNumPadModel?.QuantityDisplay))
            //{
            //    ViewModel.OrderDetailNumPadModel.QuantityDisplay = ViewModel?.quantityBeforeEdit;
            //    if (string.IsNullOrEmpty(ViewModel?.OrderDetailNumPadModel?.QuantityDisplay))
            //    {
            //        ViewModel.OrderDetailNumPadModel.Quantity = Convert.ToInt32(ViewModel?.OrderDetailNumPadModel?.QuantityDisplay);
            //    }

            //}
            //ViewModel?.QuantityChangedCommand.Execute(ViewModel?.OrderDetailNumPadModel);
        }

        private void CustomKeypad_DoneClickEvent(object sender, bool e)
        {
            PriceKeyPadFlyout.Hide();
        }

        private void UnitComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void CustomKeypad_DoneClickEvent_1(object sender, bool e)
        {
            PriceKeyPadFlyout.Hide();
        }

        private void Flyout_Closing(FlyoutBase sender, FlyoutBaseClosingEventArgs args)
        {
            //ViewModel.priceString = string.Empty;
            //ViewModel?.PriceChangedCommand.Execute(ViewModel.OrderDetailNumPadModel);
        }

        private void nonTobaccoTextEdit_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            var senderName = (TextEdit)sender;
            if (!string.IsNullOrEmpty(senderName.Text))
            {
                var priceEntered = senderName.Text.Replace(@"$", "");
                var finalstring = Regex.Replace(priceEntered, @"\s+", "");
                double temp;
                bool isOk = double.TryParse(finalstring, out temp);
                int value = isOk ? (int)temp : 0;
                if (isOk)
                {
                    ViewModel?.PriceChangedCommand.Execute(temp);
                }
            }
        }

        private void nonTobaccoTextEdit_GotFocus(object sender, RoutedEventArgs e)
        {
            ViewModel.IsOpenedFrom = "price";
            FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
        }

    }
}
