using DRLMobile.Core.Models.UIModels;
using DRLMobile.ViewModels;
using System.Linq;
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
    public sealed partial class PopOrderPage : Page
    {
        private PopOrderPageViewModel PopOrderViewModel = new PopOrderPageViewModel();

        public PopOrderPage()
        {
            this.InitializeComponent();
            DataContext = PopOrderViewModel;
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            PopOrderViewModel?.PageLoadedCommand.Execute(null);
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
            var dataSource = (PopOrderCartUiModel)dataCxtx;
            PopOrderViewModel.GridItemModel = dataSource;
            //RackOrderCartPageViewModel.quantityBeforeEdit = dataSource.QuantityDisplay;
            FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
        }

        private void QuantityTextBlock_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            var senderName = (TextBox)sender;
            if (!string.IsNullOrEmpty(senderName.Text))
            {
                //  RackOrderCartPageViewModel.RackOrderCartUIModel.QuantityDisplay = senderName.Text;
            }
            else
            {
                //  RackOrderCartPageViewModel.RackOrderCartUIModel.QuantityDisplay = "1";
            }
        }

        private void CustomKeypad_DoneClickEvent1(object sender, bool e)
        {
            QuantityGridCustomKeyPadFlyout.Hide();
        }

        private void QuantityCustomKeyPadFlyout_Closing(FlyoutBase sender, FlyoutBaseClosingEventArgs args)
        {

        }

        private void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var senderName = (Grid)sender;
            var dataCxtx = senderName.DataContext;
            var dataSource = (PopOrderCartUiModel)dataCxtx;
            PopOrderViewModel?.CartImageCommand.Execute(dataSource);
        }
    }
}
