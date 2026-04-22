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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace DRLMobile.ViewModels
{
    public class OrderHistoryDetailsPageViewModel : BaseModel
    {
        private readonly App AppReference = (App)(Application.Current);
        public string IsOpenedFrom { get; set; }

        #region command
        public ICommand PriceChangedCommand { get; private set; }
        public ICommand NumPadButtonClickCommand { get; private set; }
        public ICommand OnNavigatedToCommand { get; private set; }
        public ICommand EditSaveCommand { get; private set; }
        public ICommand ClosePopupCommand { get; private set; }
        public ICommand AddButtonCommand { get; private set; }
        public ICommand AutoSuggestTextChanged { get; private set; }
        public ICommand AutoSuggestSuggestionChoosen { get; private set; }
        public ICommand QuantityChangedCommand { get; private set; }
        public ICommand SaveDeleteCommand { get; private set; }
        public ICommand ItemClickCommand { get; private set; }
        public ICommand PrintEmailCommand { get; set; }
        #endregion


        #region properties

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set { SetProperty(ref _isLoading, value); }
        }


        private bool _loadTobaccoGrid;
        public bool LoadTobaccoGrid
        {
            get { return _loadTobaccoGrid; }
            set { SetProperty(ref _loadTobaccoGrid, value); }
        }

        private bool _isAdd;
        public bool IsAdd
        {
            get { return _isAdd; }
            set { SetProperty(ref _isAdd, value); }
        }

        private bool _loadNonTobaccoGrid;
        public bool LoadNonTobaccoGrid
        {
            get { return _loadNonTobaccoGrid; }
            set { SetProperty(ref _loadNonTobaccoGrid, value); }
        }


        private bool _loadRtnGrid;
        public bool LoadRtnGrid
        {
            get { return _loadRtnGrid; }
            set { SetProperty(ref _loadRtnGrid, value); }
        }

        private bool _loadDifGrid;
        public bool LoadDifGrid
        {
            get { return _loadDifGrid; }
            set { SetProperty(ref _loadDifGrid, value); }
        }

        private bool _isEditmode;
        public bool IsEditmode
        {
            get { return _isEditmode; }
            set { SetProperty(ref _isEditmode, value); }
        }

        private bool _isAddEditPopupVisible;
        public bool IsAddEditPopupVisible
        {
            get { return _isAddEditPopupVisible; }
            set { SetProperty(ref _isAddEditPopupVisible, value); }
        }

        private bool _isEditIconVisible;
        public bool IsEditIconVisible
        {
            get { return _isEditIconVisible; }
            set { SetProperty(ref _isEditIconVisible, value); }
        }

        private string _popupTitle;
        public string PopupTitle
        {
            get { return _popupTitle; }
            set { SetProperty(ref _popupTitle, value); }
        }

        private string _totalTobacco;
        public string TotalTobacco
        {
            get { return _totalTobacco; }
            set { SetProperty(ref _totalTobacco, value); }
        }

        private string _totalNonTobacco;
        public string TotalNonTobacco
        {
            get { return _totalNonTobacco; }
            set { SetProperty(ref _totalNonTobacco, value); }
        }

        private string _totalRtnTobacco;
        public string TotalRtnTobacco
        {
            get { return _totalRtnTobacco; }
            set { SetProperty(ref _totalRtnTobacco, value); }
        }


        private string _totalDifTobacco;
        public string TotalDifTobacco
        {
            get { return _totalDifTobacco; }
            set { SetProperty(ref _totalDifTobacco, value); }
        }


        private AddEditProductOrderHistoryUIModel _addEditUIModel;
        public AddEditProductOrderHistoryUIModel AddEditUIModel
        {
            get { return _addEditUIModel; }
            set { SetProperty(ref _addEditUIModel, value); }
        }


        private OrderHistoryDetailsPageUIModel _uiModel;
        public OrderHistoryDetailsPageUIModel UiModel
        {
            get { return _uiModel; }
            set { SetProperty(ref _uiModel, value); }
        }

        private ObservableCollection<string> _creditRequestComoBoxSource;
        public ObservableCollection<string> CreditRequestComoBoxSource
        {
            get { return _creditRequestComoBoxSource; }
            set { SetProperty(ref _creditRequestComoBoxSource, value); }
        }

        private ObservableCollection<OrderHistoryDetailsGridUIModel> _tobaccoGridItemSource;
        public ObservableCollection<OrderHistoryDetailsGridUIModel> TobaccoGridItemSource
        {
            get { return _tobaccoGridItemSource; }
            set { SetProperty(ref _tobaccoGridItemSource, value); }
        }


        private ObservableCollection<OrderHistoryDetailsGridUIModel> _nonTobaccoGridItemSource;
        public ObservableCollection<OrderHistoryDetailsGridUIModel> NonTobaccoGridItemSource
        {
            get { return _nonTobaccoGridItemSource; }
            set { SetProperty(ref _nonTobaccoGridItemSource, value); }
        }


        private ObservableCollection<OrderHistoryDetailsGridUIModel> _rtnGridItemSource;
        public ObservableCollection<OrderHistoryDetailsGridUIModel> RtnGridItemSource
        {
            get { return _rtnGridItemSource; }
            set { SetProperty(ref _rtnGridItemSource, value); }
        }

        private ObservableCollection<OrderHistoryDetailsGridUIModel> _difGridItemSource;
        public ObservableCollection<OrderHistoryDetailsGridUIModel> DifGridItemSource
        {
            get { return _difGridItemSource; }
            set { SetProperty(ref _difGridItemSource, value); }
        }


        private bool _isNonTobaccoGridVisible;
        public bool IsNonTobaccoGridVisible
        {
            get { return _isNonTobaccoGridVisible; }
            set { SetProperty(ref _isNonTobaccoGridVisible, value); }
        }


        private bool _isTobaccoGridVisible;
        public bool IsTobaccoGridVisible
        {
            get { return _isTobaccoGridVisible; }
            set { SetProperty(ref _isTobaccoGridVisible, value); }
        }

        private bool _isRtnGridVisible;
        public bool IsRtnGridVisible
        {
            get { return _isRtnGridVisible; }
            set { SetProperty(ref _isRtnGridVisible, value); }
        }

        private bool _isDifGridVisible;
        public bool IsDifGridVisible
        {
            get { return _isDifGridVisible; }
            set { SetProperty(ref _isDifGridVisible, value); }
        }
        #endregion

        #region constructor
        public OrderHistoryDetailsPageViewModel()
        {
            LoadTobaccoGrid = false;
            LoadNonTobaccoGrid = false;
            NumPadButtonClickCommand = new RelayCommand<string>(NumPadButtonClicCommandHandler);
            AutoSuggestTextChanged = new RelayCommand<string>(AutoSuggestTextChangedHandler);
            AutoSuggestSuggestionChoosen = new RelayCommand<AutoSuggestBoxSuggestionChosenEventArgs>(AutoSuggestSuggestionChoosenHandler);
            ClosePopupCommand = new RelayCommand(ClosePopupCommandHandler);
            PriceChangedCommand = new RelayCommand<double>(PriceChangedCommandHandler);
            EditSaveCommand = new AsyncRelayCommand(EditSaveCommandHandler);
            QuantityChangedCommand = new RelayCommand<int>(QuantityChangedCommandHandler);
            OnNavigatedToCommand = new AsyncRelayCommand<OrderHistoryDetailsPageUIModel>(OnNavigatedToCommandHandler);
            NonTobaccoGridItemSource = new ObservableCollection<OrderHistoryDetailsGridUIModel>();
            TobaccoGridItemSource = new ObservableCollection<OrderHistoryDetailsGridUIModel>();
            RtnGridItemSource = new ObservableCollection<OrderHistoryDetailsGridUIModel>();
            DifGridItemSource = new ObservableCollection<OrderHistoryDetailsGridUIModel>();
            CreditRequestComoBoxSource = new ObservableCollection<string>();
            AddButtonCommand = new RelayCommand(AddButtonCommandHandler);
            SaveDeleteCommand = new AsyncRelayCommand<string>(SaveDeleteCommandHandler);
            ItemClickCommand = new RelayCommand<object>(ItemClickCommandHandler);
            PrintEmailCommand = new AsyncRelayCommand<string>(PrintEmailCommandHandler);
            IsNonTobaccoGridVisible = false;
            IsTobaccoGridVisible = false;
            IsEditIconVisible = false;
            IsAddEditPopupVisible = false;
            TotalDifTobacco = string.Empty;
            TotalNonTobacco = string.Empty;
            TotalRtnTobacco = string.Empty;
            TotalTobacco = string.Empty;
        }

        private async Task PrintEmailCommandHandler(string type)
        {
            switch (type)
            {
                case "Email":
                    //await BuildEmail();
                    break;
            }
        }

        //private async Task BuildEmail()
        //{
        //    try
        //    {
        //        IsLoading = true;
        //        decimal total = 0;
        //        var user = await AppReference.QueryService.GetUserData(AppReference.LoginUserNameProperty, AppReference.LoginUserPinProperty);
        //        var distributor = await AppReference.QueryService.GetDistributorFromId(UiModel.OrderMasterData.CustomerDistributorID);

        //        List<DRLMobile.OrderEmailAndPrint.OrderDetails> listOfDetails = new List<OrderEmailAndPrint.OrderDetails>();


        //        if ("8".Equals(UiModel.OrderMasterData.SalesType))
        //        {
        //            total = Convert.ToDecimal(!string.IsNullOrEmpty(TotalRtnTobacco) ? TotalRtnTobacco.Split('$').LastOrDefault() : "0") + Convert.ToDecimal(!string.IsNullOrEmpty(TotalDifTobacco) ? TotalDifTobacco.Split('$').LastOrDefault() : "0");

        //            foreach (var rtn in RtnGridItemSource)
        //            {
        //                DRLMobile.OrderEmailAndPrint.OrderDetails emailOrderDetails = new OrderEmailAndPrint.OrderDetails()
        //                {
        //                    Description = rtn.DisplayProductDesc,
        //                    UOM = rtn.DisplayProductUnit,
        //                    Quantity = rtn.OrderDetailObject.Quantity,
        //                    Price = rtn.OrderDetailObject.Price,
        //                    RegionID = UiModel.OrderMasterData?.RegionId.ToString(),
        //                    SalesTaxCertificate = UiModel.OrderMasterData?.RetailerSalesTaxCertificate,
        //                    TobaccoLicense = UiModel.OrderMasterData?.StateTobaccoLicence,
        //                    RetailerLicense = UiModel.OrderMasterData?.RetailerLicense,
        //                    ReturnReason = rtn?.CreditRequestType,
        //                    ProductName = rtn?.DisplayProductDesc,
        //                    GrandTotal = total,
        //                    DistributorCity = distributor != null ? distributor?.PhysicalAddressCityID : string.Empty,
        //                    DistributorID = distributor != null ? distributor.DistributorID : string.Empty,
        //                    DistributorName = distributor != null ? distributor?.CustomerName : string.Empty,
        //                    DistributorState = Core.Helpers.HelperMethods.GetValueFromIdNameDictionary(UiModel.StateDictionary, distributor != null ? distributor.ShippingAddressStateID : 0)

        //                };
        //                listOfDetails.Add(emailOrderDetails);
        //            }

        //            foreach (var dif in DifGridItemSource)
        //            {
        //                DRLMobile.OrderEmailAndPrint.OrderDetails emailOrderDetails = new OrderEmailAndPrint.OrderDetails()
        //                {
        //                    Description = dif.DisplayProductDesc,
        //                    UOM = dif.DisplayProductUnit,
        //                    Quantity = dif.OrderDetailObject.Quantity,
        //                    Price = dif.OrderDetailObject.Price,
        //                    RegionID = UiModel.OrderMasterData?.RegionId.ToString(),
        //                    SalesTaxCertificate = UiModel.OrderMasterData?.RetailerSalesTaxCertificate,
        //                    TobaccoLicense = UiModel.OrderMasterData?.StateTobaccoLicence,
        //                    RetailerLicense = UiModel.OrderMasterData?.RetailerLicense,
        //                    ReturnReason = dif?.CreditRequestType,
        //                    ProductName = dif?.DisplayProductDesc,
        //                    GrandTotal = total,
        //                    DistributorCity = distributor != null ? distributor?.PhysicalAddressCityID : string.Empty,
        //                    DistributorID = distributor != null ? distributor.DistributorID : string.Empty,
        //                    DistributorName = distributor != null ? distributor?.CustomerName : string.Empty,
        //                    DistributorState = Core.Helpers.HelperMethods.GetValueFromIdNameDictionary(UiModel.StateDictionary, distributor != null ? distributor.ShippingAddressStateID : 0)

        //                };
        //                listOfDetails.Add(emailOrderDetails);
        //            }
        //        }
        //        else
        //        {
        //            total = Convert.ToDecimal(!string.IsNullOrEmpty(TotalNonTobacco) ? TotalNonTobacco.Split('$').LastOrDefault() : "0") + Convert.ToDecimal(!string.IsNullOrEmpty(TotalTobacco) ? TotalTobacco.Split('$').LastOrDefault() : "0");
        //            foreach (var item in NonTobaccoGridItemSource)
        //            {
        //                OrderEmailAndPrint.OrderDetails emailOrderDetails = new OrderEmailAndPrint.OrderDetails()
        //                {
        //                    Description = item.DisplayProductDesc,
        //                    UOM = item.DisplayProductUnit,
        //                    Quantity = item.OrderDetailObject.Quantity,
        //                    Price = item.OrderDetailObject.Price,
        //                    RegionID = UiModel.OrderMasterData?.RegionId.ToString(),
        //                    SalesTaxCertificate = UiModel.OrderMasterData?.RetailerSalesTaxCertificate,
        //                    TobaccoLicense = UiModel.OrderMasterData?.StateTobaccoLicence,
        //                    RetailerLicense = UiModel.OrderMasterData?.RetailerLicense,
        //                    ReturnReason = item?.CreditRequestType,
        //                    ProductName = item?.DisplayProductName,
        //                    GrandTotal = total,
        //                    DistributorCity = distributor != null ? distributor?.PhysicalAddressCityID : string.Empty,
        //                    DistributorID = distributor != null ? distributor.DistributorID : string.Empty,
        //                    DistributorName = distributor != null ? distributor?.CustomerName : string.Empty,
        //                    DistributorState = Core.Helpers.HelperMethods.GetValueFromIdNameDictionary(UiModel.StateDictionary, distributor != null ? distributor.ShippingAddressStateID : 0)

        //                };
        //                listOfDetails.Add(emailOrderDetails);
        //            }
        //            foreach (var item in TobaccoGridItemSource)
        //            {
        //                DRLMobile.OrderEmailAndPrint.OrderDetails emailOrderDetails = new OrderEmailAndPrint.OrderDetails()
        //                {
        //                    Description = item.DisplayProductDesc,
        //                    UOM = item.DisplayProductUnit,
        //                    Quantity = item.OrderDetailObject.Quantity,
        //                    Price = item.OrderDetailObject.Price,
        //                    RegionID = UiModel.OrderMasterData?.RegionId.ToString(),
        //                    SalesTaxCertificate = UiModel.OrderMasterData?.RetailerSalesTaxCertificate,
        //                    TobaccoLicense = UiModel.OrderMasterData?.StateTobaccoLicence,
        //                    RetailerLicense = UiModel.OrderMasterData?.RetailerLicense,
        //                    ReturnReason = item?.CreditRequestType,
        //                    ProductName = item?.DisplayProductDesc,
        //                    GrandTotal = total,
        //                    DistributorCity = distributor != null ? distributor?.PhysicalAddressCityID : string.Empty,
        //                    DistributorID = distributor != null ? distributor.DistributorID : string.Empty,
        //                    DistributorName = distributor != null ? distributor?.CustomerName : string.Empty,
        //                    DistributorState = Core.Helpers.HelperMethods.GetValueFromIdNameDictionary(UiModel.StateDictionary, distributor != null ? distributor.ShippingAddressStateID : 0)

        //                };
        //                listOfDetails.Add(emailOrderDetails);
        //            }
        //        }

        //        DRLMobile.OrderEmailAndPrint.CustomerInfo customerInfo = new OrderEmailAndPrint.CustomerInfo()
        //        {
        //            Address = UiModel.SelectedCustomer?.PhysicalAddress,
        //            City = UiModel.SelectedCustomer?.City,
        //            ContactEmail = UiModel?.SelectedCustomer?.ContactEmail,
        //            CustomerName = UiModel?.SelectedCustomer?.CustomerName,
        //            CustomerNumber = UiModel?.SelectedCustomer?.CustomerNumber,
        //            OrderDate = UiModel.OrderMasterData?.OrderDate,
        //            PurchaseNumber = UiModel.OrderMasterData.InvoiceNumber,
        //            PONumber = UiModel.OrderMasterData?.PurchaseOrderNumber,
        //            PermitNumber = UiModel.OrderMasterData?.OrderMasterSellerRepTobacco,
        //            RegionID = UiModel.OrderMasterData.RegionId.ToString(),
        //            Zip = UiModel.SelectedCustomer.PhysicalAddressZipCode,
        //            ContactPhone = UiModel.SelectedCustomer?.ContactPhone,
        //            State = Core.Helpers.HelperMethods.GetValueFromIdNameDictionary(UiModel.StateDictionary, UiModel.SelectedCustomer.PhysicalAddressStateID),
        //            ShipDate = UiModel.OrderMasterData?.PrebookShipDate,
        //            CustomStatement = UiModel.OrderMasterData?.CustomStatement,
        //            CustomerComment = UiModel.OrderMasterData?.CustomerComment,
        //            UserName = AppReference.LoginUserNameProperty,
        //            SalesRepEmail = user?.EmailID,
        //            SalesRepPhone = user?.ContactNo,
        //            SalesRepUsername = user?.UserName,
        //        };


        //        var fileName = !string.IsNullOrEmpty(UiModel.OrderMasterData.InvoiceNumber) ? UiModel.OrderMasterData.InvoiceNumber : "Invoice";
        //        var folder = ApplicationData.Current.LocalFolder;
        //        var path = Path.Combine(folder.Path, string.Format("{0}.pdf", fileName));
        //        OrderEmailAndPrint.BuildOrderEmail emailBuiler = new OrderEmailAndPrint.BuildOrderEmail();
        //        var res = emailBuiler.BuildEmailBody(Body: listOfDetails, customerInfo: customerInfo, UiModel.OrderMasterData.SalesType, "");
        //            //(string)Application.Current.Resources["RepublicBrandImage"]);
        //        var isEmailGenerted = emailBuiler.GenerateOrderPDF(path, res);

        //        var subject = string.Format("Order#: {0} - {1} - {2} - {3}", UiModel?.OrderMasterData.InvoiceNumber, UiModel?.CustomerNumber, UiModel?.CustomerName, UiModel?.SalesType);


        //        var emailBody = new EmailModel()
        //        {
        //            Subject = subject,
        //            BodyHtml = res,
        //            AttachmentListByPath = isEmailGenerted ? new List<string>() { path } : null
        //        };
        //        var isEmailSuccessfull = await EmailService.Instance.SendMailFromOutlook(emailBody);
        //        var delay = isEmailSuccessfull ? 8000 : 200;
        //        await Task.Delay(delay);
        //        IsLoading = false;
        //    }
        //    catch (Exception ex)
        //    {
        //        IsLoading = false;
        //        ErrorLogger.WriteToErrorLog(nameof(OrderHistoryDetailsPageViewModel), nameof(BuildEmail), ex.StackTrace);
        //    }
        //}

        #endregion


        #region private methods

        private void ItemClickCommandHandler(object obj)
        {
            if (obj is OrderHistoryDetailsGridUIModel && IsEditmode)
            {
                var o = obj as OrderHistoryDetailsGridUIModel;
                IsAdd = false;
                IsAddEditPopupVisible = true;
                PopupTitle = "Edit Product Quantity/Unit";

                AddEditUIModel = new AddEditProductOrderHistoryUIModel();
                AddEditUIModel.EditedOrderDetail = o.OrderDetailObject;
                AddEditUIModel.Quantity = o.DisplayProductQty;
                AddEditUIModel.Price = o.DisplayProductUnitPrice.Split('$').LastOrDefault();
                AddEditUIModel.SelectedUom = o.DisplayProductUnit;
                AddEditUIModel.SubTotal = o.DisplayProductSubtotal;
                AddEditUIModel.IsCreditRequest = "8".Equals(UiModel?.OrderMasterData?.SalesType);
                if (o.Product != null)
                {
                    AddEditUIModel.SelectedProduct = UiModel.DbProducts.FirstOrDefault(x => x.ProductID == o.Product.ProductID);
                    AddEditUIModel.ProductName = AddEditUIModel.SelectedProduct?.ProductName;
                    AddEditUIModel.ProductDesc = AddEditUIModel.SelectedProduct?.Description;
                }
                if (o.Brand != null)
                {
                    AddEditUIModel.SelectedBrand = UiModel.DbBrandData.FirstOrDefault(x => x.BrandId == o.Brand.BrandId);
                    AddEditUIModel.BrandName = AddEditUIModel.SelectedBrand?.BrandName;
                }
                if (o.Style != null)
                {
                    AddEditUIModel.SelectedStyle = UiModel.DbStyleMaster.FirstOrDefault(x => x.StyleId == o.Style.StyleId);
                    AddEditUIModel.StyleName = AddEditUIModel.SelectedStyle?.StyleName;
                }
                if (o.Category != null)
                {
                    AddEditUIModel.SelectedCategory = UiModel.DbCategoryMaster.FirstOrDefault(x => x.CategoryID == o.Category.CategoryID);
                }
                if (AddEditUIModel.IsCreditRequest)
                {
                    GetCreditRequestDropDownValues(o?.Product?.isTobbaco);
                    AddEditUIModel.SelectedCreditRequest = o.CreditRequestType;
                }

            }
        }


        private async Task SaveDeleteCommandHandler(string type)
        {
            if ("SAVE".Equals(type))
            {
                if (IsAdd)
                {
                    if (AddEditUIModel.IsCreditRequest)
                        AddProductToListForCreditRequest();
                    else
                        await AddProductToList();
                }
                else
                {
                    if (AddEditUIModel.IsCreditRequest)
                        UpdateOrderDetailsForCreditRequest();
                    else
                        await UpdateOrderDetails();

                }
            }
            else
            {
                await DeleteItem();
            }

            IsAddEditPopupVisible = false;
        }

        private void UpdateOrderDetailsForCreditRequest()
        {
            // Method intentionally left empty.


            if (!AddEditUIModel.EditedOrderDetail.CreditRequest.Equals(AddEditUIModel.SelectedCreditRequest))
            {
                OrderHistoryDetailsGridUIModel newObject = new OrderHistoryDetailsGridUIModel()
                {
                    OrderDetailObject = AddEditUIModel.EditedOrderDetail,
                    Brand = AddEditUIModel.SelectedBrand,
                    Category = AddEditUIModel.SelectedCategory,
                    CreditRequestType = AddEditUIModel.SelectedCreditRequest,
                    DisplayBrandName = AddEditUIModel?.SelectedBrand?.BrandName,
                    DisplayProductDesc = AddEditUIModel?.ProductDesc,
                    DisplayProductName = AddEditUIModel?.ProductName,
                    DisplayProductQty = AddEditUIModel?.EditedOrderDetail?.Quantity.ToString(),
                    DisplayProductSubtotal = string.Format("${0}", AddEditUIModel.SubTotal),
                    DisplayProductUnit = AddEditUIModel?.SelectedUom,
                    DisplayProductUnitPrice = string.Format("${0}", AddEditUIModel.Price),
                    DisplayStyleName = AddEditUIModel?.StyleName,
                    Product = AddEditUIModel?.SelectedProduct,
                    Style = AddEditUIModel?.SelectedStyle
                };
                if (AddEditUIModel.SelectedCreditRequest.Contains("RTN"))
                {
                    var replaceObj = DifGridItemSource.FirstOrDefault(x => x.OrderDetailObject.OrderDetailId.Value == AddEditUIModel.EditedOrderDetail.OrderDetailId.Value);
                    DifGridItemSource.Remove(replaceObj);
                    RtnGridItemSource.Add(newObject);
                }
                else
                {
                    var replaceObj = RtnGridItemSource.FirstOrDefault(x => x.OrderDetailObject.OrderDetailId.Value == AddEditUIModel.EditedOrderDetail.OrderDetailId);
                    RtnGridItemSource.Remove(replaceObj);
                    DifGridItemSource.Add(newObject);
                }



            }


            if (RtnGridItemSource.Any())
            {
                IsRtnGridVisible = true;
                LoadRtnGrid = true;
            }
            else
            {
                IsRtnGridVisible = false;
                LoadRtnGrid = false;
            }

            if (DifGridItemSource.Any())
            {
                IsDifGridVisible = true;
                LoadDifGrid = true;
            }
            else
            {
                IsDifGridVisible = false;
                LoadDifGrid = false;
            }
        }

        private void AddProductToListForCreditRequest()
        {
            // Method intentionally left empty.
        }

        private async Task DeleteItem()
        {
            try
            {
                var totalItem = NonTobaccoGridItemSource.Count() + TobaccoGridItemSource.Count();
                if (totalItem == 1)
                {
                    await AlertHelper.Instance.ShowConfirmationAlert("Alert", "You cannot delete all the item from the order", "Ok");
                    return;
                }
                var result = await AlertHelper.Instance.ShowConfirmationAlert("Delete", "Are you sure you want to delete this item?", "Yes", "No");
                if (result)
                {
                    var isDeleted = await AppReference.QueryService.DeleteOrderDetailFromHistoryPage(AddEditUIModel.EditedOrderDetail);
                    if (isDeleted)
                    {
                        if (AddEditUIModel.SelectedProduct?.isTobbaco == 1)
                        {
                            var objectToDel = TobaccoGridItemSource.FirstOrDefault(x => x.OrderDetailObject.OrderDetailId == AddEditUIModel.EditedOrderDetail.OrderDetailId);
                            var oldTotalString = TotalTobacco.Split('$').LastOrDefault();
                            var deltedTotal = AddEditUIModel.SubTotal.Split('$').LastOrDefault();
                            var newTotal = Convert.ToDouble(oldTotalString) - Convert.ToDouble(deltedTotal);
                            TotalTobacco = string.Format("${0}", newTotal.ToString("0.00"));
                            TobaccoGridItemSource.Remove(objectToDel);
                        }
                        else
                        {
                            var objectToDel = NonTobaccoGridItemSource.FirstOrDefault(x => x.OrderDetailObject.OrderDetailId == AddEditUIModel.EditedOrderDetail.OrderDetailId);
                            var oldTotalString = TotalNonTobacco.Split('$').LastOrDefault();
                            var deltedTotal = AddEditUIModel.SubTotal.Split('$').LastOrDefault();
                            var newTotal = Convert.ToDouble(oldTotalString) - Convert.ToDouble(deltedTotal);
                            TotalNonTobacco = string.Format("${0}", newTotal.ToString("0.00"));
                            NonTobaccoGridItemSource.Remove(objectToDel);
                        }
                    }
                    IsAddEditPopupVisible = false;
                    int total = 0;
                    var date = Core.Helpers.DateTimeHelper.ConvertToDbInsertDateTimeFormat(DateTime.Now);

                    if (TobaccoGridItemSource.Count == 0)
                        IsTobaccoGridVisible = false;
                    if (NonTobaccoGridItemSource.Count == 0)
                        IsNonTobaccoGridVisible = false;

                    RecalculateMainGridTotal(date, ref total);


                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(OrderHistoryDetailsPage), nameof(DeleteItem), ex.StackTrace);
            }
        }

        private async Task UpdateOrderDetails()
        {
            try
            {
                var date = Core.Helpers.DateTimeHelper.ConvertToDbInsertDateTimeFormat(DateTime.Now);
                AddEditUIModel.EditedOrderDetail.UpdatedDate = date;
                AddEditUIModel.EditedOrderDetail.Quantity = Convert.ToInt32(AddEditUIModel.Quantity);
                AddEditUIModel.EditedOrderDetail.Total = AddEditUIModel.SubTotal.Split('$').LastOrDefault();
                AddEditUIModel.EditedOrderDetail.Price = Convert.ToInt32(AddEditUIModel.Price.Split('$').LastOrDefault());
                AddEditUIModel.EditedOrderDetail.Unit = AddEditUIModel.SelectedUom;

                if (AddEditUIModel.SelectedProduct?.isTobbaco == 1)
                {
                    TobaccoGridItemSource.FirstOrDefault(x => x.OrderDetailObject.OrderDetailId == AddEditUIModel.EditedOrderDetail.OrderDetailId).DisplayProductSubtotal = AddEditUIModel.SubTotal;
                    TobaccoGridItemSource.FirstOrDefault(x => x.OrderDetailObject.OrderDetailId == AddEditUIModel.EditedOrderDetail.OrderDetailId).DisplayProductQty = AddEditUIModel.Quantity;
                    TobaccoGridItemSource.FirstOrDefault(x => x.OrderDetailObject.OrderDetailId == AddEditUIModel.EditedOrderDetail.OrderDetailId).DisplayProductUnitPrice = AddEditUIModel.Price;
                    TobaccoGridItemSource.FirstOrDefault(x => x.OrderDetailObject.OrderDetailId == AddEditUIModel.EditedOrderDetail.OrderDetailId).DisplayProductUnit = AddEditUIModel.SelectedUom;
                }
                else
                {
                    NonTobaccoGridItemSource.FirstOrDefault(x => x.OrderDetailObject.OrderDetailId == AddEditUIModel.EditedOrderDetail.OrderDetailId).DisplayProductSubtotal = AddEditUIModel.SubTotal;
                    NonTobaccoGridItemSource.FirstOrDefault(x => x.OrderDetailObject.OrderDetailId == AddEditUIModel.EditedOrderDetail.OrderDetailId).DisplayProductQty = AddEditUIModel.Quantity;
                    NonTobaccoGridItemSource.FirstOrDefault(x => x.OrderDetailObject.OrderDetailId == AddEditUIModel.EditedOrderDetail.OrderDetailId).DisplayProductUnitPrice = AddEditUIModel.Price;
                    NonTobaccoGridItemSource.FirstOrDefault(x => x.OrderDetailObject.OrderDetailId == AddEditUIModel.EditedOrderDetail.OrderDetailId).DisplayProductUnit = AddEditUIModel.SelectedUom;
                }

                int total = 0;
                await AppReference.QueryService.InsertOrUpdateOrderDetail(AddEditUIModel.EditedOrderDetail);

                RecalculateMainGridTotal(date, ref total);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(OrderHistoryDetailsPageViewModel), nameof(UpdateOrderDetails), ex.StackTrace);
            }

        }

        private async Task AddProductToList()
        {
            try
            {
                if (AddEditUIModel.SelectedProduct != null && !string.IsNullOrWhiteSpace(AddEditUIModel.ProductName))
                {
                    var date = Core.Helpers.DateTimeHelper.ConvertToDbInsertDateTimeFormat(DateTime.Now);
                    var orderDetail = new OrderDetail()
                    {
                        isTobbaco = AddEditUIModel.SelectedProduct.isTobbaco,
                        OrderId = UiModel.OrderMasterData.OrderID,
                        DeviceOrderID = UiModel.OrderMasterData.OrderID.ToString(),
                        Price = Convert.ToInt32(AddEditUIModel.Price.Split('$').LastOrDefault()),
                        ProductDescription = AddEditUIModel.ProductDesc,
                        Quantity = Convert.ToInt32(AddEditUIModel.Quantity),
                        ProductId = AddEditUIModel.SelectedProduct.ProductID,
                        BrandId = AddEditUIModel.SelectedBrand != null ? AddEditUIModel.SelectedBrand.BrandId : 0,
                        CategoryId = AddEditUIModel.SelectedCategory.CategoryID,
                        CreatedDate = date,
                        UpdatedDate = date,
                        BrandName = AddEditUIModel.SelectedBrand != null ? AddEditUIModel.SelectedBrand.BrandName : string.Empty,
                        CategoryName = AddEditUIModel.SelectedCategory.CategoryName,
                        ProductName = AddEditUIModel.SelectedProduct.ProductName,
                        StyleId = AddEditUIModel.SelectedStyle.StyleId,
                        Total = AddEditUIModel.SubTotal.Split('$').LastOrDefault(),
                        StyleName = AddEditUIModel.StyleName,
                        Unit = AddEditUIModel.SelectedUom,
                    };

                    orderDetail = await AppReference.QueryService.InsertOrUpdateOrderDetail(orderDetail);

                    var obj = new OrderHistoryDetailsGridUIModel()
                    {
                        Brand = AddEditUIModel.SelectedBrand,
                        Category = AddEditUIModel.SelectedCategory,
                        DisplayBrandName = AddEditUIModel.SelectedBrand != null ? AddEditUIModel.SelectedBrand.BrandName : string.Empty,
                        DisplayProductDesc = AddEditUIModel.ProductDesc,
                        DisplayProductName = AddEditUIModel.ProductName,
                        DisplayProductQty = AddEditUIModel.Quantity,
                        DisplayProductSubtotal = AddEditUIModel.SubTotal,
                        DisplayProductUnit = AddEditUIModel.SelectedUom,
                        DisplayProductUnitPrice = AddEditUIModel.Price,
                        DisplayStyleName = AddEditUIModel.StyleName,
                        Product = AddEditUIModel.SelectedProduct,
                        Style = AddEditUIModel.SelectedStyle,
                        OrderDetailObject = orderDetail,
                    };
                    var productTotal = Convert.ToDouble(AddEditUIModel.SubTotal.Split('$').LastOrDefault());
                    if (AddEditUIModel.SelectedProduct.isTobbaco == 1)
                    {
                        TobaccoGridItemSource.Add(obj);
                        var totalTobacco = Convert.ToDouble(TotalTobacco.Split('$').LastOrDefault());
                        var total = totalTobacco + productTotal;
                        TotalTobacco = string.Format("${0}", total.ToString("0.00"));
                        IsTobaccoGridVisible = true;
                        LoadTobaccoGrid = true;
                    }
                    else
                    {
                        NonTobaccoGridItemSource.Add(obj);
                        var totalnonTobacco = Convert.ToDouble(TotalNonTobacco.Split('$').LastOrDefault());
                        var total = totalnonTobacco + productTotal;
                        TotalNonTobacco = string.Format("${0}", total.ToString("0.00"));
                        IsNonTobaccoGridVisible = true;
                        LoadNonTobaccoGrid = true;
                    }

                }
                else
                {
                    await AlertHelper.Instance.ShowConfirmationAlert("Alert", "Please select a product", "Yes");
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(OrderHistoryDetailsPageViewModel), nameof(AddProductToList), ex.StackTrace);
            }
        }

        private void PriceChangedCommandHandler(double arg)
        {
            AddEditUIModel.Price = arg.ToString();
            CalculateTotalForPopup();
        }

        private void QuantityChangedCommandHandler(int qty)
        {
            AddEditUIModel.Quantity = qty.ToString();
            CalculateTotalForPopup();
        }

        void CalculateTotalForPopup()
        {
            try
            {
                var qty = Convert.ToDouble(AddEditUIModel.Quantity.Split('$').LastOrDefault());
                var price = Convert.ToDouble(AddEditUIModel.Price.Split('$').LastOrDefault());
                var subTotal = qty * price;
                AddEditUIModel.SubTotal = string.Format("${0}", subTotal.ToString("0.00"));
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(OrderHistoryDetailsPageViewModel), nameof(CalculateTotalForPopup), ex.StackTrace);
            }
        }

        private void NumPadButtonClicCommandHandler(string arg)
        {
            if (IsOpenedFrom == "quantity")
            {
                UpdateQuantityFieldValue(arg);
            }
            else
            {
                UpdatePriceFieldValue(arg);
            }
            CalculateTotalForPopup();
        }

        private void UpdatePriceFieldValue(string value)
        {
            if (value == "C")
            {
                if (AddEditUIModel.Price.Length > 0)
                {
                    AddEditUIModel.Price = AddEditUIModel.Price.Remove(AddEditUIModel.Price.Length - 1);
                }
            }
            else
            {
                var priceString = AddEditUIModel.Price;
                if (!priceString.Contains("."))
                {
                    if (value != ".")
                    {
                        if (priceString.Length < 4)
                        {
                            double temp;
                            priceString += value;
                            var price = Convert.ToInt32(priceString);
                            AddEditUIModel.Price = string.Format("{0:0.00}", price);
                            bool isOk = double.TryParse(AddEditUIModel.Price, out temp);
                            int data = isOk ? (int)temp : 0;
                            if (isOk)
                            {
                                AddEditUIModel.Price = data.ToString();
                            }
                        }
                    }
                    else
                    {
                        priceString += value;
                        AddEditUIModel.Price = priceString;
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
                                AddEditUIModel.Price = temp.ToString();
                            }
                        }
                        else if (tail.Length == 1)
                        {
                            priceString += value;
                            double temp;
                            bool isOk = double.TryParse(priceString, out temp);
                            int data = isOk ? (int)temp : 0;
                            if (isOk)
                            {
                                AddEditUIModel.Price = temp.ToString();
                            }
                        }
                    }
                }
            }
        }

        private void UpdateQuantityFieldValue(string value)
        {
            if (value == "C")
            {
                if (AddEditUIModel?.Quantity?.Length > 0)
                {
                    AddEditUIModel.Quantity = AddEditUIModel?.Quantity?.Remove(AddEditUIModel.Quantity.Length - 1);
                }
            }
            else
            {
                if (AddEditUIModel?.Quantity?.Length == 0)
                {
                    if (value != "0" && value != "-")
                    {
                        AddEditUIModel.Quantity = AddEditUIModel?.Quantity + value;
                    }
                    else
                    {
                        AddEditUIModel.Quantity = string.Empty;
                    }
                }
                else if (AddEditUIModel?.Quantity.Length < 4 && (value != "-"))
                {
                    AddEditUIModel.Quantity = AddEditUIModel?.Quantity + value;
                }
            }


        }

        private void AutoSuggestSuggestionChoosenHandler(AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            var product = args.SelectedItem as ProductMaster;
            if (product.SearchDisplayPath.Contains(ResourceExtensions.GetLocalized("NoResultsErrorMessage")))
                return;
            AddEditUIModel.ProductList.Clear();
            AddEditUIModel.SelectedBrand = UiModel.DbBrandData.FirstOrDefault(x => x.BrandId == product.BrandId);
            AddEditUIModel.SelectedCategory = UiModel.DbCategoryMaster.FirstOrDefault(x => x.CategoryID == product.CatId);
            AddEditUIModel.SelectedStyle = UiModel.DbStyleMaster.FirstOrDefault(x => x.StyleId == product.StyleId);
            AddEditUIModel.SelectedProduct = product;
            AddEditUIModel.SelectedUom = SetUomForCategoryId(AddEditUIModel.SelectedCategory.ERPCategoryId);
            AddEditUIModel.Quantity = "1";
            AddEditUIModel.Price = "$0.00";
            AddEditUIModel.ProductName = product.ProductName;
            AddEditUIModel.ProductDesc = product.Description;
            AddEditUIModel.BrandName = AddEditUIModel.SelectedBrand?.BrandName;
            AddEditUIModel.StyleName = AddEditUIModel.SelectedStyle?.StyleName;
            if (AddEditUIModel.IsCreditRequest)
            {
                GetCreditRequestDropDownValues(AddEditUIModel?.SelectedProduct?.isTobbaco);
                AddEditUIModel.SelectedCreditRequest = AddEditUIModel?.SelectedCategory?.CategoryID == 6 ? "DIF-Destroyed" : "RTN-Retail Returns";

            }
        }

        private void GetCreditRequestDropDownValues(int? isTobbaco)
        {
            CreditRequestComoBoxSource.Clear();
            if (isTobbaco.HasValue && isTobbaco.Value != 1)
            {
                CreditRequestComoBoxSource.Add("DIF-Destroyed");
            }
            CreditRequestComoBoxSource.Add("RTN-Retail Returns");
            CreditRequestComoBoxSource.Add("RTN-Out Of Code");
            CreditRequestComoBoxSource.Add("RTN-Defective");
            CreditRequestComoBoxSource.Add("RTN-Overstock");
            CreditRequestComoBoxSource.Add("RTN-Discontinued");
            CreditRequestComoBoxSource.Add("RTN-Reset");
            CreditRequestComoBoxSource.Add("RTN-Mispick");
            CreditRequestComoBoxSource.Add("RTN-Misship");
            CreditRequestComoBoxSource.Add("RTN-Order Error");
        }

        private void AutoSuggestTextChangedHandler(string text)
        {
            AddEditUIModel.ProductList.Clear();
            if (!string.IsNullOrWhiteSpace(text))
            {
                var tempList = UiModel.DbProducts.Where(x => x.SearchDisplayPath.ToLower().Contains(text.ToLower()) && x.CatId != -88 && x.CatId != 10 && x.CatId != 11 && x.UOM == "CA" && x.CatId != -99 && x.IsDeleted == 0 && x.SRCHoneySellable != 0 && x.SRCHoneyReturnable != 0 && x.SRCCanIOrder != 0 && !UiModel.DbOrderDetailsData.Any(y => y.ProductId == x.ProductID)).ToList();
                if (tempList == null || tempList.Count == 0)
                {
                    AddEditUIModel.ProductList.Add(new ProductMaster() { ProductName = ResourceExtensions.GetLocalized("NoResultsErrorMessage") });
                }
                else
                {
                    tempList.ForEach(x => AddEditUIModel.ProductList.Add(x));
                }
            }
        }


        private string SetUomForCategoryId(int id)
        {
            switch (id)
            {
                case 1010:
                case 1030:
                case 1031:
                case 1042:
                    return "BX";
                case 1020:
                case 1021:
                case 1040:
                case 1041:
                case 1043:
                case 1070:
                    return "EA";
                default:
                    return "CA";
            }
        }

        private void AddButtonCommandHandler()
        {
            AddEditUIModel = new AddEditProductOrderHistoryUIModel();
            AddEditUIModel.AutoSuggestionText = string.Empty;
            IsAddEditPopupVisible = true;
            IsAdd = true;
            PopupTitle = "Add Product In Cart";
            AddEditUIModel.IsCreditRequest = "8".Equals(UiModel?.OrderMasterData?.SalesType);
            ///AddEditUIModel.ProductList = new ObservableCollection<ProductMaster>(UiModel.DbProducts.Where(sRCProductUIModel => sRCProductUIModel.SRCHoneySellable != 0 && sRCProductUIModel.SRCHoneyReturnable != 0 && sRCProductUIModel.SRCCanIOrder != 0));

        }


        private void ClosePopupCommandHandler()
        {
            IsAddEditPopupVisible = false;
        }

        private async Task EditSaveCommandHandler()
        {
            if (IsEditmode)
            {
                await AddOrderDetailsToDb();
            }
            IsEditmode = !IsEditmode;
        }

        private async Task AddOrderDetailsToDb()
        {
            try
            {
                IsLoading = true;
                int total = 0;

                var date = Core.Helpers.DateTimeHelper.ConvertToDbInsertDateTimeFormat(DateTime.Now);
                RecalculateMainGridTotal(date, ref total);
                ///logic to get the updated data
                UiModel.OrderMasterData.GrandTotal = total.ToString("0.00");
                UiModel.OrderMasterData.UpdatedDate = date;
                UiModel.OrderMasterData.PurchaseOrderNumber = UiModel.PurchaseOrder;
                UiModel.OrderMasterData.OrderAddress = UiModel.PhysicalAddress;
                UiModel.OrderMasterData.OrderCityId = UiModel.PhysicalCity;
                UiModel.OrderMasterData.OrderStateId = Core.Helpers.HelperMethods.GetKeyFromIdNameDictionary(UiModel.StateDictionary, UiModel.SelectedPhysicalState);
                UiModel.OrderMasterData.OrderZipCode = UiModel.PhysicalZip;
                UiModel.OrderMasterData.StateTobaccoLicence = UiModel.StateTobaccoLicense;
                UiModel.OrderMasterData.RetailerLicense = UiModel.RetailerLicense;
                UiModel.OrderMasterData.RetailerSalesTaxCertificate = UiModel.RetailerSalesTaxCertificate;
                UiModel.OrderMasterData.EmailRecipients = UiModel.EmailTo;
                UiModel.OrderMasterData.CustomStatement = UiModel.CustomTaxStatement;
                UiModel.OrderMasterData.OrderMasterSellerRepTobacco = UiModel.OrderMasterSellerRepTobacco;
                UiModel.OrderMasterData.PrebookShipDate = Core.Helpers.DateTimeHelper.ConvertToDbInsertDateTimeFormat(UiModel.PreBookDate.DateTime);
                UiModel.OrderMasterData.CustomerComment = UiModel.Comments;

                await AppReference.QueryService.InsertOrUpdateOrderMaster(UiModel.OrderMasterData, false);
                IsLoading = false;

                await AlertHelper.Instance.ShowConfirmationAlert("Alert", "Order Updated Successfully", "Yes");

            }
            catch (Exception ex)
            {
                IsLoading = false;
                ErrorLogger.WriteToErrorLog(nameof(OrderHistoryDetailsPageViewModel), nameof(AddOrderDetailsToDb), ex.StackTrace);
            }

        }

        private void RecalculateMainGridTotal(string date, ref int total)
        {
            int qtyTotal = 0;
            int tobaccoTotal = 0;
            int nonTobaccoTotal = 0;
            foreach (var item in TobaccoGridItemSource)
            {
                item.OrderDetailObject.Quantity = (int)Convert.ToDouble(item.DisplayProductQty);
                item.OrderDetailObject.Price = (int)Convert.ToDouble(item.DisplayProductUnitPrice.Split('$').LastOrDefault());
                item.OrderDetailObject.Unit = item.DisplayProductUnit;
                item.OrderDetailObject.UpdatedDate = date;
                tobaccoTotal = tobaccoTotal + (item.OrderDetailObject.Price * item.OrderDetailObject.Quantity);
                qtyTotal += item.OrderDetailObject.Quantity;
            }

            foreach (var item in NonTobaccoGridItemSource)
            {
                item.OrderDetailObject.Quantity = (int)Convert.ToDouble(item.DisplayProductQty);
                item.OrderDetailObject.Price = (int)Convert.ToDouble(item.DisplayProductUnitPrice.Split('$').LastOrDefault());
                item.OrderDetailObject.Unit = item.DisplayProductUnit;
                item.OrderDetailObject.UpdatedDate = date;
                nonTobaccoTotal = nonTobaccoTotal + (item.OrderDetailObject.Price * item.OrderDetailObject.Quantity);
                qtyTotal += item.OrderDetailObject.Quantity;
            }
            total = tobaccoTotal + nonTobaccoTotal;

            TotalTobacco = string.Format("${0}", tobaccoTotal.ToString("0.00"));
            TotalNonTobacco = string.Format("${0}", nonTobaccoTotal.ToString("0.00"));


            OrderDetailUpdatedModel updatedObj = new OrderDetailUpdatedModel() { Qty = qtyTotal, Price = total, OrderId = UiModel.OrderMasterData.OrderID };
            OrderHistoryPage.UpdatedOrder = updatedObj;
        }

        private async Task OnNavigatedToCommandHandler(OrderHistoryDetailsPageUIModel selectedObject)
        {
            try
            {
                IsLoading = true;
                LoadTobaccoGrid = false;
                LoadNonTobaccoGrid = false;
                IsNonTobaccoGridVisible = false;
                IsTobaccoGridVisible = false;
                IsEditmode = false;
                UiModel = await AppReference.QueryService.GetOrderHistoryDetailData(selectedObject);
                UiModel.PopulateUI();
                var totalTobacco = 0;
                var totalNonTobacco = 0;
                var rtnTotal = 0;
                var difTotal = 0;
                foreach (var d in UiModel.DbOrderDetailsData)
                {
                    var obj = new OrderHistoryDetailsGridUIModel();
                    obj.Brand = UiModel.DbBrandData.FirstOrDefault(x => x.BrandId == d.BrandId);
                    obj.Category = UiModel.DbCategoryMaster.FirstOrDefault(x => x.CategoryID == d.CategoryId);
                    obj.Style = UiModel.DbStyleMaster.FirstOrDefault(x => x.StyleId == d.StyleId);
                    obj.Product = UiModel.DbProducts.FirstOrDefault(x => x.ProductID == d.ProductId);
                    obj.DisplayProductQty = d.Quantity.ToString();
                    obj.DisplayProductUnitPrice = string.Format("${0}", d.Price.ToString("0.00"));
                    int total = d.Price * d.Quantity;
                    obj.DisplayProductSubtotal = string.Format("${0}", total.ToString("0.00"));
                    obj.OrderDetailObject = d;
                    obj.CreditRequestType = d.CreditRequest;
                    obj.DisplayProductUnit = d?.Unit;
                    obj.PropulateUI();
                    if (!"8".Equals(UiModel.OrderMasterData.SalesType))
                    {
                        if (d.isTobbaco == 0)
                        {
                            NonTobaccoGridItemSource.Add(obj);
                            totalNonTobacco += total;
                        }
                        else
                        {
                            TobaccoGridItemSource.Add(obj);
                            totalTobacco += total;
                        }
                    }
                    else
                    {
                        if (d.CreditRequest.Contains("RTN"))
                        {
                            RtnGridItemSource.Add(obj);
                            rtnTotal += total;
                        }
                        else if (d.CreditRequest.Contains("DIF"))
                        {
                            DifGridItemSource.Add(obj);
                            difTotal += total;
                        }
                    }
                }


                if (!string.IsNullOrWhiteSpace(UiModel.OrderMasterData.SalesType)
                    && !UiModel.OrderMasterData.SalesType.Equals("5")
                    && !UiModel.OrderMasterData.SalesType.Equals("10")
                    && !UiModel.OrderMasterData.SalesType.Equals("13")
                    && UiModel.OrderMasterData.IsExported == 0)
                {
                    IsEditIconVisible = true;
                }
                else
                {
                    IsEditIconVisible = false;
                }

                if (NonTobaccoGridItemSource.Count > 0)
                {
                    LoadNonTobaccoGrid = true;
                    IsNonTobaccoGridVisible = true;
                    TotalNonTobacco = string.Format("${0}", totalNonTobacco.ToString("0.00"));
                }
                if (TobaccoGridItemSource.Count > 0)
                {
                    LoadTobaccoGrid = true;
                    IsTobaccoGridVisible = true;
                    TotalTobacco = string.Format("${0}", totalTobacco.ToString("0.00"));
                }
                if (RtnGridItemSource.Count > 0)
                {
                    LoadRtnGrid = true;
                    IsRtnGridVisible = true;
                    TotalRtnTobacco = string.Format("${0}", rtnTotal.ToString("0.00"));
                }
                if (DifGridItemSource.Count > 0)
                {
                    LoadDifGrid = true;
                    IsDifGridVisible = true;
                    TotalDifTobacco = string.Format("${0}", difTotal.ToString("0.00"));
                }

                IsLoading = false;
            }
            catch (Exception ex)
            {
                IsLoading = false;
                ErrorLogger.WriteToErrorLog(nameof(OrderHistoryDetailsPageViewModel), nameof(OnNavigatedToCommandHandler), ex.StackTrace);
            }
        }



        #endregion
    }
}
