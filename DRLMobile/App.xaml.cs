using DRLMobile.Core.Enums;
using DRLMobile.Core.Helpers;
using DRLMobile.Core.Interface;
using DRLMobile.Core.Services;
using DRLMobile.ExceptionHandler;
using DRLMobile.Helpers;
using DRLMobile.Services;
using DRLMobile.Views;
using System;
using System.IO;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Background;
using Windows.Networking.PushNotifications;
using Windows.Storage;
using Windows.UI.Xaml;

namespace DRLMobile
{
    public sealed partial class App : Application
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

        public bool? IsDataSyncSuccessAfterLogin
        {
            get { return (bool?)_localSettings.Values["SyncSuccessAfterLogin"] ?? false; }
            set { _localSettings.Values["SyncSuccessAfterLogin"] = value; }
        }

        public string LoginUserNameProperty
        {
            get { return _localSettings.Values["UserName"]?.ToString() ?? string.Empty; }
            set { _localSettings.Values["UserName"] = value; }
        }

        public string LoginUserPinProperty
        {
            get { return _localSettings.Values["Pin"].ToString() ?? string.Empty; }
            set { _localSettings.Values["Pin"] = value; }
        }

        public string LoginUserIdProperty
        {
            get { return _localSettings.Values["UserId"].ToString() ?? string.Empty; }
            set { _localSettings.Values["UserId"] = value; }
        }

        public string LastSyncDateTimeProperty
        {
            get { return _localSettings?.Values["LastSyncDateTime"]?.ToString() ?? string.Empty; }
            set { _localSettings.Values["LastSyncDateTime"] = value; }
        }

        public string SelectedCustomerId
        {
            get { return _localSettings.Values[Constants.Constants.SELECTED_CUSTOMER_ID]?.ToString() ?? string.Empty; }
            set { _localSettings.Values[Constants.Constants.SELECTED_CUSTOMER_ID] = value; }
        }

        public bool? IsCreditRequestOrder
        {
            get { return (bool?)_localSettings.Values["IsCreditRequest"] ?? false; }
            set { _localSettings.Values["IsCreditRequest"] = value; }
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

        private Lazy<ActivationService> _activationService;

        private Lazy<UIElement> _firstNavigationPage;

        private ActivationService ActivationService
        {
            get { return _activationService.Value; }
        }
        //public CartDataAddedFrom CartDataFromScreen
        //{
        //    get { return (CartDataAddedFrom)_localSettings.Values["CartDataFrom"] ?? CartDataAddedFrom.Product; }
        //    set { _localSettings.Values["CartDataFrom"] = value; }
        //}
        public int CartDataFromScreen
        {
            get { return (int?)_localSettings.Values["CartDataFromScreen"] ?? 0; }
            set { _localSettings.Values["CartDataFromScreen"] = value; }
        }

        public App()
        {
            InitializeComponent();

            EnteredBackground += App_EnteredBackground;
            Resuming += App_Resuming;
            UnhandledException += OnAppUnhandledException;

            // Deferred execution until used. Check https://docs.microsoft.com/dotnet/api/system.lazy-1 for further info on Lazy<T> class.
            _activationService = new Lazy<ActivationService>(CreateActivationService);
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            try
            {
                if (!args.PrelaunchActivated)
                {

                    if ((bool)IsUserAlreadyLogin)
                    {
                        _firstNavigationPage = new Lazy<UIElement>(CreateShell);
                    }

                    ApplicationConstants.APPLICATION_VERSION = HelperMethods.GetAppVersion();

                    ApplicationConstants.APP_PATH = ApplicationData.Current.LocalFolder.Path;

                    ApplicationConstants.DATABASE_PATH = Path.Combine(ApplicationData.Current.LocalFolder.Path, ApplicationConstants.DATABASE_NAME);

                    await ActivationService.ActivateAsync(args);
                }

                await RegisterBackgroundTask.Register(Constants.Constants.BackgroundNotificationEntryPoint, Constants.Constants.BackgroundNotificationEntryPoint, new PushNotificationTrigger(), null, true);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(App), nameof(OnLaunched), ex.StackTrace);
            }
        }

        protected override async void OnActivated(IActivatedEventArgs args)
        {
            await ActivationService.ActivateAsync(args);
        }

        private void OnAppUnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "OnAppUnhandledException", e.Exception);
        }

        private ActivationService CreateActivationService()
        {
            return new ActivationService(this, typeof(Views.DashboardPage), _firstNavigationPage);
        }

        private UIElement CreateShell()
        {
            return new ShellPage();
        }

        private async void App_EnteredBackground(object sender, EnteredBackgroundEventArgs e)
        {
            var deferral = e.GetDeferral();
            await Singleton<SuspendAndResumeService>.Instance.SaveStateAsync();
            deferral.Complete();
        }

        private void App_Resuming(object sender, object e)
        {
            Singleton<SuspendAndResumeService>.Instance.ResumeApp();
        }

        public void ActiveShellPage()
        {
            ///await ActivationService.ActivateAsync();
            ActivationService.NavigateShell(new Lazy<UIElement>(CreateShell));
        }
    }
}
