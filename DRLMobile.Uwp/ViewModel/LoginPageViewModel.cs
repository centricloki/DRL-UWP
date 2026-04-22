using System;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Input;

using DRLMobile.Core.Helpers;
using DRLMobile.Core.Models;
using DRLMobile.Core.Models.DataModels;
using DRLMobile.Core.Models.UIModels;
using DRLMobile.Core.Services;
using DRLMobile.ExceptionHandler;
using DRLMobile.Uwp.Helpers;
using DRLMobile.Uwp.Services;
using DRLMobile.Uwp.View;

using Microsoft.Toolkit.Mvvm.Input;

using Newtonsoft.Json;
using System.Linq;

using Windows.ApplicationModel.Resources;
using Windows.Networking.Connectivity;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace DRLMobile.Uwp.ViewModel
{
    public class LoginPageViewModel : BaseModel
    {

        private readonly App AppReference = (App)Application.Current;

        #region Properties

        private string _userName;
        private string _pin;
        private bool _isLoginSuccessful;
        private bool _isDataDownloadSuccessful;
        private bool _isDataFileSuccessful;
        private LoginUIModel _loginUserDetails;
        private readonly ResourceLoader resourceLoader;
        private Visibility _loadingVisiblity;

        private BackgroundDownloadService _backgroundDownloadService;
        public BackgroundDownloadService BackgroundDownloadService
        {
            get { return _backgroundDownloadService; }
            set { SetProperty(ref _backgroundDownloadService, value); }
        }

        private bool _isInProgress;
        public bool IsInProgress
        {
            get { return _isInProgress; }
            set { SetProperty(ref _isInProgress, value); }
        }

        private string _progressText;
        public string ProgressText
        {
            get { return _progressText; }
            set { SetProperty(ref _progressText, value); }
        }

        public string UserName
        {
            get { return _userName; }
            set { SetProperty(ref _userName, value); }
        }

        public string Pin
        {
            get { return _pin; }
            set { SetProperty(ref _pin, value); }
        }

        public bool IsLoginSuccessful
        {
            get { return _isLoginSuccessful; }
            set { SetProperty(ref _isLoginSuccessful, value); }
        }
        public bool IsDataDownloadSuccessful
        {
            get { return _isDataDownloadSuccessful; }
            set { SetProperty(ref _isDataDownloadSuccessful, value); }
        }

        public bool IsDatabaseFileDownloadSuccessful
        {
            get { return _isDataFileSuccessful; }
            set { SetProperty(ref _isDataFileSuccessful, value); }
        }

        public LoginUIModel LoginUserDetails
        {
            get { return _loginUserDetails; }
            set { SetProperty(ref _loginUserDetails, value); }
        }

        public Visibility LoadingVisibility
        {
            get { return _loadingVisiblity; }
            set { SetProperty(ref _loadingVisiblity, value); }
        }

        #endregion

        #region Command
        private ICommand _loginButtonCommand;
        public ICommand LoginButtonCommand => _loginButtonCommand ?? (_loginButtonCommand = new AsyncRelayCommand(UserLoginValidations));

        public ICommand OnNavigatedToCommand { get; private set; }
        #endregion

        #region Constructor
        public LoginPageViewModel()
        {
            resourceLoader = ResourceLoader.GetForCurrentView();

            LoadingVisibility = Visibility.Collapsed;
            ProgressText = string.Empty;

            OnNavigatedToCommand = new AsyncRelayCommand<bool>(OnNavigatedToCommandHandler);
        }
        #endregion

        #region Private Methods
        private async Task OnNavigatedToCommandHandler(bool IsResetUser)
        {
            string dbFilePath = string.Empty;
            bool IsDbFileDownloadSuccessful = false;
            try
            {
                if (IsResetUser)
                {
                    // Delete any existing file 
                    DeleteUserFiles();

                    IsInProgress = true;

                    BackgroundDownloadService = new BackgroundDownloadService();

                    await AuthenticUser(((App)Application.Current).LoginUserNameProperty, ((App)Application.Current).LoginUserPinProperty);

                    var filePath = await BackgroundDownloadService.DownloadFile(LoginUserDetails.dbfilename.Trim(), HelperMethods.GetNameFromURL(LoginUserDetails.dbfilename.Trim()));
                    
                    dbFilePath = filePath;

                    var isFileExist = File.Exists(filePath);

                    var isDataExistInFile = new FileInfo(filePath).Length != 0;
                    
                    if (!string.IsNullOrWhiteSpace(filePath) && isFileExist && isDataExistInFile)
                    {
                        var zipFile = await StorageFile.GetFileFromPathAsync(filePath);

                        try
                        {
                            var tempFolder = Path.Combine(ApplicationData.Current.LocalFolder.Path, "tempFolder");

                            ZipFile.ExtractToDirectory(filePath, tempFolder, true);

                            var _iStorageFolder = await StorageFolder.GetFolderFromPathAsync(tempFolder);

                            var _iStorageFile = await _iStorageFolder?.GetFileAsync(ApplicationConstants.DATABASE_NAME);

                            //await Task.Delay(50);

                            await _iStorageFile?.CopyAsync(ApplicationData.Current.LocalFolder, ApplicationConstants.DATABASE_NAME, NameCollisionOption.ReplaceExisting);

                            IsDbFileDownloadSuccessful = true;

                            //await Task.Delay(50);

                            await _iStorageFolder?.DeleteAsync();

                            await zipFile.DeleteAsync();
                        }
                        catch (Exception ex)
                        {
                            IsDbFileDownloadSuccessful = false;
                            IsInProgress = false;
                            ErrorLogger.WriteToErrorLog(nameof(LoginPageViewModel), "OnNavigatedToCommandHandler", ex.StackTrace + " - " + ex.Message);

                            await zipFile.DeleteAsync();

                            File.Delete(filePath);

                            await ResetUserFail();
                        }
                    }
                    else
                    {
                        if (File.Exists(filePath))
                        {
                            File.Delete(filePath);
                        }

                        IsDbFileDownloadSuccessful = false;
                        IsInProgress = false;
                    }

                    if (IsDbFileDownloadSuccessful)
                    {
                        ProgressText = "Data Sync in Progress";
                        LoadingVisibilityHandler(true);
                        IsInProgress = false;

                        if (!Directory.Exists(ApplicationConstants.APP_PATH + @"\BrandImages\"))
                        { await DataSyncHelper.DownloadBrandImages(); }

                        if (DateTime.TryParse(LoginUserDetails.lastsyncutcdate, new CultureInfo("en-US"), DateTimeStyles.None, out DateTime outDate))
                        {
                            ((App)Application.Current).LastSyncDateTimeProperty = DateTimeHelper.ConvertToDbInsertDateTimeFormat(outDate);
                        }

                        string oldterritoryid = await AppReference.QueryService.GetTerritoriesBeforeSyncOfUserAsync(((App)Application.Current).LoginUserNameProperty,
                            ((App)Application.Current).LoginUserPinProperty);
                        var downloadedData = await InvokeWebService.DataDownloadService(((App)Application.Current).LoginUserNameProperty,
                            Convert.ToInt32(((App)Application.Current).LoginUserPinProperty), ((App)Application.Current).LastSyncDateTimeProperty, oldterritoryid);

                        var deserialized = JsonConvert.DeserializeObject<SyncDataModel>(downloadedData);

                        if (deserialized != null && Convert.ToInt32(deserialized.responsestatus) == 200)
                        {
                            if ((!string.IsNullOrEmpty(deserialized.errormsg) && deserialized.errormsg.ToLower().Contains("exceeded")) || string.IsNullOrEmpty(deserialized.errormsg))
                            {
                                await AppReference.QueryService.DownloadDataOnPartialSync(deserialized, ((App)Application.Current).LoginUserIdProperty);

                                ((App)Application.Current).LastSyncDateTimeProperty = deserialized.lastsyncutcdate;

                                if (!string.IsNullOrWhiteSpace(LoginUserDetails.lastsyncutcdate))
                                {
                                    UserName = ((App)Application.Current).LoginUserNameProperty;
                                    Pin = ((App)Application.Current).LoginUserPinProperty;
                                    //On Reset Honey App Get Product additional document based on last successful date 
                                    string userLastSyncDateTime = await AppReference.QueryService.GetConfigurationValueAsync(Constants.Constants.LastSuccessfulSyncDateTime);
                                    if (string.IsNullOrWhiteSpace(userLastSyncDateTime))
                                        userLastSyncDateTime = LoginUserDetails.lastsyncutcdate;

                                    await PartialDownloadSRC(userLastSyncDateTime);
                                }

                                if ((!string.IsNullOrEmpty(deserialized.errormsg) && deserialized.errormsg.ToLower().Contains("exceeded")))
                                {
                                    var syncSuccessStatusDialog = new ContentDialog
                                    {
                                        Content = "Due to data size, only partial customer updates have synced. Please reset the user file in Honey settings tomorrow.",
                                        CloseButtonText = resourceLoader.GetString("OK")
                                    };

                                    await syncSuccessStatusDialog.ShowAsync();
                                }
                            }
                        }

                        //await Task.Delay(100);

                        await InsertLoggedInUserAsZeroCustomer(((App)Application.Current).LoginUserNameProperty, ((App)Application.Current).LoginUserPinProperty);

                        ((App)Application.Current).IsUserAlreadyLogin = true;
                        ((App)Application.Current).IsSyncSuccessProperty = true;
                        ((App)Application.Current).IsDataSyncSuccessAfterLogin = true;

                        ((App)Application.Current).CartItemCount = 0;
                        ((App)Application.Current).CurrentOrderId = 0;
                        ((App)Application.Current).CartDataFromScreen = 0;
                        ((App)Application.Current).SelectedCustomerId = string.Empty;
                        ((App)Application.Current).PreviousSelectedCustomerId = string.Empty;
                        ((App)Application.Current).CurrentDeviceOrderId = string.Empty;
                        ((App)Application.Current).AreaUserSelectedId = 0;

                        await Task.Delay(50);

                        ProgressText = "";

                        LoadingVisibilityHandler(false);

                        NavigateToDashboardPage();
                    }
                    else
                    {
                        await ResetUserFail();
                    }
                }
                else
                {
                    var isremeber = AppReference.IsRememberMe;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(GetType().Name, "OnNavigatedToCommandHandler", ex);

                if (!string.IsNullOrEmpty(dbFilePath) && File.Exists(dbFilePath))
                {
                    File.Delete(dbFilePath);
                }

                await ResetUserFail();
                ProgressText = "";
                IsDbFileDownloadSuccessful = false;
                IsInProgress = false;
                LoadingVisibilityHandler(false);
            }
        }

        private async Task ResetUserFail()
        {
            LoadingVisibilityHandler(false);

            ((App)Application.Current).LoginUserNameProperty = string.Empty;
            ((App)Application.Current).LoginUserPinProperty = string.Empty;
            ((App)Application.Current).LoginUserIdProperty = string.Empty;
            ((App)Application.Current).LastSyncDateTimeProperty = string.Empty;
            ((App)Application.Current).UserDbFileNameProperty = string.Empty;

            ContentDialog errorDialog = new ContentDialog
            {
                Title = "Reset User",
                Content = "Something went wrong, could not reset user file at this moment. Please try login again after some time.",
                CloseButtonText = "OK"
            };

            await errorDialog.ShowAsync();
        }

        private async void NoConnectionPopupAsync()
        {
            ContentDialog emptyFieldDialog = new ContentDialog
            {
                Title = resourceLoader.GetString("LoginErrorTitleText"),
                Content = resourceLoader.GetString("LoginEmptyFieldsError"),
                CloseButtonText = resourceLoader.GetString("OK")
            };

            await emptyFieldDialog.ShowAsync();
        }

        private async Task UserLoginValidations()
        {
            try
            {
                if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Pin))
                {
                    NoConnectionPopupAsync();
                }
                else
                {
                    LoadingVisibilityHandler(true);
                    ProgressText = "Authenticating User Credentials";

                    if (!IsInternetConnected())
                    {
                        ContentDialog syncSuccessStatusDialog = new ContentDialog
                        {
                            Title = "No Internet Connection",
                            Content = "Please check your internet connectivity.",
                            CloseButtonText = resourceLoader.GetString("OK")
                        };

                        await syncSuccessStatusDialog.ShowAsync();
                        return;
                    }


                    bool isPinParsable = int.TryParse(Pin, out int enteredPin);

                    if (isPinParsable)
                    {
                        await AuthenticUser(UserName, Pin);
                    }
                    else
                    {
                        IsLoginSuccessful = false;
                    }

                    if (IsLoginSuccessful)
                    {
                        await CheckForExistingUserLoginDetails();

                        if (IsDataDownloadSuccessful)
                        {
                            ((App)Application.Current).IsUserAlreadyLogin = true;
                            ((App)Application.Current).IsSyncSuccessProperty = true;
                            ((App)Application.Current).IsDataSyncSuccessAfterLogin = true;
                            ((App)Application.Current).CartItemCount = 0;
                            ((App)Application.Current).CurrentOrderId = 0;
                            ((App)Application.Current).CartDataFromScreen = 0;
                            ((App)Application.Current).SelectedCustomerId = string.Empty;
                            ((App)Application.Current).PreviousSelectedCustomerId = string.Empty;
                            ((App)Application.Current).CurrentDeviceOrderId = string.Empty;
                            ((App)Application.Current).OrderPrintName = string.Empty;
                            ((App)Application.Current).AreaUserSelectedId = 0;

                            if (((App)Application.Current).SqlLiteResetVersion != ApplicationConstants.APPLICATION_VERSION)
                            {
                                ((App)Application.Current).SqlLiteResetVersion = ApplicationConstants.APPLICATION_VERSION;
                            }

                            LoadingVisibilityHandler(false);

                            NavigateToDashboardPage();
                        }
                        else if (IsDatabaseFileDownloadSuccessful || ((App)Application.Current).LoginUserNameProperty.Equals(UserName.ToLower()))
                        {
                            ((App)Application.Current).IsUserAlreadyLogin = true;
                            ((App)Application.Current).CartItemCount = 0;
                            ((App)Application.Current).CurrentOrderId = 0;
                            ((App)Application.Current).CartDataFromScreen = 0;
                            ((App)Application.Current).SelectedCustomerId = string.Empty;
                            ((App)Application.Current).PreviousSelectedCustomerId = string.Empty;
                            ((App)Application.Current).CurrentDeviceOrderId = string.Empty;
                            ((App)Application.Current).AreaUserSelectedId = 0;

                            LoadingVisibilityHandler(false);

                            NavigateToDashboardPage();
                        }
                        else
                        {
                            ((App)Application.Current).IsUserAlreadyLogin = false;

                            ((App)Application.Current).IsSyncSuccessProperty = false;

                            LoadingVisibilityHandler(false);

                            // Download fail message to user
                            ContentDialog dataDownloadFailDialog = new ContentDialog
                            {
                                Title = resourceLoader.GetString("LoginErrorTitleText"),
                                Content = resourceLoader.GetString("FailedDataDownloadMessageText"),
                                CloseButtonText = resourceLoader.GetString("OK")
                            };

                            await dataDownloadFailDialog.ShowAsync();
                        }
                    }
                    else
                    {
                        LoadingVisibilityHandler(false);

                        // Login failed user message
                        ContentDialog loginFailDialog = new ContentDialog();

                        if (Convert.ToInt32(LoginUserDetails?.responsestatus) == 401 || !isPinParsable)
                        {
                            loginFailDialog.Title = resourceLoader.GetString("LoginErrorTitleText");
                            loginFailDialog.Content = resourceLoader.GetString("InvalidUsernamePinMessageText");
                            loginFailDialog.CloseButtonText = resourceLoader.GetString("OK");
                        }
                        else
                        {
                            loginFailDialog.Title = resourceLoader.GetString("LoginErrorTitleText");
                            loginFailDialog.Content = resourceLoader.GetString("LoginFailMessage");
                            loginFailDialog.CloseButtonText = resourceLoader.GetString("OK");
                        }

                        await loginFailDialog.ShowAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(GetType().Name, "UserLoginValidations", ex.StackTrace);

                LoadingVisibilityHandler(false);
            }
        }

        private async Task InsertLoggedInUserAsZeroCustomer(string userName, string pin)
        {
            await (Application.Current as App).QueryService.AddLoggedInUserAsZeroCustomer(userName, pin);
        }

        private void LoadingVisibilityHandler(bool isLoading)
        {
            LoadingVisibility = isLoading ? Visibility.Visible : Visibility.Collapsed;
        }

        private async Task SaveUserCredentialsToLocalSettings()
        {
            ((App)Application.Current).LoginUserNameProperty = UserName.ToLower().Trim();
            ((App)Application.Current).LoginUserPinProperty = Pin.Trim();
            ((App)Application.Current).LoginUserIdProperty = LoginUserDetails.userid.ToString().Trim();
            ((App)Application.Current).UserDbFileNameProperty = LoginUserDetails.dbfilename.Trim();
        }

        private bool IsInternetConnected()
        {
            ConnectionProfile connections = NetworkInformation.GetInternetConnectionProfile();
            bool internet = (connections != null) &&
                (connections.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess);
            return internet;
        }

        private async Task CheckForExistingUserLoginDetails()
        {
            try
            {
                // for national users
                if ((new int[] { 5, 6, 17 }).Any(x => x == ((App)Application.Current).LoggedInUserRoleId))
                {
                    if (!((App)Application.Current).LoginUserNameProperty.Equals(UserName.ToLower()))
                    {
                        await SaveUserCredentialsToLocalSettings();
                        await InsertLoggedInUserAsZeroCustomer(UserName.ToLower(), Pin);
                    }
                }
                else
                {
                    // for non-national users
                    // If the user exists download data using partial sync
                    if (((App)Application.Current).LoginUserNameProperty.Equals(UserName.ToLower()))
                    {
                        //download BrandImages from server
                        await FetchServerBrandImagesAsync();

                        ProgressText = "Data Sync in Progress";
                        await SyncDataAfterSuccessfulLogin();
                    }
                    else
                    {
                        // Delete any existing file of other user
                        DeleteUserFiles();
                        // Download new database file for the current login user
                        await DownloadDatabaseFileFromServer();
                        if (IsDatabaseFileDownloadSuccessful)
                        {
                            LoadingVisibilityHandler(true);
                            IsInProgress = false;
                            ProgressText = "Data Sync in Progress";

                            //download BrandImages from server
                            await FetchServerBrandImagesAsync();

                            AppReference.PreviousSyncDateTimeProperty = LoginUserDetails.lastsyncutcdate;
                            string SyncSuccessful = await DataSyncHelper.SyncDataAfterUserLogin(UserName.ToLower(), Pin, LoginUserDetails.lastsyncutcdate, Convert.ToString(LoginUserDetails.userid));

                            if (string.IsNullOrWhiteSpace(SyncSuccessful))
                            {
                                if (!string.IsNullOrWhiteSpace(LoginUserDetails.lastsyncutcdate))
                                {
                                    //On Login Honey App Get Product additional document based on last successful date 
                                    string userLastSyncDateTime = await AppReference.QueryService.GetConfigurationValueAsync(Constants.Constants.LastSuccessfulSyncDateTime);
                                    if (string.IsNullOrWhiteSpace(userLastSyncDateTime))
                                        userLastSyncDateTime = LoginUserDetails.lastsyncutcdate;

                                    await PartialDownloadSRC(userLastSyncDateTime);
                                }

                                ((App)Application.Current).IsDataSyncSuccessAfterLogin = true;

                                ((App)Application.Current).LastSyncDateTimeProperty = DataSyncHelper.LatestSyncDateTime;
                            }
                            else if (SyncSuccessful.ToLower().Contains("exceeded", StringComparison.OrdinalIgnoreCase))
                            {
                                var syncSuccessStatusDialog = new ContentDialog
                                {
                                    Content = "Due to data size, only partial customer updates have synced. Please reset the user file in Honey settings tomorrow.",
                                    CloseButtonText = resourceLoader.GetString("OK")
                                };

                                await syncSuccessStatusDialog.ShowAsync();


                                ((App)Application.Current).IsDataSyncSuccessAfterLogin = true;
                                ((App)Application.Current).LastSyncDateTimeProperty = DataSyncHelper.LatestSyncDateTime;
                            }
                            else
                            {
                                ((App)Application.Current).IsDataSyncSuccessAfterLogin = false;

                                ((App)Application.Current).LastSyncDateTimeProperty = LoginUserDetails.lastsyncutcdate.Trim();
                            }

                            // Save current user's credentials into local storage
                            await SaveUserCredentialsToLocalSettings();

                            await InsertLoggedInUserAsZeroCustomer(UserName.ToLower(), Pin);

                            IsDataDownloadSuccessful = true;
                        }
                        else
                        {
                            ((App)Application.Current).IsDataSyncSuccessAfterLogin = false;
                            IsDataDownloadSuccessful = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LoadingVisibilityHandler(false);

                if (ex is WebException) NoConnectionPopupAsync();
                else
                    ErrorHandler.LogAndThrowSpecifiedException("LoginPageViewModel", "CheckForExistingUserLoginDetails", ex);
            }
        }

        private async Task FetchServerBrandImagesAsync()
        {
            if (!Directory.Exists(ApplicationConstants.APP_PATH + @"\BrandImages\"))
            {
                await DataSyncHelper.DownloadBrandImages();
            }
        }

        //if make changes in this method, change the same in dashboard page also
        private async Task PartialDownloadSRC(string dtlastsyncdate)
        {
            try
            {
                var webServiceResponse = await InvokeWebService.GetPartialSRCZIPFile(UserName.ToLower(), Convert.ToInt32(Pin), dtlastsyncdate);

                if (webServiceResponse != null && webServiceResponse.responsestatus.Equals("200") && string.IsNullOrEmpty(webServiceResponse.errormsg) && !string.IsNullOrEmpty(webServiceResponse.partialsrcfilename))
                {
                    var partialSRCPath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, ApplicationConstants.PartialSRCSubFolder);

                    if (Directory.Exists(partialSRCPath))
                    {
                        Directory.Delete(partialSRCPath, true);
                    }

                    var partialSRCPathzipfile = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, LoginUserDetails.userid + "_PartialDownloadSRC.zip");

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
                if (ex is WebException) NoConnectionPopupAsync();
                else
                {
                    ErrorLogger.WriteToErrorLog(GetType().Name, "PartialDownloadSRC", ex);
                }
            }
        }
        private static void NavigateToDashboardPage()
        {
            ////((App)Application.Current).ActiveShellPage();
            NavigationService.NavigateMainFrame(typeof(ShellPage));
        }

        private async Task AuthenticUser(string u_name, string u_pin)
        {
            LoginUserDetails = await InvokeWebService.UserAuthenticateWebService(u_name.ToLower(), Convert.ToInt32(u_pin));

            if (LoginUserDetails != null && string.IsNullOrEmpty(LoginUserDetails.errormsg) && Convert.ToInt32(LoginUserDetails.responsestatus) == 200)
            {
                IsLoginSuccessful = true;
                AppReference.LoggedInUserRoleId = LoginUserDetails.roleid;
            }
            else
            {
                IsLoginSuccessful = false;
            }
        }

        private async Task SyncDataAfterSuccessfulLogin()
        {
            try
            {
                if (LoginUserDetails != null && string.IsNullOrEmpty(LoginUserDetails.errormsg) && Convert.ToInt32(LoginUserDetails.responsestatus) == 200)
                {
                    AppReference.PreviousSyncDateTimeProperty = LoginUserDetails.lastsyncutcdate;
                    string result = await DataSyncHelper.SyncDataAfterUserLogin(UserName.ToLower(), Pin, LoginUserDetails.lastsyncutcdate, Convert.ToString(LoginUserDetails.userid));
                    if (string.IsNullOrWhiteSpace(result))
                    {
                        if (!string.IsNullOrWhiteSpace(LoginUserDetails.lastsyncutcdate))
                            await PartialDownloadSRC(LoginUserDetails.lastsyncutcdate);
                        IsDataDownloadSuccessful = true;
                        ((App)Application.Current).IsDataSyncSuccessAfterLogin = IsDataDownloadSuccessful;
                    }
                    else if (result.ToLower().Contains("exceeded", StringComparison.OrdinalIgnoreCase))
                    {
                        var syncSuccessStatusDialog = new ContentDialog
                        {
                            Content = "Due to data size, only partial customer updates have synced. Please reset the user file in Honey settings tomorrow.",
                            CloseButtonText = resourceLoader.GetString("OK")
                        };
                        await syncSuccessStatusDialog.ShowAsync();
                        IsDataDownloadSuccessful = true;
                        ((App)Application.Current).IsDataSyncSuccessAfterLogin = IsDataDownloadSuccessful;
                    }
                }
                else
                {
                    ((App)Application.Current).IsDataSyncSuccessAfterLogin = false;

                    IsDataDownloadSuccessful = false;
                }
            }
            catch (Exception)
            {
                ((App)Application.Current).IsDataSyncSuccessAfterLogin = false;

                IsDataDownloadSuccessful = false;
            }
        }

        private async Task DownloadDatabaseFileFromServer()
        {
            string dbFilePath = string.Empty;

            try
            {
                LoadingVisibilityHandler(false);

                IsInProgress = true;

                if (LoginUserDetails != null && string.IsNullOrEmpty(LoginUserDetails.errormsg) && Convert.ToInt32(LoginUserDetails.responsestatus) == 200)
                {
                    BackgroundDownloadService = new BackgroundDownloadService();

                    var filePath = await BackgroundDownloadService.DownloadFile(LoginUserDetails.dbfilename, HelperMethods.GetNameFromURL(LoginUserDetails.dbfilename));

                    var isFileExist = File.Exists(filePath);

                    dbFilePath = isFileExist ? filePath : string.Empty;

                    var isDataExistInFile = new FileInfo(filePath).Length != 0;

                    if (!string.IsNullOrWhiteSpace(filePath) && isFileExist && isDataExistInFile)
                    {
                        var zipFile = await StorageFile.GetFileFromPathAsync(filePath);

                        try
                        {
                            var tempFolder = Path.Combine(ApplicationData.Current.LocalFolder.Path, "tempFolder");

                            ZipFile.ExtractToDirectory(filePath, tempFolder, true);

                            var _iStorageFolder = await StorageFolder.GetFolderFromPathAsync(tempFolder);

                            var _iStorageFile = await _iStorageFolder?.GetFileAsync(ApplicationConstants.DATABASE_NAME);

                            await Task.Delay(100);

                            await _iStorageFile?.CopyAsync(ApplicationData.Current.LocalFolder, ApplicationConstants.DATABASE_NAME, NameCollisionOption.ReplaceExisting);

                            IsDatabaseFileDownloadSuccessful = true;

                            await Task.Delay(100);

                            await _iStorageFolder?.DeleteAsync();

                            await zipFile.DeleteAsync();
                        }
                        catch (Exception ex)
                        {
                            IsDatabaseFileDownloadSuccessful = false;

                            await zipFile.DeleteAsync();

                            ErrorLogger.WriteToErrorLog(nameof(LoginPageViewModel), "DownloadDatabaseFileFromServer", ex.StackTrace + " - " + ex.Message);
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(dbFilePath))
                        {
                            File.Delete(dbFilePath);
                        }

                        IsDatabaseFileDownloadSuccessful = false;

                        IsInProgress = false;
                    }

                    // Commented -- This is the old methond to download the file. Uncomment this method if require to download db file in case of dev env.
                    // IsDatabaseFileDownloadSuccessful = await InvokeWebService.DownloadDatabaseFileFromServer(LoginUserDetails.dbfilename, LoginUserDetails.lastsyncutcdate);
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(dbFilePath))
                    {
                        File.Delete(dbFilePath);
                    }

                    IsDatabaseFileDownloadSuccessful = false;

                    IsInProgress = false;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(LoginPageViewModel), nameof(DownloadDatabaseFileFromServer), ex.Message);
                if (!string.IsNullOrWhiteSpace(dbFilePath))
                {
                    File.Delete(dbFilePath);
                }

                IsDatabaseFileDownloadSuccessful = false;

                IsInProgress = false;
            }
        }

        private void DeleteUserFiles()
        {
            string basePath = ApplicationConstants.APP_PATH;

            if (!Directory.Exists(basePath))
            {
                Directory.CreateDirectory(basePath);
            }

            // Delete any existing files at basepath
            string[] files = Directory.GetFiles(basePath);

            foreach (string fileItem in files)
            {
                File.Delete(fileItem);
            }
        }
        //private async void DeleteUserFilesAsync()
        //{
        //    try
        //    {
        //        string basePath = ApplicationConstants.APP_PATH;

        //    // Delete any existing files at basepath
        //    string[] files = Directory.GetFiles(basePath);

        //    foreach (string fileItem in files)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(SettingPageViewModel), nameof(DeleteUserFilesAsync), ex.Message);
        //    }
        //}
        #endregion
    }
}
