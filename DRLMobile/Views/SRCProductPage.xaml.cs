using DRLMobile.Core.Models.UIModels;
using DRLMobile.ViewModels;
using System;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DRLMobile.Views
{
    public sealed partial class SRCProductPage : Page
    {
        private SrcProductPageViewModel SrcProductPageViewModel = new SrcProductPageViewModel();

        #region Constructor
        public SRCProductPage()
        {
            InitializeComponent();

            DataContext = SrcProductPageViewModel;
        }
        #endregion

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            SrcProductPageViewModel?.PageLoadedCommand.Execute(DataGridcontrol);
        }

        //private void Page_Loaded(object sender, RoutedEventArgs e)
        //{
        //    SrcProductPageViewModel?.PageLoadedCommand.Execute(DataGridcontrol);
        //   // SrcProductPageViewModel?.OtherCategoryClickedCommand.Execute(DataGridcontrol);

        //}
        private void Grid_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            var senderName = (Grid)sender;
            var dataCxtx = senderName.DataContext;
            var dataSource = (SRCProductUIModel)dataCxtx;
            SrcProductPageViewModel?.CartImageCommand.Execute(dataSource);

        }

        private void CategoryRelativePanel_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            var senderName = (RelativePanel)sender;
            var dataContext = senderName.DataContext;
            var dataSource = (CategoryUIModel)dataContext;
            SrcProductPageViewModel?.CategoryClickedCommand.Execute(dataSource);
            if (dataSource.CategoryId == -99)

            {
                SrcProductPageViewModel?.SalesDocsClickedCommand.Execute(DataGridcontrol);
            }
            else
            {
                SrcProductPageViewModel?.OtherCategoryClickedCommand.Execute(DataGridcontrol);
            }
        }

        private void StyleRelativePanel_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            var senderName = (RelativePanel)sender;
            var dataContext = senderName.DataContext;
            var dataSource = (StyleUIModel)dataContext;
            SrcProductPageViewModel?.StyleClickedCommand.Execute(dataSource);

        }

        private void BrandGrid_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            var senderName = (Grid)sender;
            var dataContext = senderName.DataContext;
            var dataSource = (BrandUIModel)dataContext;
            SrcProductPageViewModel?.BrandClickedCommand.Execute(dataSource);
        }

        private void DistributionGrid_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            var senderName = (Grid)sender;
            var dataCxtx = senderName.DataContext;
            var dataSource = (SRCProductUIModel)dataCxtx;
            SrcProductPageViewModel?.DistributionImageCommand.Execute(dataSource);
        }

        private void ListIconGrid_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            var senderName = (Grid)sender;
            var dataCxtx = senderName.DataContext;
            var dataSource = (SrcProductPageViewModel)dataCxtx;
            SrcProductPageViewModel?.ListIconClickedCommand.Execute(null);
        }

        private void LeftProductDetailArrowGrid_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            SrcProductPageViewModel?.LeftArrowClickCommand.Execute(null);
        }

        private void RightProductDetailArrowGrid_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            SrcProductPageViewModel?.RightArrowClickCommand.Execute(null);

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

        private void FavoriteGrid_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            var senderName = (Grid)sender;
            var dataCxtx = senderName.DataContext;
            var dataSource = (SRCProductUIModel)dataCxtx;
            SrcProductPageViewModel?.FavoriteImageCommand.Execute(dataSource);
        }

        private void FavoriteSalesDocs_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            var senderName = (Grid)sender;
            var dataCxtx = senderName.DataContext;
            var dataSource = (SRCProductUIModel)dataCxtx;
            SrcProductPageViewModel?.FavoriteSalesDocsImageCommand.Execute(dataSource);
        }

        private void quantityTextBlock_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
        {
            char[] chars = { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            sender.Text = new string(sender.Text.Where(c => chars.Contains(c)).ToArray());

        }

        private void PdfClicked(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            SrcProductPageViewModel?.PdfFactsheetCommand?.Execute((sender as Grid).DataContext as SRCProductUIModel);
        }

        private void quantityTextBlock_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var senderName = (TextBox)sender;
            if (!string.IsNullOrEmpty(senderName.Text))
            {
                SrcProductPageViewModel.ProductDetailModel.QuantityDisplay = senderName.Text;
            }
            else
            {
                SrcProductPageViewModel.ProductDetailModel.QuantityDisplay = "1";
            }
        }
        private void CustomKeypad_DoneClickEvent(object sender, bool e)
        {
            QuantityCustomKeyPadFlyout.Hide();

        }
        private void quantityTextBlock_GotFocus(object sender, RoutedEventArgs e)
        {
           // FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
        }

        private void quantityTextBlock_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);

        }
    }

}
