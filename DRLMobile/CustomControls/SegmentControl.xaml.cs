using DRLMobile.Core.Enums;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace DRLMobile.CustomControls
{
    public sealed partial class SegmentControl : UserControl
    {
        private readonly SolidColorBrush GrayColor = (SolidColorBrush)Application.Current.Resources["LoginButtonBackground"];
        private readonly SolidColorBrush WhiteColor = (SolidColorBrush)Application.Current.Resources["FooterTextColor"];


        public SegmentControl()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }


        #region Dependency properties

        public string LeftSegmentText
        {
            get { return (string)GetValue(LeftSegmentTextProperty); }
            set { SetValue(LeftSegmentTextProperty, value); }
        }
        public static readonly DependencyProperty LeftSegmentTextProperty =
            DependencyProperty.Register(nameof(LeftSegmentText), typeof(string), typeof(SegmentControl), new PropertyMetadata(string.Empty));

        public string CenterSegmentText
        {
            get { return (string)GetValue(CenterSegmentTextProperty); }
            set { SetValue(CenterSegmentTextProperty, value); }
        }
        public static readonly DependencyProperty CenterSegmentTextProperty =
            DependencyProperty.Register(nameof(CenterSegmentText), typeof(string), typeof(SegmentControl), new PropertyMetadata(string.Empty));

        public string RightSegmentText
        {
            get { return (string)GetValue(RightSegmentTextProperty); }
            set { SetValue(RightSegmentTextProperty, value); }
        }
        public static readonly DependencyProperty RightSegmentTextProperty =
            DependencyProperty.Register(nameof(RightSegmentText), typeof(string), typeof(SegmentControl), new PropertyMetadata(string.Empty));

        public Segment SelectedSegment
        {
            get { return (Segment)GetValue(SelectedSegmentProperty); }
            set { SetValue(SelectedSegmentProperty, value); }
        }
        public static readonly DependencyProperty SelectedSegmentProperty =
            DependencyProperty.Register(nameof(SelectedSegment), typeof(Segment), typeof(SegmentControl), new PropertyMetadata(Segment.None, propertyChangedCallback: OnDefaultValueChanged));

        public ICommand LeftSegmentCommand
        {
            get { return (ICommand)GetValue(LeftSegmentCommandProperty); }
            set { SetValue(LeftSegmentCommandProperty, value); }
        }
        public static readonly DependencyProperty LeftSegmentCommandProperty =
            DependencyProperty.Register(nameof(LeftSegmentCommand), typeof(ICommand), typeof(SegmentControl), new PropertyMetadata(null));

        public ICommand CenterSegmentCommand
        {
            get { return (ICommand)GetValue(CenterSegmentCommandProperty); }
            set { SetValue(CenterSegmentCommandProperty, value); }
        }
        public static readonly DependencyProperty CenterSegmentCommandProperty =
            DependencyProperty.Register(nameof(CenterSegmentCommand), typeof(ICommand), typeof(SegmentControl), new PropertyMetadata(null));


        public ICommand RightSegmentCommand
        {
            get { return (ICommand)GetValue(RightSegmentCommandProperty); }
            set { SetValue(RightSegmentCommandProperty, value); }
        }
        public static readonly DependencyProperty RightSegmentCommandProperty =
            DependencyProperty.Register(nameof(RightSegmentCommand), typeof(ICommand), typeof(SegmentControl), new PropertyMetadata(0));

        public Visibility IsCenterSegmentTextVisible
        {
            get { return (Visibility)GetValue(IsCenterSegmentTextVisibleProperty); }
            set { SetValue(IsCenterSegmentTextVisibleProperty, value); }
        }

        public static readonly DependencyProperty IsCenterSegmentTextVisibleProperty =
            DependencyProperty.Register(name: nameof(IsCenterSegmentTextVisible), propertyType: typeof(Visibility),
               ownerType: typeof(SegmentControl),
                typeMetadata: new PropertyMetadata(defaultValue: Visibility.Collapsed, propertyChangedCallback: OnCenterSegmentVisibilityChanged));

        private static void OnCenterSegmentVisibilityChanged(DependencyObject control, DependencyPropertyChangedEventArgs e)
        {
            (control as SegmentControl).CenterSegmentGrid.Visibility = (Visibility)e.NewValue;
        }
        #endregion


        private static void OnDefaultValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var segment = (Segment)e.NewValue;
            var control = d as SegmentControl;
            if (segment== Segment.Left)
            {
                control.LeftSegmentSelection();
            }
            else if(segment== Segment.Right)
            {
                control.RightSegmentSelection();
            }
            else if (segment == Segment.Center)
            {
                control.CenterSegmentSelection();
            }
        }


        private void LeftSegmentSelection()
        {
            LeftSegmentGrid.Background = WhiteColor;
            RightSegmentGrid.Background = GrayColor;
            CenterSegmentGrid.Background = GrayColor;
            LeftSegmentCommand?.Execute(null);
        }

        private void CenterSegmentSelection()
        {
            LeftSegmentGrid.Background = GrayColor;
            RightSegmentGrid.Background = GrayColor;
            CenterSegmentGrid.Background = WhiteColor;
            CenterSegmentCommand?.Execute(null);
        }
        private void RightSegmentSelection()
        {
            RightSegmentGrid.Background = WhiteColor;
            LeftSegmentGrid.Background = GrayColor;
            CenterSegmentGrid.Background = GrayColor;
            RightSegmentCommand?.Execute(null);
        }

        private void LeftSegmentTapped(object sender, TappedRoutedEventArgs e)
        {
            LeftSegmentSelection();
        }

        private void RightSegmentTapped(object sender, TappedRoutedEventArgs e)
        {
            RightSegmentSelection();
        }

        private void CenterSegmentGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            CenterSegmentSelection();
        }
    }
}
