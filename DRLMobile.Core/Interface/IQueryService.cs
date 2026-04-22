using DRLMobile.Core.Enums;
using DRLMobile.Core.Models.DataModels;
using DRLMobile.Core.Models.FedExAddressValidationModels;
using DRLMobile.Core.Models.UIModels;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace DRLMobile.Core.Interface
{
    public interface IQueryService
    {
        Task<bool> AddLoggedInUserAsZeroCustomer(string userName, string pin);
        Task<List<CustomerPageUIModel>> GetChainLocationCustomers(int headQuarter);
        Task<List<CustomerPageUIModel>> GetHeadQuarterCustomers(int headQuarter);
        Task<List<CustomerPageUIModel>> GetCustomerPageData(int areaId, int roleId, bool top1000, string userId, CustomerTeamType teamType, Dictionary<string, string> filters = null);
        Task<List<CustomerPageUIModel>> GetCustomerPageData(bool loadAllData);
        Task<List<CustomerPageUIModel>> GetUpdatedCustomers(string updatedDate, int areaId, int roleId);
        Task<List<CustomerPageModel>> CleanDeletedCustomers();
        Task<List<SRCProductUIModel>> GetSRCProductsPageGridData(string CurrentOrderId);
        Task<List<SRCProductUIModel>> GetSRCProductsAsync(string CurrentOrderId);
        Task<int> GetCartProductQuantityAsync(string currentOrderId, int productId);
        Task<SavedCustomerInfoUIModel> GetSavedCustomerInformation(int customerId);
        Task<List<BrandUIModel>> GetFilteredBrandDataWhenCategoryIsSelected(List<int> selectedCategoryIdsList);
        Task<List<StyleUIModel>> GetFilteredStyleDataWhenCategoryIsSelected(List<int> selectedCategoryIdsList);
        Task<List<CategoryUIModel>> GetFilteredCategoryDataWhenStyleIsSelected(List<int> selectedStyleIdsList);
        Task<List<BrandUIModel>> GetFilteredBrandDataWhenStyleIsSelected(List<int> selectedStyleIdsList);

        Task<List<CategoryUIModel>> GetFilteredCategoryDataWhenBrandIsSelected(List<int> selectedBrandIdsList);
        //Task<List<CategoryUIModel>> GetCategoriesBasedOnBrandAsync(string selectedBrandIds);

        Task<List<StyleUIModel>> GetFilteredStyleDataWhenBrandIsSelected(List<int> selectedBrandIdsList);

        Task<List<StyleUIModel>> GetFilteredStyleDataWhenCategoryAndBrandSelected(List<int> selectedCategoryIdsList, List<int> selectedBrandIdsList);

        Task<List<BrandUIModel>> GetFilteredBrandDataWhenCategoryAndStyleSelected(List<int> selectedCategoryIdsList, List<int> selectedStyleIdsList);

        Task<List<CategoryUIModel>> GetFilteredCategoryDataWhenBrandAndStyleSelected(List<int> selectedBrandIdsList, List<int> selectedStyleIdsList);

        Task<List<SRCProductUIModel>> GetSRCProductsDataOnAllSelectedFilters(List<int> selectedCategoryIdsList, List<int> selectedBrandIdsList, List<int> selectedStyleIdsList, string CurrentOrderId);

        Task<List<SRCProductUIModel>> GetFilteredSRCProducts(IEnumerable<int> selectedCategories, IEnumerable<int> selectedBrands, IEnumerable<int> selectedStyles, string CurrentOrderId);
        Task<bool> DownloadDataOnPartialSync(SyncDataModel downloadedData, string userid);

        Task<bool> UploadDataOnPartialSync(string userName, string pin, string lastSyncDate);

        Task<List<CategoryUIModel>> GetCategoriesForProductsList();

        Task<List<CategoryUIModel>> GetFilterCategoriesAsync(string ids);

        Task<List<BrandUIModel>> GetBrandsAsync();
        Task<List<BrandUIModel>> GetFilterBrandsAsync(string ids);

        Task<List<StyleUIModel>> GetStylesDataAsync();
        Task<List<StyleUIModel>> GetFilterStylesAsync(string ids);

        Task<CustomerDetailScreenUIModel> GetCustomerDetailsDataForViewAndEditAsync(int customerId);

        Task UpdateOrInsertCustomerData(CustomerMaster data, string userName, string pin);

        Task InsertOrUpdateDistributorForCustomerProfilePage(ICollection<DistributorMaster> deletedRecords, ICollection<DistributorAssignUser> updadtedRecords, string userName, string pin, string selectedCustomerDeviceId);

        Task InsertOrUpdateContactForCustomerProfilePage(ICollection<ContactMaster> deletedRecords, ICollection<ContactMaster> updatedRecords, string userName, string pin, string selectedCustomerId);

        Task<LoggedInUserDetailsUIModel> GetLoggedInUserInformation(int userId);

        Task<AddCustomerUiModel> GetAddCustomerPageData(string userName, string pin);

        Task<CustomerMaster> AddNewCustomerDataToDatabase(CustomerMaster customer, ICollection<DistributorAssignUser> distributions, UserMaster loggedInUser);
        Task<CustomerMaster> GetCartCustomerInfoAsync(int savedCustomerId);

        Task<OrderDetail> InsertOrUpdateOrderDetail(OrderDetail orderDetail);

        Task<OrderMaster> InsertOrUpdateOrderMaster(OrderMaster orderMaster, bool reInsert = true);

        Task<bool> DeleteOrderDetail(int productId, string deviceOrderId);

        Task DeleteCartDetailOnPlaceOrder(string deviceOrderId);

        Task<List<OrderDetailUIModel>> GetCartProductDetailsData(string CurrentOrderId);

        Task<CartDetailsUIModel> GetCartDetailsDataForCartScreen(string CurrentOrderId, int CustomerId);

        Task<ObservableCollection<CustomerDocumentUIModel>> GetCustomerDocuments(int customerId);

        Task<CustomerDocument> InsertOrUpdateCustomerDocument(CustomerDocumentUIModel document);

        Task<ProductDetailUiModel> GetProductAdditionalDocumentData(int productId);

        Task InsertFavorite(List<Favorite> favoriteList);

        /// Task DeleteFavorite(int productId);

        Task<List<FavoriteUiModel>> GetFavoriteGridData();
        Task<List<CategoryMaster>> GetCategoryData();
        Task<RetailTransactionUIModel> GetRetailTransactionData(int customerId, string userName, string pin);
        Task<bool> InsertOrUpdateDataOnConfirmOrder(RetailTransactionUIModel retailsTransactionData);
        Task<List<int>> GetCartEligibleProducts(bool? IsCreditRequestOrder, bool? IsDistributionOptionClicked);
        Task<IEnumerable<string>> GetBrandImageURL();
        Task<string> GetUserCustomerDeviceId(string userCustomername);
        Task<Dictionary<int, string>> GetStateDict();
        Task<Dictionary<int, Classification>> GetClassificationDict();
        Task<List<TravelUiModel>> GetTravelDataForUser(string year);
        Task<List<VripUiModel>> GetVripDataForUser(string year);
        Task<List<string>> GetTravelProgramYearFromVripTravelData();
        Task<List<string>> GetVripProgramYearFromVripTravelData();
        Task<IEnumerable<OrderHistoryUIModel>> GetOrderHistoryDataAsync(string customerId, string customerType);
        Task<IEnumerable<ItemHistoryUIModel>> GetItemHistoryDataAsync(string customerId, string customerType);
        Task<bool> GetTravelVripPromotionContactDataForCustomer(string customerId);
        Task<TravelUiModel> GetTravelDataForCustomer(string customerId);
        Task<VripUiModel> GetVripDataForCustomer(string customerId);
        Task<List<PromotionUiModel>> GetPromotionDataForCustomer(string customerId);
        Task<List<ContractUiModel>> GetContractsDataForCustomer(string customerId);
        Task<OrderHistoryDetailsPageUIModel> GetOrderHistoryDetailData(OrderHistoryDetailsPageUIModel referenceObject);
        Task<bool> DeleteOrderDetailFromHistoryPage(OrderDetail detail);
        Task<UserMaster> GetUserData(string userName, string pin);
        Task<DistributorAssignUser> GetDistributorFromId(int id);
        Task<IEnumerable<OrderDetail>> GetOrderDetailsFromOrderId(string deviceOrderId);
        Task<List<RackOrderUiModel>> GetRackOrderListData();
        Task<Dictionary<int, int>> GetCartItemsCount(string currentOrderId);
        Task<bool> DeleteAllCartItems(string deviceOrderId);
        Task<List<RackOrderCartUiModel>> GetRackOrderCartData(int brandId, string CurrentDeviceOrderId);
        Task<List<OrderDetailUIModel>> GetRackCartProductsData(string CurrentDeviceOrderId);
        Task<List<PopOrderCartUiModel>> GetFilteredPopOrderCartData(string CurrentDeviceOrderId, int[] HierarchyTagArray, int HierarchyTag);
        Task<List<BrandUIModel>> GetFilteredComboboxData(int[] HierarchyTagArray, int HierarchyTag);
        Task<Dictionary<int, int>> GetCartsProductsIdQuantity(string currentOrderId);
        Task<PopOrderPageUiModel> GetPopOrderCartData(string CurrentDeviceOrderId);
        Task<List<PopOrderCartUiModel>> GetPopOrderRemainingListData(string CurrentDeviceOrderId);
        Task<List<ActivityForAllCustomerUIModel>> GetCallActivitiesOfAllCustomersForLoggedInUser(string territoryIds, bool loadAllData);
        Task<List<ActivityForAllCustomerUIModel>> GetCallActivitiesOfAllCustomersForNationalAndZoneAndRegionManagers(string territoryIds, bool loadAllData);
        Task<List<ActivityForAllCustomerUIModel>> GetCallActivitiesOfAllCustomers(bool loadAllData);
        Task<List<ActivityForIndividualCustomerUIModel>> GetCallActivitiesOfSelectedCustomerForLoggedInUser(string deviceCustomerId);
        Task<List<ActivityForIndividualCustomerUIModel>> GetCallActivitiesOfSelectedArea(string deviceCustomerId, int selectedArea, int roleId);
        Task<List<CustomerMaster>> GetCustomerListForActivity();
        Task<IEnumerable<string>> GetUserActivityTypeAsync();
        Task<IEnumerable<string>> GetCustomerActivityTypeAsync();
        Task<IEnumerable<string>> GetDocumentTypeAsync();
        Task<List<TerritoryMaster>> GetTerritoryFromIds(string territoryIdsInCvs);
        Task<CallActivityList> AddCallActivityToDb(CallActivityList activity);
        Task<bool> AddCallActivityCallDate(CallActivityList activity, string date);
        Task<List<UserTaxStatementUiModel>> GetUserTaxStatementList();
        Task<UserTaxStatement> InsertUserTaxStatement(UserTaxStatement userTaxStatement);
        Task<UserTaxStatement> DeleteUserTaxStatement(UserTaxStatement userTaxStatement);
        Task<UserTaxStatement> UpdateUserTaxStatement(UserTaxStatement userTaxStatement);
        Task<CallActivityList> GetIndidualActivityById(string activityId);
        Task<OrderMaster> GetOrderFromOrderMasterFromDeviceOrderId(string deviceOrderId);
        Task<UserMaster> GetUserFromUserId(int userId);
        Task<List<OrderDetail>> GetOrderDetailsFromDeviceOrderId(string deviceOrderId);
        Task<List<OrderHistoryDetailsGridUIModel>> GetOrderDetailsByDeviceOrderId(string deviceOrderId);
        Task<IEnumerable<ProductMaster>> GetAllProductListFromProductMaster();
        Task<IEnumerable<BrandData>> GetAllBrandDataFromBrandMaster();
        Task<IEnumerable<StyleData>> GetAllStylesFromStyleMaster();
        Task<IEnumerable<CategoryMaster>> GetAllCategoryFromCategoryMaster();
        Task<OrderDetail> UpdateOrderDetail(OrderDetail orderDetail, string currentOrderId);
        Task UpdateLastCallActivityDateForCustomerMaster(string deviceCustomerId, string updatedCallActivityDate);
        Task UpdateDateFromActivityCustomerMaster(string deviceCustomerId, string activityType, string updatedCallActivityDate);
        Task<string> GetOrderGrandTotalFromOrderDeviceId(string orderDeviceId);
        Task<IEnumerable<ZoneMasterUIModel>> GetZoneFromZoneId(int? id);
        Task<IEnumerable<RegionMaster>> GetRegionFromRegionId(int id);
        Task<IEnumerable<TerritoryMaster>> GetTerritoryFromDefferedTerritoryId(int id);
        Task<ObservableCollection<CityMasterUIModel>> GetCityMasterDataWhichHasCustomersAssociated();
        Task<IEnumerable<RegionMasterUIModel>> GetRegionsOnBasisOfZoneIdsAndPresentCustomers(IEnumerable<int> zoneIds);
        Task<IEnumerable<ZoneMasterUIModel>> GetZonesOnBasisOfCustomers();
        Task<IEnumerable<TerritoryMasterUIModel>> GetTerritoryOnBasisOfRegionIdsAndPresentCustomers(IEnumerable<int> regionIds);
        Task<IEnumerable<MapCustomerData>> GetMapDataForTradeTypeAndRankAndCallDate(int? zoneId, int? regionId, int? territoryId, string stateID, string cityName);
        Task<IEnumerable<MapCustomerData>> GetMapDataForCashSales(int? zoneId, int? regionId, int? territoryId, string stateID, string cityName, DateTime? startDateTime, DateTime? endDateTime);
        Task<IEnumerable<MapCustomerData>> GetMapDataForCashSalesForNationalAndZM(int? zoneId, int? regionId, int? territoryId, string stateID, string cityName, DateTime? startDateTime, DateTime? endDateTime);
        Task<List<RouteListUIModel>> GetScheduledRouteListData();
        Task<IEnumerable<MapCustomerData>> GetMapsDataForItemsFilter(int? zoneId, int? regionId, int? territoryId, string stateID, string cityName, int productId);
        Task<IEnumerable<ProductMaster>> GetProductDetailsOnBasisOfProductName(string name);
        Task<CustomerMaster> GetMapPopupCutomerData(int customerId, string customerDeviceId);
        Task<IEnumerable<ActivityForAllCustomerUIModel>> GetMapPopupActivityData(string deviceCustomerId);
        Task<bool> UpdateRoutesForCurrentUser(int userId);
        Task<CallActivityList> GetCurrentCustomerCallActivityDataAsync(string CustomerID, string OrderID);
        Task UpdateCallActivityDataOnUpdateOrderFromHistory(CallActivityList callActivity);
        Task<ScheduledRoutes> AddRouteToDb(ScheduledRoutes route);
        Task AddRouteStationsToDb(List<int> customerIds, int routeId, string routeDeviceId, int userId);
        Task<IEnumerable<int>> GetCustomerIdsFromRouteStationTableByRouteId(int routeId);
        Task<bool> DeleteAllRouteStationsForRouteId(int routeId);
        Task<bool> UpdateScheduleRoutesWhenRouteIdIs(string routeName, string routeBrief, string startDate, string endDate, int userId, int assignedToTSM, int routeId);
        Task<IEnumerable<MapPopAddToRouteListModel>> GetRoutesForMapPopup(int customerId);
        Task<List<ViewRouteDetailsUIModel>> GetRouteDetailsToView(string DeviceRouteId);
        Task<bool> DeleteScheduledRoute(string DeviceRouteId);
        Task<Dictionary<int, string>> GetStateDictionaryWhichHasCustomersAssociated();
        Task<List<UserMaster>> GetListOfTerritoryManagersForTerritories(string territoryIds);
        Task<List<UserMaster>> GetListOfTerritoryManagersForRegion(int regionId);
        Task<List<UserMaster>> GetListOfTerritoryManagersForZone(int zoneId);
        Task UpdateOrderGrandTotal(string grandTotal, int orderId);
        Task<bool> ClearCart(string deviceOrderId);
        Task<bool> DeleteActivity(string deviceActivityId);
        Task<bool> DeleteActivityFromDeviceOrderId(string deviceOrderId);
        Task<bool> DeleteOrderDetailsFromDeviceOrderId(string deviceOrderId);
        Task<bool> DeleteOrderMasterDataFromOrderIdAndDeviceOrderId(int orderId, string deviceOrderId);
        Task<List<ProductDistribution>> GetProductDistributionsDataForSelectedCustomer(int customerId, bool isDirectCustomer);
        Task<bool> UpdateUserMaster(UserMaster user);
        Task RecordManualDistribution(int productId, int customerId);
        Task<bool> AddDistributionDate(int productId, int customerId);
        Task RemoveManualProductDistributionRecord(int productId, int customerId);
        Task<bool> RemoveDistributionDate(int productId, int customerId);
        Task<IEnumerable<MapCustomerData>> GetMapsDataForItemsFilter1(int? zoneId, int? regionId, int? territoryId, string stateID, string cityName, int productId);
        Task<Dictionary<string, string>> GetConfiguration();
        Task<string> GetConfigurationValueAsync(string keyName);
        Task<FedExValidatedAddressResponse> CallFedExAddressValidationAPIAsync(string requestBody);
        Task<ICollection<TerritoryMaster>> GetBDTerritoriesAsync(int bdId);
        Task<ICollection<TerritoryMaster>> GetBDApproverTerritoriesAsync(int bdId,int regionId);
        Task<ICollection<TerritoryMaster>> GetAVPTerritoriesAsync(int avpId);
        Task<int> GetRoleIdAsync(string roleName);
        Task<string> GetUserFullNameAsync(string defTerritoryId);

        Task<string> GetTerritoriesBeforeSyncOfUserAsync(string userName, string pin);
    }
}