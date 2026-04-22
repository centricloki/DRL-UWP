using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

using DRLMobile.Core.Helpers;
using DRLMobile.Core.Models.DataModels;
using DRLMobile.Core.Services;
using DRLMobile.ExceptionHandler;
using DRLMobile.Uwp.Helpers;
using DRLMobile.Uwp.Services;
using DRLMobile.Uwp.View;

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

using Newtonsoft.Json;

using Windows.ApplicationModel.Resources;
using Windows.Networking.PushNotifications;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Notifications.Management;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Networking.Connectivity;
using System.Net;
using DRLMobile.Core.Models.UIModels;
using System.Linq;
using Windows.UI.Xaml.Controls.Primitives;
using System.Collections.ObjectModel;

namespace DRLMobile.Uwp.ViewModel
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

        private string _progressText;
        public string ProgressText
        {
            get { return _progressText; }
            set { SetProperty(ref _progressText, value); }
        }

        private string _badgetextupdate;
        public string Badgetextupdate
        {
            get { return _badgetextupdate; }
            set { SetProperty(ref _badgetextupdate, value); }
        }

        private readonly App AppRef = (App)Application.Current;
        private readonly Windows.UI.Core.CoreDispatcher coreDispatcher;
        private BackgroundDownloadService _backgroundDownloadService;
        public BackgroundDownloadService BackgroundDownloadService
        {
            get { return _backgroundDownloadService; }
            set { SetProperty(ref _backgroundDownloadService, value); }
        }

        private ObservableCollection<DropDownUIModel> _assignedUserAreas;
        public ObservableCollection<DropDownUIModel> AssignedUserAreas
        {
            get { return _assignedUserAreas; }
            set { SetProperty(ref _assignedUserAreas, value); }
        }


        private bool _isAnyAssignedUserAreas;
        public bool IsAnyAssignedUserAreas
        {
            get { return _isAnyAssignedUserAreas; }
            set { SetProperty(ref _isAnyAssignedUserAreas, value); }
        }
        private string _lblAssignedUserAreas;
        public string LblAssignedUserAreas
        {
            get { return _lblAssignedUserAreas; }
            set { SetProperty(ref _lblAssignedUserAreas, value); }
        }

        private DropDownUIModel _selectedUserArea;
        public DropDownUIModel SelectedUserArea
        {
            get { return _selectedUserArea; }
            set { SetProperty(ref _selectedUserArea, value); }
        }

        #region Command
        public ICommand OnNavigatedToCommand { get; private set; }

        private ICommand _syncButtonCommand;
        public ICommand SyncButtonCommand => _syncButtonCommand ?? (_syncButtonCommand = new AsyncRelayCommand(SyncDataWithServer));

        private ICommand _travelVripButtonCommand;
        public ICommand TravelVripButtonCommand => _travelVripButtonCommand ?? (_travelVripButtonCommand = new RelayCommand(NavigateToTravelVripPage));

        public ICommand SrcProductCommand { private set; get; }
        public ICommand CartCommand { get; private set; }
        public ICommand CustomerCommand { get; private set; }
        public ICommand FavoriteCommand { get; private set; }
        public ICommand SettingCommand { get; private set; }
        public ICommand RackOrderCommand { get; private set; }
        public ICommand LogoutButtonCommand { get; private set; }
        public ICommand PopOrderCommand { get; private set; }
        public ICommand MapCommand { get; private set; }
        public ICommand RouteListCommand { get; private set; }
        public ICommand ActivityCommand { get; private set; }
        public ICommand AssignedAreaCommand { get; private set; }

        public ICommand GoogleMapCommand { get; private set; }
        public ICommand POCAzureMapCommand { get; private set; }
        public ICommand AzureMapCommand { get; private set; }
        public ICommand AzureRouteMapCommand { get; private set; }
        #endregion

        #region Constructor
        public DashboardPageViewModel()
        {
            OnNavigatedToCommand = new RelayCommand(OnNavigatedToCommandHandler);
            ActivityCommand = new RelayCommand(ActivityCommandHandler);
            RouteListCommand = new RelayCommand(RouteListCommandHandler);
            MapCommand = new RelayCommand(MapCommandHandler);
            PopOrderCommand = new RelayCommand(PopOrderCommandHandler);
            CustomerCommand = new RelayCommand(CustomerCommandHandler);
            FavoriteCommand = new RelayCommand(FavoriteCommandHandler);
            SettingCommand = new RelayCommand(SettingCommandHandler);
            RackOrderCommand = new RelayCommand(RackOrderCommandHandler);
            CartCommand = new RelayCommand(CartCommandHandler);
            LogoutButtonCommand = new AsyncRelayCommand(LogoutUser);
            SrcProductCommand = new RelayCommand(SrcProductCommandHandler);
            AssignedAreaCommand = new RelayCommand<DropDownUIModel>(AssignedAreaCommandHandler);
            resourceLoader = ResourceLoader.GetForCurrentView();
            LoadingVisibility = Visibility.Collapsed;
            coreDispatcher = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;


            GoogleMapCommand = new RelayCommand(() => NavigationService.NavigateShellFrame(typeof(GoogleMapPage)));
            POCAzureMapCommand = new RelayCommand(() => NavigationService.NavigateShellFrame(typeof(POCAzureMapPage)));

            //GoogleMapCommand = new RelayCommand(() => NavigationService.NavigateShellFrame(typeof(GoogleMapPage)));

            //AzureMapCommand = new RelayCommand(() => NavigationService.NavigateShellFrame(typeof(AzureMapPage)));
            //AzureRouteMapCommand = new RelayCommand(() => NavigationService.NavigateShellFrame(typeof(AzureRouteMapPage)));
        }
        #endregion

        #region Public Methods

        public async Task SyncDataWithServer()
        {
            ContentDialog syncSuccessStatusDialog = null;
            try
            {
                // for national users
                if ((new int[] { 5, 6, 17 }).Any(x => x == ((App)Application.Current).LoggedInUserRoleId))
                {
                    syncSuccessStatusDialog = new ContentDialog
                    {
                        Title = "Data Sync",
                        Content = "Data Sync feature is currently unavailable for national users.",
                        PrimaryButtonText = resourceLoader.GetString("OK"),
                    };
                    await syncSuccessStatusDialog.ShowAsync();
                    syncSuccessStatusDialog.Hide();
                }
                else
                {
                    syncSuccessStatusDialog = new ContentDialog
                    {
                        Title = "Data Sync",
                        Content = "Data sync may take a long time, would you like to continue?",
                        PrimaryButtonText = resourceLoader.GetString("OK"),
                        SecondaryButtonText = "Cancel"
                    };
                    ContentDialogResult result = await syncSuccessStatusDialog.ShowAsync();
                    if (result == ContentDialogResult.Primary)
                    {
                        LoadingVisibilityHandler(true);
                        await StartDataSync();
                    }
                    else
                    {
                        syncSuccessStatusDialog.Hide();
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(GetType().Name, "SyncDataWithServer", ex.StackTrace);
                syncSuccessStatusDialog = new ContentDialog
                {
                    Content = "Something went wrong. Please try again after some time.",
                    CloseButtonText = resourceLoader.GetString("OK")
                };
                await syncSuccessStatusDialog.ShowAsync();
            }
            finally
            {
                LoadingVisibilityHandler(false);
            }
        }

        public async Task NotifyUserAsync()
        {
            LoadingVisibilityHandler(true);
            await FillAssignedAreaSync();
            // for national users
            if ((new int[] { 5, 6, 17 }).Any(x => x == ((App)Application.Current).LoggedInUserRoleId))
            {
                //Check Push Notification
                await CheckPushNotificationAsync();
            }
            else
            {
                // for non-national users
                if (IsSqlliteReset())
                { await ResetUserSqlliteAsync(); }
                else
                {
                    if (AppRef.IsSyncSuccessProperty ?? false)
                    {
                        await ShowSyncSuccessfulMessageAfterLogin();
                        AppRef.IsUpgradeVersion = false;
                    }
                    //Check Push Notification
                    //await CheckPushNotificationAsync();

                    var frame = Window.Current.Content as Frame;

                    if ((frame.Content as ShellPage).ViewModel.IsSyncFromSidePane)
                    {
                        (frame.Content as ShellPage).ViewModel.IsSyncFromSidePane = false;
                        await StartDataSync();
                    }
                }
            }
            LoadingVisibilityHandler(false);
        }

        private async Task CheckPushNotificationAsync()
        {
            //Check Push Notification
            if (!AppRef.IsApplicationUpdateAvailable ?? false)
            {
                AppRef.IsApplicationUpdateAvailable = true;
                await CreateChannelUriAndSendToServer();
                await CheckForUpdateAlertAndGetTheUri();
            }
        }

        public async Task FillAssignedAreaSync()
        {
            if (AppRef.LoggedInUserRoleId == 3 || AppRef.LoggedInUserRoleId == 6 || AppRef.LoggedInUserRoleId == 17 || AppRef.LoggedInUserRoleId == await AppRef.QueryService.GetRoleIdAsync(ApplicationConstants.AVPRoleName))
            {
                LoadingVisibilityHandler(true);
                AssignedUserAreas = new ObservableCollection<DropDownUIModel>();
                AssignedUserAreas.Add(new DropDownUIModel { Id = 0, Name = "All" });
                try
                {
                    if (AppRef.LoggedInUserRoleId == 6 || AppRef.LoggedInUserRoleId == 17 || AppRef.LoggedInUserRoleId == await AppRef.QueryService.GetRoleIdAsync(ApplicationConstants.AVPRoleName))
                    {
                        LblAssignedUserAreas = "Zone";

                        IEnumerable<ZoneMasterUIModel> zoneMasterUIModels = (await AppRef.QueryService.GetZonesOnBasisOfCustomers()).ToList();
                        if (zoneMasterUIModels.Any())
                        {
                            foreach (ZoneMasterUIModel zoneUIModel in zoneMasterUIModels)
                            {
                                AssignedUserAreas.Add(new DropDownUIModel { Id = zoneUIModel.ZoneID, Name = zoneUIModel.ZoneName });
                            }
                        }
                    }
                    else if (AppRef.LoggedInUserRoleId == 3)
                    {
                        LblAssignedUserAreas = "Region";

                        IEnumerable<RegionMasterUIModel> regionMasterUIModels = await AppRef.QueryService.GetRegionsOnBasisOfZoneIdsAndPresentCustomers(null);
                        if (regionMasterUIModels.Any())
                        {
                            foreach (RegionMasterUIModel regionUIModel in regionMasterUIModels)
                            {
                                AssignedUserAreas.Add(new DropDownUIModel { Id = regionUIModel.RegionID, Name = regionUIModel.Regioname });
                            }
                        }
                    }

                    SelectedUserArea = AssignedUserAreas.FirstOrDefault(x => x.Id == AppRef.AreaUserSelectedId);
                    IsAnyAssignedUserAreas = true;
                }
                catch (Exception ex)
                {
                    ErrorLogger.WriteToErrorLog(nameof(DashboardPageViewModel), nameof(FillAssignedAreaSync), ex);
                }
            }
        }

        #endregion

        #region Private Methods

        private void AssignedAreaCommandHandler(DropDownUIModel selectedItem)
        {
            if (selectedItem != null)
            {
                if (AppRef.AreaUserSelectedId != selectedItem.Id)
                    AppRef.AreaUserSelectedId = selectedItem.Id;
            }
        }

        private void OnNavigatedToCommandHandler()
        {
            BadgeText = ((App)Application.Current).CartItemCount.ToString();
            Badgetextupdate = AppRef.IsUpgradeVersion ? "Visible" : "Collapsed";

            if (string.IsNullOrWhiteSpace(InfoLogger.GetInstance.ApplicationPath))
            {
                InfoLogger.GetInstance.ApplicationPath = ApplicationConstants.APP_PATH;
                InfoLogger.GetInstance.UserId = AppRef.LoginUserIdProperty;
            }
        }

        private bool IsSqlliteReset()
        {
            if (AppRef.SqlLiteResetVersion != ApplicationConstants.APPLICATION_VERSION)
            {
                AppRef.SqlLiteResetVersion = ApplicationConstants.APPLICATION_VERSION;
                //return true;

                return ApplicationConstants.APPLICATION_VERSION == "1.9.80";
            }
            return false;
        }

        private async Task ResetUserSqlliteAsync()
        {
            try
            {
                LoadingVisibility = Visibility.Visible;
                App appRef = (App)Application.Current;

                await appRef.QueryService.UploadDataOnPartialSync(appRef.LoginUserNameProperty,
                    appRef.LoginUserPinProperty, appRef.LastSyncDateTimeProperty);

                await DeleteFilesAsync();

                ResetApplicationProperties();

                var frame = Window.Current.Content as Frame;

                LoadingVisibility = Visibility.Collapsed;

                frame.Navigate(typeof(LoginPage), true);

            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DashboardPageViewModel), nameof(ResetUserSqlliteAsync), ex);

                LoadingVisibility = Visibility.Collapsed;

                ContentDialog errorDialog = new ContentDialog
                {
                    Title = "File Reset",
                    Content = "Something went wrong, could not reset sqllite archieve file at this moment. Please try again after some time.",
                    CloseButtonText = "OK"
                };

                await errorDialog.ShowAsync();
            }
        }

        private async Task DeleteFilesAsync()
        {
            // Get the folder object that corresponds to this absolute path in the file system.
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            // Get files from requested folder
            IReadOnlyList<StorageFile> storageFiles = await localFolder.GetFilesAsync();
            foreach (StorageFile storageFile in storageFiles)
            {
                try { await storageFile.DeleteAsync(StorageDeleteOption.PermanentDelete); }
                catch (Exception ex)
                {
                    ErrorLogger.WriteToErrorLog(nameof(DashboardPageViewModel), nameof(DeleteFilesAsync), ex);
                    continue;
                }
            }
        }

        private void ResetApplicationProperties()
        {
            try
            {
                AppRef.CartItemCount = 0;
                AppRef.CurrentOrderId = 0;
                AppRef.CartDataFromScreen = 0;
                AppRef.IsCreditRequestOrder = false;
                AppRef.IsOrderTypeChanged = false;
                AppRef.IsDistributionOptionClicked = false;
                AppRef.PreviousSelectedCustomerId = AppRef.SelectedCustomerId;
                AppRef.SelectedCustomerId = string.Empty;
                AppRef.CurrentDeviceOrderId = string.Empty;
                AppRef.IsCustomerActivity = false;

                AppRef.IsUserAlreadyLogin = false;
                AppRef.IsSyncSuccessProperty = false;
                AppRef.IsDataSyncSuccessAfterLogin = false;
                AppRef.AreaUserSelectedId = 0;
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DashboardPageViewModel), nameof(ResetApplicationProperties), ex.Message);
            }
        }

        private bool IsInternetConnected()
        {
            ConnectionProfile connections = NetworkInformation.GetInternetConnectionProfile();
            bool internet = (connections != null) &&
                (connections.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess);
            return internet;
        }

        private async void NoConnectionPopupAsync()
        {
            ContentDialog syncSuccessStatusDialog = new ContentDialog
            {
                Title = "No Internet Connection",
                Content = "Please check your internet connectivity.",
                CloseButtonText = resourceLoader.GetString("OK")
            };

            await syncSuccessStatusDialog.ShowAsync();
        }

        private async Task StartDataSync()
        {
            if (!IsInternetConnected())
            {
                NoConnectionPopupAsync();
                return;
            }

            try
            {
                AppRef.IsApplicationUpdateAvailable = false;

                ContentDialog syncSuccessStatusDialog;
                var dtlastsyncdate = AppRef.LastSyncDateTimeProperty;
                AppRef.PreviousSyncDateTimeProperty = AppRef.LastSyncDateTimeProperty;

                string isSyncSuccess = await DataSyncHelper.SyncDataAfterUserLogin(AppRef.LoginUserNameProperty, AppRef.LoginUserPinProperty, AppRef.LastSyncDateTimeProperty, AppRef.LoginUserIdProperty);

                if (!string.IsNullOrEmpty(DataSyncHelper.LatestSyncDateTime))
                {
                    var frame = Window.Current.Content as Frame;
                    (frame.Content as ShellPage).ViewModel.LastSyncDateTime = DateTimeHelper.ConvertStringToSyncDateTimeFormat(DataSyncHelper.LatestSyncDateTime);
                    AppRef.LastSyncDateTimeProperty = DataSyncHelper.LatestSyncDateTime;
                }

                if (string.IsNullOrWhiteSpace(isSyncSuccess))
                {
                    if (!string.IsNullOrWhiteSpace(dtlastsyncdate))
                        await PartialDownloadSRC(dtlastsyncdate);

                    syncSuccessStatusDialog = new ContentDialog
                    {
                        Content = resourceLoader.GetString("SyncSuccessMessageText"),
                        CloseButtonText = resourceLoader.GetString("OK")
                    };

                    await syncSuccessStatusDialog.ShowAsync();
                }
                else if (isSyncSuccess.ToLower().Contains("exceeded", StringComparison.OrdinalIgnoreCase))
                {
                    syncSuccessStatusDialog = new ContentDialog
                    {
                        Content = "Due to data size, only partial customer updates have synced. Please reset the user file in Honey settings tomorrow.",
                        CloseButtonText = resourceLoader.GetString("OK")
                    };

                    await syncSuccessStatusDialog.ShowAsync();
                }
                else
                {
                    syncSuccessStatusDialog = new ContentDialog
                    {
                        Title = "A sync error has occurred",
                        Content = "Please attempt to close Honey, reopen it, and then sync again. If the error persists, kindly forward the logs to our Helpdesk for further assistance.",
                        CloseButtonText = resourceLoader.GetString("OK")
                    };

                    await syncSuccessStatusDialog.ShowAsync();
                }

                // HS2-149 Honey | New Customer Gets Deselected After Sync When Order Is Placed Immediately After Adding
                if (!string.IsNullOrEmpty(AppRef.SelectedCustomerId))
                {
                    var selectedCustomer = await ((App)Application.Current).QueryService.GetSavedCustomerInformation(Convert.ToInt32(AppRef.SelectedCustomerId));
                    if (selectedCustomer == null)
                    {
                        AppRef.SelectedCustomerId = "";
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex is WebException) NoConnectionPopupAsync();
                else
                    ErrorLogger.WriteToErrorLog(GetType().Name, nameof(StartDataSync), ex);
            }
        }

        //if make changes in this method, change the same in loginpageViewmodel also
        private async Task PartialDownloadSRC(string dtlastsyncdate)
        {
            try
            {
                var webServiceResponse = await InvokeWebService.GetPartialSRCZIPFile(AppRef.LoginUserNameProperty, Convert.ToInt32(AppRef.LoginUserPinProperty), dtlastsyncdate);

                if (webServiceResponse != null && webServiceResponse.responsestatus.Equals("200") && string.IsNullOrEmpty(webServiceResponse.errormsg) && !string.IsNullOrEmpty(webServiceResponse.partialsrcfilename))
                {
                    var partialSRCPath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, ApplicationConstants.PartialSRCSubFolder);

                    if (Directory.Exists(partialSRCPath))
                    {
                        Directory.Delete(partialSRCPath, true);
                    }

                    var partialSRCPathzipfile = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, AppRef.LoginUserIdProperty + "_PartialDownloadSRC.zip");

                    if (File.Exists(partialSRCPathzipfile))
                    {
                        File.Delete(partialSRCPath);
                    }

                    var PartialSRCZipUrl = webServiceResponse.partialsrcfilename;

                    var fileNameWithExtension = PartialSRCZipUrl.Substring(PartialSRCZipUrl.LastIndexOf("/") + 1);

                    var fileName = fileNameWithExtension.Substring(0, fileNameWithExtension.IndexOf('.'));

                    BackgroundDownloadService = new BackgroundDownloadService();

                    var path = await BackgroundDownloadService?.DownLoadPartialSRCFile(PartialSRCZipUrl, fileNameWithExtension);

                    if (File.Exists(path))
                    {
                        if (!Directory.Exists(partialSRCPath))
                        {
                            Directory.CreateDirectory(partialSRCPath);
                        }
                        try
                        {
                            ZipFile.ExtractToDirectory(sourceArchiveFileName: path, partialSRCPath, overwriteFiles: true);
                            await Task.Delay(200);
                            File.Delete(path);

                            var targetproductpath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, ApplicationConstants.PartialSRCSubFolder, ApplicationConstants.SRCZipProductFolder);
                            string sourceProductPath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, ApplicationConstants.SrzFileName, ApplicationConstants.SRCZipProductFolder);

                            if (!Directory.Exists(sourceProductPath))
                            {
                                Directory.CreateDirectory(sourceProductPath);
                            }

                            string fileNametemp = string.Empty;
                            string destFile = string.Empty;

                            if (System.IO.Directory.Exists(targetproductpath))
                            {
                                string[] files = System.IO.Directory.GetFiles(targetproductpath);

                                // Copy the files and overwrite destination files if they already exist. 
                                foreach (string s in files)
                                {
                                    try
                                    {
                                        // Use static Path methods to extract only the file name from the path.
                                        fileNametemp = System.IO.Path.GetFileName(s);
                                        destFile = System.IO.Path.Combine(sourceProductPath, fileNametemp);
                                        File.Copy(s, destFile, true);
                                    }
                                    catch (Exception ex1)
                                    { }
                                }
                            }

                            var targetSalesdocpath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, ApplicationConstants.PartialSRCSubFolder, ApplicationConstants.SRCZipSalesDocs);
                            string sourceSalesPath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, ApplicationConstants.SrzFileName, ApplicationConstants.SRCZipSalesDocs);

                            fileNametemp = string.Empty;
                            destFile = string.Empty;

                            if (!Directory.Exists(sourceSalesPath))
                            {
                                Directory.CreateDirectory(sourceSalesPath);
                            }

                            if (System.IO.Directory.Exists(targetSalesdocpath))
                            {
                                string[] files = System.IO.Directory.GetFiles(targetSalesdocpath);

                                // Copy the files and overwrite destination files if they already exist. 
                                foreach (string s in files)
                                {
                                    try
                                    {
                                        // Use static Path methods to extract only the file name from the path.
                                        fileNametemp = System.IO.Path.GetFileName(s);
                                        destFile = System.IO.Path.Combine(sourceSalesPath, fileNametemp);
                                        File.Copy(s, destFile, true);
                                    }
                                    catch (Exception ex1)
                                    { }
                                }
                            }

                            var targetBranddocpath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, ApplicationConstants.PartialSRCSubFolder, ApplicationConstants.SRCZipBrandStyle);
                            string sourceBrandPath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, ApplicationConstants.BrandImageBaseFolder);

                            fileNametemp = string.Empty;
                            destFile = string.Empty;

                            if (!Directory.Exists(sourceBrandPath))
                            {
                                Directory.CreateDirectory(sourceBrandPath);
                            }

                            if (System.IO.Directory.Exists(targetBranddocpath))
                            {
                                string[] files = System.IO.Directory.GetFiles(targetBranddocpath);

                                // Copy the files and overwrite destination files if they already exist. 
                                foreach (string s in files)
                                {
                                    try
                                    {
                                        // Use static Path methods to extract only the file name from the path.
                                        fileNametemp = System.IO.Path.GetFileName(s);
                                        destFile = System.IO.Path.Combine(sourceBrandPath, fileNametemp);
                                        File.Copy(s, destFile, true);
                                    }
                                    catch (Exception ex1)
                                    { }
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            File.Delete(path);
                        }



                    }
                    else
                    {
                        //ErrorLogger.WriteToErrorLog(GetType().Name, "PartialDownloadSRC", "Partial SRC zip file not found for this user.");
                    }
                }
                else
                {
                    if (webServiceResponse.errormsg.Contains("50"))
                    {
                        var res = await AlertHelper.Instance.ShowConfirmationAlert("Alert", "We have a Bulk update to Factsheets. Please go to settings and Download SRC to get latest files.", "OK");
                    }
                }

            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(GetType().Name, "PartialDownloadSRC", ex.StackTrace);
            }
        }

        private void ActivityCommandHandler()
        {
            NavigationService.NavigateShellFrame(typeof(ActivitiesPage));
        }

        private void RouteListCommandHandler()
        {
            NavigationService.NavigateShellFrame(typeof(RouteListPage));
        }

        private void MapCommandHandler()
        {
            NavigationService.NavigateShellFrame(typeof(MapPage));
        }

        private void PopOrderCommandHandler()
        {
            NavigationService.NavigateShellFrame(typeof(PopOrderPage));
        }

        private void SettingCommandHandler()
        {
            NavigationService.NavigateShellFrame(typeof(SettingsPage));
        }
        private void RackOrderCommandHandler()
        {
            NavigationService.NavigateShellFrame(typeof(RackOrderListPage));
        }

        private void NavigateToTravelVripPage()
        {
            NavigationService.NavigateShellFrame(typeof(TravelVripPage));
        }

        private void FavoriteCommandHandler()
        {
            NavigationService.NavigateShellFrame(typeof(FavoritePage));
        }

        private void CustomerCommandHandler()
        {
            NavigationService.NavigateShellFrame(typeof(CustomersListPage));
        }
        private void CartCommandHandler()
        {
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
        }

        private void SrcProductCommandHandler()
        {
            //if (AppRef.IsDistributionOptionClicked.HasValue && AppRef.IsDistributionOptionClicked.Value)
            //{
            //    AppRef.IsDistributionOptionClicked = false;
            //}
            //if (AppRef.IsCreditRequestOrder.HasValue && AppRef.IsCreditRequestOrder.Value)
            //{
            //    AppRef.IsCreditRequestOrder = false;
            //}
            NavigationService.NavigateShellFrame(typeof(SRCProductPage));
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
                        "This exception is of PUSH Notification ------- " + ex);
                }

                if (AppRef.Channel != null && !string.IsNullOrWhiteSpace(AppRef.Channel.Uri))
                {
                    await RegisterChannelToServer();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(GetType().Name, "CreateChannelUriAndSendToServer", ex);
            }
        }

        private async Task CheckForUpdateAlertAndGetTheUri()
        {

            if (AppRef.ShowUserPopupToUpdate.HasValue && AppRef.ShowUserPopupToUpdate.Value)
            {
                if (!string.IsNullOrWhiteSpace(AppRef.NotificationContent))
                {
                    var notificationContent = JsonConvert.DeserializeObject<NotificationModel>(AppRef.NotificationContent);

                    var message = string.Format("A new version {0} is available and has been assigned to you. Please upgrade your application.", notificationContent?.appversion);
                    var result = await AlertHelper.Instance.ShowConfirmationAlert(resourceLoader.GetString("UPDATE"), message, resourceLoader.GetString("UPDATE"), resourceLoader.GetString("InstallLaterText"));
                    if (result)
                    {
                        await UpdateAppComandHandler();
                    }
                    else
                    {
                        AppRef.IsUpgradeVersion = true;
                    }
                    AppRef.ShowUserPopupToUpdate = false;
                }
            }
            Badgetextupdate = AppRef.IsUpgradeVersion ? "Visible" : "Collapsed";
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
                var result = await AlertHelper.Instance.ShowConfirmationAlert(
        ResourceExtensions.GetLocalized("ALERT"),
        ResourceExtensions.GetLocalized("CLOSE_APP_MSG"),
        ResourceExtensions.GetLocalized("YesText"),
        ResourceExtensions.GetLocalized("NoText"));

                if (result)
                {
                    LoadingVisibility = Visibility.Visible;
                    var notificationModel = JsonConvert.DeserializeObject<NotificationModel>(AppRef.NotificationContent);

                    var downloadFilePath = await InvokeWebService.DownloadAppUpdateFile(notificationModel.downloadurl, HelperMethods.GetNameFromURL(notificationModel.downloadurl));

                    if (!string.IsNullOrWhiteSpace(downloadFilePath))
                    {
                        Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

                        Windows.Storage.StorageFile file = await storageFolder.CreateFileAsync(HelperMethods.GetNameFromURL(notificationModel.downloadurl), Windows.Storage.CreationCollisionOption.OpenIfExists);

                        var options = new Windows.System.LauncherOptions();

                        options.DisplayApplicationPicker = false;

                        await Windows.System.Launcher.LaunchFileAsync(file, options);

                        AppRef.ShowUserPopupToUpdate = false;
                        AppRef.IsApplicationUpdateAvailable = false;

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
                var NotificationData = await InvokeWebService.SendChannelUriToServer(AppRef.Channel.Uri, AppRef.LoginUserIdProperty);

                if (!string.IsNullOrEmpty(NotificationData))
                {
                    var notificationModel = JsonConvert.DeserializeObject<NotificationModel>(NotificationData);

                    if (!string.IsNullOrEmpty(notificationModel.downloadurl) && !string.IsNullOrEmpty(notificationModel.appversion))
                    {
                        AppRef.ShowUserPopupToUpdate = true;
                        AppRef.NotificationContent = NotificationData;
                    }
                    else
                    {
                        AppRef.ShowUserPopupToUpdate = false;
                        AppRef.IsUpgradeVersion = false;
                        AppRef.NotificationContent = "";
                    }
                }

            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DashboardPageViewModel), "RegisterChannelToServer", ex.Message);
            }
        }

        private async Task LogoutUser()
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
                foreach (var item in frame.BackStack)
                    frame.BackStack.Remove(item);
                InfoLogger.GetInstance.ApplicationPath = InfoLogger.GetInstance.UserId = "";
                frame.Navigate(typeof(LoginPage));
            }
            else
            {
                userLogoutDialog.Hide();
            }
        }

        private async Task ShowSyncSuccessfulMessageAfterLogin()
        {
            ((App)Application.Current).IsSyncSuccessProperty = false;

            ContentDialog syncSuccessDialog = new ContentDialog
            {
                Content = resourceLoader.GetString("SyncSuccessMessageText"),
                CloseButtonText = resourceLoader.GetString("OK")
            };

            await syncSuccessDialog.ShowAsync();
        }

        private async void LoadingVisibilityHandler(bool isLoading)
        {
            await coreDispatcher.RunAsync(CoreDispatcherPriority.High, () => LoadingVisibility = isLoading ? Visibility.Visible : Visibility.Collapsed);
        }

        #endregion
    }
}
