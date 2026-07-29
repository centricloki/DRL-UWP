using DRLMobile.Core.Helpers;
using DRLMobile.Core.Models;
using DRLMobile.Core.Models.DataModels;
using DRLMobile.Core.Services;
using DRLMobile.ExceptionHandler;
using DRLMobile.Helpers;
using DRLMobile.Services;
using DRLMobile.Views;
using Microsoft.Toolkit.Mvvm.Input;
using Newtonsoft.Json;
using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace DRLMobile.ViewModels
{
    public class SettingPageViewModel : BaseModel
    {

        #region command
        public ICommand CustomTaxStatementCommand { get; private set; }
        public ICommand LogoutCommand { get; private set; }
        public ICommand DownloadSRCZipCommand { get; private set; }
        public ICommand UpdateAppComand { get; private set; }
        public ICommand OnNavigatingFrom { get; private set; }

        #endregion


        #region properties
        private readonly App AppReference = (App)(Application.Current);

        public bool IsUpdateAvailable
        {
            get { return AppReference.IsApplicationUpdateAvailable.Value; }
            set { AppReference.IsApplicationUpdateAvailable = value; OnPropertyChanged(); }
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



        #endregion


        #region constructor
        public SettingPageViewModel()
        {
            CustomTaxStatementCommand = new RelayCommand(CustomTaxStatementCommandHandler);
            LogoutCommand = new AsyncRelayCommand(LogoutCommandHandler);
            DownloadSRCZipCommand = new AsyncRelayCommand(DownloadSRCZipCommandHandler);
            UpdateAppComand = new AsyncRelayCommand(UpdateAppComandHandler);
            OnNavigatingFrom = new RelayCommand(OnNavigatingFromHandler);
            IsInProgress = false;
        }


        #endregion


        #region private methods

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
                    var downloadFilePath = await InvokeWebService.DownloadAppUpdateFile(notificationModel.DownloadUrl, Core.Helpers.HelperMethods.GetNameFromURL(notificationModel.DownloadUrl));
                    if (!string.IsNullOrWhiteSpace(downloadFilePath))
                    {
                        StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
                        StorageFile file = await storageFolder.CreateFileAsync(HelperMethods.GetNameFromURL(notificationModel.DownloadUrl), CreationCollisionOption.OpenIfExists);
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


        private async Task DownloadSRCZipCommandHandler()
        {
            ///await InvokeWebService.SrcZipTryOut();
            /// await BackgroundDownloadService?.DownloadFile("https://10.4.0.87:14989/Content/SRCZip.zip", "SrcZip.Zip");
            IsInProgress = true;
            IsSrcZipProgressVisible = true;
            IsAppUpdateProgressVisible = false;
            BackgroundDownloadService = new BackgroundDownloadService();
            var path = await BackgroundDownloadService?.DownloadFile(ApplicationConstants.SrzZipDownloadUrl, ApplicationConstants.SrzZipFileName);
            try
            {
                var destination = Path.Combine(ApplicationData.Current.LocalFolder.Path, ApplicationConstants.SrzFileName);
                ZipFile.ExtractToDirectory(sourceArchiveFileName: path, destination,overwriteFiles: true);
                await Task.Delay(200);
                File.Delete(path);
                IsInProgress = false;
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(SettingPageViewModel), nameof(DownloadSRCZipCommandHandler), ex.StackTrace);
                IsInProgress = false;
                IsSrcZipProgressVisible = false;
                IsAppUpdateProgressVisible = false;
            }
        }

        private void OnNavigatingFromHandler()
        {
            BackgroundDownloadService?.CancelDownload();
        }

        private void CustomTaxStatementCommandHandler()
        {
            NavigationService.Navigate<CustomTaxStatementPage>();
        }


        private async Task LogoutCommandHandler()
        {
            ContentDialog userLogoutDialog = new ContentDialog
            {
                Title = Helpers.ResourceExtensions.GetLocalized("LogoutTitleText"),
                Content = Helpers.ResourceExtensions.GetLocalized("LogoutMessage"),
                PrimaryButtonText = Helpers.ResourceExtensions.GetLocalized("YesText"),
                SecondaryButtonText = Helpers.ResourceExtensions.GetLocalized("NoText")
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

        #endregion




    }
}
