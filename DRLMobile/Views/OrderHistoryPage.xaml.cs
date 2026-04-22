using DRLMobile.Core.Models.DataModels;
using DRLMobile.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DRLMobile.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class OrderHistoryPage : Page
    {
        public static OrderDetailUpdatedModel UpdatedOrder;

        private OrderHistoryPageViewModel ViewModel = new OrderHistoryPageViewModel();
        public OrderHistoryPage()
        {
            this.InitializeComponent();
            this.DataContext = ViewModel;
        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if(e.NavigationMode == NavigationMode.New)
            {
                UpdatedOrder = null;
                ViewModel.OnNavigatedToCommand.Execute(e.Parameter);
            }
            else if(e.NavigationMode == NavigationMode.Back)
            {
                if(UpdatedOrder!=null)
                {
                    ViewModel.OnNavigatedToCommand.Execute(UpdatedOrder);
                }
            }
        }

        private void ReorderTapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            ViewModel.IsReorderTapped = true;
        }
    }
}
