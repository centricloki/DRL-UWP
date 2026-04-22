using DevExpress.Mvvm.Native;

using DRLMobile.Core.Helpers;
using DRLMobile.Core.Models;
using DRLMobile.Core.Models.DataModels;
using DRLMobile.Core.Models.UIModels;
using DRLMobile.ExceptionHandler;
using DRLMobile.Uwp.Helpers;
using DRLMobile.Uwp.Services;
using DRLMobile.Uwp.View;

using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Uwp.UI.Controls;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using static AutoMapper.Internal.ExpressionFactory;

namespace DRLMobile.Uwp.ViewModel
{
    public class CartPageViewModel : BaseModel
    {
        #region Properties

        private readonly ResourceLoader resourceLoader;
        private readonly App AppRef = (App)Application.Current;

        public List<OrderDetailUIModel> DBOrderDetailsList { get; set; }

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

        private CartDetailsUIModel _cartDetailModel;
        public CartDetailsUIModel CartDetailModel
        {
            get { return _cartDetailModel; }
            set { SetProperty(ref _cartDetailModel, value); }
        }

        private string _purchaseOrderButtonText;
        public string PurchaseOrderButtonText
        {
            get { return _purchaseOrderButtonText; }
            set { SetProperty(ref _purchaseOrderButtonText, value); }
        }

        private bool _isTobaccoGrids;
        public bool LoadTobaccoGrids
        {
            get { return _isTobaccoGrids; }
            set { SetProperty(ref _isTobaccoGrids, value); }
        }

        private bool _isCreditRequestGrids;
        public bool LoadCreditRequestGrids
        {
            get { return _isCreditRequestGrids; }
            set { SetProperty(ref _isCreditRequestGrids, value); }
        }

        private string _isOpenedFrom = string.Empty;
        public string isOpenedFrom
        {
            get { return _isOpenedFrom; }
            set { SetProperty(ref _isOpenedFrom, value); }
        }

        private string _isOpenedForCRQuanity = string.Empty;
        public string IsOpenedForCRQuantity
        {
            get { return _isOpenedForCRQuanity; }
            set { SetProperty(ref _isOpenedForCRQuanity, value); }
        }

        private string _isOpenedForCRUnitPrice = string.Empty;
        public string IsOpenedForCRUnitPrice
        {
            get { return _isOpenedForCRUnitPrice; }
            set { SetProperty(ref _isOpenedForCRUnitPrice, value); }
        }

        private OrderDetailUIModel _orderDetailNumPadModel;
        public OrderDetailUIModel OrderDetailNumPadModel
        {
            get { return _orderDetailNumPadModel; }
            set { SetProperty(ref _orderDetailNumPadModel, value); }
        }

        private string _quantityString = string.Empty;
        public string quantityString
        {
            get { return _quantityString; }
            set { SetProperty(ref _quantityString, value); }
        }

        private string _quantityBeforeEdit = string.Empty;
        public string quantityBeforeEdit
        {
            get { return _quantityBeforeEdit; }
            set { SetProperty(ref _quantityBeforeEdit, value); }
        }

        private string _priceBeforeEdit = string.Empty;
        public string priceBeforeEdit
        {
            get { return _priceBeforeEdit; }
            set { SetProperty(ref _priceBeforeEdit, value); }
        }

        private string _priceString = string.Empty;
        public string priceString
        {
            get { return _priceString; }
            set { SetProperty(ref _priceString, value); }
        }

        public bool isTriggerConfirmOrder = false;
        #endregion

        #region Commands
        public ICommand OnNavigatedTo { private set; get; }
        public ICommand PlaceAnOderButtonCommand { get; private set; }
        public ICommand DeleteImageCommand { private set; get; }
        public ICommand IsAllUpperGridSelectedCommand { private set; get; }
        public ICommand IsAllUpperGridUnselectedCommand { private set; get; }
        public ICommand IsAllLowerGridSelectedCommand { private set; get; }
        public ICommand IsAllLowerGridUnselectedCommand { private set; get; }
        public ICommand EmailFactSheetCommad { get; private set; }
        public ICommand QuantityChangedCommand { get; private set; }
        public ICommand PriceChangedCommand { get; private set; }
        public ICommand UnitComboboxChangedCommand { get; private set; }
        public ICommand NumPadButtonClickCommand { get; private set; }
        public ICommand CreditRequestComboboxChangedCommand { get; private set; }

        #endregion

        #region Constructor

        public CartPageViewModel()
        {
            resourceLoader = ResourceLoader.GetForCurrentView();

            CartDetailModel = new CartDetailsUIModel();

            CartDetailModel.IsLoading = false;

            InitializeCommands();
        }

        #endregion

        #region Private Methods

        public async Task LoadInitialPageData()
        {
            ShellPage shellPage = ((Window.Current.Content as Frame).Content as ShellPage);
            if (shellPage != null)
            {
                shellPage.ViewModel.IsSideMenuItemClickable = false;
            }
            CartDetailModel.IsLoading = true;
            try
            {
                if (string.IsNullOrEmpty(AppRef.SelectedCustomerId))
                {
                    CartDetailModel.CustomerNameNumber = string.Empty;
                    CartDetailModel.ShowNoCustomerSelected();
                }
                else
                {
                    if (AppRef.CurrentOrderId != 0)
                    {
                        await SetCustomerInformationAndGetCartData();
                    }
                    else
                    {
                        CartDetailModel.ShowCartIsEmpty();
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(CartPageViewModel), nameof(LoadInitialPageData), ex);
            }
            finally
            {
                CartDetailModel.IsLoading = false;
                if (shellPage != null)
                {
                    shellPage.ViewModel.IsSideMenuItemClickable = true;
                }
            }
        }

        private void InitializeCommands()
        {
            DeleteImageCommand = new AsyncRelayCommand<OrderDetailUIModel>(DeleteImageClickedAsync);
            QuantityChangedCommand = new RelayCommand<OrderDetailUIModel>(QuantityChanged);
            PriceChangedCommand = new RelayCommand<OrderDetailUIModel>(PriceChanged);
            UnitComboboxChangedCommand = new RelayCommand<OrderDetailUIModel>(UnitComboboxChanged);
            CreditRequestComboboxChangedCommand = new RelayCommand<OrderDetailUIModel>(CreditRequestComboboxChanged);
            IsAllUpperGridSelectedCommand = new RelayCommand(IsAllUpperGridRowsSelected);
            IsAllUpperGridUnselectedCommand = new RelayCommand(IsAllUpperGridRowsUnselected);
            IsAllLowerGridSelectedCommand = new RelayCommand(IsAllLowerGridRowsSelected);
            IsAllLowerGridUnselectedCommand = new RelayCommand(IsAllLowerGridRowsUnselected);
            EmailFactSheetCommad = new AsyncRelayCommand(EmailFactSheetCommadHandler);
            PlaceAnOderButtonCommand = new AsyncRelayCommand(NavigateToRetailTransactionAsync);
            NumPadButtonClickCommand = new RelayCommand<string>(NumPadButtonClicked);
        }

        private void UnitComboboxChanged(OrderDetailUIModel orderDetailUIModel)
        {
            if ((bool)AppRef.IsCreditRequestOrder)
            {
                if (orderDetailUIModel.CreditRequest.Contains("RTN"))
                {
                    if (RtnGridDataSource?.Count > 0)
                    {
                        foreach (var item in RtnGridDataSource)
                        {
                            if (item.ProductID == orderDetailUIModel.ProductID)
                            {
                                item.Unit = orderDetailUIModel.Unit;

                                //await SaveValuesInDatabase(item);

                                break;
                            }
                        }
                    }
                }
                else
                {
                    if (DifGridDataSource?.Count > 0)
                    {
                        foreach (var item in DifGridDataSource)
                        {
                            if (item.ProductID == orderDetailUIModel.ProductID)
                            {
                                item.Unit = orderDetailUIModel.Unit;

                                //await SaveValuesInDatabase(item);

                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                if (orderDetailUIModel.isTobbaco == 1)
                {
                    if (TobaccoGridDataSource?.Count > 0)
                    {
                        foreach (var item in TobaccoGridDataSource)
                        {
                            if (item.ProductID == orderDetailUIModel.ProductID)
                            {
                                item.Unit = orderDetailUIModel.Unit;

                                //await SaveValuesInDatabase(item);

                                break;
                            }
                        }
                    }
                }
                else
                {
                    foreach (var item in NonTobaccoGridDataSource)
                    {
                        if (item.ProductID == orderDetailUIModel.ProductID)
                        {
                            item.Unit = orderDetailUIModel.Unit;

                            //await SaveValuesInDatabase(item);

                            break;
                        }
                    }
                }
            }
        }

        private void CreditRequestComboboxChanged(OrderDetailUIModel orderDetailUIModel)
        {
            if (orderDetailUIModel.CreditRequest.Contains("DIF"))
            {
                if (RtnGridDataSource != null && RtnGridDataSource?.Count > 0)
                {
                    var difItem = RtnGridDataSource.FirstOrDefault(x => x.ProductID == orderDetailUIModel.ProductID);

                    if (difItem != null)
                    {
                        RtnGridDataSource.Remove(difItem);
                        if (DifGridDataSource == null) DifGridDataSource = new ObservableCollection<OrderDetailUIModel>();
                        DifGridDataSource.Add(difItem);
                    }
                }
                //await SaveValuesInDatabase(orderDetailUIModel);
            }

            if (orderDetailUIModel.CreditRequest.Contains("RTN"))
            {
                if (DifGridDataSource != null && DifGridDataSource?.Count > 0)
                {
                    var rtnItem = DifGridDataSource.FirstOrDefault(x => x.ProductID == orderDetailUIModel.ProductID);

                    if (rtnItem != null)
                    {
                        DifGridDataSource.Remove(rtnItem);
                        if (RtnGridDataSource == null) RtnGridDataSource = new ObservableCollection<OrderDetailUIModel>();
                        RtnGridDataSource.Add(rtnItem);
                    }
                }
                //await SaveValuesInDatabase(orderDetailUIModel);
            }

            CalculateSubTotalGrandTotal();

            CartDetailModel.RtnGridVisibility = (RtnGridDataSource?.Count > 0) ? Visibility.Visible : Visibility.Collapsed;

            CartDetailModel.DifGridVisibility = (DifGridDataSource?.Count > 0) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void QuantityChanged(OrderDetailUIModel orderDetailUIModel)
        {
            if ((bool)AppRef.IsCreditRequestOrder)
            {
                if (orderDetailUIModel.CreditRequest.Contains("RTN"))
                {
                    if (RtnGridDataSource.Count > 0)
                    {
                        foreach (var item in RtnGridDataSource)
                        {
                            if (item.ProductID == orderDetailUIModel.ProductID)
                            {
                                item.Quantity = orderDetailUIModel.Quantity > 0 ? orderDetailUIModel.Quantity * -1 : orderDetailUIModel.Quantity;
                                orderDetailUIModel.QuantityDisplay = item.Quantity.ToString();
                                break;
                            }
                        }
                    }
                }
                else
                {
                    if (DifGridDataSource?.Count > 0)
                    {
                        foreach (var item in DifGridDataSource)
                        {
                            if (item.ProductID == orderDetailUIModel.ProductID)
                            {
                                item.Quantity = orderDetailUIModel.Quantity > 0 ? orderDetailUIModel.Quantity * -1 : orderDetailUIModel.Quantity;
                                orderDetailUIModel.QuantityDisplay = item.Quantity.ToString();
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                if (orderDetailUIModel.isTobbaco == 1)
                {
                    foreach (var item in TobaccoGridDataSource)
                    {
                        if (item.ProductID == orderDetailUIModel.ProductID)
                        {
                            item.Quantity = orderDetailUIModel.Quantity;
                            break;
                        }
                    }
                }
                else
                {
                    foreach (var item in NonTobaccoGridDataSource)
                    {
                        if (item.ProductID == orderDetailUIModel.ProductID)
                        {
                            item.Quantity = orderDetailUIModel.Quantity;
                            break;
                        }
                    }
                }
            }
            CalculateSubTotalGrandTotal();
        }

        private async Task SaveValuesInDatabase(OrderDetailUIModel orderDetailUIModel)
        {
            orderDetailUIModel.OrderDetailUiToDataModel();

            var _list = new List<OrderDetail>() { orderDetailUIModel.OrderDetailData };

            if ((bool)AppRef.IsCreditRequestOrder)
            {
                if (_list[0].Quantity == 0)
                {
                    _list[0].Quantity = -1;
                }
                else if (_list[0].Quantity > 0)
                {
                    _list[0].Quantity *= -1;
                }
            }

            _list[0].UpdatedDate = DateTimeHelper.ConvertToDbInsertDateTimeFormat(DateTime.Now);

            await ((App)Application.Current).QueryService.InsertOrUpdateOrderDetail(_list[0]);
        }

        public async Task SaveCartItems()
        {
            if (CartDetailModel.TobaccoGridVisibility == Visibility.Visible)
            {
                await SaveOrderListInDatabase(TobaccoGridDataSource);
            }
            if (CartDetailModel.NonTobaccoGridVisibility == Visibility.Visible)
            {
                await SaveOrderListInDatabase(NonTobaccoGridDataSource);
            }
            if (CartDetailModel.DifGridVisibility == Visibility.Visible)
            {
                await SaveOrderListInDatabase(DifGridDataSource);
            }
            if (CartDetailModel.RtnGridVisibility == Visibility.Visible)
            {
                await SaveOrderListInDatabase(RtnGridDataSource);
            }
        }

        private async Task SaveOrderListInDatabase(ObservableCollection<OrderDetailUIModel> orderDetailUIModelList)
        {
            OrderDetail orderDetail;
            foreach (OrderDetailUIModel orderDetailUIModel in orderDetailUIModelList)
            {
                OrderDetailUIModel dbOrderDetail = DBOrderDetailsList.FirstOrDefault(x => x.OrderDetailId == orderDetailUIModel.OrderDetailId);
                //if (dbOrderDetail != null && (dbOrderDetail.Quantity != orderDetailUIModel.Quantity
                //    || dbOrderDetail.Unit != orderDetailUIModel.Unit || dbOrderDetail.Price != orderDetailUIModel.Price
                //    || dbOrderDetail.Total != orderDetailUIModel.Total || dbOrderDetail.CreditRequest != orderDetailUIModel.CreditRequest))
                if (IsOrderDetailDiff(orderDetailUIModel))
                {
                    orderDetailUIModel.OrderDetailUiToDataModel();
                    orderDetail = orderDetailUIModel.OrderDetailData;
                    if ((bool)AppRef.IsCreditRequestOrder)
                    {
                        if (orderDetail.Quantity == 0)
                        {
                            orderDetail.Quantity = -1;
                        }
                        else if (orderDetail.Quantity > 0)
                        {
                            orderDetail.Quantity *= -1;
                        }
                    }
                    orderDetail.UpdatedDate = DateTimeHelper.ConvertToDbInsertDateTimeFormat(DateTime.Now);
                    await ((App)Application.Current).QueryService.InsertOrUpdateOrderDetail(orderDetail);
                }
            }
        }
        private bool IsOrderDetailDiff(OrderDetailUIModel orderDetailUIModel)
        {
            bool result = false;
            OrderDetailUIModel dbOrderDetail = DBOrderDetailsList.FirstOrDefault(x => x.OrderDetailId == orderDetailUIModel.OrderDetailId);
            result |= dbOrderDetail != null && (dbOrderDetail.Quantity != orderDetailUIModel.Quantity);
            result |= dbOrderDetail.Unit != orderDetailUIModel.Unit;
            result |= Convert.ToDecimal(dbOrderDetail?.Price) != Convert.ToDecimal(orderDetailUIModel?.Price);
            result |= Convert.ToDecimal(dbOrderDetail?.Total) != Convert.ToDecimal(orderDetailUIModel?.Total);
            result |= dbOrderDetail?.CreditRequest != orderDetailUIModel?.CreditRequest;

            return result;
        }
        private void PriceChanged(OrderDetailUIModel orderDetailUIModel)
        {
            if ((bool)AppRef.IsCreditRequestOrder)
            {
                if (orderDetailUIModel.CreditRequest.Contains("RTN"))
                {
                    if (RtnGridDataSource != null && RtnGridDataSource.Count > 0)
                    {
                        foreach (var item in RtnGridDataSource)
                        {
                            if (item.ProductID == orderDetailUIModel.ProductID)
                            {
                                var decimalValue = Convert.ToDecimal(orderDetailUIModel.Price);
                                item.Price = decimalValue.ToString("0.00");
                                break;
                            }
                        }
                    }
                }
                else
                {
                    if (DifGridDataSource != null && DifGridDataSource.Count > 0)
                    {
                        foreach (var item in DifGridDataSource)
                        {
                            if (item.ProductID == orderDetailUIModel.ProductID)
                            {
                                var decimalValue = Convert.ToDecimal(orderDetailUIModel.Price);

                                item.Price = decimalValue.ToString("0.00");
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                if (orderDetailUIModel.isTobbaco == 1)
                {
                    foreach (var item in TobaccoGridDataSource)
                    {
                        if (item.ProductID == orderDetailUIModel.ProductID)
                        {
                            var decimalValue = Convert.ToDecimal(orderDetailUIModel.Price);

                            item.Price = decimalValue.ToString("0.00");
                            break;
                        }
                    }
                }
                else
                {
                    foreach (var item in NonTobaccoGridDataSource)
                    {
                        if (item.ProductID == orderDetailUIModel.ProductID)
                        {
                            var decimalValue = Convert.ToDecimal(orderDetailUIModel.Price);

                            item.Price = decimalValue.ToString("0.00");
                            break;
                        }
                    }
                }
            }
            CalculateSubTotalGrandTotal();
        }

        private async Task SetCustomerInformationAndGetCartData()
        {
            int customerId = Convert.ToInt32(AppRef.SelectedCustomerId);

            CartDetailModel = await ((App)Application.Current).QueryService.GetCartDetailsDataForCartScreen(AppRef.CurrentDeviceOrderId, customerId);

            if (CartDetailModel.OrderDetailsList != null && CartDetailModel.OrderDetailsList.Any())
            {
                DBOrderDetailsList = new List<OrderDetailUIModel>();
                foreach (var x in CartDetailModel.OrderDetailsList)
                {
                    DBOrderDetailsList.Add(new OrderDetailUIModel
                    {
                        ProductName = x.ItemNumber,
                        ProductDescription = x.ItemDescription,
                        ItemNumber = x.ItemNumber,
                        ItemDescription = x.ItemDescription,
                        CategoryId = x.CategoryId,
                        BrandId = x.BrandId,
                        StyleId = x.StyleId,
                        Unit = x.Unit,
                        ProductID = x.ProductID,
                        isTobbaco = x.isTobbaco,
                        DeviceOrderID = x.DeviceOrderID,
                        OrderId = x.OrderId,
                        OrderDetailId = x.OrderDetailId,
                        Price = x.Price,
                        PriceDisplay = x.PriceDisplay,
                        Quantity = x.Quantity,
                        QuantityDisplay = x.QuantityDisplay,
                        BrandName = x.BrandName,
                        StyleName = x.StyleName,
                        CategoryName = x.CategoryName,
                        Total = x.Total,
                        TotalDisplay = x.TotalDisplay,
                        CreditRequest = x.CreditRequest,
                        CreatedDate = x.CreatedDate
                    });
                }

                if ((bool)AppRef.IsCreditRequestOrder)
                {
                    SetRtnDifGridViews();
                }
                else
                {
                    SetTobaccoNonTobaccoGrid();
                }

                if (CartDetailModel.CustomerAccountType == 2)
                {
                    PurchaseOrderButtonText = "Retail Transaction";
                }
                else if (CartDetailModel.CustomerAccountType != 2 && (bool)AppRef.IsCreditRequestOrder)
                {
                    PurchaseOrderButtonText = "Credit Request";
                }
                else
                {
                    PurchaseOrderButtonText = "Purchase Order";
                }

                if (AppRef.IsCarStockOrder ?? false) PurchaseOrderButtonText = "Car Stock Order";
            }
            else
            {
                CartDetailModel.ShowCartIsEmpty();
            }
        }

        /// <summary>
        /// Set Grid for Place an Order flow
        /// </summary>
        private void SetTobaccoNonTobaccoGrid()
        {
            CartDetailModel.EmptyCartVisibility = Visibility.Collapsed;
            CartDetailModel.NoSelectedCustomerVisibility = Visibility.Collapsed;
            CartDetailModel.GrandTotalVisibility = Visibility.Visible;

            LoadCreditRequestGrids = false;
            LoadTobaccoGrids = true;

            IEnumerable<OrderDetailUIModel> cartOrderList;
            if ((bool)AppRef.IsOrderTypeChanged)
            {
                CartDetailModel.OrderDetailsList
              .ForEach(orderDetailItem =>
              {

                  orderDetailItem.QuantityDisplay = orderDetailItem.Quantity.ToString();

                  if (!string.IsNullOrWhiteSpace(orderDetailItem.Price))
                  {
                      var decimalPrice = Convert.ToDecimal(orderDetailItem.Price);
                      orderDetailItem.PriceDisplay = string.Format("{0:C}", decimalPrice);
                  }
                  else
                  {
                      orderDetailItem.PriceDisplay = "$0.00";
                  }

                  if (String.IsNullOrWhiteSpace(orderDetailItem.Total))
                  {
                      orderDetailItem.Total = "0.00";
                      orderDetailItem.TotalDisplay = FormatStringToTwoDecimal(0);
                  }
                  else
                  {
                      if (orderDetailItem.Quantity < 0 && !orderDetailItem.Total.StartsWith("-") && Convert.ToDecimal(orderDetailItem.Total) > 0)
                      {
                          orderDetailItem.Total = $"-{orderDetailItem.Total}";
                      }
                      var tempTotal = Convert.ToDecimal(orderDetailItem.Total);
                      orderDetailItem.TotalDisplay = FormatStringToTwoDecimal(tempTotal);
                  }

                  orderDetailItem.CreditRequest = "";
                  if (CartDetailModel.CustomerAccountType != 2)
                  {
                      orderDetailItem.Unit = "CA";
                  }
                  else
                  {
                      if (orderDetailItem.CategoryId == 1 || orderDetailItem.CategoryId == 6 ||
                          orderDetailItem.CategoryId == 13 || orderDetailItem.CategoryId == 14)
                      {
                          orderDetailItem.Unit = "BX";
                      }
                      else if (orderDetailItem.CategoryId == 2 || orderDetailItem.CategoryId == 3 ||
                          orderDetailItem.CategoryId == 7 || orderDetailItem.CategoryId == 17 ||
                          orderDetailItem.CategoryId == 1 || orderDetailItem.CategoryId == 6 ||
                          orderDetailItem.CategoryId == 15 || orderDetailItem.CategoryId == 12)
                      {
                          orderDetailItem.Unit = "EA";
                      }
                      else
                      {
                          orderDetailItem.Unit = "CA";
                      }
                  }
              }
              );
            }
            else
            {
                CartDetailModel.OrderDetailsList
                                   .ForEach(orderDetailItem =>
                                   {
                                       if (!string.IsNullOrEmpty(orderDetailItem.Price))
                                       {
                                           var decimalPrice = Convert.ToDecimal(orderDetailItem.Price);

                                           orderDetailItem.PriceDisplay = string.Format("{0:C}", decimalPrice);

                                       }
                                       else
                                       {
                                           orderDetailItem.PriceDisplay = "$0.00";
                                       }
                                   });
            }

            cartOrderList = CartDetailModel.OrderDetailsList
           .Select(x =>
             new OrderDetailUIModel
             {
                 ProductName = x.ItemNumber,
                 ProductDescription = x.ItemDescription,
                 ItemNumber = x.ItemNumber,
                 ItemDescription = x.ItemDescription,
                 CategoryId = x.CategoryId,
                 BrandId = x.BrandId,
                 StyleId = x.StyleId,
                 Unit = x.Unit,
                 ProductID = x.ProductID,
                 isTobbaco = x.isTobbaco,
                 DeviceOrderID = x.DeviceOrderID,
                 OrderId = x.OrderId,
                 OrderDetailId = x.OrderDetailId,
                 Price = x.Price,
                 PriceDisplay = x.PriceDisplay,
                 Quantity = x.Quantity,
                 QuantityDisplay = x.QuantityDisplay,
                 BrandName = x.BrandName,
                 StyleName = x.StyleName,
                 CategoryName = x.CategoryName,
                 Total = x.Total,
                 TotalDisplay = x.TotalDisplay,
                 CreditRequest = x.CreditRequest,
                 CreatedDate = x.CreatedDate
             });

            var tobaccoList = cartOrderList.Where(x => x.isTobbaco == 1);
            CartDetailModel.TobaccoGridVisibility = tobaccoList.Any() ? Visibility.Visible : Visibility.Collapsed;
            TobaccoGridDataSource = new ObservableCollection<OrderDetailUIModel>(tobaccoList);

            var nonTobaccoList = cartOrderList.Where(x => x.isTobbaco == 0);
            CartDetailModel.NonTobaccoGridVisibility = nonTobaccoList.Any() ? Visibility.Visible : Visibility.Collapsed;
            NonTobaccoGridDataSource = new ObservableCollection<OrderDetailUIModel>(nonTobaccoList);

            CartDetailModel.BottomButtonVisiblity = string.IsNullOrEmpty(AppRef.SelectedCustomerId) ? Visibility.Collapsed : Visibility.Visible;

            CalculateSubTotalGrandTotalOnLoad();
        }

        private void SetRtnDifGridViews()
        {
            CartDetailModel.EmptyCartVisibility = Visibility.Collapsed;
            CartDetailModel.NoSelectedCustomerVisibility = Visibility.Collapsed;
            CartDetailModel.GrandTotalVisibility = Visibility.Visible;

            LoadCreditRequestGrids = true;
            LoadTobaccoGrids = false;

            IEnumerable<OrderDetailUIModel> cartOrderList;

            CartDetailModel.OrderDetailsList.ForEach(orderDetailItem =>
            {
                if ((bool)AppRef.IsOrderTypeChanged)
                {
                    if (orderDetailItem.Quantity > 0)
                    {
                        orderDetailItem.Quantity *= -1;
                    }
                    else if (orderDetailItem.Quantity == 0)
                    {
                        orderDetailItem.Quantity = -1;
                    }
                }

                orderDetailItem.QuantityDisplay = orderDetailItem.Quantity.ToString();

                if (!string.IsNullOrEmpty(orderDetailItem.Price))
                {
                    var decimalPrice = Convert.ToDecimal(orderDetailItem.Price);

                    orderDetailItem.PriceDisplay = decimalPrice.ToString("0.00");
                }
                else
                {
                    orderDetailItem.PriceDisplay = "0.00";
                }

                if (string.IsNullOrWhiteSpace(orderDetailItem.Total))
                {
                    orderDetailItem.Total = "0";
                    orderDetailItem.TotalDisplay = FormatStringToTwoDecimal(0);
                }
                else
                {
                    decimal tempTotal = 0;
                    tempTotal = Convert.ToDecimal(orderDetailItem.Price) * orderDetailItem.Quantity;
                    if (tempTotal != Convert.ToDecimal(orderDetailItem.Total))
                        orderDetailItem.Total = $"{tempTotal}";

                    orderDetailItem.TotalDisplay = FormatStringToTwoDecimal(tempTotal);
                }
                if (string.IsNullOrEmpty(orderDetailItem.CreditRequest))
                {
                    orderDetailItem.Unit = orderDetailItem.CategoryId == 1 || orderDetailItem.CategoryId == 6 ? "BX" : "EA";
                    orderDetailItem.CreditRequest = "RTN-Retail Returns";
                }

            });

            cartOrderList = CartDetailModel.OrderDetailsList.Select(
                x =>
                 new OrderDetailUIModel
                 {
                     ProductName = x.ItemNumber,
                     ProductDescription = x.ItemDescription,
                     ItemNumber = x.ItemNumber,
                     ItemDescription = x.ItemDescription,
                     CategoryId = x.CategoryId,
                     BrandId = x.BrandId,
                     StyleId = x.StyleId,
                     Unit = x.Unit,
                     ProductID = x.ProductID,
                     isTobbaco = x.isTobbaco,
                     DeviceOrderID = x.DeviceOrderID,
                     OrderId = x.OrderId,
                     OrderDetailId = x.OrderDetailId,
                     Price = x.Price,
                     PriceDisplay = x.PriceDisplay,
                     Quantity = x.Quantity,
                     QuantityDisplay = x.QuantityDisplay,
                     BrandName = x.BrandName,
                     StyleName = x.StyleName,
                     CategoryName = x.CategoryName,
                     Total = x.Total,
                     TotalDisplay = x.TotalDisplay,
                     CreditRequest = x.CreditRequest,
                     CreatedDate = x.CreatedDate
                 });

            var rtnList = cartOrderList.Where(x => x.CreditRequest.Contains("RTN"));
            if (rtnList != null && rtnList.Any())
            {
                if (rtnList.Any(x => x.isTobbaco != 1))
                {
                    SetRtnCreditRequestCombobox();
                }
                else
                {
                    if (CartDetailModel.OrderDetailModel.RtnDifCreditRequestComboboxList != null &&
                        CartDetailModel.OrderDetailModel.RtnDifCreditRequestComboboxList.Count > 0)
                    {
                        CartDetailModel.OrderDetailModel.RtnDifCreditRequestComboboxList.RemoveAt(0);
                    }
                }
                RtnGridDataSource = new ObservableCollection<OrderDetailUIModel>(rtnList);
                CartDetailModel.UperGridSubTotal = rtnList.Sum(x => Convert.ToDecimal(x.Total));
            }

            var difList = cartOrderList.Where(x => x.CreditRequest.Contains("DIF"));

            if (difList != null && difList.Any())
            {
                if (!(CartDetailModel.OrderDetailModel?.RtnDifCreditRequestComboboxList?.Count > 0))
                {
                    SetRtnCreditRequestCombobox();
                }
                DifGridDataSource = new ObservableCollection<OrderDetailUIModel>(difList);
                CartDetailModel.LowerGridSubTotal = difList.Sum(x => Convert.ToDecimal(x.Total));
            }

            CartDetailModel.RtnGridVisibility = (rtnList != null && rtnList.Any()) ? Visibility.Visible : Visibility.Collapsed;
            CartDetailModel.DifGridVisibility = (difList != null && difList.Any()) ? Visibility.Visible : Visibility.Collapsed;

            CartDetailModel.BottomButtonVisiblity = string.IsNullOrEmpty(AppRef.SelectedCustomerId) ? Visibility.Collapsed : Visibility.Visible;

            CartDetailModel.OrderDetailModel.SubTotalUpperDisplay = FormatStringToTwoDecimal(CartDetailModel.UperGridSubTotal);
            CartDetailModel.OrderDetailModel.SubTotalLowerDisplay = FormatStringToTwoDecimal(CartDetailModel.LowerGridSubTotal);

            CalculateGrandTotal(CartDetailModel.UperGridSubTotal, CartDetailModel.LowerGridSubTotal);

        }

        private void CalculateSubTotalGrandTotalOnLoad()
        {
            if (TobaccoGridDataSource?.Count > 0)
            {
                CartDetailModel.UperGridSubTotal = TobaccoGridDataSource.Sum(item => GetDisplayValues(item));
                //foreach (var item in TobaccoGridDataSource)
                //{
                //    decimal total = GetDisplayValues(item);
                //    CartDetailModel.UperGridSubTotal += total;
                //}
            }

            CartDetailModel.OrderDetailModel.SubTotalUpperDisplay = FormatStringToTwoDecimal(CartDetailModel.UperGridSubTotal);

            if (NonTobaccoGridDataSource?.Count > 0)
            {
                CartDetailModel.LowerGridSubTotal = NonTobaccoGridDataSource.Sum(item => GetDisplayValues(item));
                //foreach (var item in NonTobaccoGridDataSource)
                //{
                //    decimal total = GetDisplayValues(item);
                //    CartDetailModel.LowerGridSubTotal += total;
                //}
            }

            CartDetailModel.OrderDetailModel.SubTotalLowerDisplay = FormatStringToTwoDecimal(CartDetailModel.LowerGridSubTotal);

            CalculateGrandTotal(CartDetailModel.UperGridSubTotal, CartDetailModel.LowerGridSubTotal);
        }

        private void CalculateSubTotalGrandTotal()
        {
            decimal subTotalUpperGrid = 0;
            decimal subTotalLowerGrid = 0;
            if (TobaccoGridDataSource?.Count > 0)
            {
                foreach (var item in TobaccoGridDataSource)
                {
                    if (item != null)
                    {
                        decimal total = GetDisplayValues(item);
                        // await SaveValuesInDatabase(item);
                        subTotalUpperGrid += total;
                    }
                }
            }
            else if (RtnGridDataSource?.Count > 0)
            {
                foreach (var item in RtnGridDataSource)
                {
                    if (item != null)
                    {
                        decimal total = GetDisplayValues(item);
                        // await SaveValuesInDatabase(item);
                        subTotalUpperGrid += total;
                    }
                }
            }
            CartDetailModel.UperGridSubTotal = subTotalUpperGrid;
            CartDetailModel.OrderDetailModel.SubTotalUpperDisplay = FormatStringToTwoDecimal(subTotalUpperGrid);
            if (NonTobaccoGridDataSource?.Count > 0)
            {
                foreach (var item in NonTobaccoGridDataSource)
                {
                    if (item != null)
                    {
                        decimal total = GetDisplayValues(item);
                        // await SaveValuesInDatabase(item);
                        subTotalLowerGrid = subTotalLowerGrid + total;
                    }
                }
            }
            else if (DifGridDataSource?.Count > 0)
            {
                foreach (var item in DifGridDataSource)
                {
                    if (item != null)
                    {
                        decimal total = GetDisplayValues(item);
                        // await SaveValuesInDatabase(item);
                        subTotalLowerGrid = subTotalLowerGrid + total;
                    }
                }
            }
            CartDetailModel.LowerGridSubTotal = subTotalLowerGrid;
            CartDetailModel.OrderDetailModel.SubTotalLowerDisplay = FormatStringToTwoDecimal(subTotalLowerGrid);
            decimal grandTotal = subTotalLowerGrid + subTotalUpperGrid;
            CartDetailModel.OrderDetailModel.GrandTotalDisplay = FormatStringToTwoDecimal(grandTotal);
        }

        private void CalculateSubTotalForUperGrid()
        {
            //CartDetailModel.UperGridSubTotal = 0;

            if ((bool)AppRef.IsCreditRequestOrder)
            {
                CartDetailModel.UperGridSubTotal = RtnGridDataSource.Sum(item => GetDisplayValues(item));
                //foreach (var item in RtnGridDataSource)
                //{
                //    decimal total = GetDisplayValues(item);
                //    CartDetailModel.UperGridSubTotal += total;
                //}
            }
            else
            {
                CartDetailModel.UperGridSubTotal = TobaccoGridDataSource.Sum(item => GetDisplayValues(item));
                //foreach (var item in TobaccoGridDataSource)
                //{
                //    decimal total = GetDisplayValues(item);
                //    CartDetailModel.UperGridSubTotal += total;
                //}
            }

            CartDetailModel.OrderDetailModel.SubTotalUpperDisplay = FormatStringToTwoDecimal(CartDetailModel.UperGridSubTotal);

            CalculateGrandTotal(CartDetailModel.UperGridSubTotal, CartDetailModel.LowerGridSubTotal);
        }

        private void CalculateSubTotalForLowerGrid()
        {
            CartDetailModel.LowerGridSubTotal = 0;

            if ((bool)AppRef.IsCreditRequestOrder)
            {
                CartDetailModel.LowerGridSubTotal = DifGridDataSource.Sum(item => GetDisplayValues(item));
                //foreach (var item in DifGridDataSource)
                //{
                //    decimal total = GetDisplayValues(item);
                //    CartDetailModel.LowerGridSubTotal += total;
                //}
            }
            else
            {
                CartDetailModel.LowerGridSubTotal = NonTobaccoGridDataSource.Sum(item => GetDisplayValues(item));
                //foreach (var item in NonTobaccoGridDataSource)
                //{
                //    decimal total = GetDisplayValues(item);
                //    CartDetailModel.LowerGridSubTotal += total;
                //}
            }

            CartDetailModel.OrderDetailModel.SubTotalLowerDisplay = FormatStringToTwoDecimal(CartDetailModel.LowerGridSubTotal);

            CalculateGrandTotal(CartDetailModel.UperGridSubTotal, CartDetailModel.LowerGridSubTotal);
        }

        private void CalculateGrandTotal(decimal uperGridTotal, decimal lowerGridTotal)
        {
            decimal grandTotal = uperGridTotal + lowerGridTotal;

            CartDetailModel.OrderDetailModel.GrandTotalDisplay = FormatStringToTwoDecimal(grandTotal);
        }

        private string FormatStringToTwoDecimal(decimal subTotalUpperGrid)
        {
            var formattedString = "$ " + string.Format("{0:0.00}", subTotalUpperGrid);
            return formattedString;
        }

        private decimal GetDisplayValues(OrderDetailUIModel item)
        {
            var total = Convert.ToDecimal(item.Price) * item.Quantity;

            if (item.IsQuantityEdited && string.IsNullOrEmpty(item.QuantityDisplay))
            {
                item.QuantityDisplay = string.Empty;
                item.IsQuantityEdited = false;
            }
            else if (string.IsNullOrEmpty(item.QuantityDisplay))
            {
                item.QuantityDisplay = item.Quantity.ToString();
            }

            item.Total = string.Format("{0:0.00}", total);
            item.TotalDisplay = FormatStringToTwoDecimal(total);

            if (!item.IsPriceEdited)
            {
                if (!string.IsNullOrEmpty(item.Price))
                {
                    var decimalPrice = Convert.ToDecimal(item.Price);

                    item.PriceDisplay = decimalPrice.ToString("0.00");

                }
                else
                {
                    item.PriceDisplay = "0.00";
                }
            }
            else
            {
                item.IsPriceEdited = false;
            }

            return total;
        }

        private void SetRtnCreditRequestCombobox()
        {
            ICollection<string> RtnCreditRequestOptions = new List<string>();

            RtnCreditRequestOptions.Add("DIF-Destroyed");
            RtnCreditRequestOptions.Add("RTN-Retail Returns");
            RtnCreditRequestOptions.Add("RTN-Out Of Code");
            RtnCreditRequestOptions.Add("RTN-Defective");
            RtnCreditRequestOptions.Add("RTN-Overstock");
            RtnCreditRequestOptions.Add("RTN-Discontinued");
            RtnCreditRequestOptions.Add("RTN-Reset");
            RtnCreditRequestOptions.Add("RTN-Mispick");
            RtnCreditRequestOptions.Add("RTN-Misship");
            RtnCreditRequestOptions.Add("RTN-Order Error");

            CartDetailModel.OrderDetailModel.RtnDifCreditRequestComboboxList = new ObservableCollection<string>(RtnCreditRequestOptions);

            //RtnCreditRequestOptions.ForEach(x => CartDetailModel.OrderDetailModel.RtnDifCreditRequestComboboxList.Add(x));
        }

        /// <summary>
        /// Delete cart item
        /// </summary>
        /// <param name="orderDetailUIModel"></param>
        private async Task DeleteImageClickedAsync(OrderDetailUIModel orderDetailUIModel)
        {
            await ShowDeleteWarning(orderDetailUIModel);
        }

        /// <summary>
        /// Warning pop-up for deleting cart item
        /// </summary>
        /// <param name="orderDetailUIModel"></param>
        private async Task ShowDeleteWarning(OrderDetailUIModel orderDetailUIModel)
        {

            ContentDialog deleteCartItemDialog = new ContentDialog
            {
                Content = resourceLoader.GetString("CartItemDeleteWarning"),
                PrimaryButtonText = resourceLoader.GetString("OK"),
                SecondaryButtonText = resourceLoader.GetString("Cancel")
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
            int CartValue = ((App)Application.Current).CartItemCount;

            CartValue--;

            ((App)Application.Current).CartItemCount = CartValue;

            await DeleteItemFromDb(orderDetailUIModel);

            if ((bool)AppRef.IsCreditRequestOrder)
            {
                if (orderDetailUIModel.CreditRequest.Contains("RTN"))
                {
                    RtnGridDataSource.Remove(orderDetailUIModel);
                    CalculateSubTotalForUperGrid();
                    CartDetailModel.RtnGridVisibility = (RtnGridDataSource.Count > 0) ? Visibility.Visible : Visibility.Collapsed;
                }
                else
                {
                    DifGridDataSource.Remove(orderDetailUIModel);
                    CalculateSubTotalForLowerGrid();
                    CartDetailModel.DifGridVisibility = (DifGridDataSource.Count > 0) ? Visibility.Visible : Visibility.Collapsed;
                }

                if (RtnGridDataSource?.Count == 0 && DifGridDataSource?.Count == 0)
                {
                    CartDetailModel.ShowCartIsEmpty();
                }
            }
            else
            {
                if (orderDetailUIModel?.isTobbaco == 1)
                {
                    TobaccoGridDataSource.Remove(orderDetailUIModel);
                    CalculateSubTotalForUperGrid();
                    CartDetailModel.TobaccoGridVisibility = (TobaccoGridDataSource.Count > 0) ? Visibility.Visible : Visibility.Collapsed;
                }
                else
                {
                    NonTobaccoGridDataSource.Remove(orderDetailUIModel);
                    CalculateSubTotalForLowerGrid();
                    CartDetailModel.NonTobaccoGridVisibility = (NonTobaccoGridDataSource.Count > 0) ? Visibility.Visible : Visibility.Collapsed;
                }

                if (TobaccoGridDataSource?.Count == 0 && NonTobaccoGridDataSource?.Count == 0)
                {
                    CartDetailModel.ShowCartIsEmpty();
                }
            }
        }

        /// <summary>
        /// Delete item from database
        /// </summary>
        /// <param name="orderDetailUIModel"></param>
        private async Task DeleteItemFromDb(OrderDetailUIModel orderDetailUIModel)
        {
            orderDetailUIModel.OrderDetailUiToDataModel();

            var _list = new List<OrderDetail>() { orderDetailUIModel.OrderDetailData };

            await ((App)Application.Current).QueryService.DeleteOrderDetail(_list[0].ProductId, AppRef.CurrentDeviceOrderId);
        }

        private void IsAllLowerGridRowsUnselected()
        {
            if ((bool)AppRef.IsCreditRequestOrder)
            {
                foreach (var item in DifGridDataSource)
                {
                    item.IsAllLowerGridSelected = false;
                }
            }
            else
            {
                foreach (var item in NonTobaccoGridDataSource)
                {
                    item.IsAllLowerGridSelected = false;
                }
            }
        }

        private void IsAllLowerGridRowsSelected()
        {
            if ((bool)AppRef.IsCreditRequestOrder)
            {
                foreach (var item in DifGridDataSource)
                {
                    item.IsAllLowerGridSelected = true;
                }
            }
            else
            {
                foreach (var item in NonTobaccoGridDataSource)
                {
                    item.IsAllLowerGridSelected = true;
                }
            }
        }

        private void IsAllUpperGridRowsUnselected()
        {
            if ((bool)AppRef.IsCreditRequestOrder)
            {
                foreach (var item in RtnGridDataSource)
                {
                    item.IsAllUpperGridSelected = false;
                }
            }
            else
            {
                foreach (var item in TobaccoGridDataSource)
                {
                    item.IsAllUpperGridSelected = false;
                }
            }
        }

        private void IsAllUpperGridRowsSelected()
        {
            if ((bool)AppRef.IsCreditRequestOrder)
            {
                foreach (var item in RtnGridDataSource)
                {
                    item.IsAllUpperGridSelected = true;
                }
            }
            else
            {
                foreach (var item in TobaccoGridDataSource)
                {
                    item.IsAllUpperGridSelected = true;
                }
            }
        }

        private async Task EmailFactSheetCommadHandler()
        {
            try
            {
                await SaveCartItems();
                var selectedTobaccoProducts = TobaccoGridDataSource?.Where(x => x.IsAllUpperGridSelected).ToList();
                var selectedNonTobaccoProducts = NonTobaccoGridDataSource?.Where(x => x.IsAllLowerGridSelected).ToList();
                var selectedRtnProducts = RtnGridDataSource?.Where(x => x.IsAllUpperGridSelected).ToList();
                var selectedDifProducts = DifGridDataSource?.Where(x => x.IsAllLowerGridSelected).ToList();
                //RtnGridDataSource.Where(x=>x.)

                var total = (selectedTobaccoProducts?.Count ?? 0) + (selectedNonTobaccoProducts?.Count ?? 0) + (selectedRtnProducts?.Count ?? 0) + (selectedDifProducts?.Count ?? 0);

                if (total > 5)
                {
                    await Helpers.AlertHelper.Instance.ShowConfirmationAlert(resourceLoader.GetString("ALERT"),
                        resourceLoader.GetString("CART_FACT_SHEET_LESS_THAN_5_ITEM_ALERT"), resourceLoader.GetString("OK"));
                }
                else if (total > 0)
                {
                    if (AppRef.LocalFileService.IsSrcZipFolderExist(Core.Interface.SrcZipFileType.Product))
                    {
                        CartDetailModel.IsLoading = true;
                        CartDetailModel.LoadingText = "Please wait";

                        var listOfFilePath = new List<string>();
                        var listOfProductNumber = new List<string>();

                        if (selectedNonTobaccoProducts != null)
                        {
                            foreach (var item in selectedNonTobaccoProducts)
                            {
                                var url = await GetAdditionalDocumentsUrl(item.ProductID);

                                var path = AppRef.LocalFileService.GetLocalFilePathByFileType(Core.Interface.SrcZipFileType.Product, url);

                                if (!string.IsNullOrWhiteSpace(path))
                                {
                                    listOfFilePath.Add(path);
                                    listOfProductNumber.Add(item.ItemNumber);
                                }
                            }
                        }

                        if (selectedTobaccoProducts != null)
                        {
                            foreach (var item in selectedTobaccoProducts)
                            {
                                var url = await GetAdditionalDocumentsUrl(item.ProductID);

                                var path = AppRef.LocalFileService.GetLocalFilePathByFileType(Core.Interface.SrcZipFileType.Product, url);

                                if (!string.IsNullOrWhiteSpace(path))
                                {
                                    listOfFilePath.Add(path);
                                    listOfProductNumber.Add(item.ItemNumber);
                                }
                            }
                        }

                        if (selectedRtnProducts != null)
                        {
                            foreach (var item in selectedRtnProducts)
                            {
                                var url = await GetAdditionalDocumentsUrl(item.ProductID);

                                var path = AppRef.LocalFileService.GetLocalFilePathByFileType(Core.Interface.SrcZipFileType.Product, url);

                                if (!string.IsNullOrWhiteSpace(path))
                                {
                                    listOfFilePath.Add(path);
                                    listOfProductNumber.Add(item.ItemNumber);
                                }
                            }
                        }

                        if (selectedDifProducts != null)
                        {
                            foreach (var item in selectedDifProducts)
                            {
                                var url = await GetAdditionalDocumentsUrl(item.ProductID);

                                var path = AppRef.LocalFileService.GetLocalFilePathByFileType(Core.Interface.SrcZipFileType.Product, url);

                                if (!string.IsNullOrWhiteSpace(path))
                                {
                                    listOfFilePath.Add(path);
                                    listOfProductNumber.Add(item.ItemNumber);
                                }
                            }
                        }

                        await EmailService.Instance.SendMailFromOutlook(new EmailModel()
                        {
                            Subject = string.Join(',', listOfProductNumber),
                            AttachmentListByPath = listOfFilePath
                        });

                        //await Task.Delay(1000);

                        CartDetailModel.IsLoading = false;
                    }
                    else
                    {
                        await AlertHelper.Instance.ShowConfirmationAlert(resourceLoader.GetString("ALERT"),
                            resourceLoader.GetString("FileDoesNotExist"), resourceLoader.GetString("OK"));
                    }
                }
                else
                {
                    await AlertHelper.Instance.ShowConfirmationAlert(resourceLoader.GetString("ALERT"),
                        resourceLoader.GetString("SelectAnyProduct"), resourceLoader.GetString("OK"), string.Empty);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                CartDetailModel.IsLoading = false;
            }
        }

        private async Task NavigateToRetailTransactionAsync()
        {
            isTriggerConfirmOrder = true;
            await SaveCartItems();
            NavigationService.NavigateShellFrame(typeof(RetailTransactionPage));
        }

        private async Task<string> GetAdditionalDocumentsUrl(int productId)
        {
            var productDetailUiModel = await ((App)Application.Current).QueryService.GetProductAdditionalDocumentData(productId);

            var returnString = productDetailUiModel != null ? productDetailUiModel.Factsheet : string.Empty;

            return returnString;
        }

        private void NumPadButtonClicked(string value)
        {
            if (isOpenedFrom == "quantity")
            {
                if ((bool)AppRef.IsCreditRequestOrder)
                {
                    if (IsOpenedForCRQuantity.Equals("RtnQuantity") && RtnGridDataSource?.Count > 0)
                    {
                        UpdateQuantityFieldValue(RtnGridDataSource, value);
                    }
                    else if (IsOpenedForCRQuantity.Equals("DifQuantity") && DifGridDataSource?.Count > 0)
                    {
                        UpdateQuantityFieldValue(DifGridDataSource, value);
                    }
                }
                else
                {
                    if (CartDetailModel.OrderDetailModel.isTobbaco == 1)
                    {
                        if (TobaccoGridDataSource?.Count > 0)
                        {
                            UpdateQuantityFieldValue(TobaccoGridDataSource, value);
                        }
                    }
                    else
                    {
                        if (NonTobaccoGridDataSource?.Count > 0)
                        {
                            UpdateQuantityFieldValue(NonTobaccoGridDataSource, value);
                        }
                    }
                }
            }
            else
            {
                if ((bool)AppRef.IsCreditRequestOrder)
                {
                    if (IsOpenedForCRUnitPrice.Equals("RtnPrice") && RtnGridDataSource?.Count > 0)
                    {
                        UpdateCreditRequestPriceFieldValue(RtnGridDataSource, value);
                    }
                    else if (IsOpenedForCRUnitPrice.Equals("DifPrice") && DifGridDataSource?.Count > 0)
                    {
                        UpdateCreditRequestPriceFieldValue(DifGridDataSource, value);
                    }
                }
                else
                {
                    if (CartDetailModel.OrderDetailModel.isTobbaco == 1)
                    {
                        if (TobaccoGridDataSource?.Count > 0)
                        {
                            UpdatePriceFieldValue(TobaccoGridDataSource, value);
                        }
                    }
                    else
                    {
                        if (NonTobaccoGridDataSource?.Count > 0)
                        {
                            UpdatePriceFieldValue(NonTobaccoGridDataSource, value);
                        }
                    }
                }
            }

            CalculateSubTotalGrandTotal();
        }

        private void UpdatePriceFieldValue(ObservableCollection<OrderDetailUIModel> gridDataSource, string value)
        {
            foreach (var item in gridDataSource)
            {
                if (item.ProductID == CartDetailModel.OrderDetailModel.ProductID)
                {
                    priceBeforeEdit = CartDetailModel.OrderDetailModel.PriceDisplay;
                    item.IsPriceEdited = true;
                    if (value.Equals("C"))
                    {
                        if (item.PriceDisplay.Length > 0)
                        {
                            item.PriceDisplay = item.PriceDisplay.Remove(item.PriceDisplay.Length - 1);
                            priceString = item.PriceDisplay;
                        }
                    }
                    else
                    {
                        if (!priceString.Contains("."))
                        {
                            if (value != ".")
                            {
                                if (priceString.Length < 4)
                                {
                                    priceString += value;
                                    item.PriceDisplay = string.Format("{0:0.00}", Convert.ToDecimal(priceString));
                                    item.Price = priceString;
                                }
                            }
                            else
                            {
                                priceString += value;
                                item.PriceDisplay = priceString;
                            }
                        }
                        else
                        {
                            if (value != ".")
                            {
                                string tail = priceString.Substring(priceString.LastIndexOf('.') + 1);

                                if (tail.Length == 0)
                                {
                                    int dotIndex = priceString.IndexOf('.');
                                    int insertToIndex = ++dotIndex;
                                    var newString = priceString.Insert(insertToIndex, value);

                                    priceString = newString;

                                    item.Price = priceString;
                                    item.PriceDisplay = priceString;
                                }
                                else if (tail.Length == 1)
                                {
                                    priceString += value;
                                    item.Price = priceString;
                                    item.PriceDisplay = priceString;
                                }
                            }
                        }
                    }
                    break;
                }
            }
        }

        private void UpdateCreditRequestPriceFieldValue(ObservableCollection<OrderDetailUIModel> gridDataSource, string value)
        {
            foreach (var item in gridDataSource)
            {
                if (item.ProductID == CartDetailModel.OrderDetailModel.ProductID)
                {
                    priceBeforeEdit = CartDetailModel.OrderDetailModel.PriceDisplay;
                    item.IsPriceEdited = true;
                    if (value.Equals("C"))
                    {
                        if (item.PriceDisplay.Length > 0)
                        {
                            item.PriceDisplay = item.PriceDisplay.Remove(item.PriceDisplay.Length - 1);
                            priceString = item.PriceDisplay;
                        }
                    }
                    else
                    {
                        if (!priceString.Contains("."))
                        {
                            if (value != ".")
                            {
                                if (priceString.Length < 5)
                                {
                                    priceString += value;
                                    item.PriceDisplay = string.Format("{0:0.00}", priceString);
                                    item.Price = priceString;
                                }
                            }
                            else
                            {
                                priceString += value;
                                item.PriceDisplay = priceString;
                            }
                        }
                        else
                        {
                            if (value != ".")
                            {
                                string tail = priceString.Substring(priceString.LastIndexOf('.') + 1);
                                if (tail.Length == 0)
                                {
                                    int dotIndex = priceString.IndexOf('.');
                                    int insertToIndex = ++dotIndex;
                                    var newString = priceString.Insert(insertToIndex, value);
                                    priceString = newString;

                                    item.Price = priceString;
                                    item.PriceDisplay = priceString;

                                }
                                else if (tail.Length == 1)
                                {
                                    priceString += value;
                                    item.Price = priceString;
                                    item.PriceDisplay = priceString;
                                }
                            }
                        }
                    }
                    break;
                }
            }
        }

        private void UpdateQuantityFieldValue(ObservableCollection<OrderDetailUIModel> gridDataSource, string value)
        {
            try
            {
                foreach (var item in gridDataSource)
                {
                    if (item.ProductID == CartDetailModel.OrderDetailModel?.ProductID)
                    {
                        item.IsQuantityEdited = true;
                        quantityBeforeEdit = CartDetailModel.OrderDetailModel.QuantityDisplay;

                        if (value.Equals("C"))
                        {
                            if (item?.QuantityDisplay?.Length > 0)
                            {
                                item.QuantityDisplay = item?.QuantityDisplay?.Remove(item.QuantityDisplay.Length - 1);
                                quantityString = item.QuantityDisplay;
                            }
                        }
                        else
                        {
                            item.QuantityDisplay = item?.QuantityDisplay ?? "";
                            if (value == "-" && quantityString.IndexOf('-') != -1)
                            {
                                break;
                            }
                            else if (value == "-" && !item.QuantityDisplay.StartsWith('-'))
                            {
                                quantityString = "-" + quantityString;
                            }
                            else if (item.QuantityDisplay.Length != 0)
                            {
                                int number;
                                bool success = true;
                                if (int.TryParse(quantityString, out number))
                                {
                                    success = Math.Abs(number) <= 999;
                                }
                                if (success)
                                {
                                    quantityString += value;
                                }
                            }
                            else
                            {
                                quantityString = value;
                            }
                            item.QuantityDisplay = quantityString;
                        }

                        if (!string.IsNullOrEmpty(item?.QuantityDisplay))
                        {
                            if (item?.QuantityDisplay != "-")
                            {
                                item.Quantity = Convert.ToInt32(item.QuantityDisplay);
                                item.QuantityDisplay = item.Quantity.ToString();
                            }
                        }
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        #endregion
    }
}