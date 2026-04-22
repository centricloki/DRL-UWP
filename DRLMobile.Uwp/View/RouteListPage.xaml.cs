using DRLMobile.Core.Models.UIModels;
using DRLMobile.Uwp.ViewModel;
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
    public sealed partial class RouteListPage : Page
    {
        private RouteListPageViewModel routeListPageViewModel = new RouteListPageViewModel();

        public RouteListPage()
        {
            this.InitializeComponent();

            DataContext = routeListPageViewModel;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

           await routeListPageViewModel.InitializeDataOnPageLoad();

            if (routeListDataGrid != null)
            {
                routeListDataGrid.SelectedItem = null;
            }
        }

        private void ClearDatesButton_Click(object sender, RoutedEventArgs e)
        {
            startDatePicker.Date = null;
            endDatePicker.Date = null;

            routeListPageViewModel.ClearDateCommnd.Execute(e);
        }

        private void editRoute_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if(sender is Grid)
            {
                routeListPageViewModel.EditRouteCommand.Execute((sender as Grid).DataContext);
            }
        }

        private void viewRoute_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var senderName = (Grid)sender;
            var dataCxtx = senderName.DataContext;
            var dataSource = (RouteListUIModel)dataCxtx;

            routeListPageViewModel.ViewRouteCommand.Execute(dataSource);
        }

        private void routeListDataGrid_EndSorting(object sender, System.EventArgs e)
        {
            if (routeListDataGrid != null)
            {
                routeListDataGrid.SelectedItem = null;
            }
        }
    }
}