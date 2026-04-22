using DRLMobile.Core.Models.DataModels;
using DRLMobile.Core.Models.UIModels;
using DRLMobile.Uwp.Services;
using DRLMobile.Uwp.ViewModel;
using DRLMobile.ExceptionHandler;
using DRLMobile.Uwp.Helpers;

using System;
using System.Linq;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DRLMobile.Uwp.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CustomersListPage : Page
    {
        #region Properties
        public static CustomerMaster NewlyAddedCustomer { get; set; }
        public static CustomerPageUIModel EditedCustomer { get; set; }

        private CustomersListPageViewModel ViewModel = new CustomersListPageViewModel();

        #endregion


        #region Constructor
        public CustomersListPage()
        {
            InitializeComponent();

            DataContext = ViewModel;
            NewlyAddedCustomer = null;
            this.Loaded += CustomersListPage_Loaded;
            this.Unloaded += CustomersListPage_Unloaded;
        }

        private void CustomersListPage_Unloaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (ViewModel != null)
            {
                ViewModel.DispatcherQueueTimerCleanup();
            }            
        }

        private async void CustomersListPage_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (NewlyAddedCustomer != null)
            {
                await ViewModel.AddNewlyAddedCustomerToMainList();
                NewlyAddedCustomer = null;
            }
            else if (EditedCustomer != null)
            {
                await ViewModel.EditAlreadyPesentCustomer();
                EditedCustomer = null;
            }
            else
            {
                HeaderControl.AutoSuggestionText = string.Empty;
                ViewModel?.OnNavigatedToCommand.Execute(null);
            }
        }
        #endregion

        private void EllipsisGrid_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            var senderName = (Grid)sender;
            var dataCxtx = senderName.DataContext;
            var dataSource = (CustomerPageUIModel)dataCxtx;

            ViewModel?.EllipsisClickedCommand.Execute(dataSource);
        }

        public BitmapImage GetSortImagePath(string args)
        {
            return new BitmapImage(new Uri(String.Format("ms-appx:///Assets/Controls/{0}.png", args)));
        }

        public Boolean IsNotEmpty(string args)
        {
            return !string.IsNullOrWhiteSpace(args);
        }

        private void customerListDataGrid_ItemClick(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            var senderName = (Grid)sender;
            var dataCxtx = senderName.DataContext;
            var dataSource = (CustomerPageUIModel)dataCxtx;
            ViewModel?.ItemSelectedCommand.Execute(dataSource);
        }
        private void AllCustomerDataGrid_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            if (args.ItemIndex % 2 != 0)
            {
                //Grey background
                args.ItemContainer.Background = new SolidColorBrush(Color.FromArgb(223, 223, 223, 223));
            }
            else
            {
                //White background
                args.ItemContainer.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
            }
        }
    }
}
