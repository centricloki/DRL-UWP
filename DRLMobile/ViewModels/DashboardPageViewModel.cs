using DRLMobile.Core.Helpers;
using DRLMobile.Core.Models.DataModels;
using DRLMobile.Core.Services;
using DRLMobile.ExceptionHandler;
using DRLMobile.Helpers;
using DRLMobile.Services;
using DRLMobile.Views;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel.Resources;
using Windows.Networking.PushNotifications;
using Windows.System;
using Windows.UI.Notifications.Management;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace DRLMobile.ViewModels
{
    public class DashboardPageViewModel : ObservableObject
    {
        private readonly ResourceLoader resourceLoader;

        private Visibility _loadingVisiblity;
        public Visibility LoadingVisibility
        {
            get { return _loadingVisiblity; }
            set { SetProperty(ref _loadingVisiblity, value); }
        }

        private string _badgeText;
        public string BadgeText
        {
            get { return _badgeText; }
            set { SetProperty(ref _badgeText, value); }
        }

        private readonly App AppRef = ((App)Application.Current);

        #region Command
        public ICommand OnNavigatedToCommand { get; private set; }

        private ICommand _srcproductButtonCommand;
        public ICommand SRCButtonCommand => _srcproductButtonCommand ?? (_srcproductButtonCommand = new RelayCommand(NavigateToSRCProductPage));

        private ICommand _cartButtonCommand;
        public ICommand CartButtonCommand => _cartButtonCommand ?? (_cartButtonCommand = new RelayCommand(NavigateToCartPage));

        private ICommand _customerButtonCommand;
        public ICommand CustomerButtonCommand => _customerButtonCommand ?? (_customerButtonCommand = new RelayCommand(NavigateToCustomerPage));

        private ICommand _favoriteButtonCommand;
        public ICommand FavoriteButtonCommand => _favoriteButtonCommand ?? (_favoriteButtonCommand = new RelayCommand(NavigateToFavoritePage));

        private ICommand _routeListButtonCommand;
        public ICommand RouteListButtonCommand => _routeListButtonCommand ?? (_routeListButtonCommand = new RelayCommand(NavigateToRouteListPage));

        private ICommand _settingsButtonCommand;
        public ICommand SettingsButtonCommand => _settingsButtonCommand ?? (_settingsButtonCommand = new RelayCommand(NavigateToSettingsPage));

        private ICommand _activitiesButtonCommand;
        public ICommand ActivitiesButtonCommand => _activitiesButtonCommand ?? (_activitiesButtonCommand = new RelayCommand(NavigateToActivitiesPage));

        private ICommand _mapButtonCommand;
        public ICommand MapButtonCommand => _mapButtonCommand ?? (_mapButtonCommand = new RelayCommand(NavigateToMapPage));

        private ICommand _syncButtonCommand;
        public ICommand SyncButtonCommand => _syncButtonCommand ?? (_syncButtonCommand = new RelayCommand(SyncDataWithServer));

        private ICommand _rackOrderButtonCommand;
        public ICommand RackOrderButtonCommand => _rackOrderButtonCommand ?? (_rackOrderButtonCommand = new RelayCommand(NavigateToRackOrderPage));

        private ICommand _popOrderButtonCommand;
        public ICommand PopOrderButtonCommand => _popOrderButtonCommand ?? (_popOrderButtonCommand = new RelayCommand(NavigateToPopOrderPage));

        private ICommand _travelVripButtonCommand;
        public ICommand TravelVripButtonCommand => _travelVripButtonCommand ?? (_travelVripButtonCommand = new RelayCommand(NavigateToTravelVripPage));

        private ICommand _logoutButtonCommand;
        public ICommand LogoutButtonCommand => _logoutButtonCommand ?? (_logoutButtonCommand = new RelayCommand(LogoutUser));

        #endregion

        #region Constructor

        public DashboardPageViewModel()
        {
            OnNavigatedToCommand = new RelayCommand(OnNavigatedToCommandHandler);
            resourceLoader = ResourceLoader.GetForCurrentView();

            LoadingVisibility = Visibility.Collapsed;   
        }

        #endregion

        #region Public methods

        public void NavigateToSRCProductPage()
        {
            NavigationService.Navigate<SRCProductPage>();
        }

        public void NavigateToCartPage()
        {
            if (AppRef.CartItemCount > 0)
            {
                if (AppRef.CartDataFromScreen == 0)
                {
                    NavigationService.Navigate<CartPage>();
                }
                else if (AppRef.CartDataFromScreen == 1)
                {
                    NavigationService.Navigate<RetailTransactionPage>();
                }
            }
            else if(AppRef.CartItemCount==0)
            {
                NavigationService.Navigate<CartPage>();
            }
        }

        public void NavigateToCustomerPage()
        {
            NavigationService.Navigate<CustomerPage>();
        }

        public void NavigateToFavoritePage()
        {
            NavigationService.Navigate<FavoritePage>();
        }

        public void NavigateToRouteListPage()
        {
            NavigationService.Navigate<RouteListPage>();
        }

        public void NavigateToSettingsPage()
        {
            NavigationService.Navigate<SettingsPage>();
        }

        public void NavigateToActivitiesPage()
        {
            NavigationService.Navigate<ActivitiesPage>();
        }

        public void NavigateToMapPage()
        {
            NavigationService.Navigate<MapPage>();
        }

        public async void SyncDataWithServer()
        {
            ContentDialog syncSuccessStatusDialog;

            try
            {
                syncSuccessStatusDialog = new ContentDialog
                {
                    Title = "Data Sync",
                    Content = "Data sync may take a long time",
                    PrimaryButtonText = resourceLoader.GetString("OK")
                };

                ContentDialogResult result = await syncSuccessStatusDialog.ShowAsync();

                if (result == ContentDialogResult.Primary)
                {
                    LoadingVisibilityHandler(true);

                    var isSyncSuccess = await DataSyncHelper.SyncDataAfterUserLogin(AppRef.LoginUserNameProperty,
                        AppRef.LoginUserPinProperty, AppRef.LastSyncDateTimeProperty);

                    if (isSyncSuccess)
                    {
                        LoadingVisibilityHandler(false);

                        var frame = Window.Current.Content as Frame;

                        (frame.Content as ShellPage).ViewModel.LastSyncDateTime = DataSyncHelper.LatestSyncDateTime;

                        syncSuccessStatusDialog = new ContentDialog
                        {
                            Content = resourceLoader.GetString("SyncSuccessMessageText"),
                            CloseButtonText = resourceLoader.GetString("OK")
                        };

                        await syncSuccessStatusDialog.ShowAsync();
                    }
                    else
                    {
                        LoadingVisibilityHandler(false);

                        syncSuccessStatusDialog = new ContentDialog
                        {
                            Content = resourceLoader.GetString("SyncFailMessageText"),
                            CloseButtonText = resourceLoader.GetString("OK")
                        };

                        await syncSuccessStatusDialog.ShowAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                LoadingVisibilityHandler(false);

                ErrorLogger.WriteToErrorLog(GetType().Name, "InsertOrUpdateCustomerDocument", ex.StackTrace);

                syncSuccessStatusDialog = new ContentDialog
                {
                    Content = "Something went wrong. Please check network connection and try again.",
                    CloseButtonText = resourceLoader.GetString("OK")
                };

                await syncSuccessStatusDialog.ShowAsync();
            }
        }

        public void NavigateToRackOrderPage()
        {
            NavigationService.Navigate<RackOrderListPage>();
        }

        public void NavigateToPopOrderPage()
        {
            NavigationService.Navigate<PopOrderPage>();
        }

        public void NavigateToTravelVripPage()
        {
            NavigationService.Navigate<TravelVripPage>();
        }

        #endregion

        #region Private Methods

        private void OnNavigatedToCommandHandler()
        {
            try
            {
                Parallel.Invoke(async () =>
                {
                    if ((bool)((App)Application.Current).IsSyncSuccessProperty)
                    {
                        await ShowDataSyncMessageAfterLogin();
                    }
                    await ShowUserNotificationPermissionPromt();
                    await CreateChannelUriAndSendToServer();
                    await CheckForUpdateAlertAndGetTheUri();
                });
                BadgeText = ((App)Application.Current).CartItemCount.ToString();
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(GetType().Name, "OnNavigatedToCommandHandler", ex.StackTrace);
            }
        }

        private async Task CreateChannelUriAndSendToServer()
        {
            try
            {
                try
                {
                    AppRef.Channel = await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();
                }
                catch (Exception ex)
                {
                    ErrorLogger.WriteToErrorLog(GetType().Name, "CreatePushNotificationChannelForApplicationAsync",
                        "This exception is of PUSH Notification ------- " + ex.StackTrace);
                }
                
                if (AppRef.Channel != null && !string.IsNullOrWhiteSpace(AppRef.Channel.Uri))
                {
                    await RegisterChannelToServer();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(GetType().Name, "CreateChannelUriAndSendToServer", ex.StackTrace);
            }
        }

        private async Task CheckForUpdateAlertAndGetTheUri()
        {
            if (AppRef.ShowUserPopupToUpdate.HasValue && AppRef.ShowUserPopupToUpdate.Value)
            {
                if (!string.IsNullOrWhiteSpace(AppRef.NotificationContent))
                {
                    var notificationContent = JsonConvert.DeserializeObject<NotificationModel>(AppRef.NotificationContent);

                    var message = string.Format("A new {0} version has been assigned to you.", notificationContent?.AppVersion);

                    var result = await AlertHelper.Instance.ShowConfirmationAlert(resourceLoader.GetString("UPDATE"), message, resourceLoader.GetString("YesText"), resourceLoader.GetString("NoText"));

                    if (result)
                    {
                        await UpdateAppComandHandler();
                    }
                    AppRef.ShowUserPopupToUpdate = false;
                }
            }
        }

        private async Task ShowUserNotificationPermissionPromt()
        {
            try
            {
                UserNotificationListener listener = UserNotificationListener.Current;
                if (listener != null)
                    await listener.RequestAccessAsync();
                ///await Launcher.LaunchUriAsync(new Uri("ms-settings:privacy-notifications"));
                ///var notifier = Windows.UI.Notifications.ToastNotificationManager.CreateToastNotifier();
                ///if (notifier.Setting != Windows.UI.Notifications.NotificationSetting.Enabled)
            }
            catch (Exception ex)
            {
                ErrorHandler.LogException(GetType().Name, "ShowUserNotificationPermissionPromt", ex);
            }
        }

        private async Task UpdateAppComandHandler()
        {
            try
            {
                var result = await AlertHelper.Instance.ShowConfirmationAlert(ResourceExtensions.GetLocalized("ALERT"), ResourceExtensions.GetLocalized("CLOSE_APP_MSG"), ResourceExtensions.GetLocalized("YesText"), ResourceExtensions.GetLocalized("NoText"));
                if (result)
                {
                    AppRef.IsApplicationUpdateAvailable = false;
                    LoadingVisibility = Visibility.Visible;
                    var notificationModel = JsonConvert.DeserializeObject<NotificationModel>(AppRef.NotificationContent);
                    var downloadFilePath = await InvokeWebService.DownloadAppUpdateFile(notificationModel.DownloadUrl, HelperMethods.GetNameFromURL(notificationModel.DownloadUrl));
                    if (!string.IsNullOrWhiteSpace(downloadFilePath))
                    {
                        Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                        Windows.Storage.StorageFile file = await storageFolder.CreateFileAsync(HelperMethods.GetNameFromURL(notificationModel.DownloadUrl), Windows.Storage.CreationCollisionOption.OpenIfExists);
                        var options = new Windows.System.LauncherOptions();
                        options.DisplayApplicationPicker = false;
                        await Windows.System.Launcher.LaunchFileAsync(file, options);
                        AppRef.ShowUserPopupToUpdate = false;
                        Application.Current.Exit();
                    }
                    LoadingVisibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                LoadingVisibility = Visibility.Collapsed;
                ErrorHandler.LogException(GetType().Name, "UpdateAppComandHandler", ex);
            }
        }

        private async Task RegisterChannelToServer()
        {
            try
            {
                await InvokeWebService.SendChannelUriToServer(AppRef.Channel.Uri, AppRef.LoginUserIdProperty);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DashboardPageViewModel), nameof(RegisterChannelToServer), ex.Message);
            }
        }

        private async void LogoutUser()
        {
            ContentDialog userLogoutDialog = new ContentDialog
            {
                Title = resourceLoader.GetString("LogoutTitleText"),
                Content = resourceLoader.GetString("LogoutMessage"),
                PrimaryButtonText = resourceLoader.GetString("YesText"),
                SecondaryButtonText = resourceLoader.GetString("NoText")
            };

            ContentDialogResult result = await userLogoutDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                ((App)Application.Current).IsUserAlreadyLogin = false;

                ((App)Application.Current).IsSyncSuccessProperty = false;

                var frame = Window.Current.Content as Frame;

                frame.Navigate(typeof(LoginPage));

                foreach (var item in frame.BackStack)
                    frame.BackStack.Remove(item);
            }
            else
            {
                userLogoutDialog.Hide();
            }
        }

        private async Task ShowDataSyncMessageAfterLogin()
        {
            ((App)Application.Current).IsSyncSuccessProperty = false;

            ContentDialog syncSuccessDialog = new ContentDialog();

            if ((bool)((App)Application.Current).IsDataSyncSuccessAfterLogin)
            {
                ((App)Application.Current).IsDataSyncSuccessAfterLogin = false;

                syncSuccessDialog.Content = resourceLoader.GetString("SyncSuccessMessageText");
                syncSuccessDialog.CloseButtonText = resourceLoader.GetString("OK");
            }
            else
            {
                syncSuccessDialog.Content = resourceLoader.GetString("SyncFailMessageText");
                syncSuccessDialog.CloseButtonText = resourceLoader.GetString("OK");
            }
            
            await syncSuccessDialog.ShowAsync();
        }

        private void LoadingVisibilityHandler(bool isLoading)
        {
            LoadingVisibility = isLoading ? Visibility.Visible : Visibility.Collapsed;
        }

        #endregion
    }
}
