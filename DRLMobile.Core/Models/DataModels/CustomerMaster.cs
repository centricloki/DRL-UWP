using System;

using DRLMobile.Core.Models.UIModels;

using Newtonsoft.Json;

using SQLite;

namespace DRLMobile.Core.Models.DataModels
{
    public class CustomerMaster
    {
        [PrimaryKey]
        [JsonProperty("customerid")]
        public int? CustomerID { get; set; }

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
        public string Team { get; set; }
        public string TerritoryID { get; set; }

        [JsonProperty("broker")]
        public string Broker { get; set; }

        [JsonProperty("associatedinternalsalesguy")]
        public string AssociaatedInternalSalesGuy { get; set; }

        [JsonProperty("accountclassification")]
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

        [JsonProperty("copiedfields")]
        public int CopiedFields { get; set; }

        [JsonProperty("ranking")]
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

        [JsonProperty("taxstatement")]
        public string TaxStatement { get; set; }

        [JsonProperty("contactname")]
        public string ContactName { get; set; }

        [JsonProperty("contactrole")]
        public string ContactRole { get; set; }

        [JsonProperty("contactemail")]
        public string ContactEmail { get; set; }

        [JsonProperty("contactphone")]
        public string ContactPhone { get; set; }

        [JsonProperty("LastCallActivityDate")]
        public string LastCallActivityDate { get; set; }

        [JsonIgnore]
        public string Cslyr { get; set; }

        [JsonIgnore]
        public string Target { get; set; }

        [JsonIgnore]
        public string Csytd { get; set; }

        [JsonIgnore]
        public string CSNeeded { get; set; }
        public string Qualified { get; set; }
        public string VripYear { get; set; }

        [JsonIgnore]
        public string EarnedPoints { get; set; }

        [JsonIgnore]
        public string BonusPoints { get; set; }

        [JsonIgnore]
        public string NetPoints { get; set; }

        [JsonIgnore]
        public string NeededPoint { get; set; }

        public string TravelYear { get; set; }

        public string BuyType { get; set; }

        public string TradeType { get; set; }

        public string KeyAccountManagerName { get; set; }

        [JsonIgnore]
        public byte VripOrTravel { get; set; }

        [JsonIgnore]
        public int IsParent { get; set; }

        public int? Parent { get; set; }

        public string Awards { get; set; }

        public string Next { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        [JsonIgnore]
        public string Rebate { get; set; }

        public string PayerName { get; set; }

        public string ParentName { get; set; }

        [Ignore]
        public string StateData { get; set; }

        [Ignore]
        public CityMaster CityData { get; set; }

        [Ignore]
        public Classification ClassificationData { get; set; }

        private int _teamFromServer;
        [Ignore]
        [JsonProperty("team")]
        public int TeamFromServer
        {
            get { return _teamFromServer; }
            set
            {
                _teamFromServer = value;

                Team = Convert.ToString(value);
            }
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

        private bool _isexportedFromServer;
        [Ignore]
        [JsonProperty("isexported")]
        public bool IsexportedFromServer
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

        private int _createdbyFromServer;
        [Ignore]
        [JsonProperty("createdby")]
        public int CreatedbyFromServer
        {
            get { return _createdbyFromServer; }
            set
            {
                _createdbyFromServer = value;

                CreatedBy = Convert.ToString(value);
            }
        }

        private int _updatedbyFromServer;
        [Ignore]
        [JsonProperty("updatedby")]
        public int UpdatedbyFromServer
        {
            get { return _updatedbyFromServer; }
            set
            {
                _updatedbyFromServer = value;

                UpdatedBy = Convert.ToString(value);
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

        private bool _isdeletedFromServer;
        [Ignore]
        [JsonProperty("isdeleted")]
        public bool IsdeletedFromServer
        {
            get { return _isdeletedFromServer; }
            set
            {
                _isdeletedFromServer = value;

                isDeleted = value ? 1 : 0;
            }
        }

        private bool _IsParentFromServer;
        [Ignore]
        [JsonProperty("IsParent")]
        public bool IsParentFromServer
        {
            get { return _IsParentFromServer; }
            set
            {
                _IsParentFromServer = value;

                IsParent = value ? 1 : 0;
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

        private decimal? _RebateFromServer;
        [Ignore]
        [JsonProperty("Rebate")]
        public decimal? RebateFromServer
        {
            get { return _RebateFromServer; }
            set
            {
                _RebateFromServer = value;

                Rebate = Convert.ToString(value);
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

        private bool _VripOrTravelFromServer;
        [Ignore]
        [JsonProperty("VripOrTravel")]
        public bool VripOrTravelFromServer
        {
            get { return _VripOrTravelFromServer; }
            set
            {
                _VripOrTravelFromServer = value;

                VripOrTravel = value ? (byte)1 : (byte)0;
            }
        }

        private int? _CslyrFromServer;
        [Ignore]
        [JsonProperty("Cslyr")]
        public int? CslyrFromServer
        {
            get { return _CslyrFromServer; }
            set
            {
                _CslyrFromServer = value;

                Cslyr = Convert.ToString(value);
            }
        }

        private int? _TargetFromServer;
        [Ignore]
        [JsonProperty("Target")]
        public int? TargetFromServer
        {
            get { return _TargetFromServer; }
            set
            {
                _TargetFromServer = value;

                Target = Convert.ToString(value);
            }
        }

        private int? _CSNeededFromServer;
        [Ignore]
        [JsonProperty("CSNeeded")]
        public int? CSNeededFromServer
        {
            get { return _CSNeededFromServer; }
            set
            {
                _CSNeededFromServer = value;

                CSNeeded = Convert.ToString(value);
            }
        }

        private decimal? _CsytdFromServer;
        [Ignore]
        [JsonProperty("Csytd")]
        public decimal? CsytdFromServer
        {
            get { return _CsytdFromServer; }
            set
            {
                _CsytdFromServer = value;

                Csytd = Convert.ToString(value);
            }
        }

        private int? _EarnedPointsFromServer;
        [Ignore]
        [JsonProperty("EarnedPoints")]
        public int? EarnedPointsFromServer
        {
            get { return _EarnedPointsFromServer; }
            set
            {
                _EarnedPointsFromServer = value;

                EarnedPoints = Convert.ToString(value);
            }
        }

        private int? _BonusPointsFromServer;
        [Ignore]
        [JsonProperty("BonusPoints")]
        public int? BonusPointsFromServer
        {
            get { return _BonusPointsFromServer; }
            set
            {
                _BonusPointsFromServer = value;

                BonusPoints = Convert.ToString(value);
            }
        }

        private int? _NetPointsFromServer;
        [Ignore]
        [JsonProperty("NetPoints")]
        public int? NetPointsFromServer
        {
            get { return _NetPointsFromServer; }
            set
            {
                _NetPointsFromServer = value;

                NetPoints = Convert.ToString(value);
            }
        }

        private int? _NeededPointFromServer;
        [Ignore]
        [JsonProperty("NeededPoint")]
        public int? NeededPointFromServer
        {
            get { return _NeededPointFromServer; }
            set
            {
                _NeededPointFromServer = value;

                NeededPoint = Convert.ToString(value);
            }
        }

        //private DateTime? _LastCallActivityDateFromServer;
        //[Ignore]
        //[JsonProperty("LastCallActivityDate")]
        //public DateTime? LastCallActivityDateFromServer
        //{
        //    get { return _LastCallActivityDateFromServer; }
        //    set
        //    {
        //        _LastCallActivityDateFromServer = value;

        //        LastCallActivityDate = Convert.ToString(value);
        //    }
        //}



        private string _territoryName;
        [JsonIgnore]
        [Ignore]
        public string TerritoryName
        {
            get { return _territoryName; }
            set { _territoryName = value; }
        }

        private string _firstName;
        [JsonIgnore]
        [Ignore]
        public string FirstName
        {
            get { return _firstName; }
            set { _firstName = value; }
        }

        private string _lastName;
        [JsonIgnore]
        [Ignore]
        public string LastName
        {
            get { return _lastName; }
            set { _lastName = value; }
        }



        [JsonIgnore]
        public int IsAssociatedCustomer { get; set; }

        private bool _IsAssociatedCustomerFromServer;
        [Ignore]
        [JsonProperty("IsAssociatedCustomer")]
        public bool IsAssociatedCustomerFromServer
        {
            get { return _IsAssociatedCustomerFromServer; }
            set
            {
                _IsAssociatedCustomerFromServer = value;

                IsAssociatedCustomer = value ? 1 : 0;
            }
        }

        public string ActivityComment { get; set; }
        public string ActivityCreator { get; set; }
        public string OrderDeliveryWeekDays { get; set; }

        public CustomerPageUIModel CopyToUIModel()
        {
            var uiModel = new CustomerPageUIModel()
            {
                CustomerId = this.CustomerID.Value,
                CustomerName = this.CustomerName,
                CustomerNumber = this.CustomerNumber,
                StoreType = ClassificationData?.AccountClassificationName,
                Rank = this.Rank,
                Address = this.PhysicalAddress,
                City = this.PhysicalAddressCityID,
                State = StateData ?? string.Empty,
                //LastCallDate = Helpers.DateTimeHelper.ConvertStringDateToMM_DD_YYYY(this.LastCallActivityDate),
                LastCallDate = this.LastCallActivityDate,
                VripOrTravel = this.VripOrTravel,
                IsEllipsisVisible = VripOrTravel == 1,
                AccountType = this.AccountType,
                DeviceCustomerId = this.DeviceCustomerID,
                StateId = PhysicalAddressStateID,
                TerritoryName = TerritoryName
            };
            //IsEllipsisVisible= VripOrTravel==1,
            return uiModel;
        }


        //        [JsonProperty("team")]
    }
}