using System;

using DRLMobile.ViewModels;

using Windows.UI.Xaml.Controls;

namespace DRLMobile.Views
{
    public sealed partial class MainPage : Page
    {
        public MainViewModel ViewModel { get; } = new MainViewModel();

        public MainPage()
        {
            InitializeComponent();
        }
    }
}
