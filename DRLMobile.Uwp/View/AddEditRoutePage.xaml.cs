using DRLMobile.Uwp.ViewModel;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DRLMobile.Uwp.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddEditRoutePage : Page
    {
        private AddEditRoutePageViewModel ViewModel = new AddEditRoutePageViewModel();
        public AddEditRoutePage()
        {
            this.InitializeComponent();
            DataContext = ViewModel;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ViewModel?.OnNavigatedToCommand?.Execute(e.Parameter);
        }

        private void CheckBoxGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if(sender is Grid)
            {
                ViewModel?.OnCheckBoxClicked?.Execute((sender as Grid).DataContext);
            }
        }

        private void DataGridcontrol_EndSorting(object sender, System.EventArgs e)
        {
            if (DataGridcontrol != null)
            {
                DataGridcontrol.SelectedItem = null;
                DataGridcontrol.AutoScrollOnSorting = false;
            }
        }

        private void DataGridcontrol_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (DataGridcontrol != null)
            {
                DataGridcontrol.SelectedItem = null;
                
            }
        }
    }
}
