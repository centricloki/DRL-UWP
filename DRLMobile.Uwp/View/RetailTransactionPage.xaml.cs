using DRLMobile.Core.Models.UIModels;
using DRLMobile.Uwp.Helpers;
using DRLMobile.Uwp.ViewModel;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Globalization;
using System.Linq;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using Windows.Graphics.Printing;
using System.Collections.Generic;
using DRLMobile.Uwp.Services;
using Windows.UI.Popups;
using DevExpress.UI.Xaml.Editors;
using DRLMobile.Core.Enums;
using System.Collections.ObjectModel;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DRLMobile.Uwp.View
{
    public class StandardOptionsPrintHelper : ReceiptPrintHelper
    {
        public StandardOptionsPrintHelper(Page TestPrint) : base(TestPrint) { }

        /// <summary>
        /// This is the event handler for PrintManager.PrintTaskRequested.
        /// In order to ensure a good user experience, the system requires that the app handle the PrintTaskRequested event within the time specified by PrintTaskRequestedEventArgs.Request.Deadline.
        /// Therefore, we use this handler to only create the print task.
        /// The print settings customization can be done when the print document source is requested.
        /// </summary>
        /// <param name="sender">PrintManager</param>
        /// <param name="e">PrintTaskRequestedEventArgs</param>
        protected override void PrintTaskRequested(PrintManager sender, PrintTaskRequestedEventArgs e)
        {
            PrintTask printTask = null;
            printTask = e.Request.CreatePrintTask("Honey - Invoice Printing", sourceRequestedArgs =>
            {
                IList<string> displayedOptions = printTask.Options.DisplayedOptions;

                // Choose the printer options to be shown.
                // The order in which the options are appended determines the order in which they appear in the UI
                displayedOptions.Clear();
                displayedOptions.Add(Windows.Graphics.Printing.StandardPrintTaskOptions.Copies);
                displayedOptions.Add(Windows.Graphics.Printing.StandardPrintTaskOptions.Orientation);
                displayedOptions.Add(Windows.Graphics.Printing.StandardPrintTaskOptions.MediaSize);
                displayedOptions.Add(Windows.Graphics.Printing.StandardPrintTaskOptions.Collation);
                displayedOptions.Add(Windows.Graphics.Printing.StandardPrintTaskOptions.Duplex);

                // Preset the default value of the printer option
                printTask.Options.MediaSize = PrintMediaSize.Default;

                if (fromWhere == "RetailTransactionPage")
                {
                    // Print Task event handler is invoked when the print job is completed.
                    printTask.Completed += async (s, args1) =>
                    {

                        if (args1.Completion == PrintTaskCompletion.Failed)
                        {
                            await printPage.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                            {
                                if (DRLMobile.Uwp.Constants.Constants.NavigationFlagForRetailTransaction == true)
                                {
                                    await new MessageDialog("Your order is placed successfully.").ShowAsync();
                                    DRLMobile.Uwp.Services.NavigationService.NavigateShellFrame(typeof(DRLMobile.Uwp.View.DashboardPage));
                                }
                            });
                        }

                        if (args1.Completion == PrintTaskCompletion.Abandoned)
                        {
                            await printPage.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                            {

                                if (DRLMobile.Uwp.Constants.Constants.NavigationFlagForRetailTransaction == true)
                                {
                                    await new MessageDialog("Your order is placed successfully.").ShowAsync();
                                    DRLMobile.Uwp.Services.NavigationService.NavigateShellFrame(typeof(DRLMobile.Uwp.View.DashboardPage));
                                }
                            });
                        }

                        if (args1.Completion == PrintTaskCompletion.Canceled)
                        {
                            await printPage.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                            {
                                if (DRLMobile.Uwp.Constants.Constants.NavigationFlagForRetailTransaction == true)
                                {
                                    await new MessageDialog("Your order is placed successfully.").ShowAsync();
                                    DRLMobile.Uwp.Services.NavigationService.NavigateShellFrame(typeof(DRLMobile.Uwp.View.DashboardPage));
                                }

                            });
                        }

                        if (args1.Completion == PrintTaskCompletion.Submitted)
                        {
                            await printPage.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                            {
                                if (DRLMobile.Uwp.Constants.Constants.NavigationFlagForRetailTransaction == true)
                                {
                                    await new MessageDialog("Your order is placed successfully.").ShowAsync();
                                    DRLMobile.Uwp.Services.NavigationService.NavigateShellFrame(typeof(DRLMobile.Uwp.View.DashboardPage));
                                }

                            });
                        }

                    };
                }

                //DRLMobile.Uwp.Constants.Constants.NavigationFlagForRetailTransaction = false;
                sourceRequestedArgs.SetSource(printDocumentSource);
            });
        }
    }

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>

    public sealed partial class RetailTransactionPage : Page
    {
        private RetailTransactionPageViewModel retailTransactionPageViewModel = new RetailTransactionPageViewModel();
        private StandardOptionsPrintHelper printHelper;

        public RetailTransactionPage()
        {
            this.InitializeComponent();

            DataContext = retailTransactionPageViewModel;

        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            retailTransactionPageViewModel.RetailTransacUiModel.PrebookDateVisiblity = Visibility.Collapsed;

            prebookDatePicker.Date = DateTime.Now.Date;

            retailTransactionPageViewModel.RetailTransacUiModel.PreBookShipDate = prebookDatePicker.Date.ToString();

            DateTime datetime = Convert.ToDateTime(retailTransactionPageViewModel.RetailTransacUiModel.PreBookShipDate);

            retailTransactionPageViewModel.RetailTransacUiModel.PreBookShipDate = datetime.ToString("MM/dd/yyyy hh:mm:ss", CultureInfo.InvariantCulture);

            await retailTransactionPageViewModel.InitializeDataOnPageLoad();

            if (retailTransactionPageViewModel.RetailTransacUiModel.IsSampleOrder)
            {
                salesTypeLabel.Visibility = Visibility.Visible;
                sampleorderoption.IsChecked = true;
                retailTransactionPageViewModel.RetailTransacUiModel.DistributorVisibility = Visibility.Collapsed;
                retailTransactionPageViewModel.RetailTransacUiModel.ActivityType = "Sample Order";
                retailTransactionPageViewModel.RetailTransacUiModel.SelectedSalesType = "11";
            }
            else if (retailTransactionPageViewModel.RetailTransacUiModel.IsRackOrder)
            {
                salesTypeLabel.Visibility = Visibility.Visible;
                rackoption.IsChecked = true;
                retailTransactionPageViewModel.RetailTransacUiModel.DistributorVisibility = Visibility.Collapsed;
                retailTransactionPageViewModel.RetailTransacUiModel.ActivityType = "RackPOS";
                retailTransactionPageViewModel.RetailTransacUiModel.SelectedSalesType = "5";
            }
            else if (retailTransactionPageViewModel.RetailTransacUiModel.IsPopOrder)
            {
                retailTransactionPageViewModel.RetailTransacUiModel.DistributorVisibility = Visibility.Collapsed;
                salesTypeLabel.Visibility = Visibility.Collapsed;
                retailTransactionPageViewModel.RetailTransacUiModel.ActivityType = "POP";
                retailTransactionPageViewModel.RetailTransacUiModel.SelectedSalesType = "10";
            }
            else if (retailTransactionPageViewModel.RetailTransacUiModel.IsDirectCustomer)
            {
                if (retailTransactionPageViewModel.RetailTransacUiModel.IsCarStockOrder)
                {


                    retailTransactionPageViewModel.RetailTransacUiModel.PageTitle = "Car Stock Order";
                    retailTransactionPageViewModel.RetailTransacUiModel.SelectedSalesType = Convert.ToString((int)SalesType.CarStockOrder);
                    retailTransactionPageViewModel.RetailTransacUiModel.SaleTypeCollection = new ObservableCollection<DropDownUIModel>(
                    new List<DropDownUIModel> {
                                        new DropDownUIModel
                                            {
                                                Id = (int)(SalesType.CarStockOrder),
                                                Name = "Car Stock Order"
                                            } }
                                            );



                    retailTransactionPageViewModel.RetailTransacUiModel.ChosenSaleType = retailTransactionPageViewModel.RetailTransacUiModel.SaleTypeCollection.FirstOrDefault(i => i.Id == Convert.ToInt32(retailTransactionPageViewModel.RetailTransacUiModel.SelectedSalesType));
                    retailTransactionPageViewModel.RetailTransacUiModel.ActivityType = retailTransactionPageViewModel.RetailTransacUiModel.ChosenSaleType.Name;


                    salesTypeLabel.Visibility = Visibility.Visible;
                    retailTransactionPageViewModel.RetailTransacUiModel.OrderSalesOptionVisibility = Visibility.Visible;
                }
                else
                {
                    salesTypeLabel.Visibility = Visibility.Collapsed;
                    retailTransactionPageViewModel.RetailTransacUiModel.DistributorVisibility = Visibility.Collapsed;

                    if (retailTransactionPageViewModel.RetailTransacUiModel.IsCreditRequest)
                    {
                        retailTransactionPageViewModel.RetailTransacUiModel.ActivityType = "Credit Request";
                        retailTransactionPageViewModel.RetailTransacUiModel.SelectedSalesType = "8";
                    }
                    else
                    {
                        retailTransactionPageViewModel.RetailTransacUiModel.ActivityType = "Distributor";
                        retailTransactionPageViewModel.RetailTransacUiModel.SelectedSalesType = "6";
                    }
                }
            }
            else
            {
                retailTransactionPageViewModel.RetailTransacUiModel.SelectedSalesType = Convert.ToString((int)SalesType.CashSale);

                retailTransactionPageViewModel.RetailTransacUiModel.ChosenSaleType = retailTransactionPageViewModel.RetailTransacUiModel.SaleTypeCollection.FirstOrDefault(i => i.Id == Convert.ToInt32(retailTransactionPageViewModel.RetailTransacUiModel.SelectedSalesType));
                retailTransactionPageViewModel.RetailTransacUiModel.ActivityType = retailTransactionPageViewModel.RetailTransacUiModel.ChosenSaleType.Name;

                salesTypeLabel.Visibility = Visibility.Visible;
                retailTransactionPageViewModel.RetailTransacUiModel.OrderSalesOptionVisibility = Visibility.Visible;
            }

            // Initalize receipt print helper class and register for printing
            retailTransactionPageViewModel.PrintHelper = new StandardOptionsPrintHelper(this);
            retailTransactionPageViewModel.PrintHelper.RegisterForPrinting("RetailTransactionPage");
        }

        private void SalesTypeOption_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as RadioButtons).SelectedItem != null)
            {
                DropDownUIModel selectedItem = ((sender as RadioButtons).SelectedItem as DropDownUIModel);
                retailTransactionPageViewModel.SaleTypeCommand.Execute(selectedItem);
            }
        }

        private async void DistributorGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (retailTransactionPageViewModel.RetailTransacUiModel.DistributorsList == null ||
                retailTransactionPageViewModel.RetailTransacUiModel.DistributorsList?.Count == 0)
            {
                ContentDialog noDistributorAddedDialog = new ContentDialog
                {
                    Content = ResourceLoader.GetForCurrentView().GetString("NoDistributorAddedMessage"),
                    CloseButtonText = ResourceLoader.GetForCurrentView().GetString("OK")
                };

                await noDistributorAddedDialog.ShowAsync();
            }
            else
            {
                DistributorFlyout.ShowAt(DistributorGrid as FrameworkElement);
            }
        }

        private void DistributorTemplateGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            retailTransactionPageViewModel.DistributionSelectionCommand.Execute((sender as Grid).DataContext);

            selectedDistributor.Text = retailTransactionPageViewModel.RetailTransacUiModel.SelectedDistributorName;

            DistributorFlyout.Hide();
        }

        private void prebookDatePicker_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            retailTransactionPageViewModel.RetailTransacUiModel.PreBookShipDate = prebookDatePicker.Date?.ToString();

            DateTime datetime = Convert.ToDateTime(retailTransactionPageViewModel.RetailTransacUiModel.PreBookShipDate);

            retailTransactionPageViewModel.RetailTransacUiModel.PreBookShipDate = datetime.ToString("MM/dd/yyyy hh:mm:ss", CultureInfo.InvariantCulture);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DistributorFlyout.Hide();
        }

        private void SignatureButton_Click(object sender, RoutedEventArgs e)
        {
            retailTransactionPageViewModel.ShowSignaturePanel();
        }



        private void SignatureImageView_Tapped(object sender, TappedRoutedEventArgs e)
        {
            retailTransactionPageViewModel.ShowSignaturePanel();
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
            var dataSource = (OrderDetailUIModel)dataCxtx;
            retailTransactionPageViewModel.RetailTransacUiModel.OrderDetailModel = dataSource;
            retailTransactionPageViewModel.quantityBeforeEdit = dataSource.QuantityDisplay;
            FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
        }

        private void QuantityTextBlock_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            var senderName = (TextBox)sender;
            var dataCxtx = senderName.DataContext;
            var dataSource = (OrderDetailUIModel)dataCxtx;

            if (!string.IsNullOrEmpty(senderName.Text))
            {
                ///var quantityBefore = dataSource.Quantity;
                dataSource.Quantity = Convert.ToInt32(senderName.Text);
                retailTransactionPageViewModel?.QuantityChangedCommand.Execute(dataSource);
            }
        }

        private void QuantityCustomKeyPadFlyout_Closing(FlyoutBase sender, FlyoutBaseClosingEventArgs args)
        {
            //if (string.IsNullOrEmpty(retailTransactionPageViewModel?.RetailTransacUiModel.OrderDetailModel?.QuantityDisplay))
            //{
            //    retailTransactionPageViewModel.RetailTransacUiModel.OrderDetailModel.QuantityDisplay = retailTransactionPageViewModel?.quantityBeforeEdit;
            //    if (string.IsNullOrEmpty(retailTransactionPageViewModel?.RetailTransacUiModel.OrderDetailModel?.QuantityDisplay))
            //    {
            //        retailTransactionPageViewModel.RetailTransacUiModel.OrderDetailModel.Quantity = Convert.ToInt32(retailTransactionPageViewModel?.RetailTransacUiModel.OrderDetailModel?.QuantityDisplay);
            //    }
            //}

            if (string.IsNullOrEmpty(retailTransactionPageViewModel?.RetailTransacUiModel.OrderDetailModel?.QuantityDisplay))
            {
                retailTransactionPageViewModel.RetailTransacUiModel.OrderDetailModel.QuantityDisplay = retailTransactionPageViewModel?.quantityBeforeEdit;
            }

            retailTransactionPageViewModel.quantityString = string.Empty;

            retailTransactionPageViewModel?.QuantityChangedCommand.Execute(retailTransactionPageViewModel?.RetailTransacUiModel.OrderDetailModel);
        }

        private void CustomKeypad_DoneClickEvent1(object sender, bool e)
        {
            QuantityGridCustomKeyPadFlyout.Hide();
        }

        private void Delete_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var senderName = (Image)sender;
            var dataCxtx = senderName.DataContext;
            var dataSource = (OrderDetailUIModel)dataCxtx;
            retailTransactionPageViewModel?.DeleteImageCommand.Execute(dataSource);
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);

            if (retailTransactionPageViewModel.PrintHelper != null)
            {
                retailTransactionPageViewModel.PrintHelper.UnregisterForPrinting();
            }

            //if (printHelper != null)
            //{
            //    printHelper.UnregisterForPrinting();
            //}
        }

        private void AddTaxStatementIcon_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (retailTransactionPageViewModel.RetailTransacUiModel.UserTaxStatementList != null ||
               retailTransactionPageViewModel.RetailTransacUiModel.UserTaxStatementList?.Count > 0)
            {
                taxStatmentFlyout.ShowAt(AddTaxStatementIcon);
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var senderName = (TextBox)sender;

            if (!string.IsNullOrEmpty(senderName.Text))
            {
                retailTransactionPageViewModel?.AddCustomTaxStatementCommand.Execute(senderName.Text);
            }
        }

        private void CustomTaxtStatementTemplateGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            retailTransactionPageViewModel.SetCustomTaxStatementCommand.Execute((sender as Grid).DataContext);

            customTaxTextBox.Text = retailTransactionPageViewModel.RetailTransacUiModel.CustomTaxStatement;

            taxStatmentFlyout.Hide();
        }

        private void CancelTaxStatement_Click(object sender, RoutedEventArgs e)
        {
            taxStatmentFlyout.Hide();
        }

        private void QuantityEditText_GotFocus(object sender, RoutedEventArgs e)
        {
            var senderName = (TextEdit)sender;
            var dataCxtx = senderName.DataContext;
            var dataSource = (OrderDetailUIModel)dataCxtx;

            retailTransactionPageViewModel.RetailTransacUiModel.OrderDetailModel = dataSource;

            retailTransactionPageViewModel.quantityBeforeEdit = dataSource.QuantityDisplay;

            FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
        }

        private void QuantityEditText_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            var senderName = (TextEdit)sender;
            var dataCxtx = senderName.DataContext;
            var dataSource = (OrderDetailUIModel)dataCxtx;

            if (!string.IsNullOrEmpty(senderName.Text))
            {
                dataSource.Quantity = Convert.ToInt32(senderName.Text);

                retailTransactionPageViewModel?.QuantityChangedCommand.Execute(dataSource);
            }
        }
    }
}
