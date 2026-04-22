using System;

using DRLMobile.ViewModels;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace DRLMobile.Views
{
    // TODO WTS: Change the icons and titles for all NavigationViewItems in ShellPage.xaml.
    public sealed partial class ShellPage : Page
    {
        public ShellViewModel ViewModel { get; } = new ShellViewModel();

        public ShellPage()
        {
            InitializeComponent();
            DataContext = ViewModel;
            ViewModel.Initialize(shellFrame, navigationView, KeyboardAccelerators);
        }



        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // coming to page
            base.OnNavigatedTo(e);
            ViewModel?.NavigatedToCommand.Execute(null);
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            // moving away from the page
            base.OnNavigatingFrom(e);
            ViewModel?.NavigatingFromCommand.Execute(null);
        }

    }
}
