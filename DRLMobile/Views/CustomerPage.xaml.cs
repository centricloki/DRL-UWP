using DRLMobile.Core.Models.DataModels;
using DRLMobile.Core.Models.UIModels;
using DRLMobile.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DRLMobile.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CustomerPage : Page
    {
        #region properties

        public static CustomerMaster NewlyAddedCustomer { get; set; }
        private CustomerPageViewModel ViewModel = new CustomerPageViewModel();
        #endregion

        #region constructor
        public CustomerPage()
        {
            this.InitializeComponent();
            DataContext = ViewModel;
            NewlyAddedCustomer = null;
        }
        #endregion

        #region private methods

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.New)
            {
                HeaderControl.AutoSuggestionText = string.Empty;
                ViewModel?.OnNavigatedTo.Execute(null);
            }
            else if (NewlyAddedCustomer != null)
            {
                await ViewModel.AddNewlyAddedCustomer();
                NewlyAddedCustomer = null;
            }

        }


        #endregion

        private void ellipsisGrid_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            var senderName = (Grid)sender;
            var dataCxtx = senderName.DataContext;
            var dataSource = (CustomerPageUIModel)dataCxtx;
            ViewModel?.EllipsisClickedCommand.Execute(dataSource);
        }
    }



}
