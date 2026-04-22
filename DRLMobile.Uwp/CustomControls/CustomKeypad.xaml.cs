using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace DRLMobile.Uwp.CustomControls
{
    public sealed partial class CustomKeypad : UserControl
    {
        public event EventHandler<bool> DoneClickEvent;
        #region Constructor
        public CustomKeypad()
        {
            this.InitializeComponent();
            DataContext = this;
        }
        #endregion
        #region Dependency properties
        public static readonly DependencyProperty NumpadButtonClickProperty = DependencyProperty.Register("NumPadButtonClick",
       typeof(ICommand),
       typeof(CustomKeypad),
       new PropertyMetadata(null));
        public ICommand NumPadButtonClick
        {
            get { return (ICommand)GetValue(NumpadButtonClickProperty); }
            set { SetValue(NumpadButtonClickProperty, value); }
        }
        public static readonly DependencyProperty DoneButtonClickProperty = DependencyProperty.Register("DoneButtonClick",
       typeof(ICommand),
       typeof(CustomKeypad),
       new PropertyMetadata(null));
        public ICommand DoneButtonClick
        {
            get { return (ICommand)GetValue(DoneButtonClickProperty); }
            set { SetValue(DoneButtonClickProperty, value); }
        }
        public Visibility IsMinusButtonVisible
        {
            get { return (Visibility)GetValue(IsMinusButtonVisibleProperty); }
            set { SetValue(IsMinusButtonVisibleProperty, value); }
        }

        public static readonly DependencyProperty IsMinusButtonVisibleProperty =
            DependencyProperty.Register(name: nameof(IsMinusButtonVisible), propertyType: typeof(Visibility),
               ownerType: typeof(CustomKeypad),
                typeMetadata: new PropertyMetadata(defaultValue: Visibility.Visible, propertyChangedCallback: OnMinusButtonVisibilityChanged));
        public Visibility IsDotButtonVisible
        {
            get { return (Visibility)GetValue(IsDotButtonVisibleProperty); }
            set { SetValue(IsDotButtonVisibleProperty, value); }
        }

        public static readonly DependencyProperty IsDotButtonVisibleProperty =
            DependencyProperty.Register(name: nameof(IsDotButtonVisible), propertyType: typeof(Visibility),
               ownerType: typeof(CustomKeypad),
                typeMetadata: new PropertyMetadata(defaultValue: Visibility.Collapsed, propertyChangedCallback: OnDotButtonVisibilityChanged));


        #endregion
        #region Private Methods
        private static void OnMinusButtonVisibilityChanged(DependencyObject control, DependencyPropertyChangedEventArgs e)
        {
            (control as CustomKeypad).MinusButton.Visibility = (Visibility)e.NewValue;
        }
        private static void OnDotButtonVisibilityChanged(DependencyObject control, DependencyPropertyChangedEventArgs e)
        {
            (control as CustomKeypad).DotButton.Visibility = (Visibility)e.NewValue;
        }

        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DoneClickEvent?.Invoke(this, true);
        }
    }
}
