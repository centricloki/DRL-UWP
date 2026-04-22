using System;
using System.Linq;

using DRLMobile.Core.Models.UIModels;
using DRLMobile.Uwp.ViewModel;

using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DRLMobile.Uwp.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DashboardPage : Page
    {
        public DashboardPageViewModel ViewModel { get; } = new DashboardPageViewModel();
        public DashboardPage()
        {
            InitializeComponent();
            DataContext = ViewModel;
            Loaded += DashboardPage_Loaded;
        }

        private async void DashboardPage_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => await ViewModel.NotifyUserAsync());
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            ViewModel.OnNavigatedToCommand.Execute(false);
        }

    }
}
