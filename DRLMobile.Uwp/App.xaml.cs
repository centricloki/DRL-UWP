using DRLMobile.Core.Helpers;
using DRLMobile.Core.Interface;
using DRLMobile.Core.Services;
using DRLMobile.ExceptionHandler;
using DRLMobile.Uwp.Services;
using DRLMobile.Uwp.View;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Background;
using Windows.Networking.PushNotifications;
using Windows.Storage;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace DRLMobile.Uwp
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        private ApplicationDataContainer _localSettings = ApplicationData.Current.LocalSettings;
        private IQueryService _queryService = new QueryService();
        private IEmailService _emailService = Services.EmailService.Instance;
        private readonly ILocalFileService _localFileService = new LocalFileService();

        public PushNotificationChannel Channel { get; set; }

        public bool? IsApplicationUpdateAvailable
        {
            get { return (bool?)_localSettings.Values[Constants.Constants.IS_APPLICATION_UPDATE_AVAILABLE] ?? false; }
            set { _localSettings.Values[Constants.Constants.IS_APPLICATION_UPDATE_AVAILABLE] = value; }
        }

        public bool? ShowUserPopupToUpdate
        {
            get { return (bool?)_localSettings.Values[Constants.Constants.SHOW_USER_UPDATE_POPUP] ?? false; }
            set { _localSettings.Values[Constants.Constants.SHOW_USER_UPDATE_POPUP] = value; }
        }

        public string NotificationContent
        {
            get { return (string)_localSettings.Values[Constants.Constants.NOTIFICATION_CONTENT] ?? string.Empty; }
            set { _localSettings.Values[Constants.Constants.NOTIFICATION_CONTENT] = value; }
        }

        public bool? IsSyncSuccessProperty
        {
            get { return (bool?)_localSettings.Values["SyncSuccess"] ?? false; }
            set { _localSettings.Values["SyncSuccess"] = value; }
        }

        public string LoginUserNameProperty
        {
            get { return _localSettings.Values["UserName"]?.ToString() ?? string.Empty; }
            set { _localSettings.Values["UserName"] = value; }
        }

        public string LoginUserPinProperty
        {
            get { return _localSettings.Values["Pin"]?.ToString() ?? string.Empty; }
            set { _localSettings.Values["Pin"] = value; }
        }

        public string LoginUserIdProperty
        {
            get { return _localSettings.Values["UserId"]?.ToString() ?? string.Empty; }
            set { _localSettings.Values["UserId"] = value; }
        }

        public string LastSyncDateTimeProperty
        {
            get { return _localSettings?.Values["LastSyncDateTime"]?.ToString() ?? string.Empty; }
            set { _localSettings.Values["LastSyncDateTime"] = value; }
        }

        public string PreviousSyncDateTimeProperty
        {
            get { return _localSettings?.Values["PreviousSyncDateTime"]?.ToString() ?? string.Empty; }
            set { _localSettings.Values["PreviousSyncDateTime"] = value; }
        }

        public string UserDbFileNameProperty
        {
            get { return _localSettings?.Values["UserDbFileName"]?.ToString() ?? string.Empty; }
            set { _localSettings.Values["UserDbFileName"] = value; }
        }

        public string SelectedCustomerId
        {
            get { return _localSettings.Values[Constants.Constants.SELECTED_CUSTOMER_ID]?.ToString() ?? string.Empty; }
            set { _localSettings.Values[Constants.Constants.SELECTED_CUSTOMER_ID] = value; }
        }
        public string PreviousSelectedCustomerId
        {
            get { return _localSettings.Values[Constants.Constants.PREVIOUS_SELECTED_CUSTOMER_ID]?.ToString() ?? string.Empty; }
            set { _localSettings.Values[Constants.Constants.PREVIOUS_SELECTED_CUSTOMER_ID] = value; }
        }

        public string UserNameText
        {
            get { return _localSettings.Values[Constants.Constants.USER_NAME]?.ToString() ?? string.Empty; }
            set { _localSettings.Values[Constants.Constants.USER_NAME] = value; }
        }

        public string PinText
        {
            get { return _localSettings.Values[Constants.Constants.PASSWORD]?.ToString() ?? string.Empty; }
            set { _localSettings.Values[Constants.Constants.PASSWORD] = value; }
        }

        public bool IsRememberMe
        {
            get { return _localSettings.Values.ContainsKey(Constants.Constants.IS_REMEMBER_ME) ? (bool)_localSettings.Values[Constants.Constants.IS_REMEMBER_ME] : false; }
            set { _localSettings.Values[Constants.Constants.IS_REMEMBER_ME] = value; }
        }

        public bool? IsDistributionOptionClicked
        {
            get { return (bool?)_localSettings.Values["IsDistributionOptionClicked"] ?? false; }
            set { _localSettings.Values["IsDistributionOptionClicked"] = value; }
        }

        public bool? IsCreditRequestOrder
        {
            get { return (bool?)_localSettings.Values["IsCreditRequest"] ?? false; }
            set { _localSettings.Values["IsCreditRequest"] = value; }
        }
        public bool? IsCarStockOrder
        {
            get { return (bool?)_localSettings.Values["IsCarStockOrder"]; }
            set { _localSettings.Values["IsCarStockOrder"] = value; }
        }
        public bool? IsOrderTypeChanged
        {
            get { return (bool?)_localSettings.Values["IsOrderTypeChange"] ?? false; }
            set { _localSettings.Values["IsOrderTypeChange"] = value; }
        }

        public bool? IsUserAlreadyLogin
        {
            get { return (bool?)_localSettings.Values["IsUserLogin"] ?? false; }
            set { _localSettings.Values["IsUserLogin"] = value; }
        }

        public bool? IsCustomerActivity
        {
            get { return (bool?)_localSettings.Values["IsCustomerActivity"] ?? false; }
            set { _localSettings.Values["IsCustomerActivity"] = value; }
        }

        public int CartItemCount
        {
            get { return (int?)_localSettings.Values[Constants.Constants.BADGE_COUNT] ?? 0; }
            set { _localSettings.Values[Constants.Constants.BADGE_COUNT] = value; }
        }

        public int CurrentOrderId
        {
            get { return (int?)_localSettings.Values[Constants.Constants.CURRENT_ORDER_ID] ?? 0; }
            set { _localSettings.Values[Constants.Constants.CURRENT_ORDER_ID] = value; }
        }

        public string CurrentDeviceOrderId
        {
            get { return _localSettings.Values["CurrentDeviceOrderId"]?.ToString() ?? string.Empty; }
            set { _localSettings.Values["CurrentDeviceOrderId"] = value; }
        }

        public int LoggedInUserRoleId
        {
            get { return (int?)_localSettings.Values["LoggedInUserRoleId"] ?? 0; }
            set { _localSettings.Values["LoggedInUserRoleId"] = value; }
        }

        public int LoggedInUserRegionId
        {
            get { return (int?)_localSettings.Values["LoggedInUserRegionId"] ?? 0; }
            set { _localSettings.Values["LoggedInUserRegionId"] = value; }
        }

        public int LoggedInUserZoneId
        {
            get { return (int?)_localSettings.Values["LoggedInUserZoneId"] ?? 0; }
            set { _localSettings.Values["LoggedInUserZoneId"] = value; }
        }

        public string OrderPrintName
        {
            get { return (string)_localSettings.Values["OrderPrintName"] ?? ""; }
            set { _localSettings.Values["OrderPrintName"] = value; }
        }

        public IQueryService QueryService
        {
            get { return _queryService; }
        }

        public IEmailService EmailService
        {
            get { return _emailService; }
        }

        public ILocalFileService LocalFileService
        {
            get { return _localFileService; }
        }

        public int CartDataFromScreen
        {
            get { return (int?)_localSettings.Values["CartDataFromScreen"] ?? 0; }
            set { _localSettings.Values["CartDataFromScreen"] = value; }
        }

        public bool? IsDataSyncSuccessAfterLogin
        {
            get { return (bool?)_localSettings.Values["SyncSuccessAfterLogin"] ?? false; }
            set { _localSettings.Values["SyncSuccessAfterLogin"] = value; }
        }

        public string SqlLiteResetVersion
        {
            get { return _localSettings.Values["SqlLiteResetVersion"]?.ToString() ?? string.Empty; }
            set { _localSettings.Values["SqlLiteResetVersion"] = value; }
        }

        public bool IsUpgradeVersion { get; set; }

        public IEnumerable<string> UpdatedCustomerIds { get; set; }

        public int AreaUserSelectedId
        {
            get { return (int)(_localSettings.Values["AreaUserSelectedId"] ?? 0); }
            set { _localSettings.Values["AreaUserSelectedId"] = value; }
        }

        public string LoggedInUserZoneName
        {
            get
            {
                GetZoneNames(); return (string)_localSettings.Values["LoggedInUserZoneName"];
            }
        }


        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            this.UnhandledException += App_UnhandledException;
            TaskScheduler.UnobservedTaskException += App_UnobservedException;
        }

        private void App_UnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            // Occurs when an exception is not handled on the UI thread.
            // if you want to suppress and handle it manually, 
            // otherwise app shuts down.
            ErrorLogger.WriteToErrorLog(GetType().Name, nameof(App_UnhandledException), e.Exception);
            e.Handled = true;
        }
        private static void App_UnobservedException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            // Occurs when an exception is not handled on a background thread.
            // ie. A task is fired and forgotten Task.Run(() => {...})
            // suppress and handle it manually.
            e.SetObserved();
            ((AggregateException)e.Exception).Handle(ex =>
            {
                ErrorLogger.WriteToErrorLog(ex.GetType().Name, nameof(App_UnobservedException), ex);
                return true;
            });
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected async override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    ApplicationConstants.APP_PATH = ApplicationData.Current.LocalFolder.Path;
                    ApplicationConstants.APPLICATION_VERSION = HelperMethods.GetAppVersion();
                    ApplicationConstants.DATABASE_PATH = Path.Combine(ApplicationData.Current.LocalFolder.Path, ApplicationConstants.DATABASE_NAME);
                    ErrorLogger.ApplicationPath = ApplicationConstants.APP_PATH;

                    // for national users
                    if ((new int[] { 5, 6, 17 }).Any(x => x == ((App)Application.Current).LoggedInUserRoleId))
                    {
                        //Update last sync on every sqllite file download
                        LastSyncDateTimeProperty = await QueryService.GetConfigurationValueAsync(Constants.Constants.SQLLiteLastSyncDate);
                        if (!Directory.Exists(ApplicationConstants.APP_PATH + @"\BrandImages\"))
                        {
                            await DataSyncHelper.DownloadBrandImages();
                        }
                    }

                    IsApplicationUpdateAvailable = false;

                    if ((bool)IsUserAlreadyLogin)
                        rootFrame.Navigate(typeof(ShellPage), e.Arguments);
                    else
                        rootFrame.Navigate(typeof(LoginPage));

                    await RegisterBackgroundTask.Register(Constants.Constants.BackgroundNotificationEntryPoint, Constants.Constants.BackgroundNotificationEntryPoint, new PushNotificationTrigger(), null, true);

                }
                // Ensure the current window is active
                Window.Current.Activate();
            }
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        private async Task<string> GetZoneName(int zoneId)
        {
            List<Core.Models.UIModels.ZoneMasterUIModel> zones = (await _queryService.GetZoneFromZoneId(zoneId)).ToList();
            if (zones != null) return zones.FirstOrDefault().ZoneName;
            else return "";
        }
        private async void GetZoneNames()
        {
            string zoneName = (string)_localSettings.Values["LoggedInUserZoneName"];
            if (string.IsNullOrWhiteSpace(zoneName))
            {
                zoneName = await GetZoneName(LoggedInUserZoneId);
                _localSettings.Values["LoggedInUserZoneName"] = zoneName;
            }
        }

    }
}
