using DRLMobile.Core.Models.UIModels;
using DRLMobile.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DRLMobile.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FavoritePage : Page
    {
        private FavoritePageViewModel FavoritePageViewModel = new FavoritePageViewModel();

        public FavoritePage()
        {
            this.InitializeComponent();
            DataContext = FavoritePageViewModel;

        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            FavoritePageViewModel?.OnNavigatedTo.Execute(null);
        }
        private void headerCheckbox_Checked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            FavoritePageViewModel.IsAllRowsSelectedCommand.Execute(null);
        }

        private void headerCheckbox_Unchecked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            FavoritePageViewModel.IsAllRowsUnselectedCommand.Execute(null);
        }

        private void UnfavoriteGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var senderName = (Grid)sender;
            var dataCxtx = senderName.DataContext;
            var dataSource = (FavoriteUiModel)dataCxtx;
            FavoritePageViewModel?.UnfavoriteImageCommand.Execute(dataSource);

        }

        private void GridRowCheckBoxCheckedChanged(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            FavoritePageViewModel.GridRowChecBoxCheckChangeCommand?.Execute(true);
        }

        private void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {

        }
    }
}
