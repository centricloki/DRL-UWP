using DRLMobile.Core.Enums;
using DRLMobile.Core.Models;
using DRLMobile.Core.Models.DataModels;
using DRLMobile.Core.Models.UIModels;
using DRLMobile.ExceptionHandler;
using DRLMobile.Helpers;
using DRLMobile.Services;
using DRLMobile.Views;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;

namespace DRLMobile.ViewModels
{
    public class OrderHistoryPageViewModel : BaseModel
    {

        private readonly App AppReference = (App)(Application.Current);
        private bool IsFirstLoad { get; set; }
        public bool IsReorderTapped { get; set; }

        #region Properties

        private CustomerMaster _selectedCustomer;
        public CustomerMaster SelectedCustomer
        {
            get { return _selectedCustomer; }
            set { SetProperty(ref _selectedCustomer, value); }
        }

        private string _titleText;
        public string TitleText
        {
            get { return _titleText; }
            set { SetProperty(ref _titleText, value); }
        }


        private Segment _selectedSegment;
        public Segment SelectedSegment
        {
            get { return _selectedSegment; }
            set { SetProperty(ref _selectedSegment, value); }
        }


        public Segment CurrentSegment { get; set; }


        private ObservableCollection<OrderHistoryUIModel> _orderHistoryItemSource;
        public ObservableCollection<OrderHistoryUIModel> OrderHistoryItemSource
        {
            get { return _orderHistoryItemSource; }
            set { SetProperty(ref _orderHistoryItemSource, value); }
        }


        private ObservableCollection<ItemHistoryUIModel> _itemHistoryItemSource;
        public ObservableCollection<ItemHistoryUIModel> ItemHistoryItemSource
        {
            get { return _itemHistoryItemSource; }
            set { SetProperty(ref _itemHistoryItemSource, value); }
        }

        private bool _isOrderHistoryVisibile;
        public bool IsOrderHistoryVisibile
        {
            get { return _isOrderHistoryVisibile; }
            set { SetProperty(ref _isOrderHistoryVisibile, value); }
        }

        private bool _isItemHistoryVisibile;
        public bool IsItemHistoryVisibile
        {
            get { return _isItemHistoryVisibile; }
            set { SetProperty(ref _isItemHistoryVisibile, value); }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set { SetProperty(ref _isLoading, value); }
        }

        private ObservableCollection<object> _headerSearchItemSource;
        public ObservableCollection<object> HeaderSearchItemSource
        {
            get { return _headerSearchItemSource; }
            set { SetProperty(ref _headerSearchItemSource, value); }
        }


        private string _autoSuggestionText;
        public string AutoSuggestionText
        {
            get { return _autoSuggestionText; }
            set { SetProperty(ref _autoSuggestionText, value); }
        }


        private bool _isNoHistoryPlaceHolderVisible;
        public bool IsNoHistoryPlaceHolderVisible
        {
            get { return _isNoHistoryPlaceHolderVisible; }
            set { SetProperty(ref _isNoHistoryPlaceHolderVisible, value); }
        }


        public IEnumerable<OrderHistoryUIModel> DBOrderHistorySource { get; set; }

        public IEnumerable<ItemHistoryUIModel> DBItemHistorySource { get; set; }

        #endregion



        #region Command
        public ICommand OnNavigatedToCommand { get; private set; }
        public ICommand LeftSegmentCommand { get; private set; }
        public ICommand RightSegmentCommand { get; private set; }
        public ICommand HeaderSearchTextChangeCommand { private set; get; }
        public ICommand HeaderSearchSuggestionChoosenCommand { private set; get; }
        public ICommand ItemClickCommand { get; private set; }
        #endregion



        #region constructor
        public OrderHistoryPageViewModel()
        {
            OnNavigatedToCommand = new AsyncRelayCommand<object>(OnNavigatedToCommandHandler);
            LeftSegmentCommand = new RelayCommand(LeftSegmentCommandHandler);
            RightSegmentCommand = new AsyncRelayCommand(RightSegmentCommandHandler);
            HeaderSearchTextChangeCommand = new RelayCommand<string>(HeaderSearchTextChangeCommandHandler);
            HeaderSearchSuggestionChoosenCommand = new RelayCommand<object>(HeaderSearchSuggestionChoosenCommandHandler);
            ItemClickCommand = new AsyncRelayCommand<OrderHistoryUIModel>(ItemClickCommandHandler);
            IsFirstLoad = true;
            HeaderSearchItemSource = new ObservableCollection<object>();
            DBOrderHistorySource = new List<OrderHistoryUIModel>();
            DBItemHistorySource = new List<ItemHistoryUIModel>();
            OrderHistoryItemSource = new ObservableCollection<OrderHistoryUIModel>();
            ItemHistoryItemSource = new ObservableCollection<ItemHistoryUIModel>();
            IsNoHistoryPlaceHolderVisible = false;
            IsOrderHistoryVisibile = false;
            IsItemHistoryVisibile = false;
        }




        #endregion


        #region Private method

        private async Task OnNavigatedToCommandHandler(object parameter)
        {
            try
            {
                if (parameter is CustomerMaster)
                {
                    var selectedCustomer = parameter as CustomerMaster;
                    IsNoHistoryPlaceHolderVisible = false;
                    IsOrderHistoryVisibile = false;
                    IsItemHistoryVisibile = false;
                    AutoSuggestionText = string.Empty;
                    HeaderSearchItemSource.Clear();
                    OrderHistoryItemSource?.Clear();
                    ItemHistoryItemSource?.Clear();
                    DBItemHistorySource = new List<ItemHistoryUIModel>();
                    DBOrderHistorySource = new List<OrderHistoryUIModel>();
                    IsFirstLoad = true;
                    SelectedCustomer = selectedCustomer;
                    SelectedSegment = Segment.None;
                    SelectedSegment = Segment.Left;
                    IsOrderHistoryVisibile = true;
                    await GetTheOrderHistoryData();
                    IsFirstLoad = false;
                    CurrentSegment = Segment.Left;
                }
                else if (parameter is OrderDetailUpdatedModel)
                {
                    var order = parameter as OrderDetailUpdatedModel;
                    if (OrderHistoryItemSource.Any(x => x.OrderId == order.OrderId))
                    {
                        OrderHistoryItemSource.FirstOrDefault(x => x.OrderId == order.OrderId).TotalAmount = string.Format("${0}", order.Price.ToString("0.00"));
                        OrderHistoryItemSource.FirstOrDefault(x => x.OrderId == order.OrderId).TotalQuantity = order.Qty.ToString();
                    }
                    OrderHistoryPage.UpdatedOrder = null;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(OrderHistoryPageViewModel), nameof(OnNavigatedToCommandHandler), ex.StackTrace);
                IsLoading = false;
                IsFirstLoad = false;
            }
        }

        private async Task ItemClickCommandHandler(OrderHistoryUIModel selectedObject)
        {
            if (!IsReorderTapped)
            {
                var navObj = new OrderHistoryDetailsPageUIModel() { SelectedCustomer = SelectedCustomer, OrderHistoryModel = selectedObject };
                NavigationService.Navigate<OrderHistoryDetailsPage>(navObj);
            }
            else
            {
                IsLoading = true;
                await ReorderClicked(selectedObject);
                IsLoading = false;
            }
            IsReorderTapped = false;
        }

        private async Task ReorderClicked(OrderHistoryUIModel selectedObject)
        {
            //checking the order
            var result = await AlertHelper.Instance.ShowConfirmationAlert("Alert", "If you have items in the cart and If you proceed, those items and the customer will be removed and replaced by this order", "OK", "Cancel");
            if(result)
            {
                try
                {
                    if ("8".Equals(selectedObject.SalesType))
                    {
                        AppReference.IsCreditRequestOrder = true;
                    }
                    else
                    {
                        AppReference.IsCreditRequestOrder = false;

                    }

                    if (AppReference.CurrentOrderId == 0)
                    {
                        await InsertEntryIntoOrderMaster();
                    }
                    else
                    {
                        var details = await AppReference.QueryService.GetOrderDetailsFromOrderId(selectedObject.OrderId);

                        await AppReference.QueryService.DeleteCartDetailOnPlaceOrder(AppReference.CurrentOrderId);

                        foreach (var item in details)
                        {
                            item.OrderId = AppReference.CurrentOrderId;
                            item.DeviceOrderID = AppReference.CurrentOrderId.ToString();
                            item.OrderDetailId = null;
                            await AppReference.QueryService.InsertOrUpdateOrderDetail(item);
                        }
                        AppReference.CartItemCount = details.Count();
                        AppReference.SelectedCustomerId = SelectedCustomer.CustomerID.ToString();
                        NavigationService.Navigate(typeof(CartPage));
                    }
                }
                catch (Exception ex)
                {
                    ErrorLogger.WriteToErrorLog(nameof(OrderHistoryPageViewModel), nameof(ReorderClicked), ex.StackTrace);
                    IsReorderTapped = false;
                    IsLoading = false;
                }
            }
        }

        private async Task InsertEntryIntoOrderMaster()
        {
            var orderMasterRecord = new OrderMaster();

            orderMasterRecord.IsExported = 1;

            var newOrderAdded = await ((App)Application.Current).QueryService.InsertOrUpdateOrderMaster(orderMasterRecord);

            if (newOrderAdded != null)
            {
                ((App)Application.Current).CurrentOrderId = newOrderAdded.OrderID;
            }
        }

        private async Task GetItemHistoryData()
        {
            IsLoading = true;
            DBItemHistorySource = await AppReference.QueryService.GetItemHistoryDataAsync(SelectedCustomer.CustomerID.Value);
            foreach (var item in DBItemHistorySource)
            {
                ItemHistoryItemSource.Add(item);
            }
            IsLoading = false;
        }

        private async Task GetTheOrderHistoryData()
        {
            IsLoading = true;
            IsItemHistoryVisibile = false;
            DBOrderHistorySource = await AppReference.QueryService.GetOrderHistoryDataAsync(SelectedCustomer.CustomerID.Value);

            foreach (var item in DBOrderHistorySource)
            {
                OrderHistoryItemSource.Add(item);
            }
            OrderHistoryVisibility();
            IsLoading = false;
        }

        private void OrderHistoryVisibility()
        {
            IsNoHistoryPlaceHolderVisible = false;
            IsItemHistoryVisibile = false;
            if (OrderHistoryItemSource == null || OrderHistoryItemSource.Count == 0)
            {
                IsOrderHistoryVisibile = false;
                IsNoHistoryPlaceHolderVisible = true;
            }
            else
            {
                IsOrderHistoryVisibile = true;
                IsNoHistoryPlaceHolderVisible = false;
            }
        }

        private void ItemHistoryVisibility()
        {
            IsOrderHistoryVisibile = false;
            if (ItemHistoryItemSource.Any())
            {
                IsItemHistoryVisibile = true;
                IsNoHistoryPlaceHolderVisible = false;
            }
            else
            {
                IsItemHistoryVisibile = false;
                IsNoHistoryPlaceHolderVisible = true;
            }
        }

        private async Task RightSegmentCommandHandler()
        {
            HeaderSearchItemSource = new ObservableCollection<object>();
            CurrentSegment = Segment.Right;
            AutoSuggestionText = string.Empty;
            TitleText = ResourceExtensions.GetLocalized("ITEM_HISTORY");
            if (DBItemHistorySource == null || !DBItemHistorySource.Any())
            {
                await GetItemHistoryData();
            }
            Parallel.Invoke(() =>
            {
                ItemHistoryItemSource = new ObservableCollection<ItemHistoryUIModel>(DBItemHistorySource);
                ItemHistoryVisibility();
            });
        }

        private void LeftSegmentCommandHandler()
        {
            try
            {
                HeaderSearchItemSource = new ObservableCollection<object>();
                CurrentSegment = Segment.Left;
                AutoSuggestionText = string.Empty;
                TitleText = ResourceExtensions.GetLocalized("ORDER_HISTORY");
                OrderHistoryVisibility();
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(OrderHistoryPageViewModel), nameof(LeftSegmentCommandHandler), ex.StackTrace);
                IsLoading = false;
            }
        }


        private void HeaderSearchTextChangeCommandHandler(string text)
        {
            if (CurrentSegment == Segment.Left)
            {
                HandleHeaderTextChangeForLeftSegment(text);
            }
            else
            {
                HandleHeaderTextChangeForRightSegment(text);
            }

        }

        private void HandleHeaderTextChangeForRightSegment(string text)
        {
            try
            {
                HeaderSearchItemSource.Clear();
                if (string.IsNullOrWhiteSpace(text))
                {
                    if (DBItemHistorySource.Count() != ItemHistoryItemSource.Count())
                        ItemHistoryItemSource = new ObservableCollection<ItemHistoryUIModel>(DBItemHistorySource);
                }
                else
                {
                    var tempList = DBItemHistorySource.Where(x => x.SearchDisplayPath.ToLower().Contains(text.ToLower())).ToList();
                    if (tempList == null || tempList.Count == 0)
                    {
                        HeaderSearchItemSource.Add(new ItemHistoryUIModel() { Invoice = ResourceExtensions.GetLocalized("NoResultsErrorMessage") });
                    }
                    else
                    {
                        tempList.ForEach(x => HeaderSearchItemSource.Add(x));
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(OrderHistoryPageViewModel), nameof(HandleHeaderTextChangeForLeftSegment), ex.Message);
            }
        }

        private void HandleHeaderTextChangeForLeftSegment(string text)
        {
            try
            {
                HeaderSearchItemSource.Clear();
                if (string.IsNullOrWhiteSpace(text))
                {
                    if (DBOrderHistorySource.Count() != OrderHistoryItemSource.Count())
                    {
                        OrderHistoryItemSource = new ObservableCollection<OrderHistoryUIModel>(DBOrderHistorySource);
                    }
                }
                else
                {
                    var tempList = DBOrderHistorySource.Where(x => x.SearchDisplayPath.ToLower().Contains(text.ToLower())).ToList();
                    if (tempList == null || tempList.Count == 0)
                    {
                        HeaderSearchItemSource.Add(new OrderHistoryUIModel() { Invoice = ResourceExtensions.GetLocalized("NoResultsErrorMessage") });
                    }
                    else
                    {
                        tempList.ForEach(x => HeaderSearchItemSource.Add(x));
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(OrderHistoryPageViewModel), nameof(HandleHeaderTextChangeForLeftSegment), ex.Message);
            }
        }

        private void HeaderSearchSuggestionChoosenCommandHandler(object selectedItem)
        {
            if (selectedItem is OrderHistoryUIModel)
            {
                var item = selectedItem as OrderHistoryUIModel;
                if (item.SearchDisplayPath.Contains(ResourceExtensions.GetLocalized("NoResultsErrorMessage")))
                    return;
                HeaderSearchItemSource.Clear();
                OrderHistoryItemSource.Clear();
                OrderHistoryItemSource.Add(item);
            }
            else if (selectedItem is ItemHistoryUIModel)
            {
                var item = selectedItem as ItemHistoryUIModel;
                if (item.SearchDisplayPath.Contains(ResourceExtensions.GetLocalized("NoResultsErrorMessage")))
                    return;
                HeaderSearchItemSource.Clear();
                ItemHistoryItemSource.Clear();
                ItemHistoryItemSource.Add(item);
            }
        }



        #endregion





    }
}
