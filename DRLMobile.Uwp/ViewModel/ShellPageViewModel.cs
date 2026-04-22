using DRLMobile.Core.Helpers;
using DRLMobile.Core.Models;
using DRLMobile.Core.Models.UIModels;
using DRLMobile.Uwp.Services;
using DRLMobile.Uwp.View;

using Microsoft.Toolkit.Mvvm.Input;

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace DRLMobile.Uwp.ViewModel
{
    public class ShellPageViewModel : BaseModel
    {

        private bool _isBackEnabled;
        private string _tempuserName;
        private readonly App AppRef = (App)Application.Current;

        public ICommand LoadedCommand { private set; get; }
        public ICommand NavigatedToCommand { private set; get; }
        public ICommand NavigatingFromCommand { private set; get; }
        public ICommand SideMenuItemClicked { private set; get; }

        private LoggedInUserDetailsUIModel _loggedInUserInfo;
        public LoggedInUserDetailsUIModel UserInformation
        {
            get => _loggedInUserInfo;
            set
            {
                _loggedInUserInfo = value;
                OnPropertyChanged();
            }
        }

        private string _lastSyncDateTime;
        public string LastSyncDateTime
        {
            get { return _lastSyncDateTime; }
            set { SetProperty(ref _lastSyncDateTime, value); }
        }

        public bool IsBackEnabled
        {
            get { return _isBackEnabled; }
            set { SetProperty(ref _isBackEnabled, value); }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set { SetProperty(ref _isLoading, value); }
        }

        public string TempUserName
        {
            get { return _tempuserName; }
            set { SetProperty(ref _tempuserName, value); }
        }

        private bool _isPaneOpen;
        public bool IsPaneOpen
        {
            get { return _isPaneOpen; }
            set { SetProperty(ref _isPaneOpen, value); }
        }

        private bool _isSyncFromSidePane;
        public bool IsSyncFromSidePane
        {
            get { return _isSyncFromSidePane; }
            set { SetProperty(ref _isSyncFromSidePane, value); }
        }

        public bool IsSideMenuItemClickable { get; set; }

        public ShellPageViewModel()
        {
            NavigatedToCommand = new AsyncRelayCommand(NavigatedToCommandHandler);
            NavigatingFromCommand = new RelayCommand(NavigatingFromCommandHandler);
            SideMenuItemClicked = new AsyncRelayCommand<string>(SideMenuItemClickedHandler);
            IsPaneOpen = false;
            IsSyncFromSidePane = false;
        }

        private async Task SideMenuItemClickedHandler(string obj)
        {
            if (!IsSideMenuItemClickable) return;
            switch (obj)
            {
                case "Menu":
                    /// delay is added to complete the navigation 
                    await Task.Delay(500);
                    IsPaneOpen = !IsPaneOpen;
                    break;
                case "Back":
                    NavigationService.GoBackInShell();
                    break;
                case "Home":
                    NavigationService.NavigateShellFrame(typeof(DashboardPage));
                    break;
                case "Product":
                    NavigationService.NavigateShellFrame(typeof(SRCProductPage));
                    break;
                case "Cart":
                    if (AppRef.CartItemCount > 0)
                    {
                        if (AppRef.CartDataFromScreen == 0)
                        {
                            NavigationService.NavigateShellFrame(typeof(CartPage));
                        }
                        else
                        {
                            NavigationService.NavigateShellFrame(typeof(RetailTransactionPage));
                        }
                    }
                    else if (AppRef.CartItemCount == 0)
                    {
                        NavigationService.NavigateShellFrame(typeof(CartPage));
                    }
                    break;
                case "Customer":
                    NavigationService.NavigateShellFrame(typeof(CustomersListPage));
                    break;
                case "Favorite":
                    NavigationService.NavigateShellFrame(typeof(FavoritePage));
                    break;
                case "Settings":
                    NavigationService.NavigateShellFrame(typeof(SettingsPage));
                    break;
                case "Rack":
                    NavigationService.NavigateShellFrame(typeof(RackOrderListPage));
                    break;
                case "Pop":
                    NavigationService.NavigateShellFrame(typeof(PopOrderPage));
                    break;
                case "Travel":
                    NavigationService.NavigateShellFrame(typeof(TravelVripPage));
                    break;
                case "Map":
                    NavigationService.NavigateShellFrame(typeof(AdvanceGoogleMapPage));
                    break;
                case "Route":
                    NavigationService.NavigateShellFrame(typeof(RouteListPage));
                    break;
                case "Activities":
                    NavigationService.NavigateShellFrame(typeof(ActivitiesPage));
                    break;
                case "Sync":
                    ContentDialog syncSuccessStatusDialog = null;
                    // for national users
                    if ((new int[] { 5, 6, 17 }).Any(x => x == ((App)Application.Current).LoggedInUserRoleId))
                    {
                        syncSuccessStatusDialog = new ContentDialog
                        {
                            Title = "Data Sync",
                            Content = "Data Sync feature is currently unavailable for national users.",
                            PrimaryButtonText = "OK",
                        };
                        await syncSuccessStatusDialog.ShowAsync();
                        syncSuccessStatusDialog.Hide();
                    }
                    else
                    {
                        // for non-national users
                        syncSuccessStatusDialog = new ContentDialog
                        {
                            Title = "Data Sync",
                            Content = "Data sync may take a long time, would you like to continue?",
                            PrimaryButtonText = "OK",
                            SecondaryButtonText = "Cancel"
                        };
                        ContentDialogResult result = await syncSuccessStatusDialog.ShowAsync();
                        if (result == ContentDialogResult.Primary)
                        {
                            IsSyncFromSidePane = true;
                            NavigationService.NavigateShellFrame(typeof(DashboardPage));
                        }
                        else
                        {
                            IsSyncFromSidePane = false;
                            syncSuccessStatusDialog.Hide();
                        }
                    }
                    break;
            }
        }

        private async Task NavigatedToCommandHandler()
        {
            IsPaneOpen = false;
            IsSideMenuItemClickable = true;
            await GetLoggedInUserData();
            NavigationService.InitialShellNavigation();
            ///CustomerPageViewModel.NavigationEventHandler += NavigationEvent;
        }



        private async Task GetLoggedInUserData()
        {
            UserInformation = await (Application.Current as App).QueryService.GetLoggedInUserInformation(Convert.ToInt32(((App)Application.Current).LoginUserIdProperty));
            if (UserInformation != null)
            {
                LastSyncDateTime = DateTimeHelper.ConvertStringToSyncDateTimeFormat(((App)Application.Current).LastSyncDateTimeProperty);

                TempUserName = "Logged in as : " + UserInformation.FirstName + " " + UserInformation.LastName;

                ((App)Application.Current).LoggedInUserRoleId = UserInformation.RoleId;

                ((App)Application.Current).LoggedInUserRegionId = UserInformation.RegionId;

                ((App)Application.Current).LoggedInUserZoneId = UserInformation.ZoneId;
            }
        }

        private void NavigatingFromCommandHandler()
        {
            ///CustomerPageViewModel.NavigationEventHandler -= NavigationEvent;
        }

        private void NavigationEvent(object sender, int id)
        {
            //if (id != 0)
            //    Services.NavigationService.Navigate<CustomerDetailsPage>(id);
            //else
            //    NavigationService.Navigate<AddCustomerPage>(id);
        }
    }
}
