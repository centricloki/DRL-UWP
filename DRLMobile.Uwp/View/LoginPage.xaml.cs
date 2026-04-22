using DRLMobile.Uwp.ViewModel;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DRLMobile.Uwp.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginPage : Page
    {
        public LoginPageViewModel ViewModel { get; } = new LoginPageViewModel();
        public LoginPage()
        {
            this.InitializeComponent();
            DataContext = ViewModel;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            (Application.Current as App).IsApplicationUpdateAvailable=false;

            if (e.Parameter != null && (bool)e.Parameter)
            {
                ViewModel.OnNavigatedToCommand.Execute(true);
            }
            else
            {
                if ((Application.Current as App).IsRememberMe)
                {
                    RememberMeCheckBox.IsChecked = true;
                    UserNameTextBox.Text = (Application.Current as App).UserNameText;
                    PinBox.Password = (Application.Current as App).PinText;
                }
                else
                {
                    RememberMeCheckBox.IsChecked = false;
                    UserNameTextBox.Text = string.Empty;
                    PinBox.Password = string.Empty;
                }
            }

        }

        private void RememberMeCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            (Application.Current as App).IsRememberMe = true;

            if(!string.IsNullOrWhiteSpace(UserNameTextBox.Text))
            {
                (Application.Current as App).UserNameText = UserNameTextBox.Text;
            }
            if(!string.IsNullOrWhiteSpace(PinBox.Password))
            {
                (Application.Current as App).PinText = PinBox.Password;
            }

        }

        private void RememberMeCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            (Application.Current as App).IsRememberMe = false;
            (Application.Current as App).UserNameText = string.Empty;
            (Application.Current as App).PinText = string.Empty;

        }

        private void UserNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if ((Application.Current as App).IsRememberMe)
            {
                (Application.Current as App).UserNameText = (sender as TextBox).Text;
            }
        }

        private void PinBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if ((Application.Current as App).IsRememberMe)
            {
                (Application.Current as App).PinText = (sender as PasswordBox).Password;
            }
        }
    }
}
