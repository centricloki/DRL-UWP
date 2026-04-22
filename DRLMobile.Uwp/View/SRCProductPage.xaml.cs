using DevExpress.Mvvm.Native;
using DevExpress.UI.Xaml.Editors;

using DRLMobile.Core.Models.UIModels;
using DRLMobile.ExceptionHandler;
using DRLMobile.Uwp.Helpers;
using DRLMobile.Uwp.ViewModel;

using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DRLMobile.Uwp.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SRCProductPage : Page
    {
        private SrcProductPageViewModel SrcProductPageViewModel = new SrcProductPageViewModel();

        #region Constructor
        public SRCProductPage()
        {
            InitializeComponent();
            DataContext = SrcProductPageViewModel;
            //this.Loaded += SRCProductPage_Loaded;
            this.Loaded += async (object sender, RoutedEventArgs e) =>
            {
                ShellPage shellPage = ((Window.Current.Content as Frame).Content as ShellPage);
                if (shellPage != null)
                {
                    shellPage.ViewModel.IsSideMenuItemClickable = false;
                }
                SrcProductPageViewModel.LoadingVisibility = Visibility.Visible;
                try
                {
                    await SrcProductPageViewModel?.PageLoadedCommand.ExecuteAsync(null);
                }
                catch (Exception ex)
                {
                    ErrorLogger.WriteToErrorLog(nameof(SRCProductPage), "SRCProductPage_Loaded", ex);
                }
                finally
                {
                    SrcProductPageViewModel.LoadingVisibility = Visibility.Collapsed;
                    if (shellPage != null)
                    {
                        shellPage.ViewModel.IsSideMenuItemClickable = true;
                    }
                }
            };
        }
        #endregion

        public bool IsNotEmpty(string args) => !string.IsNullOrWhiteSpace(args);
        public BitmapImage GetSortImagePath(string args) => new BitmapImage(new Uri(String.Format("ms-appx:///Assets/Controls/{0}.png", args)));
        private Visibility InvertVisibility(Visibility value) { return (value == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible); }
        private bool InvertBool(bool value) => !value;
        private async void Grid_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            var senderName = (Grid)sender;
            var dataCxtx = senderName.DataContext;
            var dataSource = (SRCProductUIModel)dataCxtx;
            await SrcProductPageViewModel?.CartImageCommand.ExecuteAsync(dataSource);
        }

        private async void CategoryRelativePanel_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            SrcProductPageViewModel.LoadingVisibility = Visibility.Visible;
            var senderName = (RelativePanel)sender;
            var dataContext = senderName.DataContext;
            CategoryUIModel dataSource = (CategoryUIModel)dataContext;
            dataSource.IsSelected = !dataSource.IsSelected;
            await SrcProductPageViewModel?.CategoryClickedCommand.ExecuteAsync(dataSource);
            SrcProductPageViewModel.LoadingVisibility = Visibility.Collapsed;
        }
        private async void BrandGrid_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            SrcProductPageViewModel.LoadingVisibility = Visibility.Visible;
            var senderName = (Grid)sender;
            var dataContext = senderName.DataContext;
            var dataSource = (BrandUIModel)dataContext;
            dataSource.IsSelected = !dataSource.IsSelected;
            await SrcProductPageViewModel?.BrandClickedCommand.ExecuteAsync(dataSource);
            SrcProductPageViewModel.LoadingVisibility = Visibility.Collapsed;
        }

        private async void StyleRelativePanel_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            SrcProductPageViewModel.LoadingVisibility = Visibility.Visible;
            var senderName = (RelativePanel)sender;
            var dataContext = senderName.DataContext;
            var dataSource = (StyleUIModel)dataContext;
            dataSource.IsSelected = !dataSource.IsSelected;
            await SrcProductPageViewModel?.StyleClickedCommand.ExecuteAsync(dataSource);
            SrcProductPageViewModel.LoadingVisibility = Visibility.Collapsed;
        }

        private async void DistributionGrid_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            var senderName = (Grid)sender;
            var dataCxtx = senderName.DataContext;
            var dataSource = (SRCProductUIModel)dataCxtx;
            await SrcProductPageViewModel?.DistributionImageCommand.ExecuteAsync(dataSource);
        }

        private async void ListIconGrid_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            var senderName = (Grid)sender;
            var dataCxtx = senderName.DataContext;
            var dataSource = (SrcProductPageViewModel)dataCxtx;
            await SrcProductPageViewModel?.ListIconClickedCommand.ExecuteAsync(null);
        }

        private async void LeftProductDetailArrowGrid_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            await SrcProductPageViewModel?.LeftArrowClickCommand.ExecuteAsync(null);
        }

        private async void RightProductDetailArrowGrid_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            await SrcProductPageViewModel?.RightArrowClickCommand.ExecuteAsync(null);
        }

        //private void quantityTextBlock_BeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        //{
        //    if (args.NewText.Trim().Length == 1 && args.NewText.Trim().Equals("0"))
        //    {
        //        args.Cancel = true;
        //    }
        //    else
        //    {
        //        args.Cancel = args.NewText.Any(c => !char.IsDigit(c));
        //    }
        //}

        private void FavoriteGrid_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            var senderName = (Grid)sender;
            var dataCxtx = senderName.DataContext;
            var dataSource = (SRCProductUIModel)dataCxtx;
            SrcProductPageViewModel?.FavoriteImageCommand.Execute(dataSource);
        }

        private async void FavoriteSalesDocs_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            var senderName = (Grid)sender;
            var dataCxtx = senderName.DataContext;
            var dataSource = (SRCProductUIModel)dataCxtx;
            await SrcProductPageViewModel?.FavoriteSalesDocsImageCommand.ExecuteAsync(dataSource);
        }

        //private void quantityTextBlock_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
        //{
        //    char[] chars = { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        //    sender.Text = new string(sender.Text.Where(c => chars.Contains(c)).ToArray());

        //}

        private async void PdfClicked(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            await SrcProductPageViewModel?.PdfFactsheetCommand?.ExecuteAsync((sender as Grid).DataContext as SRCProductUIModel);
        }

        private void CustomKeypad_DoneClickEvent(object sender, bool e)
        {
            QuantityCustomKeyPadFlyout.Hide();
        }

        private void QuantityCustomKeyPadFlyout_Closing(FlyoutBase sender, FlyoutBaseClosingEventArgs args)
        {
            if (string.IsNullOrEmpty(SrcProductPageViewModel.ProductDetailModel?.QuantityDisplay))
            {
                SrcProductPageViewModel.ProductDetailModel.QuantityDisplay = SrcProductPageViewModel?.quantityBeforeEdit;
            }
            if (SrcProductPageViewModel.ProductDetailModel.QuantityDisplay == "-")
            {
                SrcProductPageViewModel.ProductDetailModel.QuantityDisplay = "-1";
            }

            if (!string.IsNullOrEmpty(SrcProductPageViewModel.ProductDetailModel?.QuantityDisplay))
            {
                SrcProductPageViewModel.ProductDetailModel.Quantity = Convert.ToInt32(SrcProductPageViewModel.ProductDetailModel?.QuantityDisplay);
                SrcProductPageViewModel.quantityBeforeEdit = SrcProductPageViewModel.ProductDetailModel.QuantityDisplay;
            }
            SrcProductPageViewModel.quantityString = string.Empty;
            SrcProductPageViewModel?.QuantityChangedCommand.Execute(null);
        }

        //private void DataGridcontrol_EndSorting(object sender, EventArgs e)
        //{
        //    if (DataGridcontrol != null)
        //        DataGridcontrol.SelectedItem = null;
        //}

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            if (SrcProductPageViewModel.PrintHelper != null)
            {
                SrcProductPageViewModel.PrintHelper.UnregisterForPrinting();
            }
        }

        private async void RetailImageButton_Click(object sender, RoutedEventArgs e)
        {
            await SrcProductPageViewModel.RetailImageClickCommand.ExecuteAsync(e);

            // Initalize receipt print helper class and register for printing
            if (SrcProductPageViewModel.PrintHelper != null)
            {
                SrcProductPageViewModel.PrintHelper.UnregisterForPrinting();
                SrcProductPageViewModel.PrintHelper = null;
            }

            if (SrcProductPageViewModel.PrintHelper == null)
            {
                SrcProductPageViewModel.PrintHelper = new PhotosPrintHelper(this, SrcProductPageViewModel.PreviewUrl);

                SrcProductPageViewModel.PrintHelper.RegisterForPrinting("SRCProductPage");
            }
        }

        private async void ProductImageButton_Click(object sender, RoutedEventArgs e)
        {
            await SrcProductPageViewModel.ProductImageClickCommand.ExecuteAsync(e);

            // Initalize receipt print helper class and register for printing
            if (SrcProductPageViewModel.PrintHelper != null)
            {
                SrcProductPageViewModel.PrintHelper.UnregisterForPrinting();
                SrcProductPageViewModel.PrintHelper = null;
            }

            if (SrcProductPageViewModel.PrintHelper == null)
            {
                SrcProductPageViewModel.PrintHelper = new PhotosPrintHelper(this, SrcProductPageViewModel.PreviewUrl);

                SrcProductPageViewModel.PrintHelper.RegisterForPrinting("SRCProductPage");
            }
        }

        //private void QuantityEditText_GotFocus(object sender, RoutedEventArgs e)
        //{
        //    SrcProductPageViewModel.quantityBeforeEdit = SrcProductPageViewModel.ProductDetailModel.QuantityDisplay;

        //    FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
        //}

        //private async void QuantityEditText_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        //{
        //    var senderName = (TextEdit)sender;

        //    if (!string.IsNullOrEmpty(senderName.Text))
        //    {
        //        if (senderName.Text != "-")
        //        {
        //            SrcProductPageViewModel.ProductDetailModel.QuantityDisplay = senderName.Text;

        //            await SrcProductPageViewModel.QuantityChangedCommand.ExecuteAsync(null);
        //        }
        //    }
        //}

        private void quantityImage_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e) => ShowFlyout(sender);

        private void TextBlock_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            var senderName = (TextBlock)sender;
            var dataCxtx = senderName.DataContext;
            SRCProductUIModel dataSource = (SRCProductUIModel)dataCxtx;
            if (dataSource.CatId != -99 && !SrcProductPageViewModel.IsProductDetailLoad)
            {
                SrcProductPageViewModel?.ItemSelectedCommand.Execute(dataSource);
            }
        }

        private void ShowFlyout(object sender)
        {
            //Image imgCntrl = sender as Image;
            //var dataCxtx = imgCntrl.DataContext;
            //var dataSource = (OrderDetailUIModel)dataCxtx;

            SrcProductPageViewModel.quantityBeforeEdit = SrcProductPageViewModel.ProductDetailModel.QuantityDisplay;

            FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);

        }


        private void OnContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
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