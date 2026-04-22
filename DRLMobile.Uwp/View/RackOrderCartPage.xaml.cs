using DevExpress.UI.Xaml.Editors;
using DRLMobile.Core.Models.UIModels;
using DRLMobile.Uwp.ViewModel;
using System;
using System.Linq;
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
    public sealed partial class RackOrderCartPage : Page
    {
        private RackOrderCartPageViewModel RackOrderCartPageViewModel = new RackOrderCartPageViewModel();

        #region Constructor
        public RackOrderCartPage()
        {
            this.InitializeComponent();
            DataContext = RackOrderCartPageViewModel;

        }
        #endregion

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            RackOrderCartPageViewModel?.PageLoadedCommand.Execute(e.Parameter);
        }
        private void quantityTextBlock_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var senderName = (TextBox)sender;

            if (!string.IsNullOrEmpty(senderName.Text))
            {
                RackOrderCartPageViewModel.RackOrderCartUIModel.QuantityDisplay = senderName.Text;
            }
            else
            {
                RackOrderCartPageViewModel.RackOrderCartUIModel.QuantityDisplay = RackOrderCartPageViewModel.HeaderQuantityBeforeEdit;
            }
            RackOrderCartPageViewModel.HeaderQuantityChangedCommand.Execute(null);
        }

        private void CustomKeypad_DoneClickEvent(object sender, bool e)
        {
            QuantityCustomKeyPadFlyout.Hide();

        }
        private void QuantityTextBlock_GotFocus(object sender, RoutedEventArgs e)
        {
            RackOrderCartPageViewModel.HeaderQuantityBeforeEdit = RackOrderCartPageViewModel.RackOrderCartUIModel.QuantityDisplay;
            FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
        }
        private void QuantityCustomKeyPadFlyout_Closing_1(FlyoutBase sender, FlyoutBaseClosingEventArgs args)
        {
            if (string.IsNullOrEmpty(RackOrderCartPageViewModel?.RackOrderCartUIModel?.QuantityDisplay))
            {
                RackOrderCartPageViewModel.RackOrderCartUIModel.QuantityDisplay = RackOrderCartPageViewModel?.HeaderQuantityBeforeEdit;
                if (!string.IsNullOrEmpty(RackOrderCartPageViewModel?.RackOrderCartUIModel?.QuantityDisplay))
                {
                    RackOrderCartPageViewModel.RackOrderCartUIModel.Quantity = Convert.ToInt32(RackOrderCartPageViewModel?.RackOrderCartUIModel?.QuantityDisplay);
                }

            }
            RackOrderCartPageViewModel?.HeaderQuantityChangedCommand.Execute(null);
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

        private void quantityGridTextBlock_GotFocus(object sender, RoutedEventArgs e)
        {
            var senderName = (TextBox)sender;
            var dataCxtx = senderName.DataContext;
            var dataSource = (RackOrderCartUiModel)dataCxtx;
            RackOrderCartPageViewModel.GridItemModel = dataSource;
            RackOrderCartPageViewModel.quantityBeforeEdit = dataSource.QuantityDisplay;
            FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
        }

        private void QuantityTextBlock_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var senderName = (TextBox)sender;
            var dataCxtx = senderName.DataContext;
            var dataSource = (RackOrderCartUiModel)dataCxtx;
            if (!string.IsNullOrEmpty(senderName.Text))
            {
                dataSource.Quantity = Convert.ToInt32(senderName.Text);
            }
            RackOrderCartPageViewModel.QuantityChangedCommand.Execute(dataSource);

        }
        private void CustomKeypad_DoneClickEvent1(object sender, bool e)
        {
            QuantityGridCustomKeyPadFlyout.Hide();
        }

        private void QuantityCustomKeyPadFlyout_Closing(FlyoutBase sender, FlyoutBaseClosingEventArgs args)
        {
            //if (string.IsNullOrEmpty(RackOrderCartPageViewModel?.GridItemModel?.QuantityDisplay))
            //{
            //    RackOrderCartPageViewModel.GridItemModel.QuantityDisplay = RackOrderCartPageViewModel?.quantityBeforeEdit;
            //    if (!string.IsNullOrEmpty(RackOrderCartPageViewModel?.GridItemModel?.QuantityDisplay))
            //    {
            //        RackOrderCartPageViewModel.GridItemModel.Quantity = Convert.ToInt32(RackOrderCartPageViewModel?.GridItemModel?.QuantityDisplay);
            //    }
            //}

            RackOrderCartPageViewModel.quantityString = string.Empty;

            RackOrderCartPageViewModel?.QuantityChangedCommand.Execute(RackOrderCartPageViewModel.GridItemModel);
        }

        private void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var senderName = (Grid)sender;
            var dataCxtx = senderName.DataContext;
            var dataSource = (RackOrderCartUiModel)dataCxtx;
            RackOrderCartPageViewModel?.CartImageCommand.Execute(dataSource);
        }

        private void Grid_Tapped_1(object sender, TappedRoutedEventArgs e)
        {
            RackOrderCartPageViewModel?.CartImageCommand.Execute(RackOrderCartPageViewModel.RackOrderCartUIModel);
        }

        private void QuantityEditText_GotFocus(object sender, RoutedEventArgs e)
        {
            var senderName = (TextEdit)sender;
            var dataCxtx = senderName.DataContext;
            var dataSource = (RackOrderCartUiModel)dataCxtx;

            RackOrderCartPageViewModel.GridItemModel = dataSource;
            RackOrderCartPageViewModel.quantityBeforeEdit = dataSource.QuantityDisplay;

            FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
        }

        private void QuantityEditText_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if(string.IsNullOrWhiteSpace (headerQuantity.Text))
            {
                headerQuantity.Text = RackOrderCartPageViewModel.HeaderQuantityBeforeEdit;
            }

            var senderName = (TextEdit)sender;
            var dataCxtx = senderName.DataContext;
            var dataSource = (RackOrderCartUiModel)dataCxtx;

            if (!string.IsNullOrEmpty(senderName.Text))
            {
                dataSource.Quantity = Convert.ToInt32(senderName.Text);
            }
            RackOrderCartPageViewModel.quantityString = "";
            RackOrderCartPageViewModel.QuantityChangedCommand.Execute(dataSource);
        }
    }
}
