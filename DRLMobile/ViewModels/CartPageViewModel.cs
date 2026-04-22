using DRLMobile.Core.Helpers;
using DRLMobile.Core.Models.DataModels;
using DRLMobile.Core.Models.UIModels;
using DRLMobile.Helpers;
using DRLMobile.Services;
using DRLMobile.Views;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
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

namespace DRLMobile.ViewModels
{
    public class CartPageViewModel : ObservableObject
    {
        #region Properties

        private readonly ResourceLoader resourceLoader;
        private readonly App AppRef = (App)Application.Current;

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

        private void InitializeCommands()
        {
            DeleteImageCommand = new AsyncRelayCommand<OrderDetailUIModel>(DeleteImageClickedAsync);
            QuantityChangedCommand = new AsyncRelayCommand<OrderDetailUIModel>(QuantityChangedAsync);
            PriceChangedCommand = new AsyncRelayCommand<OrderDetailUIModel>(PriceChangedAsync);
            UnitComboboxChangedCommand = new AsyncRelayCommand<OrderDetailUIModel>(UnitComboboxChanged);
            CreditRequestComboboxChangedCommand = new AsyncRelayCommand<OrderDetailUIModel>(CreditRequestComboboxChanged);
            IsAllUpperGridSelectedCommand = new RelayCommand(IsAllUpperGridRowsSelected);
            IsAllUpperGridUnselectedCommand = new RelayCommand(IsAllUpperGridRowsUnselected);
            IsAllLowerGridSelectedCommand = new RelayCommand(IsAllLowerGridRowsSelected);
            IsAllLowerGridUnselectedCommand = new RelayCommand(IsAllLowerGridRowsUnselected);
            EmailFactSheetCommad = new AsyncRelayCommand(EmailFactSheetCommadHandler);
            PlaceAnOderButtonCommand = new RelayCommand(NavigateToRetailTransaction);
            NumPadButtonClickCommand = new AsyncRelayCommand<string>(NumPadButtonClicked);
        }

        private async Task UnitComboboxChanged(OrderDetailUIModel orderDetailUIModel)
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

                                await SaveValuesInDatabase(item);

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

                                await SaveValuesInDatabase(item);

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

                                await SaveValuesInDatabase(item);

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

                            await SaveValuesInDatabase(item);

                            break;
                        }
                    }
                }
            }
        }

        private async Task CreditRequestComboboxChanged(OrderDetailUIModel orderDetailUIModel)
        {
            if (RtnGridDataSource?.Count > 0)
            {
                foreach (var item in RtnGridDataSource)
                {
                    if (item.ProductID == orderDetailUIModel.ProductID)
                    {
                        item.CreditRequest = orderDetailUIModel.CreditRequest;

                        await SaveValuesInDatabase(item);

                        if (item.CreditRequest.Contains("DIF"))
                        {
                            RtnGridDataSource.Remove(item);
                            DifGridDataSource.Add(item);
                        }

                        orderDetailUIModel = null;

                        break;
                    }
                }
            }

            if (orderDetailUIModel != null && DifGridDataSource?.Count > 0)
            {
                foreach (var item in DifGridDataSource)
                {
                    if (item.ProductID == orderDetailUIModel.ProductID)
                    {
                        item.CreditRequest = orderDetailUIModel.CreditRequest;

                        await SaveValuesInDatabase(item);

                        if (item.CreditRequest.Contains("RTN"))
                        {
                            RtnGridDataSource.Add(item);
                            DifGridDataSource.Remove(item);
                        }

                        break;
                    }
                }
            }

            CartDetailModel.RtnGridVisibility = (RtnGridDataSource?.Count > 0) ? Visibility.Visible : Visibility.Collapsed;
            CartDetailModel.DifGridVisibility = (DifGridDataSource?.Count > 0) ? Visibility.Visible : Visibility.Collapsed;
        }

        private async Task QuantityChangedAsync(OrderDetailUIModel orderDetailUIModel)
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
                                item.Quantity = orderDetailUIModel.Quantity;

                                CalculateSubTotalForUperGrid();

                                orderDetailUIModel.QuantityDisplay = (item.Quantity * -1).ToString();

                               await SaveValuesInDatabase(item);

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
                                item.Quantity = orderDetailUIModel.Quantity;

                                CalculateSubTotalForLowerGrid();

                                orderDetailUIModel.QuantityDisplay = (item.Quantity * -1).ToString();

                                await SaveValuesInDatabase(item);

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

                            CalculateSubTotalForUperGrid();

                            await SaveValuesInDatabase(item);

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

                            CalculateSubTotalForLowerGrid();

                            await SaveValuesInDatabase(item);

                            break;
                        }
                    }
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

        private async Task PriceChangedAsync(OrderDetailUIModel orderDetailUIModel)
        {
            if (orderDetailUIModel.isTobbaco == 1)
            {
                if ((bool)AppRef.IsCreditRequestOrder)
                {
                    foreach (var item in RtnGridDataSource)
                    {
                        if (item.ProductID == orderDetailUIModel.ProductID)
                        {
                            item.Price = orderDetailUIModel.Price;

                            CalculateSubTotalForUperGrid();

                            await SaveValuesInDatabase(item);

                            break;
                        }
                    }
                }
                else
                {
                    foreach (var item in TobaccoGridDataSource)
                    {
                        if (item.ProductID == orderDetailUIModel.ProductID)
                        {
                            item.Price = orderDetailUIModel.Price;

                            CalculateSubTotalForUperGrid();

                            await SaveValuesInDatabase(item);

                            break;
                        }
                    }
                }           
            }
            else
            {
                if ((bool)AppRef.IsCreditRequestOrder)
                {
                    foreach (var item in DifGridDataSource)
                    {
                        if (item.ProductID == orderDetailUIModel.ProductID)
                        {
                            item.Price = orderDetailUIModel.Price;

                            CalculateSubTotalForLowerGrid();

                            await SaveValuesInDatabase(item);

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
                            item.Price = orderDetailUIModel.Price;

                            CalculateSubTotalForLowerGrid();

                            await SaveValuesInDatabase(item);

                            break;
                        }
                    }
                }
            }
        }

        private async Task SetCustomerInformationAndGetCartData()
        {
            int customerId = Convert.ToInt32(AppRef.SelectedCustomerId);

            CartDetailModel = await ((App)Application.Current).QueryService.GetCartDetailsDataForCartScreen(AppRef.CurrentOrderId, customerId);

            if (CartDetailModel.OrderDetailsList != null && CartDetailModel.OrderDetailsList.Count > 0)
            {
                if ((bool)AppRef.IsCreditRequestOrder)
                {
                    RtnGridDataSource = new ObservableCollection<OrderDetailUIModel>();
                    DifGridDataSource = new ObservableCollection<OrderDetailUIModel>();

                    SetRtnDifGridViews();
                }
                else
                {
                    TobaccoGridDataSource = new ObservableCollection<OrderDetailUIModel>();
                    NonTobaccoGridDataSource = new ObservableCollection<OrderDetailUIModel>();

                    SetTobaccoNonTobaccoGrid();
                }

                if (CartDetailModel.CustomerData.AccountType == 2)
                {
                    PurchaseOrderButtonText = "Retail Transaction";
                }
                else if (CartDetailModel.CustomerData.AccountType != 2 && (bool)AppRef.IsCreditRequestOrder)
                {
                    PurchaseOrderButtonText = "Credit Request";
                }
                else
                {
                    PurchaseOrderButtonText = "Purchase Order";
                }
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

            if ((bool)AppRef.IsOrderTypeChanged)
            {
                foreach (var orderDetailItem in CartDetailModel.OrderDetailsList)
                {
                    if (orderDetailItem.Quantity < 0)
                    {
                        orderDetailItem.Quantity *= -1;
                    }

                    orderDetailItem.Price = 0;
                    orderDetailItem.Total = "0";
                    orderDetailItem.CreditRequest = "";
                    orderDetailItem.UpdatedDate = DateTimeHelper.ConvertToDbInsertDateTimeFormat(DateTime.Now);

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

            var tobaccoList = CartDetailModel.OrderDetailsList.Where(x => x.isTobbaco == 1).ToList();
            tobaccoList.ForEach(x => TobaccoGridDataSource.Add(x));

            var nonTobaccoList = CartDetailModel.OrderDetailsList.Where(x => x.isTobbaco == 0).ToList();
            nonTobaccoList.ForEach(x => NonTobaccoGridDataSource.Add(x));

            CartDetailModel.TobaccoGridVisibility = (tobaccoList.Count > 0) ? Visibility.Visible : Visibility.Collapsed;
            CartDetailModel.NonTobaccoGridVisibility = (nonTobaccoList.Count > 0) ? Visibility.Visible : Visibility.Collapsed;

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

            if ((bool)AppRef.IsOrderTypeChanged)
            {
                foreach (var orderDetailItem in CartDetailModel.OrderDetailsList)
                {
                    if (orderDetailItem.Quantity > 0)
                    {
                        orderDetailItem.Quantity *= -1;
                    }
                    else if (orderDetailItem.Quantity == 0)
                    {
                        orderDetailItem.Quantity = -1;
                    }

                    orderDetailItem.QuantityDisplay = orderDetailItem.Quantity.ToString();
                    orderDetailItem.Price = 0;
                    orderDetailItem.Total = "0";
                    orderDetailItem.Unit = orderDetailItem.CategoryId == 1 || orderDetailItem.CategoryId == 6 ? "BX" : "EA";
                    orderDetailItem.CreditRequest = orderDetailItem.CategoryId == 6 ? "DIF-Destroyed" : "RTN-Retail Returns";
                }
            }

            var rtnList = CartDetailModel.OrderDetailsList.Where(x => x.CreditRequest.Contains("RTN")).ToList();

            foreach (var item in rtnList)
            {
                if (item.isTobbaco != 1)
                {
                    SetRtnCreditRequestCombobox();
                }
                else
                {
                    if (CartDetailModel.OrderDetailModel.RtnDifCreditRequestComboboxList != null && CartDetailModel.OrderDetailModel.RtnDifCreditRequestComboboxList.Count > 0)
                    {
                        CartDetailModel.OrderDetailModel.RtnDifCreditRequestComboboxList.RemoveAt(0);
                    }
                }

                RtnGridDataSource.Add(item);
            }

            var difList = CartDetailModel.OrderDetailsList.Where(x => x.CreditRequest.Contains("DIF")).ToList();

            foreach (var item in difList)
            {
                SetRtnCreditRequestCombobox();

                DifGridDataSource.Add(item);
            }

            CartDetailModel.RtnGridVisibility = (rtnList.Count > 0) ? Visibility.Visible : Visibility.Collapsed;
            CartDetailModel.DifGridVisibility = (difList.Count > 0) ? Visibility.Visible : Visibility.Collapsed;

            CartDetailModel.BottomButtonVisiblity = string.IsNullOrEmpty(AppRef.SelectedCustomerId) ? Visibility.Collapsed : Visibility.Visible;

            CartDetailModel.UperGridSubTotal = 0;
            CartDetailModel.LowerGridSubTotal = 0;
            CartDetailModel.OrderDetailModel.SubTotalUpperDisplay = "0";
            CartDetailModel.OrderDetailModel.SubTotalLowerDisplay = "0";

            CalculateGrandTotal(CartDetailModel.UperGridSubTotal, CartDetailModel.LowerGridSubTotal);
        }

        private void CalculateSubTotalGrandTotalOnLoad()
        {
            if (TobaccoGridDataSource?.Count > 0)
            {
                foreach (var item in TobaccoGridDataSource)
                {
                    int total = GetDisplayValues(item);
                    CartDetailModel.UperGridSubTotal += total;
                }
            }

            CartDetailModel.OrderDetailModel.SubTotalUpperDisplay = FormatStringToTwoDecimal(CartDetailModel.UperGridSubTotal);

            if (NonTobaccoGridDataSource?.Count > 0)
            {
                foreach (var item in NonTobaccoGridDataSource)
                {
                    int total = GetDisplayValues(item);
                    CartDetailModel.LowerGridSubTotal += total;
                }
            }

            CartDetailModel.OrderDetailModel.SubTotalLowerDisplay = FormatStringToTwoDecimal(CartDetailModel.LowerGridSubTotal);

            CalculateGrandTotal(CartDetailModel.UperGridSubTotal, CartDetailModel.LowerGridSubTotal);
        }

        private async Task CalculateSubTotalGrandTotal()
        {
            int subTotalUpperGrid = 0;
            int subTotalLowerGrid = 0;
            if (TobaccoGridDataSource?.Count > 0)
            {
                foreach (var item in TobaccoGridDataSource)
                {
                    if (item != null)
                    {
                        int total = GetDisplayValues(item);
                        await SaveValuesInDatabase(item);
                        subTotalUpperGrid += total;
                    }
                }
            }
            CartDetailModel.OrderDetailModel.SubTotalUpperDisplay = FormatStringToTwoDecimal(subTotalUpperGrid);
            if (NonTobaccoGridDataSource?.Count > 0)
            {
                foreach (var item in NonTobaccoGridDataSource)
                {
                    if (item != null)
                    {
                        int total = GetDisplayValues(item);
                        await SaveValuesInDatabase(item);
                        subTotalLowerGrid = subTotalLowerGrid + total;
                    }
                }
            }
            CartDetailModel.OrderDetailModel.SubTotalLowerDisplay = FormatStringToTwoDecimal(subTotalLowerGrid);
            int grandTotal = subTotalLowerGrid + subTotalUpperGrid;
            CartDetailModel.OrderDetailModel.GrandTotalDisplay = FormatStringToTwoDecimal(grandTotal);
        }

        private void CalculateSubTotalForUperGrid()
        {
            CartDetailModel.UperGridSubTotal = 0;

            if ((bool)AppRef.IsCreditRequestOrder)
            {
                foreach (var item in RtnGridDataSource)
                {
                    int total = GetDisplayValues(item);
                    CartDetailModel.UperGridSubTotal += total;
                }
            }
            else
            {
                foreach (var item in TobaccoGridDataSource)
                {
                    int total = GetDisplayValues(item);
                    CartDetailModel.UperGridSubTotal += total;
                }
            }              

            CartDetailModel.OrderDetailModel.SubTotalUpperDisplay = FormatStringToTwoDecimal(CartDetailModel.UperGridSubTotal);

            CalculateGrandTotal(CartDetailModel.UperGridSubTotal, CartDetailModel.LowerGridSubTotal);
        }

        private void CalculateSubTotalForLowerGrid()
        {
            CartDetailModel.LowerGridSubTotal = 0;

            if ((bool)AppRef.IsCreditRequestOrder)
            {
                foreach (var item in DifGridDataSource)
                {
                    int total = GetDisplayValues(item);
                    CartDetailModel.LowerGridSubTotal += total;
                }
            }
            else
            {
                foreach (var item in NonTobaccoGridDataSource)
                {
                    int total = GetDisplayValues(item);
                    CartDetailModel.LowerGridSubTotal += total;
                }
            }         

            CartDetailModel.OrderDetailModel.SubTotalLowerDisplay = FormatStringToTwoDecimal(CartDetailModel.LowerGridSubTotal);

            CalculateGrandTotal(CartDetailModel.UperGridSubTotal, CartDetailModel.LowerGridSubTotal);
        }

        private void CalculateGrandTotal(int uperGridTotal, int lowerGridTotal)
        {
            int grandTotal = uperGridTotal + lowerGridTotal;

            CartDetailModel.OrderDetailModel.GrandTotalDisplay = FormatStringToTwoDecimal(grandTotal);
        }

        private string FormatStringToTwoDecimal(int subTotalUpperGrid)
        {
            var formattedString = "$" + string.Format("{0:00.00}", subTotalUpperGrid);
            return formattedString;
        }

        private int GetDisplayValues(OrderDetailUIModel item)
        {
            var total = item.Price * item.Quantity;
            if (item.IsQuantityEdited && string.IsNullOrEmpty(item.QuantityDisplay))
            {
                item.QuantityDisplay = string.Empty;
                item.IsQuantityEdited = false;
            }
            else if (string.IsNullOrEmpty(item.QuantityDisplay))
            {
                item.QuantityDisplay = item.Quantity.ToString();
            }
            item.Total = string.Format("{0:00.00}", total);
            item.TotalDisplay = FormatStringToTwoDecimal(total);
            if (!item.IsPriceEdited)
            {
                item.PriceDisplay = string.Format("{0:00.00}", item.Price);
            }
            else
            {
                item.IsPriceEdited = false;
            }
            return total;
        }

        private void SetRtnCreditRequestCombobox()
        {
            List<string> RtnCreditRequestOptions = new List<string>();

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

            CartDetailModel.OrderDetailModel.RtnDifCreditRequestComboboxList = new ObservableCollection<string>();

            RtnCreditRequestOptions.ForEach(x => CartDetailModel.OrderDetailModel.RtnDifCreditRequestComboboxList.Add(x));
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
                    CartDetailModel.RtnGridVisibility = (RtnGridDataSource.Count > 0) ? Visibility.Visible : Visibility.Collapsed;
                }
                else
                {
                    DifGridDataSource.Remove(orderDetailUIModel);
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

            await ((App)Application.Current).QueryService.DeleteOrderDetail(_list[0].ProductId, AppRef.CurrentOrderId);
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
                var selectedTobaccoProducts = TobaccoGridDataSource.Where(x => x.IsAllUpperGridSelected);
                var selectedNonTobaccoProducts = NonTobaccoGridDataSource.Where(x => x.IsAllLowerGridSelected);

                var total = selectedTobaccoProducts.Count() + selectedNonTobaccoProducts.Count();

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

                        await EmailService.Instance.SendMailFromOutlook(new EmailModel() { Subject = string.Join(',', listOfProductNumber),
                            AttachmentListByPath = listOfFilePath });

                        await Task.Delay(1000);

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

        private void NavigateToRetailTransaction()
        {
            NavigationService.Navigate<RetailTransactionPage>();
        }

        private async Task<string> GetAdditionalDocumentsUrl(int productId)
        {
            var productDetailUiModel = await ((App)Application.Current).QueryService.GetProductAdditionalDocumentData(productId);

            var returnString = productDetailUiModel != null ? productDetailUiModel.Factsheet : string.Empty;

            return returnString;
        }

        private async Task NumPadButtonClicked(string value)
        {
            if (isOpenedFrom == "quantity")
            {
                if ((bool)AppRef.IsCreditRequestOrder)
                {
                    if (IsOpenedForCRQuantity.Equals("RtnQuantity") && RtnGridDataSource?.Count > 0)
                    {
                        UpdateCreditRequestQuantityFieldValue(RtnGridDataSource, value);
                    }
                    else if (IsOpenedForCRQuantity.Equals("DifQuantity") && DifGridDataSource?.Count > 0)
                    {
                        UpdateCreditRequestQuantityFieldValue(DifGridDataSource, value);
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

            await CalculateSubTotalGrandTotal();
        }

        private void UpdatePriceFieldValue(ObservableCollection<OrderDetailUIModel> gridDataSource, string value)
        {
            foreach (var item in gridDataSource)
            {
                if (item.ProductID == CartDetailModel.OrderDetailModel.ProductID)
                {
                    priceBeforeEdit = CartDetailModel.OrderDetailModel.PriceDisplay;

                    if (value.Equals("C"))
                    {
                        if (item.PriceDisplay.Length >= 0)
                        {
                            item.PriceDisplay = item.PriceDisplay.Remove(item.PriceDisplay.Length - 1);
                            priceString = item.PriceDisplay;
                            item.IsPriceEdited = true;
                        }
                    }
                    else
                    {
                        item.IsPriceEdited = true;

                        if (!priceString.Contains("."))
                        {
                            if (value != ".")
                            {
                                if (priceString.Length < 4)
                                {
                                    double temp;
                                    priceString += value;
                                    var price = Convert.ToInt32(priceString);
                                    item.PriceDisplay = string.Format("{0:0.00}", price);
                                    bool isOk = double.TryParse(item.PriceDisplay, out temp);
                                    int data = isOk ? (int)temp : 0;
                                    if (isOk)
                                    {
                                        item.Price = data;
                                    }
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
                                    double temp;
                                    bool isOk = double.TryParse(priceString, out temp);
                                    int data = isOk ? (int)temp : 0;
                                    if (isOk)
                                    {
                                        item.Price = data;
                                        item.PriceDisplay = priceString;
                                    }
                                }
                                else if (tail.Length == 1)
                                {
                                    priceString += value;
                                    double temp;
                                    bool isOk = double.TryParse(item.PriceDisplay, out temp);
                                    int data = isOk ? (int)temp : 0;
                                    if (isOk)
                                    {
                                        item.Price = data;
                                        item.PriceDisplay = priceString;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void UpdateQuantityFieldValue(ObservableCollection<OrderDetailUIModel> gridDataSource, string value)
        {
            foreach (var item in gridDataSource)
            {
                if (item.ProductID == CartDetailModel.OrderDetailModel?.ProductID)
                {
                    if (value.Equals("C"))
                    {
                        if (item?.QuantityDisplay?.Length >= 0)
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
                                item.QuantityDisplay = CartDetailModel.OrderDetailModel?.QuantityDisplay + value;
                            }
                            else
                            {
                                item.QuantityDisplay = string.Empty;
                                item.IsQuantityEdited = true;
                            }
                        }
                        else if (item.QuantityDisplay.Length < 4 && (value != "-"))
                        {
                                item.QuantityDisplay = CartDetailModel.OrderDetailModel?.QuantityDisplay + value;
                        }
                    }
                    if (!string.IsNullOrEmpty(item?.QuantityDisplay))
                    {
                        item.Quantity = Convert.ToInt32(item?.QuantityDisplay);
                    }
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

                    if (value.Equals("C"))
                    {
                        if (item.PriceDisplay.Length >= 0)
                        {
                            item.PriceDisplay = item.PriceDisplay.Remove(item.PriceDisplay.Length - 1);
                            priceString = item.PriceDisplay;
                            item.IsPriceEdited = true;
                        }
                    }
                    else
                    {
                        item.IsPriceEdited = true;

                        if (!priceString.Contains("."))
                        {
                            if (value != ".")
                            {
                                if (priceString.Length < 5)
                                {
                                    double temp;
                                    priceString += value;
                                    var price = Convert.ToInt32(priceString);
                                    item.PriceDisplay = string.Format("{0:0.00}", price);
                                    bool isOk = double.TryParse(item.PriceDisplay, out temp);
                                    int data = isOk ? (int)temp : 0;
                                    if (isOk)
                                    {
                                        item.Price = data;
                                    }
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
                                    double temp;
                                    bool isOk = double.TryParse(priceString, out temp);
                                    int data = isOk ? (int)temp : 0;
                                    if (isOk)
                                    {
                                        item.Price = data;
                                        item.PriceDisplay = priceString;
                                    }
                                }
                                else if (tail.Length == 1)
                                {
                                    priceString += value;
                                    double temp;
                                    bool isOk = double.TryParse(item.PriceDisplay, out temp);
                                    int data = isOk ? (int)temp : 0;
                                    if (isOk)
                                    {
                                        item.Price = data;
                                        item.PriceDisplay = priceString;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void UpdateCreditRequestQuantityFieldValue(ObservableCollection<OrderDetailUIModel> gridDataSource, string value)
        {
            foreach (var item in gridDataSource)
            {
                if (item.ProductID == CartDetailModel.OrderDetailModel?.ProductID)
                {
                    if (value.Equals("C"))
                    {
                        if (item?.QuantityDisplay?.Length >= 0)
                        {
                            item.QuantityDisplay = item?.QuantityDisplay?.Remove(item.QuantityDisplay.Length - 1);
                            item.IsQuantityEdited = true;
                        }
                    }
                    else
                    {
                        if (item?.QuantityDisplay?.Length == 0)
                        {
                            if (value != "0")
                            {
                                item.QuantityDisplay = CartDetailModel.OrderDetailModel?.QuantityDisplay + value;
                            }
                            else
                            {
                                item.QuantityDisplay = string.Empty;
                                item.IsQuantityEdited = true;
                            }
                        }
                        else if (item.QuantityDisplay.Length < 5)
                        {
                            item.QuantityDisplay = CartDetailModel.OrderDetailModel?.QuantityDisplay + value;
                        }
                    }
                    if (!string.IsNullOrEmpty(item?.QuantityDisplay))
                    {
                        item.Quantity = Convert.ToInt32(item?.QuantityDisplay);
                    }
                }
            }
        }

        #endregion
    }
}
