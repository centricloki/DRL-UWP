using DRLMobile.Uwp.ViewModel;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DRLMobile.Uwp.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ShellPage : Page
    {
        public ShellPageViewModel ViewModel { get; } = new ShellPageViewModel();

        public ShellPage()
        {
            this.InitializeComponent();
            DataContext = this;
            this.Loaded += ShellPage_Loaded;
        }

        private void ShellPage_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ViewModel.NavigatedToCommand.Execute(null);
        }

        //protected override void OnNavigatedTo(NavigationEventArgs e)
        //{
        //    ViewModel.NavigatedToCommand.Execute(null);
        //}
    }
}