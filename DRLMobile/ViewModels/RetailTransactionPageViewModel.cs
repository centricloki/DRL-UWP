using DRLMobile.Core.Helpers;
using DRLMobile.Core.Models.DataModels;
using DRLMobile.Core.Models.UIModels;
using DRLMobile.CustomControls;
using DRLMobile.ExceptionHandler;
using DRLMobile.Services;
using DRLMobile.Views;
using Microsoft.Graphics.Canvas;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace DRLMobile.ViewModels
{
    public class RetailTransactionPageViewModel : ObservableObject
    {
        #region Properties

        private readonly ResourceLoader resourceLoader;
        private readonly App AppReference = (App)Application.Current;
        private readonly Random _random = new Random();
        private List<OrderDetailUIModel> DbOrderDetailDataSource;

        private RetailTransactionUIModel _retailTransactionUIModel;
        public RetailTransactionUIModel RetailTransacUiModel
        {
            get { return _retailTransactionUIModel; }
            set { SetProperty(ref _retailTransactionUIModel, value); }
        }

        private ObservableCollection<OrderDetailUIModel> _orderDetailGridDataSource;
        public ObservableCollection<OrderDetailUIModel> OrderDetailGridDataSource
        {
            get { return _orderDetailGridDataSource; }
            set { SetProperty(ref _orderDetailGridDataSource, value); }
        }

        private ObservableCollection<OrderDetailUIModel> _tobaccoGridDataSource;
        public ObservableCollection<OrderDetailUIModel> TobaccoGridDataSource
        {
            get { return _tobaccoGridDataSource; }
            set { SetProperty(ref _tobaccoGridDataSource, value); }
        }

        private ObservableCollection<OrderDetailUIModel> _nonTobaccoGridDataSource;
        public ObservableCollection<OrderDetailUIModel> NonTobaccoGridDataSource
        {
            get { return _nonTobaccoGridDataSource; }
            set { SetProperty(ref _nonTobaccoGridDataSource, value); }
        }

        private ObservableCollection<OrderDetailUIModel> _rtnGridDataSource;
        public ObservableCollection<OrderDetailUIModel> RtnGridDataSource
        {
            get { return _rtnGridDataSource; }
            set { SetProperty(ref _rtnGridDataSource, value); }
        }

        private ObservableCollection<OrderDetailUIModel> _difGridDataSource;
        public ObservableCollection<OrderDetailUIModel> DifGridDataSource
        {
            get { return _difGridDataSource; }
            set { SetProperty(ref _difGridDataSource, value); }
        }

        private string _uperGridSubTotal;
        public string UperGridSubTotal
        {
            get { return _uperGridSubTotal; }
            set { SetProperty(ref _uperGridSubTotal, value); }
        }

        private string _lowerGridSubTotal;
        public string LowerGridSubTotal
        {
            get { return _lowerGridSubTotal; }
            set { SetProperty(ref _lowerGridSubTotal, value); }
        }

        private string _grandTotal;
        public string GrandTotal
        {
            get { return _grandTotal; }
            set { SetProperty(ref _grandTotal, value); }
        }
        public DateTime prebookDateTime { get; set; }
        private ObservableCollection<OrderDetailUIModel> _rackOrderGridDataSource;
        public ObservableCollection<OrderDetailUIModel> RackOrderGridDataSource
        {
            get { return _rackOrderGridDataSource; }
            set { SetProperty(ref _rackOrderGridDataSource, value); }
        }
        private string _quantityBeforeEdit = string.Empty;
        public string quantityBeforeEdit
        {
            get { return _quantityBeforeEdit; }
            set { SetProperty(ref _quantityBeforeEdit, value); }
        }
        #endregion

        #region Command
        public ICommand ConfirmOrderCommand { get; private set; }
        public ICommand DistributionSelectionCommand { get; private set; }
        public ICommand DeleteImageCommand { private set; get; }
        public ICommand NumPadGridButtonClickCommand { get; private set; }
        public ICommand QuantityChangedCommand { get; private set; }


        #endregion

        #region Constructor

        public RetailTransactionPageViewModel()
        {
            RetailTransacUiModel = new RetailTransactionUIModel();
            DbOrderDetailDataSource = new List<OrderDetailUIModel>();
            OrderDetailGridDataSource = new ObservableCollection<OrderDetailUIModel>();
            TobaccoGridDataSource = new ObservableCollection<OrderDetailUIModel>();
            NonTobaccoGridDataSource = new ObservableCollection<OrderDetailUIModel>();
            RtnGridDataSource = new ObservableCollection<OrderDetailUIModel>();
            DifGridDataSource = new ObservableCollection<OrderDetailUIModel>();

            resourceLoader = ResourceLoader.GetForCurrentView();

            ConfirmOrderCommand = new AsyncRelayCommand(ConfirmOrderButtonClickCommand);
            DistributionSelectionCommand = new RelayCommand<DistributorMaster>(DistributionSelectionCommandHandler);
            RackOrderGridDataSource = new ObservableCollection<OrderDetailUIModel>();
            DeleteImageCommand = new AsyncRelayCommand<OrderDetailUIModel>(DeleteImageClickedAsync);
            NumPadGridButtonClickCommand = new RelayCommand<string>(NumPadGridButtonClicked);
            QuantityChangedCommand = new AsyncRelayCommand<OrderDetailUIModel>(QuantityChangedAsync);
        }

        #endregion

        public async Task InitializeDataOnPageLoad()
        {
            try
            {
                int customerId = Convert.ToInt32(AppReference.SelectedCustomerId);

                RetailTransacUiModel = await AppReference.QueryService.GetRetailTransactionData(customerId, AppReference.LoginUserNameProperty, AppReference.LoginUserPinProperty);

                RetailTransacUiModel.OrderId = AppReference.CurrentOrderId;
                RetailTransacUiModel.CustomerId = (int)RetailTransacUiModel.CustomerData.CustomerID;
                
               

                if (AppReference.CartDataFromScreen == 0)
                {
                    RetailTransacUiModel.SetPageTitle();
                    RetailTransacUiModel.RackOrderGridVisibility = Visibility.Collapsed;
                    RetailTransacUiModel.CommentsLabel = "Enter Retail Sales Call Notes :";
                    DbOrderDetailDataSource = await ((App)Application.Current).QueryService.GetCartProductDetailsData(AppReference.CurrentOrderId);
                    RetailTransacUiModel.OrderDetailsData = DbOrderDetailDataSource;
                    if (DbOrderDetailDataSource?.Count > 0)
                    {

                        if (RetailTransacUiModel.IsCreditRequest)
                        {
                            RetailTransacUiModel.SelectedSalesType = "8";

                            SetRtnDifGridView();
                        }
                        else
                        {
                            SetTobaccoNonTobaccoGrid();
                        }
                    }
                }
                else
                {
                    RetailTransacUiModel.PageTitle = "Rack Order";
                    RetailTransacUiModel.GrandTotalVisibility = Visibility.Collapsed;
                    RetailTransacUiModel.SelectedSalesType = "5";
                    RetailTransacUiModel.IsRackOrder = true;
                    RetailTransacUiModel.CommentsLabel = "Enter Comment :";
                    //GetRackCartProductsData
                    DbOrderDetailDataSource = await ((App)Application.Current).QueryService.GetRackCartProductsData(AppReference.CurrentOrderId);
                    RetailTransacUiModel.OrderDetailsData = DbOrderDetailDataSource;
                    SetRackOrderGrid();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(GetType().Name, "InitializeDataOnPageLoad", ex.StackTrace);
            }
        }

        private void SetTobaccoNonTobaccoGrid()
        {
            int uperGridTotal = 0;
            int lowerGridTotal = 0;
            int grandTotal = 0;

            RetailTransacUiModel.GrandTotalVisibility = Visibility.Visible;

            List<OrderDetailUIModel> orderList = DbOrderDetailDataSource.ToList();
            orderList?.ForEach(x => OrderDetailGridDataSource.Add(x));

            var tobaccoList = orderList.Where(x => x.isTobbaco == 1).ToList();
            tobaccoList.ForEach(x => TobaccoGridDataSource.Add(x));

            foreach (var item in TobaccoGridDataSource)
            {
                int total = item.Price * item.Quantity;

                uperGridTotal += total;
            }

            UperGridSubTotal = FormatStringToTwoDecimal(uperGridTotal);

            RetailTransacUiModel.TobaccoGridVisibility = (tobaccoList.Count > 0) ? Visibility.Visible : Visibility.Collapsed;

            var nonTobaccoList = orderList.Where(x => x.isTobbaco == 0).ToList();
            nonTobaccoList.ForEach(x => NonTobaccoGridDataSource.Add(x));

            foreach (var item in NonTobaccoGridDataSource)
            {
                int total = item.Price * item.Quantity;

                lowerGridTotal += total;
            }

            LowerGridSubTotal = FormatStringToTwoDecimal(lowerGridTotal);

            RetailTransacUiModel.NonTobaccoGridVisibility = (nonTobaccoList.Count > 0) ? Visibility.Visible : Visibility.Collapsed;

            grandTotal = uperGridTotal + lowerGridTotal;

            RetailTransacUiModel.GrandTotal = Convert.ToString(grandTotal);
            GrandTotal = FormatStringToTwoDecimal(grandTotal);
        }

        private void SetRtnDifGridView()
        {
            int uperGridTotal = 0;
            int lowerGridTotal = 0;
            int grandTotal = 0;

            RetailTransacUiModel.GrandTotalVisibility = Visibility.Visible;
            List<OrderDetailUIModel> orderList = DbOrderDetailDataSource.ToList();
            orderList?.ForEach(x => OrderDetailGridDataSource.Add(x));

            var rtnList = orderList.Where(x => x.CreditRequest.Contains("RTN")).ToList();
            rtnList.ForEach(x => RtnGridDataSource.Add(x));

            foreach (var item in RtnGridDataSource)
            {
                int total = item.Price * item.Quantity;

                uperGridTotal += total;
            }

            UperGridSubTotal = FormatStringToTwoDecimal(uperGridTotal);

            RetailTransacUiModel.RtnGridVisibility = (rtnList.Count > 0) ? Visibility.Visible : Visibility.Collapsed;

            var difList = orderList.Where(x => x.CreditRequest.Contains("DIF")).ToList();
            difList.ForEach(x => DifGridDataSource.Add(x));

            foreach (var item in DifGridDataSource)
            {
                int total = item.Price * item.Quantity;

                lowerGridTotal += total;
            }

            LowerGridSubTotal = FormatStringToTwoDecimal(lowerGridTotal);

            RetailTransacUiModel.DifGridVisibility = (difList.Count > 0) ? Visibility.Visible : Visibility.Collapsed;

            grandTotal = uperGridTotal + lowerGridTotal;

            RetailTransacUiModel.GrandTotal = Convert.ToString(grandTotal);
            GrandTotal = FormatStringToTwoDecimal(grandTotal);
        }

        private string RandomString(int size, bool lowerCase = false)
        {
            var builder = new StringBuilder(size);

            // char is a single Unicode character  
            char offset = lowerCase ? 'a' : 'A';
            const int lettersOffset = 26; // A...Z or a..z: length=26  

            for (var i = 0; i < size; i++)
            {
                var @char = (char)_random.Next(offset, offset + lettersOffset);
                builder.Append(@char);
            }

            return lowerCase ? builder.ToString().ToLower() : builder.ToString();
        }

        private int RandomNumber(int min, int max)
        {
            return _random.Next(min, max);
        }

        private string RandomInvoiceNumber()
        {
            var randomInvoiceNumber = new StringBuilder();

            // 4-Digits between 1000 and 9999  
            randomInvoiceNumber.Append(RandomNumber(1000, 9999));

            // 4-Letters upper case  
            randomInvoiceNumber.Append(RandomString(4));

            return randomInvoiceNumber.ToString();
        }

        private async Task ConfirmOrderButtonClickCommand()
        {
            bool isCustomerUpdated = false;

            try
            {
                //if (!string.IsNullOrEmpty(RetailTransacUiModel.PrintName) && !string.IsNullOrEmpty(RetailTransacUiModel.CustomerSignatureFileName))
                //{
                if (RackOrderGridDataSource?.Count <= 0 && AppReference.CartDataFromScreen == 1)
                {
 
                    await ShowPleaseAddRackPopUp();
                }
                else
                { 
                RetailTransacUiModel.OrderDate = Core.Helpers.DateTimeHelper.ConvertToDbInsertDateTimeFormat(DateTime.Now);

                if (string.IsNullOrEmpty(RetailTransacUiModel.PreBookShipDate))
                {
                    RetailTransacUiModel.PreBookShipDate = Core.Helpers.DateTimeHelper.ConvertToDbInsertDateTimeFormat(DateTime.Now);
                }

                if (RetailTransacUiModel.CustomerData != null && (!RetailTransacUiModel.CustomerData.CustomerName.Equals(RetailTransacUiModel.CustomerName) ||
                    (RetailTransacUiModel.CustomerData.RetailerLicense != null && !RetailTransacUiModel.CustomerData.RetailerLicense.Equals(RetailTransacUiModel.RetailerLicense)) ||
                    (RetailTransacUiModel.CustomerData.StateTobaccoLicense != null && !RetailTransacUiModel.CustomerData.StateTobaccoLicense.Equals(RetailTransacUiModel.StateTobaccoLicense)) ||
                    (RetailTransacUiModel.CustomerData.RetailerSalesTaxCertificate != null && !RetailTransacUiModel.CustomerData.RetailerSalesTaxCertificate.Equals(RetailTransacUiModel.RetailerSalesTaxCertificate))))
                {
                    RetailTransacUiModel.CustomerData.CustomerName = RetailTransacUiModel.CustomerName;
                    RetailTransacUiModel.CustomerData.RetailerLicense = RetailTransacUiModel.RetailerLicense;
                    RetailTransacUiModel.CustomerData.StateTobaccoLicense = RetailTransacUiModel.StateTobaccoLicense;
                    RetailTransacUiModel.CustomerData.RetailerSalesTaxCertificate = RetailTransacUiModel.RetailerSalesTaxCertificate;
                    RetailTransacUiModel.CustomerData.IsExported = 0;
                    RetailTransacUiModel.CustomerData.UpdatedDate = Core.Helpers.DateTimeHelper.ConvertToDbInsertDateTimeFormat(DateTime.Now);
                    RetailTransacUiModel.CustomerData.UpdatedBy = Convert.ToString(RetailTransacUiModel.LoggedInUserId);

                    isCustomerUpdated = true;
                }

                CustomContentDialog customContentDialog = new CustomContentDialog();
                customContentDialog.Title = "Please select your invoice method below. Once you have selected, your order will be confirmed.";
                customContentDialog.FirstButtonText = "Print Invoice";
                customContentDialog.SecondButtonText = "Email Invoice";
                customContentDialog.CancelButtonText = "Go Back - Do Not Confirm Order";

                await customContentDialog.ShowAsync();

                if (customContentDialog.Result == Result.Yes)
                {
                    var tempInvoice = RandomInvoiceNumber();

                    RetailTransacUiModel.InvoiceNumber = string.IsNullOrEmpty(tempInvoice) ? "" : tempInvoice;

                    var orderPlace = await AppReference.QueryService.InsertOrUpdateDataOnConfirmOrder(RetailTransacUiModel, isCustomerUpdated);

                    if (orderPlace)
                    {
                        ///await AppReference.QueryService.DeleteCartDetailOnPlaceOrder(AppReference.CurrentOrderId);
                        AppReference.CartItemCount = 0;
                        AppReference.CurrentOrderId = 0;
                        AppReference.IsCreditRequestOrder = false;
                        AppReference.IsOrderTypeChanged = false;

                        NavigationService.Navigate<DashboardPage>();
                    }
                }
                else if (customContentDialog.Result == Result.No)
                {
                    await AppReference.QueryService.InsertOrUpdateDataOnConfirmOrder(RetailTransacUiModel, isCustomerUpdated);

                    ///await AppReference.QueryService.DeleteCartDetailOnPlaceOrder(AppReference.CurrentOrderId);
                    AppReference.CartItemCount = 0;
                    AppReference.CurrentOrderId = 0;
                    AppReference.IsCreditRequestOrder = false;
                    AppReference.IsOrderTypeChanged = false;

                    NavigationService.Navigate<DashboardPage>();
                }
                //}
                //else
                //{
                //    ShowEmptyNameSignatureMessage();
                //}
            }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(GetType().Name, "ConfirmOrderButtonClickCommand", ex.StackTrace);
            }
        }

        private void DistributionSelectionCommandHandler(DistributorMaster obj)
        {
            try
            {
                var value = RetailTransacUiModel.DistributorsList?.Any(x => x.CustomerID == obj.CustomerID);

                if (value.HasValue && value.Value && obj != null)
                {
                    var isSelected = !obj.IsSelected;

                    (RetailTransacUiModel.DistributorsList?.FirstOrDefault(x => x.CustomerID == obj.CustomerID)).IsSelected = isSelected;

                    var selectedDistributor = RetailTransacUiModel.PrebookDistributorList?.FirstOrDefault(x => x.CustomerID == obj.CustomerID);

                    if (selectedDistributor != null)
                    {
                        selectedDistributor.IsSelected = isSelected;

                        var selectedStateId = selectedDistributor.PhysicalAddressStateID;

                        RetailTransacUiModel.PrebookState.TryGetValue(selectedStateId, out string physicalStateValue);
                        RetailTransacUiModel.SelectedDistributorName = selectedDistributor.CustomerName;
                        RetailTransacUiModel.SelectedPrebookState = physicalStateValue ?? string.Empty;
                        RetailTransacUiModel.PrebookCity = selectedDistributor.PhysicalAddressCityID;
                        RetailTransacUiModel.PrebookZip = selectedDistributor.PhysicalAddressZipCode;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(GetType().Name, "DistributionSelectionCommandHandler", ex.StackTrace);
            }
        }

        private string FormatStringToTwoDecimal(int subTotalUpperGrid)
        {
            var formattedString = "$" + string.Format("{0:00.00}", subTotalUpperGrid);

            return formattedString;
        }

        private byte[] ConvertSingatureCanvasToByteArray(InkCanvas canvas)
        {
            var canvasStroke = canvas.InkPresenter.StrokeContainer.GetStrokes();

            if (canvasStroke.Count > 0)
            {
                var width = (int)canvas.ActualWidth;
                var height = (int)canvas.ActualHeight;
                var device = CanvasDevice.GetSharedDevice();

                var renderTarget = new CanvasRenderTarget(device, width, height, 96);

                using (var ds = renderTarget.CreateDrawingSession())
                {
                    ds.Clear(Windows.UI.Colors.White);
                    ds.DrawInk(canvas.InkPresenter.StrokeContainer.GetStrokes());
                }

                return renderTarget.GetPixelBytes();
            }

            return null;
        }

        public async Task<WriteableBitmap> ConvertInkCanvasToWriteableBitmap(InkCanvas inkCanvas)
        {
            var bytes = ConvertSingatureCanvasToByteArray(inkCanvas);

            if (bytes != null)
            {
                var width = (int)inkCanvas.ActualWidth;
                var height = (int)inkCanvas.ActualHeight;

                var bmp = new WriteableBitmap(width, height);

                using (var stream = bmp.PixelBuffer.AsStream())
                {
                    await stream.WriteAsync(bytes, 0, bytes.Length);

                    return bmp;
                }
            }

            return null;
        }

        public void SaveSignature(WriteableBitmap signature)
        {
            if (signature != null)
            {
                //signature.
            }
        }

        public async void ShowEmptyNameSignatureMessage()
        {
            ContentDialog emptyFieldDialog = new ContentDialog
            {
                Title = "Confirm Order Error",
                Content = "Please enter both name and signature to confirm the order.",
                CloseButtonText = resourceLoader.GetString("OK")
            };

            await emptyFieldDialog.ShowAsync();
        }

        public void ClearEmailToField()
        {
            if (!string.IsNullOrEmpty(RetailTransacUiModel.EmailTo) && RetailTransacUiModel.EmailTo.Equals(RetailTransacUiModel.UserEmailId))
            {
                RetailTransacUiModel.EmailTo = string.Empty;
            }
        }

        private void SetRackOrderGrid()
        {
            List<OrderDetailUIModel> orderList = DbOrderDetailDataSource.ToList();
            orderList?.ForEach(x => RackOrderGridDataSource.Add(x));

            foreach (var item in RackOrderGridDataSource)
            {
                item.QuantityDisplay = item.Quantity.ToString();
            }
            RetailTransacUiModel.RackOrderGridVisibility = (RackOrderGridDataSource.Count > 0) ? Visibility.Visible : Visibility.Collapsed;
        }
        /// <summary>
        /// Delete rack cart item
        /// </summary>
        /// <param name="orderDetailUIModel"></param>
        private async Task DeleteImageClickedAsync(OrderDetailUIModel orderDetailUIModel)
        {
            await ShowDeleteWarning(orderDetailUIModel);
        }

        /// <summary>
        /// Warning pop-up for deleting rack cart item
        /// </summary>
        /// <param name="orderDetailUIModel"></param>
        private async Task ShowDeleteWarning(OrderDetailUIModel orderDetailUIModel)
        {

            ContentDialog deleteCartItemDialog = new ContentDialog
            {
                Content = resourceLoader.GetString("RemoveRackOrComponent"),
                PrimaryButtonText = resourceLoader.GetString("YesCaps"),
                SecondaryButtonText = resourceLoader.GetString("NoCaps")
            };

            ContentDialogResult result = await deleteCartItemDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                await DeleteCartItemAsync(orderDetailUIModel);
            }
            else
            {
                deleteCartItemDialog.Hide();
            }
        }

        /// <summary>
        /// Bl for deleting cart item.
        /// </summary>
        /// <param name="orderDetailUIModel"></param>
        private async Task DeleteCartItemAsync(OrderDetailUIModel orderDetailUIModel)
        {
            int CartValue = AppReference.CartItemCount;

            CartValue--;

            ((App)Application.Current).CartItemCount = CartValue;

            await DeleteItemFromDb(orderDetailUIModel);
            RackOrderGridDataSource.Remove(orderDetailUIModel);
            RetailTransacUiModel.RackOrderGridVisibility = (RackOrderGridDataSource.Count > 0) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Delete item from database
        /// </summary>
        /// <param name="orderDetailUIModel"></param>
        private async Task DeleteItemFromDb(OrderDetailUIModel orderDetailUIModel)
        {
            orderDetailUIModel.OrderDetailUiToDataModel();

            var _list = new List<OrderDetail>() { orderDetailUIModel.OrderDetailData };

            await ((App)Application.Current).QueryService.DeleteOrderDetail(_list[0].ProductId, AppReference.CurrentOrderId);
        }

        private void NumPadGridButtonClicked(string value)
        {
            UpdateQuantityFieldValue(RackOrderGridDataSource, value);
        }
        private void UpdateQuantityFieldValue(ObservableCollection<OrderDetailUIModel> gridDataSource, string value)
        {
            foreach (var item in gridDataSource)
            {
                if (item.ProductID == RetailTransacUiModel.OrderDetailModel?.ProductID)
                {
                    if (value.Equals("C"))
                    {
                        if (item?.QuantityDisplay?.Length > 0)
                        {
                            item.QuantityDisplay = item?.QuantityDisplay?.Remove(item.QuantityDisplay.Length - 1);
                            item.IsQuantityEdited = true;
                        }
                    }
                    else
                    {
                        if (item?.QuantityDisplay?.Length == 0)
                        {
                            if (value != "0" && value != "-")
                            {
                                item.QuantityDisplay = RetailTransacUiModel.OrderDetailModel?.QuantityDisplay + value;
                            }
                            else
                            {
                                item.QuantityDisplay = string.Empty;
                                item.IsQuantityEdited = true;
                            }
                        }
                        else if (item.QuantityDisplay.Length < 4 && (value != "-"))
                        {
                            item.QuantityDisplay = RetailTransacUiModel.OrderDetailModel?.QuantityDisplay + value;
                        }
                    }
                    if (!string.IsNullOrEmpty(item?.QuantityDisplay))
                    {
                        item.Quantity = Convert.ToInt32(item?.QuantityDisplay);
                    }
                }
            }
        }

        private async Task QuantityChangedAsync(OrderDetailUIModel orderDetailUIModel)
        {
            foreach (var item in RackOrderGridDataSource)
            {
                if (item.ProductID == orderDetailUIModel.ProductID)
                {
                    item.Quantity = orderDetailUIModel.Quantity;

                    await SaveValuesInDatabase(item);

                    break;
                }
            }
        }

        private async Task SaveValuesInDatabase(OrderDetailUIModel orderDetailUIModel)
        {
            orderDetailUIModel.OrderDetailUiToDataModel();
            var _list = new List<OrderDetail>() { orderDetailUIModel.OrderDetailData };
            _list[0].UpdatedDate = DateTimeHelper.ConvertToDbInsertDateTimeFormat(DateTime.Now);
            await ((App)Application.Current).QueryService.InsertOrUpdateOrderDetail(_list[0]);
        }

        private async Task ShowPleaseAddRackPopUp()
        {

            ContentDialog pleaseAddRackDialog = new ContentDialog
            {
                Content = resourceLoader.GetString("PleaseAddRack"),
                CloseButtonText = resourceLoader.GetString("OK")
            };

            await pleaseAddRackDialog.ShowAsync();
        }
    }
}
