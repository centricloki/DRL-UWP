using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DRLMobile.CustomControls
{
    public enum Result
    {
        Yes,
        No,
        Cancel,
        Nothing
    }
    public sealed partial class CustomContentDialog : ContentDialog
    {
        public Result Result { get; set; }

        #region Constructor
        public CustomContentDialog()
        {
            this.InitializeComponent();
            this.Result = Result.Nothing;
        }
        #endregion

        #region Private methods
        private void btn1_Click(object sender, RoutedEventArgs e)
        {
            this.Result = Result.Yes;
            dialog.Hide();
        }

        private void btn2_Click(object sender, RoutedEventArgs e)
        {
            this.Result = Result.No;
            dialog.Hide();
        }

        private void btn3_Click(object sender, RoutedEventArgs e)
        {
            this.Result = Result.Cancel;
            dialog.Hide();
        }

        #endregion
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(CustomContentDialog), new PropertyMetadata(defaultValue: string.Empty, propertyChangedCallback: OnTitleChanged));

        private static void OnTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(e.NewValue.ToString()))
                (d as CustomContentDialog).title.Text = (string)e.NewValue;
        }

        public string FirstButtonText
        {
            get { return (string)GetValue(FirstButtonTextProperty); }
            set { SetValue(FirstButtonTextProperty, value); }
        }

        public static readonly DependencyProperty FirstButtonTextProperty =
            DependencyProperty.Register(nameof(FirstButtonText), typeof(string), typeof(CustomContentDialog), new PropertyMetadata(defaultValue: string.Empty, propertyChangedCallback: OnFirstButtonTextChanged));

        private static void OnFirstButtonTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(e.NewValue.ToString()))
                (d as CustomContentDialog).btn1.Content = (string)e.NewValue;
        }

        public string SecondButtonText
        {
            get { return (string)GetValue(SecondButtonTextProperty); }
            set { SetValue(SecondButtonTextProperty, value); }
        }

        public static readonly DependencyProperty SecondButtonTextProperty =
            DependencyProperty.Register(nameof(SecondButtonText), typeof(string), typeof(CustomContentDialog), new PropertyMetadata(defaultValue: string.Empty, propertyChangedCallback: OnSecondButtonTextChanged));

        private static void OnSecondButtonTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(e.NewValue.ToString()))
                (d as CustomContentDialog).btn2.Content = (string)e.NewValue;
        }

        public string CancelButtonText
        {
            get { return (string)GetValue(CancelButtonTextProperty); }
            set { SetValue(CancelButtonTextProperty, value); }
        }

        public static readonly DependencyProperty CancelButtonTextProperty =
            DependencyProperty.Register(nameof(CancelButtonText), typeof(string), typeof(CustomContentDialog), new PropertyMetadata(defaultValue: string.Empty, propertyChangedCallback: OnCancelButtonTextChanged));

        private static void OnCancelButtonTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(e.NewValue.ToString()))
                (d as CustomContentDialog).btn3.Content = (string)e.NewValue;
        }
    }
}
