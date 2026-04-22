using DRLMobile.Core.Models.DataModels;
using DRLMobile.Core.Models.UIModels;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DRLMobile.Core.Interface
{
    public interface IDatabaseService
    {
        Task<List<CustomerMaster>> GetCustomerDataAsync();
        Task<List<CustomerMaster>> GetCustomerDataForUploadAsync();

        Task<List<CustomerMaster>> GetCustomerDataAsyncForNationalAndZoneAndRegionManagers();

        Task<List<CustomerPageModel>> GetCustomerMasterDataForCustomerPage(string query);

        Task<Dictionary<int, string>> GetStateDictionaryAsync();

        Task<List<StateMaster>> GetAllStateMasterDataForNationalAndZoneAndRegionManagers();

        Task<Dictionary<int, string>> GetCityDictionaryAsync();

        Task<Dictionary<int, Classification>> GetClassificationDictionaryAsync();

        Task<List<ProductMaster>> GetProductsListDataAsync();
        Task<List<ProductMaster>> GetSRCProductsAsync();

        //Task<List<ProductMaster>> GetFavoriteProductsListDataAsync(string CurrentOrderId);
        Task<Dictionary<int, int>> GetFavoritesProductsIdsAsync();
        Task<Favorite> GetFavoriteDataByProductIdAsync(int productId);
        Task<List<Favorite>> GetFavoriteProductDataAsync();
        Task<Dictionary<int, int>> GetCartsProductsIdsAsync(string currentOrderId);

        Task<List<ProductDistribution>> GetProductDistributionDataAsync();
        Task<List<ProductDistribution>> GetProductDistributionDataToUploadAsync();

        Task DeleteFavoriteDataAsync();

        Task<bool> DeleteProductDistrubutionDataAsync(ProductDistribution productDistribution);

        Task<CustomerMaster> GetSavedCustomerInfoAsync(int savedCustomerId);
        Task<CustomerMaster> GetCartCustomerInfoAsync(int savedCustomerId);

        Task<List<CategoryMaster>> GetCategoriesAsync();
        Task<List<CategoryMaster>> GetFilterCategoriesAsync(string ids);

        Task<List<BrandData>> GetFilterBrandsAsync(string ids);
        Task<List<BrandData>> GetBrandsAsync();

        Task<List<StyleData>> GetStylesDataAsync();
        Task<List<StyleData>> GetFilterStylesAsync(string ids);

        Task<List<CategoryMaster>> GetCategoryDataForProductAsync();

        Task<List<BrandData>> GetBrandDataForProductAsync();

        Task<CustomerMaster> GetUserSelfCustomer(string userCustomername);

        Task<List<StyleData>> GetStyleDataForProductAsync();

        Task<List<ProductMaster>> GetProductMasterData();

        Task<List<ProductMaster>> GetFilteredSRCProductData(IEnumerable<int> CatIds, IEnumerable<int> BrandIds, IEnumerable<int> StyleIds);
        Task<List<ProductMaster>> GetCatFilteredSRCProductData(IEnumerable<int> CatIds, IEnumerable<int> BrandIds, IEnumerable<int> StyleIds);

        Task<IEnumerable<ContactMaster>> GetContactsForCustomer(string customerId);

        Task<List<ContactMaster>> GetContactsDataAsync();

        Task<List<CallActivityList>> GetCallActivityDataAsync();

        Task<List<CallActivityList>> GetCallActivityDataForNationalAndZoneAndRegionManagers(string territoryIds, bool loadAllData);

        Task<List<CallActivityList>> GetCallActivityDataForAVPManagers(bool loadAllData);

        Task<List<ScheduledRoutes>> GetScheduledRoutesDataAsync();

        Task<List<UserTaxStatement>> GetUserTaxStatementsDataAsync();

        Task<List<OrderDetail>> GetOrderDetailsDataAsync(string orderId);

        Task<List<OrderDetail>> GetOrderDetailsAsIdAndTotalAsync(IEnumerable<int> orderId);

        Task<List<OrderMaster>> GetOrderMastersDataAsync();

        Task<List<OrderMaster>> GetOrderMastersIdListForNationalAndZoneAndRegionManagers();

        Task<Dictionary<int, string>> GetRankDictionaryAsync();

        Task<Dictionary<int, ZoneMaster>> GetZoneMasterDictionary();

        Task<IEnumerable<DistributorMaster>> GetDistributorMastersListAsync();
        Task<List<DistributorAssignUser>> GetDistributorAssignUserListAsync();
        Task<DistributorAssignUser> GetDistributorAssignUserAsync(int Id);

        Task<List<DistributorMaster>> GetDistributorMastersListForNationalAndZoneAndRegionManagers();

        Task<List<CustomerDistributor>> GetCustomerDistributorMasterAsync();
        Task<IEnumerable<CustomerDistributor>> GetCustomerDistributorMasterToUploadAsync();

        Task<Dictionary<int, RegionMaster>> GetRegionMasterDictionary();

        Task<VripTravelData> GetVripTravelDataForPrgramType(string type);

        Task<Dictionary<int, string>> GetPositionDictionaryAsync();

        Task<IEnumerable<UserMaster>> GetUserDataFromRoleId(int roleId);

        Task<UserMaster> GetLoggedInUserData(int userId);

        Task<List<UserMaster>> GetUserMasterData();

        Task<UserMaster> GetUserFromUserNameAndPin(string userName, string pin);

        Task<List<OrderDetailUIModel>> GetCartDetailsData(string currentOrderId);

        Task<ICollection<TerritoryMaster>> GetTerritoryMasterDataAsync();
        Task<ICollection<TerritoryMaster>> GetAVPTerritoriesAsync(int avpId);
        Task<ICollection<TerritoryMaster>> GetBDTerritoriesAsync(int bdId);
        Task<ICollection<TerritoryMaster>> GetBDApproverTerritoriesAsync(int bdId, int regionId);

        Task<bool> DeleteCartItem(int productId, string deviceOrderId);

        Task<bool> DeleteCartDataOnPlaceOrder(string deviceOrderId);

        Task<ProductAdditionalDocument> GetProductAdditionalDocumentsAsync(int productId);

        Task<ICollection<CustomerDocument>> GetCustomerDocumentsAsync(int customerId);

        Task<List<Favorite>> GetFavoriteProducts();
        Task<List<Favorite>> GetFavoriteProductsToUpload();
        Task<string> GetStateNameForStateIdAsync(int stateId);

        Task<List<ProductMaster>> GetProductsEligibleForCartListDataAsync();

        Task<bool> DeleteExistingCustomerData(CustomerMaster customer);

        Task<bool> RemoveCustomer(int customerId);

        Task<bool> DeleteExistingOrderMasterData(OrderMaster order);

        Task<bool> DeleteExistingUserTaxStatementData(UserTaxStatement taxStatement);

        Task<bool> DeleteExistingCallActivityData(CallActivityList callActivity);
        Task<List<VripTravelData>> GetTravelProgramYearFromVripTravelDataAsync();
        Task<List<VripTravelData>> GetVripProgramYearFromVripTravelDataAsync();

        Task<bool> DeleteExistingContactData(ContactMaster contact);

        Task<bool> DeleteExistingCustomerDistributorData(CustomerDistributor customerDistributor);

        Task<List<TravelUiModel>> GetTravelDataAsync(string year);

        Task<List<VripUiModel>> GetVripMasterDataAsync(string year);

        Task<List<OrderMaster>> GetOrderMasterDataAsync(string customerId, string customerType);

        Task<IEnumerable<OrderDetail>> GetOrderDetailsInOrderId(IEnumerable<int> orderId);

        Task<IEnumerable<ItemHistoryProductModel>> GetProductsForItemHistory(IEnumerable<int> ids);

        Task<OrderMaster> GetOrderMasterFromId(string id);

        Task<IEnumerable<ProductMaster>> GetProductMasterDataForOrderHistory();

        Task<IEnumerable<BrandData>> GetBrandDataForOrderHistory();
        Task<IEnumerable<StyleData>> GetStyleDataForOrderHistory();
        Task<IEnumerable<CategoryMaster>> GetCategoryDataForOrderHistory();
        Task<bool> DeleteOrderDetailFromDetailsOrderId(OrderDetail orderDetail);
        Task<bool> DeleteOrderDetailFromOrderDetailIdPostSync(OrderDetail orderDetail);

        Task<List<PromotionMaster>> GetPromotionsDataAsync(string customerId);

        Task<List<ContractMaster>> GetContractDataAsync(string customerId);

        Task<CustomerMaster> GetVripDataForCustomerAsync(string customerId);

        Task<CustomerMaster> GetTravelDataForCustomerAsync(string customerId);
        Task<IEnumerable<OrderDetail>> GetOrderDetailsFromOrderId(string deviceOrderId);

        Task<List<RackOrderListModel>> GetRackOrderListAsync();
        Task<int> GetCartItemsCountAsync(string currentOrderId);
        Task<bool> DeleteAllCartData(string deviceOrderId);
        Task<List<RackOrderCartItemModel>> GetRackCartItemsAsync(int brandId);
        Task<Dictionary<int, int>> GetCartsProductsIdQuantityAsync(string currentOrderId);
        Task<int> GetCartProductQuantityAsync(string currentOrderId, int productId);
        Task<List<OrderDetail>> GetRackCartProductsAsync(string currentOrderId);
        Task<List<PopOrderCartItemModel>> GetPopCartItemsAsync();
        Task<List<BrandData>> GetFilteredComboboxData(string hierarchyValues, int HierarchyTag);
        Task<List<BrandData>> GetPopOrderMaterialAsync();
        Task<List<BrandData>> GetPopOrderFamilyAsync();
        Task<List<BrandData>> GetPopOrderBrandAsync();
        Task<List<BrandData>> GetPopOrderGroupAsync();
        Task<List<BrandData>> GetPopOrderCategoryAsync();
        Task<List<PopOrderCartItemModel>> GetFilteredPopCartItemsAsync(string hierarchyValues, int HierarchyTag);
        Task<List<ActivityForAllCustomerUIModel>> GetCallActivitiesOfAllCustomersForLoggedInUser(string territoryIds, bool loadAllData);
        Task<List<ActivityForIndividualCustomerUIModel>> GetCallActivitiesOfSelectedCustomerForLoggedInUser(string deviceCustomerId);
        Task<List<ActivityForIndividualCustomerUIModel>> GetCallActivitiesOfSelectedArea(string deviceCustomerId, int selectedArea, int roleId);
        Task<List<CustomerMaster>> GetCustomerListForAddActivity();
        Task<List<MasterTableTypeUIModel>> GetUserActivityTypeAsync();
        Task<List<MasterTableTypeUIModel>> GetCustomerActivityTypeAsync();
        Task<List<MasterTableTypeUIModel>> GetDocumentTypeAsync();
        Task<List<TerritoryMaster>> GetTerritoryFromIds(string territoryIdsInCvs);
        Task<bool> AddCallDateToCallActivityList(CallActivityList activity, string date);
        Task<List<UserTaxStatement>> GetUserTaxStatementItemsAsync();
        Task<UserTaxStatement> InsertUserTaxStatementDataAsync(UserTaxStatement userTaxStatement);
        Task<UserTaxStatement> DeleteUserTaxStatementDataAsync(UserTaxStatement userTaxStatement);
        Task<UserTaxStatement> UpdateUserTaxStatementDataAsync(UserTaxStatement userTaxStatement);
        Task<CallActivityList> GetIndidualActivityById(string id);
        Task<OrderMaster> GetOrderFromOrderMasterFromDeviceOrderId(string deviceOrderId);
        Task<List<OrderDetail>> GetOrderDetailsFromDeviceOrderId(string deviceOrderId);
        Task<List<OrderDetail>> GetOrderDetailsByDeviceOrderId(string deviceOrderId);
        Task<OrderDetail> UpdateOrderDetailDataAsync(OrderDetail orderDetail, string currentOrderId);
        Task UpdateLastCallActivityDateForCustomerMaster(string deviceCustomerId, string updatedDate);
        Task UpdateDateFromActivityCustomerMaster(string deviceCustomerId, string activityType, string lastCallDateTime);
        Task<string> GetOrderGrandTotalFromOrderDeviceId(string orderDeviceId);
        Task<List<PopOrderCartItemModel>> GetRemainingPopCartItemsAsync();
        Task<IEnumerable<ZoneMaster>> GetZoneFromZoneId(string query);
        Task<IEnumerable<RegionMaster>> GetRegionFromRegionId(int id);
        Task<IEnumerable<TerritoryMaster>> GetTerritoryFromDefferedTerritoryId(int id);
        Task<IEnumerable<CityMaster>> GetCityMasterData();
        Task<IEnumerable<RegionMaster>> GetRegionsOnBasisOfZoneIds(string query);
        Task<IEnumerable<ZoneMaster>> GetZonesOnBasisOfZoneIds(string query);
        Task<IEnumerable<TerritoryMaster>> GetTerritoryOnBasisOfRegionIds(string query);
        Task<IEnumerable<MapCustomerData>> GetMapDataForTradeTypeAndRankAndCallDate(string query);
        Task<IEnumerable<CustomerMaster>> GetCustomersInMapAsync(int? zoneId, int? regionId, int? territoryId, string stateID, string cityName);
        Task<IEnumerable<Classification>> GetClassificationInMapAsync();
        Task<IEnumerable<MapCustomerData>> GetMapCustomerDataForCashSales(string query);
        Task<IEnumerable<MapCustomerData>> GetMapCustomerDataForItemFilter(string mainQuery);
        Task<IEnumerable<ProductMaster>> GetProductDetailsOnBasisOfProductName(string query);
        Task<CustomerMaster> GetMapPopupCutomerData(string query);
        Task<IEnumerable<ActivityForAllCustomerUIModel>> GetMapPopupActivityData(string query);

        Task<List<RouteListUIModel>> GetRouteListData();

        Task<bool> UpdateScheduleRoutesForCurrentUser(int userId);

        Task<CallActivityList> GetCurrentCustomerCallActivityDataAsync(string CustomerID, string OrderID);

        Task<List<ViewRouteDetailsUIModel>> GetViewRouteDataAsync(string deviceRouteId);

        Task<bool> DeleteScheduledRouteData(string deviceRouteId);

        Task<bool> DeleteExistingRoutesDataDuringSync(string deviceRouteId);

        Task<IEnumerable<StateMaster>> GetStateDictionaryWhichHasCustomersAssociated(string query);

        Task UpdateOrderGrandTotal(string query);

        Task<List<CustomerDocument>> GetCustomerDocumentsForUpload();

        Task<string> GetCustomerDeviceIdForDocumentUpload(int customerId);

        Task<List<ProductDistribution>> GetProductDistributionsDataForSelectedCustomer(int customerId, bool isDirectCustomer);

        Task<bool> DbExecuteAsync(string query);

        Task<bool> UpdateDatabaseTableAfterAPICall(string queryString);

        Task<List<CustomerMaster>> GetCustomerDataCashSaleAsync(int? zoneId, int? regionId, string territoryId, string stateID, string cityName);

        Task<List<CustomerMaster>> GetCustomerDataForItemSearchAsync(int? zoneId, int? regionId, string territoryId, string stateID, string cityName);

        Task<List<OrderMaster>> GetOrderMasterDataForCashSales(DateTime? startDateTime, DateTime? endDateTime);

        Task<List<Classification>> GetClassificationAsync();

        Task<List<string>> GetListOfDeviceCustomerIdsFromOrderDetailsAndOrderMaster(int productId);

        #region Insert or Update Database Tables
        Task<bool> InsertOrUpdateBrandDataAsync(List<BrandData> brandDataModel);
        Task<CallActivityList> InsertOrUpdateCallActivityListDataAsync(CallActivityList callActivityData);
        Task<CallActivityList> InsertOrUpdateCallActivityInRetailTranscationDataAsync(CallActivityList callActivityData);
        Task<bool> IsCallActivityIdUniqueAsync(int callActivityID);
        Task<CallActivityList> InsertOrUpdateDownloadedCallActivityListDataAsync(CallActivityList callActivityData);
        Task BulkInsertOrUpdateDownloadedCallActivityListDataAsync(List<CallActivityList> callActivityData);
        //Task BulkInsertOrUpdateDownloadedCallActivityListDataAsync(CallActivityList callActivityData);
        Task RunInTransactionUserActivityTypeAsync(List<UserActivityType> userActivityTypeCollection);
        Task RunInTransactionCustomerActivityTypeAsync(List<CustomerActivityType> customerActivityTypeCollection);
        Task RunInTransactionDocumentTypeAsync(List<CustomerDocumentType> documentTypeCollection);
        Task<bool> InsertOrUpdateCartAndFavDataAsync(List<CartAndFav> cartAndFavs);
        Task<bool> InsertOrUpdateCategoryMasterDataAsync(List<CategoryMaster> categoryMasterList);
        Task<bool> InsertOrUpdatetCategoryProductDataAsync(List<CategoryProduct> categoryProducts);
        Task<bool> InsertOrUpdateCityMasterDataAsync(List<CityMaster> cityMasterList);
        Task<bool> InsertOrUpdatetClassificationDataAsync(List<Classification> classifications);
        Task<bool> InsertOrUpdatetConfigurationDataAsync(List<Configuration> configurations);
        Task<ContactMaster> InsertOrUpdatetContactMasterDataAsync(ContactMaster contact);
        Task<ContactMaster> InsertOrUpdateDownloadedContactMasterDataAsync(ContactMaster contact);
        Task<bool> InsertOrUpdateContractMasterDataAsync(List<ContractMaster> contractMasterList);
        Task<bool> InsertOrUpdateCustomerDistributorDataAsync(CustomerDistributor customerDistributor);
        Task<bool> InsertOrUpdateDownloadedCustomerDistributorDataAsync(CustomerDistributor customerDistributor);
        Task<List<CustomerDocument>> InsertOrUpdatetCustomerDocumentDataAsync(List<CustomerDocument> customerDocument, int isExportedDelete);
        Task InsertOrUpdateDownloadedCustomerDocumentDataAsync(List<CustomerDocument> customerDocument);
        Task<CustomerMaster> InsertOrUpdateCustomerMasterDataAsync(CustomerMaster customer);
        Task<CustomerMaster> InsertOrUpdateDownloadedCustomerMasterAsync(CustomerMaster customer);
        //Task BulkInsertOrUpdateCustomerMasterDataAsync(List<CustomerMaster> customerMasters);
        Task BulkInsertOrUpdateCustomerMasterDataAsync(List<CustomerMaster> customerMasters);
        Task<bool> InsertOrUpdatetDistributorMasterDataAsync(List<DistributorMaster> distributorMasterList);
        Task<bool> InsertOrUpdatetFavoriteDataAsync(List<Favorite> favoritesList);
        Task<bool> InsertOrUpdatetDownloadedFavoriteDataAsync(List<Favorite> favoritesList);
        Task<bool> InsertOrUpdateLnkPopItemsDataAsync(List<LnkPopItems> lnkPopItems);
        Task<bool> InsertOrUpdatetLnkRackItemsDataAsync(List<LnkRackItems> lnkRackItemList);
        Task<OrderDetail> InsertOrUpdatetOrderDetailDataAsync(OrderDetail orderDetail);
        Task BulkInsertOrUpdateOrderDetailDataAsync(List<OrderDetail> orderDetail);
        //Task BulkInsertOrUpdateOrderDetailDataAsync(OrderDetail orderDetail);
        Task<bool> InsertOrUpdateOrderHistoryEmailDataAsync(List<OrderHistoryEmail> orderHistoryEmailList);
        Task<OrderMaster> InsertOrUpdateOrderMasterDataAsync(OrderMaster orderMaster);
        Task<OrderMaster> InsertOrUpdateDownloadedOrderMasterDataAsync(OrderMaster orderMaster);
        Task BulkInsertOrUpdateDownloadOrderMasterDataAsync(List<OrderMaster> orderMasters);
        //Task BulkInsertOrUpdateDownloadOrderMasterDataAsync(OrderMaster orderMasters);
        Task<bool> InsertOrUpdatetPositionMasterDataAsync(List<PositionMaster> positionMasterList);
        Task<bool> InsertOrUpdateProductAdditionalDocumentDataAsync(List<ProductAdditionalDocument> productAdditionalDocuments);
        Task<ProductDistribution> InsertOrUpdatetProductDistributionDataAsync(ProductDistribution productDistributionItem);
        Task<ProductDistribution> DeleteAndInsertDownloadedProductDistributionDataAsync(ProductDistribution productDistributionItem);
        Task<bool> InsertOrUpdateProductMasterDataAsync(List<ProductMaster> productMasterList);
        Task<bool> InsertOrUpdatetProductRoleLinkDataAsync(List<ProductRoleLink> productRoleLinks);
        Task<bool> InsertOrUpdatePromotionMasterDataAsync(List<PromotionMaster> promotionMasterList);
        Task<bool> InsertOrUpdatetRankMasterDataAsync(List<RankMaster> rankMasterList);
        Task<bool> InsertOrUpdatetRegionMasterDataAsync(List<RegionMaster> regionMasterList);
        Task<bool> InsertOrUpdateRoleMasterDataAsync(List<RoleMaster> roleMasterList);
        Task<bool> InsertOrUpdatetRouteStationDataAsync(RouteStations routeStation);
        Task<bool> InsertOrUpdateSalesDocumentDataAsync(List<SalesDocument> salesDocuments);
        Task<ScheduledRoutes> InsertOrUpdatetScheduledRouteDataAsync(ScheduledRoutes scheduledRoutes);
        Task<List<ScheduledRoutes>> InsertOrUpdatetDownloadedScheduledRoutesDataAsync(List<ScheduledRoutes> scheduledRoutes);
        Task InsertOrUpdatetDownloadedRouteStationsDataAsync(List<RouteStations> routeStations);
        Task<bool> InsertOrUpdateStateMasterDataAsync(List<StateMaster> stateMasterList);
        Task<bool> InsertOrUpdateStyleDataDataAsync(List<StyleData> styleData);
        Task<bool> InsertOrUpdatetSupplyChainDataAsync(List<SupplyChain> supplyChainsList);
        Task<bool> InsertOrUpdatetTerritoryMasterDataAsync(List<TerritoryMaster> territoryMasterList);
        Task<bool> InsertOrUpdateTravelMasterDataAsync(List<TravelMaster> travelMasterList);
        Task<bool> InsertOrUpdatetUserMasterDataAsync(List<UserMaster> userMasterList);
        Task<bool> InsertOrUpdateUserTaxStatementDataAsync(UserTaxStatement userTaxStatementItem);
        Task<bool> InsertOrUpdateDownloadedUserTaxStatementDataAsync(UserTaxStatement userTaxStatementItem);
        Task<bool> InsertOrUpdatetVripMasterDataAsync(List<VripMaster> vripMasterList);
        Task<bool> InsertOrUpdateVripTravelDataAsync(List<VripTravelData> vripTravelData);
        Task<bool> InsertOrUpdateZoneMasterDataAsync(List<ZoneMaster> zoneMasterList);
        Task<IEnumerable<int>> GetCustomerIdsFromRouteStationTableByRouteId(string query);
        Task<bool> DeleteAllRouteStationsForRouteId(string query);
        Task<bool> UpdateScheduleRoutesWhenRouteIdIs(string query);
        Task<IEnumerable<MapPopAddToRouteListModel>> GetRoutesForMapPopup(string query);
        Task<List<UserMaster>> GetListOfTerritoryManagersForTerritories(string territoryIds);
        Task<List<UserMaster>> GetListOfTerritoryManagersForRegion(int regionId);
        Task<List<UserMaster>> GetListOfTerritoryManagersForZone(int zoneId);
        Task<bool> DeleteExistingCustomerDocumentData(CustomerDocument customerDocument);
        Task<ProductDistribution> InsertProductDistributionDataFromSRCList(ProductDistribution productDistributionItem);
        Task<Dictionary<string, string>> GetConfiguration();
        Task<string> GetConfigurationValueAsync(string keyName);
        Task<ProductDetailUiModel> IsCustomerInSRCAsync(int _customerId);
        string SqlLiteDateTimeFormat(string colName);
        string SqlLiteDateFormat(string colName);
        Task<List<AVPMaster>> GetAVPMastersAsync();
        Task<AVPMaster> GetAVPMasterByIdAsync(int avpId);
        Task<List<ZoneMaster>> GetZoneMastersAsync();
        Task<ZoneMaster> GetZoneMasterByIdAsync(int zoneId);
        Task<TerritoryMaster> GetTerritoryMasterByIdAsync(int id);
        Task<BDMaster> GetBDMasterByIdAsync(int id);
        Task<int> GetRoleIdAsync(string roleName);
        Task<string> GetUserFullNameAsync(string defTerritoryId);
        #endregion

    }
}