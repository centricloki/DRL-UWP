using DevExpress.Mvvm.Native;

using DRLMobile.Core.Models.UIModels;
using DRLMobile.Uwp.Helpers;

using Microsoft.Toolkit.Mvvm.Input;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace DRLMobile.Uwp.CustomControls
{
    public sealed partial class CustomerListView : UserControl, INotifyPropertyChanged
    {
        IEnumerable<CustomerListControlUIModel> _initialDataList;

        public CustomerListView()
        {
            this.InitializeComponent();
            SortOrderCommand = new RelayCommand<string>(SortItems, (string args) => IsHeadQuarter == 1);
            this.Loaded += CustomerListView_Loaded;
        }

        #region Member
        public RelayCommand<string> SortOrderCommand { get; private set; }
        private short _isAscending = -1;
        public short IsAscending
        {
            get => _isAscending;
            set
            {
                _isAscending = value;
                OnPropertyChanged(nameof(IsAscending));
            }
        }
        public string _previousSortArg { get; set; }

        private string _popupEmptyText = "";
        public string PopupEmptyText
        {
            get => _popupEmptyText;
            set
            {
                _popupEmptyText = value;
                OnPropertyChanged(nameof(PopupEmptyText));
            }
        }

        private bool _isDataAvailable;
        public bool IsDataAvailable
        {
            get => _isDataAvailable;
            set
            {
                _isDataAvailable = value;
                OnPropertyChanged(nameof(IsDataAvailable));
            }
        }

        private bool _showKAMColumn;
        public bool ShowKAMColumn
        {
            get => _showKAMColumn;
            set
            {
                _showKAMColumn = value;
                OnPropertyChanged(nameof(_showKAMColumn));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion End Member

        #region Dependency properties

        public int IsHeadQuarter
        {
            get { return (int)GetValue(IsHeadQuarterProperty); }
            set
            {
                SetValue(IsHeadQuarterProperty, value);
                ShowKAMColumn = (IsHeadQuarter != 1);
            }
        }

        public static readonly DependencyProperty IsHeadQuarterProperty =
            DependencyProperty.Register(name: nameof(IsHeadQuarter), propertyType: typeof(int),
               ownerType: typeof(CustomerListView), typeMetadata: new PropertyMetadata(defaultValue: 0));

        public string PopupHeaderText
        {
            get { return (string)GetValue(PopupHeaderTextProperty); }
            set { SetValue(PopupHeaderTextProperty, value); }
        }

        public static readonly DependencyProperty PopupHeaderTextProperty =
            DependencyProperty.Register(name: nameof(PopupHeaderText), propertyType: typeof(string),
               ownerType: typeof(CustomerListView), typeMetadata: new PropertyMetadata(defaultValue: ""));

        public ObservableCollection<CustomerListControlUIModel> DataList
        {
            get { return (ObservableCollection<CustomerListControlUIModel>)GetValue(DataListProperty); }
            set { SetValue(DataListProperty, value); }
        }

        public static readonly DependencyProperty DataListProperty =
            DependencyProperty.Register(name: nameof(DataList), propertyType: typeof(ObservableCollection<CustomerListControlUIModel>),
               ownerType: typeof(CustomerListView), typeMetadata: new PropertyMetadata(defaultValue: null));


        public ICommand PopupCloseCommand
        {
            get { return (ICommand)GetValue(PopupCloseCommandProperty); }
            set { SetValue(PopupCloseCommandProperty, value); }
        }

        public static readonly DependencyProperty PopupCloseCommandProperty =
            DependencyProperty.Register(name: nameof(PopupCloseCommand), propertyType: typeof(ICommand),
               ownerType: typeof(CustomerDocumentControl), typeMetadata: new PropertyMetadata(defaultValue: null));


        #endregion End Dependency properties

        #region Methods       
        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private bool IsSortImageLoad(short args) => args != -1;
        private BitmapImage GetSortImagePath(short arg, string colName)
        {
            if (arg == -1 || colName != _previousSortArg) return null;
            string direction = (arg == 0) ? "sort_up" : "sort_down";
            return new BitmapImage(new Uri(String.Format("ms-appx:///Assets/Controls/{0}.png", direction)));
        }
        private void SortItems(string args)
        {
            short paramIsAscending = IsAscending;

            //Reset sort command
            if (_previousSortArg != args)
                paramIsAscending = -1;

            IEnumerable<CustomerListControlUIModel> sortedList;

            if (paramIsAscending == -1)
                paramIsAscending = 0;
            else if (paramIsAscending == 0)
                paramIsAscending = 1;
            else
            {
                _previousSortArg = null;
                paramIsAscending = -1;
            }


            if (paramIsAscending == -1)
                sortedList = _initialDataList.ToList();
            else
            {
                _previousSortArg = args;
                sortedList = DataList.OrderByColumnName(args, (paramIsAscending == 0)).ToList();
            }

            DataList.Clear();
            foreach (var item in sortedList)
            {
                DataList.Add(item);
            }

            _previousSortArg = _previousSortArg;
            //Keep IsAscending at last because it's trigger sort image function
            IsAscending = paramIsAscending;

        }
        #endregion End Methods

        #region Events
        private void CustomerListView_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataList != null && DataList.Count > 0)
            {
                if (IsHeadQuarter != 1)
                {
                    App AppReference = (App)(Application.Current);
                    DataList.ForEach(async x =>
                    {
                        x.ShowKAMColumn = true;
                        x.KAMUserName = await AppReference.QueryService.GetUserFullNameAsync(x.TerritoryID);
                    });
                }
                _initialDataList = DataList.ToList();
                IsDataAvailable = true;
            }
            else
            {
                IsDataAvailable = false;
                if (IsHeadQuarter == 1)
                    PopupEmptyText = "No Chain Location";
                else
                    PopupEmptyText = "No Head Quarter";
            }
        }
        private void OnContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            if (args.ItemIndex % 2 != 0)
            {
                //Grey background
                args.ItemContainer.Background = new SolidColorBrush(Color.FromArgb(223, 223, 223, 223));
            }
            else
            {
                //White background
                args.ItemContainer.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
            }
        }
        private async void ShowMoreButton_Click(object sender, RoutedEventArgs e)
        {
            // Create a style for the Close button with fixed width
            Style closeButtonStyle = new Style(typeof(Button));
            closeButtonStyle.Setters.Add(new Setter(Button.WidthProperty, 120.0)); // Set your desired width

            // Get the DataContext of the clicked button
            var button = sender as Button;
            if (button?.DataContext is CustomerPageUIModel customer)
            {
                // Create a ContentDialog to display the full ActivityComment
                var dialog = new ContentDialog
                {
                    Title = "Notes",
                    Content = new ScrollViewer
                    {
                        Content = new TextBlock
                        {
                            Text = customer.ActivityComment,
                            TextWrapping = TextWrapping.Wrap,
                            MinWidth = 400,
                        }
                    },
                    CloseButtonText = "Close",
                    CloseButtonStyle = closeButtonStyle, // Apply the custom style
                };

                // Show the dialog
                await dialog.ShowAsync();
            }
        }
        private void Button_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Hand, 0);
        }

        private void Button_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Arrow, 0);
        }
        #endregion End Events
    }
}
