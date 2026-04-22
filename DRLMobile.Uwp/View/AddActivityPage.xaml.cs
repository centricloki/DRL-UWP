using DRLMobile.Uwp.ViewModel;

using Microsoft.Toolkit.Mvvm.Input;

using System;
using System.Diagnostics;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DRLMobile.Uwp.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddActivityPage : Page
    {
        public AddActivityPageViewModel ViewModel = new AddActivityPageViewModel();
        private object navigationEventParameter;
        private NavigationMode navigationMode;
        public AddActivityPage()
        {
            this.InitializeComponent();
            this.DataContext = ViewModel;
            this.Loaded += AddActivityPage_Loaded;
            this.Unloaded += AddActivityPage_Unloaded;
        }

        private void AddActivityPage_Unloaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ViewModel.SelectedActivityType = string.Empty;
            ViewModel.SelectedCustomerName = string.Empty;
            ViewModel.SelectedCustomerNo = string.Empty;
            ViewModel.ActivityTypeList.Clear();
        }

        private void AddActivityPage_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (navigationMode == NavigationMode.New)
            {
                ViewModel.SelectedHours = string.Empty;
                ClearCacheActivityData();
            }
            ViewModel.OnNavigatedToCommand.Execute(navigationEventParameter);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            navigationMode = e.NavigationMode;
            navigationEventParameter = e.Parameter;
        }

        private void OpenActivityFlyout(object sender, TappedRoutedEventArgs e)
        {
            ActivityTypeFlyout.ShowAt(ActivityTypeGrid);
        }

        private void CustomerSelectionGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (!ViewModel.isCustomerAddActivity)
            {
                CustomerSelectionFlyout.ShowAt(CustomerSelectionGrid);
            }
        }

        private void CustomerSelected(object sender, TappedRoutedEventArgs e)
        {
            var context = (sender as Grid).DataContext;
            ViewModel.CustomerSelectionCommand.Execute(context);
            CustomerSelectionFlyout.Hide();
        }

        private void ActivitySelected(object sender, TappedRoutedEventArgs e)
        {
            ViewModel.ActivitySelectionCommand.Execute((sender as Grid).DataContext);
            ActivityTypeFlyout.Hide();
        }

        private void DeleteAddedAttachment(object sender, TappedRoutedEventArgs e)
        {
            if (sender is Border)
            {
                ViewModel?.RemoveAttachementCommand?.Execute((sender as Border).DataContext);
            }
        }

        private void OpenAttachmentTypeFlyout(object sender, TappedRoutedEventArgs e)
        {
            AttachmentTypeFlyout.ShowAt(AttachmentIcon);
        }

        private void CancelActivityButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ClearCacheActivityData();

            ViewModel.SelectedHours = string.Empty;

            ViewModel.CancelCommand.Execute(e);
        }

        private async void AddActivityButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            //ViewModel.SaveCommand.Execute(e);
            await ((IAsyncRelayCommand)ViewModel.SaveCommand).ExecuteAsync(e);
            if (!ViewModel.IsInValidConsumerActivationEngagement)
            {
                ClearCacheActivityData();
            }
        }

        private void ClearCacheActivityData()
        {
            hoursCombobox.Text = string.Empty;
            selectedAccNoTextbox.Text = string.Empty;
            customerNameTextbox.Text = string.Empty;
            activityNameTextbox.Text = string.Empty;
            notesTextbox.Text = string.Empty;
            ViewModel.SelectedCallDate = DateTime.Now;
        }
    }
}
