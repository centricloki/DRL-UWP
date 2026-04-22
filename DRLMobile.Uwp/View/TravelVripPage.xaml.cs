using DRLMobile.Uwp.ViewModel;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DRLMobile.Uwp.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TravelVripPage : Page
    {
        private TravelVripPageViewModel TravelPageViewModel = new TravelVripPageViewModel();

        public TravelVripPage()
        {
            this.InitializeComponent();
            DataContext = TravelPageViewModel;
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            TravelPageViewModel?.OnNavigatedTo.Execute(null);
        }

        private void TravelDataGridcontrol_EndSorting(object sender, EventArgs e)
        {
            if (TravelDataGridcontrol != null)
            {
                TravelDataGridcontrol.SelectedItem = null;
            }
        }

        private void VripDataGridcontrol_EndSorting(object sender, EventArgs e)
        {
            if (VripDataGridcontrol != null)
            {
                VripDataGridcontrol.SelectedItem = null;
            }
        }
    }
}
