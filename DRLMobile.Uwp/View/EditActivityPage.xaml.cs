using DRLMobile.Uwp.ViewModel;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DRLMobile.Uwp.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EditActivityPage : Page
    {
        public EditActivityPageViewModel ViewModel = new EditActivityPageViewModel();
        private object navigationEventParameter;
        public EditActivityPage()
        {
            this.InitializeComponent();
            DataContext = ViewModel;
            this.Loaded += EditActivityPage_Loaded;
            this.Unloaded += EditActivityPage_Unloaded;
        }

        private void EditActivityPage_Unloaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ViewModel = null;
        }

        private void EditActivityPage_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ViewModel?.OnNavigatedToCommand.Execute(navigationEventParameter);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            navigationEventParameter = e.Parameter;
        }

        private void OpenHoursFlyout(object sender, TappedRoutedEventArgs e)
        {
            if (ViewModel.IsHoursEditable)
                HoursFlyout?.ShowAt(HoursGrid);
        }

        private void HoursSelected(object sender, SelectionChangedEventArgs e)
        {
            HoursFlyout.Hide();
            var seletedHours = (sender as ListView).SelectedItem;
            ViewModel?.HoursSelectionCommand.Execute(seletedHours);
        }

        private void OpenActivityTypeFlyout(object sender, TappedRoutedEventArgs e)
        {
            if (ViewModel.IsAccountTypeEditable)
            {
                ActivityTypeFlyout?.ShowAt(ActivityTypeGrid);
            }
        }

        private void ActivityTypeSelected(object sender, TappedRoutedEventArgs e)
        {
            ActivityTypeFlyout.Hide();
            ViewModel?.ActivityTypeCommand.Execute((sender as Grid).DataContext);
        }

        private void OpenAccountSelectionFlyout(object sender, TappedRoutedEventArgs e)
        {
            if (ViewModel.IsAccountNameEditable)
            {
                CustomerSelectionFlyout?.ShowAt(AccountNameGrid);
            }
        }

        private void CustomerSelected(object sender, TappedRoutedEventArgs e)
        {
            CustomerSelectionFlyout?.Hide();
            ViewModel?.AccountNameCommand.Execute((sender as Grid)?.DataContext);
        }

        private void OpenAttachmentTypeFlyout(object sender, TappedRoutedEventArgs e)
        {
            AttachmentTypeFlyout.ShowAt(AttachmentIcon);
        }

        private void DeleteAddedAttachment(object sender, TappedRoutedEventArgs e)
        {
            if (sender is Border)
            {
                AttachmentView attachmentView = ((sender as Border).DataContext as AttachmentView);
                ViewModel?.RemoveAttachementCommand?.Execute(attachmentView.Path);
            }
        }
    }
}
