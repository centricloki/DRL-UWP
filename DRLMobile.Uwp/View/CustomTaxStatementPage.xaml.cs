using DRLMobile.Core.Models.UIModels;
using DRLMobile.Uwp.ViewModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DRLMobile.Uwp.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CustomTaxStatementPage : Page
    {
        private CustomTaxStatementPageViewModel CustomTaxStatementViewModel = new CustomTaxStatementPageViewModel();

        public CustomTaxStatementPage()
        {
            this.InitializeComponent();
            DataContext = CustomTaxStatementViewModel;
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {   
            base.OnNavigatedTo(e);
            CustomTaxStatementViewModel?.PageLoadedCommand.Execute(null);
        }

        private void EditIcon_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var senderName = (FontIcon)sender;
            var dataCxtx = senderName.DataContext;
            var dataSource = (UserTaxStatementUiModel)dataCxtx;
            CustomTaxStatementViewModel?.EditIconClickedCommand.Execute(dataSource);
        }

        private void DeleteIcon_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var senderName = (FontIcon)sender;
            var dataCxtx = senderName.DataContext;
            var dataSource = (UserTaxStatementUiModel)dataCxtx;
            CustomTaxStatementViewModel?.DeleteClickedCommand.Execute(dataSource);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            CustomTaxStatementViewModel?.SaveButtonCommand.Execute(null);
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            CustomTaxStatementViewModel?.ClearButtonCommand.Execute(null);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            CustomTaxStatementViewModel?.CancelButtonCommand.Execute(null);
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            CustomTaxStatementViewModel?.UpdateButtonCommand.Execute(null);
        }
    }
}
