using Newtonsoft.Json;

using SQLite;

using System;

using Windows.UI.Xaml;

namespace DRLMobile.Core.Models.DataModels
{
    public class DistributorMaster : BaseModel
    {
        [JsonProperty("customerid")]
        public int CustomerID { get; set; }

        [JsonProperty("customername")]
        public string CustomerName { get; set; }

        [JsonProperty("accounttype")]
        public int AccountType { get; set; }

        [JsonProperty("distributorid")]
        public string DistributorID { get; set; }

        [JsonProperty("accountresponsibility")]
        public string AccountResponsibility { get; set; }

        [JsonProperty("fax")]
        public string Fax { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("emailid")]
        public string EmailID { get; set; }

        [JsonProperty("team")]
        public string Team { get; set; }

        public string TerritoryID { get; set; }

        [JsonProperty("broker")]
        public string Broker { get; set; }

        [JsonProperty("associatedinternalsalesguy")]
        public string AssociaatedInternalSalesGuy { get; set; }

        public string AccountClassification { get; set; }

        [JsonProperty("buyer")]
        public string Buyer { get; set; }
        public string StoreCount { get; set; }

        [JsonProperty("supplychainid")]
        public int SupplyChainID { get; set; }

        [JsonProperty("managername")]
        public string ManagerName { get; set; }

        [JsonProperty("generalcomments")]
        public string GeneralComments { get; set; }
        public string ImportedFrom { get; set; }

        [JsonProperty("devicecustomerid")]
        public string DeviceCustomerID { get; set; }
        public int IsExported { get; set; }

        [JsonProperty("physicaladdress")]
        public string PhysicalAddress { get; set; }

        [JsonProperty("physicaladdresscityid")]
        public string PhysicalAddressCityID { get; set; }

        [JsonProperty("physicaladdressstateid")]
        public int PhysicalAddressStateID { get; set; }

        [JsonProperty("physicaladdresszipcode")]
        public string PhysicalAddressZipCode { get; set; }

        [JsonProperty("mailingaddress")]
        public string MailingAddress { get; set; }

        [JsonProperty("mailingaddresscityid")]
        public string MailingAddressCityID { get; set; }

        [JsonProperty("mailingaddressstateid")]
        public int MailingAddressStateID { get; set; }

        [JsonProperty("mailingaddresszipcode")]
        public string MailingAddressZipID { get; set; }

        [JsonProperty("shippingaddress")]
        public string ShippingAddress { get; set; }

        [JsonProperty("shippingaddresscityid")]
        public string ShippingAddressCityID { get; set; }

        [JsonProperty("shippingaddressstateid")]
        public int ShippingAddressStateID { get; set; }

        [JsonProperty("shippingaddresszipcode")]
        public string ShippingAddressZipCode { get; set; }

        [JsonProperty("createdate")]
        public string CreatedDate { get; set; }
        public string CreatedBy { get; set; }

        [JsonProperty("updatedate")]
        public string UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }

        [JsonProperty("regionid")]
        public int RegionId { get; set; }

        [JsonProperty("zoneid")]
        public int ZoneId { get; set; }
        public int CreatePermanent { get; set; }

        [JsonProperty("customernumber")]
        public string CustomerNumber { get; set; }
        public int CopiedFields { get; set; }
        public string Rank { get; set; }
        public int isDeleted { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string YTD_CurrentYear { get; set; }
        public string YTD_LastYear { get; set; }
        public string YTD_CasesCurrentYear { get; set; }
        public string YTD_CasesLastYear { get; set; }
        public string PercentVarianceYTD { get; set; }
        public string PercentVarianceMTD { get; set; }

        [JsonProperty("assignuserid")]
        public string AssignUserId { get; set; }

        [JsonProperty("statetobaccolicense")]
        public string StateTobaccoLicense { get; set; }

        [JsonProperty("retailerlicense")]
        public string RetailerLicense { get; set; }

        [JsonProperty("retailersalestaxcertificate")]
        public string RetailerSalesTaxCertificate { get; set; }
        public string ContactName { get; set; }
        public string ContactRole { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }

        [JsonProperty("buytype")]
        public string BuyType { get; set; }

        private bool _isSelected;
        [Ignore]
        public bool IsSelected
        {
            get { return _isSelected; }
            set { SetProperty(ref _isSelected, value); }
        }

        private Visibility _isDeleteIconVisibile;
        [Ignore]
        public Visibility IsDeleteIconVisibile
        {
            get { return _isDeleteIconVisibile; }
            set { SetProperty(ref _isDeleteIconVisibile, value); }
        }

        private string _stateName;
        [Ignore]
        public string StateName
        {
            get { return _stateName; }
            set { SetProperty(ref _stateName, value); }
        }

        private int _priority;
        [Ignore]
        public int Priority
        {
            get { return _priority; }
            set { SetProperty(ref _priority, value); }
        }


        private int _territoryidFromServer;
        [Ignore]
        [JsonProperty("territoryid")]
        public int TerritoryidFromServer
        {
            get { return _territoryidFromServer; }
            set
            {
                _territoryidFromServer = value;

                TerritoryID = Convert.ToString(value);
            }
        }

        private int _accountclassificationFromServer;
        [Ignore]
        [JsonProperty("accountclassification")]
        public int AccountclassificationFromServer
        {
            get { return _accountclassificationFromServer; }
            set
            {
                _accountclassificationFromServer = value;

                AccountClassification = Convert.ToString(value);
            }
        }

        private int _storecountFromServer;
        [Ignore]
        [JsonProperty("storecount")]
        public int StorecountFromServer
        {
            get { return _storecountFromServer; }
            set
            {
                _storecountFromServer = value;

                StoreCount = Convert.ToString(value);
            }
        }

        private bool _iscreatepermanentFromServer;
        [Ignore]
        [JsonProperty("iscreatepermanent")]
        public bool IscreatepermanentFromServer
        {
            get { return _iscreatepermanentFromServer; }
            set
            {
                _iscreatepermanentFromServer = value;

                CreatePermanent = value ? 1 : 0;
            }
        }

        private bool _deletedFromServer;
        [Ignore]
        [JsonProperty("deleted")]
        public bool DeletedFromServer
        {
            get { return _deletedFromServer; }
            set
            {
                _deletedFromServer = value;

                isDeleted = value ? 1 : 0;
            }
        }

        private bool _isexportedFromServer;
        [Ignore]
        [JsonProperty("isexported")]
        public bool IsExportedFromServer
        {
            get { return _isexportedFromServer; }
            set
            {
                _isexportedFromServer = value;

                IsExported = value ? 1 : 0;
            }
        }

        private int _importedfromFromServer;
        [Ignore]
        [JsonProperty("importedfrom")]
        public int ImportedfromFromServer
        {
            get { return _importedfromFromServer; }
            set
            {
                _importedfromFromServer = value;

                ImportedFrom = Convert.ToString(value);
            }
        }

        private long _CreatedByFromServer;
        [Ignore]
        [JsonProperty("createdby")]
        public long CreatedByFromServer
        {
            get { return _CreatedByFromServer; }
            set
            {
                _CreatedByFromServer = value;

                CreatedBy = Convert.ToString(value);
            }
        }

        private long _UpdatedByFromServer;
        [Ignore]
        [JsonProperty("updatedby")]
        public long UpdatedByFromServer
        {
            get { return _UpdatedByFromServer; }
            set
            {
                _UpdatedByFromServer = value;

                UpdatedBy = Convert.ToString(value);
            }
        }

        private double _longitudeFromServer;
        [Ignore]
        [JsonProperty("longitude")]
        public double LongitudeFromServer
        {
            get { return _longitudeFromServer; }
            set
            {
                _longitudeFromServer = value;

                Longitude = Convert.ToString(value);
            }
        }

        private double _latitudeFromServer;
        [Ignore]
        [JsonProperty("latitude")]
        public double LatitudeFromServer
        {
            get { return _latitudeFromServer; }
            set
            {
                _latitudeFromServer = value;

                Latitude = Convert.ToString(value);
            }
        }

        private decimal _ytdcurrentyearFromServer;
        [Ignore]
        [JsonProperty("ytdcurrentyear")]
        public decimal YtdcurrentyearFromServer
        {
            get { return _ytdcurrentyearFromServer; }
            set
            {
                _ytdcurrentyearFromServer = value;

                YTD_CurrentYear = Convert.ToString(value);
            }
        }

        private decimal _ytdlastyearFromServer;
        [Ignore]
        [JsonProperty("ytdlastyear")]
        public decimal YtdlastearFromServer
        {
            get { return _ytdlastyearFromServer; }
            set
            {
                _ytdlastyearFromServer = value;

                YTD_LastYear = Convert.ToString(value);
            }
        }

        private int _ytdcasescurrentyearFromServer;
        [Ignore]
        [JsonProperty("ytdcasescurrentyear")]
        public int YtdcasescurrentyearFromServer
        {
            get { return _ytdcasescurrentyearFromServer; }
            set
            {
                _ytdcasescurrentyearFromServer = value;

                YTD_CasesCurrentYear = Convert.ToString(value);
            }
        }

        private int _ytdcaseslastyearFromServer;
        [Ignore]
        [JsonProperty("ytdcaseslastyear")]
        public int YtdcaseslastyearFromServer
        {
            get { return _ytdcaseslastyearFromServer; }
            set
            {
                _ytdcaseslastyearFromServer = value;

                YTD_CasesLastYear = Convert.ToString(value);
            }
        }

        private decimal _varianceytdFromServer;
        [Ignore]
        [JsonProperty("varianceytd")]
        public decimal VarianceytdFromServer
        {
            get { return _varianceytdFromServer; }
            set
            {
                _varianceytdFromServer = value;

                PercentVarianceYTD = Convert.ToString(value);
            }
        }

        private decimal _variancemtdFromServer;
        [Ignore]
        [JsonProperty("variancemtd")]
        public decimal VariancemtdFromServer
        {
            get { return _variancemtdFromServer; }
            set
            {
                _variancemtdFromServer = value;

                PercentVarianceMTD = Convert.ToString(value);
            }
        }
        public int IsPayer { get; set; }
        private bool _isPayerFromServer;
        [Ignore]
        [JsonProperty("ispayer")]
        public bool IsPayerFromServer
        {
            get { return _isPayerFromServer; }
            set
            {
                _isPayerFromServer = value;

                IsPayer = value ? 1 : 0;
            }
        }
    }
    public class DistributorAssignUser : DistributorMaster
    {
        private string _assignUserName;
        public string AssignUserName
        {
            get { return _assignUserName; }
            set { SetProperty(ref _assignUserName, value); }
        }
        private string _physicalAddressStateName;
        public string PhysicalAddressStateName
        {
            get { return _physicalAddressStateName; }
            set { SetProperty(ref _physicalAddressStateName, value); }
        }
        public string FullName
        {
            get { return $"{DistributorID}, {StateName}"; }
        }
    }
}
