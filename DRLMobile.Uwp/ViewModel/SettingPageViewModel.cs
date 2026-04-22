using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

using DRLMobile.Core.Helpers;
using DRLMobile.Core.Models;
using DRLMobile.Core.Models.DataModels;
using DRLMobile.Core.Services;
using DRLMobile.ExceptionHandler;
using DRLMobile.Uwp.Helpers;
using DRLMobile.Uwp.Services;
using DRLMobile.Uwp.View;

using Microsoft.Toolkit.Mvvm.Input;

using Newtonsoft.Json;

using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace DRLMobile.Uwp.ViewModel
{
    public class SettingPageViewModel : BaseModel
    {
        #region Commands
        public ICommand CustomTaxStatementCommand { get; private set; }
        public ICommand LogoutCommand { get; private set; }
        public ICommand ResetUserComand { get; private set; }
        public ICommand DownloadSRCZipCommand { get; private set; }
        public ICommand UpdateAppComand { get; private set; }
        public ICommand OnNavigatingFrom { get; private set; }
        public ICommand ChangePinSaveButtonCommand { get; private set; }
        public ICommand SendLogsCommand { get; private set; }
        public ICommand DownloadCustomerDocumentsCommand { get; private set; }
        public ICommand InstallBuildCommand { get; private set; }
        public ICommand ResetSqlLiteArchiveFlagCommand { get; private set; }

        #endregion

        #region Properties

        private readonly App AppReference = (App)Application.Current;

        public bool IsUpdateAvailable
        {
            get { return AppReference.IsApplicationUpdateAvailable.Value; }
            set { AppReference.IsApplicationUpdateAvailable = value; OnPropertyChanged(); }
        }

        public bool IsUpgradeVersion
        {
            get { return AppReference.IsUpgradeVersion; }
            set { AppReference.IsUpgradeVersion = value; }
        }

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

        private bool _isAppUpdateProgressVisible;
        public bool IsAppUpdateProgressVisible
        {
            get { return _isAppUpdateProgressVisible; }
            set { SetProperty(ref _isAppUpdateProgressVisible, value); }
        }

        private bool _isSrcZipProgressVisible;
        public bool IsSrcZipProgressVisible
        {
            get { return _isSrcZipProgressVisible; }
            set { SetProperty(ref _isSrcZipProgressVisible, value); }
        }

        private Visibility _loadingVisiblity;
        public Visibility LoadingVisibility
        {
            get { return _loadingVisiblity; }
            set { SetProperty(ref _loadingVisiblity, value); }
        }


        private string _oldPinText;
        public string OldPinText
        {
            get { return _oldPinText; }
            set { SetProperty(ref _oldPinText, value); }
        }

        private string _newPinText;
        public string NewPinText
        {
            get { return _newPinText; }
            set { SetProperty(ref _newPinText, value); }
        }


        private string _confirmNewPinText;
        public string ConfirmNewPinText
        {
            get { return _confirmNewPinText; }
            set { SetProperty(ref _confirmNewPinText, value); }
        }

        private string _errorForOldPinText;
        public string ErrorForOldPinText
        {
            get { return _errorForOldPinText; }
            set { SetProperty(ref _errorForOldPinText, value); }
        }



        private string _errorForNewPinText;
        public string ErrorForNewPinText
        {
            get { return _errorForNewPinText; }
            set { SetProperty(ref _errorForNewPinText, value); }
        }

        private string _errorForConfirmNewPinText;
        public string ErrorForConfirmNewPinText
        {
            get { return _errorForConfirmNewPinText; }
            set { SetProperty(ref _errorForConfirmNewPinText, value); }
        }

        #endregion

        #region Constructor
        public SettingPageViewModel()
        {
            CustomTaxStatementCommand = new RelayCommand(CustomTaxStatementCommandHandler);
            LogoutCommand = new AsyncRelayCommand(LogoutCommandHandler);
            ResetUserComand = new AsyncRelayCommand(ResetUserComandHandler);
            DownloadSRCZipCommand = new AsyncRelayCommand(DownloadSRCZipCommandHandler);
            UpdateAppComand = new AsyncRelayCommand(UpdateAppComandHandler);
            OnNavigatingFrom = new RelayCommand(OnNavigatingFromHandler);
            ChangePinSaveButtonCommand = new AsyncRelayCommand<Flyout>(ChangePinSaveButtonCommandHandler);
            SendLogsCommand = new AsyncRelayCommand(SendLogsCommandHandler);
            DownloadCustomerDocumentsCommand = new AsyncRelayCommand(DownloadCustomerDocumentZipCommandHandler);
            InstallBuildCommand = new AsyncRelayCommand(InstallBuildCommandHandler);
            IsInProgress = false;
            LoadingVisibility = Visibility.Collapsed;
            ResetSqlLiteArchiveFlagCommand = new AsyncRelayCommand(ResetSqlLiteArchiveFlagCommandHandler);
        }

        #endregion

        #region Private Methods

        private async Task InstallBuildCommandHandler()
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
                    AppReference.IsApplicationUpdateAvailable = false;
                    IsUpgradeVersion = false;

                    LoadingVisibility = Visibility.Visible;

                    var notificationModel = JsonConvert.DeserializeObject<NotificationModel>(AppReference.NotificationContent);

                    var downloadFilePath = await InvokeWebService.DownloadAppUpdateFile(notificationModel.downloadurl, HelperMethods.GetNameFromURL(notificationModel.downloadurl));

                    if (!string.IsNullOrWhiteSpace(downloadFilePath))
                    {
                        StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

                        StorageFile file = await storageFolder.CreateFileAsync(HelperMethods.GetNameFromURL(notificationModel.downloadurl), Windows.Storage.CreationCollisionOption.OpenIfExists);

                        var options = new Windows.System.LauncherOptions();

                        options.DisplayApplicationPicker = false;

                        await Windows.System.Launcher.LaunchFileAsync(file, options);

                        AppReference.ShowUserPopupToUpdate = false;

                        Application.Current.Exit();
                    }

                    LoadingVisibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(SettingPageViewModel), nameof(InstallBuildCommandHandler), ex.StackTrace + " - " + ex.Message);
            }
        }

        private async Task SendLogsCommandHandler()
        {
            try
            {
                var folder = ApplicationConstants.APP_PATH;
                var logBasePath = Path.Combine(folder, "ErrorLogFile");
                ICollection<string> sendFileList = new List<string>();
                if (Directory.Exists(logBasePath))
                {
                    foreach (string filePath in Directory.GetFiles(logBasePath, $"*{DateTime.Today.ToString("dd-MM-yy")}.txt"))
                    {
                        await Task.Run(() => { if (File.Exists(filePath)) sendFileList.Add(filePath); });
                    }
                    foreach (string filePath in Directory.GetFiles(logBasePath, $"*{DateTime.Today.AddDays(-1).ToString("dd-MM-yy")}.txt"))
                    {
                        await Task.Run(() => { if (File.Exists(filePath)) sendFileList.Add(filePath); });
                    }
                }

                if (sendFileList.Any())
                {
                    EmailModel model = new EmailModel() { AttachmentListByPath = sendFileList };
                    await EmailService.Instance.SendMailFromOutlook(model);
                }
                else
                {
                    var message = string.Format("Log file is not found for {0} and {1} dates.", DateTime.Today.ToString("dd-MM-yy"), DateTime.Today.AddDays(-1).ToString("dd-MM-yy"));
                    await AlertHelper.Instance.ShowConfirmationAlert(ResourceExtensions.GetLocalized("ALERT"), message, ResourceExtensions.GetLocalized("OK"), ResourceExtensions.GetLocalized("OK"));
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(SettingPageViewModel), nameof(SendLogsCommandHandler), ex.StackTrace + " - " + ex.Message);
            }
        }

        private async Task ResetSqlLiteArchiveFlagCommandHandler()
        {
            //((App)Application.Current).IsSqlLiteArchivedReset = false;
        }
        private async Task ChangePinSaveButtonCommandHandler(Flyout flyout)
        {
            try
            {
                NavigationService.LoadingOnShellPage(true);

                var userId = Convert.ToInt32(AppReference.LoginUserIdProperty);
                var oldPin = Convert.ToInt32(AppReference.LoginUserPinProperty);

                var isEmpty = CheckForEmptyPinFields();
                if (!isEmpty)
                {
                    if (!AppReference.LoginUserPinProperty.Equals(OldPinText))
                    {
                        ErrorForOldPinText = "Current PIN and the entered PIN did not matched";
                    }
                    else
                    {
                        bool isValidRequest = IsValidPinChangeRequest();
                        if (isValidRequest)
                        {
                            flyout.Hide();
                            var newPin = Convert.ToInt32(NewPinText);
                            var response = await InvokeWebService.ChangePinService(userId, newPin, AppReference.LoginUserNameProperty, oldPin);

                            if (!string.IsNullOrEmpty(response))
                            {
                                var responseModel = JsonConvert.DeserializeObject<SyncDataModel>(response, new JsonSerializerSettings { Error = DeserilizationErrorHandler, NullValueHandling = NullValueHandling.Ignore });

                                if (responseModel.responsestatus == "200")
                                {
                                    //update user master
                                    var userData = await AppReference.QueryService.GetUserData(AppReference.LoginUserNameProperty, AppReference.LoginUserPinProperty);
                                    userData.IsExported = 0;
                                    userData.PIN = newPin;
                                    await AppReference.QueryService.UpdateUserMaster(userData);
                                    _ = AlertHelper.Instance.ShowConfirmationAlert("Success", " PIN Updated Successfully!", "OK");
                                    AppReference.LoginUserPinProperty = NewPinText;

                                }
                                else if (responseModel.responsestatus == "401")
                                {
                                    _ = AlertHelper.Instance.ShowConfirmationAlert("Error", "Current pin is not valid!", "OK");
                                }
                                else
                                {
                                    _ = AlertHelper.Instance.ShowConfirmationAlert("Error", "Error while updating PIN!", "OK");
                                }
                            }
                            else
                            {
                                _ = AlertHelper.Instance.ShowConfirmationAlert("Failure", "Server not responding", "OK");
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(SettingPageViewModel), nameof(ChangePinSaveButtonCommandHandler), ex.StackTrace + " - " + ex.Message);
            }
            finally
            {
                NavigationService.LoadingOnShellPage(false);
            }
        }

        private void DeserilizationErrorHandler(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs e)
        {
            e.ErrorContext.Handled = true;
        }

        private bool IsValidPinChangeRequest()
        {
            bool isValid = true;
            if (!NewPinText.Equals(ConfirmNewPinText))
            {
                isValid = false;
                ErrorForNewPinText = "PIN did not matched";
                ErrorForConfirmNewPinText = "PIN did not matched";
            }

            return isValid;
        }

        private bool CheckForEmptyPinFields()
        {
            var isEmpty = false;
            if (string.IsNullOrWhiteSpace(OldPinText))
            {
                isEmpty = true;
                ErrorForOldPinText = "Please enter old PIN";
            }
            if (string.IsNullOrWhiteSpace(NewPinText))
            {
                isEmpty = true;
                ErrorForNewPinText = "Please enter new PIN";
            }
            if (string.IsNullOrWhiteSpace(ConfirmNewPinText))
            {
                isEmpty = true;
                ErrorForConfirmNewPinText = "Please enter confirm PIN";
            }
            return isEmpty;
        }

        private async Task UpdateAppComandHandler()
        {
            try
            {
                var result = await AlertHelper.Instance.ShowConfirmationAlert(ResourceExtensions.GetLocalized("ALERT"), ResourceExtensions.GetLocalized("CLOSE_APP_MSG"), ResourceExtensions.GetLocalized("YesText"), ResourceExtensions.GetLocalized("NoText"));
                if (result)
                {
                    IsInProgress = true;
                    IsAppUpdateProgressVisible = true;
                    IsSrcZipProgressVisible = false;
                    var notificationModel = JsonConvert.DeserializeObject<NotificationModel>(AppReference.NotificationContent);
                    var downloadFilePath = await InvokeWebService.DownloadAppUpdateFile(notificationModel.downloadurl, Core.Helpers.HelperMethods.GetNameFromURL(notificationModel.downloadurl));
                    if (!string.IsNullOrWhiteSpace(downloadFilePath))
                    {
                        StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
                        StorageFile file = await storageFolder.CreateFileAsync(HelperMethods.GetNameFromURL(notificationModel.downloadurl), CreationCollisionOption.OpenIfExists);
                        var options = new Windows.System.LauncherOptions();
                        options.DisplayApplicationPicker = false;
                        await Windows.System.Launcher.LaunchFileAsync(file, options);
                        AppReference.ShowUserPopupToUpdate = false;
                        IsUpdateAvailable = false;
                        Application.Current.Exit();
                    }
                    IsInProgress = false;
                    IsAppUpdateProgressVisible = false;
                }
            }
            catch (Exception ex)
            {
                IsAppUpdateProgressVisible = false;
                IsInProgress = false;
                ErrorLogger.WriteToErrorLog(nameof(SettingPageViewModel), nameof(UpdateAppComandHandler), ex.Message);
            }
        }

        private async Task DownloadCustomerDocumentZipCommandHandler()
        {
            try
            {
                IsInProgress = true;
                IsSrcZipProgressVisible = true;
                IsAppUpdateProgressVisible = false;

                var webServiceResponse = await InvokeWebService.GetCustomerDocumentZIPFile(AppReference.LoginUserNameProperty, Convert.ToInt32(AppReference.LoginUserPinProperty));

                if (webServiceResponse != null && webServiceResponse.responsestatus.Equals("200") && string.IsNullOrEmpty(webServiceResponse.errormsg) && !string.IsNullOrEmpty(webServiceResponse.custdocfilename))
                {
                    var customerDocZipUrl = webServiceResponse.custdocfilename;

                    var fileNameWithExtension = customerDocZipUrl.Substring(customerDocZipUrl.LastIndexOf("/") + 1);

                    var fileName = fileNameWithExtension.Substring(0, fileNameWithExtension.IndexOf('.'));

                    BackgroundDownloadService = new BackgroundDownloadService();

                    var path = await BackgroundDownloadService?.DownLoadCustomerDocFile(customerDocZipUrl, fileNameWithExtension);

                    if (File.Exists(path))
                    {
                        var custDocumentPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, ApplicationConstants.CustomerDocumentsSubFolder);

                        if (!Directory.Exists(custDocumentPath))
                        {
                            Directory.CreateDirectory(custDocumentPath);
                        }

                        ZipFile.ExtractToDirectory(sourceArchiveFileName: path, custDocumentPath, overwriteFiles: true);

                        await Task.Delay(200);

                        File.Delete(path);

                        IsInProgress = false;
                    }
                    else
                    {
                        var res = await AlertHelper.Instance.ShowConfirmationAlert("Error", "Customer document zip file not found", "OK");

                        if (res)
                        {
                            IsInProgress = false;
                            IsSrcZipProgressVisible = false;
                            IsAppUpdateProgressVisible = false;
                        }
                    }
                }
                else
                {
                    var res = await AlertHelper.Instance.ShowConfirmationAlert("Error", "Customer document zip file not found", "OK");

                    if (res)
                    {
                        IsInProgress = false;
                        IsSrcZipProgressVisible = false;
                        IsAppUpdateProgressVisible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(SettingPageViewModel), "DownloadCustDocumentZipCommandHandler", ex);
            }
        }

        private async Task DownloadSRCZipCommandHandler()
        {
            try
            {
                IsInProgress = true;
                IsSrcZipProgressVisible = true;
                IsAppUpdateProgressVisible = false;

                BackgroundDownloadService = new BackgroundDownloadService();
                string localPath = ApplicationData.Current.LocalFolder.Path;
                string srcZipPath = Path.Combine(localPath, ApplicationConstants.SrzZipFileName);
                string srcZipFilePath = Path.Combine(localPath, ApplicationConstants.SrzFileName);
                if (File.Exists(srcZipPath))
                {
                    File.Delete(srcZipPath);
                }
                if (Directory.Exists(srcZipFilePath))
                {
                    Directory.Delete(srcZipFilePath, true);
                }

                var path = await BackgroundDownloadService?.DownloadFile(ApplicationConstants.SrzZipDownloadUrl, ApplicationConstants.SrzZipFileName);

                if (File.Exists(path))
                {
                    //var destination = Path.Combine(ApplicationData.Current.LocalFolder.Path, ApplicationConstants.SrzFileName);
                    //var destination = ApplicationData.Current.LocalFolder.Path;

                    using (ZipArchive zip = ZipFile.Open(path, ZipArchiveMode.Read))
                    {
                        foreach (ZipArchiveEntry entry in zip.Entries)
                        {
                            if (!entry.FullName.StartsWith(ApplicationConstants.SrzFileName))
                            {
                                localPath = Path.Combine(localPath, ApplicationConstants.SrzFileName);
                                break;
                            }
                            break;
                        }
                    }

                    ZipFile.ExtractToDirectory(sourceArchiveFileName: path, localPath, overwriteFiles: true);

                    await Task.Delay(200);

                    File.Delete(path);

                    #region Copy SRCZip BrandStyle folder to BrandImages

                    string sourceBrandPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, ApplicationConstants.SrzFileName, ApplicationConstants.SRCZipBrandStyle);
                    string targetBrandPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, ApplicationConstants.BrandImageBaseFolder);

                    if (Directory.Exists(sourceBrandPath))
                    {
                        if (!Directory.Exists(targetBrandPath))
                        {
                            Directory.CreateDirectory(targetBrandPath);
                        }
                        string destFile = string.Empty;
                        string[] files = Directory.GetFiles(sourceBrandPath);

                        // Copy the files and overwrite destination files if they already exist. 
                        foreach (string s in files)
                        {
                            try
                            {
                                // Use static Path methods to extract only the file name from the path.
                                destFile = Path.Combine(targetBrandPath, Path.GetFileName(s));
                                File.Copy(s, destFile, true);
                            }
                            catch (Exception ex1)
                            {
                                ErrorLogger.WriteToErrorLog(nameof(SettingPageViewModel), nameof(DownloadSRCZipCommandHandler), ex1);
                            }
                        }
                    }
                    #endregion End Copy SRCZip BrandStyle folder to BrandImages

                    IsInProgress = false;
                }
                else
                {
                    var res = await AlertHelper.Instance.ShowConfirmationAlert("Error", "File not found", "OK");

                    if (res)
                    {
                        IsInProgress = false;
                        IsSrcZipProgressVisible = false;
                        IsAppUpdateProgressVisible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(SettingPageViewModel), "DownloadSRCZipCommandHandler", ex.StackTrace);
            }
        }

        private void OnNavigatingFromHandler()
        {
            LoadingVisibility = Visibility.Collapsed;

            BackgroundDownloadService?.CancelDownload();
        }

        private void CustomTaxStatementCommandHandler()
        {
            NavigationService.NavigateShellFrame(typeof(CustomTaxStatementPage));
        }

        private async Task LogoutCommandHandler()
        {
            try
            {
                ContentDialog userLogoutDialog = new ContentDialog
                {
                    Title = ResourceExtensions.GetLocalized("LogoutTitleText"),
                    Content = ResourceExtensions.GetLocalized("LogoutMessage"),
                    PrimaryButtonText = ResourceExtensions.GetLocalized("YesText"),
                    SecondaryButtonText = ResourceExtensions.GetLocalized("NoText")
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
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(SettingPageViewModel), nameof(LogoutCommandHandler), ex.Message);
            }

        }

        private async Task ResetUserComandHandler()
        {
            // for national users
            if ((new int[] { 5, 6, 17 }).Any(x => x == ((App)Application.Current).LoggedInUserRoleId))
            {
                ContentDialog syncSuccessStatusDialog = new ContentDialog
                {
                    Title = ResourceExtensions.GetLocalized("ResetUserText"),
                    Content = $"{ResourceExtensions.GetLocalized("ResetUserText")} feature is currently unavailable for national users.",
                    PrimaryButtonText = "OK",
                };
                await syncSuccessStatusDialog.ShowAsync();
                syncSuccessStatusDialog.Hide();
            }
            else
            {
                // for non-national users
                try
                {
                    ContentDialog userLogoutDialog = new ContentDialog
                    {
                        Title = ResourceExtensions.GetLocalized("ResetUserText"),
                        Content = ResourceExtensions.GetLocalized("ResetMessage"),
                        PrimaryButtonText = ResourceExtensions.GetLocalized("YesText"),
                        SecondaryButtonText = ResourceExtensions.GetLocalized("NoText")
                    };

                    ContentDialogResult result = await userLogoutDialog.ShowAsync();

                    if (result == ContentDialogResult.Primary)
                    {
                        try
                        {
                            LoadingVisibility = Visibility.Visible;

                            //ErrorLogger.WriteToErrorLog("SettingPageViewModel", "ResetUserComandHandler", "**** Last Sync cut date passed from mobile app during data upload on reset user operation **** : " + AppReference.LastSyncDateTimeProperty);

                            await AppReference.QueryService.UploadDataOnPartialSync(AppReference.LoginUserNameProperty,
                                AppReference.LoginUserPinProperty, AppReference.LastSyncDateTimeProperty);

                            DeleteUserFiles();

                            //DeleteUserFilesAsync();
                            ResetApplicationProperties();

                            var frame = Window.Current.Content as Frame;

                            LoadingVisibility = Visibility.Collapsed;

                            frame.Navigate(typeof(LoginPage), true);



                            //foreach (var item in frame.BackStack)
                            //{
                            //    frame.BackStack.Remove(item);
                            //}
                        }
                        catch (Exception ex)
                        {
                            ErrorLogger.WriteToErrorLog(nameof(SettingPageViewModel), nameof(ResetUserComandHandler), ex.StackTrace);

                            LoadingVisibility = Visibility.Collapsed;

                            ContentDialog errorDialog = new ContentDialog
                            {
                                Title = "Reset User",
                                Content = "Something went wrong, could not reset user file at this moment. Please try again after some time.",
                                CloseButtonText = "OK"
                            };

                            await errorDialog.ShowAsync();
                        }
                    }
                    else
                    {
                        userLogoutDialog.Hide();
                    }
                }
                catch (Exception ex)
                {
                    ErrorLogger.WriteToErrorLog(nameof(SettingPageViewModel), nameof(ResetUserComandHandler), ex.Message);
                }
            }
        }


        private void DeleteUserFiles()
        {
            try
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
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(SettingPageViewModel), nameof(DeleteUserFiles), ex.Message);
            }
        }


        private void ResetApplicationProperties()
        {
            try
            {
                AppReference.CartItemCount = 0;
                AppReference.CurrentOrderId = 0;
                AppReference.CartDataFromScreen = 0;
                AppReference.IsCreditRequestOrder = false;
                AppReference.IsOrderTypeChanged = false;
                AppReference.IsDistributionOptionClicked = false;
                AppReference.SelectedCustomerId = string.Empty;
                AppReference.PreviousSelectedCustomerId = string.Empty;
                AppReference.CurrentDeviceOrderId = string.Empty;
                AppReference.IsCustomerActivity = false;

                AppReference.IsUserAlreadyLogin = false;
                AppReference.IsSyncSuccessProperty = false;
                AppReference.IsDataSyncSuccessAfterLogin = false;
                AppReference.AreaUserSelectedId = 0;
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(SettingPageViewModel), nameof(ResetApplicationProperties), ex.Message);
            }
        }

        #endregion
    }
}
