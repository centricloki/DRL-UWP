using DevExpress.UI.Xaml.Editors;

using DRLMobile.Core.Models.UIModels;
using DRLMobile.Uwp.Helpers;
using DRLMobile.Uwp.ViewModel;

using System;
using System.Linq;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DRLMobile.Uwp.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PopOrderPage : Page
    {
        private PopOrderPageViewModel PopOrderViewModel = new PopOrderPageViewModel();

        public PopOrderPage()
        {
            this.InitializeComponent();
            DataContext = PopOrderViewModel;
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.New)
            {
                base.OnNavigatedTo(e);
                PopOrderViewModel.PageLoadedCommand.Execute(null);
            }
            else
            {
                PopOrderViewModel.UpdateDataOnBackPressedCommand.Execute(null);
            }
        }

        private void quantityTextBlock_BeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            if (args.NewText.Trim().Length == 1 && args.NewText.Trim().Equals("0"))
            {
                args.Cancel = true;
            }
            else
            {
                args.Cancel = args.NewText.Any(c => !char.IsDigit(c));

            }
        }

        private void quantityTextBlock_GotFocus(object sender, RoutedEventArgs e)
        {
            var senderName = (TextBox)sender;
            var dataCxtx = senderName.DataContext;
            var dataSource = (PopOrderCartUiModel)dataCxtx;
            PopOrderViewModel.GridItemModel = dataSource;
            PopOrderViewModel.quantityBeforeEdit = dataSource.QuantityDisplay;
            FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
        }

        private void QuantityTextBlock_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            var senderName = (TextBox)sender;
            var dataCxtx = senderName.DataContext;
            var dataSource = (PopOrderCartUiModel)dataCxtx;
            if (!string.IsNullOrEmpty(senderName.Text))
            {
                dataSource.Quantity = Convert.ToInt32(senderName.Text);
            }
            PopOrderViewModel.QuantityChangedCommand.Execute(dataSource);

        }

        private void CustomKeypad_DoneClickEvent1(object sender, bool e)
        {
            QuantityGridCustomKeyPadFlyout.Hide();
        }

        private void QuantityCustomKeyPadFlyout_Closing(FlyoutBase sender, FlyoutBaseClosingEventArgs args)
        {
            //if (string.IsNullOrEmpty(PopOrderViewModel?.GridItemModel?.QuantityDisplay))
            //{
            //    PopOrderViewModel.GridItemModel.QuantityDisplay = PopOrderViewModel?.quantityBeforeEdit;

            //    if (!string.IsNullOrEmpty(PopOrderViewModel?.GridItemModel?.QuantityDisplay))
            //    {
            //        PopOrderViewModel.GridItemModel.Quantity = Convert.ToInt32(PopOrderViewModel?.GridItemModel?.QuantityDisplay);
            //    }
            //}

            PopOrderViewModel.quantityString = string.Empty;

            PopOrderViewModel.QuantityChangedCommand.Execute(PopOrderViewModel.GridItemModel);
        }

        private void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var senderName = (Image)sender;
            var dataCxtx = senderName.DataContext;
            var dataSource = (PopOrderCartUiModel)dataCxtx;
            PopOrderViewModel?.CartImageCommand.Execute(dataSource);
        }

        private void CategoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var senderName = (ComboBox)sender;
            var selectedItem = (BrandUIModel)senderName.SelectedItem;
            if (selectedItem != null)
            {
                PopOrderViewModel.HierarchyTag = 1;
                PopOrderViewModel.PopOrderPageModel.SelectedCategory = selectedItem;
                SetSelectedItem(selectedItem);
            }
        }

        private void SetSelectedItem(BrandUIModel selectedItem)
        {
            if (selectedItem?.BrandName != "Select")
            {
                var BrandId = selectedItem?.BrandId;
                PopOrderViewModel.ComboBoxSelectionChangedCommand.Execute(BrandId);
            }
            else
            {
                PopOrderViewModel.ComboBoxSelectionChangedCommand.Execute(0);
            }
        }

        private void MaterialComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var senderName = (ComboBox)sender;
            var selectedItem = (BrandUIModel)senderName.SelectedItem;
            if (selectedItem != null)
            {
                PopOrderViewModel.HierarchyTag = 2;
                PopOrderViewModel.PopOrderPageModel.SelectedMaterial = selectedItem;
                SetSelectedItem(selectedItem);
            }
        }

        private void FamilyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var senderName = (ComboBox)sender;
            var selectedItem = (BrandUIModel)senderName.SelectedItem;
            if (selectedItem != null)
            {

                PopOrderViewModel.HierarchyTag = 3;
                PopOrderViewModel.PopOrderPageModel.SelectedFamily = selectedItem;
                SetSelectedItem(selectedItem);
            }
        }

        private void BrandComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var senderName = (ComboBox)sender;
            var selectedItem = (BrandUIModel)senderName.SelectedItem;
            if (selectedItem != null)
            {

                PopOrderViewModel.HierarchyTag = 4;
                PopOrderViewModel.PopOrderPageModel.SelectedBrand = selectedItem;
                SetSelectedItem(selectedItem);
            }
        }

        private void GroupComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var senderName = (ComboBox)sender;
            var selectedItem = (BrandUIModel)senderName.SelectedItem;
            if (selectedItem != null)
            {

                PopOrderViewModel.HierarchyTag = 5;
                PopOrderViewModel.PopOrderPageModel.SelectedGroup = selectedItem;
                SetSelectedItem(selectedItem);
            }
        }

        private void ProductImage_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (sender is Grid)
            {
                var dataContext = (sender as Grid).DataContext as PopOrderCartUiModel;

                if (dataContext != null && !string.IsNullOrWhiteSpace(dataContext.ProductImagePath) && !dataContext.ProductImagePath.Equals((string)Application.Current.Resources["PlaceholderImage"]))
                {
                    PopOrderViewModel.PreviewUrl = dataContext.ProductImagePath;
                    PopOrderViewModel.IsPreviewDocumentVisibile = true;
                }

                if (PopOrderViewModel.PrintHelper == null)
                {
                    // Initalize receipt print helper class and register for printing
                    PopOrderViewModel.PrintHelper = new PhotosPrintHelper(this, PopOrderViewModel.PreviewUrl);

                    PopOrderViewModel.PrintHelper.RegisterForPrinting("PopOrderPage");
                }
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);

            if (PopOrderViewModel.PrintHelper != null)
            {
                PopOrderViewModel.PrintHelper.UnregisterForPrinting();
                PopOrderViewModel.PrintHelper = null;
            }
        }

        private void QuantityEditText_GotFocus(object sender, RoutedEventArgs e)
        {
            var senderName = (TextEdit)sender;
            var dataCxtx = senderName.DataContext;
            var dataSource = (PopOrderCartUiModel)dataCxtx;

            PopOrderViewModel.GridItemModel = dataSource;
            PopOrderViewModel.quantityBeforeEdit = dataSource.QuantityDisplay;

            FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
        }

        private void QuantityEditText_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            var senderName = (TextEdit)sender;
            var dataCxtx = senderName.DataContext;
            var dataSource = (PopOrderCartUiModel)dataCxtx;

            if (!string.IsNullOrEmpty(senderName.Text))
            {
                dataSource.Quantity = Convert.ToInt32(senderName.Text);
            }
            PopOrderViewModel.quantityString = "";
            PopOrderViewModel.QuantityChangedCommand.Execute(dataSource);
        }
    }
}
