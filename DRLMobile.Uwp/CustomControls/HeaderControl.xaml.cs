using DRLMobile.Core.Models.UIModels;

using Microsoft.Toolkit.Mvvm.Input;

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace DRLMobile.Uwp.CustomControls
{
    public sealed partial class HeaderControl : UserControl
    {
        #region Dependency properties

        public ICommand rbActivityDataChangedCommand
        {
            get { return (ICommand)GetValue(rbActivityDataChangedCommandProperty); }
            set { SetValue(rbActivityDataChangedCommandProperty, value); }
        }

        public static readonly DependencyProperty rbActivityDataChangedCommandProperty =
            DependencyProperty.Register(nameof(rbActivityDataChangedCommand), typeof(ICommand),
                typeof(HeaderControl), new PropertyMetadata(defaultValue: null, OnPropertyChanged));

        public Visibility IsRbDataVisible
        {
            get { return (Visibility)GetValue(IsRbDataVisibleProperty); }
            set { SetValue(IsRbDataVisibleProperty, value); }
        }

        public static readonly DependencyProperty IsRbDataVisibleProperty =
            DependencyProperty.Register(name: nameof(IsRbDataVisible), propertyType: typeof(Visibility),
               ownerType: typeof(HeaderControl),
                typeMetadata: new PropertyMetadata(defaultValue: Visibility.Collapsed, propertyChangedCallback: OnRbDataVisibilityChanged));
        public ICommand SuggestionChoosenCommand
        {
            get { return (ICommand)GetValue(SuggestionChoosenCommandProperty); }
            set { SetValue(SuggestionChoosenCommandProperty, value); }
        }

        public static readonly DependencyProperty SuggestionChoosenCommandProperty =
            DependencyProperty.Register(nameof(SuggestionChoosenCommand), typeof(ICommand),
                typeof(HeaderControl), new PropertyMetadata(defaultValue: null, OnPropertyChanged));

        public ICommand TextValueChangedCommand
        {
            get { return (ICommand)GetValue(TextValueChangedCommandProperty); }
            set { SetValue(TextValueChangedCommandProperty, value); }
        }
        public ICommand ComboBoxValueChangedCommand
        {
            get { return (ICommand)GetValue(ComboBoxValueChangedCommandProperty); }
            set { SetValue(ComboBoxValueChangedCommandProperty, value); }
        }

        public static readonly DependencyProperty TextValueChangedCommandProperty =
            DependencyProperty.Register(nameof(TextValueChangedCommand), typeof(ICommand),
                typeof(HeaderControl), new PropertyMetadata(defaultValue: null, OnPropertyChanged));

        public static readonly DependencyProperty ComboBoxValueChangedCommandProperty =
           DependencyProperty.Register(nameof(ComboBoxValueChangedCommand), typeof(ICommand),
               typeof(HeaderControl), new PropertyMetadata(defaultValue: null, OnPropertyChanged));
        public ICommand TravelComboBoxValueChangedCommand
        {
            get { return (ICommand)GetValue(TravelComboBoxValueChangedCommandProperty); }
            set { SetValue(TravelComboBoxValueChangedCommandProperty, value); }
        }

        public static readonly DependencyProperty TravelComboBoxValueChangedCommandProperty =
           DependencyProperty.Register(nameof(TravelComboBoxValueChangedCommand), typeof(ICommand),
               typeof(HeaderControl), new PropertyMetadata(defaultValue: null, OnPropertyChanged));
        public string DisplayMemberPathValue
        {
            get { return (string)GetValue(DisplayMemberPathValueProperty); }
            set { SetValue(DisplayMemberPathValueProperty, value); }
        }

        public static readonly DependencyProperty DisplayMemberPathValueProperty =
            DependencyProperty.Register(nameof(DisplayMemberPathValue), typeof(string), typeof(HeaderControl), new PropertyMetadata(defaultValue: null, propertyChangedCallback: OnDisplayPathChanged));


        public object ItemList
        {
            get { return GetValue(ItemListProperty); }
            set { SetValue(ItemListProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemListProperty =
            DependencyProperty.Register(nameof(ItemList), typeof(object), typeof(HeaderControl), new PropertyMetadata(defaultValue: null, propertyChangedCallback: OnItemSourceChanged));

        public object TravelComboboxItemList
        {
            get { return GetValue(TravelComboboxItemListProperty); }
            set { SetValue(TravelComboboxItemListProperty, value); }
        }

        public static readonly DependencyProperty TravelComboboxItemListProperty =
            DependencyProperty.Register(nameof(TravelComboboxItemList), typeof(object), typeof(HeaderControl), new PropertyMetadata(defaultValue: null, propertyChangedCallback: OnTravelComboboxItemSourceChanged));

        public Visibility IsComboBoxVisible
        {
            get { return (Visibility)GetValue(IsComboBoxVisibleProperty); }
            set { SetValue(IsComboBoxVisibleProperty, value); }
        }

        public static readonly DependencyProperty IsComboBoxVisibleProperty =
            DependencyProperty.Register(name: nameof(IsComboBoxVisible), propertyType: typeof(Visibility),
               ownerType: typeof(HeaderControl),
                typeMetadata: new PropertyMetadata(defaultValue: Visibility.Collapsed, propertyChangedCallback: OnComboBoxVisibilityChanged));

        public Visibility IsCustomerPanelVisible
        {
            get { return (Visibility)GetValue(IsCustomerPanelVisibleProperty); }
            set { SetValue(IsCustomerPanelVisibleProperty, value); }
        }

        public static readonly DependencyProperty IsCustomerPanelVisibleProperty =
            DependencyProperty.Register(name: nameof(IsCustomerPanelVisible), propertyType: typeof(Visibility),
               ownerType: typeof(HeaderControl),
                typeMetadata: new PropertyMetadata(defaultValue: Visibility.Collapsed, propertyChangedCallback: OnCustomerPanelVisibilityChanged));

        public string CustomerNameText
        {
            get { return (string)GetValue(CustomerNameProperty); }
            set { SetValue(CustomerNameProperty, value); }
        }

        public static readonly DependencyProperty CustomerNameProperty =
            DependencyProperty.Register(nameof(CustomerNameText), typeof(string), typeof(HeaderControl), new PropertyMetadata(defaultValue: string.Empty, propertyChangedCallback: OnCustomerNameChanged));
        public string CustomerAddressText
        {
            get { return (string)GetValue(CustomerAddressProperty); }
            set { SetValue(CustomerAddressProperty, value); }
        }

        public static readonly DependencyProperty CustomerAddressProperty =
            DependencyProperty.Register(nameof(CustomerAddressText), typeof(string), typeof(HeaderControl), new PropertyMetadata(defaultValue: string.Empty, propertyChangedCallback: OnCustomerAddressChanged));

        public string CustomerSubAddressText
        {
            get { return (string)GetValue(CustomerSubAddressProperty); }
            set { SetValue(CustomerSubAddressProperty, value); }
        }

        public static readonly DependencyProperty CustomerSubAddressProperty =
            DependencyProperty.Register(nameof(CustomerSubAddressText), typeof(string), typeof(HeaderControl), new PropertyMetadata(defaultValue: string.Empty, propertyChangedCallback: OnCustomerSubAddressChanged));
        public Visibility IsCustomerTitlePanelVisible
        {
            get { return (Visibility)GetValue(IsCustomerTitlePanelVisibleProperty); }
            set { SetValue(IsCustomerTitlePanelVisibleProperty, value); }
        }

        public static readonly DependencyProperty IsCustomerTitlePanelVisibleProperty =
            DependencyProperty.Register(name: nameof(IsCustomerTitlePanelVisible), propertyType: typeof(Visibility),
               ownerType: typeof(HeaderControl),
                typeMetadata: new PropertyMetadata(defaultValue: Visibility.Collapsed, propertyChangedCallback: OnCustomerTitlePanelVisibilityChanged));
        public string CustomerTitleNameText
        {
            get { return (string)GetValue(CustomerTitleNameTextProperty); }
            set { SetValue(CustomerTitleNameTextProperty, value); }
        }

        public static readonly DependencyProperty CustomerTitleNameTextProperty =
            DependencyProperty.Register(nameof(CustomerTitleNameText), typeof(string), typeof(HeaderControl), new PropertyMetadata(defaultValue: string.Empty, propertyChangedCallback: OnCustomerTitleNameChanged));
        public string CustomerTitleNumberText
        {
            get { return (string)GetValue(CustomerTitleNumberTextProperty); }
            set { SetValue(CustomerTitleNumberTextProperty, value); }
        }

        public static readonly DependencyProperty CustomerTitleNumberTextProperty =
            DependencyProperty.Register(nameof(CustomerTitleNumberText), typeof(string), typeof(HeaderControl), new PropertyMetadata(defaultValue: string.Empty, propertyChangedCallback: OnCustomerTitleNumberChanged));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(HeaderControl), new PropertyMetadata(defaultValue: string.Empty, propertyChangedCallback: OnPageTitleChanged));

        public Visibility IsPageTitleVisible
        {
            get { return (Visibility)GetValue(IsPageTitleVisibleProperty); }
            set { SetValue(IsPageTitleVisibleProperty, value); }
        }

        public static readonly DependencyProperty IsPageTitleVisibleProperty =
            DependencyProperty.Register(name: nameof(IsPageTitleVisible), propertyType: typeof(Visibility),
               ownerType: typeof(HeaderControl),
                typeMetadata: new PropertyMetadata(defaultValue: Visibility.Visible, propertyChangedCallback: OnPageTitleVisibilityChanged));

        public Visibility IsCustomerPageTitleVisible
        {
            get { return (Visibility)GetValue(IsCustomerPageTitleVisibleProperty); }
            set { SetValue(IsCustomerPageTitleVisibleProperty, value); }
        }

        public static readonly DependencyProperty IsCustomerPageTitleVisibleProperty =
            DependencyProperty.Register(name: nameof(IsCustomerPageTitleVisible), propertyType: typeof(Visibility),
               ownerType: typeof(HeaderControl),
                typeMetadata: new PropertyMetadata(defaultValue: Visibility.Collapsed, propertyChangedCallback: OnCustomerPageTitleVisibilityChanged));


        public Visibility IsAutoSuggestBoxVisible
        {
            get { return (Visibility)GetValue(IsAutoSuggestBoxVisibleProperty); }
            set { SetValue(IsAutoSuggestBoxVisibleProperty, value); }
        }

        public static readonly DependencyProperty IsAutoSuggestBoxVisibleProperty =
            DependencyProperty.Register(name: nameof(IsAutoSuggestBoxVisible), propertyType: typeof(Visibility),
               ownerType: typeof(HeaderControl),
                typeMetadata: new PropertyMetadata(defaultValue: Visibility.Collapsed, propertyChangedCallback: OnAutoSuggestBoxVisibilityChanged));

        public string PlaceHolderText
        {
            get { return (string)GetValue(PlaceHolderTextProperty); }
            set { SetValue(PlaceHolderTextProperty, value); }
        }

        public static readonly DependencyProperty PlaceHolderTextProperty =
            DependencyProperty.Register(nameof(PlaceHolderText), typeof(string), typeof(HeaderControl), new PropertyMetadata(defaultValue: string.Empty, propertyChangedCallback: OnPlaceHolderTextChanged));

        public Visibility IsTravelComboBoxVisible
        {
            get { return (Visibility)GetValue(IsTravelComboBoxVisibleProperty); }
            set { SetValue(IsTravelComboBoxVisibleProperty, value); }
        }

        public string TravelComboBoxPlaceHolderText
        {
            get { return (string)GetValue(TravelComboBoxPlaceHolderTextProperty); }
            set { SetValue(TravelComboBoxPlaceHolderTextProperty, value); }
        }

        public static readonly DependencyProperty TravelComboBoxPlaceHolderTextProperty =
            DependencyProperty.Register(nameof(TravelComboBoxPlaceHolderText), typeof(string), typeof(HeaderControl), new PropertyMetadata(defaultValue: string.Empty, propertyChangedCallback: OnTravelPlaceHolderTextChanged));

        public static readonly DependencyProperty IsTravelComboBoxVisibleProperty =
            DependencyProperty.Register(name: nameof(IsTravelComboBoxVisible), propertyType: typeof(Visibility),
               ownerType: typeof(HeaderControl),
                typeMetadata: new PropertyMetadata(defaultValue: Visibility.Collapsed, propertyChangedCallback: OnTravelComboBoxVisibilityChanged));

        public string BadgeText
        {
            get { return (string)GetValue(BadgeTextProperty); }
            set { SetValue(BadgeTextProperty, value); }
        }

        public static readonly DependencyProperty BadgeTextProperty =
            DependencyProperty.Register(nameof(BadgeText), typeof(string), typeof(HeaderControl), new PropertyMetadata(defaultValue: string.Empty, propertyChangedCallback: OnBadgeTextChanged));

        public Visibility IsLogOutButtonVisible
        {
            get { return (Visibility)GetValue(IsLogOutButtonVisibleProperty); }
            set { SetValue(IsLogOutButtonVisibleProperty, value); }
        }

        public static readonly DependencyProperty IsLogOutButtonVisibleProperty =
            DependencyProperty.Register(name: nameof(IsLogOutButtonVisible), propertyType: typeof(Visibility),
               ownerType: typeof(HeaderControl),
                typeMetadata: new PropertyMetadata(defaultValue: Visibility.Collapsed, propertyChangedCallback: OnLogOutVisibilityChanged));

        public Visibility IsCartButtonVisible
        {
            get { return (Visibility)GetValue(IsCartButtonVisibleProperty); }
            set { SetValue(IsCartButtonVisibleProperty, value); }
        }

        public static readonly DependencyProperty IsCartButtonVisibleProperty =
            DependencyProperty.Register(name: nameof(IsCartButtonVisible), propertyType: typeof(Visibility),
               ownerType: typeof(HeaderControl),
                typeMetadata: new PropertyMetadata(defaultValue: Visibility.Collapsed, propertyChangedCallback: OnCartVisibilityChanged));

        public Visibility IsBadgeVisible
        {
            get { return (Visibility)GetValue(IsBadgeVisibleProperty); }
            set { SetValue(IsBadgeVisibleProperty, value); }
        }

        public static readonly DependencyProperty IsBadgeVisibleProperty =
            DependencyProperty.Register(name: nameof(IsBadgeVisible), propertyType: typeof(Visibility),
               ownerType: typeof(HeaderControl),
                typeMetadata: new PropertyMetadata(defaultValue: Visibility.Collapsed, propertyChangedCallback: OnBadgeVisibilityChanged));


        public Visibility IsAccountOwnerFilterVisible
        {
            get { return (Visibility)GetValue(IsAccountOwnerFilterVisibleProperty); }
            set { SetValue(IsAccountOwnerFilterVisibleProperty, value); }
        }

        public static readonly DependencyProperty IsAccountOwnerFilterVisibleProperty =
            DependencyProperty.Register(name: nameof(IsAccountOwnerFilterVisible), propertyType: typeof(Visibility),
               ownerType: typeof(HeaderControl),
                typeMetadata: new PropertyMetadata(defaultValue: Visibility.Collapsed, propertyChangedCallback: OnRbAccountOwnerFilterChanged));


        public static readonly DependencyProperty ButtonClickProperty = DependencyProperty.Register("ButtonClick",
        typeof(ICommand),
        typeof(HeaderControl),
        new PropertyMetadata(null));
        public ICommand ButtonClick
        {
            get { return (ICommand)GetValue(ButtonClickProperty); }
            set { SetValue(ButtonClickProperty, value); }
        }

        public static readonly DependencyProperty CartButtonClickProperty = DependencyProperty.Register("CartButtonClick",
          typeof(ICommand),
          typeof(HeaderControl),
          new PropertyMetadata(null));
        public ICommand CartButtonClick
        {
            get { return (ICommand)GetValue(CartButtonClickProperty); }
            set { SetValue(CartButtonClickProperty, value); }
        }
        public static readonly DependencyProperty CustomerTitlePanelClickProperty = DependencyProperty.Register("CustomerTitlePanelClick",
         typeof(ICommand),
         typeof(HeaderControl),
         new PropertyMetadata(null));
        public ICommand CustomerTitlePanelClick
        {
            get { return (ICommand)GetValue(CustomerTitlePanelClickProperty); }
            set { SetValue(CustomerTitlePanelClickProperty, value); }
        }
        public static readonly DependencyProperty CustomerPanelClickProperty = DependencyProperty.Register("CustomerPanelClick",
        typeof(ICommand),
        typeof(HeaderControl),
        new PropertyMetadata(null));
        public ICommand CustomerPanelClick
        {
            get { return (ICommand)GetValue(CustomerPanelClickProperty); }
            set { SetValue(CustomerPanelClickProperty, value); }
        }

        public Visibility CustomIconButtonVisibility
        {
            get { return (Visibility)GetValue(CustomIconButtonVisibilityProperty); }
            set { SetValue(CustomIconButtonVisibilityProperty, value); }
        }
        public static readonly DependencyProperty CustomIconButtonVisibilityProperty =
            DependencyProperty.Register(nameof(CustomIconButtonVisibilityProperty), typeof(Visibility), typeof(HeaderControl), new PropertyMetadata(defaultValue: Visibility.Collapsed));



        public string NormalImageSource
        {
            get { return (string)GetValue(NormalImageSourceProperty); }
            set { SetValue(NormalImageSourceProperty, value); }
        }
        public static readonly DependencyProperty NormalImageSourceProperty =
            DependencyProperty.Register(nameof(NormalImageSource), typeof(string), typeof(HeaderControl), new PropertyMetadata(null));




        public string HoverImageSource
        {
            get { return (string)GetValue(HoverImageSourceProperty); }
            set { SetValue(HoverImageSourceProperty, value); }
        }
        public static readonly DependencyProperty HoverImageSourceProperty =
            DependencyProperty.Register("HoverImageSource", typeof(string), typeof(HeaderControl), new PropertyMetadata(null));



        public Visibility IsSecondaryTitleVisible
        {
            get { return (Visibility)GetValue(IsSecondaryTitleVisibleVisibleProperty); }
            set { SetValue(IsSecondaryTitleVisibleVisibleProperty, value); }
        }

        public static readonly DependencyProperty IsSecondaryTitleVisibleVisibleProperty =
            DependencyProperty.Register(name: nameof(IsSecondaryTitleVisible), propertyType: typeof(Visibility),
               ownerType: typeof(HeaderControl),
                typeMetadata: new PropertyMetadata(defaultValue: Visibility.Collapsed, propertyChangedCallback: OnSecondaryTitleVisibilityChanged));


        public string SecondaryTitleText
        {
            get { return (string)GetValue(SecondaryTitleTextProperty); }
            set { SetValue(SecondaryTitleTextProperty, value); }
        }

        public static readonly DependencyProperty SecondaryTitleTextProperty =
            DependencyProperty.Register(nameof(SecondaryTitleText), typeof(string), typeof(HeaderControl), new PropertyMetadata(defaultValue: string.Empty));




        public string AutoSuggestionText
        {
            get { return (string)GetValue(AutoSuggestionTextProperty); }
            set { SetValue(AutoSuggestionTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AutoSuggestionText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AutoSuggestionTextProperty =
            DependencyProperty.Register(nameof(AutoSuggestionText), typeof(string), typeof(HeaderControl), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty rbAssignmentTypeChangedCommandProperty =
    DependencyProperty.Register(nameof(rbAssignmentTypeChangedCommand), typeof(IAsyncRelayCommand),
        typeof(HeaderControl), new PropertyMetadata(defaultValue: null, OnPropertyChanged));

        public IAsyncRelayCommand rbAssignmentTypeChangedCommand
        {
            get { return (IAsyncRelayCommand)GetValue(rbAssignmentTypeChangedCommandProperty); }
            set { SetValue(rbAssignmentTypeChangedCommandProperty, value); }
        }



        #endregion

        #region constructor
        public HeaderControl()
        {
            this.InitializeComponent();
            DataContext = this;
        }
        #endregion

        #region Private methods

        private static void OnDisplayPathChanged(DependencyObject control, DependencyPropertyChangedEventArgs e)
        {
            (control as HeaderControl).AutoSuggestBox.DisplayMemberPath = (string)(e?.NewValue);
        }


        private static void OnCustomerNameChanged(DependencyObject control, DependencyPropertyChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(e.NewValue.ToString()))
                (control as HeaderControl).CustomerName.Text = (string)e.NewValue;

        }
        private static void OnCustomerAddressChanged(DependencyObject control, DependencyPropertyChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(e.NewValue.ToString()))
                (control as HeaderControl).CustomerAddress.Text = (string)e.NewValue;

        }
        private static void OnCustomerSubAddressChanged(DependencyObject control, DependencyPropertyChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(e.NewValue.ToString()))
                (control as HeaderControl).CustomerSubAddress.Text = (string)e.NewValue;

        }
        private static void OnCustomerTitleNameChanged(DependencyObject control, DependencyPropertyChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(e.NewValue.ToString()))
                (control as HeaderControl).CustomerTitleName.Text = (string)e.NewValue;

        }
        private static void OnCustomerTitleNumberChanged(DependencyObject control, DependencyPropertyChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(e.NewValue.ToString()))
                (control as HeaderControl).CustomerTitleNumber.Text = (string)e.NewValue;

        }
        private static void OnComboBoxVisibilityChanged(DependencyObject control, DependencyPropertyChangedEventArgs e)
        {
            (control as HeaderControl).HeaderComboBox.Visibility = (Visibility)e.NewValue;
        }
        private static void OnTravelComboBoxVisibilityChanged(DependencyObject control, DependencyPropertyChangedEventArgs e)
        {
            (control as HeaderControl).TravelComboBox.Visibility = (Visibility)e.NewValue;
        }
        private static void OnCustomerPanelVisibilityChanged(DependencyObject control, DependencyPropertyChangedEventArgs e)
        {
            (control as HeaderControl).CustomerPanel.Visibility = (Visibility)e.NewValue;
        }
        private static void OnCustomerTitlePanelVisibilityChanged(DependencyObject control, DependencyPropertyChangedEventArgs e)
        {
            (control as HeaderControl).CustomerTitlePanel.Visibility = (Visibility)e.NewValue;
        }

        private static void OnPageTitleVisibilityChanged(DependencyObject control, DependencyPropertyChangedEventArgs e)
        {
            var headerControl = control as HeaderControl;
            if (headerControl.IsAccountOwnerFilterVisible == Visibility.Visible)
            {
                headerControl.PageTitle.Visibility = Visibility.Collapsed;
            }
            else
            {
                headerControl.PageTitle.Visibility = (Visibility)e.NewValue;
            }
        }
        private static void OnCustomerPageTitleVisibilityChanged(DependencyObject control, DependencyPropertyChangedEventArgs e)
        {
            (control as HeaderControl).sp_CustomerPageTitle.Visibility = (Visibility)e.NewValue;
            if ((Visibility)e.NewValue == Visibility.Visible)
            {
                (control as HeaderControl).PageTitle.Visibility = Visibility.Collapsed;
                (control as HeaderControl).SecondayTitleTextBlock.Visibility = Visibility.Collapsed;
            }
        }
        private static void OnPageTitleChanged(DependencyObject control, DependencyPropertyChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(e.NewValue.ToString()))
                (control as HeaderControl).PageTitle.Text = (string)e.NewValue;
        }
        private static void OnAutoSuggestBoxVisibilityChanged(DependencyObject control, DependencyPropertyChangedEventArgs e)
        {
            (control as HeaderControl).AutoSuggestBox.Visibility = (Visibility)e.NewValue;
        }
        private static void OnPlaceHolderTextChanged(DependencyObject control, DependencyPropertyChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(e.NewValue.ToString()))
                (control as HeaderControl).AutoSuggestBox.PlaceholderText = (string)e.NewValue;

        }
        private static void OnTravelPlaceHolderTextChanged(DependencyObject control, DependencyPropertyChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(e.NewValue.ToString()))
                (control as HeaderControl).TravelComboBox.PlaceholderText = (string)e.NewValue;

        }

        private static void OnBadgeTextChanged(DependencyObject control, DependencyPropertyChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(e.NewValue.ToString()))
                (control as HeaderControl).BadgeValue.Text = (string)e.NewValue;

        }
        private static void OnLogOutVisibilityChanged(DependencyObject control, DependencyPropertyChangedEventArgs e)
        {
            (control as HeaderControl).LogOutButton.Visibility = (Visibility)e.NewValue;
        }
        private static void OnCartVisibilityChanged(DependencyObject control, DependencyPropertyChangedEventArgs e)
        {
            (control as HeaderControl).CartButton.Visibility = (Visibility)e.NewValue;
        }
        private static void OnBadgeVisibilityChanged(DependencyObject control, DependencyPropertyChangedEventArgs e)
        {
            (control as HeaderControl).BadgeView.Visibility = (Visibility)e.NewValue;
        }
        private static void OnRbAccountOwnerFilterChanged(DependencyObject control, DependencyPropertyChangedEventArgs e)
        {
            (control as HeaderControl).sp_AssignmentType.Visibility = (Visibility)e.NewValue;
        }
        private static void OnItemSourceChanged(DependencyObject control, DependencyPropertyChangedEventArgs e)
        {
            (control as HeaderControl).AutoSuggestBox.ItemsSource = e?.NewValue;
        }
        private static void OnTravelComboboxItemSourceChanged(DependencyObject control, DependencyPropertyChangedEventArgs e)
        {
            (control as HeaderControl).TravelComboBox.ItemsSource = e?.NewValue;
        }
        private void headerComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var senderName = (ComboBox)sender;
            ComboBoxValueChangedCommand?.Execute(senderName.SelectedIndex);
        }


        /// <summary>
        /// this event is fired when text is changed in suggestion box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private async void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (string.IsNullOrWhiteSpace(sender.Text))
            {
                AutoSuggestBox.Text = string.Empty;
                AutoSuggestBox.IsSuggestionListOpen = false;
            }

            await Task.Delay(200);
            TextValueChangedCommand?.Execute(sender.Text);

            await Task.Delay(200);
            if (string.IsNullOrWhiteSpace(sender.Text))
            {
                AutoSuggestBox.Text = string.Empty;
                AutoSuggestBox.IsSuggestionListOpen = false;
            }
        }


        private void AutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            SuggestionChoosenCommand?.Execute(args.SelectedItem);
        }

        private void AutoSuggestBox_FocusEngaged(Control sender, FocusEngagedEventArgs args)
        {
            if (string.IsNullOrWhiteSpace(AutoSuggestBox.Text))
                AutoSuggestBox.IsSuggestionListOpen = false;
            else
                AutoSuggestBox.IsSuggestionListOpen = true;
        }

        private static void OnPropertyChanged(DependencyObject control, DependencyPropertyChangedEventArgs e)
        {
            // Method intentionally left empty.
        }

        private static void OnSecondaryTitleVisibilityChanged(DependencyObject control, DependencyPropertyChangedEventArgs e)
        {
            (control as HeaderControl).SecondayTitleTextBlock.Visibility = (Visibility)e.NewValue;
        }

        private void TravelComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var senderName = (ComboBox)sender;
            TravelComboBoxValueChangedCommand?.Execute(senderName.SelectedIndex);
        }

        private void rbActivityData_Checked(object sender, RoutedEventArgs e)
        {
            rbActivityDataChangedCommand?.Execute(sender);
        }
        private async void rbAssignmentType_Checked(object sender, RoutedEventArgs e)
        {
            if (rbAssignmentTypeChangedCommand != null)
                await rbAssignmentTypeChangedCommand.ExecuteAsync(sender);
        }
        private static void OnRbDataVisibilityChanged(DependencyObject control, DependencyPropertyChangedEventArgs e)
        {
            (control as HeaderControl).sp_rbdata.Visibility = (Visibility)e.NewValue;
        }
        #endregion

        #region Public methods


        #endregion


    }
}
