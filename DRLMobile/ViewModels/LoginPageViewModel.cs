using DRLMobile.Core.Helpers;
using DRLMobile.Core.Models.UIModels;
using DRLMobile.Core.Services;
using DRLMobile.ExceptionHandler;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace DRLMobile.ViewModels
{
    public class LoginPageViewModel : ObservableObject
    {
        #region Properties

        private string _userName;
        private string _pin;
        private bool _isLoginSuccessful;
        private bool _isDataDownloadSuccessful;
        private bool _isDataFileSuccessful;
        private LoginUIModel _loginUserDetails;
        private readonly ResourceLoader resourceLoader;
        private Visibility _loadingVisiblity;

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
        public ICommand LoginButtonCommand => _loginButtonCommand ?? (_loginButtonCommand = new RelayCommand(UserLoginValidations));
        #endregion

        #region Constructor
        public LoginPageViewModel()
        {
            resourceLoader = ResourceLoader.GetForCurrentView();

            LoadingVisibility = Visibility.Collapsed;
        }
        #endregion

        #region Private Methods
        private async void UserLoginValidations()
        {
            try
            {
                if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Pin))
                {
                    ContentDialog emptyFieldDialog = new ContentDialog
                    {
                        Title = resourceLoader.GetString("LoginErrorTitleText"),
                        Content = resourceLoader.GetString("LoginEmptyFieldsError"),
                        CloseButtonText = resourceLoader.GetString("OK")
                    };

                    await emptyFieldDialog.ShowAsync();
                }
                else
                {
                    LoadingVisibilityHandler(true);

                    bool isPinParsable = int.TryParse(Pin, out int enteredPin);

                    if (isPinParsable)
                    {
                        await AuthenticUser();
                    }
                    else
                    {
                        IsLoginSuccessful = false;
                    }

                    if (IsLoginSuccessful)
                    {
                       await CheckForExistingUserLoginDetails();

                        await DataSyncHelper.DownloadBrandImages();

                        if (IsDataDownloadSuccessful)
                        {
                            ((App)Application.Current).IsUserAlreadyLogin = true;
                            ((App)Application.Current).IsSyncSuccessProperty = true;
                            ((App)Application.Current).CartItemCount = 0;
                            ((App)Application.Current).CurrentOrderId = 0;
                            ((App)Application.Current).SelectedCustomerId = string.Empty;

                            LoadingVisibilityHandler(false);

                            NavigateToDashboardPage();
                        }
                        else if (IsDatabaseFileDownloadSuccessful || ((App)Application.Current).LoginUserNameProperty.Equals(UserName))
                        {
                            ((App)Application.Current).IsUserAlreadyLogin = true;
                            ((App)Application.Current).CartItemCount = 0;
                            ((App)Application.Current).CurrentOrderId = 0;
                            ((App)Application.Current).SelectedCustomerId = string.Empty;

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

        private void LoadingVisibilityHandler(bool isLoading)
        {
            LoadingVisibility = isLoading ? Visibility.Visible : Visibility.Collapsed;
        }

        private void SaveUserCredentialsToLocalSettings()
        {
            ((App)Application.Current).LoginUserNameProperty = UserName.Trim();
            ((App)Application.Current).LoginUserPinProperty = Pin.Trim();
            ((App)Application.Current).LoginUserIdProperty = LoginUserDetails.userid.ToString().Trim();
            ((App)Application.Current).LastSyncDateTimeProperty = LoginUserDetails.lastsyncutcdate.Trim();
        }

        private async Task CheckForExistingUserLoginDetails()
        {
            // If the user exists download data using partial sync
            if (((App)Application.Current).LoginUserNameProperty.Equals(UserName))
            {
               await SyncDataAfterSuccessfulLogin(); 
            }
            else
            {
                // Download new database file for the user
                await DownloadDatabaseFileFromServer();

                if (IsDatabaseFileDownloadSuccessful)
                {
                    // Save user credentials into local storage
                    SaveUserCredentialsToLocalSettings();

                    // Sync data after downloading database, to download any changed data for user
                    var isDataSyncSuccessAfterLogin = await DataSyncHelper.SyncDataAfterUserLogin(UserName, Pin, LoginUserDetails.lastsyncutcdate);

                    ((App)Application.Current).IsDataSyncSuccessAfterLogin = isDataSyncSuccessAfterLogin;

                    IsDataDownloadSuccessful = true;
                }
                else
                {
                    ((App)Application.Current).IsDataSyncSuccessAfterLogin = false;

                    IsDataDownloadSuccessful = false;
                }
            }
        }
     
        private static void NavigateToDashboardPage()
        {
            ((App)Application.Current).ActiveShellPage();
        }

        private async Task AuthenticUser()
        {
            LoginUserDetails = await InvokeWebService.UserAuthenticateWebService(UserName, Convert.ToInt32(Pin));

            if (LoginUserDetails != null && string.IsNullOrEmpty(LoginUserDetails.errormsg) && Convert.ToInt32(LoginUserDetails.responsestatus) == 200)
            {
                IsLoginSuccessful = true;
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
                    IsDataDownloadSuccessful = await DataSyncHelper.SyncDataAfterUserLogin(UserName, Pin, LoginUserDetails.lastsyncutcdate);

                    ((App)Application.Current).IsDataSyncSuccessAfterLogin = IsDataDownloadSuccessful;
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
            if (LoginUserDetails != null && string.IsNullOrEmpty(LoginUserDetails.errormsg) && Convert.ToInt32(LoginUserDetails.responsestatus) == 200)
            {
                IsDatabaseFileDownloadSuccessful = await InvokeWebService.DownloadDatabaseFileFromServer(LoginUserDetails.dbfilename, LoginUserDetails.lastsyncutcdate);
            }
            else
            {
                IsDatabaseFileDownloadSuccessful = false;
            }
        }

        #endregion
    }
}
