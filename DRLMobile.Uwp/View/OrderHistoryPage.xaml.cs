using DRLMobile.Core.Models.DataModels;
using DRLMobile.Core.Models.UIModels;
using DRLMobile.ExceptionHandler;
using DRLMobile.Uwp.ViewModel;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DRLMobile.Uwp.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class OrderHistoryPage : Page
    {
        public static OrderDetailUpdatedModel UpdatedOrder;

        private OrderHistoryPageViewModel ViewModel = new OrderHistoryPageViewModel();
        public OrderHistoryPage()
        {
            this.InitializeComponent();
            this.DataContext = ViewModel;
        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.NavigationMode == NavigationMode.New)
            {
                UpdatedOrder = null;
                ViewModel.OnNavigatedToCommand.Execute(e.Parameter);
            }
            else if (e.NavigationMode == NavigationMode.Back)
            {
                if (UpdatedOrder != null)
                {
                    ViewModel.OnNavigatedToCommand.Execute(UpdatedOrder);
                }
            }
        }

        private void ReorderTapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            ViewModel.IsReorderTapped = true;
        }

        private void ContextMenu_DeleteClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                (((FrameworkElement)((FrameworkElement)((FrameworkElement)sender).Parent).Parent).Parent as Popup).IsOpen = false;
                var dataContext = ((DevExpress.UI.Xaml.Grid.GridRowCellContextMenuInfo)((Windows.UI.Xaml.FrameworkElement)sender).DataContext).Row.DataContext as OrderHistoryUIModel;
                if(dataContext!=null)
                {
                    ViewModel?.DeleteOrderHistoryCommand.Execute(dataContext);
                }

            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(OrderHistoryPage), nameof(ContextMenu_DeleteClicked), ex.StackTrace + " - " + ex.Message);
            }
        }

        private void Flyout_Opened(object sender, object e)
        {
            try
            {
                var isExported = (((DevExpress.UI.Xaml.Grid.GridRowCellContextMenuInfo)((FrameworkElement)((Flyout)sender).Content).DataContext).Row.DataContext as OrderHistoryUIModel)?.IsExported;
                if (isExported.HasValue)
                {
                    var childrens = ((Panel)((Flyout)sender).Content).Children;
                    if (isExported.Value == 0)
                    {
                        ShowDeleteOnContextFlyout(childrens);
                    }
                    else
                    {
                        ShowNoActionOnContextFlyout(childrens);
                    }

                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(OrderHistoryPage), nameof(Flyout_Opened), ex.StackTrace);
            }
        }

        private void ShowNoActionOnContextFlyout(UIElementCollection childrens)
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

        private void ShowDeleteOnContextFlyout(UIElementCollection childrens)
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
    }
}
