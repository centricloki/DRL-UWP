using DRLMobile.Core.Helpers;
using DRLMobile.Core.Models.DataModels;
using DRLMobile.ExceptionHandler;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace DRLMobile.Core.Models.UIModels
{
    public class OrderHistoryDetailsPageUIModel : BaseModel
    {
        public CustomerMaster SelectedCustomer { get; set; }
        public OrderHistoryUIModel OrderHistoryModel { get; set; }
        public OrderMaster OrderMasterData { get; set; }
        public IEnumerable<OrderDetail> DbOrderDetailsData { get; set; }
        public IEnumerable<ProductMaster> DbProducts { get; set; }
        public IEnumerable<BrandData> DbBrandData { get; set; }
        public IEnumerable<CategoryMaster> DbCategoryMaster { get; set; }
        public IEnumerable<StyleData> DbStyleMaster { get; set; }

        private Dictionary<int, string> _stateDictionary;
        public Dictionary<int, string> StateDictionary
        {
            get { return _stateDictionary; }
            set { SetProperty(ref _stateDictionary, value); }
        }



        private string _customerNumber;
        public string CustomerNumber
        {
            get { return _customerNumber; }
            set { SetProperty(ref _customerNumber, value); }
        }


        private string _customerName;
        public string CustomerName
        {
            get { return _customerName; }
            set { SetProperty(ref _customerName, value); }
        }

        private string _physicalAddress;
        public string PhysicalAddress
        {
            get { return _physicalAddress; }
            set { SetProperty(ref _physicalAddress, value); }
        }

        private string _physicalCity;
        public string PhysicalCity
        {
            get { return _physicalCity; }
            set { SetProperty(ref _physicalCity, value); }
        }

        private string _selectedPhysicalState;
        public string SelectedPhysicalState
        {
            get { return _selectedPhysicalState; }
            set { SetProperty(ref _selectedPhysicalState, value); }
        }


        private string _physicalZip;
        public string PhysicalZip
        {
            get { return _physicalZip; }
            set { SetProperty(ref _physicalZip, value); }
        }



        private string _invoiceNumber;
        public string InvoiceNumber
        {
            get { return _invoiceNumber; }
            set { SetProperty(ref _invoiceNumber, value); }
        }


        private string _purchaseOrder;
        public string PurchaseOrder
        {
            get { return _purchaseOrder; }
            set { SetProperty(ref _purchaseOrder, value); }
        }


        private string _stateTobaccoLicense;
        public string StateTobaccoLicense
        {
            get { return _stateTobaccoLicense; }
            set { SetProperty(ref _stateTobaccoLicense, value); }
        }

        private string _retailerSalesTaxCertificate;
        public string RetailerSalesTaxCertificate
        {
            get { return _retailerSalesTaxCertificate; }
            set { SetProperty(ref _retailerSalesTaxCertificate, value); }
        }

        private string _retailerLicense;
        public string RetailerLicense
        {
            get { return _retailerLicense; }
            set { SetProperty(ref _retailerLicense, value); }
        }

        private string _emailTo;
        public string EmailTo
        {
            get { return _emailTo; }
            set { SetProperty(ref _emailTo, value); }
        }


        private string _customTaxStatement;
        public string CustomTaxStatement
        {
            get { return _customTaxStatement; }
            set { SetProperty(ref _customTaxStatement, value); }
        }


        private string _createdDate;
        public string CreatedDate
        {
            get { return _createdDate; }
            set { SetProperty(ref _createdDate, value); }
        }

        private string _orderMasterSellerRepTobacco;
        public string OrderMasterSellerRepTobacco
        {
            get { return _orderMasterSellerRepTobacco; }
            set { SetProperty(ref _orderMasterSellerRepTobacco, value); }
        }

        private DateTimeOffset _preBookDate;
        public DateTimeOffset PreBookDate
        {
            get { return _preBookDate; }
            set { SetProperty(ref _preBookDate, value); }
        }

        private string _salesType;
        public string SalesType
        {
            get { return _salesType; }
            set { SetProperty(ref _salesType, value); }
        }


        private string _comments;
        public string Comments
        {
            get { return _comments; }
            set { SetProperty(ref _comments, value); }
        }

        private DistributorMaster _uiDistributorMaster;
        public DistributorMaster UiDistributorMaster
        {
            get { return _uiDistributorMaster; }
            set { SetProperty(ref _uiDistributorMaster, value); }
        }
        private string _distributorState;
        public string DistributorState
        {
            get { return _distributorState; }
            set { SetProperty(ref _distributorState, value); }
        }

        private string _distributorZip;
        public string DistributorZip
        {
            get { return _distributorZip; }
            set { SetProperty(ref _distributorZip, value); }
        }
        private string _distributorName;
        public string DistributorName
        {
            get { return _distributorName; }
            set { SetProperty(ref _distributorName, value); }
        }
        private string _distributorCity;
        public string DistributorCity
        {
            get { return _distributorCity; }
            set { SetProperty(ref _distributorCity, value); }
        }
        private string _retailDistributorNumber;
        public string RetailDistributorNumber
        {
            get { return _retailDistributorNumber; }
            set { SetProperty(ref _retailDistributorNumber, value); }
        }



        public void PopulateUI(DistributorAssignUser distributor)
        {
            try
            {
                CustomerNumber = SelectedCustomer?.CustomerNumber;
                CustomerName = SelectedCustomer?.CustomerName;
                PhysicalAddress = OrderMasterData?.OrderAddress;
                PhysicalCity = OrderMasterData?.OrderCityId;
                PhysicalZip = OrderMasterData?.OrderZipCode;
                SelectedPhysicalState = Helpers.HelperMethods.GetValueFromIdNameDictionary(StateDictionary, OrderMasterData.OrderStateId);
                PurchaseOrder = OrderMasterData?.PurchaseOrderNumber;
                StateTobaccoLicense = OrderMasterData?.StateTobaccoLicence;
                RetailerSalesTaxCertificate = OrderMasterData?.RetailerSalesTaxCertificate;
                EmailTo = OrderMasterData?.EmailRecipients;
                CustomTaxStatement = OrderMasterData?.CustomStatement;
                RetailerLicense = OrderMasterData?.RetailerLicense;
                CreatedDate = OrderMasterData?.CreatedDate?.Split(' ')[0];
                OrderMasterSellerRepTobacco = OrderMasterData?.OrderMasterSellerRepTobacco;
                PreBookDate = DateTime.Parse(OrderMasterData?.PrebookShipDate, CultureInfo.InvariantCulture);
                SalesType = Helpers.HelperMethods.GetSalesTypeString(OrderMasterData?.SalesType);
                Comments = OrderMasterData?.CustomerComment;

                if (SalesType.Equals("Prebook"))
                {
                    RetailDistributorNumber = OrderMasterData?.RetailDistributorNumber;
                }

                DistributorCity = distributor != null ? distributor?.PhysicalAddressCityID : string.Empty;
                DistributorZip = distributor != null ? distributor.PhysicalAddressZipCode : string.Empty;
                DistributorName = distributor != null ? $"{distributor.DistributorID}, {distributor.PhysicalAddressStateName}{((!string.IsNullOrWhiteSpace(distributor?.AssignUserName))?$" - {distributor.AssignUserName}":"")}" : string.Empty;
                DistributorState = HelperMethods.GetValueFromIdNameDictionary(StateDictionary, distributor != null ? distributor.ShippingAddressStateID : 0);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog("OrderHistoryDetailsPageUIModel", nameof(PopulateUI), ex);
            }
        }
    }
}

