using DRLMobile.Core.Models.UIModels;

using System.Collections.Generic;

namespace DRLMobile.Core.Models.DataModels
{
    public class SyncDataModel : BaseModel
    {
        public string responsestatus { get; set; }
        public string errormsg { get; set; }
        public string lastsyncutcdate { get; set; }
        public string versionnumber { get; set; }
        public string toupload { get; set; }
        public List<CategoryMaster> catdata { get; set; }
        public List<BrandData> branddata { get; set; }
        public List<StyleData> styledata { get; set; }
        public List<ProductMaster> productdata { get; set; }
        public List<RoleMaster> roledata { get; set; }
        public List<object> recordresourcetype { get; set; }
        public List<StateMaster> statemasterdata { get; set; }
        public List<CityMaster> citymasterdata { get; set; }
        public List<RegionMaster> regionmasterdata { get; set; }
        public List<TerritoryMaster> territorymasterdata { get; set; }
        public List<UserMaster> userdata { get; set; }
        public List<ZoneMaster> zonemasterdata { get; set; }
        public List<CustomerMaster> customerdata { get; set; }
        public List<ProductAdditionalDocument> productdocument { get; set; }
        public List<CategoryProduct> categoryproduct { get; set; }
        public List<CustomerDocument> customerdocument { get; set; }
        public List<OrderMaster> orderdata { get; set; }
        public List<OrderDetail> orderdetails { get; set; }
        public List<CallActivityList> callactivitylist { get; set; }
        public List<ScheduledRoutes> scheduleroutes { get; set; }
        public List<ProductDistribution> customerproduct { get; set; }
        public List<SupplyChain> supplychain { get; set; }
        public List<Classification> accountclassification { get; set; }
        public List<object> distributer { get; set; }
        public List<RouteStations> routestation { get; set; }
        public ConfigurationDataUIModel configurationdata { get; set; }
        public List<DistributorMaster> distributordata { get; set; }
        public List<ContactMaster> contactdata { get; set; }
        public List<CustomerDistributor> CustomerDistributorData { get; set; }
        public List<OrderHistoryEmail> OrderHistoryEmailData { get; set; }
        public List<UserTaxStatement> UserTaxStatementdata { get; set; }
        public List<object> DeleteRoutes { get; set; }
        public List<LnkRackItems> LnkRackItem { get; set; }
        public List<LnkPopItems> LnkPopItem { get; set; }
        public List<Configuration> configurations { get; set; }
        public List<RankMaster> ranks { get; set; }
        public List<PositionMaster> positions { get; set; }
        public List<VripMaster> Vrip { get; set; }
        public List<TravelMaster> Travel { get; set; }
        public List<Favorite> FavoriteEntity { get; set; }
        public List<UserActivityType> UserActivityTypeEntity { get; set; }
        public List<CustomerActivityType> CustomerActivityTypeEntity { get; set; }
        public List<CustomerDocumentType> DocumentTypeEntity { get; set; }
    }
}
