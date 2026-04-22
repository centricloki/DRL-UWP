using System;
using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace DRLMobile.CustomControls
{
    public sealed partial class FooterControl : UserControl
    {

        #region Properties

        public string Version { get => GetAppVersion(); }


        #endregion 


        #region dependency properties

        public string SyncDateTime
        {
            get { return (string)GetValue(SyncDateTimeProperty); }
            set { SetValue(SyncDateTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SyncDateTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SyncDateTimeProperty =
            DependencyProperty.Register(nameof(SyncDateTime), typeof(string), typeof(FooterControl), new PropertyMetadata(defaultValue: string.Empty, propertyChangedCallback: OnSyncDateChanged));

        public string LoggedInUsername
        {
            get { return (string)GetValue(LoggedInUsernameProperty); }
            set { SetValue(LoggedInUsernameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LoggedInUsername.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LoggedInUsernameProperty =
            DependencyProperty.Register(nameof(LoggedInUsername), typeof(string), typeof(FooterControl), new PropertyMetadata(defaultValue: string.Empty, propertyChangedCallback: OnLoggedInUsernameChanged));


        public Visibility IsSyncVisible
        {
            get { return (Visibility)GetValue(IsSyncVisibleProperty); }
            set { SetValue(IsSyncVisibleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TitleText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsSyncVisibleProperty =
            DependencyProperty.Register(name: nameof(IsSyncVisible), propertyType: typeof(Visibility),
               ownerType: typeof(FooterControl),
                typeMetadata: new PropertyMetadata(defaultValue: Visibility.Visible, propertyChangedCallback: OnSyncVisibilityChanged));


        public Visibility IsLoggedInVisible
        {
            get { return (Visibility)GetValue(IsLoggedInVisibleProperty); }
            set { SetValue(IsLoggedInVisibleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TitleText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsLoggedInVisibleProperty =
            DependencyProperty.Register(name: nameof(IsLoggedInVisible), propertyType: typeof(Visibility),
               ownerType: typeof(FooterControl),
                typeMetadata: new PropertyMetadata(defaultValue: Visibility.Visible, propertyChangedCallback: OnLoggedInVisibilityChanged));





        #endregion


        #region constructor
        public FooterControl()
        {
            this.InitializeComponent();
            DataContext = this;

           // SyncDateTime = DateTime.Now.ToString("g");
        }
        #endregion




        #region private method

        private string GetAppVersion()
        {
            Package package = Package.Current;
            PackageId packageId = package.Id;
            PackageVersion version = packageId.Version;

            return string.Format("V {0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);
        }

        private static void OnSyncDateChanged(DependencyObject control, DependencyPropertyChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(e.NewValue.ToString()))
                (control as FooterControl).SyncDateTimeTextBlock.Text = (string)e.NewValue;

        }
        private static void OnSyncVisibilityChanged(DependencyObject control, DependencyPropertyChangedEventArgs e)
        {
            (control as FooterControl).SyncStackPanel.Visibility = (Visibility)e.NewValue;
        }
        private static void OnLoggedInVisibilityChanged(DependencyObject control, DependencyPropertyChangedEventArgs e)
        {
            (control as FooterControl).LoggedInUserName.Visibility = (Visibility)e.NewValue;
        }
        private static void OnLoggedInUsernameChanged(DependencyObject control, DependencyPropertyChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(e.NewValue.ToString()))
                (control as FooterControl).LoggedInUserName.Text = (string)e.NewValue;

        }
        #endregion
    }
}
