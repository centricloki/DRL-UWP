using DRLMobile.Core.Models.UIModels;
using DRLMobile.Services;
using DRLMobile.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DRLMobile.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RackOrderListPage : Page
    {
        private RackOrderListPageViewModel RackOrderListPageViewModel = new RackOrderListPageViewModel();

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
    

        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var rackOrderUiModel = (RackOrderUiModel)e.ClickedItem;
            RackOrderListPageViewModel?.NavigateToRackCartScreenCommand.Execute(rackOrderUiModel);
        }
    }
}
