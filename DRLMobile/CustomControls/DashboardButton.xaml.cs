using System;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace DRLMobile.CustomControls
{
    public sealed partial class DashboardButton : UserControl
    {

        #region Depencency properties

        public double TitleFontSize
        {
            get { return (double)GetValue(TitleFontSizeProperty); }
            set { SetValue(TitleFontSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TitleText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleFontSizeProperty =
            DependencyProperty.Register(name: nameof(TitleFontSize), propertyType: typeof(double),
               ownerType: typeof(DashboardButton),
                typeMetadata: new PropertyMetadata(defaultValue: 16d, propertyChangedCallback: OnTitleFontSizeChanged));



        public Visibility IsBadgeVisible
        {
            get { return (Visibility)GetValue(IsBadgeVisibleProperty); }
            set { SetValue(IsBadgeVisibleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TitleText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsBadgeVisibleProperty =
            DependencyProperty.Register(name: nameof(IsBadgeVisible), propertyType: typeof(Visibility),
               ownerType: typeof(DashboardButton),
                typeMetadata: new PropertyMetadata(defaultValue: Visibility.Collapsed, propertyChangedCallback: OnBadgeVisibilityChanged));



        public string TitleText
        {
            get { return (string)GetValue(TitleTextProperty); }
            set { SetValue(TitleTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TitleText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleTextProperty =
            DependencyProperty.Register(name: nameof(TitleText), propertyType: typeof(string),
               ownerType: typeof(DashboardButton),
                typeMetadata: new PropertyMetadata(defaultValue: string.Empty, propertyChangedCallback: OnTextChange));

        public string NormalStateImageSource
        {
            get { return (string)GetValue(NormalStateImageSourceProperty); }
            set { SetValue(NormalStateImageSourceProperty, value); }
        }

        public static readonly DependencyProperty NormalStateImageSourceProperty =
            DependencyProperty.Register(name: nameof(NormalStateImageSource), propertyType: typeof(string),
               ownerType: typeof(DashboardButton),
               typeMetadata: new PropertyMetadata(defaultValue: string.Empty));


        public string HoverStateImageSource
        {
            get { return (string)GetValue(HoverStateImageSourceProperty); }
            set { SetValue(HoverStateImageSourceProperty, value); }
        }

        public static readonly DependencyProperty HoverStateImageSourceProperty =
            DependencyProperty.Register(name: nameof(HoverStateImageSource), propertyType: typeof(string),
               ownerType: typeof(DashboardButton),
               typeMetadata: new PropertyMetadata(defaultValue: string.Empty));

        public static readonly DependencyProperty ButtonClickProperty =
        DependencyProperty.Register(
            "ButtonClick",
            typeof(ICommand),
            typeof(DashboardButton),
            new PropertyMetadata(null));
        public ICommand ButtonClick
        {
            get { return (ICommand)GetValue(ButtonClickProperty); }
            set { SetValue(ButtonClickProperty, value); }
        }

        public double ImageHeightWidth
        {
            get { return (double)GetValue(ImageHeightWidthProperty); }
            set { SetValue(ImageHeightWidthProperty, value); }
        }
        public static readonly DependencyProperty ImageHeightWidthProperty =
            DependencyProperty.Register(nameof(ImageHeightWidth), typeof(double), typeof(DashboardButton), new PropertyMetadata(defaultValue: 80d));


        public string BadgeText
        {
            get { return (string)GetValue(BadgeTextProperty); }
            set { SetValue(BadgeTextProperty, value); }
        }

        public static readonly DependencyProperty BadgeTextProperty =
            DependencyProperty.Register(nameof(BadgeText), typeof(string), typeof(DashboardButton), new PropertyMetadata(defaultValue: string.Empty, propertyChangedCallback: OnBadgeTextChanged));

      

        #endregion

        #region constructor
        public DashboardButton()
        {
            this.InitializeComponent();
            DataContext = this;
        }
        #endregion

        #region private methods
        private static void OnTextChange(DependencyObject control, DependencyPropertyChangedEventArgs e)
        {
            (control as DashboardButton).Title.Text = e.NewValue.ToString() ?? string.Empty;
        }


        private static void OnTitleFontSizeChanged(DependencyObject control, DependencyPropertyChangedEventArgs e)
        {
            (control as DashboardButton).Title.FontSize = (double)e.NewValue;
        }

        private static void OnBadgeVisibilityChanged(DependencyObject control, DependencyPropertyChangedEventArgs e)
        {
            (control as DashboardButton).BadgeView.Visibility = (Visibility)e.NewValue;
        }

        private void Button_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(HoverStateImageSource))
                ButtonImage.Source = new BitmapImage(new Uri(HoverStateImageSource));
        }

        private void Button_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(HoverStateImageSource))
                ButtonImage.Source = new BitmapImage(new Uri(NormalStateImageSource));
        }
        private static void OnBadgeTextChanged(DependencyObject control, DependencyPropertyChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(e.NewValue.ToString()))
                (control as DashboardButton).BadgeValue.Text = (string)e.NewValue;

        }
        #endregion

    }
}
