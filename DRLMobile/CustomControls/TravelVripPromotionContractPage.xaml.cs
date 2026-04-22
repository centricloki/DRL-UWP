using DRLMobile.Core.Enums;
using DRLMobile.ViewModels;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;


namespace DRLMobile.CustomControls
{

    public sealed partial class TravelVripPromotionContractPage : UserControl
    {
        #region Properties
        private TravelVripPromotionContractPageViewModel ViewModel = new TravelVripPromotionContractPageViewModel();
        #endregion

        #region Constructor
        public TravelVripPromotionContractPage()
        {
            this.InitializeComponent();
            segmentControl.SelectedSegment = Segment.Left;
            DataContext = ViewModel;


        }
        #endregion

        #region Dependency Properties
        public ICommand CloseCommad
        {
            get { return (ICommand)GetValue(CloseCommadProperty); }
            set { SetValue(CloseCommadProperty, value); }
        }

        public static readonly DependencyProperty CloseCommadProperty =
            DependencyProperty.Register(nameof(CloseCommad), typeof(ICommand), typeof(TravelVripPromotionContractPage), new PropertyMetadata(null));

        public string CustomerNameText
        {
            get { return (string)GetValue(CustomerNameProperty); }
            set { SetValue(CustomerNameProperty, value); }
        }

        public static readonly DependencyProperty CustomerNameProperty =
            DependencyProperty.Register(nameof(CustomerNameText), typeof(string), typeof(TravelVripPromotionContractPage), new PropertyMetadata(defaultValue: string.Empty, propertyChangedCallback: OnCustomerNameChanged));

        public bool ResetControl
        {
            get { return (bool)GetValue(ResetControlProperty); }
            set { SetValue(ResetControlProperty, value); }
        }
        public static readonly DependencyProperty ResetControlProperty =
            DependencyProperty.Register(name: nameof(ResetControl), propertyType: typeof(bool),
               ownerType: typeof(TravelVripPromotionContractPage),
                typeMetadata: new PropertyMetadata(defaultValue: false, propertyChangedCallback: ResetControlHandler));


        #endregion
        #region Private Methods
        private void CrossIcon_Tapped(object sender, TappedRoutedEventArgs e)
        {
            CloseCommad?.Execute(null);
        }
        private static void OnCustomerNameChanged(DependencyObject control, DependencyPropertyChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(e.NewValue.ToString()))
                (control as TravelVripPromotionContractPage).TitleTextBlock.Text = (string)e.NewValue;

        }
        private static void ResetControlHandler(DependencyObject control, DependencyPropertyChangedEventArgs e)
        {
            TravelVripPromotionContractPage cdc = control as TravelVripPromotionContractPage;
            if (cdc.ResetControl)
            {
                cdc.ResetData();
            }
        }
        private void ResetData()
        {
            segmentControl.SelectedSegment = Segment.Left;
            ViewModel?.LoadTravelDataCommand?.Execute(null);
        }
        #endregion

        private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            ViewModel?.HeaderSearchTextChangeCommand?.Execute(sender.Text);
        }

        private void AutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            ViewModel?.HeaderSearchSuggestionChoosenCommand?.Execute(args.SelectedItem);

        }
    }
}
