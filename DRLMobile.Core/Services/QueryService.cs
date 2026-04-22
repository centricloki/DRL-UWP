using AutoMapper;

using DRLMobile.Core.AutoMapperProfiler;
using DRLMobile.Core.Enums;
using DRLMobile.Core.Helpers;
using DRLMobile.Core.Interface;
using DRLMobile.Core.Models.DataModels;
using DRLMobile.Core.Models.DataSyncRequestModels;
using DRLMobile.Core.Models.FedExAddressValidationModels;
using DRLMobile.Core.Models.UIModels;
using DRLMobile.ExceptionHandler;

using Newtonsoft.Json;

using SQLite;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace DRLMobile.Core.Services
{
    public class QueryService : IQueryService
    {
        private readonly IDatabaseService DbService;
        protected readonly ProfileMatcher _profileMatcher;
        protected IMapper _mapper;

        public QueryService()
        {
            DbService = new DatabaseService();
            this._profileMatcher = ProfileMatcher.GetInstance;
        }

        /// <summary>
        /// Get UI model for associated chain location customer page
        /// </summary>
        /// <returns>List of UI model for associated chain location customer page</returns>
        public async Task<List<CustomerPageUIModel>> GetChainLocationCustomers(int headQuarter)
        {
            try
            {
                string query = $@"SELECT * FROM CustomerMaster LEFT JOIN TerritoryMaster on 
                    TerritoryMaster.TerritoryID = CustomerMaster.TerritoryId 
                    WHERE CustomerMaster.isDeleted = 0 and CustomerMaster.isExported != 2 
                    AND CustomerMaster.IsParent=0 AND CustomerMaster.Parent={headQuarter}  ";
                var stateDict = await DbService.GetStateDictionaryAsync();
                var classificationData = await DbService.GetClassificationDictionaryAsync();
                var customerList = await DbService.GetCustomerMasterDataForCustomerPage(query);

                if (customerList != null)
                {
                    customerList.ForEach(x =>
                    {
                        string tempState = null;
                        Classification tempClassification = null;
                        stateDict.TryGetValue(x.PhysicalAddressStateID, out tempState);
                        classificationData.TryGetValue(Convert.ToInt32(x?.AccountClassification), out tempClassification);
                        x.StateData = tempState;
                        x.ClassificationData = tempClassification;
                    });

                    // filter the customer list with where clause
                    return customerList.Select(x => x.CopyToUIModel()).OrderByDescending(x => x.CallDate).ToList();
                }
                return null;
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(nameof(QueryService), nameof(GetChainLocationCustomers), ex);
                return null;
            }
        }

        /// <summary>
        /// Get UI model for associated head quarter customer page
        /// </summary>
        /// <returns>List of UI model for associated head quarter customer page</returns>
        public async Task<List<CustomerPageUIModel>> GetHeadQuarterCustomers(int headQuarter)
        {
            try
            {
                string query = $@"SELECT * FROM CustomerMaster LEFT JOIN TerritoryMaster on 
                    TerritoryMaster.TerritoryID = CustomerMaster.TerritoryId 
                    WHERE CustomerMaster.isDeleted = 0 and CustomerMaster.isExported != 2 
                    AND CustomerMaster.IsParent=1 AND CustomerMaster.CustomerId={headQuarter}  ";
                var stateDict = await DbService.GetStateDictionaryAsync();
                var classificationData = await DbService.GetClassificationDictionaryAsync();
                var customerList = await DbService.GetCustomerMasterDataForCustomerPage(query);

                if (customerList != null)
                {
                    customerList.ForEach(x =>
                    {
                        string tempState = null;
                        Classification tempClassification = null;
                        stateDict.TryGetValue(x.PhysicalAddressStateID, out tempState);
                        classificationData.TryGetValue(Convert.ToInt32(x?.AccountClassification), out tempClassification);
                        x.StateData = tempState;
                        x.ClassificationData = tempClassification;
                    });

                    // filter the customer list with where clause
                    return customerList.Select(x => x.CopyToUIModel()).OrderByDescending(x => x.CallDate).ToList();
                }
                return null;
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(nameof(QueryService), nameof(GetHeadQuarterCustomers), ex);
                return null;
            }
        }

        /// <summary>
        /// Get UI model for customer page
        /// </summary>
        /// <returns>List of UI model for customer page</returns>
        public async Task<List<CustomerPageUIModel>> GetCustomerPageData(
     int areaId,
     int roleId,
     bool allData,
     string userId,
     CustomerTeamType teamType,
     Dictionary<string, string> filters = null)  // <-- New parameter
        {
            try
            {
                var query = new StringBuilder();
                query.Append(@"SELECT CustomerMaster.*,TerritoryMaster.* FROM CustomerMaster 
                             LEFT JOIN TerritoryMaster ON TerritoryMaster.TerritoryID = CustomerMaster.TerritoryId
                             LEFT JOIN StateMaster ON StateMaster.StateID = CustomerMaster.PhysicalAddressStateID
                             LEFT JOIN Classification ON Classification.AccountClassificationId = CustomerMaster.AccountClassification
                             WHERE CustomerMaster.IsAssociatedCustomer = 0
                             AND CustomerMaster.isDeleted = 0 
                             AND CustomerMaster.isExported != 2");

                // Role/Area filtering
                if (roleId >= 3 && areaId > 0)
                {
                    if (roleId == 3)
                    {
                        query.Append($" AND CustomerMaster.RegionId = {areaId}");
                    }
                    else if (roleId == 6 || roleId == 17 || roleId == await GetRoleIdAsync(ApplicationConstants.AVPRoleName))
                    {
                        query.Append($" AND CustomerMaster.ZoneId = {areaId}");
                    }
                }

                // Team-type filtering
                if (CustomerTeamType.Both != teamType)
                {
                    var userDetail = await GetLoggedInUserInformation(Convert.ToInt32(userId));
                    string territoriesBelongingToUser = userDetail?.TerritoryId;

                    if (userDetail != null)
                    {
                        if (userDetail.RoleId == await GetRoleIdAsync(ApplicationConstants.BDRoleName))
                        {
                            var territories = await GetBDApproverTerritoriesAsync(userDetail.BDId, userDetail.RegionId);
                            territoriesBelongingToUser = string.Join(",", territories.Select(x => x.TerritoryID));
                        }
                        else if (userDetail.RoleId == await GetRoleIdAsync(ApplicationConstants.AVPRoleName))
                        {
                            var territories = await GetAVPTerritoriesAsync(userDetail.AVPId);
                            territoriesBelongingToUser = string.Join(",", territories.Select(x => x.TerritoryID));
                        }
                    }

                    if (!string.IsNullOrEmpty(territoriesBelongingToUser))
                    {
                        switch (teamType)
                        {
                            case CustomerTeamType.AssignedAccount:
                                query.Append($" AND CustomerMaster.TerritoryId IN ({territoriesBelongingToUser})");
                                break;
                            case CustomerTeamType.TeamMember:
                                query.Append($" AND CustomerMaster.TerritoryId NOT IN ({territoriesBelongingToUser})");
                                break;
                        }
                    }
                }

                // ===== APPLY USER FILTERS (IF ANY) =====
                if (filters != null && filters.Count > 0)
                {
                    var filterClauses = new List<string>();

                    // Helper to safely add filter if value is non-empty
                    void AddFilter(string key, string columnName, bool useLike = true)
                    {
                        if (filters.TryGetValue(key, out string value) && !string.IsNullOrWhiteSpace(value))
                        {
                            var safeValue = EscapeSql(value);
                            if (useLike)
                                filterClauses.Add($"{columnName} LIKE '%{safeValue}%'");
                            else
                                filterClauses.Add($"{columnName} = '{safeValue}'");
                        }
                    }

                    // Map known filter keys to DB columns
                    AddFilter("CustomerNumber", "CustomerMaster.CustomerNumber");
                    AddFilter("CustomerName", "CustomerMaster.CustomerName");
                    AddFilter("City", "CustomerMaster.PhysicalAddressCityId");
                    AddFilter("State", "StateMaster.StateName");
                    AddFilter("StoreType", "Classification.AccountClassificationName");
                    AddFilter("Rank", "CustomerMaster.Rank");
                    AddFilter("Address", "CONCAT(CustomerMaster.PhysicalAddress)");
                    AddFilter("Territory", "TerritoryMaster.TerritoryName");

                    // Special handling for Calldate
                    if (filters.TryGetValue("Calldate", out string calldateInput) && !string.IsNullOrWhiteSpace(calldateInput))
                    {
                        if (DateTime.TryParseExact(calldateInput, "MM-dd-yyyy", null, DateTimeStyles.None, out DateTime parsedDate))
                        {
                            string dbDate = parsedDate.ToString("MM/dd/yyyy");
                            filterClauses.Add($"CustomerMaster.LastCallActivityDate LIKE '{EscapeSql(dbDate)}%'");
                        }
                    }

                    if (filterClauses.Count > 0)
                    {
                        query.Append(" AND (");
                        query.Append(string.Join(" OR ", filterClauses)); // At least one filter matches
                        query.Append(")");
                    }
                }

                if (!allData)
                {
                    query.Append(" LIMIT 1000");
                }

                // Fetch and enrich data
                var stateDict = await DbService.GetStateDictionaryAsync();
                var classificationData = await DbService.GetClassificationDictionaryAsync();
                var customerList = await DbService.GetCustomerMasterDataForCustomerPage(query.ToString());

                if (customerList != null)
                {
                    foreach (var cust in customerList)
                    {
                        stateDict.TryGetValue(cust.PhysicalAddressStateID, out var stateName);
                        classificationData.TryGetValue(Convert.ToInt32(cust.AccountClassification), out var classification);
                        cust.StateData = stateName;
                        cust.ClassificationData = classification;
                    }

                    return customerList
                        .Select(x => x.CopyToUIModel())
                        .OrderBy(x => x.CustomerName)
                        .ToList();
                }

                return null;
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(nameof(QueryService), nameof(GetCustomerPageData), ex);
                return null;
            }
        }

        private string EscapeSql(string input)
        {
            return input?.Replace("'", "''") ?? string.Empty;
        }


        /// <summary>
        /// Get UI model for customer page
        /// </summary>
        /// <returns>List of UI model for customer page</returns>
        public async Task<List<CustomerPageUIModel>> GetCustomerPageData(bool loadAllData)
        {
            try
            {
                string query;
                if (loadAllData)
                {
                    query = "SELECT * " +
                    "FROM CustomerMaster LEFT JOIN TerritoryMaster on " +
                    "TerritoryMaster.TerritoryID = CustomerMaster.TerritoryId " +
                    " WHERE CustomerMaster.IsAssociatedCustomer = 0 and CustomerMaster.isDeleted = 0 and CustomerMaster.isExported != 2";
                }
                else
                {
                    query = "SELECT * " +
                   "FROM CustomerMaster LEFT JOIN TerritoryMaster on " +
                   "TerritoryMaster.TerritoryID = CustomerMaster.TerritoryId " +
                   " WHERE CustomerMaster.IsAssociatedCustomer = 0 and CustomerMaster.isDeleted = 0 and CustomerMaster.isExported != 2 ORDER BY CustomerName LIMIT 1000";
                }
                var stateDict = await DbService.GetStateDictionaryAsync();
                var classificationData = await DbService.GetClassificationDictionaryAsync();
                var customerList = await DbService.GetCustomerMasterDataForCustomerPage(query);

                if (customerList != null)
                {
                    customerList.ForEach(x =>
                    {
                        string tempState = null;
                        Classification tempClassification = null;
                        stateDict.TryGetValue(x.PhysicalAddressStateID, out tempState);
                        classificationData.TryGetValue(Convert.ToInt32(x?.AccountClassification), out tempClassification);
                        x.StateData = tempState;
                        x.ClassificationData = tempClassification;
                    });

                    // filter the customer list with where clause
                    return customerList.Select(x => x.CopyToUIModel()).OrderBy(x => x.CustomerName).ToList();
                }
                return null;
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(nameof(QueryService), nameof(GetCustomerPageData), ex);
                return null;
            }
        }

        public async Task<List<CustomerPageUIModel>> GetUpdatedCustomers(string updatedDate, int areaId, int roleId)
        {
            try
            {
                string query = $"SELECT * FROM CustomerMaster " +
                    $" LEFT JOIN TerritoryMaster on TerritoryMaster.TerritoryID = CustomerMaster.TerritoryId " +
                    $" WHERE  CustomerMaster.IsAssociatedCustomer = 0 and CustomerMaster.isDeleted = 0 and CustomerMaster.isExported != 2 AND {DbService.SqlLiteDateFormat("UpdatedDate")} >= " +
                    $" {DbService.SqlLiteDateFormat("'" + updatedDate + "'")} ";


                if (roleId >= 3 && areaId > 0)
                {
                    if (roleId == 3)
                    {
                        query += $" AND CustomerMaster.RegionId= {areaId} ";
                    }
                    else if (roleId == 6 || roleId == 17 || roleId == await GetRoleIdAsync(ApplicationConstants.AVPRoleName))
                    {
                        query += $" AND CustomerMaster.ZoneId= {areaId} ";
                    }
                }


                var stateDict = await DbService.GetStateDictionaryAsync();
                var classificationData = await DbService.GetClassificationDictionaryAsync();
                var customerList = await DbService.GetCustomerMasterDataForCustomerPage(query);

                if (customerList != null)
                {
                    customerList.ForEach(x =>
                    {
                        string tempState = null;
                        Classification tempClassification = null;
                        stateDict.TryGetValue(x.PhysicalAddressStateID, out tempState);
                        classificationData.TryGetValue(Convert.ToInt32(x?.AccountClassification), out tempClassification);
                        x.StateData = tempState;
                        x.ClassificationData = tempClassification;
                    });

                    // filter the customer list with where clause
                    return customerList.Select(x => x.CopyToUIModel()).OrderBy(x => x.CustomerName).ToList();
                }
                return null;
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(nameof(QueryService), nameof(GetUpdatedCustomers), ex);
                return null;
            }
        }

        public async Task<List<CustomerPageModel>> CleanDeletedCustomers()
        {
            try
            {
                string query = "SELECT * FROM CustomerMaster WHERE ISDELETED='1' AND IsExported!=0";

                var customerList = await DbService.GetCustomerMasterDataForCustomerPage(query).ConfigureAwait(false);

                if (customerList != null)
                {
                    foreach (var customer in customerList)
                    {
                        await DbService.RemoveCustomer(customer.CustomerID ?? -1).ConfigureAwait(false);
                    }
                    // filter the customer list with where clause
                    return customerList;
                }
                return null;
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(nameof(QueryService), nameof(CleanDeletedCustomers), ex);
                return null;
            }
        }

        /// <summary>
        /// Method to get the list of SRC Products for Data Grid control
        /// </summary>
        /// <returns> product list for data grid control</returns>
        public async Task<List<SRCProductUIModel>> GetSRCProductsPageGridData(string CurrentOrderId)
        {
            Dictionary<int, int> cartProduct = new Dictionary<int, int>();
            Dictionary<int, int> cartProductQuantity = new Dictionary<int, int>();

            var productsList = await DbService.GetProductsListDataAsync();
            var favorites = await DbService.GetFavoritesProductsIdsAsync();

            if (!string.IsNullOrEmpty(CurrentOrderId) && !CurrentOrderId.Equals("0"))
            {
                cartProduct = await DbService.GetCartsProductsIdsAsync(CurrentOrderId);
                cartProductQuantity = await DbService.GetCartsProductsIdQuantityAsync(CurrentOrderId);
            }

            List<SRCProductUIModel> filteredSRCProductsListData = new List<SRCProductUIModel>();

            if (productsList != null)
            {
                productsList.ForEach(p =>
                {
                    var favTemp = favorites?.TryGetValue(p.ProductID, out int tempFav);
                    p.FavoriteData = favTemp.HasValue && favTemp.Value;

                    if (cartProduct?.Count > 0)
                    {
                        var cartTemp = cartProduct?.TryGetValue(p.ProductID, out int tempCart);
                        p.CartData = cartTemp.HasValue && cartTemp.Value;
                    }

                    if (cartProductQuantity?.Count > 0)
                    {
                        int tempQuantity = 0;
                        var cartQuantityTemp = cartProductQuantity?.TryGetValue(p.ProductID, out tempQuantity);
                        if (cartQuantityTemp.HasValue && cartQuantityTemp.Value)
                        {
                            p.CartQuantity = tempQuantity;
                        }
                        else
                        {
                            p.CartQuantity = 1;
                        }
                    }
                });

                filteredSRCProductsListData = productsList.Select(x => x.CopyToUIModel()).ToList();
            }

            return filteredSRCProductsListData;
        }

        /// <summary>
        /// Method to get the list of SRC Products for Data Grid control
        /// </summary>
        /// <returns> product list for data grid control</returns>
        //public async Task<List<SRCProductUIModel>> GetSRCProductsAsync(string CurrentOrderId)
        //{
        //    this._mapper = this._profileMatcher.GetMapper(
        //           (IMapperConfigurationExpression cfg) => this._profileMatcher.CreateSRCProductMapping(cfg));

        //    var products = this._mapper.Map<List<ProductMaster>, List<SRCProductUIModel>>(
        //           await DbService.GetSRCProductsAsync().ConfigureAwait(false));

        //    // var favorites = await DbService.GetFavoritesProductsIdsAsync().ConfigureAwait(false);
        //    var cartProduct = await DbService.GetCartsProductsIdQuantityAsync(CurrentOrderId).ConfigureAwait(false);

        //    foreach (var item in products)
        //    {
        //        item.FavoriteMasterData = await DbService.GetFavoriteDataByProductIdAsync(item.ProductID).ConfigureAwait(false);
        //        item.IsFavorite = item?.FavoriteMasterData != null && !(item.FavoriteMasterData.isDeleted);

        //        if (cartProduct != null && cartProduct.Any(x => x.Key == item.ProductID))
        //        {
        //            item.IsAddedToCart = true;
        //            int tempQuantity = 0;
        //            var cartQuantityTemp = cartProduct?.TryGetValue(item.ProductID, out tempQuantity);
        //            if (cartQuantityTemp.HasValue && cartQuantityTemp.Value)
        //            {
        //                item.Quantity = tempQuantity;
        //            }
        //            else
        //            {
        //                item.Quantity = 1;
        //            }
        //        }
        //        else
        //        {
        //            item.IsAddedToCart = false;
        //        }
        //    }

        //    return products;
        //}

        public async Task<List<SRCProductUIModel>> GetSRCProductsAsync(string CurrentOrderId)
        {
            this._mapper = this._profileMatcher.GetMapper(
                   (IMapperConfigurationExpression cfg) => this._profileMatcher.CreateSRCProductMapping(cfg));

            var products = this._mapper.Map<List<ProductMaster>, List<SRCProductUIModel>>(
                   await DbService.GetSRCProductsAsync().ConfigureAwait(false));

            List<Favorite> favorites = await DbService.GetFavoriteProductDataAsync().ConfigureAwait(false);
            var cartProduct = await DbService.GetCartsProductsIdQuantityAsync(CurrentOrderId).ConfigureAwait(false);

            foreach (var item in products)
            {
                if (favorites != null && favorites?.Count > 0)
                {
                    item.FavoriteMasterData = favorites.FirstOrDefault(x => x.ProductId == item.ProductID);
                    item.IsFavorite = item?.FavoriteMasterData != null;
                }

                if (cartProduct != null && cartProduct.Any(x => x.Key == item.ProductID))
                {
                    item.IsAddedToCart = true;
                    int tempQuantity = 0;
                    var cartQuantityTemp = cartProduct?.TryGetValue(item.ProductID, out tempQuantity);
                    if (cartQuantityTemp.HasValue && cartQuantityTemp.Value)
                    {
                        item.Quantity = tempQuantity;
                    }
                    else
                    {
                        item.Quantity = 1;
                    }
                }
                else
                {
                    item.IsAddedToCart = false;
                }
            }

            return products;
        }

        public async Task<int> GetCartProductQuantityAsync(string currentOrderId, int productId)
            => await DbService.GetCartProductQuantityAsync(currentOrderId, productId).ConfigureAwait(false);

        //To DO
        //public async Task<List<SRCProductUIModel>> GetSRCProductsPageGridData(string CurrentOrderId)
        //{


        //    this._mapper = this._profileMatcher.GetMapper(
        //           (IMapperConfigurationExpression cfg) => this._profileMatcher.CreateSRCProductsPageDataMapping(cfg));

        //    var filteredSRCProductsListData = await DbService.GetFavoriteProductsListDataAsync(CurrentOrderId).ConfigureAwait(false);

        //    return this._mapper.Map<List<ProductMaster>, List<SRCProductUIModel>>(
        //        filteredSRCProductsListData
        //        );
        //}

        /// <summary>
        /// Method to get the list of SRC Product Ids eligible to be added in cart
        /// </summary>
        /// <returns> product list for data grid control</returns>
        public async Task<List<int>> GetCartEligibleProducts(bool? IsCreditRequestOrder, bool? IsDistributionOptionClicked)
        {
            try
            {
                var productsList = await DbService.GetProductsEligibleForCartListDataAsync().ConfigureAwait(false);
                if (IsCreditRequestOrder == true)
                {
                    productsList = productsList.Where(x => x.SRCHoneyReturnable != 0).ToList();
                }
                else if (IsDistributionOptionClicked == true)
                {
                    productsList = productsList.Where(x => x.SRCCanIOrder != 0).ToList();
                }
                else
                {
                    productsList = productsList.Where(x => x.SRCHoneySellable != 0).ToList();
                }
                var filteredProductIds = productsList.Select(x => x.ProductID).ToList();
                return filteredProductIds;
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(nameof(QueryService), nameof(GetCartEligibleProducts), ex);
                return null;
            }
        }
        /// <summary>
        /// This method will return the saved customer information
        /// </summary>
        /// <param name="customerId">customer id of saved customer</param>
        /// <returns>saved customer information</returns>
        public async Task<SavedCustomerInfoUIModel> GetSavedCustomerInformation(int customerId)
        {
            SavedCustomerInfoUIModel savedCustomerInfo = null;

            try
            {
                ///var stateDict = await DbService.GetStateDictionaryAsync();
                var selectedCustomer = await DbService.GetSavedCustomerInfoAsync(customerId);

                if (selectedCustomer != null)
                {
                    savedCustomerInfo = new SavedCustomerInfoUIModel();
                    savedCustomerInfo.CustomerName = selectedCustomer.CustomerName;
                    savedCustomerInfo.CustomerNumber = selectedCustomer.CustomerNumber;
                    savedCustomerInfo.PhysicalAddress = selectedCustomer.PhysicalAddress;
                    savedCustomerInfo.AccountType = selectedCustomer.AccountType;
                    savedCustomerInfo.PhysicalAddressCityID = selectedCustomer.PhysicalAddressCityID;
                    savedCustomerInfo.ShippingAddressStateID = selectedCustomer.ShippingAddressStateID;

                    ///stateDict.TryGetValue(savedCustomerInfo.ShippingAddressStateID, out stateName);

                    string stateName = await DbService.GetStateNameForStateIdAsync(selectedCustomer.ShippingAddressStateID);

                    if (!string.IsNullOrEmpty(stateName))
                    {
                        savedCustomerInfo.SubAddressText = savedCustomerInfo.PhysicalAddressCityID + ", " + stateName;
                    }
                    else
                    {
                        savedCustomerInfo.SubAddressText = savedCustomerInfo.PhysicalAddressCityID;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetSavedCustomerInformation", ex);
            }

            return savedCustomerInfo;
        }

        /// <summary>
        /// Method to get filtered BrandData when Category is selected
        /// </summary>
        /// <param name="selectedCategoryIdsList"></param>
        /// <returns>Filtered BrandData</returns>
        public async Task<List<BrandUIModel>> GetFilteredBrandDataWhenCategoryIsSelected(List<int> selectedCategoryIdsList)
        {
            List<BrandUIModel> filteredBrandUIModelList = new List<BrandUIModel>();

            try
            {
                var brandData = await DbService.GetBrandDataForProductAsync();
                var productData = await DbService.GetProductMasterData();

                if (productData != null)
                {
                    var filterdBrandIdsList = productData.Where(t => selectedCategoryIdsList.Contains(t.CatId)).Select(d => d.BrandId).Distinct().ToList();

                    var filteredBrandData = brandData?.Where(x => filterdBrandIdsList.Contains(x.BrandId)).Distinct().ToList();

                    filteredBrandUIModelList = filteredBrandData?.Select(x => x.CopyToUIModel()).ToList();
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetFilteredBrandDataWhenCategoryIsSelected", ex);
            }

            return filteredBrandUIModelList;
        }

        /// <summary>
        /// Method to get filtered StyleData when Category is selected
        /// </summary>
        /// <param name="selectedCategoryIdsList"></param>
        /// <returns>Filtered StyleData</returns>
        public async Task<List<StyleUIModel>> GetFilteredStyleDataWhenCategoryIsSelected(List<int> selectedCategoryIdsList)
        {
            List<StyleUIModel> filteredStyleUIModelList = new List<StyleUIModel>();

            try
            {
                var styleData = await DbService.GetStyleDataForProductAsync();
                var productData = await DbService.GetProductMasterData();

                if (productData != null)
                {
                    var filterdStyleIdsList = productData.Where(t => selectedCategoryIdsList.Contains(t.CatId) && t.CatId != -99).Select(d => d.StyleId).Distinct().ToList();

                    var filteredStyleData = styleData?.Where(x => filterdStyleIdsList.Contains(x.StyleId)).Distinct().ToList();

                    filteredStyleUIModelList = filteredStyleData?.Select(x => x.CopyToUIModel()).ToList();
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetFilteredStyleDataWhenCategoryIsSelected", ex);
            }

            return filteredStyleUIModelList;
        }

        /// <summary>
        /// Method to get filtered Category when Style is selected
        /// </summary>
        /// <param name="SelectedStyleIdsList"></param>
        /// <returns>Filtered Category List</returns>
        public async Task<List<CategoryUIModel>> GetFilteredCategoryDataWhenStyleIsSelected(List<int> selectedStyleIdsList)
        {
            List<CategoryUIModel> filteredCategoryUIModelList = new List<CategoryUIModel>();

            try
            {
                var categoryData = await DbService.GetCategoryDataForProductAsync();
                var productData = await DbService.GetProductMasterData();

                if (productData != null)
                {
                    var filterdCategoryIdsList = productData.Where(t => selectedStyleIdsList.Contains(t.StyleId)).Select(d => d.CatId).Distinct().ToList();

                    var filteredCatData = categoryData?.Where(x => filterdCategoryIdsList.Contains(x.CategoryID)).ToList();

                    filteredCategoryUIModelList = filteredCatData?.Select(x => x.CopyToUIModel()).ToList();
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetFilteredCategoryDataWhenStyleIsSelected", ex);
            }

            return filteredCategoryUIModelList;
        }

        /// <summary>
        /// Method to get filtered Category when Brand is selected
        /// </summary>
        /// <param name="SelectedBrandIdsList"></param>
        /// <returns>Filtered Category List</returns>
        public async Task<List<CategoryUIModel>> GetFilteredCategoryDataWhenBrandIsSelected(List<int> selectedBrandIdsList)
        {
            List<CategoryUIModel> filteredCategoryUIModelList = new List<CategoryUIModel>();

            try
            {
                var categoryData = await DbService.GetCategoryDataForProductAsync();
                var productData = await DbService.GetProductMasterData();

                if (productData != null)
                {
                    var filterdCategoryIdsList = productData.Where(t => selectedBrandIdsList.Contains(t.BrandId)).Select(d => d.CatId).Distinct().ToList();

                    var filteredCatData = categoryData?.Where(x => filterdCategoryIdsList.Contains(x.CategoryID)).ToList();

                    filteredCategoryUIModelList = filteredCatData?.Select(x => x.CopyToUIModel()).ToList();
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetFilteredCategoryDataWhenBrandIsSelected", ex);
            }
            return filteredCategoryUIModelList;
        }

        /// <summary>
        /// Method to get filtered brand data when style is selected
        /// </summary>
        /// <param name="selectedStyleIdsList"></param>
        /// <returns>Filtered brand List</returns>
        public async Task<List<BrandUIModel>> GetFilteredBrandDataWhenStyleIsSelected(List<int> selectedStyleIdsList)
        {
            List<BrandUIModel> filteredBrandUIModelList = new List<BrandUIModel>();

            try
            {
                var brandData = await DbService.GetBrandDataForProductAsync();
                var productData = await DbService.GetProductMasterData();

                if (productData != null)
                {
                    var filterdBrandIdsList = productData.Where(t => selectedStyleIdsList.Contains(t.StyleId)).Select(d => d.BrandId).Distinct().ToList();

                    var filteredBrandData = brandData?.Where(x => filterdBrandIdsList.Contains(x.BrandId)).ToList();

                    filteredBrandUIModelList = filteredBrandData?.Select(x => x.CopyToUIModel()).ToList();
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetFilteredBrandDataWhenStyleIsSelected", ex);
            }

            return filteredBrandUIModelList;
        }

        /// <summary>
        /// Method to get filtered brand data when style is selected
        /// </summary>
        /// <param name="selectedBrandIdsList"></param>
        /// <returns>Filtered brand List</returns>
        public async Task<List<StyleUIModel>> GetFilteredStyleDataWhenBrandIsSelected(List<int> selectedBrandIdsList)
        {
            List<StyleUIModel> filteredStyleUIModelList = new List<StyleUIModel>();

            try
            {
                var styleData = await DbService.GetStyleDataForProductAsync();
                var productData = await DbService.GetProductMasterData();

                if (productData != null)
                {
                    var filterdStyleIdsList = productData.Where(t => selectedBrandIdsList.Contains(t.BrandId)).Select(d => d.StyleId).Distinct().ToList();

                    var filteredStyleData = styleData?.Where(x => filterdStyleIdsList.Contains(x.StyleId)).ToList();

                    filteredStyleUIModelList = filteredStyleData?.Select(x => x.CopyToUIModel()).ToList();
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetFilteredStyleDataWhenBrandIsSelected", ex);
            }

            return filteredStyleUIModelList;
        }

        public async Task<List<StyleUIModel>> GetFilteredStyleDataWhenCategoryAndBrandSelected(List<int> selectedCategoryIdsList, List<int> selectedBrandIdsList)
        {
            List<StyleUIModel> filteredStyleUIModelList = new List<StyleUIModel>();

            try
            {
                var styleData = await DbService.GetStyleDataForProductAsync();
                var productData = await DbService.GetProductMasterData();

                if (productData != null)
                {
                    var filterdStyleIdsList = productData.Where(t => selectedCategoryIdsList.Contains(t.CatId) && selectedBrandIdsList.Contains(t.BrandId)).Select(d => d.StyleId).Distinct().ToList();

                    var filteredStyleData = styleData?.Where(x => filterdStyleIdsList.Contains(x.StyleId)).ToList();

                    filteredStyleUIModelList = filteredStyleData?.Select(x => x.CopyToUIModel()).ToList();
                }

            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetFilteredStyleDataWhenCategoryAndBrandSelected", ex);
            }

            return filteredStyleUIModelList;
        }

        public async Task<List<BrandUIModel>> GetFilteredBrandDataWhenCategoryAndStyleSelected(List<int> selectedCategoryIdsList, List<int> selectedStyleIdsList)
        {
            List<BrandUIModel> filteredBrandUIModelList = new List<BrandUIModel>();

            try
            {
                var brandData = await DbService.GetBrandDataForProductAsync();
                var productData = await DbService.GetProductMasterData();

                if (productData != null)
                {
                    var filterdBrandIdsList = productData.Where(t => selectedCategoryIdsList.Contains(t.CatId) && selectedStyleIdsList.Contains(t.StyleId)).Select(d => d.BrandId).Distinct().ToList();

                    var filteredBrandData = brandData?.Where(x => filterdBrandIdsList.Contains(x.BrandId)).ToList();

                    filteredBrandUIModelList = filteredBrandData?.Select(x => x.CopyToUIModel()).ToList();
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetFilteredBrandDataWhenCategoryAndStyleSelected", ex);
            }

            return filteredBrandUIModelList;
        }

        public async Task<List<CategoryUIModel>> GetFilteredCategoryDataWhenBrandAndStyleSelected(List<int> selectedBrandIdsList, List<int> selectedStyleIdsList)
        {
            List<CategoryUIModel> filteredCategoryUIModelList = new List<CategoryUIModel>();

            try
            {
                var categoryData = await DbService.GetCategoryDataForProductAsync();
                var productData = await DbService.GetProductMasterData();

                if (productData != null)
                {
                    var filterdCategoryIdsList = productData.Where(t => selectedBrandIdsList.Contains(t.BrandId) && selectedStyleIdsList.Contains(t.StyleId)).Select(d => d.CatId).Distinct().ToList();

                    var filteredCatData = categoryData?.Where(x => filterdCategoryIdsList.Contains(x.CategoryID)).ToList();

                    filteredCategoryUIModelList = filteredCatData?.Select(x => x.CopyToUIModel()).ToList();
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetFilteredCategoryDataWhenBrandAndStyleSelected", ex);
            }
            return filteredCategoryUIModelList;
        }

        /// <summary>
        /// Method to get the SRC products list data as per all three selected filters
        /// </summary>
        /// <returns>Filtered SRC Products list</returns>
        public async Task<List<SRCProductUIModel>> GetSRCProductsDataOnAllSelectedFilters(List<int> selectedCategoryIdsList, List<int> selectedBrandIdsList, List<int> selectedStyleIdsList, string CurrentOrderId)//todo
        {
            Dictionary<int, int> cartProduct = new Dictionary<int, int>();
            Dictionary<int, int> cartProductQuantity = new Dictionary<int, int>();

            List<ProductMaster> productData;

            if (selectedCategoryIdsList.Count == 1 && selectedCategoryIdsList[0] == -99)
            {
                productData = await DbService.GetProductMasterData();
            }
            else
            {
                productData = await DbService.GetProductsListDataAsync();
            }

            var favorites = await DbService.GetFavoritesProductsIdsAsync();

            if (!string.IsNullOrEmpty(CurrentOrderId) && !CurrentOrderId.Equals("0"))
            {
                cartProduct = await DbService.GetCartsProductsIdsAsync(CurrentOrderId);
                cartProductQuantity = await DbService.GetCartsProductsIdQuantityAsync(CurrentOrderId);
            }

            List<SRCProductUIModel> filteredSRCProductsListData = new List<SRCProductUIModel>();

            List<ProductMaster> filteredProductsListData;

            try
            {
                if (productData != null && productData.Count > 0)
                {
                    filteredProductsListData = new List<ProductMaster>();

                    if (selectedCategoryIdsList != null && selectedBrandIdsList != null && selectedStyleIdsList != null)
                    {
                        if (selectedCategoryIdsList.Count > 0 && selectedBrandIdsList.Count > 0 && selectedStyleIdsList.Count > 0)
                        {
                            filteredProductsListData = productData.Where(c => selectedCategoryIdsList.Contains(c.CatId)).Where(b => selectedBrandIdsList.Contains(b.BrandId)).Where(s => selectedStyleIdsList.Contains(s.StyleId)).ToList();
                        }
                        else if (selectedCategoryIdsList.Count > 0 && selectedBrandIdsList.Count > 0)
                        {
                            filteredProductsListData = productData.Where(c => selectedCategoryIdsList.Contains(c.CatId)).Where(b => selectedBrandIdsList.Contains(b.BrandId)).ToList();
                        }
                        else if (selectedCategoryIdsList.Count > 0 && selectedStyleIdsList.Count > 0)
                        {
                            filteredProductsListData = productData.Where(c => selectedCategoryIdsList.Contains(c.CatId)).Where(s => selectedStyleIdsList.Contains(s.StyleId)).ToList();
                        }
                        else if (selectedStyleIdsList.Count > 0 && selectedBrandIdsList.Count > 0)
                        {
                            filteredProductsListData = productData.Where(b => selectedBrandIdsList.Contains(b.BrandId)).Where(s => selectedStyleIdsList.Contains(s.StyleId)).ToList();
                        }
                        else if (selectedCategoryIdsList.Count > 0)
                        {
                            filteredProductsListData = productData.Where(c => selectedCategoryIdsList.Contains(c.CatId)).ToList();
                        }
                        else if (selectedBrandIdsList.Count > 0)
                        {
                            filteredProductsListData = productData.Where(b => selectedBrandIdsList.Contains(b.BrandId)).ToList();
                        }
                        else if (selectedStyleIdsList.Count > 0)
                        {
                            filteredProductsListData = productData.Where(s => selectedStyleIdsList.Contains(s.StyleId)).ToList();
                        }
                        else
                        {
                            filteredProductsListData = productData;
                        }
                    }

                    if (filteredProductsListData != null && filteredProductsListData.Count > 0)
                    {
                        filteredProductsListData?.ForEach(p =>
                        {
                            var favTemp = favorites?.TryGetValue(p.ProductID, out int tempFav);
                            p.FavoriteData = favTemp.HasValue && favTemp.Value;

                            if (cartProduct?.Count > 0)
                            {
                                var cartTemp = cartProduct?.TryGetValue(p.ProductID, out int tempCart);
                                p.CartData = cartTemp.HasValue && cartTemp.Value;
                            }

                            if (cartProductQuantity?.Count > 0)
                            {
                                int tempQuantity = 0;
                                var cartQuantityTemp = cartProductQuantity?.TryGetValue(p.ProductID, out tempQuantity);
                                if (cartQuantityTemp.HasValue && cartQuantityTemp.Value)
                                {
                                    p.CartQuantity = tempQuantity;
                                }
                                else
                                {
                                    p.CartQuantity = 1;
                                }
                            }

                        });

                        filteredSRCProductsListData = filteredProductsListData?.Select(x => x.CopyToUIModel()).OrderBy(x => x.ItemNumber).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetFilteredStyleDataWhenBrandIsSelected", ex);
            }
            return filteredSRCProductsListData;
        }

        /// <summary>
        /// Method to get the SRC products list data as per all three selected filters
        /// </summary>
        /// <returns>Filtered SRC Products list</returns>
        public async Task<List<SRCProductUIModel>> GetFilteredSRCProducts(IEnumerable<int> selectedCategories, IEnumerable<int> selectedBrands, IEnumerable<int> selectedStyles, string CurrentOrderId)
        {
            this._mapper = this._profileMatcher.GetMapper(
                  (IMapperConfigurationExpression cfg) => this._profileMatcher.CreateSRCProductMapping(cfg));

            List<SRCProductUIModel> products = null;
            try
            {
                if (selectedCategories.Count() == 0 && selectedBrands.Count() == 0 && selectedStyles.Count() == 0)
                {
                    products = this._mapper.Map<List<ProductMaster>, List<SRCProductUIModel>>(await DbService.GetFilteredSRCProductData(selectedCategories, selectedBrands, selectedStyles));
                }
                else if (selectedCategories.Count() == 1 && selectedCategories.FirstOrDefault() == -99)
                {
                    products = this._mapper.Map<List<ProductMaster>, List<SRCProductUIModel>>(await DbService.GetFilteredSRCProductData(selectedCategories, selectedBrands, selectedStyles));
                }
                else
                {
                    products = this._mapper.Map<List<ProductMaster>, List<SRCProductUIModel>>(await DbService.GetCatFilteredSRCProductData(selectedCategories, selectedBrands, selectedStyles));
                }

                var favorites = await DbService.GetFavoritesProductsIdsAsync().ConfigureAwait(false);
                var cartProduct = await DbService.GetCartsProductsIdQuantityAsync(CurrentOrderId).ConfigureAwait(false);

                foreach (var item in products)
                {
                    item.IsFavorite = favorites != null ? (favorites.Any(x => x.Key == item.ProductID)) : false;
                    if (cartProduct != null && cartProduct.Any(x => x.Key == item.ProductID))
                    {
                        item.IsAddedToCart = true;
                        int tempQuantity = 0;
                        var cartQuantityTemp = cartProduct?.TryGetValue(item.ProductID, out tempQuantity);
                        if (cartQuantityTemp.HasValue && cartQuantityTemp.Value)
                        {
                            item.Quantity = tempQuantity;
                        }
                        else
                        {
                            item.Quantity = 1;
                        }
                    }
                    else
                    {
                        item.IsAddedToCart = false;
                    }
                }

            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, nameof(GetFilteredSRCProducts), ex);
            }
            return products;
        }

        private IEnumerable<int> FetchInteger(string argString)
        {
            //string input = "There are 4 numbers in this string: 40, 30, and 10.";
            // Split on one or more non-digit characters.
            string[] numbers = Regex.Split(argString, @"\D+");
            ICollection<int> result = new List<int>();
            foreach (string value in numbers)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    if (int.TryParse(value, out int i)) result.Add(i);
                }
            }
            return result;
        }

        private string FetchValidValue(string argString, string pattern)
        {
            string result = null;
            var match = Regex.Match(argString, pattern);
            if (match.Success)
                result = match.Value;

            return result;
        }

        /// <summary>
        /// Method to upload data on server on partial sync
        /// </summary>
        /// <param name="uploadData"></param>
        /// <returns>True, if success, else False</returns>
        public async Task<bool> UploadDataOnPartialSync(string userName, string pin, string lastSyncDate)
        {
            bool result = true;
            try
            {
                #region Upload Sync start

                #region CustomerMaster Upload
                var customerList = await DbService.GetCustomerDataForUploadAsync().ConfigureAwait(false);
                if (customerList != null && customerList.Any())
                {
                    var customerDataToUpload = DataSyncHelper.GetCustomersListForUpload(customerList);

                    var webServiceResponse = await InvokeWebService.UploadCustomerDataDuringSyncProcess(userName, Convert.ToInt32(pin), customerDataToUpload).ConfigureAwait(false);

                    if (webServiceResponse != null && webServiceResponse.responsestatus.Equals("200"))
                    {
                        string queryString;
                        if (webServiceResponse.error.Any())
                        {
                            int index = 0;
                            string failedCustomerDeviceIdList = "";
                            foreach (var msg in webServiceResponse.error)
                            {
                                ErrorLogger.WriteToErrorLog(GetType().Name, "UploadCustomerDataDuringSyncProcess", msg);
                                if (index > 0)
                                {
                                    failedCustomerDeviceIdList += ",";
                                }
                                var fetchCustomerId = FetchValidValue(msg, @"[{(]?[0-9A-F]{8}[-]?([0-9A-F]{4}[-]?){3}[0-9A-F]{12}[)}]?");
                                if (!string.IsNullOrWhiteSpace(fetchCustomerId))
                                {
                                    if (Guid.TryParse(fetchCustomerId, out Guid g))
                                    {
                                        failedCustomerDeviceIdList += string.Format("'{0}'", fetchCustomerId);
                                    }
                                }
                                else
                                {
                                    fetchCustomerId = FetchValidValue(msg, @"[{(]?[0-9a-f]{8}[-]?([0-9a-f]{4}[-]?){3}[0-9a-f]{12}[)}]?");
                                    if (!string.IsNullOrWhiteSpace(fetchCustomerId))
                                    {
                                        if (Guid.TryParse(fetchCustomerId, out Guid g))
                                        {
                                            failedCustomerDeviceIdList += string.Format("'{0}'", fetchCustomerId);
                                        }
                                    }
                                    else
                                    {
                                        fetchCustomerId = FetchValidValue(msg, @"\d+");
                                        if (!string.IsNullOrWhiteSpace(fetchCustomerId))
                                        {
                                            if (int.TryParse(fetchCustomerId, out int i))
                                            {
                                                failedCustomerDeviceIdList += string.Format("'{0}'", i);
                                            }
                                        }
                                    }
                                }
                                index++;
                            }
                            queryString = string.Format("UPDATE CustomerMaster SET isExported = 1 WHERE isExported == 0 AND DeviceCustomerID NOT IN ({0})", failedCustomerDeviceIdList);
                            //result = result && false;
                        }
                        else
                        {
                            queryString = "UPDATE CustomerMaster SET isExported = 1 WHERE isExported == 0";
                        }
                        await DbService.UpdateDatabaseTableAfterAPICall(queryString).ConfigureAwait(false);
                    }
                    else
                    {
                        result = result && false;
                    }
                }
                #endregion

                #region Order Master Upload
                var orderMasterData = await DbService.GetOrderMastersDataAsync().ConfigureAwait(false);
                if (orderMasterData != null && orderMasterData.Any())
                {
                    var orderMasterDataToUpload = await DataSyncHelper.GetOrdersListForUpload(orderMasterData);
                    var webServiceResponse = await InvokeWebService.UploadOrderDataDuringSyncProcess(userName, Convert.ToInt32(pin), orderMasterDataToUpload).ConfigureAwait(false);

                    if (webServiceResponse != null && webServiceResponse.responsestatus.Equals("200"))
                    {
                        string queryString;
                        if (webServiceResponse.error.Any())
                        {
                            int index = 0;
                            string failedDeviceOrderIDList = "";
                            foreach (var msg in webServiceResponse.error)
                            {
                                ErrorLogger.WriteToErrorLog(GetType().Name, "UploadOrderDataDuringSyncProcess", msg);
                                if (index > 0)
                                {
                                    failedDeviceOrderIDList += ",";
                                }
                                ///Guid.NewGuid().ToString() = 36 characters(Hyphenated) of DeviceOrderID
                                failedDeviceOrderIDList += string.Format("'{0}'", msg.Substring(0, 36));
                                index++;
                            }
                            queryString = string.Format("UPDATE OrderMaster SET isExported = 1 WHERE isExported == 0 AND DeviceOrderID NOT IN ({0})", failedDeviceOrderIDList);
                            //result = result && false;
                        }
                        else
                        {
                            queryString = "UPDATE OrderMaster SET isExported = 1 WHERE isExported == 0";
                        }
                        await DbService.UpdateDatabaseTableAfterAPICall(queryString).ConfigureAwait(false);
                    }
                    else
                    {
                        result = result && false;
                    }

                    #region  uploading order signatures
                    var orderSignatureDataToUpload = DataSyncHelper.GetOrderSignaturesForUpload(orderMasterData, userName, pin, lastSyncDate);
                    foreach (var orderSignature in orderSignatureDataToUpload)
                    {
                        await InvokeWebService.UploadUserDocumentToServer(orderSignature, null).ConfigureAwait(false);
                    }
                    #endregion
                }
                #endregion

                #region Contact Master Upload
                var contactList = await DbService.GetContactsDataAsync().ConfigureAwait(false);
                if (contactList != null && contactList.Any())
                {
                    var contactDataToUpload = DataSyncHelper.GetContactsListForUpload(contactList);
                    var webServiceResponse = await InvokeWebService.UploadContactDataDuringSyncProcess(userName, Convert.ToInt32(pin), contactDataToUpload).ConfigureAwait(false);

                    if (webServiceResponse != null && webServiceResponse.responsestatus.Equals("200"))
                    {
                        string queryString;
                        if (webServiceResponse.error.Any())
                        {
                            int index = 0;
                            string failedDeviceContactIDList = "";
                            foreach (var msg in webServiceResponse.error)
                            {
                                ErrorLogger.WriteToErrorLog(GetType().Name, "UploadContactDataDuringSyncProcess", msg);
                                if (index > 0)
                                {
                                    failedDeviceContactIDList += ",";
                                }
                                ///Guid.NewGuid().ToString() = 36 characters(Hyphenated) of DeviceContactID
                                failedDeviceContactIDList += string.Format("'{0}'", msg.Substring(0, 36));
                                index++;
                            }
                            queryString = string.Format("UPDATE ContactMaster SET isExported = 1 WHERE isExported == 0 AND DeviceContactID NOT IN ({0})", failedDeviceContactIDList);
                            //result = result && false;
                        }
                        else
                        {
                            queryString = "UPDATE ContactMaster SET isExported = 1 WHERE isExported == 0";
                        }
                        await DbService.UpdateDatabaseTableAfterAPICall(queryString).ConfigureAwait(false);
                    }
                    else
                    {
                        result = result && false;
                    }
                }
                #endregion

                #region CallActivity Upload
                var callActivityList = await DbService.GetCallActivityDataAsync().ConfigureAwait(false);
                if (callActivityList != null && callActivityList.Any())
                {
                    var callActivityDataToUpload = DataSyncHelper.GetCallActivityListForUpload(callActivityList);

                    string[] attachmentType = new string[] { "Wholesale Invoice", "Narrative-Weekly Report" };
                    var attachmentActivities = callActivityDataToUpload.Where(x => attachmentType.Contains(x.activitytype));
                    if (attachmentActivities != null && attachmentActivities?.Count() > 0)
                    {
                        byte[] FileBytes = null;
                        string fileName = null;
                        foreach (var item in attachmentActivities)
                        {
                            var webServiceResponse = await InvokeWebService.UploadCallActivityDataDuringSyncProcess(userName, Convert.ToInt32(pin)
                                , new List<Models.DataSyncRequestModels.AddCallActivityModel> { item }).ConfigureAwait(false);
                            if (webServiceResponse != null && webServiceResponse.responsestatus.Equals("200"))
                            {
                                if (webServiceResponse.error.Any())
                                {
                                    int index = 0;
                                    string failedCallActivityDeviceIDList = "";
                                    foreach (var msg in webServiceResponse.error)
                                    {
                                        ErrorLogger.WriteToErrorLog(GetType().Name, "UploadCallActivityDataDuringSyncProcess", msg);
                                        if (index > 0)
                                        {
                                            failedCallActivityDeviceIDList += ",";
                                        }
                                        ///Guid.NewGuid().ToString() = 36 characters(Hyphenated) of CallActivityDeviceID
                                        failedCallActivityDeviceIDList += string.Format("'{0}'", msg.Substring(0, 36));
                                        index++;
                                    }
                                }
                                else
                                {
                                    fileName = callActivityList.First(x => x.CallActivityDeviceID == item.devicecallactivityid).WholesaleInvoiceFilePath;
                                    if (!string.IsNullOrWhiteSpace(fileName))
                                    {
                                        if (!fileName.ToUpper().StartsWith("HTTP"))
                                        {
                                            if (File.Exists(fileName))
                                            {
                                                using (FileStream fileStream = File.Open(fileName, FileMode.Open))
                                                {
                                                    using (var memoryStream = new MemoryStream())
                                                    {
                                                        fileStream.CopyTo(memoryStream);
                                                        FileBytes = memoryStream.ToArray();
                                                    }
                                                }
                                            }
                                            UploadActivityRequestModel uploadActivityRequestModel = new UploadActivityRequestModel
                                            {
                                                DeviceCallActivityID = item.devicecallactivityid,
                                                filetype = 7,
                                                filename = HelperMethods.GetNameFromURL(fileName),
                                                filestream = Convert.ToBase64String(FileBytes),
                                                username = userName,
                                                pin = Convert.ToInt32(pin),
                                                updatedate = lastSyncDate,
                                                versionnumber = ApplicationConstants.APPLICATION_VERSION
                                            };
                                            await InvokeWebService.UploadActivityToServer(null, uploadActivityRequestModel).ConfigureAwait(false);
                                        }
                                    }
                                    string queryString = string.Format("UPDATE CallActivityList SET isExported = 1 WHERE CallActivityDeviceID ='{0}'", item.devicecallactivityid);
                                    await DbService.UpdateDatabaseTableAfterAPICall(queryString).ConfigureAwait(false);
                                }
                            }
                        }
                    }
                    var nonAttachmentActivities = callActivityDataToUpload.Where(x => !attachmentType.Contains(x.activitytype));
                    if (nonAttachmentActivities != null && nonAttachmentActivities?.Count() > 0)
                    {
                        var webServiceResponse = await InvokeWebService.UploadCallActivityDataDuringSyncProcess(userName, Convert.ToInt32(pin), nonAttachmentActivities.ToList()).ConfigureAwait(false);
                        if (webServiceResponse != null && webServiceResponse.responsestatus.Equals("200"))
                        {
                            string queryString;
                            if (webServiceResponse.error.Any())
                            {
                                int index = 0;
                                string failedCallActivityDeviceIDList = "";
                                foreach (var msg in webServiceResponse.error)
                                {
                                    ErrorLogger.WriteToErrorLog(GetType().Name, "UploadCallActivityDataDuringSyncProcess", msg);
                                    if (index > 0)
                                    {
                                        failedCallActivityDeviceIDList += ",";
                                    }
                                    ///Guid.NewGuid().ToString() = 36 characters(Hyphenated) of CallActivityDeviceID
                                    failedCallActivityDeviceIDList += string.Format("'{0}'", msg.Substring(0, 36));
                                    index++;
                                }
                                queryString = string.Format("UPDATE CallActivityList SET isExported = 1 WHERE isExported == 0 AND CallActivityDeviceID NOT IN ({0})", failedCallActivityDeviceIDList);
                                //result = result && false;
                            }
                            else
                            {
                                foreach (var item in nonAttachmentActivities)
                                {
                                    string queryString2 = string.Format("UPDATE CallActivityList SET isExported = 1 WHERE CallActivityDeviceID ='{0}'", item.devicecallactivityid);
                                    await DbService.UpdateDatabaseTableAfterAPICall(queryString2).ConfigureAwait(false);
                                }
                            }
                        }
                        else
                        {
                            result = result && false;
                        }
                    }
                }
                #endregion

                #region ScheduleRoute Upload
                var scheduleRouteList = await DbService.GetScheduledRoutesDataAsync();
                if (scheduleRouteList != null && scheduleRouteList.Any())
                {
                    var scheduleRouteDataToUpload = await DataSyncHelper.GetScheduleRouteListForUpload(scheduleRouteList);

                    var webServiceResponse = await InvokeWebService.UploadScheduledRouteDataDuringSyncProcess(userName, Convert.ToInt32(pin), scheduleRouteDataToUpload);

                    if (webServiceResponse != null && webServiceResponse.responsestatus.Equals("200"))
                    {
                        string queryString;
                        if (webServiceResponse.error.Any())
                        {
                            int index = 0;
                            string failedDeviceRouteIDList = "";
                            foreach (var msg in webServiceResponse.error)
                            {
                                ErrorLogger.WriteToErrorLog(GetType().Name, "UploadScheduledRouteDataDuringSyncProcess", msg);
                                if (index > 0)
                                {
                                    failedDeviceRouteIDList += ",";
                                }
                                ///Guid.NewGuid().ToString() = 36 characters(Hyphenated) of DeviceRouteID
                                failedDeviceRouteIDList += string.Format("'{0}'", msg.Substring(0, 36));
                                index++;
                            }
                            queryString = string.Format("UPDATE ScheduledRoutes SET isExported = 1 WHERE isExported == 0 AND DeviceRouteID NOT IN ({0})", failedDeviceRouteIDList);
                            //result = result && false;
                        }
                        else
                        {
                            queryString = "UPDATE ScheduledRoutes SET isExported = 1 WHERE isExported == 0";
                        }
                        await DbService.UpdateDatabaseTableAfterAPICall(queryString).ConfigureAwait(false);
                    }
                    else
                    {
                        result = result && false;
                    }
                }
                #endregion

                #region TaxStatement Upload
                var userTaxStatementList = await DbService.GetUserTaxStatementsDataAsync().ConfigureAwait(false);
                if (userTaxStatementList != null && userTaxStatementList.Any())
                {
                    var userTaxStatementDataToUpload = DataSyncHelper.GetUserTaxStatementListForUpload(userTaxStatementList);

                    var webServiceResponse = await InvokeWebService.UploadUserTaxStatementsDataDuringSyncProcess(userName, Convert.ToInt32(pin), userTaxStatementDataToUpload).ConfigureAwait(false);

                    if (webServiceResponse != null && webServiceResponse.responsestatus.Equals("200"))
                    {
                        string queryString;
                        if (webServiceResponse.error.Any())
                        {
                            int index = 0;
                            string failedDeviceUserTaxStatementIDList = "";
                            foreach (var msg in webServiceResponse.error)
                            {
                                ErrorLogger.WriteToErrorLog(GetType().Name, "UploadUserTaxStatementsDataDuringSyncProcess", msg);
                                if (index > 0)
                                {
                                    failedDeviceUserTaxStatementIDList += ",";
                                }
                                ///Guid.NewGuid().ToString() = 36 characters(Hyphenated) of DeviceUserTaxStatementID
                                failedDeviceUserTaxStatementIDList += string.Format("'{0}'", msg.Substring(0, 36));
                                index++;
                            }
                            queryString = string.Format("UPDATE UserTaxStatement SET isExported = 1 WHERE isExported == 0 AND DeviceUserTaxStatementID NOT IN ({0})", failedDeviceUserTaxStatementIDList);
                            //result = result && false;
                        }
                        else
                        {
                            queryString = "UPDATE UserTaxStatement SET isExported = 1 WHERE isExported == 0";
                        }
                        await DbService.UpdateDatabaseTableAfterAPICall(queryString).ConfigureAwait(false);
                    }
                    else
                    {
                        result = result && false;
                    }
                }
                #endregion

                #region CustomerDistributorMaster Upload
                var customerDistributor = (await DbService.GetCustomerDistributorMasterToUploadAsync().ConfigureAwait(false)).ToList();
                if (customerDistributor != null && customerDistributor.Any())
                {
                    var customerDistributorDataToUpload = DataSyncHelper.GetCustomerDistributorListForUpload(customerDistributor);
                    var webServiceResponse = await InvokeWebService.UploadCustomerDistributorDuringSyncProcess(userName, Convert.ToInt32(pin), customerDistributorDataToUpload).ConfigureAwait(false);

                    if (webServiceResponse != null && webServiceResponse.responsestatus.Equals("200"))
                    {
                        string queryString;
                        if (webServiceResponse.error.Any())
                        {
                            int index = 0;
                            string failedDeviceCustomerIDList = "";
                            foreach (var msg in webServiceResponse.error)
                            {
                                ErrorLogger.WriteToErrorLog(GetType().Name, "UploadCustomerDistributorDuringSyncProcess", msg);
                                if (index > 0)
                                {
                                    failedDeviceCustomerIDList += ",";
                                }
                                ///Guid.NewGuid().ToString() = 36 characters(Hyphenated) of DeviceCustomerID
                                failedDeviceCustomerIDList += string.Format("'{0}'", msg.Substring(0, 36));
                                index++;
                            }
                            queryString = string.Format("UPDATE CustomerDistributor SET isExported = 1 WHERE isExported == 0 AND DeviceCustomerID NOT IN ({0})", failedDeviceCustomerIDList);
                            //result = result && false;
                        }
                        else
                        {
                            queryString = "UPDATE CustomerDistributor SET isExported = 1 WHERE isExported == 0";
                        }
                        await DbService.UpdateDatabaseTableAfterAPICall(queryString).ConfigureAwait(false);
                    }
                    else
                    {
                        result = result && false;
                    }
                }
                #endregion

                #region Favorite Upload
                var favoritesData = await DbService.GetFavoriteProductsToUpload().ConfigureAwait(false);
                if (favoritesData != null && favoritesData.Any())
                {
                    var favoriteDataToUpload = DataSyncHelper.GetFavoriteListForUpload(favoritesData);

                    await InvokeWebService.UploadFavoriteDuringSyncProcess(favoriteDataToUpload).ConfigureAwait(false);
                }
                #endregion

                #region ProductDistribution Upload
                var productDistributionData = await DbService.GetProductDistributionDataToUploadAsync().ConfigureAwait(false);
                if (productDistributionData != null && productDistributionData.Any())
                {
                    var productDistributionDataToUpload = await DataSyncHelper.GetProductDistributionDataForUpload(productDistributionData).ConfigureAwait(false);
                    var webServiceResponse = await InvokeWebService.UploadProductDistributionDuringSyncProcess(userName, Convert.ToInt32(pin), productDistributionDataToUpload).ConfigureAwait(false);

                    if (webServiceResponse != null && webServiceResponse.responsestatus.Equals("200"))
                    {
                        string queryString;
                        if (webServiceResponse.error.Any())
                        {
                            int index = 0;
                            string failedDeviceCustomerProductIDList = "";
                            foreach (var msg in webServiceResponse.error)
                            {
                                ErrorLogger.WriteToErrorLog(GetType().Name, "UploadProductDistributionDuringSyncProcess", msg);
                                if (index > 0)
                                {
                                    failedDeviceCustomerProductIDList += ",";
                                }
                                ///Guid.NewGuid().ToString() = 36 characters(Hyphenated) of DeviceCustomerProductID
                                failedDeviceCustomerProductIDList += string.Format("'{0}'", msg.Substring(0, 36));
                                index++;
                            }
                            queryString = string.Format("UPDATE ProductDistribution SET isExported = 1 WHERE isExported == 0 AND DeviceCustomerProductID NOT IN ({0})", failedDeviceCustomerProductIDList);
                            //result = result && false;
                        }
                        else
                        {
                            queryString = "UPDATE ProductDistribution SET isExported = 1 WHERE isExported == 0";
                        }
                        await DbService.UpdateDatabaseTableAfterAPICall(queryString).ConfigureAwait(false);
                    }
                    else
                    {
                        result = result && false;
                    }
                }
                #endregion

                #region  uploading customer documents
                var customerDocumentsData = await DbService.GetCustomerDocumentsForUpload().ConfigureAwait(false);
                if (customerDocumentsData != null && customerDocumentsData.Count > 0)
                {
                    var customerDocumentsDataToUpload = await DataSyncHelper.GetCustomerDocumentsForUpload(customerDocumentsData, userName, pin, lastSyncDate).ConfigureAwait(false);

                    foreach (var customerDocument in customerDocumentsDataToUpload)
                    {
                        await InvokeWebService.UploadUserDocumentToServer(null, customerDocument).ConfigureAwait(false);
                    }
                }
                #endregion

                #endregion
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException("QueryService", "UploadDataOnPartialSync", ex);
                result = false;
            }
            return result;
        }

        /// <summary>
        /// Method to download and save data into database on partial sync
        /// </summary>
        /// <param name="downloadedData"></param>
        /// <returns>True, if success, else False</returns>
        public async Task<bool> DownloadDataOnPartialSync(SyncDataModel downloadedData, string userid)
        {
            try
            {
                if (downloadedData != null)
                {
                    if (downloadedData.branddata != null && downloadedData.branddata.Any())
                    {
                        await DbService.InsertOrUpdateBrandDataAsync(downloadedData.branddata).ConfigureAwait(false);
                    }

                    if (downloadedData.styledata != null && downloadedData.styledata.Any())
                    {
                        await DbService.InsertOrUpdateStyleDataDataAsync(downloadedData.styledata).ConfigureAwait(false);
                    }

                    if (downloadedData.roledata != null && downloadedData.roledata.Any())
                    {
                        await DbService.InsertOrUpdateRoleMasterDataAsync(downloadedData.roledata).ConfigureAwait(false);
                    }

                    if (downloadedData.citymasterdata != null && downloadedData.citymasterdata.Any())
                    {
                        await DbService.InsertOrUpdateCityMasterDataAsync(downloadedData.citymasterdata).ConfigureAwait(false);
                    }

                    if (downloadedData.statemasterdata != null && downloadedData.statemasterdata.Any())
                    {
                        await DbService.InsertOrUpdateStateMasterDataAsync(downloadedData.statemasterdata).ConfigureAwait(false);
                    }

                    if (downloadedData.regionmasterdata != null && downloadedData.regionmasterdata.Any())
                    {
                        await DbService.InsertOrUpdatetRegionMasterDataAsync(downloadedData.regionmasterdata).ConfigureAwait(false);
                    }

                    if (downloadedData.territorymasterdata != null && downloadedData.territorymasterdata.Any())
                    {
                        await DbService.InsertOrUpdatetTerritoryMasterDataAsync(downloadedData.territorymasterdata).ConfigureAwait(false);
                    }

                    if (downloadedData.userdata != null && downloadedData.userdata.Any())
                    {
                        await DbService.InsertOrUpdatetUserMasterDataAsync(downloadedData.userdata).ConfigureAwait(false);
                    }

                    if (downloadedData.UserTaxStatementdata != null && downloadedData.UserTaxStatementdata.Any())
                    {
                        foreach (var item in downloadedData.UserTaxStatementdata)
                        {
                            #region Previous Code
                            //item.IsExported = 1;

                            //await DbService.DeleteExistingUserTaxStatementData(item).ConfigureAwait(false);

                            //await Task.Delay(50);
                            //await DbService.InsertOrUpdateUserTaxStatementDataAsync(item).ConfigureAwait(false);
                            #endregion
                            await DbService.InsertOrUpdateDownloadedUserTaxStatementDataAsync(item).ConfigureAwait(false);
                        }
                    }

                    if (downloadedData.zonemasterdata != null && downloadedData.zonemasterdata.Any())
                    {
                        await DbService.InsertOrUpdateZoneMasterDataAsync(downloadedData.zonemasterdata).ConfigureAwait(false);
                    }

                    if (downloadedData.Vrip != null && downloadedData.Vrip.Any())
                    {
                        await DbService.InsertOrUpdatetVripMasterDataAsync(downloadedData.Vrip).ConfigureAwait(false);
                    }

                    if (downloadedData.Travel != null && downloadedData.Travel.Any())
                    {
                        await DbService.InsertOrUpdateTravelMasterDataAsync(downloadedData.Travel).ConfigureAwait(false);
                    }

                    if (downloadedData.configurations != null && downloadedData.configurations.Any())
                    {
                        await DbService.InsertOrUpdatetConfigurationDataAsync(downloadedData.configurations).ConfigureAwait(false);
                    }

                    if (downloadedData.distributordata != null && downloadedData.distributordata.Any())
                    {
                        await DbService.InsertOrUpdatetDistributorMasterDataAsync(downloadedData.distributordata).ConfigureAwait(false);
                    }

                    if (downloadedData.customerdocument != null && downloadedData.customerdocument.Any())
                    {
                        await DbService.InsertOrUpdateDownloadedCustomerDocumentDataAsync(downloadedData.customerdocument).ConfigureAwait(false);
                    }

                    if (downloadedData.CustomerDistributorData != null && downloadedData.CustomerDistributorData.Any())
                    {
                        foreach (var customerDistributorItem in downloadedData.CustomerDistributorData)
                        {
                            #region Previous code
                            //customerDistributorItem.IsExported = 1;
                            //await DbService.DeleteExistingCustomerDistributorData(customerDistributorItem).ConfigureAwait(false);

                            ////await Task.Delay(50);

                            //await DbService.InsertOrUpdateCustomerDistributorDataAsync(customerDistributorItem).ConfigureAwait(false);
                            #endregion
                            await DbService.InsertOrUpdateDownloadedCustomerDistributorDataAsync(customerDistributorItem).ConfigureAwait(false);
                        }
                    }

                    if (downloadedData.contactdata != null && downloadedData.contactdata.Any())
                    {
                        foreach (var contact in downloadedData.contactdata)
                        {
                            #region Previous code
                            //contact.IsExported = 1;

                            //await DbService.DeleteExistingContactData(contact).ConfigureAwait(false);

                            ////await Task.Delay(50);

                            //await DbService.InsertOrUpdatetContactMasterDataAsync(contact).ConfigureAwait(false);
                            #endregion
                            await DbService.InsertOrUpdateDownloadedContactMasterDataAsync(contact).ConfigureAwait(false);
                        }
                    }

                    if (downloadedData.categoryproduct != null && downloadedData.categoryproduct.Any())
                    {
                        await DbService.InsertOrUpdatetCategoryProductDataAsync(downloadedData.categoryproduct).ConfigureAwait(false);
                    }

                    if (downloadedData.productdata != null && downloadedData.productdata.Any())
                    {
                        await DbService.InsertOrUpdateProductMasterDataAsync(downloadedData.productdata).ConfigureAwait(false);
                    }

                    if (downloadedData.productdocument != null && downloadedData.productdocument.Any())
                    {
                        await DbService.InsertOrUpdateProductAdditionalDocumentDataAsync(downloadedData.productdocument).ConfigureAwait(false);
                    }

                    if (downloadedData.scheduleroutes != null && downloadedData.scheduleroutes.Any())
                    {
                        #region Previous code
                        //downloadedData.scheduleroutes.ForEach(x => x.IsExported = 1);

                        //foreach (var item in downloadedData.scheduleroutes)
                        //{
                        //    await DbService.DeleteExistingRoutesDataDuringSync(item.DeviceRouteId).ConfigureAwait(false);
                        //}

                        //await DbService.InsertOrUpdatetScheduledRoutesDataAsync(downloadedData.scheduleroutes).ConfigureAwait(false);
                        #endregion
                        await DbService.InsertOrUpdatetDownloadedScheduledRoutesDataAsync(downloadedData.scheduleroutes).ConfigureAwait(false);
                    }

                    if (downloadedData.routestation != null && downloadedData.routestation.Any())
                    {
                        await DbService.InsertOrUpdatetDownloadedRouteStationsDataAsync(downloadedData.routestation).ConfigureAwait(false);
                    }

                    if (downloadedData.ranks != null && downloadedData.ranks.Any())
                    {
                        await DbService.InsertOrUpdatetRankMasterDataAsync(downloadedData.ranks).ConfigureAwait(false);
                    }

                    if (downloadedData.positions != null && downloadedData.positions.Any())
                    {
                        await DbService.InsertOrUpdatetPositionMasterDataAsync(downloadedData.positions).ConfigureAwait(false);
                    }

                    if (downloadedData.LnkRackItem != null && downloadedData.LnkRackItem.Any())
                    {
                        await DbService.InsertOrUpdatetLnkRackItemsDataAsync(downloadedData.LnkRackItem).ConfigureAwait(false);
                    }

                    if (downloadedData.LnkPopItem != null && downloadedData.LnkPopItem.Any())
                    {
                        await DbService.InsertOrUpdateLnkPopItemsDataAsync(downloadedData.LnkPopItem).ConfigureAwait(false);
                    }
                    if (downloadedData.supplychain != null && downloadedData.supplychain.Any())
                    {
                        await DbService.InsertOrUpdatetSupplyChainDataAsync(downloadedData.supplychain).ConfigureAwait(false);
                    }

                    if (downloadedData.FavoriteEntity != null && downloadedData.FavoriteEntity.Any())
                    {
                        #region Previous code
                        downloadedData.FavoriteEntity.ForEach(x => x.IsExported = 1);
                        downloadedData.FavoriteEntity.ForEach(x => x.UserId = Convert.ToInt32(userid));

                        //await DbService.DeleteFavoriteDataAsync().ConfigureAwait(false);

                        ////await Task.Delay(50);

                        //await DbService.InsertOrUpdatetFavoriteDataAsync(downloadedData.FavoriteEntity).ConfigureAwait(false);
                        #endregion
                        await DbService.InsertOrUpdatetDownloadedFavoriteDataAsync(downloadedData.FavoriteEntity).ConfigureAwait(false);
                    }

                    if (downloadedData.customerproduct != null && downloadedData.customerproduct.Any())
                    {
                        foreach (var item in downloadedData.customerproduct)
                        {
                            #region Previous Code
                            //item.isExported = 1;

                            //await DbService.DeleteProductDistrubutionDataAsync(item).ConfigureAwait(false);

                            ////await Task.Delay(50);

                            //await DbService.InsertOrUpdatetProductDistributionDataAsync(item).ConfigureAwait(false);
                            #endregion
                            await DbService.DeleteAndInsertDownloadedProductDistributionDataAsync(item).ConfigureAwait(false);
                        }
                    }

                    if (downloadedData.accountclassification != null && downloadedData.accountclassification.Any())
                    {
                        await DbService.InsertOrUpdatetClassificationDataAsync(downloadedData.accountclassification).ConfigureAwait(false);
                    }

                    if (downloadedData.customerdata != null && downloadedData.customerdata.Any())
                    {
                        await DbService.BulkInsertOrUpdateCustomerMasterDataAsync(downloadedData.customerdata).ConfigureAwait(false);
                    }
                    if (downloadedData.orderdata != null && downloadedData.orderdata.Any())
                    {
                        await DbService.BulkInsertOrUpdateDownloadOrderMasterDataAsync(downloadedData.orderdata).ConfigureAwait(false);
                    }
                    if (downloadedData.orderdetails != null && downloadedData.orderdetails.Any())
                    {
                        await DbService.BulkInsertOrUpdateOrderDetailDataAsync(downloadedData.orderdetails).ConfigureAwait(false);
                    }
                    if (downloadedData.callactivitylist != null && downloadedData.callactivitylist.Any())
                    {
                        await DbService.BulkInsertOrUpdateDownloadedCallActivityListDataAsync(downloadedData.callactivitylist).ConfigureAwait(false);
                    }
                    if (downloadedData.UserActivityTypeEntity != null && downloadedData.UserActivityTypeEntity.Any())
                    {
                        await DbService.RunInTransactionUserActivityTypeAsync(downloadedData.UserActivityTypeEntity).ConfigureAwait(false);
                    }
                    if (downloadedData.CustomerActivityTypeEntity != null && downloadedData.CustomerActivityTypeEntity.Any())
                    {
                        await DbService.RunInTransactionCustomerActivityTypeAsync(downloadedData.CustomerActivityTypeEntity).ConfigureAwait(false);
                    }
                    if (downloadedData.DocumentTypeEntity != null && downloadedData.DocumentTypeEntity.Any())
                    {
                        await DbService.RunInTransactionDocumentTypeAsync(downloadedData.DocumentTypeEntity).ConfigureAwait(false);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(GetType().Name, "DownloadDataOnPartialSync", ex.Message + " - " + ex.StackTrace);

                return false;
            }
        }


        //public async Task<List<CategoryUIModel>> GetCategoryFilterDataForProductsList()
        //{
        //    try
        //    {
        //        var filteredCategoryData = await DbService.GetCategoryFiltersDataAsync();

        //        return filteredCategoryData.Select(x => x.CopyToUIModel()).ToList();
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetCategoryFilterDataForProductsList", ex);
        //        return null;
        //    }
        //}
        public async Task<List<CategoryUIModel>> GetCategoriesForProductsList()
        {
            try
            {
                this._mapper = this._profileMatcher.GetMapper(
                   (IMapperConfigurationExpression cfg) => this._profileMatcher.CreateCategoryDataMapping(cfg));

                var filteredCategoryData = await DbService.GetCategoriesAsync().ConfigureAwait(false);
                return this._mapper.Map<List<CategoryMaster>, List<CategoryUIModel>>(
                    filteredCategoryData
                    );
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetCategoriesForProductsList", ex);
                return null;
            }
        }

        public async Task<List<CategoryUIModel>> GetCategoryFilterDataForProductsList(int CategoryId)
        {
            try
            {
                this._mapper = this._profileMatcher.GetMapper(
                   (IMapperConfigurationExpression cfg) => this._profileMatcher.CreateCategoryDataMapping(cfg));

                var filteredCategoryData = await DbService.GetCategoriesAsync().ConfigureAwait(false);
                return this._mapper.Map<List<CategoryMaster>, List<CategoryUIModel>>(
                    filteredCategoryData
                    );
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetCategoryFilterDataForProductsList", ex);
                return null;
            }
        }

        public async Task<List<CategoryUIModel>> GetFilterCategoriesAsync(string ids)
        {
            try
            {
                this._mapper = this._profileMatcher.GetMapper(
                    (IMapperConfigurationExpression cfg) => this._profileMatcher.CreateCategoryDataMapping(cfg));

                var finalFilteredBrandData = await DbService.GetFilterCategoriesAsync(ids).ConfigureAwait(false);
                return this._mapper.Map<List<CategoryMaster>, List<CategoryUIModel>>(finalFilteredBrandData);
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetFilterCategoriesAsync", ex);
                return null;
            }
        }

        public async Task<List<BrandUIModel>> GetBrandsAsync()
        {
            try
            {
                this._mapper = this._profileMatcher.GetMapper(
                    (IMapperConfigurationExpression cfg) => this._profileMatcher.CreateBrandDataMapping(cfg));

                var finalFilteredBrandData = await DbService.GetBrandsAsync().ConfigureAwait(false);
                return this._mapper.Map<List<BrandData>, List<BrandUIModel>>(finalFilteredBrandData);
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetBrandFilterDataForProductsList", ex);
                return null;
            }
        }

        public async Task<List<BrandUIModel>> GetFilterBrandsAsync(string ids)
        {
            try
            {
                this._mapper = this._profileMatcher.GetMapper(
                    (IMapperConfigurationExpression cfg) => this._profileMatcher.CreateBrandDataMapping(cfg));

                var finalFilteredBrandData = await DbService.GetFilterBrandsAsync(ids).ConfigureAwait(false);
                return this._mapper.Map<List<BrandData>, List<BrandUIModel>>(finalFilteredBrandData);
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetFilterBrandsAsync", ex);
                return null;
            }
        }

        public async Task<List<StyleUIModel>> GetStylesDataAsync()
        {
            try
            {
                this._mapper = this._profileMatcher.GetMapper(
                   (IMapperConfigurationExpression cfg) => this._profileMatcher.CreateStyleDataMapping(cfg));
                var filteredStyle = await DbService.GetStylesDataAsync().ConfigureAwait(false);

                return this._mapper.Map<List<StyleData>, List<StyleUIModel>>(filteredStyle
                     );
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetStyleFilterDataForProductsList", ex);
                return null;
            }
        }
        public async Task<List<StyleUIModel>> GetFilterStylesAsync(string ids)
        {
            try
            {
                this._mapper = this._profileMatcher.GetMapper(
                   (IMapperConfigurationExpression cfg) => this._profileMatcher.CreateStyleDataMapping(cfg));

                return this._mapper.Map<List<StyleData>, List<StyleUIModel>>(await DbService.GetFilterStylesAsync(ids).ConfigureAwait(false));
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, nameof(GetFilterStylesAsync), ex);
                return null;
            }
        }

        public async Task<LoggedInUserDetailsUIModel> GetLoggedInUserInformation(int userId)
        {
            LoggedInUserDetailsUIModel loggedInUserDetails = null;

            try
            {
                var loggedInUser = await DbService.GetLoggedInUserData(userId).ConfigureAwait(false);

                if (loggedInUser != null)
                {
                    loggedInUserDetails = new LoggedInUserDetailsUIModel
                    {
                        UserId = loggedInUser.UserId,
                        FirstName = loggedInUser.FirstName,
                        LastName = loggedInUser.LastName,
                        UserName = loggedInUser.UserName,
                        RoleId = loggedInUser.RoleID,
                        RegionId = loggedInUser.RegionId,
                        ZoneId = loggedInUser.ZoneId,
                        EmailId = loggedInUser.EmailID,
                        TerritoryId = loggedInUser.TerritoryID,
                        BDId = loggedInUser.BDID,
                        AVPId = loggedInUser.AVPID
                    };
                }

                return loggedInUserDetails;
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetLoggedInUserInformation", ex);
            }

            return loggedInUserDetails;
        }

        /// <summary>
        /// this methods return the UI object for customer details screen
        /// </summary>
        /// <returns>CustomerDetailScreenUIModel</returns>
        public async Task<CustomerDetailScreenUIModel> GetCustomerDetailsDataForViewAndEditAsync(int customerId)
        {
            CustomerDetailScreenUIModel uiModel = null;
            try
            {
                var selectedCustomer = await DbService.GetSavedCustomerInfoAsync(customerId);
                if (selectedCustomer != null)
                {
                    var classifications = await DbService.GetClassificationDictionaryAsync();

                    var contacts = await DbService.GetContactsForCustomer(selectedCustomer.DeviceCustomerID);

                    var ranks = await DbService.GetRankDictionaryAsync();

                    var stateDictionary = await DbService.GetStateDictionaryAsync();

                    List<DistributorAssignUser> selectedDistributionList = null;
                    List<DistributorAssignUser> distributionList = null;
                    distributionList = await DbService.GetDistributorAssignUserListAsync();
                    var customerDristributorData = await DbService.GetCustomerDistributorMasterAsync();
                    if (customerDristributorData != null && customerDristributorData?.Count > 0)
                    {
                        customerDristributorData = customerDristributorData.Where(x => x.DeviceCustomerID == selectedCustomer.DeviceCustomerID).OrderBy(x => x.DistributorPriority).ToList();
                        if (distributionList != null && distributionList.Count > 0)
                        {
                            selectedDistributionList = new List<DistributorAssignUser>();
                            foreach (var item in customerDristributorData)
                            {
                                var tempList = distributionList.Where(x => x.CustomerID.ToString().Equals(item.DistributorID)).ToList();
                                tempList.ForEach(x => { x.Priority = item.DistributorPriority; });
                                selectedDistributionList.AddRange(tempList);
                            }
                            //HS2-180 Honey | Customer Profile new change need to add filter isPayer = 1 accounts and not show them in this distributor list
                            distributionList = distributionList.Where(x => x.IsPayer != 1).ToList();
                        }
                    }

                    var zoneDictObject = await DbService.GetZoneMasterDictionary();
                    var regionDictObject = await DbService.GetRegionMasterDictionary();
                    var territory = await DbService.GetTerritoryMasterDataAsync();
                    var travelPointsData = await DbService.GetVripTravelDataForPrgramType("Travel");
                    var vripPointsData = await DbService.GetVripTravelDataForPrgramType("Vrip");
                    var positions = await DbService.GetPositionDictionaryAsync();
                    var territoryManagerList = await DbService.GetUserDataFromRoleId(1);
                    var regionManagerList = await DbService.GetUserDataFromRoleId(2);
                    var zoneManagerList = await DbService.GetUserDataFromRoleId(3);
                    var bdManagerList = await DbService.GetUserDataFromRoleId(await GetRoleIdAsync(ApplicationConstants.BDRoleName));
                    var avpList = await DbService.GetUserDataFromRoleId(await GetRoleIdAsync(ApplicationConstants.AVPRoleName));
                    int locationId = 0;
                    int territoryId;
                    Int32.TryParse(selectedCustomer.TerritoryID, out territoryId);
                    UserMaster selectedTerritoryManager = territoryManagerList?
                        .FirstOrDefault(x => x.TerritoryID.Split(',').Contains(selectedCustomer.TerritoryID)
                        && x.IsInActive == 0 && x.IsDeleted == 0);
                    if (selectedTerritoryManager != null)
                    {
                        locationId = selectedTerritoryManager?.RegionId ?? 0;
                    }
                    else
                    {
                        var singleTerritory = territory.FirstOrDefault(x => x.TerritoryID == territoryId);
                        if (singleTerritory != null)
                        {
                            locationId = singleTerritory.RegionID;
                        }
                    }
                    UserMaster selectedRegionManager = regionManagerList?
                        .FirstOrDefault(x => x.RegionId == locationId
                        && x.IsInActive == 0 && x.IsDeleted == 0);
                    if (selectedRegionManager != null)
                    {
                        locationId = selectedRegionManager?.ZoneId ?? 0;
                    }
                    else
                    {
                        if (regionDictObject.ContainsKey(locationId))
                        {
                            locationId = regionDictObject[locationId].ZoneID;
                        }
                    }
                    UserMaster selectedZoneManager = zoneManagerList?.FirstOrDefault(x => x.ZoneId == locationId && x.IsInActive == 0 && x.IsDeleted == 0);
                    ZoneMaster selectedZone = await DbService.GetZoneMasterByIdAsync(locationId);
                    UserMaster selectedAVP = avpList?.FirstOrDefault(x => x.AVPID == selectedZone?.AVPID && x.IsInActive == 0 && x.IsDeleted == 0);
                    TerritoryMaster territoryMaster = await DbService.GetTerritoryMasterByIdAsync(territoryId);
                    UserMaster selectedBDManager = bdManagerList?.FirstOrDefault(x => x.BDID == territoryMaster?.BDID && x.IsInActive == 0 && x.IsDeleted == 0);
                    var regionDict = regionDictObject?.ToDictionary(x => x.Key, y => y.Value.Regioname);
                    var zoneDict = zoneDictObject?.ToDictionary(x => x.Key, y => y.Value.ZoneName);
                    classifications.TryGetValue(Convert.ToInt32(selectedCustomer?.AccountClassification), out Classification selectedClassification);

                    uiModel = new CustomerDetailScreenUIModel()
                    {
                        SelectedAccountClassification = selectedClassification ?? null,
                        CustomerData = selectedCustomer,
                        Classifications = classifications != null ?
                        classifications.Where(x => x.Value.CustomerType == selectedCustomer.AccountType && x.Value.AccountClassificationId != 3 && x.Value.AccountClassificationId != 7 && x.Value.AccountClassificationId != 20).ToDictionary(x => x.Key, y => y.Value) : null,
                        States = stateDictionary,
                        OnLoadContactList = contacts != null ? contacts.Take(3).ToList() : null,
                        Ranks = ranks,
                        RegionName = HelperMethods.GetValueFromIdNameDictionary(regionDict, selectedCustomer.RegionId),
                        ZoneName = HelperMethods.GetValueFromIdNameDictionary(zoneDict, selectedCustomer.ZoneId),
                        TerritoryName = territory != null ? territory.FirstOrDefault(x => x.TerritoryID.ToString().Equals(selectedCustomer.TerritoryID))?.TerritoryName : null,
                        TravelPoints = travelPointsData?.ProgramYear == selectedCustomer.TravelYear ? travelPointsData : null,
                        VripPoints = vripPointsData?.ProgramYear == selectedCustomer.VripYear ? vripPointsData : null,
                        Positions = positions,
                        TerritoryManagerName = selectedTerritoryManager != null ? (selectedTerritoryManager.FirstName + " " + selectedTerritoryManager.LastName) : "",
                        RegionManagerName = selectedRegionManager != null ? (selectedRegionManager.FirstName + " " + selectedRegionManager.LastName) : "",
                        ZoneManagerName = selectedZoneManager != null ? (selectedZoneManager.FirstName + " " + selectedZoneManager.LastName) : "",
                        AVPName = selectedAVP != null ? (selectedAVP.FirstName + " " + selectedAVP.LastName) : "",
                        BDManagerName = selectedBDManager != null ? (selectedBDManager.FirstName + " " + selectedBDManager.LastName) : "",
                        MainDistributorList = selectedCustomer.AccountType == 2 ? distributionList?.Where(x => x.AccountType == 1).OrderBy(x => x.CustomerName) : null,
                        IsAssociatedCustomerAvailable = (Convert.ToBoolean(selectedCustomer.IsParent)) || (!Convert.ToBoolean(selectedCustomer.IsParent) && (selectedCustomer?.Parent ?? 0) > 0),
                    };
                    if (selectedDistributionList != null)
                    {
                        uiModel.DistributorList = new ObservableCollection<DistributorAssignUser>(
                             selectedDistributionList.OrderBy(x => x.Priority));
                        uiModel.OnLoadDistributorList = selectedDistributionList;
                    }
                    else
                    {
                        uiModel.DistributorList = new ObservableCollection<DistributorAssignUser>();
                    }

                    if (selectedCustomer?.Parent > 0)
                    {
                        uiModel.ChainPlayBookProduct = await DbService.IsCustomerInSRCAsync(selectedCustomer?.Parent ?? 0);
                        if (uiModel.ChainPlayBookProduct != null)
                        {
                            var productAdditionalDoc = await GetProductAdditionalDocumentData(uiModel.ChainPlayBookProduct.ProductId);
                            uiModel.ChainPlayBookProduct.SalesDocs = productAdditionalDoc?.SalesDocs;
                            uiModel.IsChainPlaybookAvailable = true;
                        }
                    }

                    uiModel.PopulateUiModel();
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetCustomerDetailsDataForViewAndEditAsync", ex);
            }
            return uiModel;
        }

        public async Task UpdateOrInsertCustomerData(CustomerMaster data, string userName, string pin)
        {
            try
            {
                var loggedInUser = await DbService.GetUserFromUserNameAndPin(userName, pin);

                data.UpdatedDate = DateTimeHelper.ConvertToDbInsertDateTimeFormat(DateTime.Now);

                data.UpdatedBy = loggedInUser?.UserId.ToString();

                await DbService.InsertOrUpdateCustomerMasterDataAsync(data);
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(nameof(QueryService), "UpdateOrInsertCustomerData", ex);
            }
        }

        public async Task InsertOrUpdateDistributorForCustomerProfilePage(ICollection<DistributorMaster> deletedRecords, ICollection<DistributorAssignUser> updadtedRecords, string userName, string pin, string selectedCustomerDeviceId)
        {
            try
            {
                var customerDistributors = await DbService.GetCustomerDistributorMasterAsync();

                var loggedInUser = await DbService.GetUserFromUserNameAndPin(userName.Trim(), pin.Trim()).ConfigureAwait(false);

                string nowDateTime = Helpers.DateTimeHelper.ConvertToDbInsertDateTimeFormat(DateTime.Now);

                if (deletedRecords != null)
                {
                    foreach (DistributorMaster records in deletedRecords)
                    {
                        CustomerDistributor alreadPresentRecordExist = customerDistributors.FirstOrDefault(x => x.DeviceCustomerID.Equals(selectedCustomerDeviceId)
                        && x.DistributorID.Equals(records.CustomerID.ToString()));

                        if (alreadPresentRecordExist != null)
                        {
                            alreadPresentRecordExist.IsDeleted = 1;
                            alreadPresentRecordExist.IsExported = 0;
                            alreadPresentRecordExist.CreatedDate = alreadPresentRecordExist.UpdatedDate = nowDateTime;
                            await DbService.InsertOrUpdateCustomerDistributorDataAsync(alreadPresentRecordExist);
                        }
                    }
                }

                for (int i = 0; i < updadtedRecords.Count; i++)
                {
                    var item = updadtedRecords.ElementAt(i);

                    var alreadPresentRecordExist = customerDistributors.FirstOrDefault(x => x.DeviceCustomerID.Equals(selectedCustomerDeviceId)
                    && x.DistributorID.Equals(item.CustomerID.ToString()));

                    if (alreadPresentRecordExist == null)
                    {
                        var customerDistributorData = new CustomerDistributor()
                        {
                            CustomerDistributorID = HelperMethods.GenerateRandomNumberForGivenRange(10000, 99999),
                            IsExported = 0,
                            IsDeleted = 0,
                            UpdatedDate = nowDateTime,
                            DeviceCustomerID = selectedCustomerDeviceId,
                            DistributorID = item.CustomerID.ToString(),
                            UpdatedBy = loggedInUser.UserId.ToString(),
                            CreatedDate = nowDateTime,
                            DistributorPriority = i,
                            CreatedBy = loggedInUser.UserId.ToString()
                        };

                        await DbService.InsertOrUpdateCustomerDistributorDataAsync(customerDistributorData);
                    }
                    //else
                    //{
                    //    alreadPresentRecordExist.UpdatedBy = loggedInUser.UserId.ToString();
                    //    alreadPresentRecordExist.IsDeleted = 0;
                    //    alreadPresentRecordExist.IsExported = 0;
                    //    alreadPresentRecordExist.UpdatedDate = nowDateTime;
                    //    alreadPresentRecordExist.DistributorPriority = i;
                    //    alreadPresentRecordExist.DeviceCustomerID = selectedCustomerDeviceId;
                    //    alreadPresentRecordExist.DistributorID = item.CustomerID.ToString();

                    //    await DbService.InsertOrUpdateCustomerDistributorDataAsync(alreadPresentRecordExist);
                    //}
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(nameof(QueryService), nameof(InsertOrUpdateDistributorForCustomerProfilePage), ex);
            }
        }

        public async Task InsertOrUpdateContactForCustomerProfilePage(ICollection<ContactMaster> deletedRecords, ICollection<ContactMaster> updatedRecords, string userName, string pin, string selectedCustomerId)
        {
            try
            {
                var loggedInUser = await DbService.GetUserFromUserNameAndPin(userName.Trim(), pin.Trim()).ConfigureAwait(false);

                var nowDateTime = DateTimeHelper.ConvertToDbInsertDateTimeFormat(DateTime.Now);

                if (deletedRecords != null)
                {
                    foreach (var item in deletedRecords)
                    {
                        //item.IsDeleted = 1;
                        //item.IsExported = 0;
                        //item.UpdatedBy = loggedInUser?.UserId.ToString();
                        //item.UpdatedDate = nowDateTime;

                        //await DbService.InsertOrUpdatetContactMasterDataAsync(item);
                        await DbService.DeleteExistingContactData(item);
                    }
                }

                if (updatedRecords != null)
                {
                    foreach (var item in updatedRecords)
                    {
                        Guid deviceContactId = Guid.NewGuid();

                        item.IsDeleted = 0;
                        item.IsExported = 0;
                        item.DeviceCustomerID = selectedCustomerId;
                        item.DeviceContactID = !string.IsNullOrEmpty(item.DeviceContactID) ? item.DeviceContactID : deviceContactId.ToString();
                        item.RankID = HelperMethods.GetKeyFromIdNameDictionary(item.Ranks, item.SelectedRank);
                        item.PositionID = HelperMethods.GetKeyFromIdNameDictionary(item.Positions, item.SelectedPosition);

                        if (item.ContactID == null)
                        {
                            var randomContactId = HelperMethods.GenerateRandomNumberForGivenRange(10000, 99999);

                            item.ContactID = randomContactId;
                        }

                        if (string.IsNullOrEmpty(item.CreatedBy))
                        {
                            item.CreatedBy = loggedInUser?.UserId.ToString();
                        }
                        else
                        {
                            item.UpdatedBy = loggedInUser?.UserId.ToString();
                        }

                        if (string.IsNullOrEmpty(item.CreatedDate))
                        {
                            item.CreatedDate = nowDateTime;
                        }
                        else
                        {
                            item.UpdatedDate = nowDateTime;
                        }

                        var newlyUpdateContact = await DbService.InsertOrUpdatetContactMasterDataAsync(item);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(nameof(QueryService), "InsertOrUpdateContactForCustomerProfilePage", ex);
            }
        }

        public async Task<AddCustomerUiModel> GetAddCustomerPageData(string userName, string pin)
        {
            try
            {
                var ranks = await DbService.GetRankDictionaryAsync();
                var classifications = await DbService.GetClassificationDictionaryAsync();
                Dictionary<int, string> states = new Dictionary<int, string>();
                states.Add(0, "Select State");
                var statesFromApi = await DbService.GetStateDictionaryAsync();
                foreach (var item in statesFromApi)
                {
                    states[item.Key] = item.Value;
                }
                var distributionList = await DbService.GetDistributorAssignUserListAsync();
                if (distributionList != null && distributionList.Any())
                {
                    //HS2-180 Honey | Customer Profile new change need to add filter isPayer = 1 accounts and not show them in this distributor list
                    distributionList = distributionList.Where(x => x.IsPayer != 1).ToList();
                }
                var loggedInUser = await DbService.GetUserFromUserNameAndPin(userName, pin).ConfigureAwait(false);
                var regions = await DbService.GetRegionMasterDictionary();
                var zones = await DbService.GetZoneMasterDictionary();
                var territory = await DbService.GetTerritoryMasterDataAsync();
                IEnumerable<TerritoryMaster> filteredTerritory = new List<TerritoryMaster>();
                if (loggedInUser.RoleID == await GetRoleIdAsync(ApplicationConstants.BDRoleName)) // get territories for BD
                {
                    filteredTerritory = await DbService.GetBDApproverTerritoriesAsync(loggedInUser.BDID, loggedInUser.RegionId);

                }
                else if (loggedInUser.RoleID == await GetRoleIdAsync(ApplicationConstants.AVPRoleName)) // get territories for AVP
                {
                    filteredTerritory = await DbService.GetAVPTerritoriesAsync(loggedInUser.AVPID);
                }
                else
                {
                    var territoryIds = loggedInUser.TerritoryID.Split(',');
                    filteredTerritory = territory.Where(x => territoryIds != null && territoryIds.Contains(x.TerritoryID.ToString()));
                }
                AddCustomerUiModel uiObj = new AddCustomerUiModel()
                {
                    LoggedInUser = loggedInUser,
                    RegionDictionary = regions,
                    ZoneDictionary = zones,
                    TerritoryListItemSource = new ObservableCollection<TerritoryMaster>(filteredTerritory),
                    SelectedTerritory = filteredTerritory.FirstOrDefault(),
                    Ranks = ranks,
                    States = states,
                    Classifications = classifications.Where(x => x.Value.CustomerType == 2 && x.Value.AccountClassificationId != 3 && x.Value.AccountClassificationId != 7 && x.Value.AccountClassificationId != 20).ToDictionary(x => x.Key, y => y.Value),
                    MainDistributorList = distributionList.Where(x => x.AccountType == 1).OrderBy(x => x.CustomerName).ToList(),
                };
                uiObj.PopulateUI();

                return uiObj;
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(nameof(QueryService), nameof(GetAddCustomerPageData), ex);
                return null;
            }
        }

        public async Task<CustomerMaster> AddNewCustomerDataToDatabase(CustomerMaster customer, ICollection<DistributorAssignUser> distributions, UserMaster loggedInUser)
        {
            try
            {
                if (string.IsNullOrEmpty(customer.DeviceCustomerID))
                {
                    Guid customerdeviceguid = Guid.NewGuid();
                    customer.DeviceCustomerID = customerdeviceguid.ToString();
                }

                if (customer.CustomerID == null)
                {
                    var randonCustomerId = HelperMethods.GenerateRandomNumberForGivenRange(10000, 99999);

                    customer.CustomerID = randonCustomerId;
                }

                var updatedCustomer = await DbService.InsertOrUpdateCustomerMasterDataAsync(customer);

                if (distributions != null && distributions.Count() > 0)
                {
                    for (int i = 0; i < distributions.Count; i++)
                    {
                        var item = distributions.ElementAt(i);

                        var nowDateTime = DateTimeHelper.ConvertToDbInsertDateTimeFormat(DateTime.Now);

                        var customerDistributorData = new CustomerDistributor()
                        {
                            IsExported = 0,
                            IsDeleted = 0,
                            UpdatedDate = nowDateTime,
                            DeviceCustomerID = updatedCustomer?.DeviceCustomerID,
                            DistributorID = item?.CustomerID.ToString(),
                            UpdatedBy = loggedInUser?.UserId.ToString(),
                            CreatedDate = nowDateTime,
                            DistributorPriority = i,
                            CreatedBy = loggedInUser?.UserId.ToString(),
                        };

                        await DbService.InsertOrUpdateCustomerDistributorDataAsync(customerDistributorData);
                    }
                }

                return updatedCustomer;
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(nameof(QueryService), "AddNewCustomerDataToDatabase", ex);

                return null;
            }
        }

        public async Task<OrderMaster> InsertOrUpdateOrderMaster(OrderMaster orderMaster, bool reInsert = true)
        {
            try
            {
                if (reInsert)
                {
                    int randomOrderId = HelperMethods.GenerateRandomNumberForGivenRange(10000, 99999);

                    orderMaster.OrderID = randomOrderId;

                    Guid newDeviceOrderId = Guid.NewGuid();

                    orderMaster.DeviceOrderID = newDeviceOrderId.ToString();
                }
                var newOrder = await DbService.InsertOrUpdateOrderMasterDataAsync(orderMaster).ConfigureAwait(false);

                return newOrder;
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(nameof(QueryService), "InsertOrUpdateOrderMaster", ex);
                return null;
            }
        }

        public async Task<OrderDetail> InsertOrUpdateOrderDetail(OrderDetail orderDetail)
        {
            try
            {
                if (orderDetail?.OrderDetailId == null)
                {
                    var randomOrderDetailId = HelperMethods.GenerateRandomNumberForGivenRange(10000, 99999);

                    orderDetail.OrderDetailId = randomOrderDetailId;
                }

                var updatedOrder = await DbService.InsertOrUpdatetOrderDetailDataAsync(orderDetail).ConfigureAwait(false);

                return updatedOrder;
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(QueryService), nameof(InsertOrUpdateOrderDetail), ex.StackTrace);
                return null;
            }
        }

        public async Task<bool> DeleteOrderDetail(int productId, string deviceOrderId)
        {
            try
            {
                return await DbService.DeleteCartItem(productId, deviceOrderId).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(QueryService), nameof(DeleteOrderDetail), ex.StackTrace);
                return false;
            }
        }

        public async Task DeleteCartDetailOnPlaceOrder(string deviceOrderId)
        {
            try
            {
                await DbService.DeleteCartDataOnPlaceOrder(deviceOrderId);
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "DeleteCartDetailOnPlaceOrder", ex);
            }
        }


        public async Task<List<FavoriteUiModel>> GetFavoriteGridData()
        {
            try
            {
                var favoriteList = await DbService.GetFavoriteProducts();
                // commented by AK on 19/10 for sending all data to server
                // return favoriteList.Select(x => x.FavoriteDataToUiModel()).OrderBy(x => x.ProductId).ToList();
                var tempList = favoriteList.Where(x => x.isDeleted == false).ToList();
                return tempList.Select(x => x.FavoriteDataToUiModel()).OrderBy(x => x.ProductId).ToList();
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetFavoriteGridData", ex);

                return null;
            }
        }

        public async Task<List<CategoryMaster>> GetCategoryData()
        {
            try
            {
                var categoryData = await DbService.GetCategoryDataForProductAsync();

                return categoryData.OrderBy(x => x.CategoryID).ToList();
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetCategoryData", ex);
                return null;
            }
        }

        public async Task InsertFavorite(List<Favorite> favoriteList)
        {
            try
            {
                await DbService.InsertOrUpdatetFavoriteDataAsync(favoriteList).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "InsertFavorite", ex);
            }
        }

        public async Task<CartDetailsUIModel> GetCartDetailsDataForCartScreen(string CurrentOrderId, int CustomerId)
        {
            //var selectedCustomer = await DbService.GetSavedCustomerInfoAsync(CustomerId);
            var selectedCustomer = await DbService.GetCartCustomerInfoAsync(CustomerId);

            var orderDetailsData = await DbService.GetCartDetailsData(CurrentOrderId);

            CartDetailsUIModel cartDetails = new CartDetailsUIModel();

            try
            {
                //cartDetails.CustomerData = selectedCustomer;
                cartDetails.CustomerNameNumber = selectedCustomer.CustomerName + " " +
                    (!string.IsNullOrWhiteSpace(selectedCustomer.CustomerNumber) ? selectedCustomer.CustomerNumber : "");
                cartDetails.CustomerAccountType = selectedCustomer.AccountType;
                cartDetails.OrderDetailsList = orderDetailsData;

                cartDetails.PopulateCartDetailsUiModel();

                return cartDetails;
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetCartDetailsDataForCartScreen", ex);
                return null;
            }
        }

        public async Task<List<OrderDetailUIModel>> GetCartProductDetailsData(string CurrentOrderId)
        {
            try
            {
                var cartItemDetailsList = await DbService.GetCartDetailsData(CurrentOrderId).ConfigureAwait(false);

                return cartItemDetailsList;
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetCartProductDetailsData", ex);
                return null;
            }
        }

        public async Task<ProductDetailUiModel> GetProductAdditionalDocumentData(int productId)
        {
            try
            {
                var productAddDocs = await DbService.GetProductAdditionalDocumentsAsync(productId).ConfigureAwait(false);

                if (productAddDocs != null)
                {
                    string[] separateFileName = productAddDocs.DocumentFileName.Split('|');
                    string[] docType = productAddDocs.DocumentType.Split('|');
                    var productAddDocUI = new ProductDetailUiModel();
                    for (int i = 0; i < docType.Length - 1; i++)
                    {
                        switch (docType[i])
                        {
                            case "1":
                                productAddDocUI.Factsheet = separateFileName[i];
                                break;
                            case "2":
                                productAddDocUI.ProductImage = separateFileName[i];
                                break;
                            case "4":
                                productAddDocUI.RetailImage = separateFileName[i];
                                break;
                            case "5":
                                productAddDocUI.IpImage = separateFileName[i];
                                break;
                            case "-1":
                                productAddDocUI.SalesDocs = separateFileName[i];
                                break;
                        }
                    }
                    return productAddDocUI;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(GetType().Name, nameof(GetProductAdditionalDocumentData), ex);
            }
            return null;
        }

        public async Task<ObservableCollection<CustomerDocumentUIModel>> GetCustomerDocuments(int customerId)
        {
            try
            {
                var docs = await DbService.GetCustomerDocumentsAsync(customerId);

                var uimodel = new List<CustomerDocumentUIModel>(docs?.Select(x => x.CopyTo()));

                var orderedList = uimodel.OrderByDescending(a => a.DocumentDateTime).ToList();

                var documentsList = new ObservableCollection<CustomerDocumentUIModel>(orderedList);

                return documentsList;
            }
            catch (Exception e)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetCustomerDocuments", e);
                return null;
            }
        }

        public async Task<CustomerDocument> InsertOrUpdateCustomerDocument(CustomerDocumentUIModel document)
        {
            try
            {
                var isExportedDelete = document.CustomerDocument != null ? document.CustomerDocument.IsExported : 0; //to check if deleted document is synced or not
                if (document.DocumentId == null)
                {
                    var listOfDoc = new List<CustomerDocument>();

                    var parentDocument = new CustomerDocument()
                    {
                        CustomerDocumentID = HelperMethods.GenerateRandomNumberForGivenRange(10000, 99999),
                        CreateDateTime = DateTimeHelper.ConvertToDbInsertDateTimeFormat(DateTime.Now),
                        CustomerDocDesc = document.DocDesc,
                        CustomerDocType = document.DocType,
                        CustomerDocumentName = document.DocUrl,
                        CustomerID = document.CustomerId,
                        IsDelete = "0",
                        IsExported = 0,
                        IsPublishedToChild = document.IsPublishToChildren ? "1" : "0",
                        IsDownload = 0,
                        OriginalFileName = document.DocUrl,
                    };

                    listOfDoc.Insert(0, parentDocument);

                    if (document.IsPublishToChildren)
                    {
                        ///find the children
                        var customerList = await DbService.GetCustomerDataAsync();

                        var childrenList = customerList.Where(x => x.Parent == document.CustomerId);

                        foreach (var child in childrenList)
                        {
                            var childDocument = new CustomerDocument()
                            {
                                CustomerDocumentID = HelperMethods.GenerateRandomNumberForGivenRange(10000, 99999),
                                CreateDateTime = DateTimeHelper.ConvertToDbInsertDateTimeFormat(DateTime.Now),
                                CustomerDocDesc = document.DocDesc,
                                CustomerDocType = document.DocType,
                                CustomerDocumentName = document.DocUrl,
                                CustomerID = child.CustomerID.Value,
                                IsDelete = "0",
                                IsExported = 0,
                                IsPublishedToChild = "0",
                                IsDownload = 0,
                                OriginalFileName = document.DocUrl,
                            };

                            listOfDoc.Add(childDocument);
                        }
                    }
                    var returnList = await DbService.InsertOrUpdatetCustomerDocumentDataAsync(listOfDoc, isExportedDelete);

                    var tempList = new List<CustomerDocument>();

                    if (returnList != null)
                    {
                        foreach (var doc in returnList)
                        {
                            if (string.IsNullOrWhiteSpace(doc.DeviceDocID))
                            {
                                await Task.Delay(50);

                                Guid deviceDocumentId = Guid.NewGuid();

                                doc.DeviceDocID = deviceDocumentId.ToString();

                                var addedDoc = await DbService.InsertOrUpdatetCustomerDocumentDataAsync(new List<CustomerDocument> { doc }, isExportedDelete);

                                tempList.AddRange(addedDoc);
                            }
                        }
                    }
                    return tempList.FirstOrDefault();
                }
                else
                {
                    document.CopyToDeleteObject();

                    var returnList = await DbService.InsertOrUpdatetCustomerDocumentDataAsync(new List<CustomerDocument>() { document.CustomerDocument }, isExportedDelete);

                    return returnList.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "InsertOrUpdateCustomerDocument", ex);

                return null;
            }
        }

        public async Task<RetailTransactionUIModel> GetRetailTransactionData(int customerId, string userName, string pin)
        {
            try
            {
                var selectedCustomer = await DbService.GetSavedCustomerInfoAsync(customerId).ConfigureAwait(false);
                var stateDictionary = await DbService.GetStateDictionaryAsync().ConfigureAwait(false);
                var loggedInUserInfo = await DbService.GetUserFromUserNameAndPin(userName.Trim(), pin.Trim()).ConfigureAwait(false);
                var customerDristributorData = await DbService.GetCustomerDistributorMasterAsync().ConfigureAwait(false);
                var distributionList = await DbService.GetDistributorAssignUserListAsync().ConfigureAwait(false);
                var taxStatementData = await DbService.GetUserTaxStatementItemsAsync().ConfigureAwait(false);

                customerDristributorData = customerDristributorData.Where(x => x.DeviceCustomerID == selectedCustomer?.DeviceCustomerID).OrderBy(x => x.DistributorPriority).ToList();

                var selectedDistributionList = new List<DistributorAssignUser>();

                foreach (var item in customerDristributorData)
                {
                    var tempList = distributionList.Where(x => x.CustomerID.ToString().Equals(item.DistributorID)).ToList();

                    tempList.ForEach(x => { x.Priority = item.DistributorPriority; });

                    selectedDistributionList.AddRange(tempList);
                }

                RetailTransactionUIModel transactionUIModel = new RetailTransactionUIModel()
                {
                    CustomerData = selectedCustomer,
                    IsDirectCustomer = selectedCustomer.AccountType != 2,
                    CustomerName = selectedCustomer.CustomerName,
                    Address = (selectedCustomer.AccountType.Equals(1) ? selectedCustomer.ShippingAddress : selectedCustomer.PhysicalAddress),
                    CityName = (selectedCustomer.AccountType.Equals(1) ? selectedCustomer.ShippingAddressCityID : selectedCustomer.PhysicalAddressCityID),
                    States = stateDictionary,
                    PrebookState = stateDictionary,
                    Zip = (selectedCustomer.AccountType.Equals(1) ? selectedCustomer.ShippingAddressZipCode : selectedCustomer.PhysicalAddressZipCode),
                    StateTobaccoLicense = selectedCustomer.StateTobaccoLicense,
                    RetailerLicense = selectedCustomer.RetailerLicense,
                    RetailerSalesTaxCertificate = selectedCustomer.RetailerSalesTaxCertificate,
                    RepublicSalesRepName = loggedInUserInfo.FirstName + " " + loggedInUserInfo.LastName,
                    SellerRepresentativeTobaccoPermit = loggedInUserInfo.SellerRepresentativeTobaccoPermitNo,
                    UserEmailId = loggedInUserInfo.EmailID,
                    LoggedInUserId = loggedInUserInfo.UserId,
                    UserPhone = loggedInUserInfo.ContactNo,
                    LoggedInUsername = loggedInUserInfo.UserName,
                    PrebookDistributorList = selectedDistributionList,
                    UserTaxStatementList = taxStatementData
                };

                transactionUIModel.PopulateRetailTransactionUiModel();

                return transactionUIModel;
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(GetType().Name, "GetRetailTransactionData", ex);
            }

            return null;
        }

        public async Task<bool> InsertOrUpdateDataOnConfirmOrder(RetailTransactionUIModel retailsTransactionData)
        {
            try
            {
                if (retailsTransactionData != null)
                {
                    var orderData = new OrderMaster()
                    {
                        OrderID = retailsTransactionData.OrderId,
                        CustomerID = retailsTransactionData.CustomerId,
                        CustomerDistributorID = retailsTransactionData.CustomerDistributorId,
                        DeviceOrderID = retailsTransactionData.DeviceOrderId,
                        DeviceCustomerID = retailsTransactionData.CustomerData.DeviceCustomerID,
                        IsExported = 0,
                        PrintName = retailsTransactionData.PrintName,
                        InvoiceNumber = retailsTransactionData.InvoiceNumber,
                        CustomerSignatureFileName = retailsTransactionData.CustomerSignatureFileName,
                        SalesType = retailsTransactionData.SelectedSalesType,
                        StateTobaccoLicence = retailsTransactionData.StateTobaccoLicense,
                        OrderAddress = retailsTransactionData.Address,
                        OrderCityId = retailsTransactionData.CityName,
                        OrderStateId = HelperMethods.GetKeyFromIdNameDictionary(retailsTransactionData.States, retailsTransactionData.SelectedPhysicalState),
                        OrderZipCode = retailsTransactionData.Zip,
                        RetailerLicense = retailsTransactionData.RetailerLicense,
                        RetailerSalesTaxCertificate = retailsTransactionData.RetailerSalesTaxCertificate,
                        EmailRecipients = retailsTransactionData.EmailTo,
                        CustomerComment = retailsTransactionData.RetailsSalesCallNotes,
                        CustomStatement = retailsTransactionData.CustomTaxStatement,
                        PurchaseOrderNumber = retailsTransactionData.PurchaseOrderNumber,
                        OrderMasterSellerRepTobacco = retailsTransactionData.SellerRepresentativeTobaccoPermit,
                        OrderMasterSellerName = retailsTransactionData.RepublicSalesRepName,
                        IsOrderEmailSent = 0,
                        ZoneId = retailsTransactionData.CustomerData.ZoneId,
                        RegionId = retailsTransactionData.CustomerData.RegionId,
                        TerritoryId = Convert.ToInt32(!string.IsNullOrEmpty(retailsTransactionData.CustomerData.TerritoryID)),
                        CustomerName = retailsTransactionData.CustomerName,
                        OrderDate = retailsTransactionData.OrderDate,
                        CreatedBy = Convert.ToString(retailsTransactionData.LoggedInUserId),
                        CreatedDate = retailsTransactionData.OrderDate,
                        UpdatedDate = retailsTransactionData.OrderDate,
                        UpdatedBy = Convert.ToString(retailsTransactionData.LoggedInUserId),
                        GrandTotal = retailsTransactionData.GrandTotal,
                        ImportedFrom = 1,
                        PrebookShipDate = retailsTransactionData.PreBookShipDate,
                        IsOrderConfirmed = 1,
                        RepublicSalesRepository = retailsTransactionData.RepublicSalesRepName,

                        //CustomerShippingCityID = string.IsNullOrEmpty(retailsTransactionData.CustomerData.ShippingAddressCityID) ?
                        //retailsTransactionData.CityName : retailsTransactionData.CustomerData.ShippingAddressCityID,

                        //CustomerShippingStateID = (retailsTransactionData.CustomerData.ShippingAddressStateID == 0) ?
                        //HelperMethods.GetKeyFromIdNameDictionary(retailsTransactionData.States, retailsTransactionData.SelectedPhysicalState)
                        //: retailsTransactionData.CustomerData.ShippingAddressStateID,

                        //CustomerShippingZipCode = string.IsNullOrEmpty(retailsTransactionData.CustomerData.ShippingAddressZipCode) ?
                        //retailsTransactionData.Zip : retailsTransactionData.CustomerData.ShippingAddressZipCode

                        CustomerShippingCityID = string.IsNullOrEmpty(retailsTransactionData.CityName) ?
                        retailsTransactionData.CustomerData.ShippingAddressCityID : retailsTransactionData.CityName,

                        CustomerShippingStateID = string.IsNullOrEmpty(retailsTransactionData.SelectedPhysicalState) ?
                        retailsTransactionData.CustomerData.ShippingAddressStateID
                        : HelperMethods.GetKeyFromIdNameDictionary(retailsTransactionData.States, retailsTransactionData.SelectedPhysicalState),

                        CustomerShippingZipCode = string.IsNullOrEmpty(retailsTransactionData.Zip) ?
                         retailsTransactionData.CustomerData.ShippingAddressZipCode : retailsTransactionData.Zip,

                        RetailDistributorNumber = retailsTransactionData.RetailDistributorNumber
                    };

                    await DbService.InsertOrUpdateOrderMasterDataAsync(orderData).ConfigureAwait(false);

                    Guid deviceCallActivityId = Guid.NewGuid();

                    //Fixed to Address while placing order:- UNIQUE constraint failed: CallActivityList.CallActivityID
                    int randonCallActivityId = HelperMethods.GenerateRandomNumberForGivenRange(10000, 99999);
                    bool IsInLoop = true;
                    while (IsInLoop)
                    {
                        if (await DbService.IsCallActivityIdUniqueAsync(randonCallActivityId).ConfigureAwait(false))
                            IsInLoop = false;
                        else
                            randonCallActivityId = HelperMethods.GenerateRandomNumberForGivenRange(10000, 99999);
                    }

                    var activityData = new CallActivityList()
                    {
                        GrandTotal = Convert.ToDecimal(retailsTransactionData.GrandTotal),
                        CustomerID = retailsTransactionData.CustomerData.DeviceCustomerID,
                        OrderID = retailsTransactionData.DeviceOrderId,
                        UserID = retailsTransactionData.LoggedInUserId,
                        IsExported = 0,
                        isDeleted = 0,
                        CreatedDate = retailsTransactionData.OrderDate,
                        CallDate = DateTimeHelper.ConvertToDbInsertDateTimeFormat(DateTime.Now),
                        ActivityType = retailsTransactionData.ActivityType,
                        TerritoryID = Convert.ToInt32(retailsTransactionData.CustomerData.TerritoryID),
                        TerritoryName = retailsTransactionData.CustomerData.TerritoryName,
                        CallActivityDeviceID = Convert.ToString(deviceCallActivityId),
                        CallActivityID = randonCallActivityId,
                        Comments = retailsTransactionData.RetailsSalesCallNotes
                    };

                    await InfoLogger.GetInstance.WriteToLogAsync(SourceName: $"{nameof(QueryService)}:{nameof(InsertOrUpdateDataOnConfirmOrder)}"
                      , CustomeMessage: "Clicked ConfirmOrder To AddCallActivity");

                    await DbService.InsertOrUpdateCallActivityInRetailTranscationDataAsync(activityData).ConfigureAwait(false);

                    if (!string.IsNullOrWhiteSpace(retailsTransactionData.CustomerData.CustomerNumber)
                        && retailsTransactionData.CustomerData.CustomerNumber.ToLower().StartsWith("x"))
                    {
                        /*** JIRA Ticket https://republicbrands.atlassian.net/browse/HS2-57
                         * Update Last call date on Customer List
                         * Car Stock Order
                         */
                        switch (retailsTransactionData.ActivityType)
                        {
                            case "Car Stock Order":
                                retailsTransactionData.CustomerData.LastCallActivityDate = activityData.CallDate;
                                break;
                            default:
                                break;
                        }
                    }
                    else if (!retailsTransactionData.IsDirectCustomer && retailsTransactionData.CustomerData?.IsParent != 1)
                    {
                        /*** JIRA Ticket https://republicbrands.atlassian.net/browse/HS2-57
                         * Update Last call date on Customer List
                         * Indirect Stores:Cash Sale,Cash Sales Initiative
                         */
                        switch (retailsTransactionData.ActivityType)
                        {
                            case "Cash Sale":
                            case "Cash Sales Initiative":
                                retailsTransactionData.CustomerData.LastCallActivityDate = activityData.CallDate;
                                break;
                            default:
                                break;
                        }
                    }
                    retailsTransactionData.CustomerData.IsExported = 0;
                    retailsTransactionData.CustomerData.UpdatedDate = DateTimeHelper.ConvertToDbInsertDateTimeFormat(DateTime.Now);
                    retailsTransactionData.CustomerData.UpdatedBy = Convert.ToString(retailsTransactionData.LoggedInUserId);
                    await DbService.InsertOrUpdateCustomerMasterDataAsync(retailsTransactionData.CustomerData).ConfigureAwait(false);

                    // product distribution data
                    if (retailsTransactionData.SelectedSalesType != "2")
                    {
                        DateTime dateAndTime = DateTime.Now;
                        var DistributionDateInString = dateAndTime.ToString("MM/dd/yyyy").Replace("-", "/");
                        foreach (var orderDetail in retailsTransactionData.OrderDetailsData)
                        {
                            var productDistributionData = new ProductDistribution()
                            {
                                ProductId = orderDetail.ProductID,
                                CustomerId = retailsTransactionData.CustomerId,
                                DistributionDate = DistributionDateInString,
                                isExported = 0,
                                IsDeleted = 0
                            };

                            var updatedProdDistributeData = await DbService.InsertProductDistributionDataFromSRCList(productDistributionData).ConfigureAwait(false);

                            if (updatedProdDistributeData != null)
                            {
                                updatedProdDistributeData.CustomerProductID = updatedProdDistributeData.ID.ToString();

                                await DbService.InsertProductDistributionDataFromSRCList(updatedProdDistributeData).ConfigureAwait(false);
                            }
                        }
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "InsertOrUpdateDataOnConfirmOrder", ex);
            }

            return false;
        }

        public async Task RecordManualDistribution(int productId, int customerId)
        {
            try
            {
                DateTime dateAndTime = DateTime.Now;

                var DistributionDateInString = dateAndTime.ToString("MM/dd/yyyy").Replace("-", "/");

                var productDistributionData = new ProductDistribution()
                {
                    ProductId = productId,
                    CustomerId = customerId,
                    DistributionDate = DistributionDateInString,
                    isExported = 0,
                    IsDeleted = 0
                };

                var updatedProdDistributeData = await DbService.InsertProductDistributionDataFromSRCList(productDistributionData).ConfigureAwait(false);

                if (updatedProdDistributeData != null)
                {
                    updatedProdDistributeData.CustomerProductID = updatedProdDistributeData.ID.ToString();

                    await DbService.InsertProductDistributionDataFromSRCList(updatedProdDistributeData).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "RecordManualDistribution", ex);
            }
        }

        public async Task<bool> AddDistributionDate(int productId, int customerId)
        {
            bool result = true;
            try
            {
                DateTime dateAndTime = DateTime.Now;

                var DistributionDateInString = dateAndTime.ToString("MM/dd/yyyy").Replace("-", "/");

                var productDistributionData = new ProductDistribution()
                {
                    ProductId = productId,
                    CustomerId = customerId,
                    DistributionDate = DistributionDateInString,
                    isExported = 0,
                    IsDeleted = 0
                };

                var updatedProdDistributeData = await DbService.InsertProductDistributionDataFromSRCList(productDistributionData).ConfigureAwait(false);

                if (updatedProdDistributeData != null)
                {
                    updatedProdDistributeData.CustomerProductID = updatedProdDistributeData.ID.ToString();

                    await DbService.InsertProductDistributionDataFromSRCList(updatedProdDistributeData).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                result = false;
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "RecordManualDistribution", ex);
            }
            return result;
        }

        public async Task RemoveManualProductDistributionRecord(int productId, int customerId)
        {
            try
            {
                DateTime dateAndTime = DateTime.Now;

                var DistributionDateInString = dateAndTime.ToString("MM/dd/yyyy").Replace("-", "/");

                var productDistributionData = new ProductDistribution()
                {
                    ProductId = productId,
                    CustomerId = customerId,
                    DistributionDate = DistributionDateInString,
                    isExported = 0,
                    IsDeleted = 1
                };

                await DbService.InsertProductDistributionDataFromSRCList(productDistributionData).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "RemoveManualProductDistributionRecord", ex);
            }
        }

        public async Task<bool> RemoveDistributionDate(int productId, int customerId)
        {
            bool result = true;
            try
            {
                DateTime dateAndTime = DateTime.Now;

                var DistributionDateInString = dateAndTime.ToString("MM/dd/yyyy").Replace("-", "/");

                var productDistributionData = new ProductDistribution()
                {
                    ProductId = productId,
                    CustomerId = customerId,
                    DistributionDate = DistributionDateInString,
                    isExported = 0,
                    IsDeleted = 1
                };
                await DbService.InsertProductDistributionDataFromSRCList(productDistributionData).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                result = false;
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "RemoveManualProductDistributionRecord", ex);
            }
            return result;
        }

        public async Task<IEnumerable<string>> GetBrandImageURL()
        {
            try
            {
                var brands = await DbService.GetBrandDataForProductAsync();
                IEnumerable<string> urls = null;
                if (brands != null)
                    urls = brands.Select(x => x.ImageFileName);
                return urls;
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(GetType().Name, "GetBrandImageURL", ex.StackTrace + " - " + ex.Message);
                return null;
            }
        }

        public async Task<string> GetUserCustomerDeviceId(string userCustomername)
        {
            try
            {
                var UserSelfCustomer = await DbService.GetUserSelfCustomer(userCustomername);
                if (UserSelfCustomer == null)
                { return ""; }
                else
                { return UserSelfCustomer.DeviceCustomerID; }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(GetType().Name, "GetUserCustomerDeviceId", ex.StackTrace + " - " + ex.Message);
                return null;
            }
        }

        public async Task<Dictionary<int, string>> GetStateDict()
        {
            var states = await DbService.GetStateDictionaryAsync().ConfigureAwait(false);

            return states;
        }

        public async Task<Dictionary<int, Classification>> GetClassificationDict()
        {
            var classification = await DbService.GetClassificationDictionaryAsync();
            return classification;
        }

        public async Task<List<TravelUiModel>> GetTravelDataForUser(string year)
        {
            List<TravelUiModel> travelUiList;

            try
            {
                travelUiList = await DbService.GetTravelDataAsync(year);

                return travelUiList;
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetTravelDataForUser", ex);
                return null;
            }
        }

        public async Task<List<VripUiModel>> GetVripDataForUser(string year)
        {
            List<VripUiModel> vripUiList;

            try
            {
                vripUiList = await DbService.GetVripMasterDataAsync(year);

                return vripUiList;
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetVripDataForUser", ex);
                return null;
            }
        }

        public async Task<List<string>> GetTravelProgramYearFromVripTravelData()
        {

            try
            {
                var travelList = await DbService.GetTravelProgramYearFromVripTravelDataAsync();

                var filteredProgramYears = travelList.Select(x => x.ProgramYear).Distinct().ToList();

                return filteredProgramYears;
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(nameof(QueryService), nameof(GetTravelProgramYearFromVripTravelData), ex);
                return null;
            }
        }
        public async Task<List<string>> GetVripProgramYearFromVripTravelData()
        {
            try
            {
                var travelList = await DbService.GetVripProgramYearFromVripTravelDataAsync();

                var filteredProgramYears = travelList.Select(x => x.ProgramYear).Distinct().ToList();

                return filteredProgramYears;
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(nameof(QueryService), nameof(GetVripProgramYearFromVripTravelData), ex);
                return null;
            }
        }


        public async Task<IEnumerable<OrderHistoryUIModel>> GetOrderHistoryDataAsync(string customerId, string customerType)
        {
            try
            {
                var distributor = await DbService.GetDistributorMastersListAsync();
                var orderMasterData = await DbService.GetOrderMasterDataAsync(customerId, customerType);
                var orderIdsList = orderMasterData?.Select(x => x.OrderID);
                var orderDetails = await DbService.GetOrderDetailsAsIdAndTotalAsync(orderIdsList);

                var result = orderMasterData.Select(x => new OrderHistoryUIModel
                {
                    DeviceOrderID = x.DeviceOrderID,
                    DistributorName = distributor.FirstOrDefault(y => y.CustomerID.Equals(x.CustomerDistributorID))?.CustomerName,
                    Invoice = x.InvoiceNumber,
                    IsOrderConfirmed = x.IsOrderConfirmed,
                    OrderId = x.OrderID,
                    OrderPlacedOn = x.OrderDate.Split(' ')[0],
                    OrderOnDate = DateTime.Parse(x.OrderDate, CultureInfo.InvariantCulture),
                    SalesType = x.SalesType,
                    ShippingCompany = x.ShippingCompany,
                    TotalAmount = !string.IsNullOrEmpty(x.GrandTotal) ? string.Format("${0:0.00}", x.GrandTotal) : "$0.00",
                    TotalQuantity = orderDetails != null ? orderDetails.FirstOrDefault(z => !string.IsNullOrEmpty(z.DeviceOrderID) && z.DeviceOrderID.Equals(x.DeviceOrderID))?.Quantity.ToString() : "null",
                    TrackingNumber = x.TrackingNumber,
                    TrackingUrl = x.TrackingURL,
                    IsExported = x.IsExported,
                    SignaturePath = x.CustomerSignatureFileName
                });

                return result.OrderByDescending(x => x.OrderOnDate);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(QueryService), "GetOrderHistoryDataAsync", ex.StackTrace);
                return null;
            }
        }

        public async Task<IEnumerable<ItemHistoryUIModel>> GetItemHistoryDataAsync(string customerId, string customerType)
        {
            try
            {
                var orderMasterData = await DbService.GetOrderMasterDataAsync(customerId, customerType);
                var ids = orderMasterData.Select(x => x.OrderID);
                var orderDetails = await DbService.GetOrderDetailsInOrderId(ids);
                ids = orderDetails.Select(x => x.ProductId);
                var products = await DbService.GetProductsForItemHistory(ids);

                var result = orderDetails.Select(x => new ItemHistoryUIModel
                {
                    UOM = x.Unit,
                    ProductID = x.ProductId,
                    Price = string.Format("${0:0.00}", x.Price),
                    Order = orderMasterData?.FirstOrDefault(y => y.OrderID == x.OrderId.Value),
                    ProductDescription = products?.FirstOrDefault(y => y.ProductID == x.ProductId)?.Description,
                    ProductName = products?.FirstOrDefault(y => y.ProductID == x.ProductId)?.ProductName,
                    Product = products?.FirstOrDefault(y => y.ProductID == x.ProductId),
                    OrderOnDate = DateTime.Parse(orderMasterData?.FirstOrDefault(y => y.OrderID == x.OrderId.Value).OrderDate, CultureInfo.InvariantCulture),
                });

                return result.OrderByDescending(x => x.OrderOnDate);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(QueryService), "GetItemHistoryDataAsync", ex.StackTrace);
                return null;
            }
        }

        public async Task<bool> GetTravelVripPromotionContactDataForCustomer(string customerId)
        {
            try
            {
                var travelDataForCustomer = await DbService.GetTravelDataForCustomerAsync(customerId);
                var vripDataForCustomer = await DbService.GetVripDataForCustomerAsync(customerId);
                var promotionDataForCustomer = await DbService.GetPromotionsDataAsync(customerId);
                var contractDataForCustomer = await DbService.GetContractDataAsync(customerId);

                if (travelDataForCustomer != null || vripDataForCustomer != null || promotionDataForCustomer?.Count > 0 || contractDataForCustomer?.Count > 0)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.LogException(GetType().Name, nameof(GetTravelVripPromotionContactDataForCustomer), ex);
            }

            return false;
        }

        public async Task<TravelUiModel> GetTravelDataForCustomer(string customerId)
        {
            try
            {
                var travelDataForCustomer = await DbService.GetTravelDataForCustomerAsync(customerId);

                TravelUiModel travelUiModel = new TravelUiModel();

                if (travelDataForCustomer != null)
                {
                    travelUiModel.Awards = travelDataForCustomer.Awards;
                    travelUiModel.EarnedPoints = travelDataForCustomer.EarnedPoints;
                    travelUiModel.BonusPoints = travelDataForCustomer.BonusPoints;
                    travelUiModel.NetPoints = travelDataForCustomer.NetPoints;
                    travelUiModel.NeededPoint = travelDataForCustomer.NeededPoint;

                }

                return travelUiModel;
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetTravelDataForCustomer", ex);
                return null;
            }
        }

        public async Task<VripUiModel> GetVripDataForCustomer(string customerId)
        {
            try
            {
                var vripDataForCustomer = await DbService.GetVripDataForCustomerAsync(customerId);

                VripUiModel vripUiModel = new VripUiModel();

                if (vripDataForCustomer != null)
                {
                    vripUiModel.Csytd = vripDataForCustomer.Csytd;
                    vripUiModel.Target = vripDataForCustomer.Target;
                    vripUiModel.CSNeeded = vripDataForCustomer.CSNeeded;
                    vripUiModel.Cslyr = vripDataForCustomer.Cslyr;
                }

                return vripUiModel;
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetVripDataForCustomer", ex);
                return null;
            }
        }

        public async Task<List<PromotionUiModel>> GetPromotionDataForCustomer(string customerId)
        {
            try
            {
                var promotionDataForCustomer = await DbService.GetPromotionsDataAsync(customerId);

                List<PromotionUiModel> promotionsData = new List<PromotionUiModel>();

                if (promotionDataForCustomer != null && promotionDataForCustomer.Count > 0)
                {
                    foreach (var item in promotionDataForCustomer)
                    {
                        var promotionUiModel = new PromotionUiModel()
                        {
                            PromotionID = item.PromotionID,
                            PromotionPlanType = item.PromotionPlanType,
                            FirstPaymentID = item.FirstPaymentID,
                            FirstPaymentAmount = item.FirstPaymentAmount,
                            SecondPaymentID = item.SecondPaymentID,
                            SecondPaymentAmount = item.SecondPaymentAmount,
                            StartDate = item.StartDate,
                            EndDate = item.EndDate,
                        };

                        promotionsData.Add(promotionUiModel);
                    }
                }

                return promotionsData;
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetPromotionDataForCustomer", ex);
                return null;
            }
        }

        public async Task<List<ContractUiModel>> GetContractsDataForCustomer(string customerId)
        {
            try
            {
                var contractDataForCustomer = await DbService.GetContractDataAsync(customerId);

                List<ContractUiModel> contractsData = new List<ContractUiModel>();

                if (contractDataForCustomer != null && contractDataForCustomer.Count > 0)
                {
                    foreach (var item in contractDataForCustomer)
                    {
                        var contractUiModel = new ContractUiModel()
                        {
                            ContractID = item.ContractID,
                            ContractPlanType = item.ContractPlanType,
                            ContractYear = item.ContractYear,
                            NumberOfPayments = item.NumberOfPayments,
                            FirstPaymentID = item.FirstPaymentID,
                            FirstPaymentAmount = item.FirstPaymentAmount,
                            SecondPaymentID = item.SecondPaymentID,
                            SecondPaymentAmount = item.SecondPaymentAmount,
                            ThirdPaymentID = item.ThirdPaymentID,
                            ThirdPaymentAmount = item.ThirdPaymentAmount,
                            FourthPaymentID = item.FourthPaymentID,
                            FourthPaymentAmount = item.FourthPaymentAmount
                        };

                        contractsData.Add(contractUiModel);
                    }
                }

                return contractsData;
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetContractsDataForCustomer", ex);
                return null;
            }
        }

        public async Task<OrderHistoryDetailsPageUIModel> GetOrderHistoryDetailData(OrderHistoryDetailsPageUIModel referenceObject)
        {
            if (referenceObject != null)
            {
                var state = await DbService.GetStateDictionaryAsync();
                var order = await DbService.GetOrderMasterFromId(referenceObject.OrderHistoryModel.DeviceOrderID);
                var details = await DbService.GetOrderDetailsInOrderId(new List<int>() { order.OrderID });
                var products = await DbService.GetProductMasterDataForOrderHistory();
                var brandData = await DbService.GetBrandDataForOrderHistory();
                var styleData = await DbService.GetStyleDataForOrderHistory();
                var categoryData = await DbService.GetCategoryDataForOrderHistory();
                referenceObject.OrderMasterData = order;
                referenceObject.StateDictionary = state;
                if (details != null && details.Count() > 0)
                {
                    // Before sync
                    if (referenceObject?.OrderHistoryModel?.IsExported == 0)
                        referenceObject.DbOrderDetailsData = details.OrderBy(x => DateTimeHelper.ConvertToDBDateTime(x.CreatedDate));
                    // After sync
                    else
                        referenceObject.DbOrderDetailsData = details.OrderBy(x => x.OrderDetailId);
                }
                else
                {
                    referenceObject.DbOrderDetailsData = details;
                }
                referenceObject.DbProducts = products;
                referenceObject.DbStyleMaster = styleData;
                referenceObject.DbCategoryMaster = categoryData;
                referenceObject.DbBrandData = brandData;
                return referenceObject;
            }
            else
                return null;
        }

        public async Task<bool> DeleteOrderDetailFromHistoryPage(OrderDetail detail)
        {
            try
            {
                var result = await DbService.DeleteOrderDetailFromDetailsOrderId(detail);
                return result;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<RackOrderUiModel>> GetRackOrderListData()
        {
            try
            {
                var rackOrderList = await DbService.GetRackOrderListAsync();
                List<RackOrderUiModel> rackOrderListData = new List<RackOrderUiModel>();

                if (rackOrderList != null && rackOrderList.Count > 0)
                {
                    foreach (var item in rackOrderList)
                    {
                        var rackOrderUiModel = new RackOrderUiModel()
                        {
                            ProductID = item.ProductID,
                            BrandName = item.BrandName,
                            Description = item.Description,
                            DocumentFileName = item.DocumentFileName,
                            DocumentType = item.DocumentType,
                            IsDeleted = item.IsDeleted,
                            IsPopOrder = item.IsPopOrder,
                            SortOrder = item.SortOrder,
                            BrandId = item.BrandId
                        };

                        rackOrderListData.Add(rackOrderUiModel);
                    }
                }

                return rackOrderListData;

            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetRackOrderListData", ex);
                return null;
            }
        }

        public async Task<Dictionary<int, int>> GetCartItemsCount(string currentOrderId)
        {
            try
            {
                var cartData = await DbService.GetCartsProductsIdsAsync(currentOrderId);
                return cartData;

            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetCartItemsCount", ex);
                return null;
            }
        }

        public async Task<bool> DeleteAllCartItems(string deviceOrderId)
        {
            try
            {
                var success = await DbService.DeleteAllCartData(deviceOrderId).ConfigureAwait(false);
                return success;
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "DeleteAllCartItems", ex);
                return false;
            }
        }

        public async Task<List<RackOrderCartUiModel>> GetRackOrderCartData(int brandId, string CurrentDeviceOrderId)
        {
            try
            {
                var rackOrderCartList = await DbService.GetRackCartItemsAsync(brandId);
                var cartProduct = await DbService.GetCartsProductsIdQuantityAsync(CurrentDeviceOrderId);

                List<RackOrderCartUiModel> rackOrderCartListData = new List<RackOrderCartUiModel>();

                if (rackOrderCartList != null && rackOrderCartList.Count > 0)
                {
                    foreach (var item in rackOrderCartList)
                    {
                        int tempCart = 1;
                        var cartTemp = cartProduct?.TryGetValue(item.ProductID, out tempCart);
                        item.CartData = cartTemp.HasValue && cartTemp.Value;

                        if (cartTemp == true)
                        {
                            item.Quantity = tempCart;
                        }
                        else
                        {
                            item.Quantity = 1;
                        }

                        var rackOrderUiModel = new RackOrderCartUiModel()
                        {
                            ProductID = item.ProductID,
                            ProductName = item.ProductName,
                            Description = item.Description,
                            DocumentFileName = item.DocumentFileName,
                            DocumentType = item.DocumentType,
                            IsDeleted = item.IsDeleted,
                            Quantity = item.Quantity,
                            SRCCanIOrder = item.SRCCanIOrder,
                            SRCHoneyReturnable = item.SRCHoneyReturnable,
                            SRCHoneySellable = item.SRCHoneySellable,
                            BrandId = item.BrandId,
                            CatId = item.CatId,
                            CreatedDate = item.CreatedDate,
                            UpdatedDate = item.UpdatedDate,
                            Price = item.Price,
                            StyleId = item.StyleId,
                            IsAddedToCart = item.CartData,
                            QuantityDisplay = item.Quantity.ToString()
                        };

                        rackOrderCartListData.Add(rackOrderUiModel);
                    }
                }

                return rackOrderCartListData;

            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetRackOrderCartData", ex);
                return null;
            }
        }
        public async Task<UserMaster> GetUserData(string userName, string pin)
        {
            try
            {
                var result = await DbService.GetUserFromUserNameAndPin(userName, pin).ConfigureAwait(false);
                return result;
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(QueryService), nameof(GetUserData), ex.StackTrace);
                return null;
            }
        }
        //public async Task<DistributorMaster> GetDistributorFromId(int id)
        //{
        //    try
        //    {
        //        var distributorList = await DbService.GetDistributorMastersListAsync();
        //        var returnObject = distributorList.FirstOrDefault(x => x.CustomerID == id);
        //        return returnObject;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(QueryService), nameof(GetDistributorFromId), ex.StackTrace);
        //        return null;
        //    }
        //}
        public async Task<DistributorAssignUser> GetDistributorFromId(int id)
        {
            try
            {
                return await DbService.GetDistributorAssignUserAsync(id).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(QueryService), nameof(GetDistributorFromId), ex);
            }
            return null;
        }

        public async Task<IEnumerable<OrderDetail>> GetOrderDetailsFromOrderId(string deviceOrderId)
        {
            try
            {
                var details = await DbService.GetOrderDetailsFromOrderId(deviceOrderId);
                return details;
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(QueryService), "GetOrderDetailsFromOrderId", ex.StackTrace);
                return null;
            }
        }

        public async Task<PopOrderPageUiModel> GetPopOrderCartData(string CurrentDeviceOrderId)
        {
            try
            {
                var popOrderCartList = await DbService.GetPopCartItemsAsync();

                var cartProduct = await DbService.GetCartsProductsIdQuantityAsync(CurrentDeviceOrderId);

                var categoryList = await DbService.GetPopOrderCategoryAsync();

                var materialList = await DbService.GetPopOrderMaterialAsync();

                var familyList = await DbService.GetPopOrderFamilyAsync();

                var brandList = await DbService.GetPopOrderBrandAsync();

                var groupList = await DbService.GetPopOrderGroupAsync();

                BrandData placeholderCategory = new BrandData();
                placeholderCategory.BrandName = "Select";
                categoryList.Insert(0, placeholderCategory);

                BrandData placeholderMaterial = new BrandData();
                placeholderMaterial.BrandName = "Select";
                materialList.Insert(0, placeholderMaterial);

                BrandData placeholderFamily = new BrandData();
                placeholderFamily.BrandName = "Select";
                familyList.Insert(0, placeholderFamily);

                BrandData placeholderBrand = new BrandData();
                placeholderBrand.BrandName = "Select";
                brandList.Insert(0, placeholderBrand);

                BrandData placeholderGroup = new BrandData();
                placeholderGroup.BrandName = "Select";
                groupList.Insert(0, placeholderGroup);

                PopOrderPageUiModel popOrderPageUiModel = new PopOrderPageUiModel();

                List<PopOrderCartUiModel> popOrderCartListData = new List<PopOrderCartUiModel>();

                if (popOrderCartList != null && popOrderCartList.Count > 0)
                {
                    foreach (var item in popOrderCartList)
                    {
                        int tempCart = 1;
                        var cartTemp = cartProduct?.TryGetValue(item.ProductID, out tempCart);
                        item.CartData = cartTemp.HasValue && cartTemp.Value;

                        if (cartTemp == true)
                        {
                            item.Quantity = tempCart;
                        }
                        else
                        {
                            item.Quantity = 1;
                        }

                        var popOrderUiModel = new PopOrderCartUiModel()
                        {
                            ProductID = item.ProductID,
                            ProductName = item.ProductName,
                            Description = item.Description,
                            DocumentFileName = item.DocumentFileName,
                            DocumentType = item.DocumentType,
                            IsDeleted = item.IsDeleted,
                            Quantity = item.Quantity,
                            SRCCanIOrder = item.SRCCanIOrder,
                            SRCHoneyReturnable = item.SRCHoneyReturnable,
                            SRCHoneySellable = item.SRCHoneySellable,
                            BrandId = item.BrandId,
                            CatId = item.CatId,
                            CreatedDate = item.CreatedDate,
                            UpdatedDate = item.UpdatedDate,
                            Price = item.Price,
                            StyleId = item.StyleId,
                            IsAddedToCart = item.CartData,
                            QuantityDisplay = item.Quantity.ToString()
                        };

                        popOrderCartListData.Add(popOrderUiModel);
                    }
                }
                popOrderPageUiModel.PopOrderCartUiModelList = popOrderCartListData;

                var filteredcategoryList = categoryList?.Select(x => x.CopyToUIModel()).ToList();
                var filteredmaterialList = materialList?.Select(x => x.CopyToUIModel()).ToList();
                var filteredfamilyList = familyList?.Select(x => x.CopyToUIModel()).ToList();
                var filteredbrandList = brandList?.Select(x => x.CopyToUIModel()).ToList();
                var filteredgroupList = groupList?.Select(x => x.CopyToUIModel()).ToList();

                popOrderPageUiModel.CategoryList = filteredcategoryList;
                popOrderPageUiModel.MaterialList = filteredmaterialList;
                popOrderPageUiModel.FamilyList = filteredfamilyList;
                popOrderPageUiModel.BrandList = filteredbrandList;
                popOrderPageUiModel.GroupList = filteredgroupList;

                return popOrderPageUiModel;
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetPopOrderCartData", ex);
                return null;
            }
        }

        public async Task<List<PopOrderCartUiModel>> GetPopOrderRemainingListData(string CurrentDeviceOrderId)
        {
            try
            {
                var popOrderCartList = await DbService.GetRemainingPopCartItemsAsync();
                var cartProduct = await DbService.GetCartsProductsIdQuantityAsync(CurrentDeviceOrderId);
                List<PopOrderCartUiModel> popOrderCartListData = new List<PopOrderCartUiModel>();

                if (popOrderCartList != null && popOrderCartList.Count > 0)
                {
                    foreach (var item in popOrderCartList)
                    {
                        int tempCart = 1;
                        var cartTemp = cartProduct?.TryGetValue(item.ProductID, out tempCart);
                        item.CartData = cartTemp.HasValue && cartTemp.Value;

                        if (cartTemp == true)
                        {
                            item.Quantity = tempCart;
                        }
                        else
                        {
                            item.Quantity = 1;
                        }

                        var popOrderUiModel = new PopOrderCartUiModel()
                        {
                            ProductID = item.ProductID,
                            ProductName = item.ProductName,
                            Description = item.Description,
                            DocumentFileName = item.DocumentFileName,
                            DocumentType = item.DocumentType,
                            IsDeleted = item.IsDeleted,
                            Quantity = item.Quantity,
                            SRCCanIOrder = item.SRCCanIOrder,
                            SRCHoneyReturnable = item.SRCHoneyReturnable,
                            SRCHoneySellable = item.SRCHoneySellable,
                            BrandId = item.BrandId,
                            CatId = item.CatId,
                            CreatedDate = item.CreatedDate,
                            UpdatedDate = item.UpdatedDate,
                            Price = item.Price,
                            StyleId = item.StyleId,
                            IsAddedToCart = item.CartData,
                            QuantityDisplay = item.Quantity.ToString()
                        };

                        popOrderCartListData.Add(popOrderUiModel);
                    }
                }
                return popOrderCartListData;

            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetPopOrderRemainingListData", ex);
                return null;
            }
        }

        public async Task<List<PopOrderCartUiModel>> GetFilteredPopOrderCartData(string CurrentDeviceOrderId, int[] HierarchyTagArray, int HierarchyTag)
        {
            try
            {
                string hierarchyValues = string.Empty;

                for (int i = 0; i < HierarchyTagArray.Length; i++)
                {
                    if (HierarchyTagArray[i] != 0)
                    {
                        int index = i + 1;
                        hierarchyValues = " (Hierarchy" + index + "=" + HierarchyTagArray[i] + ") and " + hierarchyValues;
                    }
                }

                var popOrderCartList = await DbService.GetFilteredPopCartItemsAsync(hierarchyValues, HierarchyTag);

                var cartProduct = await DbService.GetCartsProductsIdQuantityAsync(CurrentDeviceOrderId);

                List<PopOrderCartUiModel> popOrderCartListData = new List<PopOrderCartUiModel>();

                if (popOrderCartList != null && popOrderCartList.Count > 0)
                {
                    foreach (var item in popOrderCartList)
                    {
                        int tempCart = 1;
                        var cartTemp = cartProduct?.TryGetValue(item.ProductID, out tempCart);
                        item.CartData = cartTemp.HasValue && cartTemp.Value;

                        if (cartTemp == true)
                        {
                            item.Quantity = tempCart;
                        }
                        else
                        {
                            item.Quantity = 1;
                        }

                        var popOrderUiModel = new PopOrderCartUiModel()
                        {
                            ProductID = item.ProductID,
                            ProductName = item.ProductName,
                            Description = item.Description,
                            DocumentFileName = item.DocumentFileName,
                            DocumentType = item.DocumentType,
                            IsDeleted = item.IsDeleted,
                            Quantity = item.Quantity,
                            SRCCanIOrder = item.SRCCanIOrder,
                            SRCHoneyReturnable = item.SRCHoneyReturnable,
                            SRCHoneySellable = item.SRCHoneySellable,
                            BrandId = item.BrandId,
                            CatId = item.CatId,
                            CreatedDate = item.CreatedDate,
                            UpdatedDate = item.UpdatedDate,
                            Price = item.Price,
                            StyleId = item.StyleId,
                            IsAddedToCart = item.CartData,
                            QuantityDisplay = item.Quantity.ToString()
                        };

                        popOrderCartListData.Add(popOrderUiModel);
                    }
                }
                return popOrderCartListData;

            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetFilteredPopOrderCartData", ex);
                return null;
            }
        }

        public async Task<List<OrderDetailUIModel>> GetRackCartProductsData(string CurrentDeviceOrderId)
        {
            var orderDetailsData = await DbService.GetRackCartProductsAsync(CurrentDeviceOrderId).ConfigureAwait(false);

            List<OrderDetailUIModel> cartItemDetailsList = new List<OrderDetailUIModel>();
            try
            {
                if (orderDetailsData != null && orderDetailsData.Count > 0)
                {
                    this._mapper = this._profileMatcher.GetMapper((IMapperConfigurationExpression cfg) => this._profileMatcher.CreateOrderDetailMapping(cfg));
                    cartItemDetailsList = this._mapper.Map<List<OrderDetail>, List<OrderDetailUIModel>>(orderDetailsData);
                }

                return cartItemDetailsList;
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetRackCartProductsData", ex);
                return null;
            }
        }

        public async Task<List<BrandUIModel>> GetFilteredComboboxData(int[] HierarchyTagArray, int HierarchyTag)
        {
            try
            {
                List<BrandUIModel> filteredBrandUIModelList;

                string hierarchyValues = string.Empty;

                if (HierarchyTag != 0)
                {
                    for (int i = 0; i < HierarchyTagArray.Length; i++)
                    {
                        if (HierarchyTagArray[i] != 0)
                        {
                            int index = i + 1;
                            if (HierarchyTag != index)
                            {
                                hierarchyValues = "and (Hierarchy" + index + "=" + HierarchyTagArray[i] + ") " + hierarchyValues;
                            }
                        }
                    }
                }

                var categoryList = await DbService.GetFilteredComboboxData(hierarchyValues, HierarchyTag);

                BrandData placeholderCategory = new BrandData();

                placeholderCategory.BrandName = "Select";
                categoryList?.Insert(0, placeholderCategory);
                filteredBrandUIModelList = categoryList?.Select(x => x.CopyToUIModel()).ToList();

                return filteredBrandUIModelList;
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetFilteredComboboxData", ex);
                return null;
            }
        }

        public async Task<List<ActivityForAllCustomerUIModel>> GetCallActivitiesOfAllCustomersForLoggedInUser(string territoryIds, bool loadAllData)
        {
            try
            {
                var _list = await DbService.GetCallActivitiesOfAllCustomersForLoggedInUser(territoryIds, loadAllData).ConfigureAwait(false);
                return _list;
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(QueryService), "GetCallActivitiesOfAllCustomersForLoggedInUser", ex.Message);
                return null;
            }
        }

        public async Task<List<ActivityForAllCustomerUIModel>> GetCallActivitiesOfAllCustomersForNationalAndZoneAndRegionManagers(string territoryIds, bool loadAllData)
        {
            var callActivitiesList = await DbService.GetCallActivityDataForNationalAndZoneAndRegionManagers(territoryIds, loadAllData).ConfigureAwait(false);
            var customersDataList = await DbService.GetCustomerDataAsyncForNationalAndZoneAndRegionManagers().ConfigureAwait(false);
            var usersList = await DbService.GetUserMasterData().ConfigureAwait(false);
            var distributorNumbersListForCustomers = await DbService.GetDistributorMastersListForNationalAndZoneAndRegionManagers().ConfigureAwait(false);
            var orderMasterIdsList = await DbService.GetOrderMastersIdListForNationalAndZoneAndRegionManagers().ConfigureAwait(false);
            var stateNameList = await DbService.GetAllStateMasterDataForNationalAndZoneAndRegionManagers().ConfigureAwait(false);

            try
            {//todo refactor query
                var tempCallActivitiesList = (from callActivityData in callActivitiesList
                                              join customerData in customersDataList on callActivityData.CustomerID equals customerData.DeviceCustomerID
                                              join userData in usersList on callActivityData.UserID equals userData.UserId into UserDataGroup
                                              from userData in UserDataGroup.DefaultIfEmpty()
                                              join orderIds in orderMasterIdsList on callActivityData.OrderID equals orderIds.DeviceOrderID into CallActivityOrdersData
                                              from callOrdersData in CallActivityOrdersData.DefaultIfEmpty()
                                              join customerDistributionData in distributorNumbersListForCustomers on callOrdersData?.CustomerDistributorID equals customerDistributionData.CustomerID into DistributorOrdersData
                                              from distrOrderData in DistributorOrdersData.DefaultIfEmpty()
                                              from states in stateNameList.Where(x => x.StateID == customerData.PhysicalAddressStateID).DefaultIfEmpty()
                                              select new ActivityForAllCustomerUIModel
                                              {
                                                  CallActivityID = callActivityData.CallActivityID ?? 0,
                                                  CallActivityDeviceID = callActivityData.CallActivityDeviceID,
                                                  IsVoided = callActivityData.IsVoided,
                                                  WholesaleInvoiceFilePath = callActivityData.WholesaleInvoiceFilePath,
                                                  CustomerID = callActivityData.CustomerID,
                                                  UserID = callActivityData.UserID,
                                                  ActivityType = callActivityData.ActivityType,
                                                  Objective = callActivityData.Objective,
                                                  Result = callActivityData.Result,
                                                  Comments = callActivityData.Comments,
                                                  CreatedDate = callActivityData.CreatedDate,
                                                  UpdateDate = callActivityData.UpdateDate,
                                                  CallDate = callActivityData.CallDate,
                                                  OrderID = callActivityData.OrderID,
                                                  SalesPerson = callActivityData.SalesPerson,
                                                  GratisProduct = callActivityData.GratisProduct,
                                                  //CallActivityID = (int)callActivityData.CallActivityID,
                                                  IsExported = callActivityData.IsExported,
                                                  isDeleted = callActivityData.isDeleted,
                                                  TerritoryID = callActivityData.TerritoryID,
                                                  TerritoryName = callActivityData.TerritoryName,
                                                  Hours = callActivityData.Hours,
                                                  GrandTotal = Convert.ToString(callActivityData.GrandTotal),
                                                  PhysicalAddressCityID = customerData.PhysicalAddressCityID,
                                                  PhysicalAddressStateID = Convert.ToString(customerData.PhysicalAddressStateID),
                                                  CustomerName = Convert.ToString(customerData.CustomerName),
                                                  CustomerNumber = Convert.ToString(customerData.CustomerNumber),
                                                  CustomerCreatedDate = customerData.CreatedDate,
                                                  UserNameFull = userData.FirstName + " " + userData.LastName,
                                                  UserName = userData.UserName,
                                                  DistributorNo = distrOrderData == null ? string.Empty : distrOrderData.CustomerNumber,
                                                  StateName = states == null ? string.Empty : states.StateName
                                              }).OrderByDescending(x => x.LastcallDate).ToList();
                return tempCallActivitiesList;
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(QueryService), "GetCallActivitiesOfAllCustomersForLoggedInNationalAndZMUser", ex.StackTrace);

                return null;
            }
            finally
            {
                callActivitiesList.Clear();
                customersDataList.Clear();
                distributorNumbersListForCustomers.Clear();
                orderMasterIdsList.Clear();
            }
        }

        public async Task<List<ActivityForAllCustomerUIModel>> GetCallActivitiesOfAllCustomers(bool loadAllData)
        {
            var callActivitiesList = await DbService.GetCallActivityDataForAVPManagers(loadAllData).ConfigureAwait(false);
            var customersDataList = await DbService.GetCustomerDataAsyncForNationalAndZoneAndRegionManagers().ConfigureAwait(false);
            var usersList = await DbService.GetUserMasterData().ConfigureAwait(false);
            var distributorNumbersListForCustomers = await DbService.GetDistributorMastersListForNationalAndZoneAndRegionManagers().ConfigureAwait(false);
            var orderMasterIdsList = await DbService.GetOrderMastersIdListForNationalAndZoneAndRegionManagers().ConfigureAwait(false);
            var stateNameList = await DbService.GetAllStateMasterDataForNationalAndZoneAndRegionManagers().ConfigureAwait(false);

            try
            {//todo refactor query
                var tempCallActivitiesList = (from callActivityData in callActivitiesList
                                              join customerData in customersDataList on callActivityData.CustomerID equals customerData.DeviceCustomerID
                                              join userData in usersList on callActivityData.UserID equals userData.UserId
                                              join orderIds in orderMasterIdsList on callActivityData.OrderID equals orderIds.DeviceOrderID into CallActivityOrdersData
                                              from callOrdersData in CallActivityOrdersData.DefaultIfEmpty()
                                              join customerDistributionData in distributorNumbersListForCustomers on callOrdersData?.CustomerDistributorID equals customerDistributionData.CustomerID into DistributorOrdersData
                                              from distrOrderData in DistributorOrdersData.DefaultIfEmpty()
                                              from states in stateNameList.Where(x => x.StateID == customerData.PhysicalAddressStateID).DefaultIfEmpty()
                                              select new ActivityForAllCustomerUIModel
                                              {
                                                  CallActivityID = callActivityData.CallActivityID ?? 0,
                                                  CallActivityDeviceID = callActivityData.CallActivityDeviceID,
                                                  IsVoided = callActivityData.IsVoided,
                                                  WholesaleInvoiceFilePath = callActivityData.WholesaleInvoiceFilePath,
                                                  CustomerID = callActivityData.CustomerID,
                                                  UserID = callActivityData.UserID,
                                                  ActivityType = callActivityData.ActivityType,
                                                  Objective = callActivityData.Objective,
                                                  Result = callActivityData.Result,
                                                  Comments = callActivityData.Comments,
                                                  CreatedDate = callActivityData.CreatedDate,
                                                  UpdateDate = callActivityData.UpdateDate,
                                                  CallDate = callActivityData.CallDate,
                                                  OrderID = callActivityData.OrderID,
                                                  SalesPerson = callActivityData.SalesPerson,
                                                  GratisProduct = callActivityData.GratisProduct,
                                                  //CallActivityID = (int)callActivityData.CallActivityID,
                                                  IsExported = callActivityData.IsExported,
                                                  isDeleted = callActivityData.isDeleted,
                                                  TerritoryID = callActivityData.TerritoryID,
                                                  TerritoryName = callActivityData.TerritoryName,
                                                  Hours = callActivityData.Hours,
                                                  GrandTotal = Convert.ToString(callActivityData.GrandTotal),
                                                  PhysicalAddressCityID = customerData.PhysicalAddressCityID,
                                                  PhysicalAddressStateID = Convert.ToString(customerData.PhysicalAddressStateID),
                                                  CustomerName = Convert.ToString(customerData.CustomerName),
                                                  CustomerNumber = Convert.ToString(customerData.CustomerNumber),
                                                  CustomerCreatedDate = customerData.CreatedDate,
                                                  UserNameFull = userData.FirstName + " " + userData.LastName,
                                                  UserName = userData.UserName,
                                                  DistributorNo = distrOrderData == null ? string.Empty : distrOrderData.CustomerNumber,
                                                  StateName = states == null ? string.Empty : states.StateName
                                              }).OrderByDescending(x => x.LastcallDate).ToList();
                return tempCallActivitiesList;
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(QueryService), "GetCallActivitiesOfAllCustomersForLoggedInNationalAndZMUser", ex.StackTrace);

                return null;
            }
            finally
            {
                callActivitiesList.Clear();
                customersDataList.Clear();
                distributorNumbersListForCustomers.Clear();
                orderMasterIdsList.Clear();
            }
        }

        public async Task<Dictionary<int, int>> GetCartsProductsIdQuantity(string currentOrderId)
        {
            try
            {
                var cartData = await DbService.GetCartsProductsIdQuantityAsync(currentOrderId);
                return cartData;

            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetCartsProductsIdQuantity", ex);
                return null;
            }
        }

        public async Task<List<ActivityForIndividualCustomerUIModel>> GetCallActivitiesOfSelectedCustomerForLoggedInUser(string deviceCustomerId)
        {
            try
            {
                var tableData = await DbService.GetCallActivitiesOfSelectedCustomerForLoggedInUser(deviceCustomerId).ConfigureAwait(false);

                return tableData;
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(QueryService), "GetCallActivitiesOfSelectedCustomerForLoggedInUser", ex.StackTrace);

                return null;
            }
        }
        public async Task<List<ActivityForIndividualCustomerUIModel>> GetCallActivitiesOfSelectedArea(string deviceCustomerId, int selectedArea, int roleId)
        {
            try
            {
                return await DbService.GetCallActivitiesOfSelectedArea(deviceCustomerId, selectedArea, roleId);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(QueryService), "GetCallActivitiesOfSelectedArea", ex);

                return null;
            }
        }
        public async Task<List<CustomerMaster>> GetCustomerListForActivity()
        {
            try
            {
                var tableData = await DbService.GetCustomerListForAddActivity();
                return tableData;
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(QueryService), "GetCustomerListForActivity", ex.StackTrace);
                return null;
            }
        }

        public async Task<IEnumerable<string>> GetUserActivityTypeAsync()
        {
            try
            {
                List<MasterTableTypeUIModel> userActivityTypes = await DbService.GetUserActivityTypeAsync().ConfigureAwait(false);
                if (userActivityTypes != null && userActivityTypes.Count > 0)
                    return userActivityTypes.Select(i => i.Name);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(QueryService), nameof(GetUserActivityTypeAsync), ex);
            }
            return null;
        }

        public async Task<IEnumerable<string>> GetCustomerActivityTypeAsync()
        {
            try
            {
                List<MasterTableTypeUIModel> customerActivityTypes = await DbService.GetCustomerActivityTypeAsync().ConfigureAwait(false);
                if (customerActivityTypes != null && customerActivityTypes.Count > 0)
                    return customerActivityTypes.Select(i => i.Name);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(QueryService), nameof(GetCustomerActivityTypeAsync), ex);
            }
            return null;
        }

        public async Task<IEnumerable<string>> GetDocumentTypeAsync()
        {
            try
            {
                List<MasterTableTypeUIModel> customerActivityTypes = await DbService.GetDocumentTypeAsync().ConfigureAwait(false);
                if (customerActivityTypes != null && customerActivityTypes.Count > 0)
                    return customerActivityTypes.Select(i => i.Name);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(QueryService), nameof(GetDocumentTypeAsync), ex);
            }
            return null;
        }

        public async Task<List<TerritoryMaster>> GetTerritoryFromIds(string territoryIdsInCvs)
        {
            try
            {
                var tableData = await DbService.GetTerritoryFromIds(territoryIdsInCvs).ConfigureAwait(false);
                return tableData;
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(QueryService), "GetTerritoryFromIds", ex.StackTrace);
                return null;
            }
        }

        public async Task<CallActivityList> AddCallActivityToDb(CallActivityList activity)
        {
            try
            {
                var tableData = await DbService.InsertOrUpdateCallActivityListDataAsync(activity).ConfigureAwait(false);
                return tableData;
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(QueryService), "AddCallActivityToDb", ex.StackTrace);
                return null;
            }
        }

        public async Task<bool> AddCallActivityCallDate(CallActivityList activity, string date)
        {
            try
            {
                return await DbService.AddCallDateToCallActivityList(activity, date);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(QueryService), nameof(AddCallActivityCallDate), ex.StackTrace);
                return false;
            }
        }

        public async Task<bool> AddLoggedInUserAsZeroCustomer(string userName, string pin)
        {
            try
            {
                var loggedInuser = await DbService.GetUserFromUserNameAndPin(userName, pin).ConfigureAwait(false);

                var customer = new CustomerMaster()
                {
                    CustomerID = 0,
                    DeviceCustomerID = "0-0",
                    CustomerName = loggedInuser?.FirstName + " " + loggedInuser?.LastName,
                    TerritoryID = loggedInuser?.TerritoryID,
                    IsExported = 2,
                };

                await DbService.InsertOrUpdateCustomerMasterDataAsync(customer).ConfigureAwait(false);

                return true;
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(QueryService), nameof(AddLoggedInUserAsZeroCustomer), ex.StackTrace);
                return false;
            }
        }

        public async Task<List<UserTaxStatementUiModel>> GetUserTaxStatementList()
        {
            try
            {
                var taxStatementList = await DbService.GetUserTaxStatementItemsAsync();
                return taxStatementList?.Select(x => x.CopyToUIModel()).ToList();
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetUserTaxStatementList", ex);
                return null;
            }
        }

        public async Task<UserTaxStatement> InsertUserTaxStatement(UserTaxStatement userTaxStatement)
        {
            try
            {
                var updatedUserTaxStatement = await DbService.InsertUserTaxStatementDataAsync(userTaxStatement);
                return updatedUserTaxStatement;
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "InsertUserTaxStatement", ex);
                return null;
            }
        }

        public async Task<UserTaxStatement> DeleteUserTaxStatement(UserTaxStatement userTaxStatement)
        {
            try
            {
                var updatedUserTaxStatement = await DbService.DeleteUserTaxStatementDataAsync(userTaxStatement);
                return updatedUserTaxStatement;
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "DeleteUserTaxStatement", ex);
                return null;
            }
        }

        public async Task<UserTaxStatement> UpdateUserTaxStatement(UserTaxStatement userTaxStatement)
        {
            try
            {
                var updatedUserTaxStatement = await DbService.UpdateUserTaxStatementDataAsync(userTaxStatement);
                return updatedUserTaxStatement;
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "UpdateUserTaxStatement", ex);
                return null;
            }
        }

        public async Task<OrderDetail> UpdateOrderDetail(OrderDetail orderDetail, string currentOrderId) => await DbService.UpdateOrderDetailDataAsync(orderDetail, currentOrderId).ConfigureAwait(false);

        public async Task<CallActivityList> GetIndidualActivityById(string activityId)
        {
            try
            {
                var activity = await DbService.GetIndidualActivityById(activityId).ConfigureAwait(false);
                return activity;
            }
            catch (Exception ex)
            {
                ErrorHandler.LogException(nameof(QueryService), nameof(GetIndidualActivityById), ex);
                return null;
            }
        }

        public async Task<OrderMaster> GetOrderFromOrderMasterFromDeviceOrderId(string deviceOrderId)
        {
            try
            {
                var order = await DbService.GetOrderFromOrderMasterFromDeviceOrderId(deviceOrderId).ConfigureAwait(false);
                return order;
            }
            catch (Exception ex)
            {
                ErrorHandler.LogException(nameof(QueryService), nameof(GetIndidualActivityById), ex);
                return null;
            }
        }

        public async Task<UserMaster> GetUserFromUserId(int userId)
        {
            try
            {
                var user = await DbService.GetLoggedInUserData(userId).ConfigureAwait(false);
                return user;
            }
            catch (Exception ex)
            {
                ErrorHandler.LogException(nameof(QueryService), nameof(GetIndidualActivityById), ex);
                return null;
            }
        }

        public async Task<List<OrderDetail>> GetOrderDetailsFromDeviceOrderId(string deviceOrderId)
        {
            try
            {
                var details = await DbService.GetOrderDetailsFromDeviceOrderId(deviceOrderId).ConfigureAwait(false);
                return details;
            }
            catch (Exception ex)
            {
                ErrorHandler.LogException(nameof(QueryService), nameof(GetOrderDetailsFromDeviceOrderId), ex);
                return null;
            }
        }

        //public async Task<List<OrderDetail>> GetOrderDetailsByDeviceOrderId(string deviceOrderId)
        //{
        //    try
        //    {
        //        var details = await DbService.GetOrderDetailsByDeviceOrderId(deviceOrderId).ConfigureAwait(false);
        //        return details;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorHandler.LogException(nameof(QueryService), nameof(GetOrderDetailsByDeviceOrderId), ex);
        //        return null;
        //    }
        //}

        public async Task<List<OrderHistoryDetailsGridUIModel>> GetOrderDetailsByDeviceOrderId(string deviceOrderId)
        {
            try
            {
                this._mapper = this._profileMatcher.GetMapper(
                   (IMapperConfigurationExpression cfg) => this._profileMatcher.CreateOrderHistoryDetailsGridMapping(cfg));
                var orderMaster = await DbService.GetOrderFromOrderMasterFromDeviceOrderId(deviceOrderId).ConfigureAwait(false);
                List<OrderDetail> orderDetails = await DbService.GetOrderDetailsByDeviceOrderId(deviceOrderId).ConfigureAwait(false);
                if (orderDetails != null && orderDetails.Count > 0)
                {
                    if (orderMaster.IsExported == 0)
                    {
                        orderDetails = orderDetails.OrderBy(x => DateTimeHelper.ConvertToDBDateTime(x.CreatedDate)).ToList();
                    }
                    else
                    {
                        orderDetails = orderDetails.OrderBy(x => x.OrderDetailId).ToList();
                    }
                }
                else orderDetails = null;
                return this._mapper.Map<List<OrderDetail>, List<OrderHistoryDetailsGridUIModel>>(orderDetails);
            }
            catch (Exception ex)
            {
                ErrorHandler.LogException(nameof(QueryService), nameof(GetOrderDetailsByDeviceOrderId), ex);
                return null;
            }
        }

        public async Task<IEnumerable<ProductMaster>> GetAllProductListFromProductMaster()
        {
            try
            {
                var products = await DbService.GetProductMasterDataForOrderHistory().ConfigureAwait(false);
                return products;
            }
            catch (Exception ex)
            {
                ErrorHandler.LogException(nameof(QueryService), nameof(GetAllProductListFromProductMaster), ex);
                return null;
            }
        }

        public async Task<IEnumerable<BrandData>> GetAllBrandDataFromBrandMaster()
        {
            try
            {
                var brands = await DbService.GetBrandDataForOrderHistory().ConfigureAwait(false);
                return brands;
            }
            catch (Exception ex)
            {
                ErrorHandler.LogException(nameof(QueryService), nameof(GetAllBrandDataFromBrandMaster), ex);
                return null;
            }
        }

        public async Task<IEnumerable<StyleData>> GetAllStylesFromStyleMaster()
        {
            try
            {
                var styles = await DbService.GetStyleDataForOrderHistory().ConfigureAwait(false);
                return styles;
            }
            catch (Exception ex)
            {
                ErrorHandler.LogException(nameof(QueryService), nameof(GetAllStylesFromStyleMaster), ex);
                return null;
            }
        }

        public async Task<IEnumerable<CategoryMaster>> GetAllCategoryFromCategoryMaster()
        {
            try
            {
                var categories = await DbService.GetCategoryDataForOrderHistory().ConfigureAwait(false);
                return categories;
            }
            catch (Exception ex)
            {
                ErrorHandler.LogException(nameof(QueryService), nameof(GetAllCategoryFromCategoryMaster), ex);
                return null;
            }
        }

        public async Task UpdateLastCallActivityDateForCustomerMaster(string deviceCustomerId, string updatedCallActivityDate)
        {
            try
            {
                await DbService.UpdateLastCallActivityDateForCustomerMaster(deviceCustomerId, updatedCallActivityDate).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorHandler.LogException(nameof(QueryService), nameof(UpdateLastCallActivityDateForCustomerMaster), ex);
            }
        }

        public async Task UpdateDateFromActivityCustomerMaster(string deviceCustomerId, string activityType, string updatedCallActivityDate)
        {
            try
            {
                await DbService.UpdateDateFromActivityCustomerMaster(deviceCustomerId, activityType, updatedCallActivityDate).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorHandler.LogException(nameof(QueryService), nameof(UpdateDateFromActivityCustomerMaster), ex);
            }
        }

        public async Task<string> GetOrderGrandTotalFromOrderDeviceId(string orderDeviceId)
        {
            try
            {
                var total = await DbService.GetOrderGrandTotalFromOrderDeviceId(orderDeviceId);
                return total;
            }
            catch (Exception ex)
            {
                ErrorHandler.LogException(nameof(QueryService), nameof(GetOrderGrandTotalFromOrderDeviceId), ex);
                return null;
            }

        }

        public async Task<IEnumerable<ZoneMasterUIModel>> GetZoneFromZoneId(int? id)
        {
            try
            {
                StringBuilder builder = new StringBuilder("SELECT ZoneMaster.ZoneID, ZoneMaster.ZoneName from ZoneMaster");
                builder.Append(id.HasValue ? string.Format(" WHERE ZoneMaster.ZoneID = {0}", id) : string.Empty);
                var _list = await DbService.GetZoneFromZoneId(builder.ToString());
                return _list.Select(x => x.CopyToUIModel());
            }
            catch (Exception ex)
            {
                ErrorHandler.LogException(nameof(QueryService), nameof(GetZoneFromZoneId), ex);
                return null;
            }
        }

        public async Task<IEnumerable<RegionMaster>> GetRegionFromRegionId(int id)
        {
            try
            {
                var _list = await DbService.GetRegionFromRegionId(id);
                return _list;
            }
            catch (Exception ex)
            {
                ErrorHandler.LogException(nameof(QueryService), nameof(GetRegionFromRegionId), ex);
                return null;
            }
        }

        public async Task<IEnumerable<TerritoryMaster>> GetTerritoryFromDefferedTerritoryId(int id)
        {
            try
            {
                var _list = await DbService.GetTerritoryFromDefferedTerritoryId(id);
                return _list;
            }
            catch (Exception ex)
            {
                ErrorHandler.LogException(nameof(QueryService), nameof(GetTerritoryFromDefferedTerritoryId), ex);
                return null;
            }
        }

        public async Task<ObservableCollection<CityMasterUIModel>> GetCityMasterDataWhichHasCustomersAssociated()
        {
            try
            {
                var _list = await DbService.GetCityMasterData();
                return new ObservableCollection<CityMasterUIModel>(_list.Select(x => x.CopyToUIModel()));
            }
            catch (Exception ex)
            {
                ErrorHandler.LogException(nameof(QueryService), nameof(GetCityMasterDataWhichHasCustomersAssociated), ex);
                return null;
            }
        }


        public async Task<IEnumerable<ZoneMasterUIModel>> GetZonesOnBasisOfCustomers()
        {
            try
            {
                var _list = await DbService.GetZonesOnBasisOfZoneIds("SELECT ZoneId,ZoneName from ZoneMaster WHERE ZoneId IN (Select DISTINCT ZoneId from CustomerMaster WHERE ZoneId<>0 AND IsAssociatedCustomer = 0) ORDER BY ZoneId");
                return _list.Select(x => x.CopyToUIModel());
            }
            catch (Exception ex)
            {
                ErrorHandler.LogException(nameof(QueryService), nameof(GetZonesOnBasisOfCustomers), ex);
                return null;
            }
        }


        public async Task<IEnumerable<RegionMasterUIModel>> GetRegionsOnBasisOfZoneIdsAndPresentCustomers(IEnumerable<int> zoneIds)
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder("SELECT DISTINCT RegionID,Regioname,ZoneID from RegionMaster ");
                stringBuilder.Append("WHERE RegionMaster.RegionID in (SELECT DISTINCT TerritoryMaster.RegionId from TerritoryMaster INNER JOIN CustomerMaster on CustomerMaster.TerritoryId = TerritoryMaster.TerritoryId Where CustomerMaster.IsAssociatedCustomer = 0 ) ");
                if (zoneIds != null)
                {
                    stringBuilder.Append(string.Format("and RegionMaster.ZoneId in ({0})", string.Join(",", zoneIds)));
                }
                stringBuilder.Append(" ORDER BY  RegionMaster.ZoneID,RegionMaster.RegionID ");
                var _list = await DbService.GetRegionsOnBasisOfZoneIds(stringBuilder.ToString());
                return _list.Select(x => x.CopyToUIModel());
            }
            catch (Exception ex)
            {
                ErrorHandler.LogException(nameof(QueryService), nameof(GetRegionsOnBasisOfZoneIdsAndPresentCustomers), ex);
                return null;
            }
        }


        public async Task<IEnumerable<TerritoryMasterUIModel>> GetTerritoryOnBasisOfRegionIdsAndPresentCustomers(IEnumerable<int> regionIds)
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder("SELECT DISTINCT TerritoryMaster.TerritoryID,TerritoryMaster.TerritoryName,TerritoryMaster.RegionID from TerritoryMaster ");
                stringBuilder.Append("INNER JOIN CustomerMaster on CustomerMaster.TerritoryId = TerritoryMaster.TerritoryId where TerritoryMaster.RegionID <> 0 ");

                if (regionIds != null)
                {
                    stringBuilder.Append(string.Format("and TerritoryMaster.RegionID in ({0})", string.Join(",", regionIds)));
                }
                stringBuilder.Append(" ORDER BY  TerritoryMaster.RegionID,TerritoryMaster.TerritoryID ");
                var _list = await DbService.GetTerritoryOnBasisOfRegionIds(stringBuilder.ToString());
                return _list.Select(x => x.CopyToUIModel());
            }
            catch (Exception ex)
            {
                ErrorHandler.LogException(nameof(QueryService), nameof(GetTerritoryOnBasisOfRegionIdsAndPresentCustomers), ex);
                return null;
            }
        }


        public async Task<IEnumerable<MapCustomerData>> GetMapDataForTradeTypeAndRankAndCallDate(int? zoneId, int? regionId, int? territoryId, string stateID, string cityName)
        {
            try
            {
                IEnumerable<CustomerMaster> customers = await DbService.GetCustomersInMapAsync(zoneId, regionId, territoryId, stateID, cityName).ConfigureAwait(false);
                IEnumerable<Classification> classifications = await DbService.GetClassificationInMapAsync().ConfigureAwait(false);

                var data = customers    // your starting point - table in the "from" statement
                            .Join(classifications, // the source table of the inner join
                            c => Convert.ToInt32(c.AccountClassification),        // Select the primary key (the first part of the "on" clause in an sql "join" statement)
                            cl => cl.AccountClassificationId,   // Select the foreign key (the second part of the "on" clause)
                            (c, cl) => new MapCustomerData
                            {
                                AccountClassificationName = cl.AccountClassificationName,
                                LastCallActivityDate = c.LastCallActivityDate,
                                PhysicalAddress = c.PhysicalAddress,
                                PhysicalAddressStateID = c.PhysicalAddressStateID.ToString(),
                                PhysicalAddressCityID = c.PhysicalAddressCityID,
                                PhysicalAddressZipCode = c.PhysicalAddressZipCode,
                                CustomerNumber = c.CustomerNumber,
                                CustomerName = c.CustomerName,
                                AccountClassification = c.AccountClassification,
                                Rank = c.Rank,
                                AccountType = c.AccountType.ToString(),
                                CustomerID = c.CustomerID ?? 0,
                                DeviceCustomerID = c.DeviceCustomerID,
                                Latitude = c.Latitude,
                                Longitude = c.Longitude
                            }); // selection

                return data.OrderByDescending(x => Convert.ToInt16(x.AccountClassification));
                //return data;
            }
            catch (Exception ex)
            {
                ErrorHandler.LogException(nameof(QueryService), nameof(GetMapDataForTradeTypeAndRankAndCallDate), ex);
                return null;
            }
        }

        //public async Task<IEnumerable<MapCustomerData>> GetMapDataForTradeTypeAndRankAndCallDate(int? zoneId, int? regionId, int? territoryId, string stateID, string cityName)
        //{
        //    try
        //    {
        //        StringBuilder builder = new StringBuilder();
        //        builder.Append("SELECT Classification.AccountClassificationName as " +
        //            "AccountClassificationName, CustomerMaster.LastCallActivityDate," +
        //            "CustomerMaster.PhysicalAddress,CustomerMaster.PhysicalAddressCityID," +
        //            "CustomerMaster.PhysicalAddressStateID," +
        //            "CustomerMaster.PhysicalAddressZipCode," +
        //             "CustomerMaster.CustomerNumber,CustomerMaster.CustomerName" +
        //            ",CustomerMaster.AccountClassification, CustomerMaster.Rank," +
        //            " CustomerMaster.AccountType, CustomerMaster.CustomerID," +
        //            "CustomerMaster.DeviceCustomerID, CustomerMaster.Latitude," +
        //            "CustomerMaster.Longitude from CustomerMaster, Classification WHERE ");

        //        builder.Append(zoneId.HasValue ? string.Format("CustomerMaster.ZoneId = {0} AND ", zoneId.Value) : string.Empty);
        //        builder.Append(regionId.HasValue ? string.Format("CustomerMaster.RegionId = {0} AND ", regionId.Value) : string.Empty);
        //        builder.Append(territoryId.HasValue ? string.Format("CustomerMaster.TerritoryId = '{0}' AND ", territoryId.Value) : string.Empty);
        //        builder.Append(!string.IsNullOrEmpty(stateID) ? string.Format("CustomerMaster.PhysicalAddressStateID = '{0}' AND ", stateID) : string.Empty);
        //        builder.Append(!string.IsNullOrEmpty(cityName) ? string.Format("CustomerMaster.PhysicalAddressCityID = '{0}' AND ", cityName) : string.Empty);
        //        builder.Append("CustomerMaster.DeviceCustomerID != '0-0' AND ");
        //        builder.Append("CustomerMaster.isDeleted != 1 AND ");
        //        builder.Append("CustomerMaster.AccountClassification= Classification.AccountClassificationId AND ");
        //        builder.Append("Classification.AccountClassificationId NOT in (3,7,20) ORDER BY Classification.AccountClassificationId DESC");

        //        var data = await DbService.GetMapDataForTradeTypeAndRankAndCallDate(builder.ToString()).ConfigureAwait(false);
        //        return data;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorHandler.LogException(nameof(QueryService), nameof(GetMapDataForTradeTypeAndRankAndCallDate), ex);
        //        return null;
        //    }
        //}

        private string SqlLiteDateTimeFormat(string colName)
        {
            return string.Format("datetime( substr({0},7,4)||'-'||substr({0},1,2)||'-'||substr({0},4,2)||' '||substr({0},12) )", colName);
        }
        private string SqlLiteDateFormat(string colName)
        {
            return string.Format("date( substr({0},7,4)||'-'||substr({0},1,2)||'-'||substr({0},4,2))", colName);
        }
        public async Task<IEnumerable<MapCustomerData>> GetMapDataForCashSales(int? zoneId, int? regionId, int? territoryId, string stateID, string cityName, DateTime? startDateTime, DateTime? endDateTime)
        {
            try
            {
                string dateFilter = "";
                if (startDateTime.HasValue && endDateTime.HasValue)
                {
                    string formattedStartDate = DateTimeHelper.ConvertToDbInsertDateTimeFormat(startDateTime.Value);
                    string formattedEndDate = DateTimeHelper.ConvertToDbInsertDateTimeFormat(endDateTime.Value);
                    dateFilter = string.Format(" AND ( {0} >= {1} AND {0} <= {2} ) ",
                  SqlLiteDateFormat("om.CreatedDate"),
                  SqlLiteDateFormat("'" + formattedStartDate + "'"),
                  SqlLiteDateFormat("'" + formattedEndDate + "'")
                  );
                }
                var stringBuilder = new StringBuilder();
                stringBuilder.Append(string.Format("select cm.customername,cm.customernumber,cm.devicecustomerid as DeviceCustomerID, SUM(IFNULL(om.GrandTotal,0)) as" +
                    " TotalAmount, IFNULL(om.SalesType,1) as SalesType, classifi.AccountClassificationName as" +
                    " AccountClassificationName, LastCallActivityDate, cm.AccountClassification as AccountClassification" +
                    ", Rank, AccountType, cm.customerid as CustomerID, Latitude, Longitude from " +
                    "CustomerMaster cm, Classification classifi left join OrderMaster om " +
                    "on om.devicecustomerid = cm.devicecustomerid and om.salestype = 1 {0} where ", dateFilter));
                stringBuilder.Append(zoneId.HasValue ? string.Format("(cm.ZoneId = {0}) AND ", zoneId.Value) : string.Empty);
                stringBuilder.Append(regionId.HasValue ? string.Format("(cm.RegionId = {0}) AND ", regionId.Value) : string.Empty);
                stringBuilder.Append(territoryId.HasValue ? string.Format("(cm.TerritoryId = '{0}') AND ", territoryId.Value) : string.Empty);
                stringBuilder.Append(!string.IsNullOrEmpty(stateID) ? string.Format("cm.PhysicalAddressStateID = '{0}' AND ", stateID) : string.Empty);
                stringBuilder.Append(!string.IsNullOrEmpty(cityName) ? string.Format("cm.PhysicalAddressCityID = '{0}' AND ", cityName) : string.Empty);
                stringBuilder.Append("cm.DeviceCustomerID != '0-0' AND ");
                stringBuilder.Append("cm.isDeleted != 1 AND ");
                stringBuilder.Append("cm.AccountClassification = classifi.AccountClassificationId AND cm.accounttype = 2 AND ");
                stringBuilder.Append("classifi.AccountClassificationId NOT IN(3, 7, 20) ");
                stringBuilder.Append("group by cm.devicecustomerid order by classifi.AccountClassificationId desc");

                var data = await DbService.GetMapCustomerDataForCashSales(stringBuilder.ToString()).ConfigureAwait(false);

                return data;
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(QueryService), "GetMapDataForCashSales", ex.StackTrace);
                return null;
            }
        }

        public async Task<IEnumerable<MapCustomerData>> GetMapDataForCashSalesForNationalAndZM(int? zoneId, int? regionId, int? territoryId, string stateID, string cityName, DateTime? startDateTime, DateTime? endDateTime)
        {
            try
            {
                string strTerritiryId = territoryId.HasValue ? territoryId.ToString() : "";
                var customersDataList = await DbService.GetCustomerDataCashSaleAsync(zoneId, regionId, strTerritiryId, stateID, cityName).ConfigureAwait(false);
                var orderDataList = await DbService.GetOrderMasterDataForCashSales(startDateTime, endDateTime).ConfigureAwait(false);
                var classftnDataList = await DbService.GetClassificationAsync().ConfigureAwait(false);

                var listmapbyCashsale = (from customermasterdata in customersDataList
                                         join ordermasterData in orderDataList on customermasterdata.DeviceCustomerID equals ordermasterData.DeviceCustomerID into customerorders
                                         join classfctnData in classftnDataList on customermasterdata.AccountClassification equals classfctnData.AccountClassificationId.ToString()
                                         from co in customerorders.DefaultIfEmpty(new OrderMaster { GrandTotal = "0", SalesType = "1" })
                                         orderby classfctnData.AccountClassificationId descending
                                         select new MapCustomerData
                                         {
                                             CustomerID = (int)customermasterdata.CustomerID,
                                             CustomerName = customermasterdata.CustomerName,
                                             CustomerNumber = customermasterdata.CustomerNumber,
                                             DeviceCustomerID = customermasterdata.DeviceCustomerID,
                                             TotalAmount = co.GrandTotal,
                                             SalesType = co.SalesType,
                                             AccountClassificationName = classfctnData.AccountClassificationName,
                                             LastCallActivityDate = customermasterdata.LastCallActivityDate,
                                             AccountClassification = customermasterdata.AccountClassification,
                                             Rank = customermasterdata.Rank,
                                             AccountType = customermasterdata.AccountType.ToString(),
                                             Latitude = customermasterdata.Latitude,
                                             Longitude = customermasterdata.Longitude,
                                         });

                return listmapbyCashsale;
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(QueryService), "GetMapDataForCashSalesForNationalAndZM", ex.StackTrace);
                return null;
            }
        }

        public async Task<IEnumerable<MapCustomerData>> GetMapsDataForItemsFilter(int? zoneId, int? regionId, int? territoryId, string stateID, string cityName, int productId)
        {
            try
            {
                var stringBuilder = new StringBuilder();

                stringBuilder.Append("select cm.devicecustomerid as DeviceCustomerID, classifi.AccountClassificationName as AccountClassificationName, " +
                    "cm.LastCallActivityDate, cm.AccountClassification as AccountClassification, cm.Rank, cm.AccountType,cm.customernumber,cm.customername,cm.customerid as CustomerID, cm.Latitude, " +
                    "cm.Longitude, cm.devicecustomerid in ({0}) as Sold from CustomerMaster cm, Classification classifi left join " +
                    "OrderMaster om on om.customerid = cm.customerid and om.DeviceCustomerID != '' where ");
                stringBuilder.Append(zoneId.HasValue ? string.Format("cm.ZoneId = {0} AND ", zoneId.Value) : string.Empty);
                stringBuilder.Append(regionId.HasValue ? string.Format("cm.RegionId = {0} AND ", regionId.Value) : string.Empty);
                stringBuilder.Append(territoryId.HasValue ? string.Format("cm.TerritoryId = '{0}' AND ", territoryId.Value) : string.Empty);
                stringBuilder.Append(!string.IsNullOrEmpty(stateID) ? string.Format("cm.PhysicalAddressStateID = '{0}' AND ", stateID) : string.Empty);
                stringBuilder.Append(!string.IsNullOrEmpty(cityName) ? string.Format("cm.PhysicalAddressCityID = '{0}' AND ", cityName) : string.Empty);
                stringBuilder.Append("cm.DeviceCustomerID != '0-0' AND ");
                stringBuilder.Append("cm.AccountClassification = classifi.AccountClassificationId AND cm.accounttype = 2 AND ");
                stringBuilder.Append("classifi.AccountClassificationId NOT IN(3, 7, 20) ");
                stringBuilder.Append("group by cm.devicecustomerid order by classifi.AccountClassificationId desc");

                var subQuery1 = string.Format("select DeviceOrderID from OrderDetail where ProductId = {0}", productId);

                var subQuery2 = string.Format("select DeviceCustomerID from OrderMaster where DeviceOrderID in ({0}) and DeviceCustomerID != ''", subQuery1);

                var mainQuery = string.Format(stringBuilder.ToString(), subQuery2);

                var data = await DbService.GetMapCustomerDataForItemFilter(mainQuery).ConfigureAwait(false);

                return data;
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(QueryService), "GetMapsDataForItemsFilter", ex.StackTrace);
                return null;
            }
        }

        public async Task<IEnumerable<MapCustomerData>> GetMapsDataForItemsFilter1(int? zoneId, int? regionId, int? territoryId, string stateID, string cityName, int productId)
        {
            try
            {
                string strTerritiryId = territoryId.HasValue ? territoryId.ToString() : "";

                var customersDataList = await DbService.GetCustomerDataForItemSearchAsync(zoneId, regionId, strTerritiryId, stateID, cityName);
                //var orderDataList = await DbService.GetOrderMasterDataForItemsFilter();
                var classftnDataList = await DbService.GetClassificationAsync();

                // List<OrderDetail> deviceOrderIDs = await DbService.GetListOfDeviceCustomerIdsFromOrderDetailsAndOrderMaster(productId);

                var DeviceCustomerIdsList = await DbService.GetListOfDeviceCustomerIdsFromOrderDetailsAndOrderMaster(productId);

                var customerAccountClassificationNameList = classftnDataList.Where(y => customersDataList.Any
                  (z => z.AccountClassification == y.AccountClassificationId.ToString())).Select(a => a.AccountClassificationName).ToList();


                List<MapCustomerData> lstMapCustData = new List<MapCustomerData>();

                var testData = from customerData in customersDataList
                               join classificationData in classftnDataList on customerData.AccountClassification
                               equals classificationData.AccountClassificationId.ToString()
                               where customerData.ZoneId == zoneId.Value
                               && customerData.RegionId == regionId.Value
                               && customerData.TerritoryID == territoryId.Value.ToString()
                               && customerData.AccountType == 2
                               && customerData.DeviceCustomerID != "0-0"
                               && classificationData.AccountClassificationId != 3
                               && classificationData.AccountClassificationId != 7
                               && classificationData.AccountClassificationId != 20
                               orderby classificationData.AccountClassificationId descending
                               select new MapCustomerData
                               {
                                   DeviceCustomerID = customerData.DeviceCustomerID,
                                   LastCallActivityDate = customerData.LastCallActivityDate,
                                   AccountClassification = customerData.AccountClassification,
                                   Rank = customerData.Rank,
                                   AccountType = customerData.AccountType.ToString(),
                                   CustomerID = (int)customerData.CustomerID,
                                   Latitude = customerData.Latitude,
                                   Longitude = customerData.Longitude,
                                   Sold = DeviceCustomerIdsList.Contains(customerData.DeviceCustomerID) ? 1 : 0
                               };

                lstMapCustData = testData.ToList();


                return lstMapCustData;

            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(QueryService), "GetMapsDataForItemsFilter1", ex.StackTrace);
                return null;
            }
        }

        public async Task<IEnumerable<ProductMaster>> GetProductDetailsOnBasisOfProductName(string name)
        {
            try
            {
                var query = string.Format("SELECT ProductID, ProductName, Description FROM ProductMaster WHERE UOM = 'CA' OR UOM = 'HID' AND CatId != '-99' AND ProductName like'%{0}%' order by ProductName Asc", name);

                var _products = await DbService.GetProductDetailsOnBasisOfProductName(query);

                return _products;
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(QueryService), "GetProductDetailsOnBasisOfProductName", ex.StackTrace);
                return null;
            }
        }

        public async Task<CustomerMaster> GetMapPopupCutomerData(int customerId, string customerDeviceId)
        {
            try
            {
                string query = string.Format("select Distinct AccountType,CustomerName,cm.DeviceCustomerID as" +
                        " DeviceCustomerID,cm.CustomerID as CustomerID,CustomerNumber," +
                        "PhysicalAddress,PhysicalAddressCityID,PhysicalAddressStateID," +
                        "PhysicalAddressZipCode,Phone,ManagerName,Latitude,Longitude,LastCallActivityDate from CustomerMaster " +
                        "cm where cm.CustomerID={0}  ",
                        customerId);
                if (!string.IsNullOrWhiteSpace(customerDeviceId))
                    query += string.Format(" and cm.DeviceCustomerID='{0}' ", customerDeviceId);

                var customer = await DbService.GetMapPopupCutomerData(query);
                return customer;
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(QueryService), nameof(GetMapPopupCutomerData), ex.StackTrace);
                return null;
            }
        }

        public async Task<IEnumerable<ActivityForAllCustomerUIModel>> GetMapPopupActivityData(string deviceCustomerId)
        {
            try
            {
                //// test =116630
                var query = string.Format("select CallActivityList.ActivityType," +
                    "CallActivityList.CallDate,(select TerritoryName from TerritoryMaster" +
                    " where  TerritoryID =CustomerMaster.TerritoryID) as TerritoryName from" +
                    " CallActivityList inner join CustomerMaster ON" +
                    " CustomerMaster.DeviceCustomerID = CallActivityList.CustomerID" +
                    " where CallActivityList.CustomerID = '{0}' and " +
                    "CallActivityList.IsExported!='2' and CallActivityList.isDeleted != '1' " +
                    "order by date(substr(CallActivityList.CallDate,7,4)||'-'||" +
                    "substr(CallActivityList.CallDate,1,2)||'-'||substr(CallActivityList.CallDate,4,2)||" +
                    "'T'||substr(CallActivityList.CallDate,12)) DESC limit 3 offset 0 ", deviceCustomerId);
                var activity = await DbService.GetMapPopupActivityData(query);
                return activity;
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(QueryService), nameof(GetMapPopupActivityData), ex.StackTrace);
                return null;
            }
        }

        public async Task<List<RouteListUIModel>> GetScheduledRouteListData()
        {
            try
            {
                List<RouteListUIModel> scheduledRouteList;

                scheduledRouteList = await DbService.GetRouteListData();

                return scheduledRouteList;
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(QueryService), nameof(GetScheduledRouteListData), ex.StackTrace);
                return null;
            }
        }

        public async Task<bool> UpdateRoutesForCurrentUser(int userId)
        {
            try
            {
                var result = await DbService.UpdateScheduleRoutesForCurrentUser(userId);

                return result;
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(QueryService), nameof(UpdateRoutesForCurrentUser), ex.StackTrace);
                return false;
            }
        }

        public async Task<CallActivityList> GetCurrentCustomerCallActivityDataAsync(string CustomerID, string OrderID)
        {
            try
            {
                var result = await DbService.GetCurrentCustomerCallActivityDataAsync(CustomerID, OrderID);

                return result;
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(QueryService), nameof(GetCurrentCustomerCallActivityDataAsync), ex.StackTrace);

                return null;
            }
        }

        public async Task UpdateCallActivityDataOnUpdateOrderFromHistory(CallActivityList callActivity)
        {
            await DbService.InsertOrUpdateCallActivityListDataAsync(callActivity);
        }

        public Task<ScheduledRoutes> AddRouteToDb(ScheduledRoutes route) => DbService.InsertOrUpdatetScheduledRouteDataAsync(route);


        public async Task AddRouteStationsToDb(List<int> customerIds, int routeId, string routeDeviceId, int userId)
        {
            try
            {
                foreach (var customerid in customerIds)
                {
                    await DbService.InsertOrUpdatetRouteStationDataAsync(new RouteStations
                    {
                        RouteStationId = null,
                        RouteId = routeId,
                        CustomerId = customerid,
                        DeviceRouteId = routeDeviceId,
                        UserId = userId
                    }).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(QueryService), nameof(AddRouteStationsToDb), ex);
            }
        }

        public async Task<IEnumerable<int>> GetCustomerIdsFromRouteStationTableByRouteId(int routeId)
        {
            try
            {
                var query = string.Format("SELECT RouteStations.CustomerId from RouteStations WHERE RouteStations.RouteId in ({0})", routeId);
                return await DbService.GetCustomerIdsFromRouteStationTableByRouteId(query);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(QueryService), nameof(GetCustomerIdsFromRouteStationTableByRouteId), ex.StackTrace);
                return null;
            }
        }

        public async Task<List<UserMaster>> GetListOfTerritoryManagersForRegion(int regionId)
        {
            try
            {
                var usersData = await DbService.GetListOfTerritoryManagersForRegion(regionId);

                return usersData;
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(QueryService), nameof(GetListOfTerritoryManagersForRegion), ex.StackTrace);
                return null;
            }
        }

        public async Task<List<UserMaster>> GetListOfTerritoryManagersForTerritories(string territoryIds) => await DbService.GetListOfTerritoryManagersForTerritories(territoryIds);

        public async Task<List<UserMaster>> GetListOfTerritoryManagersForZone(int zoneId)
        {
            try
            {
                var usersData = await DbService.GetListOfTerritoryManagersForZone(zoneId);

                return usersData;
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(QueryService), nameof(GetListOfTerritoryManagersForZone), ex.StackTrace);
                return null;
            }
        }

        public async Task<bool> DeleteAllRouteStationsForRouteId(int routeId)
        {
            try
            {
                var query = string.Format("DELETE from RouteStations WHERE RouteStations.RouteId in ({0})", routeId);
                return await DbService.DeleteAllRouteStationsForRouteId(query);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(QueryService), nameof(DeleteAllRouteStationsForRouteId), ex.StackTrace);
                return false;
            }
        }

        public async Task<bool> UpdateScheduleRoutesWhenRouteIdIs(string routeName, string routeBrief, string startDate, string endDate, int userId, int assignedToTSM, int routeId)
        {
            try
            {
                var query = string.Format("UPDATE ScheduledRoutes SET RouteName ='{0}', RouteBrief='{1}', StartDate='{2}', EndDate='{3}', isExported=0,  UpdatedBy='{4}', idAssignToTSM='{5}' WHERE RouteId={6}", routeName, routeBrief, startDate, endDate, userId, assignedToTSM, routeId);
                return await DbService.UpdateScheduleRoutesWhenRouteIdIs(query);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(QueryService), nameof(UpdateScheduleRoutesWhenRouteIdIs), ex.StackTrace);
                return false;
            }
        }

        public async Task<IEnumerable<MapPopAddToRouteListModel>> GetRoutesForMapPopup(int customerId)
        {
            try
            {
                /// string subQuery3 = string.Format("SELECT RouteStations.RouteId FROM RouteStations WHERE RouteStations.CustomerId='{0}'", customerId);
                /// string subQuery2 = "SELECT UserMaster.UserName, UserMaster.UserId FROM UserMaster";
                ///string subQuery1= "SELECT UserMaster.UserName,UserMaster.UserId , TerritoryMaster.TerritoryName FROM UserMaster,TerritoryMaster WHERE UserMaster.DefTerritoryId = TerritoryMaster.TerritoryId";
                var query = string.Format("select sr.*,um.username,um.TerritoryName,umcreator.username as CreatorName from ScheduledRoutes sr left join ( select username,userid,TerritoryName from usermaster,TerritoryMaster where usermaster.DefTerritoryId=TerritoryMaster.TerritoryID ) um on sr.idassigntotsm=um.userid left join ( select username,userid from usermaster ) umcreator on sr.userid=umcreator.userid where sr.IsExported !='2' and sr.IsDeleted!='1' and (idAssignToTSM=2 or idAssignToTSM=0) and sr.RouteId not in(select RouteId from RouteStations where customerid='{0}') order by RouteId desc", customerId);
                var result = await DbService.GetRoutesForMapPopup(query);
                return result;
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(QueryService), nameof(GetRoutesForMapPopup), ex.StackTrace);
                return null;
            }
        }

        public async Task<List<ViewRouteDetailsUIModel>> GetRouteDetailsToView(string DeviceRouteId)
        {
            try
            {
                var viewRouteDetails = await DbService.GetViewRouteDataAsync(DeviceRouteId);

                return viewRouteDetails;
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(QueryService), nameof(GetRouteDetailsToView), ex.StackTrace);
                return null;
            }
        }

        public async Task<bool> DeleteScheduledRoute(string DeviceRouteId)
        {
            try
            {
                var status = await DbService.DeleteScheduledRouteData(DeviceRouteId);

                return status;
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(QueryService), nameof(DeleteScheduledRoute), ex.StackTrace);
                return false;
            }
        }

        public async Task<Dictionary<int, string>> GetStateDictionaryWhichHasCustomersAssociated()
        {
            try
            {
                var query = "SELECT DISTINCT StateMaster.StateID, StateMaster.StateName from StateMaster INNER JOIN  CustomerMaster on CustomerMaster.PhysicalAddressStateID = StateMaster.StateID";
                var states = await DbService.GetStateDictionaryWhichHasCustomersAssociated(query);
                var dict = states.OrderBy(x => x.StateName).ToDictionary(x => x.StateID, y => y.StateName);
                return dict;

            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(QueryService), nameof(GetStateDictionaryWhichHasCustomersAssociated), ex.StackTrace);

                return null;
            }
        }

        public async Task UpdateOrderGrandTotal(string grandTotal, int orderId)
        {
            try
            {
                var query = string.Format("UPDATE OrderMaster SET GrandTotal ='{0}' WHERE OrderMaster.OrderId= {1}", grandTotal, orderId);

                await DbService.DbExecuteAsync(query).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(QueryService), nameof(UpdateOrderGrandTotal), ex.StackTrace + " - " + ex.Message);
            }
        }

        public async Task<bool> DeleteActivity(string deviceCallActivityId)
        {
            try
            {
                var query = string.Format("DELETE FROM CallActivityList WHERE CallActivityList.CallActivityDeviceID = '{0}'", deviceCallActivityId);

                var result = await DbService.DbExecuteAsync(query);

                return result;
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(QueryService), "DeleteActivity", ex.StackTrace + " - " + ex.Message);
                return false;
            }
        }

        public async Task<bool> DeleteActivityFromDeviceOrderId(string deviceOrderId)
        {
            try
            {
                var query = string.Format("DELETE FROM CallActivityList WHERE CallActivityList.OrderId = '{0}'", deviceOrderId);
                var result = await DbService.DbExecuteAsync(query).ConfigureAwait(false);
                return result;
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(QueryService), nameof(DeleteActivityFromDeviceOrderId), ex.StackTrace + " - " + ex.Message);
                return false;
            }
        }

        public async Task<bool> ClearCart(string deviceOrderId)
        {
            try
            {
                var query = string.Format("DELETE FROM OrderDetail WHERE OrderDetail.DeviceOrderID in ('{0}')", deviceOrderId);
                var result = await DbService.DbExecuteAsync(query).ConfigureAwait(false);
                if (result)
                {
                    query = string.Format("DELETE FROM OrderMaster WHERE OrderMaster.DeviceOrderID = '{0}' ", deviceOrderId);
                    result = await DbService.DbExecuteAsync(query).ConfigureAwait(false);
                }
                return result;
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(QueryService), nameof(ClearCart), ex);
                return false;
            }
        }

        public async Task<bool> DeleteOrderDetailsFromDeviceOrderId(string deviceOrderId)
        {
            try
            {
                var query = string.Format("DELETE FROM OrderDetail WHERE OrderDetail.DeviceOrderID in ('{0}')", deviceOrderId);
                var result = await DbService.DbExecuteAsync(query).ConfigureAwait(false);
                return result;
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(QueryService), nameof(DeleteOrderDetailsFromDeviceOrderId), ex.StackTrace + " - " + ex.Message);
                return false;
            }
        }

        public async Task<bool> DeleteOrderMasterDataFromOrderIdAndDeviceOrderId(int orderId, string deviceOrderId)
        {
            try
            {
                var query = string.Format("DELETE FROM OrderMaster WHERE OrderMaster.DeviceOrderID = '{0}' AND OrderMaster.OrderId={1}", deviceOrderId, orderId);
                var result = await DbService.DbExecuteAsync(query).ConfigureAwait(false);
                return result;
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(QueryService), nameof(DeleteOrderMasterDataFromOrderIdAndDeviceOrderId), ex.StackTrace + " - " + ex.Message);
                return false;
            }
        }

        //public async Task<List<ProductDistribution>> GetProductDistributionsDataForSelectedCustomer(int customerId, bool isDirectCustomer)
        //{
        //    var productDistributionsItems = await DbService.GetProductDistributionsDataForSelectedCustomer(customerId, isDirectCustomer);

        //    return productDistributionsItems;
        //}

        public async Task<List<ProductDistribution>> GetProductDistributionsDataForSelectedCustomer(int customerId, bool isDirectCustomer)
        {
            var productDistributionsItems = await DbService.GetProductDistributionsDataForSelectedCustomer(customerId, isDirectCustomer);

            return productDistributionsItems;
        }

        public async Task<bool> UpdateUserMaster(UserMaster user)
        {
            try
            {
                string query = string.Format("UPDATE UserMaster SET IsExported=0, PIN={0} WHERE UserMaster.UserId={1}", user.PIN, user.UserId);
                var isSuccess = await DbService.DbExecuteAsync(query).ConfigureAwait(false);
                return isSuccess;
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(QueryService), nameof(UpdateUserMaster), ex.StackTrace + " - " + ex.Message);
                return false;
            }
        }

        public async Task<Dictionary<string, string>> GetConfiguration()
        {
            Dictionary<string, string> result = null;
            try
            {
                result = await DbService.GetConfiguration().ConfigureAwait(false);

            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(QueryService), nameof(GetConfiguration), ex);
            }
            return result;
        }

        public async Task<string> GetConfigurationValueAsync(string _keyName)
        {
            try
            {
                return await DbService.GetConfigurationValueAsync(_keyName).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(QueryService), nameof(GetConfigurationValueAsync), ex);
            }
            return null;
        }


        public async Task<CustomerMaster> GetCartCustomerInfoAsync(int savedCustomerId) => await DbService.GetCartCustomerInfoAsync(savedCustomerId).ConfigureAwait(false);

        public async Task<FedExValidatedAddressResponse> CallFedExAddressValidationAPIAsync(string requestBody)
        {
            FedExValidatedAddressResponse result = null;
            try
            {
                string content = await InvokeWebService.PostFedExAPIServiceResponseAsync("address/v1/addresses/resolve", requestBody).ConfigureAwait(false);

                result = JsonConvert.DeserializeObject<FedExValidatedAddressResponse>(content);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(QueryService), nameof(CallFedExAddressValidationAPIAsync), ex);
            }
            return result;
        }

        public async Task<ICollection<TerritoryMaster>> GetBDTerritoriesAsync(int bdId)
        {
            try
            {
                return await DbService.GetBDTerritoriesAsync(bdId);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(QueryService), nameof(GetBDTerritoriesAsync), ex.StackTrace);
                return null;
            }
        }

        public async Task<ICollection<TerritoryMaster>> GetBDApproverTerritoriesAsync(int bdId, int regionId) => await DbService.GetBDApproverTerritoriesAsync(bdId, regionId);

        public async Task<ICollection<TerritoryMaster>> GetAVPTerritoriesAsync(int avpId)
        {
            try
            {
                return await DbService.GetAVPTerritoriesAsync(avpId);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(QueryService), nameof(GetAVPTerritoriesAsync), ex.StackTrace);
                return null;
            }
        }

        public async Task<int> GetRoleIdAsync(string roleName) => await DbService.GetRoleIdAsync(roleName);

        public async Task<string> GetUserFullNameAsync(string defTerritoryId) => await DbService.GetUserFullNameAsync(defTerritoryId);

        public async Task<string> GetTerritoriesBeforeSyncOfUserAsync(string userName, string pin)
        {
            UserMaster loggedInUser = await GetUserData(userName, pin);
            string loggedInUserOldTerritories = loggedInUser.TerritoryID;
            if (loggedInUser.RoleID == await GetRoleIdAsync(ApplicationConstants.BDRoleName)) // fetch territoriy ids for BD
            {
                var territories = await GetBDApproverTerritoriesAsync(loggedInUser.BDID, loggedInUser.RegionId);
                loggedInUserOldTerritories = string.Join(",", territories.Select(x => x.TerritoryID));
            }
            else if (loggedInUser.RoleID == await GetRoleIdAsync(ApplicationConstants.AVPRoleName)) // fetch territoriy ids for AVP
            {
                var territories = await GetAVPTerritoriesAsync(loggedInUser.AVPID);
                loggedInUserOldTerritories = string.Join(",", territories.Select(x => x.TerritoryID));
            }
            return loggedInUserOldTerritories;
        }
    }
}