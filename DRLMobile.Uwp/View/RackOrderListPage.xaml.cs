using DRLMobile.Core.Models.UIModels;
using DRLMobile.Uwp.Helpers;
using DRLMobile.Uwp.ViewModel;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DRLMobile.Uwp.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RackOrderListPage : Page
    {
        private RackOrderListPageViewModel RackOrderListPageViewModel = new RackOrderListPageViewModel();

        private bool isImageTapped = false;

        #region Constructor
        public RackOrderListPage()
        {
            this.InitializeComponent();
            DataContext = RackOrderListPageViewModel;

        }
        #endregion

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            RackOrderListPageViewModel?.PageLoadedCommand.Execute(null);
        }


        //private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        //{
        //    var rackOrderUiModel = (RackOrderUiModel)e.ClickedItem;
        //    RackOrderListPageViewModel?.NavigateToRackCartScreenCommand.Execute(rackOrderUiModel);
        //}

        private async void ProductImage_Tapped(object sender, TappedRoutedEventArgs e)
        {
            isImageTapped = true;

            if (sender is Image)
            {
                var dataContext = (sender as Image).DataContext as RackOrderUiModel;

                if (dataContext != null && !string.IsNullOrEmpty(dataContext.ProductImagePath) && !dataContext.ProductImagePath.Equals((string)Application.Current.Resources["PlaceholderImage"]))
                {
                    RackOrderListPageViewModel.PreviewUrl = dataContext.ProductImagePath;
                    RackOrderListPageViewModel.IsPreviewDocumentVisibile = true;

                    if (RackOrderListPageViewModel.PrintHelper == null)
                    {
                        // Initalize receipt print helper class and register for printing
                        RackOrderListPageViewModel.PrintHelper = new PhotosPrintHelper(this, RackOrderListPageViewModel.PreviewUrl);

                        RackOrderListPageViewModel.PrintHelper.RegisterForPrinting("RackOrderPage");
                    }
                }
                else
                {
                    RackOrderListPageViewModel?.NavigateToRackCartScreenCommand.Execute(dataContext);
                }
            }

            await Task.Delay(500);
            isImageTapped = false;
        }

        private void RackItemClicked(object sender, TappedRoutedEventArgs e)
        {
            if (!isImageTapped && sender is StackPanel)
            {
                var dataContext = (sender as StackPanel).DataContext as RackOrderUiModel;
                RackOrderListPageViewModel?.NavigateToRackCartScreenCommand.Execute(dataContext);
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);

            if (RackOrderListPageViewModel.PrintHelper != null)
            {
                RackOrderListPageViewModel.PrintHelper.UnregisterForPrinting();

                RackOrderListPageViewModel.PrintHelper = null;
            }
        }
    }
}
