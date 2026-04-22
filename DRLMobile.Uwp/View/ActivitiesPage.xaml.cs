using DRLMobile.Core.Models.UIModels;
using DRLMobile.ExceptionHandler;
using DRLMobile.Uwp.Helpers;
using DRLMobile.Uwp.Services;
using DRLMobile.Uwp.ViewModel;

using System;
using System.Linq;

using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DRLMobile.Uwp.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ActivitiesPage : Page
    {
        private ActivitiesPageViewModel ViewModel = new ActivitiesPageViewModel();
        private object navigationEventParameter;
        private ActivityForAllCustomerUIModel DeleteActivtyParameter;
        public ActivitiesPage()
        {
            this.InitializeComponent();
            this.Loaded += ActivitiesPage_Loaded;
            this.Unloaded += ActivitiesPage_Unloaded;
        }

        private void ActivitiesPage_Unloaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            //ViewModel.DbDataForAllCustomer = null;
            //ViewModel.AllCustomerActivitiesItemSource = null;
            //ViewModel.DbDataForIndividualCustomer = null;
            //ViewModel.HeaderSearchItemSource = null;
            ViewModel.DispatcherQueueTimerCleanup();
        }

        private void ActivitiesPage_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (((Frame)Parent) != null)
            {
                var cacheSize = ((Frame)Parent).CacheSize;
                ((Frame)Parent).CacheSize = 0;
                ((Frame)Parent).CacheSize = cacheSize;
            }
            HeaderControl.AutoSuggestionText = string.Empty;

            ActivitiesPageViewModel.AddedActivity = null;
            ActivitiesPageViewModel.EditedActivity = null;

            ViewModel.OnNavigatedToCommand.Execute(navigationEventParameter);

            if (ToolkitDataGrid != null)
            {
                ToolkitDataGrid.SelectedItem = null;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            navigationEventParameter = e.Parameter;

            //HeaderControl.AutoSuggestionText = string.Empty;

            //ActivitiesPageViewModel.AddedActivity = null;
            //ActivitiesPageViewModel.EditedActivity = null;

            //ViewModel.OnNavigatedToCommand.Execute(e.Parameter);

            //if (ToolkitDataGrid != null)
            //{
            //    ToolkitDataGrid.SelectedItem = null;
            //}

            ////if (e.NavigationMode == NavigationMode.New)
            ////{
            ////    base.OnNavigatedTo(e);
            ////    HeaderControl.AutoSuggestionText = string.Empty;
            ////    ActivitiesPageViewModel.AddedActivity = null;
            ////    ActivitiesPageViewModel.EditedActivity = null;
            ////    ViewModel.OnNavigatedToCommand.Execute(e.Parameter);
            ////}
            ////else if (ActivitiesPageViewModel.EditedActivity != null)
            ////{
            ////    ViewModel.EditedCommand.Execute(null);
            ////    if (ToolkitDataGrid != null)
            ////        ToolkitDataGrid.SelectedItem = null;
            ////}
            ////else
            ////{
            ////    ViewModel.AddNewActivityCommand.Execute(null);

            ////    if (ToolkitDataGrid != null)
            ////    {
            ////        ToolkitDataGrid.SelectedItem = null;
            ////    }
            ////}
        }

        private void ToolkitDataGrid_LoadingRowGroup(object sender, Microsoft.Toolkit.Uwp.UI.Controls.DataGridRowGroupHeaderEventArgs e)
        {
            ICollectionViewGroup group = e.RowGroupHeader.CollectionViewGroup;
            ActivityForIndividualCustomerUIModel item = group.GroupItems[0] as ActivityForIndividualCustomerUIModel;
            e.RowGroupHeader.PropertyValue = item.Date;
        }

        private void ToolkitDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.RemovedItems.FirstOrDefault() is ActivityForIndividualCustomerUIModel && (e.RemovedItems.FirstOrDefault() as ActivityForIndividualCustomerUIModel).ActivityType.Contains(ResourceExtensions.GetLocalized("NoResultsErrorMessage")))
                return;
            if ((sender as Microsoft.Toolkit.Uwp.UI.Controls.DataGrid).SelectedItem != null)
            {
                ViewModel?.CustomerGridItemClick.Execute((sender as Microsoft.Toolkit.Uwp.UI.Controls.DataGrid).SelectedItem);
            }
        }

        //private void AllCustomerDataGrid_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        //{
        //    if (AllCustomerDataGrid != null)
        //    {
        //        AllCustomerDataGrid.SelectedItem = null;
        //    }
        //}

        //private void AllCustomerDataGrid_EndSorting(object sender, System.EventArgs e)
        //{
        //    if (AllCustomerDataGrid != null)
        //    {
        //        AllCustomerDataGrid.SelectedItem = null;
        //    }
        //}
        private void Flyout_Opened(object sender, object e)
        {
            try
            {
                var isExported = ((ActivityForAllCustomerUIModel)((DevExpress.UI.Xaml.Grid.GridRowCellContextMenuInfo)((Windows.UI.Xaml.FrameworkElement)((Flyout)sender).Content).DataContext).Row.DataContext).IsExported;
                var childrens = ((Panel)((Flyout)sender).Content).Children;
                if (isExported == 0)
                {
                    foreach (var ui in childrens)
                    {
                        if (ui is TextBlock)
                        {
                            (ui as TextBlock).Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                        }
                        else if (ui is Button)
                        {
                            (ui as Button).Visibility = Windows.UI.Xaml.Visibility.Visible;
                        }
                    }
                }
                else
                {
                    foreach (var ui in childrens)
                    {
                        if (ui is TextBlock)
                        {
                            (ui as TextBlock).Visibility = Windows.UI.Xaml.Visibility.Visible;
                        }
                        else if (ui is Button)
                        {
                            (ui as Button).Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(ActivitiesPage), nameof(Flyout_Opened), ex.StackTrace);
            }
        }

        private void ContextMenu_DeleteClicked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            try
            {
                (((Windows.UI.Xaml.FrameworkElement)((Windows.UI.Xaml.FrameworkElement)((Windows.UI.Xaml.FrameworkElement)sender).Parent).Parent).Parent as Popup).IsOpen = false;

                var dataContext = ((DevExpress.UI.Xaml.Grid.GridRowCellContextMenuInfo)((Windows.UI.Xaml.FrameworkElement)sender).DataContext).Row.DataContext;

                if (dataContext != null)
                {
                    ViewModel?.DeleteActivityCommand.Execute(dataContext);
                }
            }
            catch (System.Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(ActivitiesPage), nameof(ContextMenu_DeleteClicked), ex.StackTrace);
            }
        }

        //protected override void OnNavigatedFrom(NavigationEventArgs e)
        //{
        //    base.OnNavigatedFrom(e);

        //    ViewModel.DbDataForAllCustomer.Clear();
        //    ViewModel.AllCustomerActivitiesItemSource.Clear();
        //    ViewModel.DbDataForIndividualCustomer.Clear();
        //    ViewModel.HeaderSearchItemSource.Clear();

        //    ViewModel.DbDataForAllCustomer = null;
        //    ViewModel.AllCustomerActivitiesItemSource = null;
        //    ViewModel.DbDataForIndividualCustomer = null;
        //    ViewModel.HeaderSearchItemSource = null;
        //}

        public BitmapImage GetSortImagePath(string args)
        {
            return new BitmapImage(new Uri(String.Format("ms-appx:///Assets/Controls/{0}.png", args)));
        }

        public Boolean IsNotEmpty(string args)
        {
            return !string.IsNullOrWhiteSpace(args);
        }

        private void AllCustomerDataGrid_ItemClick(object sender, ItemClickEventArgs e)
        {
            NavigationService.NavigateShellFrame(typeof(EditActivityPage), (e.ClickedItem as ActivityForAllCustomerUIModel));
        }
        private void gridItem_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            try
            {
                ListView listView = (ListView)sender;
                deleteflyout.ShowAt(listView, e.GetPosition(listView));
                var lstobj = ((FrameworkElement)e.OriginalSource).DataContext;
                DeleteActivtyParameter = ((ActivityForAllCustomerUIModel)(lstobj));
                CheckDeleteVisbility();
            }
            catch (System.Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(ActivitiesPage), nameof(gridItem_RightTapped), ex.StackTrace);
            }
        }
        private void lstAllActivities_Holding(object sender, HoldingRoutedEventArgs e)
        {
            try
            {
                ListView listView = (ListView)sender;
                deleteflyout.ShowAt(listView, e.GetPosition(listView));
                var lstobj = ((FrameworkElement)e.OriginalSource).DataContext;
                DeleteActivtyParameter = ((ActivityForAllCustomerUIModel)(lstobj));
                CheckDeleteVisbility();
            }
            catch (System.Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(ActivitiesPage), nameof(gridItem_RightTapped), ex.StackTrace);
            }
        }

        private void CheckDeleteVisbility()
        {
            var isExported = DeleteActivtyParameter.IsExported;
            var activityOrder = DeleteActivtyParameter.OrderID;
            if (isExported == 0 && string.IsNullOrEmpty(activityOrder))
            {
                ViewModel.IsDeleteActivityVisible = true;
                ViewModel.IsNoActionVisible = false;
            }
            else
            {
                ViewModel.IsDeleteActivityVisible = false;
                ViewModel.IsNoActionVisible = true;
            }
        }
        private void DeleteActivity_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (DeleteActivtyParameter != null)
                {
                    ViewModel?.DeleteActivityCommand.Execute(DeleteActivtyParameter);
                }
            }
            catch (System.Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(ActivitiesPage), nameof(DeleteActivity_Click), ex.StackTrace);
            }
        }
        private void AllCustomerDataGrid_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
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

    }
}
