using DRLMobile.Core.Helpers;
using DRLMobile.Core.Interface;
using DRLMobile.Core.Models.DataModels;
using DRLMobile.Core.Models.UIModels;
using DRLMobile.ExceptionHandler;

using SQLite;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using static SQLite.SQLite3;

namespace DRLMobile.Core.Services
{
    public class DatabaseService : IDatabaseService
    {
        SemaphoreSlim _semaphore = new SemaphoreSlim(1);


        /// <summary>
        /// get city master data in dictionary
        /// </summary>
        /// <returns></returns>
        public async Task<Dictionary<int, string>> GetCityDictionaryAsync()
        {
            Dictionary<int, string> stateDictionary = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                var table = await db.Table<CityMaster>().ToListAsync();
                stateDictionary = table.ToDictionary(key => key.CityID, val => val.CityName);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetCityDictionaryAsync), ex.Message);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return stateDictionary;
        }

        /// <summary>
        /// get classification table data in dictionary
        /// </summary>
        /// <returns></returns>
        //public async Task<Dictionary<int, Classification>> GetClassificationDictionaryAsync()
        //{
        //    try
        //    {
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);

        //        var table = await db.Table<Classification>().ToListAsync();

        //        var classificationDictionary = table.ToDictionary(key => key.AccountClassificationId);

        //        await db.CloseAsync();

        //        return classificationDictionary;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "GetClassificationDictionaryAsync", ex.Message);
        //        return null;
        //    }
        //}

        public async Task<Dictionary<int, Classification>> GetClassificationDictionaryAsync()
        {
            Dictionary<int, Classification> classificationDictionary = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                var table = await db.Table<Classification>().ToListAsync().ConfigureAwait(false);
                classificationDictionary = table.ToDictionary(key => key.AccountClassificationId);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "GetClassificationDictionaryAsync", ex.Message);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return classificationDictionary;
        }

        //public async Task<List<CustomerMaster>> GetCustomerDataAsync()
        //{
        //    try
        //    {
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);

        //        var customerMasterTabel = await db.Table<CustomerMaster>().Where(x => (x.isDeleted == 0 && x.IsExported != 2) || (x.AccountClassification.Equals("38") && x.IsExported != 2)).ToListAsync();

        //        await db.CloseAsync();

        //        return customerMasterTabel;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "GetCustomerDataAsync", ex.StackTrace);
        //        return null;
        //    }
        //}

        public async Task<List<CustomerMaster>> GetCustomerDataAsync()
        {
            List<CustomerMaster> customerMasterTable = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<CustomerMaster>().ConfigureAwait(false);
                customerMasterTable = await db.Table<CustomerMaster>().Where(x => (x.isDeleted == 0 && x.IsExported != 2) || (x.AccountClassification.Equals("38") && x.IsExported != 2)).ToListAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "GetCustomerDataAsync", ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return customerMasterTable;
        }

        public async Task<List<CustomerMaster>> GetCustomerDataForUploadAsync()
        {
            List<CustomerMaster> customerMasterTable = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                //customerMasterTable = await db.Table<CustomerMaster>().Where(x => (x.isDeleted == 0 && x.AccountClassification.Equals("38")) || (x.IsExported == 0 && x.IsExported != 2)).ToListAsync().ConfigureAwait(false);
                customerMasterTable = await db.Table<CustomerMaster>().Where(x => x.IsExported == 0).ToListAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetCustomerDataForUploadAsync", ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
                db = null;
            }
            return customerMasterTable;
        }

        public async Task<List<CustomerMaster>> GetCustomerDataAsyncForNationalAndZoneAndRegionManagers()
        {
            string query = "SELECT CustomerID, DeviceCustomerID, CustomerName, CustomerNumber, CreatedDate, PhysicalAddressCityID, PhysicalAddressStateID " +
                "FROM CustomerMaster WHERE isDeleted == '0' ";
            List<CustomerMaster> customerMasterTabel = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                customerMasterTabel = await db.QueryAsync<CustomerMaster>(query).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "GetCustomerDataAsyncForNationalAndZoneAndRegionManagers", ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return customerMasterTabel;
        }

        public async Task<List<StateMaster>> GetAllStateMasterDataForNationalAndZoneAndRegionManagers()
        {
            string queryString = "SELECT * from StateMaster";
            List<StateMaster> stateMasterTable = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                stateMasterTable = await db.QueryAsync<StateMaster>(queryString).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetAllStateMasterDataForNationalAndZoneAndRegionManagers", ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return stateMasterTable;
        }

        public async Task<Dictionary<int, string>> GetStateDictionaryAsync()
        {
            Dictionary<int, string> stateDictionary = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                string queryString = "SELECT * from StateMaster";

                var stateMasterTable = await db.QueryAsync<StateMaster>(queryString).ConfigureAwait(false);

                stateDictionary = stateMasterTable.OrderBy(x => x.StateName).ToDictionary(key => key.StateID, val => val.StateName);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetStateDictionaryAsync), ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return stateDictionary;
        }

        //public async Task<string> GetStateNameForStateIdAsync(int stateId)
        //{
        //    SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);
        //    var stateMasterData = await db.Table<StateMaster>().Where(x => x.StateID == stateId).FirstOrDefaultAsync();
        //    await db.CloseAsync();
        //    return stateMasterData?.StateName;
        //}
        public async Task<string> GetStateNameForStateIdAsync(int stateId)
        {
            string StateName = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                var stateMasterData = await db.Table<StateMaster>().Where(x => x.StateID == stateId).FirstOrDefaultAsync();
                StateName = stateMasterData?.StateName;
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetStateNameForStateIdAsync", ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return StateName;
        }

        //public async Task<List<ProductMaster>> GetProductsListDataAsync()
        //{
        //    string queryString = "SELECT ProductId, ProductName, Description, isDistributed, CatId, StyleId, BrandId, ProductType, UOM, isDeleted, SRCHoneySellable, SRCHoneyReturnable, SRCCanIOrder, isTobbaco, DistributionRecordedDate FROM ProductMaster WHERE CatId NOT IN(-88,-99, 10, 11) AND UOM == 'CA' AND isDeleted = 0 ORDER BY ProductName";

        //    SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);

        //    var productMasterData = await db.QueryAsync<ProductMaster>(queryString);

        //    await db.CloseAsync();

        //    var productsList = productMasterData.ToList();

        //    await db.CloseAsync();

        //    return productsList;

        //}
        //TO DO FavoriteProduct
        //public async Task<List<ProductMaster>> GetFavoriteProductsListDataAsync(string CurrentOrderId)
        //{
        //    List<ProductMaster> productMasterlList = null;
        //    SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH,
        //          SQLiteOpenFlags.ReadOnly | SQLiteOpenFlags.SharedCache, false);
        //    try
        //    {
        //        string queryString = String.Format("SELECT pm.ProductId, pm.ProductName, pm.Description, pm.isDistributed, pm.CatId, pm.StyleId, pm.BrandId, pm.ProductType, pm.UOM, pm.isDeleted, pm.SRCHoneySellable, pm.SRCHoneyReturnable, pm.SRCCanIOrder, pm.isTobbaco, pm.DistributionRecordedDate FROM ProductMaster pm LEFT JOIN Favorite as f ON pm.ProductId = f.ProductId AND f.isDeleted = 0  LEFT JOIN (SELECT ProductId, Quantity FROM OrderDetail WHERE {0} DeviceOrderID != '0') as od ON pm.ProductId = od.ProductId WHERE pm.CatId NOT IN(-88, -99, 10, 11) AND pm.UOM == 'CA' AND pm.isDeleted = 0 ORDER BY pm.ProductName", (string.IsNullOrWhiteSpace(CurrentOrderId)?"": "DeviceOrderID = '"+ CurrentOrderId + "' AND " ));

        //        queryString = "SELECT pm.ProductId, pm.ProductName, pm.Description, pm.isDistributed, pm.CatId, pm.StyleId, pm.BrandId, pm.ProductType, pm.UOM, pm.isDeleted, pm.SRCHoneySellable, pm.SRCHoneyReturnable, pm.SRCCanIOrder, pm.isTobbaco, pm.DistributionRecordedDate FROM ProductMaster pm LEFT JOIN Favorite as f ON pm.ProductId = f.ProductId AND f.isDeleted = 0  WHERE pm.CatId NOT IN(-88, -99, 10, 11) AND pm.UOM == 'CA' AND pm.isDeleted = 0 ORDER BY pm.ProductName";
        //        productMasterlList = await db.QueryAsync<ProductMaster>(queryString).ConfigureAwait(false);

        //        queryString = "SELECT pm.ProductId, pm.ProductName, pm.Description, pm.isDistributed, pm.CatId, pm.StyleId, pm.BrandId, pm.ProductType, pm.UOM, pm.isDeleted, pm.SRCHoneySellable, pm.SRCHoneyReturnable, pm.SRCCanIOrder, pm.isTobbaco, pm.DistributionRecordedDate FROM ProductMaster pm LEFT JOIN OrderDetail rd ON pm.ProductId = rd.ProductId WHERE pm.CatId NOT IN(-88, -99, 10, 11) AND pm.UOM == 'CA' AND pm.isDeleted = 0 ORDER BY pm.ProductName";
        //        productMasterlList = await db.QueryAsync<ProductMaster>(queryString).ConfigureAwait(false);
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetFavoriteProductsListDataAsync", ex);
        //    }
        //    finally
        //    {
        //        await db.CloseAsync().ConfigureAwait(false);
        //    }
        //    return productMasterlList;
        //}

        public async Task<List<ProductMaster>> GetProductsListDataAsync()
        {
            string queryString = "SELECT ProductId, ProductName, Description, isDistributed, CatId, StyleId, BrandId, ProductType, UOM, isDeleted, SRCHoneySellable, SRCHoneyReturnable, SRCCanIOrder, isTobbaco, DistributionRecordedDate FROM ProductMaster WHERE CatId NOT IN(-88,-99, 10, 11) AND UOM == 'CA' AND isDeleted = 0 ORDER BY ProductName";
            List<ProductMaster> productsList = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<ProductMaster>().ConfigureAwait(false);
                productsList = await db.QueryAsync<ProductMaster>(queryString).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetProductsListDataAsync", ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false); db = null;
            }
            return productsList;
        }

        public async Task<List<ProductMaster>> GetSRCProductsAsync()
        {
            string queryString = "SELECT ProductId, ProductName, Description, isDistributed, CatId, StyleId, BrandId, ProductType, UOM, isDeleted, SRCHoneySellable, SRCHoneyReturnable, SRCCanIOrder, isTobbaco, DistributionRecordedDate FROM ProductMaster WHERE UOM == 'CA' AND isDeleted = 0 ORDER BY ProductName";
            List<ProductMaster> productsList = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<ProductMaster>();
                productsList = await db.QueryAsync<ProductMaster>(queryString);
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetSRCProductsAsync", ex);
            }
            finally
            {
                await db.CloseAsync(); db = null;
            }
            return productsList;
        }

        public async Task<List<ProductMaster>> GetCatFilteredSRCProductData(IEnumerable<int> CatIds, IEnumerable<int> BrandIds, IEnumerable<int> StyleIds)
        {
            string subQueryCategory = "AND CatId IN ()";
            string subQueryBrand = "AND CatId IN ()";
            string subQueryStyle = "AND CatId IN ()";

            List<ProductMaster> productsList = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<ProductMaster>().ConfigureAwait(false);
                string queryString = "SELECT ProductId, ProductName, Description, isDistributed, CatId, StyleId, BrandId, ProductType, UOM, isDeleted, SRCHoneySellable, SRCHoneyReturnable, SRCCanIOrder, isTobbaco, DistributionRecordedDate FROM ProductMaster WHERE CatId NOT IN(-88,-99, 10, 11) AND UOM == 'CA' AND isDeleted = 0 ";

                if (CatIds.Any())
                {
                    subQueryCategory = $" AND CatId IN ({string.Join(",", CatIds)}) ";
                    queryString += subQueryCategory;
                }
                if (BrandIds.Any())
                {
                    subQueryBrand = $" AND BrandId IN ({string.Join(",", BrandIds)}) ";
                    queryString += subQueryBrand;
                }
                if (StyleIds.Any())
                {
                    subQueryStyle = $" AND StyleId IN ({string.Join(",", StyleIds)}) ";
                    queryString += subQueryStyle;
                }

                queryString += "  ORDER BY ProductName";
                productsList = await db.QueryAsync<ProductMaster>(queryString).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetCatFilteredSRCProductData", ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false); db = null;
            }
            return productsList;
        }


        //public async Task<List<ProductMaster>> GetProductsEligibleForCartListDataAsync()
        //{
        //    try
        //    {
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);
        //        //var productMasterData = await db.Table<ProductMaster>().Where(a => a.CatId != -88 && a.CatId != 10 && a.CatId != 11 && a.UOM == "CA"
        //        //&& a.CatId != -99 && a.IsDeleted == 0 && a.SRCHoneySellable != 0 && a.SRCHoneyReturnable != 0 && a.SRCCanIOrder != 0).OrderBy(x => x.ProductID).ToListAsync();
        //        var productMasterData = await db.Table<ProductMaster>().Where(a => a.CatId != -88 && a.CatId != 10 && a.CatId != 11 && a.UOM == "CA"
        //        && a.CatId != -99 && a.IsDeleted == 0).OrderBy(x => x.ProductID).ToListAsync();
        //        await db.CloseAsync();
        //        return productMasterData;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetProductsEligibleForCartListDataAsync), ex.StackTrace);
        //        return null;
        //    }
        //}

        public async Task<List<ProductMaster>> GetProductsEligibleForCartListDataAsync()
        {
            List<ProductMaster> productMasterData = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                productMasterData = await db.Table<ProductMaster>().Where(a => a.CatId != -88 && a.CatId != 10 && a.CatId != 11 && a.UOM == "CA"
                  && a.CatId != -99 && a.IsDeleted == 0).OrderBy(x => x.ProductID).ToListAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetProductsEligibleForCartListDataAsync", ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return productMasterData;
        }

        //public async Task<Dictionary<int, int>> GetFavoritesProductsIdsAsync()
        //{
        //    SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);
        //    var favoritesList = await db.Table<Favorite>().Where(x => x.isDeleted == false).ToListAsync();
        //    var favoritesProductIdsDictionary = favoritesList?.ToDictionary(i => i.ProductId, j => j.ProductId);
        //    await db.CloseAsync();
        //    return favoritesProductIdsDictionary;
        //}
        public async Task<Dictionary<int, int>> GetFavoritesProductsIdsAsync()
        {
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            Dictionary<int, int> favoritesProductIdsDictionary = null;
            try
            {
                await db.CreateTableAsync<Favorite>().ConfigureAwait(false);
                var favoritesList = await db.Table<Favorite>().Where(x => x.isDeleted == false)
                    .ToListAsync().ConfigureAwait(false);
                if (favoritesList != null && favoritesList.Any())
                {
                    favoritesProductIdsDictionary = new Dictionary<int, int>();
                    foreach (var item in favoritesList)
                    {
                        if (!favoritesProductIdsDictionary.ContainsKey(item.ProductId))
                            favoritesProductIdsDictionary.Add(item.ProductId, item.ProductId);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetFavoritesProductsIdsAsync), ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false); db = null;
            }
            return favoritesProductIdsDictionary;
        }
        public async Task<List<Favorite>> GetFavoriteProductDataAsync()
        {
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<Favorite>().ConfigureAwait(false);
                return await db.Table<Favorite>().Where(x => x.isDeleted == false)
                    .ToListAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetFavoriteProductDataAsync), ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false); db = null;
            }
            return null;
        }
        public async Task<Favorite> GetFavoriteDataByProductIdAsync(int productId)
        {
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<Favorite>().ConfigureAwait(false);
                return await db.Table<Favorite>().FirstOrDefaultAsync(x => x.ProductId == productId && x.isDeleted == false).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetFavoriteDataByProductIdAsync), ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false); db = null;
            }
            return null;
        }


        //public async Task<List<Favorite>> GetFavoriteProducts()
        //{
        //    SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);
        //    // commented by amol 
        //    // var favoriteMasterData = await db.Table<Favorite>().Where(x => x.isDeleted == 0).OrderBy(x => x.ProductId).ToListAsync();
        //    var favoriteMasterData = await db.Table<Favorite>().OrderBy(x => x.UserId).ToListAsync();
        //    await db.CloseAsync();
        //    return favoriteMasterData;
        //}

        public async Task<List<Favorite>> GetFavoriteProducts()
        {
            List<Favorite> favoriteMasterData = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                favoriteMasterData = await db.Table<Favorite>().OrderBy(x => x.UserId).ToListAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetFavoriteProducts", ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return favoriteMasterData;
        }

        public async Task<List<Favorite>> GetFavoriteProductsToUpload()
        {
            List<Favorite> favoriteMasterData = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<Favorite>().ConfigureAwait(false);
                favoriteMasterData = await db.Table<Favorite>()
                    .Where(x => x.IsExported == 0).ToListAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetFavoriteProductsToUpload", ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
                db = null;
            }
            return favoriteMasterData;
        }

        //public async Task<Dictionary<int, int>> GetCartsProductsIdsAsync(string currentOrderId)
        //{
        //    SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);
        //    var orderDataList = await db.Table<OrderDetail>().Where(o => o.DeviceOrderID == currentOrderId).ToListAsync();
        //    await db.CloseAsync();
        //    var cartsProductIdsDictionary = orderDataList?.ToDictionary(i => i.ProductId, j => j.ProductId);
        //    return cartsProductIdsDictionary;
        //}

        public async Task<Dictionary<int, int>> GetCartsProductsIdsAsync(string currentOrderId)
        {
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            Dictionary<int, int> cartsProductIdsDictionary = null;
            try
            {
                await db.CreateTableAsync<OrderDetail>().ConfigureAwait(false);
                var orderDataList = await db.Table<OrderDetail>()
                   .Where(o => o.DeviceOrderID == currentOrderId)
                   .ToListAsync().ConfigureAwait(false);
                if (orderDataList != null && orderDataList.Any())
                {
                    cartsProductIdsDictionary = new Dictionary<int, int>();
                    foreach (var item in orderDataList)
                    {
                        if (!cartsProductIdsDictionary.ContainsKey(item.ProductId))
                            cartsProductIdsDictionary.Add(item.ProductId, item.ProductId);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetCartsProductsIdsAsync), ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false); db = null;
            }
            return cartsProductIdsDictionary;
        }

        public async Task<int> GetCartProductQuantityAsync(string currentOrderId, int productId)
        {
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<OrderDetail>().ConfigureAwait(false);
                OrderDetail orderDetail = await db.Table<OrderDetail>()
                   .Where(o => o.DeviceOrderID == currentOrderId && o.ProductId == productId)
                   .FirstOrDefaultAsync().ConfigureAwait(false);
                if (orderDetail != null)
                {
                    return orderDetail.Quantity;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetCartProductQuantityAsync), ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false); db = null;
            }

            return 0;
        }

        public async Task<Dictionary<int, int>> GetCartsProductsIdQuantityAsync(string currentOrderId)
        {
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            Dictionary<int, int> cartsProductIdsDictionary = null;
            try
            {
                await db.CreateTableAsync<OrderDetail>().ConfigureAwait(false);
                var orderDataList = await db.Table<OrderDetail>()
                   .Where(o => o.DeviceOrderID == currentOrderId && !"0".Equals(o.DeviceOrderID))
                   .ToListAsync().ConfigureAwait(false);
                if (orderDataList != null && orderDataList.Any())
                {
                    cartsProductIdsDictionary = new Dictionary<int, int>();
                    foreach (var item in orderDataList)
                    {
                        if (!cartsProductIdsDictionary.ContainsKey(item.ProductId))
                            cartsProductIdsDictionary.Add(item.ProductId, item.Quantity);
                        else cartsProductIdsDictionary[item.ProductId] = item.Quantity;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetCartsProductsIdQuantityAsync), ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false); db = null;
            }

            return cartsProductIdsDictionary;
        }

        //public async Task<List<ProductDistribution>> GetProductDistributionDataAsync()
        //{
        //    try
        //    {
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);
        //        var productDistributionInfo = await db.Table<ProductDistribution>().Where(d => d.IsDeleted == 0).ToListAsync();
        //        await db.CloseAsync();
        //        return productDistributionInfo;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetProductDistributionDataAsync), ex.Message);
        //        return null;
        //    }
        //}

        public async Task<List<ProductDistribution>> GetProductDistributionDataAsync()
        {
            List<ProductDistribution> productDistributionInfo = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                productDistributionInfo = await db.Table<ProductDistribution>().Where(d => d.IsDeleted == 0).ToListAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetProductDistributionDataAsync", ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return productDistributionInfo;
        }

        public async Task<List<ProductDistribution>> GetProductDistributionDataToUploadAsync()
        {
            List<ProductDistribution> productDistributionInfo = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<ProductDistribution>().ConfigureAwait(false);
                productDistributionInfo = await db.Table<ProductDistribution>()
                    .Where(d => d.isExported == 0).ToListAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetProductDistributionDataToUploadAsync", ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false); db = null;
            }
            return productDistributionInfo;
        }

        public async Task<bool> DeleteProductDistrubutionDataAsync(ProductDistribution productDistribution)
        {
            bool success = false;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.Table<ProductDistribution>().DeleteAsync(a => a.CustomerProductID == productDistribution.CustomerProductID).ConfigureAwait(false);
                success = true;
            }
            catch (Exception ex)
            {
                success = false;
                ErrorHandler.LogAndThrowSpecifiedException(nameof(DatabaseService), nameof(DeleteProductDistrubutionDataAsync), ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }

            return success;
        }

        //public async Task DeleteFavoriteDataAsync()
        //{
        //    try
        //    {
        //        string deleteQuery = "DELETE FROM Favorite";

        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);

        //        await db.QueryAsync<Favorite>(deleteQuery);

        //        await db.CloseAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorHandler.LogAndThrowSpecifiedException(nameof(DatabaseService), nameof(DeleteFavoriteDataAsync), ex);
        //    }
        //}

        public async Task DeleteFavoriteDataAsync()
        {
            string deleteQuery = "DELETE FROM Favorite";

            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.QueryAsync<Favorite>(deleteQuery).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(DeleteFavoriteDataAsync), ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
        }

        //public async Task<CustomerMaster> GetSavedCustomerInfoAsync(int savedCustomerId)
        //{
        //    try
        //    {
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);

        //        var customerInfo = await db.Table<CustomerMaster>().Where(d => d.CustomerID == savedCustomerId).FirstOrDefaultAsync();

        //        await db.CloseAsync();

        //        return customerInfo;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorHandler.LogAndThrowSpecifiedException(nameof(DatabaseService), nameof(GetSavedCustomerInfoAsync), ex);

        //        return null;
        //    }

        //}
        public async Task<CustomerMaster> GetSavedCustomerInfoAsync(int savedCustomerId)
        {
            CustomerMaster customerInfo = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<CustomerMaster>().ConfigureAwait(false);
                customerInfo = await db.Table<CustomerMaster>().FirstOrDefaultAsync(d => d.CustomerID == savedCustomerId).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetSavedCustomerInfoAsync), ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false); db = null;
            }
            return customerInfo;
        }
        public async Task<CustomerMaster> GetCartCustomerInfoAsync(int savedCustomerId)
        {
            CustomerMaster customerInfo = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<CustomerMaster>().ConfigureAwait(false);
                string query = string.Format("Select CustomerId,CustomerName, CustomerNumber,AccountType From CustomerMaster Where CustomerID={0}", savedCustomerId);
                customerInfo = await db.FindWithQueryAsync<CustomerMaster>(query);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetSavedCustomerInfoAsync), ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false); db = null;
            }
            return customerInfo;
        }

        #region Methods to Insert/Update Database Tables

        public async Task<bool> InsertOrUpdateBrandDataAsync(List<BrandData> brandDataModel)
        {
            int success = 0;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<BrandData>();
                foreach (var brandItem in brandDataModel)
                    success = await db.InsertOrReplaceAsync(brandItem, typeof(BrandData));
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(InsertOrUpdateBrandDataAsync), ex);
            }
            finally
            {
                await db.CloseAsync(); db = null;
            }
            return success == 1;
        }


        public async Task<bool> IsCallActivityIdUniqueAsync(int callActivityID)
        {
            CallActivityList existingRecord = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<CallActivityList>();
                existingRecord = await db.Table<CallActivityList>()
                    .FirstOrDefaultAsync(a => a.CallActivityID == callActivityID);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "IsCallActivityIdUniqueAsync", ex);
            }
            finally
            {
                await db.CloseAsync(); db = null;
            }
            return (existingRecord == null);
        }


        public async Task<CallActivityList> InsertOrUpdateCallActivityListDataAsync(CallActivityList callActivityData)
        {
            int success = 0;
            CallActivityList existingRecord = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<CallActivityList>();
                existingRecord = await db.Table<CallActivityList>()
                    .FirstOrDefaultAsync(a => a.CallActivityID == callActivityData.CallActivityID);
                if (existingRecord != null)
                {
                    success = await db.UpdateAsync(callActivityData);
                    await InfoLogger.GetInstance.WriteToLogAsync(SourceName: $"{nameof(DatabaseService)}:{nameof(InsertOrUpdateCallActivityListDataAsync)}"
                       , SourceArgument: callActivityData
                       , CustomeMessage: "To Update Table CallActivity");
                }
                else
                {
                    success = await db.InsertAsync(callActivityData);
                    await InfoLogger.GetInstance.WriteToLogAsync(SourceName: $"{nameof(DatabaseService)}:{nameof(InsertOrUpdateCallActivityListDataAsync)}"
                       , SourceArgument: callActivityData
                       , CustomeMessage: "To Insert Table CallActivity");
                }
            }
            catch (Exception ex)
            {
                success = 0;
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "InsertOrUpdateCallActivityListDataAsync", ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync(); db = null;
            }
            return success == 1 ? callActivityData : null;
        }

        public async Task<CallActivityList> InsertOrUpdateCallActivityInRetailTranscationDataAsync(CallActivityList callActivityData)
        {
            int success = 0;
            CallActivityList existingRecord = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<CallActivityList>().ConfigureAwait(false);

                existingRecord = await db.Table<CallActivityList>()
                    .FirstOrDefaultAsync(a => a.OrderID == callActivityData.OrderID).ConfigureAwait(false);
                if (existingRecord != null)
                {
                    success = await db.UpdateAsync(callActivityData).ConfigureAwait(false);
                    await InfoLogger.GetInstance.WriteToLogAsync(SourceName: $"{nameof(DatabaseService)}:{nameof(InsertOrUpdateCallActivityInRetailTranscationDataAsync)}"
                        , SourceArgument: callActivityData
                        , CustomeMessage: "To Update Table CallActivity").ConfigureAwait(false);
                }
                else
                {
                    success = await db.InsertAsync(callActivityData).ConfigureAwait(false);
                    await InfoLogger.GetInstance.WriteToLogAsync(SourceName: $"{nameof(DatabaseService)}:{nameof(InsertOrUpdateCallActivityInRetailTranscationDataAsync)}"
                        , SourceArgument: callActivityData
                        , CustomeMessage: "To Insert Table CallActivity").ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                success = 0;
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "InsertOrUpdateCallActivityInRetailTranscationDataAsync", ex);

                await InfoLogger.GetInstance.WriteToLogAsync(SourceName: $"{nameof(DatabaseService)}:{nameof(InsertOrUpdateCallActivityInRetailTranscationDataAsync)}"
                  , SourceArgument: callActivityData
                  , CustomeMessage: ex.Message ?? "Failed").ConfigureAwait(false);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false); db = null;
            }
            return success == 1 ? callActivityData : null;
        }

        public async Task<CallActivityList> InsertOrUpdateDownloadedCallActivityListDataAsync(CallActivityList callActivityData)
        {
            int success = 0;
            CallActivityList existingRecord = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<CallActivityList>();
                existingRecord = await db.Table<CallActivityList>()
                    .FirstOrDefaultAsync(a => a.CallActivityDeviceID == callActivityData.CallActivityDeviceID);
                if (existingRecord != null)
                    await db.DeleteAsync(existingRecord);
                callActivityData.IsExported = 1;
                success = await db.InsertAsync(callActivityData);
            }
            catch (Exception ex)
            {
                success = 0;
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "InsertOrUpdateDownloadedCallActivityListDataAsync", ex);
            }
            finally
            {
                await db.CloseAsync(); db = null;
            }
            return success == 1 ? callActivityData : null;
        }

        public async Task BulkInsertOrUpdateDownloadedCallActivityListDataAsync(List<CallActivityList> callActivityCollection)
        {
            //CallActivityList existingRecord = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache, false);
            //await db.EnableWriteAheadLoggingAsync().ConfigureAwait(false);
            try
            {
                await db.CreateTableAsync<CallActivityList>().ConfigureAwait(false);
                foreach (CallActivityList callActivityData in callActivityCollection)
                {
                    try
                    {
                        //await _semaphore.WaitAsync().ConfigureAwait(false);

                        //existingRecord = await db.Table<CallActivityList>()
                        //    .FirstOrDefaultAsync(a => a.CallActivityDeviceID == callActivityData.CallActivityDeviceID);
                        //if (existingRecord != null)
                        //{
                        //    //await db.DeleteAsync(existingRecord);
                        //    await db.Table<CallActivityList>().DeleteAsync(a => a.CallActivityDeviceID == existingRecord.CallActivityDeviceID).ConfigureAwait(false);
                        //}
                        await db.Table<CallActivityList>().DeleteAsync(a => a.CallActivityDeviceID == callActivityData.CallActivityDeviceID).ConfigureAwait(false);
                        callActivityData.IsExported = 1;
                        await db.InsertAsync(callActivityData).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "BulkInsertOrUpdateDownloadedCallActivityListDataAsync InnerLoop", ex, callActivityData);
                    }
                    finally
                    {
                        //_semaphore.Release();
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "BulkInsertOrUpdateDownloadedCallActivityListDataAsync", ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
        }

        //public async Task BulkInsertOrUpdateDownloadedCallActivityListDataAsync(List<CallActivityList> callActivities)
        //{
        //    if (callActivities != null)
        //    {
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache, false);
        //        SemaphoreSlim _semaphore = new SemaphoreSlim(1);
        //        try
        //        {
        //            await db.CreateTableAsync<CallActivityList>().ConfigureAwait(false);
        //            List<Task> tasks = new List<Task>(callActivities.Count);
        //            foreach (CallActivityList item in callActivities)
        //            {
        //                tasks.Add(Task.Run(async () =>
        //                {
        //                    await _semaphore.WaitAsync().ConfigureAwait(false);
        //                    try
        //                    {
        //                        await db.Table<CallActivityList>().Where(a => a.CallActivityDeviceID == item.CallActivityDeviceID).DeleteAsync();
        //                        item.IsExported = 1;
        //                        await db.InsertAsync(item);
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "BulkInsertOrUpdateDownloadedCallActivityListDataAsync >> Inner Loop >>", ex, item);
        //                    }
        //                    finally
        //                    {
        //                        _semaphore.Release();
        //                    }
        //                }));
        //            }
        //            await Task.WhenAll(tasks);
        //        }
        //        catch (AggregateException ae)
        //        {
        //            ae.Handle((ex) =>
        //            {
        //                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "BulkInsertOrUpdateDownloadedCallActivityListDataAsync", ex); return true;
        //            });
        //        }
        //        catch (Exception ex)
        //        {
        //            ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "BulkInsertOrUpdateDownloadedCallActivityListDataAsync", ex);
        //        }
        //        finally
        //        {
        //            _semaphore.Dispose();
        //            await db.CloseAsync().ConfigureAwait(false); db = null;
        //        }
        //    }
        //}


        //public async Task<bool> InsertOrUpdateCartAndFavDataAsync(List<CartAndFav> cartAndFavs)
        //{
        //    int success = 0;

        //    try
        //    {
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);

        //        foreach (var cartAndFavItem in cartAndFavs)
        //        {
        //            CartAndFav existingRecord = cartAndFavItem;

        //            if (existingRecord != null)
        //            {
        //                success = await db.UpdateAsync(existingRecord);
        //            }
        //        }

        //        await db.CloseAsync();

        //        return success == 1;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(InsertOrUpdateCartAndFavDataAsync), ex.StackTrace);
        //        return false;
        //    }
        //}

        public async Task RunInTransactionUserActivityTypeAsync(List<UserActivityType> userActivityTypeCollection)
        {
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<UserActivityType>().ConfigureAwait(false);

                await db.RunInTransactionAsync((SQLiteConnection sQLiteConnection) =>
                  {
                      foreach (UserActivityType item in userActivityTypeCollection)
                      {
                          sQLiteConnection.InsertOrReplace(item);
                      }
                  }).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(RunInTransactionUserActivityTypeAsync), ex);
                await db.CloseAsync().ConfigureAwait(false);
                await InsertAllUserActivityTypeAsync(userActivityTypeCollection).ConfigureAwait(false);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
        }

        private async Task InsertAllUserActivityTypeAsync(List<UserActivityType> userActivityTypeCollection)
        {
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<UserActivityType>().ConfigureAwait(false);

                foreach (UserActivityType item in userActivityTypeCollection)
                {
                    try
                    {
                        await db.Table<UserActivityType>().DeleteAsync(a => a.Id == item.Id).ConfigureAwait(false);
                        await db.InsertAsync(item).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "InsertAllUserActivityTypeAsync Loop", ex, item);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(InsertAllUserActivityTypeAsync), ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
        }

        public async Task RunInTransactionCustomerActivityTypeAsync(List<CustomerActivityType> customerActivityTypeCollection)
        {
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<CustomerActivityType>().ConfigureAwait(false);

                await db.RunInTransactionAsync((SQLiteConnection sQLiteConnection) =>
                {
                    foreach (CustomerActivityType item in customerActivityTypeCollection)
                    {
                        sQLiteConnection.InsertOrReplace(item);
                    }
                }).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(RunInTransactionCustomerActivityTypeAsync), ex);
                await db.CloseAsync().ConfigureAwait(false);
                await InsertAllCustomerActivityTypeAsync(customerActivityTypeCollection).ConfigureAwait(false);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
        }

        private async Task InsertAllCustomerActivityTypeAsync(List<CustomerActivityType> customerActivityTypeCollection)
        {
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<CustomerActivityType>().ConfigureAwait(false);

                foreach (CustomerActivityType item in customerActivityTypeCollection)
                {
                    try
                    {
                        await db.Table<CustomerActivityType>().DeleteAsync(a => a.Id == item.Id).ConfigureAwait(false);
                        await db.InsertAsync(item).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "InsertAllCustomerActivityTypeAsync Loop", ex, item);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(InsertAllCustomerActivityTypeAsync), ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
        }

        public async Task RunInTransactionDocumentTypeAsync(List<CustomerDocumentType> documentTypeCollection)
        {
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<CustomerDocumentType>().ConfigureAwait(false);

                await db.RunInTransactionAsync((SQLiteConnection sQLiteConnection) =>
                {
                    foreach (CustomerDocumentType item in documentTypeCollection)
                    {
                        sQLiteConnection.InsertOrReplace(item);
                    }
                }).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(RunInTransactionDocumentTypeAsync), ex);
                await db.CloseAsync().ConfigureAwait(false);
                await InsertAllDocumentTypeTypeAsync(documentTypeCollection).ConfigureAwait(false);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
        }

        private async Task InsertAllDocumentTypeTypeAsync(List<CustomerDocumentType> documentTypeCollection)
        {
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<CustomerDocumentType>().ConfigureAwait(false);

                foreach (CustomerDocumentType item in documentTypeCollection)
                {
                    try
                    {
                        await db.Table<CustomerDocumentType>().DeleteAsync(a => a.Id == item.Id).ConfigureAwait(false);
                        await db.InsertAsync(item).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "InsertAllDocumentTypeAsync Loop", ex, item);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(InsertAllDocumentTypeTypeAsync), ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
        }

        public async Task<bool> InsertOrUpdateCartAndFavDataAsync(List<CartAndFav> cartAndFavs)
        {
            int success = 0;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                foreach (var cartAndFavItem in cartAndFavs)
                {
                    CartAndFav existingRecord = cartAndFavItem;

                    if (existingRecord != null)
                    {
                        success = await db.UpdateAsync(existingRecord).ConfigureAwait(false);
                    }
                }
            }
            catch (Exception ex)
            {
                success = 0;
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(InsertOrUpdateCartAndFavDataAsync), ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return success == 1;
        }

        //public async Task<bool> InsertOrUpdateCategoryMasterDataAsync(List<CategoryMaster> categoryMasterList)
        //{
        //    int success = 0;

        //    CategoryMaster existingRecord = null;

        //    try
        //    {
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);

        //        foreach (var categoryItem in categoryMasterList)
        //        {
        //            existingRecord = await (db.Table<CategoryMaster>().Where(a => a.CategoryID == categoryItem.CategoryID)).FirstOrDefaultAsync();

        //            if (existingRecord != null)
        //            {
        //                success = await db.UpdateAsync(categoryItem);
        //            }
        //            else
        //            {
        //                success = await db.InsertAsync(categoryItem);
        //            }
        //        }

        //        await db.CloseAsync();

        //        return success == 1;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(InsertOrUpdateCategoryMasterDataAsync), ex.StackTrace);
        //        return false;
        //    }
        //}

        public async Task<bool> InsertOrUpdateCategoryMasterDataAsync(List<CategoryMaster> categoryMasterList)
        {
            int success = 0;
            CategoryMaster existingRecord = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                foreach (var categoryItem in categoryMasterList)
                {
                    existingRecord = await (db.Table<CategoryMaster>().FirstOrDefaultAsync(a => a.CategoryID == categoryItem.CategoryID)).ConfigureAwait(false);

                    if (existingRecord != null)
                    {
                        success = await db.UpdateAsync(categoryItem).ConfigureAwait(false);
                    }
                    else
                    {
                        success = await db.InsertAsync(categoryItem).ConfigureAwait(false);
                    }
                }
            }
            catch (Exception ex)
            {
                success = 0;
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(InsertOrUpdateCategoryMasterDataAsync), ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }

            return success == 1;
        }

        public async Task<bool> InsertOrUpdateCityMasterDataAsync(List<CityMaster> cityMasterList)
        {
            int success = 0;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<CityMaster>();
                foreach (var cityItem in cityMasterList)
                    success = await db.InsertOrReplaceAsync(cityItem);
            }
            catch (Exception ex)
            {
                success = 0;
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(InsertOrUpdateCityMasterDataAsync), ex);
            }
            finally
            {
                await db.CloseAsync(); db = null;
            }
            return success != 0;
        }

        public async Task<bool> InsertOrUpdateContractMasterDataAsync(List<ContractMaster> contractMasterList)
        {
            int success = 0;
            ContractMaster existingRecord = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                foreach (var contractItem in contractMasterList)
                {
                    existingRecord = await (db.Table<ContractMaster>().FirstOrDefaultAsync(a => a.ContractID == contractItem.ContractID)).ConfigureAwait(false);

                    if (existingRecord != null)
                    {
                        success = await db.UpdateAsync(contractItem).ConfigureAwait(false);
                    }
                    else
                    {
                        success = await db.InsertAsync(contractItem).ConfigureAwait(false);
                    }
                }
            }
            catch (Exception ex)
            {
                success = 0;
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(InsertOrUpdateContractMasterDataAsync), ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return success == 1;
        }

        //public async Task<CustomerMaster> InsertOrUpdateCustomerMasterDataAsync(CustomerMaster customer)
        //{
        //    int success = 0;

        //    CustomerMaster existingRecord = null;

        //    try
        //    {
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);

        //        existingRecord = await db.Table<CustomerMaster>().Where(a => a.CustomerID == customer.CustomerID).FirstOrDefaultAsync();

        //        if (existingRecord != null)
        //        {
        //            success = await db.UpdateAsync(customer);
        //        }
        //        else
        //        {
        //            success = await db.InsertAsync(customer);
        //        }
        //        await db.CloseAsync();

        //        return success == 1 ? customer : null;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "InsertOrUpdateCustomerMasterDataAsync", ex.StackTrace);
        //        return null;
        //    }
        //}

        public async Task<CustomerMaster> InsertOrUpdateCustomerMasterDataAsync(CustomerMaster customer)
        {
            int success = 0;
            CustomerMaster existingRecord = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<CustomerMaster>();

                existingRecord = await db.Table<CustomerMaster>().FirstOrDefaultAsync(x => x.CustomerID == customer.CustomerID);
                if (existingRecord != null)
                {
                    success = await db.UpdateAsync(customer);
                }
                else
                {
                    success = await db.InsertAsync(customer);
                }
            }
            catch (Exception ex)
            {
                success = 0;
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "InsertOrUpdateCustomerMasterDataAsync", ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }

            return success == 1 ? customer : null;
        }

        public async Task<CustomerMaster> InsertOrUpdateDownloadedCustomerMasterAsync(CustomerMaster customer)
        {
            int success = 0;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<CustomerMaster>();
                await db.Table<CustomerMaster>().Where(a => a.DeviceCustomerID == customer.DeviceCustomerID).DeleteAsync();
                customer.IsExported = 1;
                success = await db.InsertAsync(customer);
            }
            catch (Exception ex)
            {
                success = 0;
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "InsertOrUpdateDownloadedCustomerMasterAsync", ex);
            }
            finally
            {
                await db.CloseAsync(); db = null;
            }
            return success == 1 ? customer : null;
        }

        public async Task BulkInsertOrUpdateCustomerMasterDataAsync(List<CustomerMaster> customerCollection)
        {
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache, false);
            //await db.EnableWriteAheadLoggingAsync().ConfigureAwait(false);

            try
            {
                await db.CreateTableAsync<CustomerMaster>().ConfigureAwait(false);
                foreach (var customer in customerCollection)
                {
                    try
                    {
                        //await _semaphore.WaitAsync().ConfigureAwait(false);

                        //CustomerMaster existingCustomer = await db.Table<CustomerMaster>().FirstOrDefaultAsync(a => a.DeviceCustomerID == customer.DeviceCustomerID);
                        //if (existingCustomer != null)
                        //{
                        //await db.Table<CustomerMaster>().DeleteAsync(a => a.DeviceCustomerID == existingCustomer.DeviceCustomerID).ConfigureAwait(false);
                        //}
                        await db.Table<CustomerMaster>().DeleteAsync(a => a.DeviceCustomerID == customer.DeviceCustomerID).ConfigureAwait(false);
                        customer.IsExported = 1;
                        await db.InsertAsync(customer).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "BulkInsertOrUpdateCustomerMasterDataAsync InnerLoop", ex);
                    }
                    finally
                    {
                        //_semaphore.Release();
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "BulkInsertOrUpdateCustomerMasterDataAsync", ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
        }

        //public async Task BulkInsertOrUpdateCustomerMasterDataAsync(List<CustomerMaster> customerMasters)
        //{
        //    if (customerMasters != null)
        //    {
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache, false);
        //        SemaphoreSlim _semaphore = new SemaphoreSlim(1);
        //        try
        //        {
        //            await db.CreateTableAsync<CustomerMaster>().ConfigureAwait(false);
        //            List<Task> tasks = new List<Task>(customerMasters.Count);
        //            foreach (CustomerMaster item in customerMasters)
        //            {
        //                tasks.Add(Task.Run(async () =>
        //                {
        //                    await _semaphore.WaitAsync().ConfigureAwait(false);
        //                    try
        //                    {
        //                        await db.Table<CustomerMaster>().Where(a => a.DeviceCustomerID == item.DeviceCustomerID).DeleteAsync();
        //                        item.IsExported = 1;
        //                        await db.InsertAsync(item);
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "BulkInsertOrUpdateCustomerMasterDataAsync >> Inner Loop >>", ex, item);
        //                    }
        //                    finally
        //                    {
        //                        _semaphore.Release();
        //                    }
        //                }));
        //            }
        //            await Task.WhenAll(tasks);
        //        }
        //        catch (AggregateException ae)
        //        {
        //            ae.Handle((ex) => { ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "BulkInsertOrUpdateCustomerMasterDataAsync", ex); return true; });
        //        }
        //        catch (Exception ex)
        //        {
        //            ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "BulkInsertOrUpdateCustomerMasterDataAsync", ex);
        //        }
        //        finally
        //        {
        //            _semaphore.Dispose();
        //            await db.CloseAsync().ConfigureAwait(false); db = null;
        //        }
        //    }
        //}


        public async Task<bool> InsertOrUpdateLnkPopItemsDataAsync(List<LnkPopItems> lnkPopItems)
        {
            int success = 0;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<LnkPopItems>();
                foreach (var lnkPopItem in lnkPopItems)
                {
                    await db.Table<LnkPopItems>().DeleteAsync(a => a.ID == lnkPopItem.ID);
                    success = await db.InsertAsync(lnkPopItem);
                }
            }
            catch (Exception ex)
            {
                success = 0;
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(InsertOrUpdateLnkPopItemsDataAsync), ex);
            }
            finally
            {
                await db.CloseAsync(); db = null;
            }
            return success == 1;
        }

        public async Task<bool> InsertOrUpdateOrderHistoryEmailDataAsync(List<OrderHistoryEmail> orderHistoryEmailList)
        {
            int success = 0;
            OrderHistoryEmail existingRecord = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                foreach (var orderHistoryEmailItem in orderHistoryEmailList)
                {
                    existingRecord = await (db.Table<OrderHistoryEmail>().Where(a => a.Id == orderHistoryEmailItem.Id)).FirstOrDefaultAsync();

                    if (existingRecord != null)
                    {
                        success = await db.UpdateAsync(orderHistoryEmailItem);
                    }
                    else
                    {
                        success = await db.InsertAsync(orderHistoryEmailItem);
                    }
                }
            }
            catch (Exception ex)
            {
                success = 0;
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(InsertOrUpdateOrderHistoryEmailDataAsync), ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return success == 1;
        }

        public async Task<bool> InsertOrUpdateProductAdditionalDocumentDataAsync(List<ProductAdditionalDocument> productAdditionalDocuments)
        {
            int success = 0;
            ProductAdditionalDocument existingRecord = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<ProductAdditionalDocument>();
                foreach (var productAdditionalDocumentItem in productAdditionalDocuments)
                {
                    await db.Table<ProductAdditionalDocument>().DeleteAsync(x => x.ProductID == productAdditionalDocumentItem.ProductID);

                    if (string.IsNullOrWhiteSpace(productAdditionalDocumentItem.LocalFilePath)) productAdditionalDocumentItem.LocalFilePath = productAdditionalDocumentItem.DocumentFileName;

                    success = await db.InsertAsync(productAdditionalDocumentItem);
                }
            }
            catch (Exception ex)
            {
                success = 0;
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(InsertOrUpdateProductAdditionalDocumentDataAsync), ex);
            }
            finally
            {
                await db.CloseAsync(); db = null;
            }

            return success == 1;
        }

        public async Task<bool> InsertOrUpdateProductMasterDataAsync(List<ProductMaster> productMasterList)
        {
            int success = 0;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<ProductMaster>();
                foreach (var productItem in productMasterList)
                {
                    await db.Table<ProductMaster>().DeleteAsync(a => a.ProductID == productItem.ProductID).ConfigureAwait(false);
                    success = await db.InsertAsync(productItem).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                success = 0;
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(InsertOrUpdateProductMasterDataAsync), ex);
            }
            finally
            {
                await db.CloseAsync(); db = null;
            }

            return success == 1;
        }

        public async Task<bool> InsertOrUpdatePromotionMasterDataAsync(List<PromotionMaster> promotionMasterList)
        {
            int success = 0;

            PromotionMaster existingRecord = null;

            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                foreach (var promotionItem in promotionMasterList)
                {
                    existingRecord = await (db.Table<PromotionMaster>().Where(a => a.PromotionID == promotionItem.PromotionID)).FirstOrDefaultAsync();

                    if (existingRecord != null)
                    {
                        success = await db.UpdateAsync(promotionItem);
                    }
                    else
                    {
                        success = await db.InsertAsync(promotionItem);
                    }
                }
            }
            catch (Exception ex)
            {
                success = 0;
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(InsertOrUpdatePromotionMasterDataAsync), ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return success == 1;
        }

        public async Task<bool> InsertOrUpdateRoleMasterDataAsync(List<RoleMaster> roleMasterList)
        {
            int success = 0;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<RoleMaster>();
                foreach (var roleItem in roleMasterList)
                {
                    await db.Table<RoleMaster>().DeleteAsync(a => a.RoleID == roleItem.RoleID);
                    success = await db.InsertAsync(roleItem);
                }
            }
            catch (Exception ex)
            {
                success = 0;
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(InsertOrUpdateRoleMasterDataAsync), ex);
            }
            finally
            {
                await db.CloseAsync(); db = null;
            }
            return success > 0;
        }


        public async Task<bool> InsertOrUpdateSalesDocumentDataAsync(List<SalesDocument> salesDocuments)
        {
            int success = 0;

            SalesDocument existingRecord = null;

            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<SalesDocument>().ConfigureAwait(false);
                foreach (var salesDocItem in salesDocuments)
                {
                    existingRecord = await (db.Table<SalesDocument>().Where(a => a.SalesDocumentID == salesDocItem.SalesDocumentID)).FirstOrDefaultAsync();

                    if (existingRecord != null)
                    {
                        success = await db.UpdateAsync(salesDocItem);
                    }
                    else
                    {
                        success = await db.InsertAsync(salesDocItem);
                    }
                }
            }
            catch (Exception ex)
            {
                success = 0;
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(InsertOrUpdateSalesDocumentDataAsync), ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return success == 1;
        }

        public async Task<bool> InsertOrUpdateStateMasterDataAsync(List<StateMaster> stateMasterList)
        {
            int success = 0;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<StateMaster>();
                foreach (var stateItem in stateMasterList)
                {
                    await db.Table<StateMaster>().DeleteAsync(a => a.StateID == stateItem.StateID);
                    success = await db.InsertAsync(stateItem);
                }
            }
            catch (Exception ex)
            {
                success = 0;
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(InsertOrUpdateStateMasterDataAsync), ex);
            }
            finally
            {
                await db.CloseAsync(); db = null;
            }
            return success != 0;
        }

        public async Task<bool> InsertOrUpdateStyleDataDataAsync(List<StyleData> styleData)
        {
            int success = 0;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<StyleData>();
                foreach (var styleItem in styleData)
                    success = await db.InsertOrReplaceAsync(styleItem, typeof(StyleData));
            }
            catch (Exception ex)
            {
                success = 0;
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(InsertOrUpdateStyleDataDataAsync), ex);
            }
            finally
            {
                await db.CloseAsync(); db = null;
            }
            return success == 1;
        }

        public async Task<bool> InsertOrUpdatetCategoryProductDataAsync(List<CategoryProduct> categoryProducts)
        {
            int success = 0;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<CategoryProduct>();
                foreach (var categoryProductItem in categoryProducts)
                {
                    await db.Table<CategoryProduct>().DeleteAsync(a => a.CategoryProductID == categoryProductItem.CategoryProductID);

                    success = await db.InsertAsync(categoryProductItem);
                }
            }
            catch (Exception ex)
            {
                success = 0;
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(InsertOrUpdatetCategoryProductDataAsync), ex);
            }
            finally
            {
                await db.CloseAsync(); db = null;
            }

            return success == 1;
        }


        public async Task<bool> InsertOrUpdatetClassificationDataAsync(List<Classification> classifications)
        {
            int success = 0;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<Classification>();
                foreach (var classificationItem in classifications)
                {
                    success = await db.InsertOrReplaceAsync(classificationItem);
                }
            }
            catch (Exception ex)
            {
                success = 0;
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "InsertOrUpdatetClassificationDataAsync", ex);
            }
            finally
            {
                await db.CloseAsync(); db = null;
            }

            return success != 0;
        }

        //public async Task<bool> InsertOrUpdatetConfigurationDataAsync(List<Configuration> configurations)
        //{
        //    int success = 0;

        //    Configuration existingRecord = null;

        //    try
        //    {
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);

        //        foreach (var configurationItem in configurations)
        //        {
        //            existingRecord = await (db.Table<Configuration>().Where(a => a.ConfigID == configurationItem.ConfigID)).FirstOrDefaultAsync();

        //            if (existingRecord != null)
        //            {
        //                success = await db.UpdateAsync(configurationItem);
        //            }
        //            else
        //            {
        //                success = await db.InsertAsync(configurationItem);
        //            }
        //        }

        //        await db.CloseAsync();

        //        return success == 1;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(InsertOrUpdatetConfigurationDataAsync), ex.StackTrace);
        //        return false;
        //    }
        //}

        public async Task<bool> InsertOrUpdatetConfigurationDataAsync(List<Configuration> configurations)
        {
            int success = 0;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<Configuration>();
                await db.Table<Configuration>().DeleteAsync(a => a.KEYName != "LastSyncDate");
                foreach (var configurationItem in configurations)
                {
                    success = await db.InsertAsync(configurationItem);
                }
            }
            catch (Exception ex)
            {
                success = 0;
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(InsertOrUpdatetConfigurationDataAsync), ex);
            }
            finally
            {
                await db.CloseAsync(); db = null;
            }
            return success == 1;
        }

        public async Task<ContactMaster> InsertOrUpdatetContactMasterDataAsync(ContactMaster contact)
        {
            int success = 0;
            ContactMaster existingRecord = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.EnableWriteAheadLoggingAsync().ConfigureAwait(false);
                existingRecord = await (db.Table<ContactMaster>().FirstOrDefaultAsync(a => a.ContactID == contact.ContactID)).ConfigureAwait(false);

                if (existingRecord != null)
                {
                    success = await db.UpdateAsync(contact).ConfigureAwait(false);
                }
                else
                {
                    success = await db.InsertAsync(contact).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                success = 0;
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "InsertOrUpdatetContactMasterDataAsync", ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }

            return success == 1 ? contact : null;
        }

        public async Task<ContactMaster> InsertOrUpdateDownloadedContactMasterDataAsync(ContactMaster contact)
        {
            int success = 0;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<ContactMaster>();
                await db.Table<ContactMaster>().DeleteAsync(a => a.DeviceContactID == contact.DeviceContactID || a.ContactID == contact.ContactID);

                contact.IsExported = 1;
                success = await db.InsertAsync(contact);
            }
            catch (Exception ex)
            {
                success = 0;
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "InsertOrUpdateDownloadedContactMasterDataAsync", ex);
            }
            finally
            {
                await db.CloseAsync(); db = null;
            }

            return success == 1 ? contact : null;
        }

        //public async Task<bool> InsertOrUpdateCustomerDistributorDataAsync(CustomerDistributor customerDistributor)
        //{
        //    int success = 0;

        //    CustomerDistributor existingRecord = null;

        //    try
        //    {
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);

        //        existingRecord = await (db.Table<CustomerDistributor>().Where(a => a.CustomerDistributorID == customerDistributor.CustomerDistributorID)).FirstOrDefaultAsync();

        //        if (existingRecord != null)
        //        {
        //            success = await db.UpdateAsync(customerDistributor);
        //        }
        //        else
        //        {
        //            success = await db.InsertAsync(customerDistributor);
        //        }

        //        await db.CloseAsync();

        //        return success == 1;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "InsertOrUpdateCustomerDistributorDataAsync", ex.StackTrace);
        //        return false;
        //    }
        //}

        public async Task<bool> InsertOrUpdateCustomerDistributorDataAsync(CustomerDistributor customerDistributor)
        {
            int success = 0;

            CustomerDistributor existingRecord = null;

            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.EnableWriteAheadLoggingAsync().ConfigureAwait(false);
                existingRecord = await (db.Table<CustomerDistributor>().FirstOrDefaultAsync(a => a.CustomerDistributorID == customerDistributor.CustomerDistributorID)).ConfigureAwait(false);

                if (existingRecord != null)
                {
                    success = await db.UpdateAsync(customerDistributor).ConfigureAwait(false);
                }
                else
                {
                    success = await db.InsertAsync(customerDistributor).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                success = 0;
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "InsertOrUpdateCustomerDistributorDataAsync", ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }

            return success == 1;
        }

        public async Task<bool> InsertOrUpdateDownloadedCustomerDistributorDataAsync(CustomerDistributor customerDistributor)
        {
            int success = 0;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<CustomerDistributor>();

                await (db.Table<CustomerDistributor>().Where(a => a.DeviceCustomerID == customerDistributor.DeviceCustomerID && a.DistributorID == customerDistributor.DistributorID)).DeleteAsync();

                customerDistributor.IsExported = 1;
                success = await db.InsertAsync(customerDistributor);
            }
            catch (Exception ex)
            {
                success = 0;
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "InsertOrUpdateDownloadedCustomerDistributorDataAsync", ex);
            }
            finally
            {
                await db.CloseAsync(); db = null;
            }

            return success == 1;
        }

        //public async Task<List<CustomerDocument>> InsertOrUpdatetCustomerDocumentDataAsync(List<CustomerDocument> customerDocuments)
        //{
        //    int success = 0;

        //    CustomerDocument existingRecord = null;

        //    try
        //    {
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);

        //        int index = 0;

        //        var returnList = new List<CustomerDocument>();

        //        foreach (var customerDocItem in customerDocuments)
        //        {
        //            existingRecord = await db.Table<CustomerDocument>().Where(a => a.CustomerDocumentID == customerDocItem.CustomerDocumentID).FirstOrDefaultAsync();

        //            if (existingRecord != null)
        //            {
        //                //success = await db.UpdateAsync(customerDocItem);
        //                string query = string.Format("Update CustomerDocument set CustomerID = {1}, OriginalFileName = '{2}', CustomerDocumentName = '{3}', ImportedFrom = {4}, IsExported = {5}, CreateDateTime = '{6}', UpdateDateTime = '{7}', CustomerDocDesc = '{8}', CustomerDocType = '{9}', DeviceDocID = '{10}', IsDelete = '{11}', IsDownload = {12}, IsPublishedToChild = '{13}' Where CustomerDocumentID = {0}", customerDocItem.CustomerDocumentID, customerDocItem.CustomerID, customerDocItem.OriginalFileName, customerDocItem.CustomerDocumentName, customerDocItem.ImportedFrom, customerDocItem.IsExported, customerDocItem.CreateDateTime, customerDocItem.UpdateDateTime, customerDocItem.CustomerDocDesc, customerDocItem.CustomerDocType, customerDocItem.DeviceDocID, customerDocItem.IsDelete, customerDocItem.IsDownload, customerDocItem.IsPublishedToChild);
        //                success = await db.ExecuteAsync(query);
        //            }
        //            else
        //            {
        //                //success = await db.InsertAsync(customerDocItem);
        //                string query = string.Format("Insert into CustomerDocument (CustomerDocumentID, CustomerID, OriginalFileName, CustomerDocumentName, ImportedFrom, IsExported, CreateDateTime, UpdateDateTime, CustomerDocDesc, CustomerDocType, DeviceDocID, IsDelete, IsDownload, IsPublishedToChild) Values({0},{1},'{2}','{3}',{4},{5},'{6}','{7}','{8}','{9}','{10}','{11}',{12},'{13}')", customerDocItem.CustomerDocumentID, customerDocItem.CustomerID, customerDocItem.OriginalFileName, customerDocItem.CustomerDocumentName, customerDocItem.ImportedFrom, customerDocItem.IsExported, customerDocItem.CreateDateTime, customerDocItem.UpdateDateTime, customerDocItem.CustomerDocDesc, customerDocItem.CustomerDocType, customerDocItem.DeviceDocID, customerDocItem.IsDelete, customerDocItem.IsDownload, customerDocItem.IsPublishedToChild);
        //                success = await db.ExecuteAsync(query);
        //            }
        //            returnList.Add(customerDocItem);
        //            index++;
        //        }

        //        await db.CloseAsync();

        //        return returnList;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "InsertOrUpdatetCustomerDocumentDataAsync", ex.StackTrace);
        //        return null;
        //    }
        //}

        public async Task<List<CustomerDocument>> InsertOrUpdatetCustomerDocumentDataAsync(List<CustomerDocument> customerDocuments, int isExportedDelete)
        {
            int success = 0;
            CustomerDocument existingRecord = null;
            int index = 0;
            List<CustomerDocument> returnList = new List<CustomerDocument>();
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.EnableWriteAheadLoggingAsync().ConfigureAwait(false);
                foreach (var customerDocItem in customerDocuments)
                {
                    existingRecord = await db.Table<CustomerDocument>().FirstOrDefaultAsync(a => a.CustomerDocumentID == customerDocItem.CustomerDocumentID).ConfigureAwait(false);

                    if (existingRecord != null)
                    {
                        //success = await db.UpdateAsync(customerDocItem);
                        if (isExportedDelete == 0 && (customerDocItem.IsDelete == "1" || customerDocItem.IsDelete == "true"))
                        {
                            string query = string.Format("Delete from CustomerDocument Where CustomerDocumentID = {0}", customerDocItem.CustomerDocumentID);
                            success = await db.ExecuteAsync(query).ConfigureAwait(false);
                        }
                        else
                        {
                            string query = string.Format("Update CustomerDocument set CustomerID = {1}, OriginalFileName = '{2}', CustomerDocumentName = '{3}', ImportedFrom = {4}, IsExported = {5}, CreateDateTime = '{6}', UpdateDateTime = '{7}', CustomerDocDesc = '{8}', CustomerDocType = '{9}', DeviceDocID = '{10}', IsDelete = '{11}', IsDownload = {12}, IsPublishedToChild = '{13}' Where CustomerDocumentID = {0}", customerDocItem.CustomerDocumentID, customerDocItem.CustomerID, customerDocItem.OriginalFileName, customerDocItem.CustomerDocumentName, customerDocItem.ImportedFrom, customerDocItem.IsExported, customerDocItem.CreateDateTime, customerDocItem.UpdateDateTime, customerDocItem.CustomerDocDesc, customerDocItem.CustomerDocType, customerDocItem.DeviceDocID, customerDocItem.IsDelete, customerDocItem.IsDownload, customerDocItem.IsPublishedToChild);
                            success = await db.ExecuteAsync(query).ConfigureAwait(false);
                        }
                    }
                    else
                    {
                        //success = await db.InsertAsync(customerDocItem);
                        string query = string.Format("Insert into CustomerDocument (CustomerDocumentID, CustomerID, OriginalFileName, CustomerDocumentName, ImportedFrom, IsExported, CreateDateTime, UpdateDateTime, CustomerDocDesc, CustomerDocType, DeviceDocID, IsDelete, IsDownload, IsPublishedToChild) Values({0},{1},'{2}','{3}',{4},{5},'{6}','{7}','{8}','{9}','{10}','{11}',{12},'{13}')", customerDocItem.CustomerDocumentID, customerDocItem.CustomerID, customerDocItem.OriginalFileName, customerDocItem.CustomerDocumentName, customerDocItem.ImportedFrom, customerDocItem.IsExported, customerDocItem.CreateDateTime, customerDocItem.UpdateDateTime, customerDocItem.CustomerDocDesc, customerDocItem.CustomerDocType, customerDocItem.DeviceDocID, customerDocItem.IsDelete, customerDocItem.IsDownload, customerDocItem.IsPublishedToChild);
                        success = await db.ExecuteAsync(query).ConfigureAwait(false);
                    }
                    returnList.Add(customerDocItem);
                    index++;
                }
            }
            catch (Exception ex)
            {
                returnList = null;
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "InsertOrUpdatetCustomerDocumentDataAsync", ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }

            return returnList;
        }

        public async Task InsertOrUpdateDownloadedCustomerDocumentDataAsync(List<CustomerDocument> customerDocuments)
        {
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<CustomerDocument>();
                foreach (var customerDocItem in customerDocuments)
                {
                    await db.Table<CustomerDocument>().DeleteAsync(a => a.DeviceDocID == customerDocItem.DeviceDocID);
                    customerDocItem.IsExported = 1;
                    await db.InsertAsync(customerDocItem).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "InsertOrUpdateDownloadedCustomerDocumentDataAsync", ex);
            }
            finally
            {
                await db.CloseAsync(); db = null;
            }
        }

        public async Task<bool> InsertOrUpdatetDistributorMasterDataAsync(List<DistributorMaster> distributorMasterList)
        {
            int success = 0;

            DistributorMaster existingRecord = null;

            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                foreach (var distributorItem in distributorMasterList)
                {
                    existingRecord = await db.Table<DistributorMaster>().FirstOrDefaultAsync(a => a.CustomerID == distributorItem.CustomerID).ConfigureAwait(false);
                    if (existingRecord != null)
                    {
                        await db.Table<DistributorMaster>().DeleteAsync(a => a.CustomerID == distributorItem.CustomerID).ConfigureAwait(false);
                    }

                    if (distributorItem.isDeleted != 1)
                        success = await db.InsertAsync(distributorItem).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                success = 0;
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(InsertOrUpdatetDistributorMasterDataAsync), ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return success == 1;
        }

        //public async Task<bool> InsertOrUpdatetFavoriteDataAsync(List<Favorite> favoritesList)
        //{
        //    int success = 0;

        //    try
        //    {
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);

        //        foreach (var favoritesItem in favoritesList)
        //        {
        //            var existingRecord = await db.Table<Favorite>().Where(a => a.ProductId == favoritesItem.ProductId).FirstOrDefaultAsync();

        //            if (existingRecord != null)
        //            {
        //                await db.Table<Favorite>().DeleteAsync(x => x.ProductId == favoritesItem.ProductId);
        //            }

        //            success = await db.InsertAsync(favoritesItem);
        //        }

        //        await db.CloseAsync();

        //        return success == 1;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(InsertOrUpdatetFavoriteDataAsync), ex.StackTrace);
        //        return false;
        //    }
        //}
        public async Task<bool> InsertOrUpdatetDownloadedFavoriteDataAsync(List<Favorite> favoritesList)
        {
            int success = 0;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<Favorite>().ConfigureAwait(false);
                foreach (var favoritesItem in favoritesList)
                {
                    try
                    {
                        await db.Table<Favorite>().DeleteAsync(a => a.ProductId == favoritesItem.ProductId).ConfigureAwait(false);
                        await db.Table<Favorite>().DeleteAsync(a => a.FavoriteID == favoritesItem.FavoriteID).ConfigureAwait(false);
                        success = await db.InsertAsync(favoritesItem).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(InsertOrUpdatetFavoriteDataAsync), ex, favoritesItem);
                    }
                }
            }
            catch (Exception ex)
            {
                success = 0;
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(InsertOrUpdatetFavoriteDataAsync), ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false); db = null;
            }
            return success != 0;
        }
        public async Task<bool> InsertOrUpdatetFavoriteDataAsync(List<Favorite> favoritesList)
        {
            int success = 0;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<Favorite>();
                foreach (var favoritesItem in favoritesList)
                {
                    try
                    {
                        await db.Table<Favorite>().DeleteAsync(a => a.ProductId == favoritesItem.ProductId);
                        success = await db.InsertAsync(favoritesItem);
                    }
                    catch (Exception ex)
                    {
                        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(InsertOrUpdatetFavoriteDataAsync), ex, favoritesItem);
                    }
                }
            }
            catch (Exception ex)
            {
                success = 0;
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(InsertOrUpdatetFavoriteDataAsync), ex);
            }
            finally
            {
                await db.CloseAsync(); db = null;
            }
            return success != 0;
        }

        public async Task<bool> InsertOrUpdatetLnkRackItemsDataAsync(List<LnkRackItems> lnkRackItemList)
        {
            int success = 0;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<LnkRackItems>();
                foreach (var lnkRackItem in lnkRackItemList)
                {
                    await db.Table<LnkRackItems>().DeleteAsync(a => a.ID == lnkRackItem.ID);
                    success = await db.InsertAsync(lnkRackItem);
                }
            }
            catch (Exception ex)
            {
                success = 0;
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(InsertOrUpdatetLnkRackItemsDataAsync), ex);
            }
            finally
            {
                await db.CloseAsync(); db = null;
            }

            return success == 1;
        }

        public async Task<OrderDetail> InsertOrUpdatetOrderDetailDataAsync(OrderDetail orderDetail)
        {
            int success = 0;
            if (orderDetail != null)
            {
                SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
                try
                {
                    await db.CreateTableAsync<OrderDetail>();
                    await db.Table<OrderDetail>().DeleteAsync(a => a.OrderDetailId == orderDetail.OrderDetailId).ConfigureAwait(false);

                    // Sanitize null string properties BEFORE insert
                    orderDetail.CategoryName = orderDetail.CategoryName ?? string.Empty;
                    orderDetail.BrandName = orderDetail.BrandName ?? string.Empty;
                    orderDetail.StyleName = orderDetail.StyleName ?? string.Empty;
                    orderDetail.ProductName = orderDetail.ProductName ?? string.Empty;
                    orderDetail.Price = orderDetail.Price ?? "0.00";
                    orderDetail.Unit = orderDetail.Unit ?? string.Empty;
                    orderDetail.Total = orderDetail.Total ?? "0.00";
                    orderDetail.CreatedDate = orderDetail.CreatedDate ?? DateTimeHelper.ConvertToDbInsertDateTimeMilliSecondFormat(DateTime.Now);
                    await db.InsertAsync(orderDetail);
                    success = 1;
                }
                catch (Exception ex)
                {
                    ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "InsertOrUpdatetOrderDetailDataAsync will try again in recurrsive call", ex, orderDetail);
                    await db.CloseAsync(); db = null;
                    // Max 2 attempt
                    Int16 tryCount = 1;
                    while (tryCount < 3)
                    {
                        if (await TryInsertOrUpdatetOrderDetailDataAsync(orderDetail, tryCount))
                        { success = 1; break; }
                        else tryCount--;
                    }
                }
                finally
                {
                    if (db != null)
                    {
                        await db.CloseAsync(); db = null;
                    }
                }
            }
            return success == 1 ? orderDetail : null;
        }

        private async Task<bool> TryInsertOrUpdatetOrderDetailDataAsync(OrderDetail orderDetail, Int16 tryCount)
        {
            int success = 0;
            if (orderDetail != null)
            {
                SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
                try
                {
                    await db.CreateTableAsync<OrderDetail>().ConfigureAwait(false);
                    await db.Table<OrderDetail>().DeleteAsync(a => a.OrderDetailId == orderDetail.OrderDetailId).ConfigureAwait(false);
                    await db.InsertAsync(orderDetail).ConfigureAwait(false);
                    success = 1;
                }
                catch (Exception ex)
                {
                    ErrorLogger.WriteToErrorLog(nameof(DatabaseService), $"TryInsertOrUpdatetOrderDetailDataAsync occur in attempted {tryCount}", ex, orderDetail);
                    success = 0;
                }
                finally
                {
                    await db.CloseAsync(); db = null;
                }
            }
            return success == 1;
        }

        public async Task BulkInsertOrUpdateOrderDetailDataAsync(List<OrderDetail> orderDetailCollection)
        {
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache, false);
            //await db.EnableWriteAheadLoggingAsync().ConfigureAwait(false);
            try
            {
                await db.CreateTableAsync<OrderDetail>();
                foreach (OrderDetail orderDetail in orderDetailCollection)
                {
                    try
                    {
                        await db.Table<OrderDetail>().DeleteAsync(a => a.OrderDetailId == orderDetail.OrderDetailId).ConfigureAwait(false);
                        await db.InsertAsync(orderDetail);
                    }
                    catch (Exception ex)
                    {
                        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "BulkInsertOrUpdateOrderDetailDataAsync InnerLoop", ex, orderDetail);
                    }
                    finally
                    {
                        //_semaphore.Release();
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "BulkInsertOrUpdateOrderDetailDataAsync", ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }

        }



        //public async Task BulkInsertOrUpdateOrderDetailDataAsync(List<OrderDetail> orderDetails)
        //{
        //    if (orderDetails != null)
        //    {
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache, false);
        //        SemaphoreSlim _semaphore = new SemaphoreSlim(1);
        //        try
        //        {
        //            await db.CreateTableAsync<OrderDetail>().ConfigureAwait(false);
        //            List<Task> tasks = new List<Task>(orderDetails.Count);
        //            foreach (OrderDetail item in orderDetails)
        //            {
        //                tasks.Add(Task.Run(async () =>
        //                {
        //                    await _semaphore.WaitAsync().ConfigureAwait(false);
        //                    try
        //                    {
        //                        await db.Table<OrderDetail>().Where(a => a.OrderDetailId == item.OrderDetailId).DeleteAsync().ConfigureAwait(false);
        //                        await db.InsertAsync(item).ConfigureAwait(false);
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "InsertOrUpdatetOrderDetailDataAsync >> Inner Loop >>", ex, item);
        //                    }
        //                    finally
        //                    {
        //                        _semaphore.Release();//}
        //                    }
        //                }));
        //            }
        //            await Task.WhenAll(tasks);
        //        }
        //        catch (AggregateException ae)
        //        {
        //            ae.Handle((ex) => { ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "InsertOrUpdatetOrderDetailDataAsync", ex); return true; });
        //        }
        //        catch (Exception ex)
        //        {
        //            ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "InsertOrUpdatetOrderDetailDataAsync", ex);
        //        }
        //        finally
        //        {
        //            _semaphore.Dispose();
        //            await db.CloseAsync().ConfigureAwait(false); db = null;
        //        }
        //    }


        public async Task<OrderMaster> InsertOrUpdateOrderMasterDataAsync(OrderMaster orderMaster)
        {
            int success = 0;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<OrderDetail>().ConfigureAwait(false);
                var existingRecord = await db.Table<OrderMaster>()
                     .FirstOrDefaultAsync(a => a.DeviceOrderID == orderMaster.DeviceOrderID).ConfigureAwait(false);
                if (existingRecord != null)
                {
                    success = await db.UpdateAsync(orderMaster).ConfigureAwait(false);
                }
                else
                {
                    success = await db.InsertAsync(orderMaster).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "InsertOrUpdateOrderMasterDataAsync", ex.StackTrace);
                success = 0;
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false); db = null;
            }
            return success == 1 ? orderMaster : null;
        }

        public async Task<OrderMaster> InsertOrUpdateDownloadedOrderMasterDataAsync(OrderMaster orderMaster)
        {
            int success = 0;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<OrderMaster>();
                var existingRecord = await db.Table<OrderMaster>()
                     .FirstOrDefaultAsync(a => a.DeviceOrderID == orderMaster.DeviceOrderID);
                if (existingRecord != null)
                    await db.DeleteAsync(existingRecord);
                orderMaster.IsExported = 1;
                success = await db.InsertAsync(orderMaster);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "InsertOrUpdateDownloadedOrderMasterDataAsync", ex);
                success = 0;
            }
            finally
            {
                await db.CloseAsync(); db = null;
            }
            return success == 1 ? orderMaster : null;
        }

        public async Task BulkInsertOrUpdateDownloadOrderMasterDataAsync(List<OrderMaster> orderCollection)
        {
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache, false);
            //await db.EnableWriteAheadLoggingAsync().ConfigureAwait(false);
            try
            {
                await db.CreateTableAsync<OrderMaster>();
                foreach (OrderMaster orderMaster in orderCollection)
                {
                    try
                    {
                        //await _semaphore.WaitAsync().ConfigureAwait(false);

                        //var existingRecord = await db.Table<OrderMaster>()
                        //     .FirstOrDefaultAsync(a => a.DeviceOrderID == orderMaster.DeviceOrderID);
                        //if (existingRecord != null)
                        //{
                        //var orderDetails = await db.Table<OrderDetail>().Where(a => a.DeviceOrderID == existingRecord.DeviceOrderID).ToListAsync();
                        //if (orderDetails != null)
                        //    await db.Table<OrderDetail>().Where(a => a.DeviceOrderID == existingRecord.DeviceOrderID).DeleteAsync().ConfigureAwait(false);

                        //await db.DeleteAsync(existingRecord).ConfigureAwait(false);
                        //}

                        await db.Table<OrderDetail>().DeleteAsync(a => a.DeviceOrderID == orderMaster.DeviceOrderID).ConfigureAwait(false);
                        await db.Table<OrderMaster>().DeleteAsync(a => a.DeviceOrderID == orderMaster.DeviceOrderID).ConfigureAwait(false);

                        orderMaster.IsExported = 1;
                        await db.InsertAsync(orderMaster);
                    }
                    catch (Exception ex)
                    {
                        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "BulkInsertOrUpdateDownloadOrderMasterDataAsync InnerLoop", ex, orderMaster);
                    }
                    finally
                    {
                        //_semaphore.Release();
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "BulkInsertOrUpdateDownloadOrderMasterDataAsync", ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
        }



        //public async Task BulkInsertOrUpdateDownloadOrderMasterDataAsync(List<OrderMaster> orderMasters)
        //{
        //    if (orderMasters != null)
        //    {
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache, false);
        //        SemaphoreSlim _semaphore = new SemaphoreSlim(1);
        //        try
        //        {
        //            await db.CreateTableAsync<OrderMaster>().ConfigureAwait(false);
        //            await db.CreateTableAsync<OrderDetail>().ConfigureAwait(false);
        //            List<Task> tasks = new List<Task>(orderMasters.Count);
        //            foreach (OrderMaster item in orderMasters)
        //            {
        //                tasks.Add(Task.Run(async () =>
        //                {
        //                    await _semaphore.WaitAsync().ConfigureAwait(false);
        //                    try
        //                    {
        //                        //var existingOrderDetailItem = await db.Table<OrderDetail>().Where(a => a.DeviceOrderID == item.DeviceOrderID).ToListAsync();
        //                        //if (existingOrderDetailItem != null && existingOrderDetailItem?.Count > 0)
        //                        //{
        //                        //    await db.Table<OrderDetail>().DeleteAsync(a => a.DeviceOrderID == item.DeviceOrderID);
        //                        //}
        //                        await db.Table<OrderDetail>().Where(a => a.DeviceOrderID == item.DeviceOrderID).DeleteAsync().ConfigureAwait(false);
        //                        await db.Table<OrderMaster>().Where(a => a.DeviceOrderID == item.DeviceOrderID).DeleteAsync().ConfigureAwait(false);

        //                        //var existingOrderItem = await db.Table<OrderMaster>().Where(a => a.DeviceOrderID == item.DeviceOrderID).FirstOrDefaultAsync();
        //                        //if (existingOrderItem != null)
        //                        //{
        //                        //    await db.Table<OrderMaster>().DeleteAsync(a => a.DeviceOrderID == item.DeviceOrderID);
        //                        //}

        //                        item.IsExported = 1;

        //                        await db.InsertAsync(item).ConfigureAwait(false);
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "BulkInsertOrUpdateOrderMasterDataAsync >> Inner Loop >>", ex, item);
        //                    }
        //                    finally
        //                    {
        //                        _semaphore.Release();
        //                    }
        //                }));
        //            }
        //            await Task.WhenAll(tasks);
        //        }
        //        catch (AggregateException ae)
        //        {
        //            ae.Handle((ex) => { ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "BulkInsertOrUpdateOrderMasterDataAsync", ex); return true; });
        //        }
        //        catch (Exception ex)
        //        {
        //            ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "BulkInsertOrUpdateOrderMasterDataAsync", ex);
        //        }
        //        finally
        //        {
        //            _semaphore.Dispose();
        //            await db.CloseAsync().ConfigureAwait(false); db = null;
        //        }
        //    }
        //}




        //public async Task<bool> InsertOrUpdatetPositionMasterDataAsync(List<PositionMaster> positionMasterList)
        //{
        //    int success = 0;

        //    PositionMaster existingRecord = null;

        //    try
        //    {
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);

        //        foreach (var positionItem in positionMasterList)
        //        {
        //            existingRecord = await (db.Table<PositionMaster>().Where(a => a.PositionID == positionItem.PositionID)).FirstOrDefaultAsync();

        //            if (existingRecord != null)
        //            {
        //                success = await db.UpdateAsync(positionItem);
        //            }
        //            else
        //            {
        //                success = await db.InsertAsync(positionItem);
        //            }
        //        }

        //        await db.CloseAsync();

        //        return success == 1;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(InsertOrUpdatetPositionMasterDataAsync), ex.StackTrace);
        //        return false;
        //    }
        //}
        public async Task<bool> InsertOrUpdatetPositionMasterDataAsync(List<PositionMaster> positionMasterList)
        {
            int success = 0;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<PositionMaster>();
                foreach (var positionItem in positionMasterList)
                {
                    await db.Table<PositionMaster>().DeleteAsync(a => a.PositionID == positionItem.PositionID);

                    success = await db.InsertAsync(positionItem);
                }
            }
            catch (Exception ex)
            {
                success = 0;
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(InsertOrUpdatetPositionMasterDataAsync), ex);
            }
            finally
            {
                await db.CloseAsync(); db = null;
            }

            return success == 1;
        }

        //public async Task<ProductDistribution> InsertOrUpdatetProductDistributionDataAsync(ProductDistribution productDistributionItem)
        //{
        //    int success = 0;

        //    ProductDistribution existingRecord = null;

        //    try
        //    {
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);

        //        existingRecord = await db.Table<ProductDistribution>().Where(a => a.ID == productDistributionItem.ID).FirstOrDefaultAsync();

        //        if (existingRecord != null)
        //        {
        //            success = await db.UpdateAsync(productDistributionItem);
        //        }
        //        else
        //        {
        //            success = await db.InsertAsync(productDistributionItem);
        //        }

        //        await db.CloseAsync();

        //        return success == 1 ? productDistributionItem : null;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "InsertOrUpdatetProductDistributionDataAsync", ex.StackTrace);
        //        return null;
        //    }
        //}
        public async Task<ProductDistribution> InsertOrUpdatetProductDistributionDataAsync(ProductDistribution productDistributionItem)
        {
            int success = 0;
            ProductDistribution existingRecord = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.EnableWriteAheadLoggingAsync().ConfigureAwait(false);
                existingRecord = await db.Table<ProductDistribution>().FirstOrDefaultAsync(a => a.ID == productDistributionItem.ID).ConfigureAwait(false);

                if (existingRecord != null)
                {
                    success = await db.UpdateAsync(productDistributionItem).ConfigureAwait(false);
                }
                else
                {
                    success = await db.InsertAsync(productDistributionItem).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                success = 0;
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "InsertOrUpdatetProductDistributionDataAsync", ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }

            return success == 1 ? productDistributionItem : null;
        }

        public async Task<ProductDistribution> DeleteAndInsertDownloadedProductDistributionDataAsync(ProductDistribution productDistributionItem)
        {
            int success = 0;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<ProductDistribution>();
                await db.Table<ProductDistribution>().DeleteAsync(a => a.ProductId == productDistributionItem.ProductId && a.CustomerId == productDistributionItem.CustomerId);

                productDistributionItem.isExported = 1;
                success = await db.InsertAsync(productDistributionItem);
            }
            catch (Exception ex)
            {
                success = 0;
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "DeleteAndInsertDownloadedProductDistributionDataAsync", ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false); db = null;
            }
            return success != 0 ? productDistributionItem : null;
        }

        //public async Task<ProductDistribution> InsertProductDistributionDataFromSRCList(ProductDistribution productDistributionItem)
        //{
        //    int success = 0;

        //    ProductDistribution existingRecord = null;

        //    try
        //    {
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);

        //        existingRecord = await db.Table<ProductDistribution>().Where(a =>
        //            a.CustomerId == productDistributionItem.CustomerId && a.ProductId == productDistributionItem.ProductId).FirstOrDefaultAsync();

        //        if (existingRecord != null)
        //        {
        //            existingRecord.CustomerId = productDistributionItem.CustomerId;
        //            existingRecord.ProductId = productDistributionItem.ProductId;
        //            existingRecord.DistributionDate = productDistributionItem.DistributionDate;
        //            existingRecord.UpdateDate = DateTimeHelper.ConvertToDbInsertDateTimeFormat(DateTime.Now);
        //            existingRecord.isExported = productDistributionItem.isExported;
        //            existingRecord.IsDeleted = productDistributionItem.IsDeleted;
        //            if (!string.IsNullOrEmpty(productDistributionItem.CustomerProductID))
        //            {
        //                existingRecord.CustomerProductID = productDistributionItem.CustomerProductID;
        //            }

        //            await db.UpdateAsync(existingRecord);
        //        }
        //        else
        //        {
        //            success = await db.InsertAsync(productDistributionItem);
        //        }

        //        await db.CloseAsync();

        //        return success == 1 ? productDistributionItem : null;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "InsertProductDistributionDataFromSRCList", ex.StackTrace);
        //        return null;
        //    }
        //}

        public async Task<ProductDistribution> InsertProductDistributionDataFromSRCList(ProductDistribution productDistributionItem)
        {
            int success = 0;

            ProductDistribution existingRecord = null;

            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                existingRecord = await db.Table<ProductDistribution>().Where(a =>
                   a.CustomerId == productDistributionItem.CustomerId && a.ProductId == productDistributionItem.ProductId)
                    .FirstOrDefaultAsync();

                if (existingRecord != null)
                {
                    existingRecord.CustomerId = productDistributionItem.CustomerId;
                    existingRecord.ProductId = productDistributionItem.ProductId;
                    existingRecord.DistributionDate = productDistributionItem.DistributionDate;
                    existingRecord.UpdateDate = DateTimeHelper.ConvertToDbInsertDateTimeFormat(DateTime.Now);
                    existingRecord.isExported = productDistributionItem.isExported;
                    existingRecord.IsDeleted = productDistributionItem.IsDeleted;
                    if (!string.IsNullOrEmpty(productDistributionItem.CustomerProductID))
                    {
                        existingRecord.CustomerProductID = productDistributionItem.CustomerProductID;
                    }

                    await db.UpdateAsync(existingRecord);
                }
                else
                {
                    success = await db.InsertAsync(productDistributionItem);
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "InsertProductDistributionDataFromSRCList", ex);
            }
            finally
            {
                await db.CloseAsync();
            }
            return success == 1 ? productDistributionItem : null;
        }

        //public async Task<bool> InsertOrUpdatetProductRoleLinkDataAsync(List<ProductRoleLink> productRoleLinks)
        //{
        //    int success = 0;

        //    ProductRoleLink existingRecord = null;

        //    try
        //    {
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);

        //        foreach (var productRoleLinkItem in productRoleLinks)
        //        {
        //            existingRecord = await (db.Table<ProductRoleLink>().Where(a => a.ProductRoleLinkID == productRoleLinkItem.ProductRoleLinkID)).FirstOrDefaultAsync();

        //            if (existingRecord != null)
        //            {
        //                success = await db.UpdateAsync(productRoleLinkItem);
        //            }
        //            else
        //            {
        //                success = await db.InsertAsync(productRoleLinkItem);
        //            }
        //        }

        //        await db.CloseAsync();

        //        return success == 1;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(InsertOrUpdatetProductRoleLinkDataAsync), ex.StackTrace);
        //        return false;
        //    }
        //}

        public async Task<bool> InsertOrUpdatetProductRoleLinkDataAsync(List<ProductRoleLink> productRoleLinks)
        {
            int success = 0;

            ProductRoleLink existingRecord = null;

            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.EnableWriteAheadLoggingAsync().ConfigureAwait(false);
                foreach (var productRoleLinkItem in productRoleLinks)
                {
                    existingRecord = await (db.Table<ProductRoleLink>().FirstOrDefaultAsync(a => a.ProductRoleLinkID == productRoleLinkItem.ProductRoleLinkID)).ConfigureAwait(false);

                    if (existingRecord != null)
                    {
                        success = await db.UpdateAsync(productRoleLinkItem).ConfigureAwait(false);
                    }
                    else
                    {
                        success = await db.InsertAsync(productRoleLinkItem).ConfigureAwait(false);
                    }
                }
            }
            catch (Exception ex)
            {
                success = 0;
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(InsertOrUpdatetProductRoleLinkDataAsync), ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return success == 1;
        }

        public async Task<bool> InsertOrUpdatetRankMasterDataAsync(List<RankMaster> rankMasterList)
        {
            int success = 0;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<RankMaster>();
                foreach (var rankItem in rankMasterList)
                {
                    await db.Table<RankMaster>().DeleteAsync(a => a.RankID == rankItem.RankID);

                    success = await db.InsertAsync(rankItem);
                }
            }
            catch (Exception ex)
            {
                success = 0;
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(InsertOrUpdatetRankMasterDataAsync), ex);
            }
            finally
            {
                await db.CloseAsync(); db = null;
            }

            return success == 1;
        }

        //public async Task<bool> InsertOrUpdateTravelMasterDataAsync(List<TravelMaster> travelMasterList)
        //{
        //    int success = 0;

        //    TravelMaster existingRecord = null;

        //    try
        //    {
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);

        //        foreach (var travelItem in travelMasterList)
        //        {
        //            existingRecord = await (db.Table<TravelMaster>().Where(a => a.TravelID == travelItem.TravelID)).FirstOrDefaultAsync();

        //            if (existingRecord != null)
        //            {
        //                success = await db.UpdateAsync(travelItem);
        //            }
        //            else
        //            {
        //                success = await db.InsertAsync(travelItem);
        //            }
        //        }

        //        await db.CloseAsync();

        //        return success == 1;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(InsertOrUpdateTravelMasterDataAsync), ex.StackTrace);
        //        return false;
        //    }
        //}

        public async Task<bool> InsertOrUpdateTravelMasterDataAsync(List<TravelMaster> travelMasterList)
        {
            int success = 0;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<TravelMaster>();
                foreach (var travelItem in travelMasterList)
                {
                    await db.Table<TravelMaster>().DeleteAsync(a => a.TravelID == travelItem.TravelID);

                    success = await db.InsertAsync(travelItem);
                }
            }
            catch (Exception ex)
            {
                success = 0;
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(InsertOrUpdateTravelMasterDataAsync), ex);
            }
            finally
            {
                await db.CloseAsync(); db = null;
            }

            return success == 1;
        }

        //public async Task<bool> InsertOrUpdatetRegionMasterDataAsync(List<RegionMaster> regionMasterList)
        //{
        //    int success = 0;

        //    RegionMaster existingRecord = null;

        //    try
        //    {
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);

        //        foreach (var regionItem in regionMasterList)
        //        {
        //            existingRecord = await (db.Table<RegionMaster>().Where(a => a.RegionID == regionItem.RegionID)).FirstOrDefaultAsync();

        //            if (existingRecord != null)
        //            {
        //                success = await db.UpdateAsync(regionItem);
        //            }
        //            else
        //            {
        //                success = await db.InsertAsync(regionItem);
        //            }
        //        }

        //        await db.CloseAsync();

        //        return success == 1;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(InsertOrUpdatetRegionMasterDataAsync), ex.StackTrace);
        //        return false;
        //    }
        //}

        public async Task<bool> InsertOrUpdatetRegionMasterDataAsync(List<RegionMaster> regionMasterList)
        {
            int success = 0;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<RegionMaster>();
                foreach (var regionItem in regionMasterList)
                {
                    await db.Table<RegionMaster>().DeleteAsync(a => a.RegionID == regionItem.RegionID);

                    success = await db.InsertAsync(regionItem);
                }
            }
            catch (Exception ex)
            {
                success = 0;
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(InsertOrUpdatetRegionMasterDataAsync), ex);
            }
            finally
            {
                await db.CloseAsync(); db = null;
            }

            return success != 0;
        }

        //public async Task<bool> InsertOrUpdatetRouteStationsDataAsync(List<RouteStations> routeStationsList)
        //{
        //    int success = 0;

        //    RouteStations existingRecord = null;

        //    try
        //    {
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);

        //        foreach (var routeStationItem in routeStationsList)
        //        {
        //            existingRecord = await (db.Table<RouteStations>().Where(a => a.RouteStationId == routeStationItem.RouteStationId)).FirstOrDefaultAsync();

        //            if (existingRecord != null)
        //            {
        //                success = await db.UpdateAsync(routeStationItem);
        //            }
        //            else
        //            {
        //                success = await db.InsertAsync(routeStationItem);
        //            }
        //        }

        //        await db.CloseAsync();

        //        return success == 1;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(InsertOrUpdatetRouteStationsDataAsync), ex.StackTrace);
        //        return false;
        //    }
        //}
        public async Task<bool> InsertOrUpdatetRouteStationDataAsync(RouteStations routeStation)
        {
            int success = 0;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<RouteStations>().ConfigureAwait(false);
                success = await db.InsertOrReplaceAsync(routeStation).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(InsertOrUpdatetRouteStationDataAsync), ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false); db = null;
            }
            return success > 0;
        }

        //public async Task<List<ScheduledRoutes>> InsertOrUpdatetScheduledRoutesDataAsync(List<ScheduledRoutes> scheduledRoutes)
        //{
        //    int success = 0;

        //    ScheduledRoutes existingRecord = null;

        //    try
        //    {
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);

        //        foreach (var scheduledRouteItem in scheduledRoutes)
        //        {
        //            existingRecord = await db.Table<ScheduledRoutes>().Where(a => a.RouteId == scheduledRouteItem.RouteId).FirstOrDefaultAsync();

        //            if (existingRecord != null)
        //            {
        //                success = await db.UpdateAsync(scheduledRouteItem);
        //            }
        //            else
        //            {
        //                success = await db.InsertAsync(scheduledRouteItem);
        //            }
        //        }

        //        await db.CloseAsync();

        //        return scheduledRoutes;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(InsertOrUpdatetScheduledRoutesDataAsync), ex.StackTrace);
        //        return scheduledRoutes;
        //    }
        //}

        public async Task<ScheduledRoutes> InsertOrUpdatetScheduledRouteDataAsync(ScheduledRoutes scheduledRoute)
        {
            ScheduledRoutes existingRecord = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<ScheduledRoutes>().ConfigureAwait(false);
                existingRecord = await db.Table<ScheduledRoutes>().FirstOrDefaultAsync(a => a.RouteId == scheduledRoute.RouteId).ConfigureAwait(false);
                if (existingRecord != null)
                {
                    await db.UpdateAsync(scheduledRoute).ConfigureAwait(false);
                }
                else
                {
                    await db.InsertAsync(scheduledRoute).ConfigureAwait(false);
                    scheduledRoute = await db.Table<ScheduledRoutes>().FirstOrDefaultAsync(a => a.DeviceRouteId == scheduledRoute.DeviceRouteId).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(InsertOrUpdatetScheduledRouteDataAsync), ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false); db = null;
            }
            return scheduledRoute;
        }

        public async Task<List<ScheduledRoutes>> InsertOrUpdatetDownloadedScheduledRoutesDataAsync(List<ScheduledRoutes> scheduledRoutes)
        {
            int success = 0;
            ScheduledRoutes existingRecord = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<RouteStations>();
                await db.CreateTableAsync<ScheduledRoutes>();
                foreach (ScheduledRoutes scheduledRouteItem in scheduledRoutes)
                {
                    await db.Table<ScheduledRoutes>().DeleteAsync(a => a.DeviceRouteId == scheduledRouteItem.DeviceRouteId);
                    scheduledRouteItem.IsExported = 1;

                    int downloadedRouteId = scheduledRouteItem.RouteId ?? 0;//hold download value before it changes
                    success = await db.InsertAsync(scheduledRouteItem);
                    if (success > 0)
                    {
                        await db.QueryAsync<ScheduledRoutes>($"UPDATE SCHEDULEDROUTES SET ROUTEID={downloadedRouteId} WHERE DEVICEROUTEID='{scheduledRouteItem.DeviceRouteId}'");
                        await db.QueryAsync<RouteStations>($"DELETE FROM ROUTESTATIONS WHERE DEVICEROUTEID = '{scheduledRouteItem.DeviceRouteId}'");
                    }
                }
            }
            catch (Exception ex)
            {
                success = 0;
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(InsertOrUpdatetDownloadedScheduledRoutesDataAsync), ex);
            }
            finally
            {
                await db.CloseAsync(); db = null;
            }
            return scheduledRoutes;
        }

        public async Task InsertOrUpdatetDownloadedRouteStationsDataAsync(List<RouteStations> routeStations)
        {
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<RouteStations>();
                foreach (RouteStations item in routeStations)
                {
                    await db.Table<RouteStations>().DeleteAsync(a => a.DeviceRouteId == item.DeviceRouteId && a.RouteStationId == item.RouteStationId);
                    await db.InsertAsync(item);
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(InsertOrUpdatetDownloadedRouteStationsDataAsync), ex);
            }
            finally
            {
                await db.CloseAsync(); db = null;
            }
        }

        public async Task<bool> InsertOrUpdatetSupplyChainDataAsync(List<SupplyChain> supplyChainsList)
        {
            int success = 0; SupplyChain existingRecord = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<SupplyChain>();
                foreach (var supplyChainItem in supplyChainsList)
                {
                    existingRecord = await (db.Table<SupplyChain>().Where(a => a.supplychainid == supplyChainItem.supplychainid)).FirstOrDefaultAsync();

                    if (existingRecord != null)
                    {
                        success = await db.UpdateAsync(supplyChainItem);
                    }
                    else
                    {
                        success = await db.InsertAsync(supplyChainItem);
                    }
                }
            }
            catch (Exception ex)
            {
                success = 0;
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(InsertOrUpdatetSupplyChainDataAsync), ex);
            }
            finally
            {
                await db.CloseAsync(); db = null;
            }

            return success == 1;
        }

        public async Task<bool> InsertOrUpdatetTerritoryMasterDataAsync(List<TerritoryMaster> territoryMasterList)
        {
            int success = 0;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<TerritoryMaster>();
                foreach (var territoryMasterItem in territoryMasterList)
                {
                    await db.Table<TerritoryMaster>().DeleteAsync(a => a.TerritoryID == territoryMasterItem.TerritoryID);

                    success = await db.InsertAsync(territoryMasterItem);
                }
            }
            catch (Exception ex)
            {
                success = 0;
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "InsertOrUpdatetTerritoryMasterDataAsync", ex);
            }
            finally
            {
                await db.CloseAsync(); db = null;
            }

            return success == 1;
        }

        public async Task<bool> InsertOrUpdatetUserMasterDataAsync(List<UserMaster> userMasterList)
        {
            int success = 0;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<UserMaster>();
                foreach (var userItem in userMasterList)
                {
                    await db.Table<UserMaster>().DeleteAsync(a => a.UserId == userItem.UserId);
                    success = await db.InsertAsync(userItem);
                }
            }
            catch (Exception ex)
            {
                success = 0;
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "InsertOrUpdatetUserMasterDataAsync", ex);
            }
            finally
            {
                await db.CloseAsync(); db = null;
            }

            return success != 0;
        }

        public async Task<bool> InsertOrUpdatetVripMasterDataAsync(List<VripMaster> vripMasterList)
        {
            int success = 0;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<VripMaster>();
                foreach (var vripItem in vripMasterList)
                {
                    await db.Table<VripMaster>().DeleteAsync(a => a.VripID == vripItem.VripID);

                    success = await db.InsertAsync(vripItem);
                }
            }
            catch (Exception ex)
            {
                success = 0;
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(InsertOrUpdatetVripMasterDataAsync), ex);
            }
            finally
            {
                await db.CloseAsync(); db = null;
            }
            return success == 1;
        }

        public async Task<bool> InsertOrUpdateUserTaxStatementDataAsync(UserTaxStatement userTaxStatementItem)
        {
            int success = 0;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.EnableWriteAheadLoggingAsync().ConfigureAwait(false);
                var existingRecord = await (db.Table<UserTaxStatement>().FirstOrDefaultAsync(a => a.DeviceUserTaxStatementID == userTaxStatementItem.DeviceUserTaxStatementID)).ConfigureAwait(false);
                if (existingRecord != null)
                {
                    success = await db.UpdateAsync(userTaxStatementItem).ConfigureAwait(false);
                }
                else
                {
                    success = await db.InsertAsync(userTaxStatementItem).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                success = 0;
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(InsertOrUpdateUserTaxStatementDataAsync), ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }

            return success == 1;
        }

        public async Task<bool> InsertOrUpdateDownloadedUserTaxStatementDataAsync(UserTaxStatement userTaxStatementItem)
        {
            int success = 0;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<UserTaxStatement>();
                userTaxStatementItem.IsExported = 1;

                var existingRecord = await (db.Table<UserTaxStatement>().FirstOrDefaultAsync(a => a.DeviceUserTaxStatementID == userTaxStatementItem.DeviceUserTaxStatementID)).ConfigureAwait(false);
                if (existingRecord != null)
                {
                    success = await db.UpdateAsync(userTaxStatementItem).ConfigureAwait(false);
                }
                else
                {
                    userTaxStatementItem.IsExported = 1;
                    success = await db.InsertAsync(userTaxStatementItem).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                success = 0;
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(InsertOrUpdateDownloadedUserTaxStatementDataAsync), ex);
            }
            finally
            {
                await db.CloseAsync(); db = null;
            }
            return success != 0;
        }

        //public async Task<bool> InsertOrUpdateVripTravelDataAsync(List<VripTravelData> vripTravelData)
        //{
        //    int success = 0;

        //    VripTravelData existingRecord = null;

        //    try
        //    {
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);

        //        foreach (var vripTravelDataItem in vripTravelData)
        //        {
        //            existingRecord = await (db.Table<VripTravelData>().Where(a => a.ProgramID == vripTravelDataItem.ProgramID)).FirstOrDefaultAsync();

        //            if (existingRecord != null)
        //            {
        //                success = await db.UpdateAsync(vripTravelDataItem);
        //            }
        //            else
        //            {
        //                success = await db.InsertAsync(vripTravelDataItem);
        //            }
        //        }

        //        await db.CloseAsync();

        //        return success == 1;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(InsertOrUpdateVripTravelDataAsync), ex.StackTrace);
        //        return false;
        //    }
        //}

        //public async Task<bool> InsertOrUpdateZoneMasterDataAsync(List<ZoneMaster> zoneMasterList)
        //{
        //    int success = 0;

        //    ZoneMaster existingRecord = null;

        //    try
        //    {
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);

        //        foreach (var zoneMasterItem in zoneMasterList)
        //        {
        //            existingRecord = await (db.Table<ZoneMaster>().Where(a => a.ZoneID == zoneMasterItem.ZoneID)).FirstOrDefaultAsync();

        //            if (existingRecord != null)
        //            {
        //                success = await db.UpdateAsync(zoneMasterItem);
        //            }
        //            else
        //            {
        //                success = await db.InsertAsync(zoneMasterItem);
        //            }
        //        }

        //        await db.CloseAsync();

        //        return success == 1;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(InsertOrUpdateZoneMasterDataAsync), ex.StackTrace);
        //        return false;
        //    }
        //}

        public async Task<bool> InsertOrUpdateVripTravelDataAsync(List<VripTravelData> vripTravelData)
        {
            int success = 0;

            VripTravelData existingRecord = null;

            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.EnableWriteAheadLoggingAsync().ConfigureAwait(false);
                foreach (var vripTravelDataItem in vripTravelData)
                {
                    existingRecord = await (db.Table<VripTravelData>().FirstOrDefaultAsync(a => a.ProgramID == vripTravelDataItem.ProgramID)).ConfigureAwait(false);

                    if (existingRecord != null)
                    {
                        success = await db.UpdateAsync(vripTravelDataItem).ConfigureAwait(false);
                    }
                    else
                    {
                        success = await db.InsertAsync(vripTravelDataItem).ConfigureAwait(false);
                    }
                }
            }
            catch (Exception ex)
            {
                success = 0;
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(InsertOrUpdateVripTravelDataAsync), ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return success == 1;
        }

        public async Task<bool> InsertOrUpdateZoneMasterDataAsync(List<ZoneMaster> zoneMasterList)
        {
            int success = 0;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<ZoneMaster>();
                foreach (var zoneMasterItem in zoneMasterList)
                {
                    await db.Table<ZoneMaster>().DeleteAsync(a => a.ZoneID == zoneMasterItem.ZoneID);

                    success = await db.InsertAsync(zoneMasterItem);
                }
            }
            catch (Exception ex)
            {
                success = 0;
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(InsertOrUpdateZoneMasterDataAsync), ex);
            }
            finally
            {
                await db.CloseAsync(); db = null;
            }
            return success == 1;
        }

        #endregion

        //public async Task<List<CategoryMaster>> GetCategoryDataForProductAsync()
        //{
        //    try
        //    {
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);

        //        var data = await db.Table<CategoryMaster>().Where(x => x.CategoryID != -88 && x.CategoryID != 10 && x.CategoryID != 11).ToListAsync();

        //        await db.CloseAsync();

        //        return data.Select(x => new CategoryMaster() { CategoryID = x.CategoryID, CategoryName = x.CategoryName, ERPCategoryId = x.ERPCategoryId }).ToList();
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetCategoryFilterForProductAsync", ex);

        //        return null;
        //    }
        //}
        public async Task<List<CategoryMaster>> GetCategoryDataForProductAsync()
        {
            List<CategoryMaster> categoryMasters = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                var data = await db.Table<CategoryMaster>().Where(x => x.CategoryID != -88 && x.CategoryID != 10 && x.CategoryID != 11).ToListAsync().ConfigureAwait(false);

                categoryMasters = data.Select(x => new CategoryMaster() { CategoryID = x.CategoryID, CategoryName = x.CategoryName, ERPCategoryId = x.ERPCategoryId }).ToList();
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetCategoryDataForProductAsync", ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return categoryMasters;
        }

        //public async Task<List<CategoryMaster>> GetCategoryFiltersDataAsync()
        //{
        //    var query = "SELECT CategoryId, CategoryName, ERPCategoryId  FROM CategoryMaster WHERE CategoryID NOT IN(-88,10,11) AND CategoryId IN (SELECT DISTINCT CatId FROM ProductMaster WHERE UOM='CA' AND IsDeleted=0)";

        //    SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);

        //    var data = await db.QueryAsync<CategoryMaster>(query);

        //    await db.CloseAsync();

        //    return data;
        //}

        public async Task<List<CategoryMaster>> GetCategoriesAsync()
        {
            List<CategoryMaster> data = null;
            var query = " SELECT cm.CategoryId, cm.CategoryName, cm.ERPCategoryId  FROM CategoryMaster cm INNER JOIN ( SELECT DISTINCT CatId FROM ProductMaster WHERE UOM = 'CA' AND IsDeleted = 0 ) as pm ON cm.CategoryId = pm.CatId WHERE cm.CategoryID NOT IN (-88, 10, 11) Order by  cm.CategoryId ASC";
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<CategoryMaster>().ConfigureAwait(false);
                data = await db.QueryAsync<CategoryMaster>(query).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, nameof(GetCategoriesAsync), ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return data;
        }

        public async Task<List<CategoryMaster>> GetFilterCategoriesAsync(string ids)
        {
            List<CategoryMaster> data = null;
            var query = $" SELECT CategoryId, CategoryName, ERPCategoryId  FROM CategoryMaster WHERE CategoryID NOT IN (-88, 10, 11) AND CategoryId IN ({ids}) Order by  CategoryId ASC";

            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<CategoryMaster>().ConfigureAwait(false);
                data = await db.QueryAsync<CategoryMaster>(query).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, nameof(GetFilterCategoriesAsync), ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false); db = null;
            }
            return data;
        }

        public async Task<List<BrandData>> GetFilterBrandsAsync(string ids)
        {
            var query = $"SELECT BrandId, BrandName, ImageFileName, CatId FROM BrandData WHERE BrandId IN ({ids}) AND IsDeleted = 0 AND IsPopOrder != 1 ORDER BY BrandName ASC";

            List<BrandData> data = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<BrandData>().ConfigureAwait(false);
                data = await db.QueryAsync<BrandData>(query);
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, nameof(GetFilterBrandsAsync), ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false); db = null;
            }
            return data;
        }

        public async Task<List<BrandData>> GetBrandsAsync()
        {
            var query = "SELECT BrandId, BrandName, ImageFileName, CatId FROM BrandData WHERE BrandId IN (SELECT DISTINCT BrandId FROM ProductMaster WHERE UOM='CA' ) AND CatId != -99 AND IsPopOrder!=1 AND IsDeleted=0 ORDER BY SortOrder ASC";
            List<BrandData> data = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<BrandData>().ConfigureAwait(false);
                data = await db.QueryAsync<BrandData>(query).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, nameof(GetBrandsAsync), ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false); db = null;
            }
            return data;
        }

        public async Task<List<StyleData>> GetStylesDataAsync()
        {
            var query = "SELECT StyleName, StyleId, CatId FROM StyleData WHERE StyleId IN (SELECT DISTINCT StyleId FROM ProductMaster WHERE UOM='CA' AND IsDeleted=0) AND StyleId != -1 ORDER BY StyleName ASC";

            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);

            var data = await db.QueryAsync<StyleData>(query);

            await db.CloseAsync();

            return data;
        }

        public async Task<List<StyleData>> GetFilterStylesAsync(string ids)
        {
            var query = $"SELECT StyleName, StyleId, CatId FROM StyleData WHERE StyleId IN ({ids}) AND StyleId != -1 ORDER BY StyleName ASC";

            List<StyleData> data = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<StyleData>().ConfigureAwait(false);
                data = await db.QueryAsync<StyleData>(query).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, nameof(GetFilterStylesAsync), ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false); db = null;
            }
            return data;
        }


        //public async Task<List<BrandData>> GetBrandDataForProductAsync()
        //{
        //    try
        //    {
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);

        //        var data = await db.Table<BrandData>().Where(b => b.IsDeleted == 0 && b.IsPopOrder != 1).ToListAsync();

        //        await db.CloseAsync();

        //        return data.Select(x => new BrandData() { ImageFileName = x.ImageFileName, BrandId = x.BrandId, CatId = x.CatId, BrandName = x.BrandName }).ToList();
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetBrandDataForProductAsync", ex);

        //        return null;
        //    }
        //}
        public async Task<List<BrandData>> GetBrandDataForProductAsync()
        {
            List<BrandData> brandDatas = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                var data = await db.Table<BrandData>().Where(b => b.IsDeleted == 0 && b.IsPopOrder != 1)
                    .ToListAsync().ConfigureAwait(false);
                if (data != null)
                    brandDatas = data.Select(x => new BrandData() { ImageFileName = x.ImageFileName, BrandId = x.BrandId, CatId = x.CatId, BrandName = x.BrandName }).ToList();
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetBrandDataForProductAsync", ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return brandDatas;
        }


        public async Task<CustomerMaster> GetUserSelfCustomer(string userCustomername)
        {
            CustomerMaster data = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                data = await db.Table<CustomerMaster>().FirstOrDefaultAsync(x => x.CustomerName.ToLower() == userCustomername.ToLower() && x.DeviceCustomerID != "0-0");
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetUserSelfCustomer), ex);
            }
            finally
            {
                await db.CloseAsync();
            }
            return data;
        }

        //public async Task<List<StyleData>> GetStyleDataForProductAsync()
        //{
        //    try
        //    {
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);

        //        var data = await db.Table<StyleData>().ToListAsync();

        //        await db.CloseAsync();

        //        return data.Select(x => new StyleData() { StyleId = x.StyleId, CatId = x.CatId, StyleName = x.StyleName }).ToList();
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetStyleDataForProductAsync", ex);

        //        return null;
        //    }
        //}

        public async Task<List<StyleData>> GetStyleDataForProductAsync()
        {
            List<StyleData> styleDatas = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                var data = await db.Table<StyleData>().ToListAsync().ConfigureAwait(false);
                styleDatas = data.Select(x => new StyleData() { StyleId = x.StyleId, CatId = x.CatId, StyleName = x.StyleName }).ToList();
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetStyleDataForProductAsync", ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return styleDatas;
        }

        //public async Task<List<ProductMaster>> GetProductMasterData()
        //{
        //    try
        //    {
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);

        //        var data = await db.Table<ProductMaster>().Where(d => d.UOM == "CA" && d.IsDeleted == 0).ToListAsync();

        //        await db.CloseAsync();

        //        return data;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetProductMasterData", ex);

        //        return null;
        //    }
        //}
        public async Task<List<ProductMaster>> GetProductMasterData()
        {
            List<ProductMaster> data = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<ProductMaster>().ConfigureAwait(false);
                data = await db.Table<ProductMaster>().Where(d => d.UOM == "CA" && d.IsDeleted == 0).ToListAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetProductMasterData), ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false); db = null;
            }
            return data;
        }
        public async Task<List<ProductMaster>> GetFilteredSRCProductData(IEnumerable<int> CatIds, IEnumerable<int> BrandIds, IEnumerable<int> StyleIds)
        {
            string subQueryCategory = "AND CatId IN ()";
            string subQueryBrand = "AND CatId IN ()";
            string subQueryStyle = "AND CatId IN ()";

            List<ProductMaster> productsList = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<ProductMaster>().ConfigureAwait(false);
                string queryString = "SELECT ProductId, ProductName, Description, isDistributed, CatId, StyleId, BrandId, ProductType, UOM, isDeleted, SRCHoneySellable, SRCHoneyReturnable, SRCCanIOrder, isTobbaco, DistributionRecordedDate FROM ProductMaster WHERE UOM == 'CA' AND isDeleted = 0 ";

                if (CatIds.Any())
                {
                    subQueryCategory = $" AND CatId IN ({string.Join(",", CatIds)}) ";
                    queryString += subQueryCategory;
                }
                if (BrandIds.Any())
                {
                    subQueryBrand = $" AND BrandId IN ({string.Join(",", BrandIds)}) ";
                    queryString += subQueryBrand;
                }
                if (StyleIds.Any())
                {
                    subQueryStyle = $" AND StyleId IN ({string.Join(",", StyleIds)}) ";
                    queryString += subQueryStyle;
                }

                queryString += "  ORDER BY ProductName";
                productsList = await db.QueryAsync<ProductMaster>(queryString).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetFilteredSRCProductData", ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false); db = null;
            }
            return productsList;
        }


        //public async Task<Dictionary<int, string>> GetRankDictionaryAsync()
        //{
        //    try
        //    {
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);

        //        var table = await db.Table<RankMaster>().ToListAsync();

        //        var dict = table.ToDictionary(key => key.RankID, val => val.Rank);

        //        await db.CloseAsync();

        //        return dict;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetRankDictionaryAsync", ex);

        //        return null;
        //    }
        //}
        public async Task<Dictionary<int, string>> GetRankDictionaryAsync()
        {
            Dictionary<int, string> dict = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                var table = await db.Table<RankMaster>().ToListAsync();

                dict = table.ToDictionary(key => key.RankID, val => val.Rank);
            }
            catch (Exception ex)
            {
                //ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetRankDictionaryAsync", ex);
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetRankDictionaryAsync), ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return dict;
        }

        //public async Task<Dictionary<int, ZoneMaster>> GetZoneMasterDictionary()
        //{
        //    try
        //    {
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);
        //        var table = await db.Table<ZoneMaster>().ToListAsync();
        //        var dict = table.ToDictionary(key => key.ZoneID);
        //        await db.CloseAsync();
        //        return dict;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetZoneMasterDictionary", ex);

        //        return null;
        //    }
        //}
        public async Task<Dictionary<int, ZoneMaster>> GetZoneMasterDictionary()
        {
            Dictionary<int, ZoneMaster> dict = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                var table = await db.Table<ZoneMaster>().ToListAsync().ConfigureAwait(false);
                dict = table.ToDictionary(key => key.ZoneID);
            }
            catch (Exception ex)
            {
                //ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetZoneMasterDictionary", ex);
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetZoneMasterDictionary), ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return dict;
        }

        //public async Task<Dictionary<int, RegionMaster>> GetRegionMasterDictionary()
        //{
        //    try
        //    {
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);
        //        var table = await db.Table<RegionMaster>().ToListAsync();
        //        var dict = table.ToDictionary(key => key.RegionID);
        //        await db.CloseAsync();
        //        return dict;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetRegionMasterDictionary", ex);

        //        return null;
        //    }
        //}
        public async Task<Dictionary<int, RegionMaster>> GetRegionMasterDictionary()
        {
            Dictionary<int, RegionMaster> dict = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<RegionMaster>().ConfigureAwait(false);
                var table = await db.Table<RegionMaster>().ToListAsync().ConfigureAwait(false);
                if (table.Any())
                {
                    dict = new Dictionary<int, RegionMaster>();
                    foreach (var item in table)
                    {
                        if (!dict.ContainsKey(item.RegionID))
                        {
                            dict.Add(item.RegionID, item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetRegionMasterDictionary", ex);
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetRegionMasterDictionary), ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false); db = null;
            }
            return dict;
        }


        /// <summary>
        /// get the dristributors from device ids
        /// </summary>
        /// <param name="deviceIds">comma seprated values</param>
        /// <returns></returns>
        //public async Task<IEnumerable<DistributorMaster>> GetDistributorMastersListAsync()
        //{
        //    try
        //    {
        //        string query = "SELECT * FROM DistributorMaster WHERE DistributorMaster.isDeleted=0";
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);
        //        var table = await db.QueryAsync<DistributorMaster>(query);
        //        await db.CloseAsync();
        //        return table.AsQueryable();
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetDistributorMastersListAsync", ex);
        //        return null;
        //    }
        //}
        public async Task<IEnumerable<DistributorMaster>> GetDistributorMastersListAsync()
        {
            IEnumerable<DistributorMaster> table = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                string query = "SELECT * FROM DistributorMaster WHERE DistributorMaster.isDeleted=0";
                table = await db.QueryAsync<DistributorMaster>(query).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetDistributorMastersListAsync", ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return table;
        }

        public async Task<List<DistributorAssignUser>> GetDistributorAssignUserListAsync()
        {
            List<DistributorAssignUser> table = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                string query = @"SELECT d.CustomerID,d.CustomerName,d.AccountType,d.DistributorID,
                                d.AccountResponsibility,d.Fax,d.Phone,d.EmailID,d.Team,d.TerritoryID,d.Broker,d.AssociaatedInternalSalesGuy,
                                d.AccountClassification,d.Buyer,d.StoreCount,d.SupplyChainID,d.ManagerName,d.GeneralComments,d.ImportedFrom,d.DeviceCustomerID,d.IsExported,d.PhysicalAddress,
                                d.PhysicalAddressCityID,d.PhysicalAddressStateID,d.PhysicalAddressZipCode,d.MailingAddress,d.MailingAddressCityID,d.MailingAddressStateID,d.MailingAddressZipID,
                                d.ShippingAddress,d.ShippingAddressCityID,d.ShippingAddressStateID,d.ShippingAddressZipCode,d.CreatedDate,d.CreatedBy,d.UpdatedDate,d.UpdatedBy,d.RegionId,d.ZoneId,
                                d.CreatePermanent,d.CustomerNumber,d.CopiedFields,d.Rank,d.isDeleted,d.Latitude,d.Longitude,d.YTD_CurrentYear,d.YTD_LastYear,d.YTD_CasesCurrentYear,d.YTD_CasesLastYear,
                                d.PercentVarianceYTD,d.PercentVarianceMTD,d.AssignUserId,d.StateTobaccoLicense,d.RetailerLicense,d.RetailerSalesTaxCertificate,d.ContactName,d.ContactRole,d.ContactEmail,
                                d.ContactPhone,d.BuyType,d.IsPayer,COALESCE(u.FirstName || ' ' || u.LastName, '') AssignUserName
                                FROM DistributorMaster d 
                                LEFT JOIN UserMaster u 
                                On d.TerritoryId=u.DefTerritoryId 
                                AND u.defTerritoryId>0 AND length(u.PIN)==4 AND u.IsInActive=0 AND u.IsDeleted=0
                                AND u.RoleId NOT IN (6, 5, 7, 17)                                 
                                WHERE d.isDeleted=0 and d.TerritoryId<>0                                
                            ";
                table = await db.QueryAsync<DistributorAssignUser>(query).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                //ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetDistributorAssignUserListAsync", ex);
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetDistributorAssignUserListAsync), ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return table;
        }

        public async Task<DistributorAssignUser> GetDistributorAssignUserAsync(int Id)
        {
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                string query = $@"SELECT d.CustomerID,d.CustomerName,d.AccountType,d.DistributorID,
                                d.AccountResponsibility,d.Fax,d.Phone,d.EmailID,d.Team,d.TerritoryID,d.Broker,d.AssociaatedInternalSalesGuy,
                                d.AccountClassification,d.Buyer,d.StoreCount,d.SupplyChainID,d.ManagerName,d.GeneralComments,d.ImportedFrom,d.DeviceCustomerID,d.IsExported,d.PhysicalAddress,
                                d.PhysicalAddressCityID,d.PhysicalAddressStateID,d.PhysicalAddressZipCode,d.MailingAddress,d.MailingAddressCityID,d.MailingAddressStateID,d.MailingAddressZipID,
                                d.ShippingAddress,d.ShippingAddressCityID,d.ShippingAddressStateID,d.ShippingAddressZipCode,d.CreatedDate,d.CreatedBy,d.UpdatedDate,d.UpdatedBy,d.RegionId,d.ZoneId,
                                d.CreatePermanent,d.CustomerNumber,d.CopiedFields,d.Rank,d.isDeleted,d.Latitude,d.Longitude,d.YTD_CurrentYear,d.YTD_LastYear,d.YTD_CasesCurrentYear,d.YTD_CasesLastYear,
                                d.PercentVarianceYTD,d.PercentVarianceMTD,d.AssignUserId,d.StateTobaccoLicense,d.RetailerLicense,d.RetailerSalesTaxCertificate,d.ContactName,d.ContactRole,d.ContactEmail,
                                d.ContactPhone,d.BuyType,COALESCE(u.FirstName || ' ' || u.LastName, '') AS AssignUserName,s.StateName AS PhysicalAddressStateName
                                FROM DistributorMaster d 
                                LEFT JOIN UserMaster u 
                                ON d.TerritoryId=u.DefTerritoryId
                                LEFT JOIN StateMaster s 
                                ON d.PhysicalAddressStateID=s.StateID
                                AND u.defTerritoryId>0 AND length(u.PIN)==4 AND u.IsInActive=0 AND u.IsDeleted=0
                                AND u.RoleId NOT IN (6, 5, 7, 17)                                 
                                WHERE d.isDeleted=0 AND d.CustomerID={Id}                                
                            ";
                return await db.FindWithQueryAsync<DistributorAssignUser>(query).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetDistributorAssignUserAsync", ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return null;
        }

        public async Task<List<DistributorMaster>> GetDistributorMastersListForNationalAndZoneAndRegionManagers()
        {
            List<DistributorMaster> distributorsList = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                var table = await db.Table<DistributorMaster>().Where(x => x.isDeleted == 0).ToListAsync().ConfigureAwait(false);
                distributorsList = table.Select(x => new DistributorMaster
                {
                    CustomerID = x.CustomerID,
                    CustomerNumber = x.CustomerNumber
                }).ToList();
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetDistributorMastersListForNationalAndZM", ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return distributorsList;
        }

        //public async Task<IEnumerable<CustomerDistributor>> GetCustomerDistributorMasterAsync()
        //{
        //    try
        //    {
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);
        //        var table = await db.Table<CustomerDistributor>().Where(x => x.IsDeleted == 0).ToListAsync();
        //        await db.CloseAsync();
        //        return table.OrderBy(x => x.DistributorPriority);
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetCustomerDistributorMasterAsync", ex);

        //        return null;
        //    }
        //}
        public async Task<List<CustomerDistributor>> GetCustomerDistributorMasterAsync()
        {
            List<CustomerDistributor> table = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                table = await db.Table<CustomerDistributor>().Where(x => x.IsDeleted == 0).ToListAsync();
            }
            catch (Exception ex)
            {
                //ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetCustomerDistributorMasterAsync", ex);
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "GetCustomerDistributorMasterAsync", ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return table != null ? table.OrderBy(x => x.DistributorPriority).ToList() : table;
        }

        public async Task<IEnumerable<CustomerDistributor>> GetCustomerDistributorMasterToUploadAsync()
        {
            IEnumerable<CustomerDistributor> table = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<CustomerDistributor>().ConfigureAwait(false);
                table = await db.Table<CustomerDistributor>()
                    .Where(x => x.IsExported == 0).ToListAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetCustomerDistributorMasterToUploadAsync", ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
                db = null;
            }
            return table;
        }

        //public async Task<VripTravelData> GetVripTravelDataForPrgramType(string type)
        //{
        //    try
        //    {
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);
        //        var table = await db.Table<VripTravelData>().Where(x => x.ProgramType.Equals(type)).ToListAsync();
        //        await db.CloseAsync();
        //        return table.FirstOrDefault();
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetVripTravelDataForPrgramType", ex);

        //        return null;
        //    }
        //}
        public async Task<VripTravelData> GetVripTravelDataForPrgramType(string type)
        {
            VripTravelData data = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                data = await db.Table<VripTravelData>().FirstOrDefaultAsync(x => x.ProgramType.Equals(type)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetVripTravelDataForPrgramType), ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return data;
        }

        //public async Task<IEnumerable<ContactMaster>> GetContactsForCustomer(int customerId)
        //{
        //    try
        //    {
        //        var stringCustomerId = customerId.ToString();
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);
        //        var table = await db.Table<ContactMaster>().
        //            Where(x => x.DeviceCustomerID.Equals(stringCustomerId) && x.IsDeleted == 0).
        //            ToListAsync();
        //        await db.CloseAsync();
        //        return table.OrderByDescending(x => x.UpdatedDate).Where(x => !string.IsNullOrWhiteSpace(x.ContactName) || !string.IsNullOrWhiteSpace(x.FirstName) || !string.IsNullOrWhiteSpace(x.ContactEmail) || !string.IsNullOrWhiteSpace(x.ContactPhone) || !string.IsNullOrWhiteSpace(x.ContactFax) || !string.IsNullOrWhiteSpace(x.ContactCellPhone));
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetContactsForCustomer", ex);

        //        return null;
        //    }
        //}

        public async Task<IEnumerable<ContactMaster>> GetContactsForCustomer(string customerId)
        {
            IEnumerable<ContactMaster> contactMasters = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            string selectQuery = string.Format("SELECT * FROM ContactMaster WHERE DeviceCustomerID='{0}' AND IsDeleted=0 AND LENGTH(IFNULL(ContactName,''))>0 AND (IFNULL(FirstName,''))>0  AND (IFNULL(ContactEmail,''))>0 AND (IFNULL(ContactPhone,''))>0 AND (IFNULL(ContactFax,''))>0  AND (IFNULL(ContactCellPhone,''))>0",
                customerId);
            try
            {
                var tabe = await db.QueryAsync<ContactMaster>(selectQuery).ConfigureAwait(false);
                if (tabe.Any())
                {
                    contactMasters = tabe.OrderByDescending(x =>
                    {
                        DateTimeStyles styles = DateTimeStyles.None;
                        DateTime dateResult;
                        if (DateTime.TryParse(x.UpdatedDate, CultureInfo.InvariantCulture, styles, out dateResult))
                        {
                            return dateResult;
                        }
                        else
                        {
                            return default;
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetContactsForCustomer), ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return contactMasters;
        }


        //public async Task<List<ContactMaster>> GetContactsDataAsync()
        //{
        //    try
        //    {
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);

        //        var contactsList = await db.Table<ContactMaster>().Where(x => x.IsExported == 0 && x.IsDeleted == 0).ToListAsync();

        //        await db.CloseAsync();

        //        return contactsList;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetContactsDataAsync", ex);

        //        return null;
        //    }
        //}

        public async Task<List<ContactMaster>> GetContactsDataAsync()
        {
            List<ContactMaster> contactsList = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<ContactMaster>().ConfigureAwait(false);
                contactsList = await db.Table<ContactMaster>()
                    .Where(x => x.IsExported == 0 && x.IsDeleted == 0)
                    .ToListAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetContactsDataAsync", ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
                db = null;
            }
            return contactsList;
        }

        //public async Task<List<CallActivityList>> GetCallActivityDataAsync()
        //{
        //    try
        //    {
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);

        //        var callActivityList = await db.Table<CallActivityList>().Where(x => x.IsExported == 0).ToListAsync();

        //        await db.CloseAsync();

        //        return callActivityList;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetCallActivityDataAsync", ex);

        //        return null;
        //    }
        //}

        public async Task<List<CallActivityList>> GetCallActivityDataAsync()
        {
            List<CallActivityList> callActivityList = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<CallActivityList>().ConfigureAwait(false);
                callActivityList = await db.Table<CallActivityList>()
                    .Where(x => x.IsExported == 0).ToListAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetCallActivityDataAsync", ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
                db = null;
            }
            return callActivityList;
        }

        public async Task<List<CallActivityList>> GetCallActivityDataForNationalAndZoneAndRegionManagers(string territoryIds, bool loadAllData)
        {
            string query;
            if (loadAllData)
            {
                query = string.Format("SELECT * FROM CallActivityList WHERE IsExported !='2' AND isDeleted != '1' AND IsVoided == '0' AND ActivityType NOT like '%AR note%' AND ActivityType NOT like '%ARnote%' AND TerritoryId in ({0})", territoryIds);
            }
            else
            { query = string.Format("SELECT * FROM CallActivityList WHERE IsExported !='2' AND isDeleted != '1' AND IsVoided == '0' AND ActivityType NOT like '%AR note%' AND ActivityType NOT like '%ARnote%' AND TerritoryId in ({0})  order by (substr(CallDate,7,4)||' - '||substr(CallDate,1,2)||' - '||substr(CallDate,4,2)) DESC Limit 1000", territoryIds); }

            List<CallActivityList> callActivityList = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                callActivityList = await db.QueryAsync<CallActivityList>(query).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetCallActivityDataForNationalAndZMUsers", ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false); db = null;
            }
            return callActivityList;
        }

        public async Task<List<CallActivityList>> GetCallActivityDataForAVPManagers(bool loadAllData)
        {
            string query;
            if (loadAllData)
            {
                query = string.Format("SELECT * FROM CallActivityList WHERE IsExported !='2' AND isDeleted != '1' AND IsVoided == '0' AND ActivityType NOT like '%AR note%' AND ActivityType NOT like '%ARnote%'");
            }
            else
            { query = string.Format("SELECT * FROM CallActivityList WHERE IsExported !='2' AND isDeleted != '1' AND IsVoided == '0' AND ActivityType NOT like '%AR note%' AND ActivityType NOT like '%ARnote%'  order by (substr(CallDate,7,4)||' - '||substr(CallDate,1,2)||' - '||substr(CallDate,4,2)) DESC Limit 1000"); }

            List<CallActivityList> callActivityList = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                callActivityList = await db.QueryAsync<CallActivityList>(query).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetCallActivityDataForAVPManagers", ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false); db = null;
            }
            return callActivityList;
        }

        //public async Task<List<ScheduledRoutes>> GetScheduledRoutesDataAsync()
        //{
        //    SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);

        //    var scheduledRouteList = await db.Table<ScheduledRoutes>().Where(x => x.IsExported == 0).ToListAsync();

        //    await db.CloseAsync();

        //    return scheduledRouteList;
        //}

        public async Task<List<ScheduledRoutes>> GetScheduledRoutesDataAsync()
        {
            List<ScheduledRoutes> scheduledRouteList = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<ScheduledRoutes>().ConfigureAwait(false);
                scheduledRouteList = await db.Table<ScheduledRoutes>()
                    .Where(x => x.IsExported == 0).ToListAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetScheduledRoutesDataAsync", ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
                db = null;
            }
            return scheduledRouteList;
        }

        //public async Task<List<UserTaxStatement>> GetUserTaxStatementsDataAsync()
        //{
        //    try
        //    {
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);

        //        var userTaxStatementList = await db.Table<UserTaxStatement>().Where(x => x.IsExported == 0).ToListAsync();

        //        await db.CloseAsync();

        //        return userTaxStatementList;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetUserTaxStatementsDataAsync", ex);
        //        return null;
        //    }
        //}

        public async Task<List<UserTaxStatement>> GetUserTaxStatementsDataAsync()
        {
            List<UserTaxStatement> userTaxStatementList = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<UserTaxStatement>().ConfigureAwait(false);
                userTaxStatementList = await db.Table<UserTaxStatement>()
                    .Where(x => x.IsExported == 0).ToListAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetUserTaxStatementsDataAsync", ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
                db = null;
            }
            return userTaxStatementList;
        }

        //public async Task<List<OrderDetail>> GetOrderDetailsDataAsync(string deviceorderId)
        //{
        //    try
        //    {
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);

        //        var orderDetails = await db.Table<OrderDetail>().Where(x => x.DeviceOrderID == deviceorderId).ToListAsync();

        //        await db.CloseAsync();

        //        return orderDetails;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetOrderDetailsDataAsync", ex);
        //        return null;
        //    }
        //}
        public async Task<List<OrderDetail>> GetOrderDetailsDataAsync(string deviceorderId)
        {
            List<OrderDetail> orderDetails = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<OrderDetail>().ConfigureAwait(false);
                orderDetails = await db.Table<OrderDetail>()
                   .Where(x => x.DeviceOrderID == deviceorderId).ToListAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetOrderDetailsDataAsync), ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false); db = null;
            }
            return orderDetails;
        }

        //public async Task<List<OrderDetail>> GetOrderDetailsAsIdAndTotalAsync(IEnumerable<int> orderId)
        //{
        //    try
        //    {
        //        // var format = string.Format("SELECT OD.DeviceOrderID, sum(OD.Quantity) as TotalQuantity FROM OrderDetail OD WHERE OrderId in ({0}) GROUP by DeviceOrderID", string.Join(",", orderId));
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);
        //        // List<OrderDetailGroupedTotal> data = await db.QueryAsync<OrderDetailGroupedTotal>(format);


        //        var deviceorderidquery = string.Format("SELECT deviceorderid FROM ORDERMASTER  WHERE OrderId in ({0})", string.Join(",", orderId));
        //        SQLiteAsyncConnection dbdvcord = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);
        //        List<OrderDetail> deviceorder = await dbdvcord.QueryAsync<OrderDetail>(deviceorderidquery);
        //        await dbdvcord.CloseAsync();

        //        List<string> deviceorderids = deviceorder.Select(x => x.DeviceOrderID).ToList();




        //        var format11 = string.Format("SELECT OD.DeviceOrderID, OD.ProductId, sum(OD.Quantity) as Quantity FROM OrderDetail OD WHERE deviceorderid in ('{0}') GROUP by DeviceOrderID", string.Join("','", deviceorderids));
        //        var data11 = await db.QueryAsync<OrderDetail>(format11);

        //        await db.CloseAsync();
        //        return data11;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "GetOrderDetailsAsIdAndTotalAsync", ex.StackTrace);
        //        return null;
        //    }
        //}
        public async Task<List<OrderDetail>> GetOrderDetailsAsIdAndTotalAsync(IEnumerable<int> orderId)
        {
            List<OrderDetail> data11 = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                var deviceorderidquery = string.Format("SELECT deviceorderid FROM ORDERMASTER  WHERE OrderId in ({0})", string.Join(",", orderId));
                SQLiteAsyncConnection dbdvcord = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);
                List<OrderDetail> deviceorder = await dbdvcord.QueryAsync<OrderDetail>(deviceorderidquery);

                List<string> deviceorderids = deviceorder.Select(x => x.DeviceOrderID).ToList();

                var format11 = string.Format("SELECT OD.DeviceOrderID, OD.ProductId, sum(OD.Quantity) as Quantity FROM OrderDetail OD WHERE deviceorderid in ('{0}') GROUP by DeviceOrderID", string.Join("','", deviceorderids));
                data11 = await db.QueryAsync<OrderDetail>(format11);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "GetOrderDetailsAsIdAndTotalAsync", ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return data11;
        }


        //public async Task<List<OrderMaster>> GetOrderMastersDataAsync()
        //{
        //    try
        //    {
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);

        //        var orderMasters = await db.Table<OrderMaster>().Where(x => x.IsExported == 0).ToListAsync();

        //        await db.CloseAsync();

        //        return orderMasters;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetOrderMastersDataAsync", ex);
        //        return null;
        //    }
        //}

        public async Task<List<OrderMaster>> GetOrderMastersDataAsync()
        {
            List<OrderMaster> orderMasters = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<OrderMaster>().ConfigureAwait(false);
                orderMasters = await db.Table<OrderMaster>()
                    .Where(x => x.IsExported == 0).ToListAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetOrderMastersDataAsync", ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
                db = null;
            }
            return orderMasters;
        }

        public async Task<List<OrderMaster>> GetOrderMastersIdListForNationalAndZoneAndRegionManagers()
        {
            string query = "SELECT DeviceOrderID, CustomerDistributorID FROM OrderMaster";
            List<OrderMaster> orderMasters = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                orderMasters = await db.QueryAsync<OrderMaster>(query).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetUserFromUserNameAndPin", ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return orderMasters;
        }

        //public async Task<Dictionary<int, string>> GetPositionDictionaryAsync()
        //{
        //    try
        //    {
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);
        //        var table = await db.Table<PositionMaster>().ToListAsync();
        //        await db.CloseAsync();
        //        return table.OrderBy(x => x.Position).ToDictionary(x => x.PositionID, y => y.Position);
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetPositionDictionaryAsync", ex);
        //        return null;
        //    }
        //}
        public async Task<Dictionary<int, string>> GetPositionDictionaryAsync()
        {
            Dictionary<int, string> positions = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                var table = await db.Table<PositionMaster>().ToListAsync().ConfigureAwait(false);
                if (table != null && table?.Count > 0)
                    positions = table.OrderBy(x => x.Position).ToDictionary(x => x.PositionID, y => y.Position);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetPositionDictionaryAsync), ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return positions;

        }

        //public async Task<IEnumerable<UserMaster>> GetUserDataFromRoleId(int roleId)
        //{
        //    try
        //    {
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);
        //        var table = await db.Table<UserMaster>().Where(x => x.RoleID == roleId).ToListAsync();
        //        await db.CloseAsync();
        //        return table;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetUserDataFromRoleId", ex);
        //        return null;
        //    }
        //}
        public async Task<IEnumerable<UserMaster>> GetUserDataFromRoleId(int roleId)
        {
            IEnumerable<UserMaster> table = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                table = await db.Table<UserMaster>().Where(x => x.RoleID == roleId).ToListAsync();
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetUserDataFromRoleId), ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return table;
        }

        public async Task<UserMaster> GetLoggedInUserData(int userId)
        {
            UserMaster userInformation = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<UserMaster>().ConfigureAwait(false);
                userInformation = await db.Table<UserMaster>()
                    .FirstOrDefaultAsync(x => x.UserId == userId)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetLoggedInUserData), ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false); db = null;
            }
            return userInformation;
        }

        public async Task<List<UserMaster>> GetUserMasterData()
        {
            List<UserMaster> userInformation = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                var table = await db.Table<UserMaster>().ToListAsync().ConfigureAwait(false);
                userInformation = table.Select(x => new UserMaster()
                {
                    UserId = x.UserId,
                    UserName = x.UserName,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    TerritoryID = x.TerritoryID,
                    CreatedDate = x.CreatedDate
                }).ToList();
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetUserFromUserNameAndPin", ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return userInformation;

        }

        public async Task<UserMaster> GetUserFromUserNameAndPin(string userName, string pin)
        {
            int.TryParse(pin, out int pin_int);
            UserMaster userMaster = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                userMaster = await db.Table<UserMaster>().FirstOrDefaultAsync(x => x.UserName.ToLower().Equals(userName.ToLower()) && x.PIN == pin_int)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetUserFromUserNameAndPin", ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return userMaster;
        }

        //public async Task<ICollection<TerritoryMaster>> GetTerritoryMasterDataAsync()
        //{
        //    SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);
        //    var territories = await db.Table<TerritoryMaster>().ToListAsync();
        //    await db.CloseAsync();
        //    return territories;

        //}
        public async Task<ICollection<TerritoryMaster>> GetTerritoryMasterDataAsync()
        {
            ICollection<TerritoryMaster> territories = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                territories = await db.Table<TerritoryMaster>().ToListAsync();
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetTerritoryMasterDataAsync), ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return territories;
        }

        //public async Task<ICollection<TerritoryMaster>> GetAVPTerritoriesAsync(int avpId)
        //{
        //    ICollection<TerritoryMaster> territories = null;
        //    SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
        //    try
        //    {
        //        string userMasterQuery = $"SELECT UM.* FROM ZoneMaster ZM JOIN UserMaster UM ON ZM.ZoneId = UM.ZoneId WHERE ZM.AVPID = {avpId} AND UM.RoleID = 3";
        //        var zoneManagers = await db.QueryAsync<UserMaster>(userMasterQuery).ConfigureAwait(false);

        //        var territoryIds = zoneManagers
        //                            .SelectMany(x => x.TerritoryID.Split(','))
        //                            .Select(x => x.Trim())
        //                            .Distinct()
        //                            .ToList();
        //        var territoryIdsString = string.Join(",", territoryIds);

        //        var territoryQuery = $"SELECT * FROM TerritoryMaster WHERE TerritoryId IN ({territoryIdsString})";

        //        territories = await db.QueryAsync<TerritoryMaster>(territoryQuery).ConfigureAwait(false);
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetAVPTerritoriesAsync), ex);
        //    }
        //    finally
        //    {
        //        await db.CloseAsync().ConfigureAwait(false);
        //    }
        //    return territories;
        //}

        public async Task<ICollection<TerritoryMaster>> GetAVPTerritoriesAsync(int avpId)
        {
            ICollection<TerritoryMaster> territories = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                string query = $"SELECT TM.* FROM TerritoryMaster TM JOIN RegionMaster RM ON RM.RegionId = TM.RegionID JOIN ZoneMaster ZM ON ZM.ZoneID = RM.ZoneID WHERE TM.ISACTIVE=1 AND TM.isDeleted=0 AND ZM.AVPID = {avpId}";
                territories = await db.QueryAsync<TerritoryMaster>(query).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetAVPTerritoriesAsync), ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return territories;
        }

        public async Task<ICollection<TerritoryMaster>> GetBDTerritoriesAsync(int bdId)
        {
            ICollection<TerritoryMaster> territories = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                string query = $"SELECT TM.* FROM TerritoryMaster TM WHERE  TM.ISACTIVE=1 AND TM.isDeleted=0 AND TM.BDID = {bdId}";
                territories = await db.QueryAsync<TerritoryMaster>(query).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetBDTerritoriesAsync), ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return territories;
        }

        public async Task<ICollection<TerritoryMaster>> GetBDApproverTerritoriesAsync(int bdId, int regionId)
        {
            ICollection<TerritoryMaster> territories = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                string query = null;
                var bdMaster = await db.Table<BDMaster>().FirstOrDefaultAsync(x => x.BDID == bdId && x.Approver == 1).ConfigureAwait(false);
                if (bdMaster != null)
                {
                    var rmUser = await db.Table<UserMaster>().FirstOrDefaultAsync(x => x.RegionId == regionId
                    && x.IsInActive == 0 && x.IsDeleted == 0 && x.RoleID == 2).ConfigureAwait(false);
                    if (rmUser != null)
                    {
                        var territoryIds = rmUser.TerritoryID.Split(',')
                            .Where(x => x.Trim() != rmUser.defterritoryid.ToString())
                            .Select(x => x.Trim()).Distinct().ToList();
                        var territoryIdsString = string.Join(",", territoryIds);
                        query = $"SELECT TM.* FROM TerritoryMaster TM WHERE TM.TerritoryId IN ({territoryIdsString})";
                    }
                }
                if (string.IsNullOrEmpty(query))
                {
                    query = $"SELECT TM.* FROM TerritoryMaster TM WHERE TM.BDID = {bdId}";
                }
                territories = await db.QueryAsync<TerritoryMaster>(query).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetBDApproverTerritoriesAsync), ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return territories;
        }

        public async Task<bool> DeleteCartItem(int productId, string deviceOrderId)
        {
            int success = 0;
            string deleteQuery = String.Format("DELETE FROM OrderDetail WHERE ProductId={0} AND DeviceOrderID='{1}'", productId, deviceOrderId);
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                success = await db.ExecuteAsync(deleteQuery).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(GetType().Name, "DeleteCartItem", ex.StackTrace);
                success = 0;
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }

            return success > 0;
        }

        //public async Task<bool> DeleteCartDataOnPlaceOrder(string deviceOrderId)
        //{
        //    try
        //    {
        //        int success = 0;

        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);

        //        var records = await db.Table<OrderDetail>().Where(a => a.DeviceOrderID == deviceOrderId).ToListAsync();

        //        foreach (var item in records)
        //        {
        //            success = await db.DeleteAsync(item);
        //        }

        //        await db.CloseAsync();

        //        return success == 1;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(DeleteCartDataOnPlaceOrder), ex.StackTrace);
        //        return false;
        //    }
        //}
        public async Task<bool> DeleteCartDataOnPlaceOrder(string deviceOrderId)
        {
            int success = 0;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                var records = await db.Table<OrderDetail>().Where(a => a.DeviceOrderID == deviceOrderId)
                    .ToListAsync().ConfigureAwait(false);
                foreach (var item in records)
                {
                    success = await db.DeleteAsync(item);
                }
            }
            catch (Exception ex)
            {
                success = 0;
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(DeleteCartDataOnPlaceOrder), ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return success == 1;
        }

        public async Task<ProductAdditionalDocument> GetProductAdditionalDocumentsAsync(int productId)
        {
            ProductAdditionalDocument prdouctAddtionalDocData = null;

            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                prdouctAddtionalDocData = await db.Table<ProductAdditionalDocument>()
                .FirstOrDefaultAsync(a => a.ProductID == productId).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetProductAdditionalDocumentsAsync), ex, productId);
            }
            return prdouctAddtionalDocData;
        }

        //public async Task<ICollection<CustomerDocument>> GetCustomerDocumentsAsync(int customerId)
        //{
        //    try
        //    {
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);

        //        var table = await db.Table<CustomerDocument>().Where(x => x.CustomerID == customerId && x.IsDelete.Equals("0")).ToListAsync();

        //        await db.CloseAsync();

        //        return table;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetCustomerDocumentsAsync), ex.StackTrace);
        //        return null;
        //    }
        //}
        public async Task<ICollection<CustomerDocument>> GetCustomerDocumentsAsync(int customerId)
        {
            ICollection<CustomerDocument> table = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                table = await db.Table<CustomerDocument>()
                    .Where(x => x.CustomerID == customerId && (x.IsDelete.Equals("0") || x.IsDelete.Equals("false")))
                    .ToListAsync()
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetCustomerDocumentsAsync), ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return table;
        }

        //public async Task<bool> DeleteExistingCustomerData(CustomerMaster customer)
        //{
        //    SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);

        //    await db.Table<CustomerMaster>().Where(a => a.DeviceCustomerID == customer.DeviceCustomerID).DeleteAsync();

        //    await db.CloseAsync();

        //    return true;
        //}

        public async Task<bool> DeleteExistingCustomerData(CustomerMaster customer)
        {
            bool success = true;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<CustomerMaster>().ConfigureAwait(false);
                await db.Table<CustomerMaster>().DeleteAsync(a => a.DeviceCustomerID == customer.DeviceCustomerID).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                success = false;
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(DeleteExistingCustomerData), ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false); db = null;
            }

            return success;
        }

        public async Task<bool> RemoveCustomer(int customerId)
        {
            int affectedRow = 0;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<CustomerMaster>();
                affectedRow = await db.Table<CustomerMaster>().DeleteAsync(a => a.CustomerID == customerId);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(RemoveCustomer), ex);
            }
            finally
            {
                await db.CloseAsync(); db = null;
            }

            return affectedRow > 0;
        }

        public async Task<bool> DeleteExistingOrderMasterData(OrderMaster order)
        {
            bool success = true;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<OrderMaster>().ConfigureAwait(false);
                await db.Table<OrderMaster>().DeleteAsync(a => a.DeviceOrderID == order.DeviceOrderID).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                success = false;
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(DeleteExistingOrderMasterData), ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }

            return success;
        }

        //public async Task<bool> DeleteExistingUserTaxStatementData(UserTaxStatement taxStatement)
        //{
        //    SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);

        //    await db.Table<UserTaxStatement>().Where(a => a.DeviceUserTaxStatementID == taxStatement.DeviceUserTaxStatementID).DeleteAsync();

        //    await db.CloseAsync();

        //    return true;
        //}

        public async Task<bool> DeleteExistingUserTaxStatementData(UserTaxStatement taxStatement)
        {
            bool success = true;

            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.Table<UserTaxStatement>().DeleteAsync(a => a.DeviceUserTaxStatementID == taxStatement.DeviceUserTaxStatementID).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                success = false;
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(DeleteExistingUserTaxStatementData), ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }

            return success;
        }

        public async Task<bool> DeleteExistingCallActivityData(CallActivityList callActivity)
        {
            bool success = true;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<CallActivityList>().ConfigureAwait(false);
                await db.Table<CallActivityList>().DeleteAsync(a => a.CallActivityDeviceID == callActivity.CallActivityDeviceID).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                success = false;
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(DeleteExistingCallActivityData), ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false); db = null;
            }
            return success;
        }

        //public async Task<bool> DeleteExistingContactData(ContactMaster contact)
        //{
        //    SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);

        //    await db.Table<ContactMaster>().Where(a => a.DeviceContactID == contact.DeviceContactID).DeleteAsync();

        //    await db.CloseAsync();

        //    return true;
        //}

        public async Task<bool> DeleteExistingContactData(ContactMaster contact)
        {
            bool success = true;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.Table<ContactMaster>().DeleteAsync(a => a.DeviceContactID == contact.DeviceContactID).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                success = false;
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(DeleteExistingContactData), ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }

            return success;
        }

        //public async Task<bool> DeleteExistingCustomerDistributorData(CustomerDistributor customerDistributor)
        //{
        //    SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);

        //    await db.Table<CustomerDistributor>().Where(a => a.DeviceCustomerID == customerDistributor.DeviceCustomerID && a.DistributorID == customerDistributor.DistributorID).DeleteAsync();

        //    await db.CloseAsync();

        //    return true;
        //}

        public async Task<bool> DeleteExistingCustomerDistributorData(CustomerDistributor customerDistributor)
        {
            bool success = true;

            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.Table<CustomerDistributor>().DeleteAsync(a => a.DeviceCustomerID == customerDistributor.DeviceCustomerID && a.DistributorID == customerDistributor.DistributorID).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                success = false;
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(DeleteExistingCustomerDistributorData), ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }

            return success;
        }

        //public async Task<List<TravelUiModel>> GetTravelDataAsync(string year)
        //{
        //    try
        //    {
        //        string queryString = string.Format("SELECT cm.CustomerNumber, cm.CustomerID, cm.CustomerName, tm.Year, tm.NetPoints, tm.Awards, tm.NeededPoint, tm.Next, tm.City, tm.State FROM CustomerMaster cm, TravelMaster tm WHERE tm.CustomerID = cm.CustomerID and tm.Year = '{0}' AND CAST(cm.VripOrTravel as Integer) = 1 AND cm.TerritoryID in (1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,41,42,43,44,45,46,47,48,49,50,51,52,53,54,55,56,57,58,59,60,61,62,63,64,65,66,67,68,69,70,71,72,73,74,75,76,251,252,253,254,255,256,257,258,259,264,265,266,267,268,269,270,271,272,273,274,275,276,277,278,279,280,281,283,284,285,286,287,288,0)", year);

        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);

        //        var travelData = await db.QueryAsync<TravelUiModel>(queryString);

        //        await db.CloseAsync();

        //        return travelData;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetTravelDataAsync), ex.StackTrace);
        //        return null;
        //    }
        //}
        public async Task<List<TravelUiModel>> GetTravelDataAsync(string year)
        {
            string queryString = string.Format("SELECT cm.CustomerNumber, cm.CustomerID, cm.CustomerName, tm.Year, tm.NetPoints, tm.Awards, tm.NeededPoint, tm.Next, tm.City, tm.State FROM CustomerMaster cm, TravelMaster tm WHERE tm.CustomerID = cm.CustomerID and tm.Year = '{0}' AND CAST(cm.VripOrTravel as Integer) = 1 AND cm.TerritoryID in (1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,41,42,43,44,45,46,47,48,49,50,51,52,53,54,55,56,57,58,59,60,61,62,63,64,65,66,67,68,69,70,71,72,73,74,75,76,251,252,253,254,255,256,257,258,259,264,265,266,267,268,269,270,271,272,273,274,275,276,277,278,279,280,281,283,284,285,286,287,288,0)", year);
            List<TravelUiModel> travelData = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                travelData = await db.QueryAsync<TravelUiModel>(queryString);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetTravelDataAsync), ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return travelData;
        }

        //public async Task<List<VripUiModel>> GetVripMasterDataAsync(string year)
        //{
        //    try
        //    {
        //        string queryString = string.Format("SELECT cm.CustomerNumber, cm.CustomerID, cm.CustomerName, vm.Year, vm.Cslyr, vm.Target, vm.Csytd, vm.CSNeeded, vm.Qualified, vm.Rebate FROM CustomerMaster cm, VripMaster vm WHERE vm.CustomerID = cm.CustomerID AND vm.Year = '{0}' AND CAST(cm.VripOrTravel as Integer) = 1 AND cm.TerritoryID in (1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,41,42,43,44,45,46,47,48,49,50,51,52,53,54,55,56,57,58,59,60,61,62,63,64,65,66,67,68,69,70,71,72,73,74,75,76,251,252,253,254,255,256,257,258,259,264,265,266,267,268,269,270,271,272,273,274,275,276,277,278,279,280,281,283,284,285,286,287,288,0)", year);

        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);

        //        var vripData = await db.QueryAsync<VripUiModel>(queryString);

        //        await db.CloseAsync();

        //        return vripData;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetVripMasterDataAsync), ex.StackTrace);
        //        return null;
        //    }
        //}
        public async Task<List<VripUiModel>> GetVripMasterDataAsync(string year)
        {
            List<VripUiModel> vripData = null;
            string queryString = string.Format("SELECT cm.CustomerNumber, cm.CustomerID, cm.CustomerName, vm.Year, vm.Cslyr, vm.Target, vm.Csytd, vm.CSNeeded, vm.Qualified, vm.Rebate FROM CustomerMaster cm, VripMaster vm WHERE vm.CustomerID = cm.CustomerID AND vm.Year = '{0}' AND CAST(cm.VripOrTravel as Integer) = 1 AND cm.TerritoryID in (1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,41,42,43,44,45,46,47,48,49,50,51,52,53,54,55,56,57,58,59,60,61,62,63,64,65,66,67,68,69,70,71,72,73,74,75,76,251,252,253,254,255,256,257,258,259,264,265,266,267,268,269,270,271,272,273,274,275,276,277,278,279,280,281,283,284,285,286,287,288,0)", year);
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                vripData = await db.QueryAsync<VripUiModel>(queryString).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetVripMasterDataAsync), ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return vripData;

        }


        //public async Task<List<VripTravelData>> GetTravelProgramYearFromVripTravelDataAsync()
        //{
        //    try
        //    {
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);
        //        var programYearData = await db.Table<VripTravelData>().Where(a => a.ProgramType == "Travel").ToListAsync();
        //        await db.CloseAsync();
        //        return programYearData;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetTravelProgramYearFromVripTravelDataAsync), ex.StackTrace);
        //        return null;
        //    }

        //}
        public async Task<List<VripTravelData>> GetTravelProgramYearFromVripTravelDataAsync()
        {
            List<VripTravelData> programYearData = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                programYearData = await db.Table<VripTravelData>().Where(a => a.ProgramType == "Travel")
                    .ToListAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetTravelProgramYearFromVripTravelDataAsync), ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return programYearData;
        }

        //public async Task<List<VripTravelData>> GetVripProgramYearFromVripTravelDataAsync()
        //{
        //    try
        //    {
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);
        //        var programYearData = await db.Table<VripTravelData>().Where(a => a.ProgramType == "Vrip").ToListAsync();
        //        await db.CloseAsync();
        //        return programYearData;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetVripProgramYearFromVripTravelDataAsync), ex.StackTrace);
        //        return null;
        //    }
        //}
        public async Task<List<VripTravelData>> GetVripProgramYearFromVripTravelDataAsync()
        {
            List<VripTravelData> programYearData = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                programYearData = await db.Table<VripTravelData>().Where(a => a.ProgramType == "Vrip")
                    .ToListAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetVripProgramYearFromVripTravelDataAsync), ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return programYearData;
        }

        //public async Task<List<OrderMaster>> GetOrderMasterDataAsync(string customerId, string customerType)
        //{
        //    try
        //    {
        //        //string query = string.Format("select * from OrderMaster where OrderMaster.DeviceCustomerID = '{0}' AND OrderMaster.IsVoided = 0 and OrderMaster.IsOrderConfirmed = 1 ", customerId);

        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);
        //        var table = new List<OrderMaster>();
        //        if (!string.IsNullOrEmpty(customerType))
        //        {


        //            if (customerType == "2")
        //            {
        //                table = await db.Table<OrderMaster>().Where(a => a.DeviceCustomerID.Equals(customerId) && a.IsVoided == 0 && a.IsOrderConfirmed == 1).ToListAsync();
        //            }
        //            else
        //            {
        //                table = await db.Table<OrderMaster>().Where(a => (a.DeviceCustomerID.Equals(customerId)) || (a.CustomerID.Equals(customerId) && a.SalesType == "7") && a.IsVoided == 0 && a.IsOrderConfirmed == 1).ToListAsync();
        //            }
        //        }
        //        await db.CloseAsync();

        //        return table;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "GetOrderMasterDataAsync", ex.StackTrace);
        //        return null;
        //    }
        //}
        public async Task<List<OrderMaster>> GetOrderMasterDataAsync(string customerId, string customerType)
        {
            var table = new List<OrderMaster>();
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                if (!string.IsNullOrEmpty(customerType))
                {
                    if (customerType == "2")
                    {
                        table = await db.Table<OrderMaster>().Where(a => a.DeviceCustomerID.Equals(customerId) && a.IsVoided == 0 && a.IsOrderConfirmed == 1).ToListAsync();
                    }
                    else
                    {
                        table = await db.Table<OrderMaster>().Where(a => (a.DeviceCustomerID.Equals(customerId)) || (a.CustomerID.Equals(customerId) && a.SalesType == "7") && a.IsVoided == 0 && a.IsOrderConfirmed == 1).ToListAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetOrderMasterDataAsync), ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return table;
        }

        //public async Task<IEnumerable<ItemHistoryProductModel>> GetProductsForItemHistory(IEnumerable<int> ids)
        //{
        //    try
        //    {
        //        string query = string.Format("SELECT PM.ProductID, PM.ProductName, PM.Description ,PM.UOM from ProductMaster PM WHERE PM.ProductID in ({0})", string.Join(",", ids));
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);
        //        ///var table = await db.Table<OrderMaster>().Where(x => x.CustomerID == customerId && x.IsOrderConfirmed == 1 && x.IsVoided == 0).ToListAsync();
        //        var table = await db.QueryAsync<ItemHistoryProductModel>(query);
        //        await db.CloseAsync();
        //        return table;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetProductsForItemHistory), ex.StackTrace);
        //        return null;
        //    }
        //}

        public async Task<IEnumerable<ItemHistoryProductModel>> GetProductsForItemHistory(IEnumerable<int> ids)
        {
            IEnumerable<ItemHistoryProductModel> table = null;
            string query = string.Format("SELECT PM.ProductID, PM.ProductName, PM.Description ,PM.UOM from ProductMaster PM WHERE PM.ProductID in ({0})", string.Join(",", ids));
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                table = await db.QueryAsync<ItemHistoryProductModel>(query).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetProductsForItemHistory), ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return table;
        }

        //public async Task<IEnumerable<OrderDetail>> GetOrderDetailsInOrderId(IEnumerable<int> orderId)
        //{
        //    try
        //    {
        //        // fetch data from order master to retrieve deviceorderid. this is done to fix the issue where the data for order ,aster and detail was mimatch.

        //        var deviceorderidquery = string.Format("SELECT deviceorderid FROM ORDERMASTER  WHERE OrderId in ({0})", string.Join(",", orderId));
        //        SQLiteAsyncConnection dbdvcord = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);
        //        List<OrderDetail> deviceorder = await dbdvcord.QueryAsync<OrderDetail>(deviceorderidquery);
        //        await dbdvcord.CloseAsync();

        //        List<string> deviceorderids = deviceorder.Select(x => x.DeviceOrderID).ToList();

        //        var format = string.Format("SELECT * FROM OrderDetail OD WHERE deviceorderid in ('{0}')", string.Join("','", deviceorderids));
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);
        //        var data = await db.QueryAsync<OrderDetail>(format);
        //        await db.CloseAsync();
        //        return data;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "GetOrderDetailsInOrderId", ex.StackTrace);
        //        return null;
        //    }
        //}
        public async Task<IEnumerable<OrderDetail>> GetOrderDetailsInOrderId(IEnumerable<int> orderId)
        {
            IEnumerable<OrderDetail> data = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                var deviceorderidquery = string.Format("SELECT deviceorderid FROM ORDERMASTER  WHERE OrderId in ({0})", string.Join(",", orderId));
                List<OrderDetail> deviceorder = await db.QueryAsync<OrderDetail>(deviceorderidquery).ConfigureAwait(false);

                List<string> deviceorderids = deviceorder.Select(x => x.DeviceOrderID).ToList();

                string format = string.Format("SELECT * FROM OrderDetail OD WHERE deviceorderid in ('{0}') Order By OrderDetailId", string.Join("','", deviceorderids));
                data = await db.QueryAsync<OrderDetail>(format).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "GetOrderDetailsInOrderId", ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return data;
        }


        //public async Task<IEnumerable<OrderDetail>> GetOrderDetailsFromOrderId(string deviceOrderId)
        //{
        //    try
        //    {
        //        var format = string.Format("SELECT OrderDetail.BrandId, OrderDetail.BrandName, OrderDetail.CategoryId, OrderDetail.CategoryName, OrderDetail.CreatedDate, OrderDetail.CreditRequest, OrderDetail.DeviceOrderID, OrderDetail.OrderDetailId, OrderDetail.OrderId, OrderDetail.Price, OrderDetail.ProductId,OrderDetail.Quantity, OrderDetail.StyleId, OrderDetail.StyleName, OrderDetail.Total, OrderDetail.Unit, OrderDetail.UpdatedDate , OrderDetail.isTobbaco ,ProductMaster.Description as ProductDescription, ProductMaster.Description as ProductName from OrderDetail INNER JOIN  ProductMaster on ProductMaster.ProductId= OrderDetail.ProductId WHERE DeviceOrderId ='{0}' ", deviceOrderId);
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);
        //        var data = await db.QueryAsync<OrderDetail>(format);
        //        await db.CloseAsync();
        //        return data;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "GetOrderDetailsInOrderId", ex.StackTrace);
        //        return null;
        //    }
        //}
        public async Task<IEnumerable<OrderDetail>> GetOrderDetailsFromOrderId(string deviceOrderId)
        {
            var format = string.Format("SELECT OrderDetail.BrandId, OrderDetail.BrandName, OrderDetail.CategoryId, OrderDetail.CategoryName, OrderDetail.CreatedDate, OrderDetail.CreditRequest, OrderDetail.DeviceOrderID, OrderDetail.OrderDetailId, OrderDetail.OrderId, OrderDetail.Price, OrderDetail.ProductId,OrderDetail.Quantity, OrderDetail.StyleId, OrderDetail.StyleName, OrderDetail.Total, OrderDetail.Unit, OrderDetail.UpdatedDate , OrderDetail.isTobbaco ,ProductMaster.Description as ProductDescription, ProductMaster.Description as ProductName from OrderDetail INNER JOIN  ProductMaster on ProductMaster.ProductId= OrderDetail.ProductId WHERE DeviceOrderId ='{0}' ", deviceOrderId);
            IEnumerable<OrderDetail> data = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                data = await db.QueryAsync<OrderDetail>(format);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetOrderDetailsInOrderId), ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return data;

        }

        //public async Task<List<PromotionMaster>> GetPromotionsDataAsync(string customerId)
        //{
        //    try
        //    {
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);

        //        var promotionData = await db.Table<PromotionMaster>().Where(x => x.CustomerID.Equals(customerId)).OrderByDescending(s => s.StartDate).ToListAsync();

        //        await db.CloseAsync();

        //        return promotionData;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetPromotionsDataAsync), ex.StackTrace);
        //        return null;
        //    }
        //}
        public async Task<List<PromotionMaster>> GetPromotionsDataAsync(string customerId)
        {
            List<PromotionMaster> promotionData = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                promotionData = await db.Table<PromotionMaster>()
                    .Where(x => x.CustomerID.Equals(customerId))
                    .OrderByDescending(s => s.StartDate).ToListAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetPromotionsDataAsync), ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return promotionData;
        }

        //public async Task<List<ContractMaster>> GetContractDataAsync(string customerId)
        //{
        //    try
        //    {
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);

        //        var contractData = await db.Table<ContractMaster>().Where(x => x.CustomerID.Equals(customerId)).OrderByDescending(s => s.ContractYear).ToListAsync();

        //        await db.CloseAsync();

        //        return contractData;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetContractDataAsync), ex.StackTrace);
        //        return null;
        //    }
        //}
        public async Task<List<ContractMaster>> GetContractDataAsync(string customerId)
        {
            List<ContractMaster> contractData = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                contractData = await db.Table<ContractMaster>()
                    .Where(x => x.CustomerID.Equals(customerId))
                    .OrderByDescending(s => s.ContractYear).ToListAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetContractDataAsync), ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return contractData;
        }

        //public async Task<CustomerMaster> GetVripDataForCustomerAsync(string customerId)
        //{
        //    try
        //    {
        //        var query = string.Format("SELECT C1.Cslyr, C1.Target, C1.Csytd, C1.CSNeeded, C1.VripYear, C1.TravelYear FROM CustomerMaster C1 INNER JOIN VripTravelData V ON V.ProgramYear = C1.VripYear WHERE C1.DeviceCustomerID = {0} and V.ProgramType='Vrip'", customerId);

        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);

        //        var table = await db.QueryAsync<CustomerMaster>(query);

        //        await db.CloseAsync();

        //        return table.FirstOrDefault();
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetVripDataForCustomerAsync), ex.StackTrace);
        //        return null;
        //    }
        //}
        public async Task<CustomerMaster> GetVripDataForCustomerAsync(string customerId)
        {
            var query = string.Format("SELECT C1.Cslyr, C1.Target, C1.Csytd, C1.CSNeeded, C1.VripYear, C1.TravelYear FROM CustomerMaster C1 INNER JOIN VripTravelData V ON V.ProgramYear = C1.VripYear WHERE C1.DeviceCustomerID = {0} and V.ProgramType='Vrip'", customerId);
            IEnumerable<CustomerMaster> table = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                table = await db.QueryAsync<CustomerMaster>(query).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetVripDataForCustomerAsync), ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return table?.FirstOrDefault();
        }

        //public async Task<CustomerMaster> GetTravelDataForCustomerAsync(string customerId)
        //{
        //    try
        //    {
        //        var query = string.Format("SELECT C1.Qualified, C1.EarnedPoints, C1.BonusPoints, C1.NetPoints, C1.NeededPoint, C1.VripYear, C1.TravelYear, C1.Awards FROM CustomerMaster C1 INNER JOIN VripTravelData V ON V.ProgramYear = C1.TravelYear WHERE C1.DeviceCustomerID = {0} AND V.ProgramType = 'Travel'", customerId);

        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);

        //        var table = await db.QueryAsync<CustomerMaster>(query);

        //        await db.CloseAsync();

        //        return table.FirstOrDefault();
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetTravelDataForCustomerAsync), ex.StackTrace);
        //        return null;
        //    }
        //}
        public async Task<CustomerMaster> GetTravelDataForCustomerAsync(string customerId)
        {
            IEnumerable<CustomerMaster> table = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                var query = string.Format("SELECT C1.Qualified, C1.EarnedPoints, C1.BonusPoints, C1.NetPoints, C1.NeededPoint, C1.VripYear, C1.TravelYear, C1.Awards FROM CustomerMaster C1 INNER JOIN VripTravelData V ON V.ProgramYear = C1.TravelYear WHERE C1.DeviceCustomerID = {0} AND V.ProgramType = 'Travel'", customerId);
                table = await db.QueryAsync<CustomerMaster>(query).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetTravelDataForCustomerAsync), ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return table?.FirstOrDefault();
        }
        //public async Task<OrderMaster> GetOrderMasterFromId(string id)
        //{
        //    try
        //    {
        //        string query = string.Format("SELECT * from OrderMaster WHERE OrderMaster.DeviceOrderId='{0}'", id);
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);
        //        var table = await db.QueryAsync<OrderMaster>(query);
        //        await db.CloseAsync();
        //        return table.FirstOrDefault();
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetOrderMasterFromId), ex.StackTrace);
        //        return null;
        //    }
        //}
        public async Task<OrderMaster> GetOrderMasterFromId(string id)
        {
            OrderMaster table = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                table = await db.Table<OrderMaster>().FirstOrDefaultAsync(x => x.DeviceOrderID == id).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetOrderMasterFromId), ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return table;
        }

        public async Task<IEnumerable<ProductMaster>> GetProductMasterDataForOrderHistory()
        {
            ///and SRCHoneySellable!=0 and SRCHoneyReturnable!=0 and SRCCanIOrder!=0
            string query = "SELECT * from ProductMaster";
            IEnumerable<ProductMaster> table = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                table = await db.QueryAsync<ProductMaster>(query).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetOrderMasterFromId), ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return table;
        }

        public async Task<IEnumerable<BrandData>> GetBrandDataForOrderHistory()
        {
            IEnumerable<BrandData> table = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                table = await db.Table<BrandData>().ToListAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetBrandDataForOrderHistory), ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return table;
        }

        public async Task<IEnumerable<StyleData>> GetStyleDataForOrderHistory()
        {
            IEnumerable<StyleData> table = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                table = await db.Table<StyleData>().ToListAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetStyleDataForOrderHistory", ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return table;
        }

        public async Task<IEnumerable<CategoryMaster>> GetCategoryDataForOrderHistory()
        {
            IEnumerable<CategoryMaster> table = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                table = await db.Table<CategoryMaster>().ToListAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetStyleDataForOrderHistory", ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return table;
        }

        //public async Task<bool> DeleteOrderDetailFromDetailsOrderId(OrderDetail orderDetail)
        //{
        //    try
        //    {
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);
        //        var isDel = await db.DeleteAsync(orderDetail);
        //        await db.CloseAsync();
        //        return isDel == 1;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "DeleteOrderDetailFromDetailsOrderId", ex.StackTrace);
        //        return false;
        //    }
        //}
        public async Task<bool> DeleteOrderDetailFromDetailsOrderId(OrderDetail orderDetail)
        {
            int isDel = 0;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                isDel = await db.DeleteAsync(orderDetail);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(DeleteOrderDetailFromDetailsOrderId), ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return isDel > 0;
        }

        public async Task<bool> DeleteOrderDetailFromOrderDetailIdPostSync(OrderDetail orderDetail)
        {
            bool success = true;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.Table<OrderDetail>().DeleteAsync(a => a.DeviceOrderID == orderDetail.DeviceOrderID);
            }
            catch (Exception ex)
            {
                success = false;
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(DeleteOrderDetailFromOrderDetailIdPostSync), ex);
            }
            finally
            {
                await db.CloseAsync(); db = null;
            }
            return success;
        }
        public async Task<List<RackOrderListModel>> GetRackOrderListAsync()
        {
            var query = "SELECT pd.DocumentFileName as DocumentFileName, pd.DocumentType as DocumentType, lk.ProductID, br.* from BrandData br JOIN LnkRackItems lk ON br.BrandID= lk.BrandID LEFT JOIN ProductAdditionalDocument pd on pd.productid= lk.productid WHERE br.CatID= -88 and br.IsDeleted= 0 and lk.IsDeleted= 0 and lk.SortOrder= 0 and lk.Status= 1 order by br.SortOrder";
            List<RackOrderListModel> table = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                table = await db.QueryAsync<RackOrderListModel>(query).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetRackOrderListAsync), ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return table;
        }

        //public async Task<int> GetCartItemsCountAsync(string currentOrderId)
        //{
        //    try
        //    {
        //        int count = 0;
        //        var query = string.Format("SELECT COUNT(*) from OrderDetail WHERE DeviceOrderID ={0}", currentOrderId);
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);
        //        var table = await db.QueryAsync<OrderDetail>(query);
        //        if (table != null)
        //        {
        //            count = table.Count;
        //        }
        //        return count;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetCartItemsCountAsync), ex.StackTrace);
        //        return 0;
        //    }
        //}
        public async Task<int> GetCartItemsCountAsync(string currentOrderId)
        {
            int count = 0;
            var query = string.Format("SELECT COUNT(*) from OrderDetail WHERE DeviceOrderID ={0}", currentOrderId);
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<OrderDetail>().ConfigureAwait(false);
                count = await db.ExecuteScalarAsync<int>(query).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetCartItemsCountAsync), ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false); db = null;
            }
            return count;
        }

        //public async Task<bool> DeleteAllCartData(string deviceOrderId)
        //{
        //    try
        //    {
        //        int success = 0;
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);
        //        success = await db.Table<OrderDetail>().Where(a => a.DeviceOrderID == deviceOrderId)?.DeleteAsync();
        //        await db.CloseAsync();

        //        return success >= 0;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(DeleteAllCartData), ex.StackTrace);
        //        return false;
        //    }
        //}
        public async Task<bool> DeleteAllCartData(string deviceOrderId)
        {
            int success = 0;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<OrderDetail>().ConfigureAwait(false);
                success = await db.Table<OrderDetail>()
                .DeleteAsync(a => a.DeviceOrderID == deviceOrderId)
                .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "DeleteAllCartData", ex); db = null;
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return success >= 0;
        }

        public async Task<List<OrderDetailUIModel>> GetCartDetailsData(string currentOrderId)
        {
            var queryString = string.Format("SELECT SD.StyleName, SD.StyleId, BD.BrandName, BD.BrandId, PM.CatId as CategoryId, OD.OrderDetailId, OD.DeviceOrderID, OD.OrderId, OD.Total, OD.Price, OD.Quantity, OD.Unit, OD.CreditRequest, OD.ProductId as ProductID, OD.CreatedDate, PM.isTobbaco, PM.Description as ItemDescription, PM.ProductName as ItemNumber FROM OrderDetail OD INNER JOIN ProductMaster PM ON PM.ProductId = OD.ProductId LEFT OUTER JOIN BrandData BD ON BD.BrandId = PM.BrandId INNER JOIN StyleData SD ON SD.Styleid = PM.Styleid WHERE OD.DeviceOrderID='{0}' ORDER by OD.CreatedDate ASC", currentOrderId);
            List<OrderDetailUIModel> orderDataList = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<OrderDetail>().ConfigureAwait(false);
                orderDataList = await db.QueryAsync<OrderDetailUIModel>(queryString).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetCartDetailsData", ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false); db = null;
            }
            return orderDataList;
        }

        //public async Task<List<RackOrderCartItemModel>> GetRackCartItemsAsync(int brandId)
        //{
        //    try
        //    {
        //        var query = string.Format("SELECT pd.DocumentFileName as DocumentFileName, pd.DocumentType as DocumentType, pm.* from ProductMaster pm, (select distinct productid from LnkRackItems where brandid in ({0}) and IsDeleted = 0 and Status = 1 order by brandid,SortOrder ) lri left join ProductAdditionalDocument pd on pm.productid=pd.productid where pm.productid in (lri.productid) and IsDeleted=0", brandId);

        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);

        //        var table = await db.QueryAsync<RackOrderCartItemModel>(query);

        //        await db.CloseAsync();

        //        return table;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetRackCartItemsAsync), ex.StackTrace);
        //        return null;
        //    }
        //}
        public async Task<List<RackOrderCartItemModel>> GetRackCartItemsAsync(int brandId)
        {
            var query = string.Format("SELECT pd.DocumentFileName as DocumentFileName, pd.DocumentType as DocumentType, pm.* from ProductMaster pm, (select distinct productid from LnkRackItems where brandid in ({0}) and IsDeleted = 0 and Status = 1 order by brandid,SortOrder ) lri left join ProductAdditionalDocument pd on pm.productid=pd.productid where pm.productid in (lri.productid) and IsDeleted=0", brandId);
            List<RackOrderCartItemModel> table = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                table = await db.QueryAsync<RackOrderCartItemModel>(query).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetRackCartItemsAsync), ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return table;
        }
        public async Task<List<OrderDetail>> GetRackCartProductsAsync(string currentOrderId)
        {
            List<OrderDetail> orderDataList = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                orderDataList = await db.Table<OrderDetail>().Where(o => o.DeviceOrderID == currentOrderId).ToListAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetRackCartProductsAsync), ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return orderDataList;
        }

        public async Task<List<PopOrderCartItemModel>> GetPopCartItemsAsync()
        {
            var query = string.Format("SELECT pd.ProductAdditionalDocumentID,pd.DocumentFileName,pd.DocumentType,pm.* from ProductMaster pm left join ProductAdditionalDocument pd on pm.productid=pd.productid where pm.productid in (select productid from LnkPopItems where (Hierarchy1!=0 or Hierarchy2!=0 or Hierarchy3!=0 or Hierarchy4!=0 or Hierarchy5!=0) and IsDeleted=0) and IsDeleted=0");

            List<PopOrderCartItemModel> returnTable = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                var table = await db.QueryAsync<PopOrderCartItemModel>(query);
                returnTable = table.ToList();
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetPopCartItemsAsync), ex);
                return null;
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return returnTable;
        }

        //public async Task<List<PopOrderCartItemModel>> GetRemainingPopCartItemsAsync()
        //{
        //    try
        //    {
        //        var query = string.Format("SELECT pd.ProductAdditionalDocumentID,pd.DocumentFileName,pd.DocumentType,pm.* from ProductMaster pm left join ProductAdditionalDocument pd on pm.productid=pd.productid where pm.productid in (select productid from LnkPopItems where (Hierarchy1!=0 or Hierarchy2!=0 or Hierarchy3!=0 or Hierarchy4!=0 or Hierarchy5!=0) and IsDeleted=0) and IsDeleted=0");

        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);

        //        var table = await db.QueryAsync<PopOrderCartItemModel>(query);

        //        await db.CloseAsync();
        //        var returnTable = table.Skip(25).ToList();

        //        return returnTable;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetPopCartItemsAsync), ex.StackTrace);
        //        return null;
        //    }
        //}
        public async Task<List<PopOrderCartItemModel>> GetRemainingPopCartItemsAsync()
        {
            var query = string.Format("SELECT pd.ProductAdditionalDocumentID,pd.DocumentFileName,pd.DocumentType,pm.* from ProductMaster pm left join ProductAdditionalDocument pd on pm.productid=pd.productid where pm.productid in (select productid from LnkPopItems where (Hierarchy1!=0 or Hierarchy2!=0 or Hierarchy3!=0 or Hierarchy4!=0 or Hierarchy5!=0) and IsDeleted=0) and IsDeleted=0");
            List<PopOrderCartItemModel> table = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<ProductMaster>().ConfigureAwait(false);
                await db.CreateTableAsync<ProductAdditionalDocument>().ConfigureAwait(false);
                await db.CreateTableAsync<LnkPopItems>().ConfigureAwait(false);
                table = await db.QueryAsync<PopOrderCartItemModel>(query).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetRemainingPopCartItemsAsync), ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false); db = null;
            }
            return table;
        }

        //public async Task<List<BrandData>> GetFilteredComboboxData(string hierarchyValues, int HierarchyTag)
        //{
        //    try
        //    {
        //        string formattedquery = string.Empty;
        //        formattedquery = "select distinct Hierarchy" + HierarchyTag + " " + "as BrandId,bd.BrandName,bd.Description from LnkPopItems lnkPOP join BrandData bd on bd.BrandId=lnkPOP.Hierarchy" + HierarchyTag + " " + "and bd.IsDeleted=0 and bd.IsPopOrder=1 " + hierarchyValues + " where lnkPOP.IsDeleted=0";
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);
        //        var data = await db.QueryAsync<BrandData>(formattedquery);
        //        await db.CloseAsync();
        //        return data;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetFilteredComboboxData), ex.StackTrace);
        //        return null;
        //    }
        //}
        public async Task<List<BrandData>> GetFilteredComboboxData(string hierarchyValues, int HierarchyTag)
        {
            string formattedquery = "select distinct Hierarchy" + HierarchyTag + " " + "as BrandId,bd.BrandName,bd.Description from LnkPopItems lnkPOP join BrandData bd on bd.BrandId=lnkPOP.Hierarchy" + HierarchyTag + " " + "and bd.IsDeleted=0 and bd.IsPopOrder=1 " + hierarchyValues + " where lnkPOP.IsDeleted=0";

            List<BrandData> data = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                data = await db.QueryAsync<BrandData>(formattedquery).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetFilteredComboboxData), ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return data;
        }
        //public async Task<List<BrandData>> GetPopOrderMaterialAsync()
        //{
        //    try
        //    {
        //        var query = "select distinct Hierarchy2 as BrandId,bd.BrandName,bd.Description from LnkPopItems lnkPOP join BrandData bd on bd.BrandId=lnkPOP.Hierarchy2 and bd.IsDeleted=0 and bd.IsPopOrder=1  where lnkPOP.IsDeleted=0";

        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);

        //        var data = await db.QueryAsync<BrandData>(query);

        //        await db.CloseAsync();

        //        return data;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetPopOrderMaterialAsync), ex.StackTrace);
        //        return null;
        //    }
        //}
        public async Task<List<BrandData>> GetPopOrderMaterialAsync()
        {
            var query = "select distinct Hierarchy2 as BrandId,bd.BrandName,bd.Description from LnkPopItems lnkPOP join BrandData bd on bd.BrandId=lnkPOP.Hierarchy2 and bd.IsDeleted=0 and bd.IsPopOrder=1  where lnkPOP.IsDeleted=0";
            List<BrandData> data = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                data = await db.QueryAsync<BrandData>(query);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetPopOrderMaterialAsync), ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return data;
        }
        //public async Task<List<BrandData>> GetPopOrderFamilyAsync()
        //{
        //    try
        //    {
        //        var query = "select distinct Hierarchy3 as BrandId,bd.BrandName,bd.Description from LnkPopItems lnkPOP join BrandData bd on bd.BrandId=lnkPOP.Hierarchy3 and bd.IsDeleted=0 and bd.IsPopOrder=1  where lnkPOP.IsDeleted=0";

        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);

        //        var data = await db.QueryAsync<BrandData>(query);

        //        await db.CloseAsync();

        //        return data;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetPopOrderFamilyAsync), ex.StackTrace);
        //        return null;
        //    }
        //}
        public async Task<List<BrandData>> GetPopOrderFamilyAsync()
        {
            var query = "select distinct Hierarchy3 as BrandId,bd.BrandName,bd.Description from LnkPopItems lnkPOP join BrandData bd on bd.BrandId=lnkPOP.Hierarchy3 and bd.IsDeleted=0 and bd.IsPopOrder=1  where lnkPOP.IsDeleted=0";
            List<BrandData> data = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                data = await db.QueryAsync<BrandData>(query).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetPopOrderFamilyAsync", ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return data;
        }

        //public async Task<List<BrandData>> GetPopOrderBrandAsync()
        //{
        //    try
        //    {
        //        var query = "select distinct Hierarchy4 as BrandId,bd.BrandName,bd.Description from LnkPopItems lnkPOP join BrandData bd on bd.BrandId=lnkPOP.Hierarchy4 and bd.IsDeleted=0 and bd.IsPopOrder=1  where lnkPOP.IsDeleted=0";

        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);

        //        var data = await db.QueryAsync<BrandData>(query);

        //        await db.CloseAsync();

        //        return data;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetPopOrderBrandAsync), ex.StackTrace);
        //        return null;
        //    }
        //}
        public async Task<List<BrandData>> GetPopOrderBrandAsync()
        {
            var query = "select distinct Hierarchy4 as BrandId,bd.BrandName,bd.Description from LnkPopItems lnkPOP join BrandData bd on bd.BrandId=lnkPOP.Hierarchy4 and bd.IsDeleted=0 and bd.IsPopOrder=1  where lnkPOP.IsDeleted=0";
            List<BrandData> data = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                data = await db.QueryAsync<BrandData>(query).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetPopOrderBrandAsync), ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return data;
        }
        //public async Task<List<BrandData>> GetPopOrderGroupAsync()
        //{
        //    try
        //    {
        //        var query = "select distinct Hierarchy5 as BrandId,bd.BrandName,bd.Description from LnkPopItems lnkPOP join BrandData bd on bd.BrandId=lnkPOP.Hierarchy5 and bd.IsDeleted=0 and bd.IsPopOrder=1  where lnkPOP.IsDeleted=0";

        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);

        //        var data = await db.QueryAsync<BrandData>(query);

        //        await db.CloseAsync();

        //        return data;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetPopOrderGroupAsync), ex.StackTrace);
        //        return null;
        //    }
        //}
        public async Task<List<BrandData>> GetPopOrderGroupAsync()
        {
            var query = "select distinct Hierarchy5 as BrandId,bd.BrandName,bd.Description from LnkPopItems lnkPOP join BrandData bd on bd.BrandId=lnkPOP.Hierarchy5 and bd.IsDeleted=0 and bd.IsPopOrder=1  where lnkPOP.IsDeleted=0";
            List<BrandData> data = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                data = await db.QueryAsync<BrandData>(query).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetPopOrderGroupAsync", ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return data;
        }
        //public async Task<List<BrandData>> GetPopOrderCategoryAsync()
        //{
        //    try
        //    {
        //        var query = "select distinct Hierarchy1 as BrandId,bd.BrandName,bd.Description from LnkPopItems lnkPOP join BrandData bd on bd.BrandId=lnkPOP.Hierarchy1 and bd.IsDeleted=0 and bd.IsPopOrder=1  where lnkPOP.IsDeleted=0";

        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);

        //        var data = await db.QueryAsync<BrandData>(query);

        //        await db.CloseAsync();

        //        return data;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetPopOrderCategoryAsync), ex.StackTrace);
        //        return null;
        //    }
        //}
        public async Task<List<BrandData>> GetPopOrderCategoryAsync()
        {
            var query = "select distinct Hierarchy1 as BrandId,bd.BrandName,bd.Description from LnkPopItems lnkPOP join BrandData bd on bd.BrandId=lnkPOP.Hierarchy1 and bd.IsDeleted=0 and bd.IsPopOrder=1  where lnkPOP.IsDeleted=0";
            List<BrandData> data = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                data = await db.QueryAsync<BrandData>(query).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetPopOrderCategoryAsync", ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return data;
        }
        //public async Task<List<PopOrderCartItemModel>> GetFilteredPopCartItemsAsync(string hierarchyValues, int HierarchyTag)
        //{
        //    try
        //    {
        //        string formattedquery = string.Empty;
        //        formattedquery = "SELECT pd.ProductAdditionalDocumentID,pd.DocumentFileName,pd.DocumentType,pm.* from ProductMaster pm left join ProductAdditionalDocument pd on pm.productid=pd.productid where pm.productid in (select productid from LnkPopItems where" + hierarchyValues + "  IsDeleted=0) and IsDeleted=0";
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);
        //        var table = await db.QueryAsync<PopOrderCartItemModel>(formattedquery);
        //        await db.CloseAsync();
        //        return table;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetPopCartItemsAsync), ex.StackTrace);
        //        return null;
        //    }
        //}
        public async Task<List<PopOrderCartItemModel>> GetFilteredPopCartItemsAsync(string hierarchyValues, int HierarchyTag)
        {
            string formattedquery = "SELECT pd.ProductAdditionalDocumentID,pd.DocumentFileName,pd.DocumentType,pm.* from ProductMaster pm left join ProductAdditionalDocument pd on pm.productid=pd.productid where pm.productid in (select productid from LnkPopItems where" + hierarchyValues + "  IsDeleted=0) and IsDeleted=0";
            List<PopOrderCartItemModel> table = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                table = await db.QueryAsync<PopOrderCartItemModel>(formattedquery).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetFilteredPopCartItemsAsync", ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return table;
        }

        public async Task<List<ActivityForAllCustomerUIModel>> GetCallActivitiesOfAllCustomersForLoggedInUser(string territoryIds, bool loadAllData)
        {
            // Validate territoryIds to prevent SQL injection
            if (!string.IsNullOrWhiteSpace(territoryIds))
            {
                foreach (char c in territoryIds)
                {
                    if (!char.IsDigit(c) && c != ',')
                        throw new ArgumentException("Invalid characters in territoryIds", nameof(territoryIds));
                }
            }

            const string selectColumns = @"cm.CreatedDate as CustomerCreatedDate, cm.PhysicalAddressCityID, st.StateName, cm.CustomerName, 
                cm.CustomerNumber, ca.CallActivityID, ca.IsVoided, ca.IsVoidedSync, ca.WholesaleInvoiceFilePath, ca.CustomerID, 
                ca.UserID, ca.ActivityType, ca.Objective, ca.Result, ca.Comments, ca.CreatedDate, ca.UpdateDate, ca.CallDate, 
                ca.OrderID, ca.SalesPerson, ca.GratisProduct, ca.CallActivityDeviceID, ca.IsExported, ca.isDeleted, ca.TerritoryID, 
                ca.TerritoryName, ca.Hours, ca.GrandTotal, um.UserName, (FirstName ||' '||LastName) as UserNameFull, 
                dm.CustomerNumber as DistributorNo";

            string query;
            if (loadAllData)
            {
                query = $@" SELECT {selectColumns} FROM CallActivityList ca 
                    INNER JOIN CustomerMaster cm ON cm.DeviceCustomerID = ca.CustomerID
                    LEFT JOIN UserMaster um On um.UserID = ca.UserID 
                    LEFT JOIN OrderMaster om ON om.DeviceOrderID = ca.OrderID 
                    LEFT join DistributorMaster dm ON dm.CustomerID = om.CustomerDistributorID 
                    LEFT JOIN StateMaster st on cm.PhysicalAddressStateID = st.StateID 
                    WHERE cm.isDeleted != '1' AND (ca.ActivityType NOT like '%AR note%' AND ca.ActivityType NOT like '%ARnote%') AND ca.IsExported !='2' 
                    AND ca.isDeleted != '1' AND ca.IsVoided = '0' AND ca.TerritoryId in ({territoryIds}) ";
            }
            else
            {
                query = $@"SELECT {selectColumns} FROM (select * from CallActivityList 
                    where (ActivityType NOT like '%AR note%' AND ActivityType NOT like '%ARnote%') AND IsExported !='2' AND isDeleted != '1' AND IsVoided = '0'
                    AND TerritoryId in ({territoryIds})  ORDER BY (substr(CallDate,7,4)||' - '||substr(CallDate,1,2)||' - '||substr(CallDate,4,2)) DESC LIMIT 1000 ) ca
                    INNER JOIN CustomerMaster cm ON cm.DeviceCustomerID = ca.CustomerID
                    LEFT JOIN UserMaster um On um.UserID = ca.UserID
                    LEFT JOIN OrderMaster om ON om.DeviceOrderID = ca.OrderID
                    LEFT join DistributorMaster dm ON dm.CustomerID = om.CustomerDistributorID
                    LEFT JOIN StateMaster st on cm.PhysicalAddressStateID = st.StateID WHERE cm.isDeleted != '1' ";
            }

            List<ActivityForAllCustomerUIModel> table = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                table = await db.QueryAsync<ActivityForAllCustomerUIModel>(query).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "GetCallActivitiesOfAllCustomersForLoggedInUser", ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
                db = null;
            }
            return table;
        }

        public async Task<List<ActivityForIndividualCustomerUIModel>> GetCallActivitiesOfSelectedCustomerForLoggedInUser(string deviceCustomerId)
        {
            StringBuilder queryStringBuilder;

            queryStringBuilder = new StringBuilder("SELECT cm.CustomerName, cm.CustomerNumber, cm.PhysicalAddressStateID, cm.PhysicalAddressCityID, ");
            queryStringBuilder.Append("ca.CallActivityID, ca.CustomerID, ca.UserID, ca.WholesaleInvoiceFilePath, ca.ActivityType, ca.Objective, ");
            queryStringBuilder.Append("ca.Result, ca.Comments, ca.CreatedDate, ca.UpdateDate, ca.CallDate, ca.OrderID, ca.SalesPerson, ca.GratisProduct, ");
            queryStringBuilder.Append("ca.CallActivityDeviceID, ca.IsExported, ca.isDeleted, ca.TerritoryID, ca.TerritoryName, ");
            queryStringBuilder.Append("ca.IsVoided, ca.IsVoidedSync, ca.Hours, ca.GrandTotal as GrandTotal, ");
            queryStringBuilder.Append("tm.TerritoryNumber as DisplayTerritoryName, um.UserName, um.FirstName, um.LastName ");
            queryStringBuilder.Append("FROM CallActivityList ca INNER JOIN CustomerMaster cm on cm.DeviceCustomerID = ca.CustomerID ");
            queryStringBuilder.Append("INNER JOIN UserMaster um on um.UserID = ca.UserID ");
            queryStringBuilder.Append("LEFT JOIN TerritoryMaster tm on tm.TerritoryID = um.DefTerritoryId ");
            queryStringBuilder.Append("WHERE ca.CustomerID = '{0}' ");
            queryStringBuilder.Append("AND ca.IsExported != '2' AND ca.isDeleted != '1' AND ca.IsVoided == '0' AND cm.isDeleted != '1'");

            var queryString = queryStringBuilder.ToString();

            string query = string.Format(queryString, deviceCustomerId);

            List<ActivityForIndividualCustomerUIModel> table = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<CallActivityList>().ConfigureAwait(false);
                await db.CreateTableAsync<CustomerMaster>().ConfigureAwait(false);
                await db.CreateTableAsync<UserMaster>().ConfigureAwait(false);
                await db.CreateTableAsync<TerritoryMaster>().ConfigureAwait(false);
                table = await db.QueryAsync<ActivityForIndividualCustomerUIModel>(query).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "GetCallActivitiesOfSelectedCustomerForLoggedInUser", ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false); db = null;
            }
            return table;

        }
        public async Task<List<ActivityForIndividualCustomerUIModel>> GetCallActivitiesOfSelectedArea(string deviceCustomerId, int selectedArea, int roleId)
        {
            StringBuilder queryStringBuilder;

            queryStringBuilder = new StringBuilder("SELECT cm.CustomerName, cm.CustomerNumber, cm.PhysicalAddressStateID, cm.PhysicalAddressCityID, ");
            queryStringBuilder.Append("ca.CallActivityID, ca.CustomerID, ca.UserID, ca.WholesaleInvoiceFilePath, ca.ActivityType, ca.Objective, ");
            queryStringBuilder.Append("ca.Result, ca.Comments, ca.CreatedDate, ca.UpdateDate, ca.CallDate, ca.OrderID, ca.SalesPerson, ca.GratisProduct, ");
            queryStringBuilder.Append("ca.CallActivityDeviceID, ca.IsExported, ca.isDeleted, ca.TerritoryID, ca.TerritoryName, ");
            queryStringBuilder.Append("ca.IsVoided, ca.IsVoidedSync, ca.Hours, ca.GrandTotal as GrandTotal, ");
            queryStringBuilder.Append("tm.TerritoryNumber as DisplayTerritoryName, um.UserName, um.FirstName, um.LastName ");
            queryStringBuilder.Append("FROM CallActivityList ca INNER JOIN CustomerMaster cm on cm.DeviceCustomerID = ca.CustomerID ");
            queryStringBuilder.Append("LEFT JOIN UserMaster um on um.UserID = ca.UserID ");
            queryStringBuilder.Append("LEFT JOIN TerritoryMaster tm on tm.TerritoryID = um.DefTerritoryId ");
            queryStringBuilder.Append("WHERE ca.CustomerID = '{0}' ");
            queryStringBuilder.Append("AND ca.IsExported != '2' AND ca.isDeleted != '1' AND ca.IsVoided == '0' AND cm.isDeleted != '1'");

            if (selectedArea > 0)
            {
                if (roleId == 6 || roleId == 17 || roleId == await GetRoleIdAsync(ApplicationConstants.AVPRoleName))
                    queryStringBuilder.Append($" AND cm.zoneid={selectedArea} ");
                else if (roleId == 3)
                    queryStringBuilder.Append($" AND cm.regionid={selectedArea} ");
            }
            var queryString = queryStringBuilder.ToString();

            string query = string.Format(queryString, deviceCustomerId);

            List<ActivityForIndividualCustomerUIModel> table = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<CallActivityList>();
                await db.CreateTableAsync<CustomerMaster>();
                await db.CreateTableAsync<UserMaster>();
                await db.CreateTableAsync<TerritoryMaster>();
                table = await db.QueryAsync<ActivityForIndividualCustomerUIModel>(query);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "GetCallActivitiesOfSelectedArea", ex);
            }
            finally
            {
                await db.CloseAsync(); db = null;
            }
            return table;

        }
        public async Task<List<CustomerMaster>> GetCustomerListForAddActivity()
        {
            string query = "SELECT CustomerMaster.PhysicalAddressCityID, CustomerMaster.PhysicalAddressStateID, CustomerMaster.AccountType, CustomerMaster.customername , CustomerMaster.CustomerNumber, CustomerMaster.DeviceCustomerID, CustomerMaster.customerid from CustomerMaster";
            List<CustomerMaster> table = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                table = await db.QueryAsync<CustomerMaster>(query).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetCustomerListForAddActivity), ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false); db = null;
            }
            return table;
        }

        public async Task<List<MasterTableTypeUIModel>> GetUserActivityTypeAsync()
        {
            string query = "SELECT Name FROM UserActivityType WHERE IsActive=1 ORDER BY DisplayOrder ASC";
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                return await db.QueryAsync<MasterTableTypeUIModel>(query);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetUserActivityTypeAsync), ex);
            }
            return null;
        }

        public async Task<List<MasterTableTypeUIModel>> GetCustomerActivityTypeAsync()
        {
            string query = "SELECT Name FROM CustomerActivityType WHERE IsActive=1 ORDER BY DisplayOrder ASC";
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                return await db.QueryAsync<MasterTableTypeUIModel>(query);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetCustomerActivityTypeAsync), ex);
            }
            return null;
        }

        public async Task<List<MasterTableTypeUIModel>> GetDocumentTypeAsync()
        {
            string query = "SELECT Name FROM CustomerDocumentType WHERE IsActive=1 ORDER BY DisplayOrder ASC";
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                return await db.QueryAsync<MasterTableTypeUIModel>(query);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetDocumentTypeAsync), ex);
            }
            return null;
        }

        public async Task<List<TerritoryMaster>> GetTerritoryFromIds(string territoryIdsInCvs)
        {
            List<TerritoryMaster> territoryMasterList = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            List<int> territoryIdList = new List<int>();
            territoryIdList = territoryIdsInCvs.Split(',').Select(int.Parse).ToList();
            try
            {
                await db.CreateTableAsync<TerritoryMaster>().ConfigureAwait(false);
                territoryMasterList = await db.Table<TerritoryMaster>().Where(y => territoryIdList.Contains(y.TerritoryID)).ToListAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "GetTerritoryFromIds", ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false); db = null;
            }
            return territoryMasterList;
        }

        //public async Task<bool> AddCallDateToCallActivityList(CallActivityList activity, string date)
        //{
        //    try
        //    {
        //        string query = string.Format("UPDATE CallActivityList SET CallDate = '{0}' WHERE CallActivityDeviceID = {1}", date, activity.CallActivityDeviceID);
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);
        //        var success = await db.ExecuteAsync(query);
        //        await db.CloseAsync();
        //        return success == 1;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(AddCallDateToCallActivityList), ex.StackTrace);
        //        return false;
        //    }
        //}

        public async Task<bool> AddCallDateToCallActivityList(CallActivityList activity, string date)
        {
            int success = 0;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<CallActivityList>().ConfigureAwait(false);
                string query = string.Format("UPDATE CallActivityList SET CallDate = '{0}' WHERE CallActivityDeviceID = {1}", date, activity.CallActivityDeviceID);
                success = await db.ExecuteAsync(query).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(AddCallDateToCallActivityList), ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return success == 1;
        }

        //public async Task<List<UserTaxStatement>> GetUserTaxStatementItemsAsync()
        //{
        //    try
        //    {
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);

        //        var table = await db.Table<UserTaxStatement>().Where(a => a.IsDeleted == 0).ToListAsync();

        //        await db.CloseAsync();

        //        return table;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetUserTaxStatementItemsAsync), ex.StackTrace);
        //        return null;
        //    }
        //}
        public async Task<List<UserTaxStatement>> GetUserTaxStatementItemsAsync()
        {
            List<UserTaxStatement> table = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<UserTaxStatement>().ConfigureAwait(false);
                table = await db.Table<UserTaxStatement>().Where(a => a.IsDeleted == 0).ToListAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(GetType().Name, "GetUserTaxStatementItemsAsync", ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return table;
        }

        //public async Task<UserTaxStatement> InsertUserTaxStatementDataAsync(UserTaxStatement userTaxStatement)
        //{
        //    try
        //    {
        //        int success = 0;
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);
        //        success = await db.InsertAsync(userTaxStatement);
        //        await db.CloseAsync();
        //        return success == 1 ? userTaxStatement : null;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "InsertUserTaxStatementDataAsync", ex.StackTrace);
        //        return null;
        //    }
        //}
        public async Task<UserTaxStatement> InsertUserTaxStatementDataAsync(UserTaxStatement userTaxStatement)
        {
            int success = 0;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                success = await db.InsertAsync(userTaxStatement).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "InsertUserTaxStatementDataAsync", ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return success > 0 ? userTaxStatement : null;
        }

        //public async Task<UserTaxStatement> DeleteUserTaxStatementDataAsync(UserTaxStatement userTaxStatement)
        //{
        //    try
        //    {
        //        int success = 0;
        //        UserTaxStatement existingRecord = null;

        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);

        //        existingRecord = await (db.Table<UserTaxStatement>().Where(a => a.DeviceUserTaxStatementID == userTaxStatement.DeviceUserTaxStatementID)).FirstOrDefaultAsync();

        //        if (existingRecord != null)
        //        {
        //            if (existingRecord.IsExported == 0)
        //            {
        //                success = await db.DeleteAsync<UserTaxStatement>(existingRecord.UserTaxStatementID);
        //            }
        //            else
        //            {
        //                existingRecord.IsDeleted = 1;
        //                existingRecord.IsExported = 0;
        //                existingRecord.UpdatedBy = userTaxStatement.UpdatedBy;
        //                existingRecord.UpdatedDate = userTaxStatement.UpdatedDate;

        //                success = await db.UpdateAsync(existingRecord);
        //            }
        //        }
        //        await db.CloseAsync();
        //        return success == 1 ? userTaxStatement : null;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(DeleteUserTaxStatementDataAsync), ex.StackTrace);
        //        return null;
        //    }
        //}

        public async Task<UserTaxStatement> DeleteUserTaxStatementDataAsync(UserTaxStatement userTaxStatement)
        {
            int success = 0;
            UserTaxStatement existingRecord = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                existingRecord = await (db.Table<UserTaxStatement>().FirstOrDefaultAsync(a => a.DeviceUserTaxStatementID == userTaxStatement.DeviceUserTaxStatementID)).ConfigureAwait(false);

                if (existingRecord != null)
                {
                    if (existingRecord.IsExported == 0)
                    {
                        success = await db.DeleteAsync<UserTaxStatement>(existingRecord.UserTaxStatementID).ConfigureAwait(false);
                    }
                    else
                    {
                        existingRecord.IsDeleted = 1;
                        existingRecord.IsExported = 0;
                        existingRecord.UpdatedBy = userTaxStatement.UpdatedBy;
                        existingRecord.UpdatedDate = userTaxStatement.UpdatedDate;

                        success = await db.UpdateAsync(existingRecord).ConfigureAwait(false);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetStyleDataForProductAsync", ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return success == 1 ? userTaxStatement : null;
        }

        //public async Task<UserTaxStatement> UpdateUserTaxStatementDataAsync(UserTaxStatement userTaxStatement)
        //{
        //    try
        //    {
        //        int success = 0;

        //        UserTaxStatement existingRecord = null;

        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);

        //        existingRecord = await (db.Table<UserTaxStatement>().Where(a => a.DeviceUserTaxStatementID == userTaxStatement.DeviceUserTaxStatementID)).FirstOrDefaultAsync();

        //        if (existingRecord != null)
        //        {
        //            existingRecord.IsExported = 0;
        //            existingRecord.UpdatedBy = userTaxStatement.UpdatedBy;
        //            existingRecord.UpdatedDate = userTaxStatement.UpdatedDate;
        //            existingRecord.Title = userTaxStatement.Title;
        //            existingRecord.Description = userTaxStatement.Description;
        //            success = await db.UpdateAsync(existingRecord);
        //        }

        //        await db.CloseAsync();

        //        return success == 1 ? userTaxStatement : null;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "UpdateUserTaxStatementDataAsync", ex.StackTrace);
        //        return null;
        //    }
        //}

        public async Task<UserTaxStatement> UpdateUserTaxStatementDataAsync(UserTaxStatement userTaxStatement)
        {
            int success = 0;
            UserTaxStatement existingRecord = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                existingRecord = await (db.Table<UserTaxStatement>().FirstOrDefaultAsync(a => a.DeviceUserTaxStatementID == userTaxStatement.DeviceUserTaxStatementID)).ConfigureAwait(false);

                if (existingRecord != null)
                {
                    existingRecord.IsExported = 0;
                    existingRecord.UpdatedBy = userTaxStatement.UpdatedBy;
                    existingRecord.UpdatedDate = userTaxStatement.UpdatedDate;
                    existingRecord.Title = userTaxStatement.Title;
                    existingRecord.Description = userTaxStatement.Description;
                    success = await db.UpdateAsync(existingRecord).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "UpdateUserTaxStatementDataAsync", ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return success == 1 ? userTaxStatement : null;
        }

        //public async Task<OrderDetail> UpdateOrderDetailDataAsync(OrderDetail orderDetail, string currentOrderId)
        //{
        //    int success = 0;

        //    OrderDetail existingRecord = null;

        //    SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);

        //    existingRecord = await db.Table<OrderDetail>().Where(a => a.ProductId == orderDetail.ProductId && a.DeviceOrderID == currentOrderId).FirstOrDefaultAsync();

        //    if (existingRecord != null)
        //    {
        //        existingRecord.Quantity = orderDetail.Quantity;
        //        success = await db.UpdateAsync(existingRecord);
        //    }

        //    await db.CloseAsync();

        //    return success == 1 ? orderDetail : null;
        //}

        public async Task<OrderDetail> UpdateOrderDetailDataAsync(OrderDetail orderDetail, string currentOrderId)
        {
            int success = 0;

            OrderDetail existingRecord = null;

            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<OrderDetail>().ConfigureAwait(false);
                existingRecord = await db.Table<OrderDetail>()
                    .FirstOrDefaultAsync(a => a.ProductId == orderDetail.ProductId && a.DeviceOrderID == currentOrderId).ConfigureAwait(false);

                if (existingRecord != null)
                {
                    existingRecord.Quantity = orderDetail.Quantity;
                    success = await db.UpdateAsync(existingRecord).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(GetType().Name, nameof(UpdateOrderDetailDataAsync), ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false); db = null;
            }
            return success == 1 ? orderDetail : null;
        }

        public async Task<CallActivityList> GetIndidualActivityById(string id)
        {
            CallActivityList activity = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                activity = await db.Table<CallActivityList>().FirstOrDefaultAsync(a => a.CallActivityDeviceID == id).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetIndidualActivityById), ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return activity;
        }

        public async Task<OrderMaster> GetOrderFromOrderMasterFromDeviceOrderId(string deviceOrderId)
        {
            //try
            //{
            //    string query = string.Format("SELECT * from OrderMaster WHERE OrderMaster.DeviceOrderID='{0}'", deviceOrderId);
            //    SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);
            //    var table = await db.QueryAsync<OrderMaster>(query);
            //    await db.CloseAsync();
            //    return table.FirstOrDefault();
            //}
            //catch (Exception ex)
            //{
            //    ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetOrderFromOrderMasterFromDeviceOrderId), ex.StackTrace);
            //    return null;
            //}
            OrderMaster deviceOrder = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<OrderMaster>().ConfigureAwait(false);
                deviceOrder = await db.Table<OrderMaster>()
                    .FirstOrDefaultAsync(x => x.DeviceOrderID == deviceOrderId).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetOrderFromOrderMasterFromDeviceOrderId), ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return deviceOrder;
        }

        public async Task<List<OrderDetail>> GetOrderDetailsFromDeviceOrderId(string deviceOrderId)
        {
            List<OrderDetail> data = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                var query = string.Format("SELECT * FROM OrderDetail OD WHERE DeviceOrderID in ('{0}')", string.Join(",", deviceOrderId));
                data = await db.QueryAsync<OrderDetail>(query).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetOrderDetailsFromDeviceOrderId), ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return data;
        }


        public async Task<List<OrderDetail>> GetOrderDetailsByDeviceOrderId(string deviceOrderId)
        {
            List<OrderDetail> data = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<OrderMaster>().ConfigureAwait(false);
                await db.CreateTableAsync<ProductMaster>().ConfigureAwait(false);
                await db.CreateTableAsync<CategoryMaster>().ConfigureAwait(false);
                await db.CreateTableAsync<BrandData>().ConfigureAwait(false);
                await db.CreateTableAsync<StyleData>().ConfigureAwait(false);
                await db.CreateTableAsync<OrderDetail>().ConfigureAwait(false);
                string query = string.Format(@"SELECT od.OrderDetailId,od.OrderId,od.CategoryId,cm.CategoryName,od.BrandId,bd.BrandName,od.StyleId,sd.StyleName,od.ProductId,pm.ProductName
                ,pm.isTobbaco, pm.Description AS ProductDescription,od.Quantity,od.Price,od.CreatedDate,od.UpdatedDate
                ,od.Unit,od.Total,od.DeviceOrderID,od.CreditRequest 
                FROM OrderDetail od         
                INNER JOIN ORDERMASTER om ON om.DeviceOrderID = od.DeviceOrderID 
                INNER JOIN ProductMaster pm ON pm.ProductId = od.ProductId 
                LEFT JOIN CategoryMaster cm ON cm.CategoryId = od.CategoryId 
                LEFT JOIN BrandData bd ON bd.BrandId = od.BrandId 
                LEFT JOIN StyleData sd ON sd.StyleId = od.StyleId where om.DeviceOrderID = '{0}'", deviceOrderId);
                data = await db.QueryAsync<OrderDetail>(query).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetOrderDetailsByDeviceOrderId), ex.Message + " - " + ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return data;
        }

        //    public async Task UpdateLastCallActivityDateForCustomerMaster(string deviceCustomerId, string updatedDate)
        //    {
        //        var query = string.Format("update CustomerMaster set LastCallActivityDate = '{0}' where DevicecustomerID = '{1}'", updatedDate, deviceCustomerId);
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
        //        try
        //        {
        //await db.CreateTableAsync<CustomerMaster>().ConfigureAwait(false);
        //            var s = await db.ExecuteAsync(query).ConfigureAwait(false);
        //        }
        //        catch (Exception ex)
        //        {
        //            ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(UpdateLastCallActivityDateForCustomerMaster), ex.StackTrace);
        //        }
        //        finally
        //        {
        //            await db.CloseAsync().ConfigureAwait(false); db=null;
        //        }
        //    }

        public async Task UpdateLastCallActivityDateForCustomerMaster(string deviceCustomerId, string updatedDate)
        {
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<CustomerMaster>().ConfigureAwait(false);
                CustomerMaster customerMaster = await db.Table<CustomerMaster>()
                    .FirstOrDefaultAsync(x => x.DeviceCustomerID == deviceCustomerId)
                    .ConfigureAwait(false);
                if (customerMaster != null)
                {
                    customerMaster.IsExported = 0;
                    customerMaster.UpdatedDate = updatedDate;
                    customerMaster.LastCallActivityDate = updatedDate;
                }
                await db.UpdateAsync(customerMaster).ConfigureAwait(false);
                await InfoLogger.GetInstance.WriteToLogAsync(SourceName: $"{nameof(DatabaseService)}:{nameof(UpdateLastCallActivityDateForCustomerMaster)}"
                       , SourceArgument: new { deviceCustomerId, updatedDate }
                       , CustomeMessage: "To Update Table CustomerMaster").ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(UpdateLastCallActivityDateForCustomerMaster), ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false); db = null;
            }
        }

        public async Task UpdateDateFromActivityCustomerMaster(string deviceCustomerId, string activityType, string lastCallDateTime)
        {
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<CustomerMaster>().ConfigureAwait(false);
                CustomerMaster customerMaster = await db.Table<CustomerMaster>()
                    .FirstOrDefaultAsync(x => x.DeviceCustomerID == deviceCustomerId)
                    .ConfigureAwait(false);
                if (customerMaster != null)
                {

                    /**** JIRA Ticket https://republicbrands.atlassian.net/browse/HS2-57
                     * Indirect Stores:Retail Sales Call
                     * For Chain HQ:Chain HQ Sales Call
                     * For Distributor account types:Distributor Sales Call
                     */
                    if (string.IsNullOrWhiteSpace(customerMaster.CustomerNumber)
                        || (!string.IsNullOrWhiteSpace(customerMaster.CustomerNumber)
                        && !customerMaster.CustomerNumber.ToLower().StartsWith("x")))
                    {
                        if (customerMaster?.IsParent == 1 && activityType.Equals("Chain Sales Call"))
                            customerMaster.LastCallActivityDate = lastCallDateTime;
                        else if ((customerMaster?.IsParent != 1 && customerMaster.AccountType.Equals(2)) && activityType.Equals("Retail Sales Call"))
                            customerMaster.LastCallActivityDate = lastCallDateTime;
                        else if ((customerMaster?.IsParent != 1 && customerMaster.AccountType.Equals(1)) && activityType.Equals("Distributor Sales Call"))
                            customerMaster.LastCallActivityDate = lastCallDateTime;
                    }
                    customerMaster.IsExported = 0;
                    customerMaster.UpdatedDate = DateTimeHelper.ConvertToDbInsertDateTimeFormat(DateTime.Now);
                    await db.UpdateAsync(customerMaster).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(UpdateDateFromActivityCustomerMaster), ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false); db = null;
            }
        }

        //public async Task<string> GetOrderGrandTotalFromOrderDeviceId(string orderDeviceId)
        //{
        //    try
        //    {
        //        var query = string.Format("SELECT OrderMaster.GrandTotal from OrderMaster WHERE OrderMaster.DeviceOrderID = '{0}'", orderDeviceId);
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);
        //        var total = await db.QueryScalarsAsync<string>(query);
        //        await db.CloseAsync();
        //        return total?.FirstOrDefault();
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(UpdateLastCallActivityDateForCustomerMaster), ex.StackTrace);
        //        return null;
        //    }
        //}
        public async Task<string> GetOrderGrandTotalFromOrderDeviceId(string orderDeviceId)
        {
            var query = string.Format("SELECT OrderMaster.GrandTotal from OrderMaster WHERE OrderMaster.DeviceOrderID = '{0}'", orderDeviceId);
            string total = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                total = await db.ExecuteScalarAsync<string>(query).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetOrderGrandTotalFromOrderDeviceId", ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return total;
        }

        //public async Task<IEnumerable<ZoneMaster>> GetZoneFromZoneId(string query)
        //{
        //    try
        //    {
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);
        //        var zoneList = await db.QueryAsync<ZoneMaster>(query);
        //        await db.CloseAsync();
        //        return zoneList;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetZoneFromZoneId), ex.StackTrace);
        //        return null;
        //    }
        //}
        public async Task<IEnumerable<ZoneMaster>> GetZoneFromZoneId(string query)
        {
            IEnumerable<ZoneMaster> zoneList = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                zoneList = await db.QueryAsync<ZoneMaster>(query).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetZoneFromZoneId", ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return zoneList;
        }

        //public async Task<IEnumerable<RegionMaster>> GetRegionFromRegionId(int id)
        //{
        //    try
        //    {
        //        var query = string.Format("SELECT RegionMaster.RegionID, RegionMaster.Regioname, RegionMaster.ZoneID from RegionMaster WHERE RegionMaster.RegionID == {0}", id);
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);
        //        var zoneList = await db.QueryAsync<RegionMaster>(query);
        //        await db.CloseAsync();
        //        return zoneList;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetZoneFromZoneId), ex.StackTrace);
        //        return null;
        //    }
        //}
        public async Task<IEnumerable<RegionMaster>> GetRegionFromRegionId(int id)
        {
            var query = string.Format("SELECT RegionMaster.RegionID, RegionMaster.Regioname, RegionMaster.ZoneID from RegionMaster WHERE RegionMaster.RegionID == {0}", id);
            IEnumerable<RegionMaster> regionList = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                regionList = await db.QueryAsync<RegionMaster>(query).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetRegionFromRegionId", ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return regionList;
        }

        //public async Task<IEnumerable<TerritoryMaster>> GetTerritoryFromDefferedTerritoryId(int id)
        //{
        //    try
        //    {
        //        var query = string.Format("SELECT TerritoryMaster.TerritoryID, TerritoryMaster.TerritoryName from TerritoryMaster WHERE TerritoryId = {0}", id);
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);
        //        var zoneList = await db.QueryAsync<TerritoryMaster>(query);
        //        await db.CloseAsync();
        //        return zoneList;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetZoneFromZoneId), ex.StackTrace);
        //        return null;
        //    }
        //}
        public async Task<IEnumerable<TerritoryMaster>> GetTerritoryFromDefferedTerritoryId(int id)
        {
            var query = string.Format("SELECT TerritoryMaster.TerritoryID, TerritoryMaster.TerritoryName from TerritoryMaster WHERE TerritoryId = {0}", id);
            IEnumerable<TerritoryMaster> territoryList = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                territoryList = await db.QueryAsync<TerritoryMaster>(query).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetTerritoryFromDefferedTerritoryId), ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return territoryList;
        }

        //public async Task<IEnumerable<CityMaster>> GetCityMasterData()
        //{
        //    try
        //    {
        //        // var query = "SELECT CityMaster.CityID, CityMaster.CityName,CityMaster.StateID from CityMaster";
        //        var query = "SELECT DISTINCT CityMaster.CityID, CityMaster.CityName,CityMaster.StateID  from CityMaster INNER JOIN  CustomerMaster on CustomerMaster.PhysicalAddressCityID = CityMaster.CityName";
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);
        //        var cityList = await db.QueryAsync<CityMaster>(query);
        //        await db.CloseAsync();
        //        return cityList;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetCityMasterData), ex.StackTrace);
        //        return null;
        //    }
        //}
        //public async Task<IEnumerable<CityMaster>> GetCityMasterData()
        //{
        //    try
        //    {
        //        // var query = "SELECT CityMaster.CityID, CityMaster.CityName,CityMaster.StateID from CityMaster";
        //        var query = "SELECT DISTINCT CityMaster.CityID, CityMaster.CityName,CityMaster.StateID  from CityMaster INNER JOIN  CustomerMaster on CustomerMaster.PhysicalAddressCityID = CityMaster.CityName";
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);
        //        var cityList = await db.QueryAsync<CityMaster>(query);
        //        await db.CloseAsync();
        //        return cityList;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetCityMasterData), ex.StackTrace);
        //        return null;
        //    }
        //}
        public async Task<IEnumerable<CityMaster>> GetCityMasterData()
        {
            var query = "SELECT DISTINCT CityMaster.CityID, CityMaster.CityName,CityMaster.StateID  from CityMaster INNER JOIN  CustomerMaster on CustomerMaster.PhysicalAddressCityID = CityMaster.CityName";
            IEnumerable<CityMaster> cityList = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                cityList = await db.QueryAsync<CityMaster>(query).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetCityMasterData", ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return cityList;
        }

        public async Task<IEnumerable<ZoneMaster>> GetZonesOnBasisOfZoneIds(string query)
        {
            IEnumerable<ZoneMaster> zoneMasters = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                zoneMasters = await db.QueryAsync<ZoneMaster>(query);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetZonesOnBasisOfZoneIds), ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return zoneMasters;
        }
        public async Task<IEnumerable<CustomerMaster>> GetCustomersInMapAsync(int? zoneId, int? regionId, int? territoryId, string stateID, string cityName)
        {
            List<CustomerMaster> customerMasterTable = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<CustomerMaster>().ConfigureAwait(false);
                string queryText = @"SELECT CustomerMaster.LastCallActivityDate,
                    CustomerMaster.PhysicalAddress,CustomerMaster.PhysicalAddressCityID,CustomerMaster.PhysicalAddressStateID,
                    CustomerMaster.PhysicalAddressZipCode,CustomerMaster.CustomerNumber,CustomerMaster.CustomerName,CustomerMaster.AccountClassification, CustomerMaster.Rank,
                     CustomerMaster.AccountType, CustomerMaster.CustomerID,CustomerMaster.DeviceCustomerID, CustomerMaster.Latitude,
                    CustomerMaster.Longitude from CustomerMaster WHERE 
                    CustomerMaster.DeviceCustomerID != '0-0' AND CustomerMaster.isDeleted != 1 AND 
                    CustomerMaster.AccountClassification NOT IN('3','7','20')  ";
                queryText += (zoneId.HasValue ? string.Format("AND CustomerMaster.ZoneId = {0} ", zoneId.Value) : string.Empty);
                queryText += (regionId.HasValue ? string.Format("AND CustomerMaster.RegionId = {0} ", regionId.Value) : string.Empty);
                queryText += (territoryId.HasValue ? string.Format("AND CustomerMaster.TerritoryId = {0} ", territoryId.Value) : string.Empty);
                queryText += (!string.IsNullOrEmpty(stateID) ? string.Format("AND  CustomerMaster.PhysicalAddressStateID = '{0}' ", stateID) : string.Empty);
                queryText += (!string.IsNullOrEmpty(cityName) ? string.Format("AND CustomerMaster.PhysicalAddressCityID = '{0}' ", cityName) : string.Empty);
                //queryText += " ORDER BY CAST( CustomerMaster.AccountClassification AS INTEGER) DESC ";

                customerMasterTable = await db.QueryAsync<CustomerMaster>(queryText).ConfigureAwait(false);

            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "GetCustomerDataAsync", ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return customerMasterTable;
        }
        public async Task<IEnumerable<Classification>> GetClassificationInMapAsync()
        {
            List<Classification> table = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                string queryString = "SELECT AccountClassificationName,AccountClassificationId FROM Classification WHERE AccountClassificationId NOT in (3,7,20)";
                table = await db.QueryAsync<Classification>(queryString).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "GetClassificationAsync", ex.Message);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return table;
        }

        public async Task<IEnumerable<RegionMaster>> GetRegionsOnBasisOfZoneIds(string query)
        {
            IEnumerable<RegionMaster> _regions = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                _regions = await db.QueryAsync<RegionMaster>(query);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetRegionsOnBasisOfZoneIds), ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return _regions;
        }

        //public async Task<IEnumerable<TerritoryMaster>> GetTerritoryOnBasisOfRegionIds(string query)
        //{
        //    try
        //    {
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);
        //        var _territories = await db.QueryAsync<TerritoryMaster>(query);
        //        await db.CloseAsync();
        //        return _territories;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetTerritoryOnBasisOfRegionIds), ex.StackTrace);
        //        return null;
        //    }
        //}
        public async Task<IEnumerable<TerritoryMaster>> GetTerritoryOnBasisOfRegionIds(string query)
        {
            IEnumerable<TerritoryMaster> _territories = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                _territories = await db.QueryAsync<TerritoryMaster>(query).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetTerritoryOnBasisOfRegionIds), ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return _territories;
        }

        //public async Task<IEnumerable<MapCustomerData>> GetMapDataForTradeTypeAndRankAndCallDate(string query)
        //{
        //    try
        //    {
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);
        //        var _data = await db.QueryAsync<MapCustomerData>(query);
        //        await db.CloseAsync();
        //        return _data;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetMapDataForTradeTypeAndRankAndCallDate), ex.StackTrace);
        //        return null;
        //    }
        //}
        public async Task<IEnumerable<MapCustomerData>> GetMapDataForTradeTypeAndRankAndCallDate(string query)
        {
            IEnumerable<MapCustomerData> _data = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<CustomerMaster>().ConfigureAwait(false);
                await db.CreateTableAsync<Classification>().ConfigureAwait(false);
                _data = await db.QueryAsync<MapCustomerData>(query).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetMapDataForTradeTypeAndRankAndCallDate), ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return _data;
        }

        //public async Task<IEnumerable<MapCustomerData>> GetMapCustomerDataForCashSales(string query)
        //{
        //    try
        //    {
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);

        //        var _data = await db.QueryAsync<MapCustomerData>(query);

        //        await db.CloseAsync();

        //        return _data;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetMapCustomerDataForCashSales), ex.StackTrace);
        //        return null;
        //    }
        //}
        public async Task<IEnumerable<MapCustomerData>> GetMapCustomerDataForCashSales(string query)
        {
            IEnumerable<MapCustomerData> _data = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                _data = await db.QueryAsync<MapCustomerData>(query).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetMapCustomerDataForCashSales", ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return _data;
        }

        //public async Task<IEnumerable<MapCustomerData>> GetMapCustomerDataForItemFilter(string query)
        //{
        //    try
        //    {
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);

        //        var _data = await db.QueryAsync<MapCustomerData>(query);

        //        await db.CloseAsync();

        //        return _data;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "GetMapCustomerDataForItemFilter", ex.StackTrace);
        //        return null;
        //    }
        //}
        public async Task<IEnumerable<MapCustomerData>> GetMapCustomerDataForItemFilter(string query)
        {
            IEnumerable<MapCustomerData> _data = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                _data = await db.QueryAsync<MapCustomerData>(query).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "GetMapCustomerDataForItemFilter", ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return _data;
        }

        //public async Task<IEnumerable<ProductMaster>> GetProductDetailsOnBasisOfProductName(string query)
        //{
        //    try
        //    {
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);

        //        var _data = await db.QueryAsync<ProductMaster>(query);

        //        await db.CloseAsync();

        //        return _data;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetProductDetailsOnBasisOfProductName), ex.StackTrace);
        //        return null;
        //    }
        //}
        public async Task<IEnumerable<ProductMaster>> GetProductDetailsOnBasisOfProductName(string query)
        {
            IEnumerable<ProductMaster> _data = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                _data = await db.QueryAsync<ProductMaster>(query).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetProductDetailsOnBasisOfProductName", ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return _data;
        }

        //public async Task<CustomerMaster> GetMapPopupCutomerData(string query)
        //{
        //    try
        //    {
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
        //        var _data = await db.QueryAsync<CustomerMaster>(query);
        //        await db.CloseAsync();
        //        return _data.FirstOrDefault();
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetMapPopupCutomerData), ex.StackTrace);
        //        return null;
        //    }
        //}
        public async Task<CustomerMaster> GetMapPopupCutomerData(string query)
        {
            IEnumerable<CustomerMaster> _data = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                _data = await db.QueryAsync<CustomerMaster>(query).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetMapPopupCutomerData), ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return _data != null ? _data.FirstOrDefault() : null;
        }

        //public async Task<IEnumerable<ActivityForAllCustomerUIModel>> GetMapPopupActivityData(string query)
        //{
        //    try
        //    {
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
        //        var _data = await db.QueryAsync<ActivityForAllCustomerUIModel>(query);
        //        await db.CloseAsync();
        //        return _data;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetMapPopupActivityData), ex.StackTrace);
        //        return null;
        //    }
        //}
        public async Task<IEnumerable<ActivityForAllCustomerUIModel>> GetMapPopupActivityData(string query)
        {
            IEnumerable<ActivityForAllCustomerUIModel> _data = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                _data = await db.QueryAsync<ActivityForAllCustomerUIModel>(query).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetMapPopupActivityData), ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return _data;
        }

        //public async Task<List<RouteListUIModel>> GetRouteListData()
        //{
        //    string queryString = "SELECT sr.*, um.username as UserName, um.TerritoryName as TerritoryName, umcr.username as CreatorName FROM ScheduledRoutes sr LEFT JOIN (SELECT username, userid, TerritoryName FROM UserMaster, TerritoryMaster WHERE UserMaster.DefTerritoryId = TerritoryMaster.TerritoryID ) um ON um.UserID = CASE WHEN sr.idAssignToTSM= 0 THEN sr.UserId ELSE idAssignToTSM END LEFT JOIN (SELECT username, userid FROM UserMaster) umcr ON sr.userid = umcr.userid WHERE sr.IsDeleted!='1' ORDER BY RouteId DESC";

        //    SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);

        //    var routeListData = await db.QueryAsync<RouteListUIModel>(queryString);

        //    await db.CloseAsync();

        //    return routeListData;
        //}
        public async Task<List<RouteListUIModel>> GetRouteListData()
        {
            string queryString = $@"SELECT sr.*, um.username as UserName, um.TerritoryName as TerritoryName, umcr.username as CreatorName FROM ScheduledRoutes sr 
                LEFT JOIN (SELECT username, userid, TerritoryName FROM UserMaster, TerritoryMaster 
                    WHERE UserMaster.DefTerritoryId = TerritoryMaster.TerritoryID AND UserMaster.IsInActive=0 AND UserMaster.IsDeleted=0 ) um 
                    ON um.UserID = CASE WHEN sr.idAssignToTSM= 0 THEN sr.UserId ELSE idAssignToTSM END 
                LEFT JOIN (SELECT username, userid FROM UserMaster) umcr ON sr.userid = umcr.userid 
                WHERE sr.IsDeleted!='1' AND um.UserID IS NOT NULL
                ORDER BY RouteId DESC";

            List<RouteListUIModel> routeListData = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                routeListData = await db.QueryAsync<RouteListUIModel>(queryString).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetRouteListData", ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return routeListData;
        }

        //public async Task<bool> UpdateScheduleRoutesForCurrentUser(int userId)
        //{
        //    string queryString = string.Format("UPDATE ScheduledRoutes SET UserId = '{0}', CreatedBy = '{0}', idAssignToTSM = 0  WHERE (UserId is Null or UserId = '') AND (idAssignToTSM is Null or idAssignToTSM = '')", userId);

        //    SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);

        //    var result = await db.QueryAsync<ScheduledRoutes>(queryString);

        //    await db.CloseAsync();

        //    if (result != null)
        //    {
        //        return true;
        //    }

        //    return false;
        //}

        public async Task<bool> UpdateScheduleRoutesForCurrentUser(int userId)
        {
            string queryString = string.Format("UPDATE ScheduledRoutes SET UserId = '{0}', CreatedBy = '{0}', idAssignToTSM = 0  WHERE (UserId is Null or UserId = '') AND (idAssignToTSM is Null or idAssignToTSM = '')", userId);
            int result = 0;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                result = await db.ExecuteAsync(queryString).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                result = 0;
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "UpdateScheduleRoutesForCurrentUser", ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return result > 0;
        }

        //public async Task<CallActivityList> GetCurrentCustomerCallActivityDataAsync(string CustomerID, string OrderID)
        //{
        //    string queryString = string.Format("SELECT * FROM CallActivityList WHERE customerid = '{0}' AND OrderId = '{1}'", CustomerID, OrderID);

        //    SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);

        //    var dataSet = await db.QueryAsync<CallActivityList>(queryString);

        //    await db.CloseAsync();

        //    return dataSet.FirstOrDefault();
        //}
        public async Task<CallActivityList> GetCurrentCustomerCallActivityDataAsync(string CustomerID, string OrderID)
        {
            CallActivityList callActivityList = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                callActivityList = await db.Table<CallActivityList>().FirstOrDefaultAsync(x => x.CustomerID == CustomerID && x.OrderID == OrderID)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetCurrentCustomerCallActivityDataAsync", ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return callActivityList;
        }

        //public async Task<IEnumerable<int>> GetCustomerIdsFromRouteStationTableByRouteId(string query)
        //{
        //    try
        //    {
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
        //        var _data = await db.QueryScalarsAsync<int>(query);
        //        await db.CloseAsync();
        //        return _data;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetCustomerIdsFromRouteStationTableByRouteId), ex.StackTrace);
        //        return null;
        //    }
        //}
        public async Task<IEnumerable<int>> GetCustomerIdsFromRouteStationTableByRouteId(string query)
        {
            IEnumerable<int> _data = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                _data = await db.QueryScalarsAsync<int>(query).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetCustomerIdsFromRouteStationTableByRouteId), ex);
            }
            return _data;
        }

        //public async Task<bool> DeleteAllRouteStationsForRouteId(string query)
        //{
        //    try
        //    {
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
        //        var res = await db.ExecuteAsync(query);
        //        await db.CloseAsync();
        //        return res >= 0;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(DeleteAllRouteStationsForRouteId), ex.StackTrace);
        //        return false;
        //    }
        //}
        public async Task<bool> DeleteAllRouteStationsForRouteId(string query)
        {
            int res = 0;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                res = await db.ExecuteAsync(query).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(DeleteAllRouteStationsForRouteId), ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return res > 0;
        }

        //public async Task<bool> UpdateScheduleRoutesWhenRouteIdIs(string query)
        //{
        //    try
        //    {
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
        //        var res = await db.ExecuteAsync(query);
        //        await db.CloseAsync();
        //        return res == 1;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(UpdateScheduleRoutesWhenRouteIdIs), ex.StackTrace);
        //        return false;
        //    }
        //}
        public async Task<bool> UpdateScheduleRoutesWhenRouteIdIs(string query)
        {
            int res = 0;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                res = await db.ExecuteAsync(query);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(UpdateScheduleRoutesWhenRouteIdIs), ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return res > 0;
        }

        //public async Task<IEnumerable<MapPopAddToRouteListModel>> GetRoutesForMapPopup(string query)
        //{
        //    try
        //    {
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
        //        var result = await db.QueryAsync<MapPopAddToRouteListModel>(query);
        //        await db.CloseAsync();
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetRoutesForMapPopup), ex.StackTrace);
        //        return null;
        //    }
        //}
        public async Task<IEnumerable<MapPopAddToRouteListModel>> GetRoutesForMapPopup(string query)
        {
            IEnumerable<MapPopAddToRouteListModel> result = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                result = await db.QueryAsync<MapPopAddToRouteListModel>(query).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetStyleDataForProductAsync", ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return result;
        }

        //public async Task<List<ViewRouteDetailsUIModel>> GetViewRouteDataAsync(string deviceRouteId)
        //{
        //    string queryString = string.Format("SELECT cm.Latitude, cm.Longitude, cm.CustomerID, cm.DeviceCustomerID, cm.CustomerName, cm.PhysicalAddress, cm.PhysicalAddressCityID, cm.AccountType, cm.PhysicalAddressStateID, cm.PhysicalAddressZipCode, sm.StateName FROM CustomerMaster cm INNER JOIN StateMaster sm WHERE cm.PhysicalAddressStateID = sm.StateID AND cm.CustomerID in (SELECT CustomerId FROM RouteStations WHERE DeviceRouteId='{0}')", deviceRouteId);

        //    SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);

        //    var dataSet = await db.QueryAsync<ViewRouteDetailsUIModel>(queryString);

        //    await db.CloseAsync();

        //    return dataSet;
        //}
        public async Task<List<ViewRouteDetailsUIModel>> GetViewRouteDataAsync(string deviceRouteId)
        {
            string queryString = string.Format("SELECT cm.Latitude, cm.Longitude, cm.CustomerID,cm.CustomerNumber, cm.DeviceCustomerID, cm.CustomerName, cm.PhysicalAddress, cm.PhysicalAddressCityID, cm.AccountType, cm.PhysicalAddressStateID, cm.PhysicalAddressZipCode, sm.StateName FROM CustomerMaster cm INNER JOIN StateMaster sm WHERE cm.PhysicalAddressStateID = sm.StateID AND cm.CustomerID in (SELECT CustomerId FROM RouteStations WHERE DeviceRouteId='{0}')", deviceRouteId);

            List<ViewRouteDetailsUIModel> dataSet = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                dataSet = await db.QueryAsync<ViewRouteDetailsUIModel>(queryString).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetViewRouteDataAsync", ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return dataSet;
        }

        //public async Task<bool> DeleteScheduledRouteData(string deviceRouteId)
        //{
        //    string queryString = string.Format("UPDATE ScheduledRoutes SET IsExported='2', IsDeleted='1' WHERE DeviceRouteId='{0}'", deviceRouteId);

        //    SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);

        //    var result = await db.QueryAsync<ScheduledRoutes>(queryString);

        //    await db.CloseAsync();

        //    if (result != null)
        //    {
        //        return true;
        //    }

        //    return false;
        //}
        public async Task<bool> DeleteScheduledRouteData(string deviceRouteId)
        {
            int result = 0;
            string queryString = string.Format("UPDATE ScheduledRoutes SET IsExported='0', IsDeleted='1' WHERE DeviceRouteId='{0}'", deviceRouteId);
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                result = await db.ExecuteAsync(queryString).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                result = 0;
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "DeleteScheduledRouteData", ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return (result > 0);
        }

        public async Task<bool> DeleteExistingRoutesDataDuringSync(string deviceRouteId)
        {
            bool success = true;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.Table<ScheduledRoutes>().DeleteAsync(a => a.DeviceRouteId == deviceRouteId).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                success = false;
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(InsertOrUpdateBrandDataAsync), ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }

            return success;
        }

        //public async Task<IEnumerable<StateMaster>> GetStateDictionaryWhichHasCustomersAssociated(string query)
        //{
        //    try
        //    {
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
        //        var result = await db.QueryAsync<StateMaster>(query);
        //        await db.CloseAsync();
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetStateDictionaryWhichHasCustomersAssociated), ex.StackTrace);
        //        return null;
        //    }
        //}
        public async Task<IEnumerable<StateMaster>> GetStateDictionaryWhichHasCustomersAssociated(string query)
        {
            IEnumerable<StateMaster> result = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                result = await db.QueryAsync<StateMaster>(query).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetStateDictionaryWhichHasCustomersAssociated), ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return result;
        }

        //public async Task<List<UserMaster>> GetListOfTerritoryManagersForRegion(int regionId)
        //{
        //    try
        //    {
        //        string query = string.Format("SELECT * from UserMaster WHERE RoleID = 1 AND RegionId = {0}", regionId);
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
        //        var result = await db.QueryAsync<UserMaster>(query);
        //        await db.CloseAsync();

        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetListOfTerritoryManagersForRegion), ex.StackTrace);
        //        return null;
        //    }
        //}
        public async Task<List<UserMaster>> GetListOfTerritoryManagersForRegion(int regionId)
        {
            List<UserMaster> result = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                string query = string.Format("SELECT * from UserMaster WHERE RoleID = 1 AND IsInActive=0 AND IsDeleted=0 AND RegionId = {0}", regionId);
                result = await db.QueryAsync<UserMaster>(query).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetListOfTerritoryManagersForRegion), ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return result;
        }

        public async Task<List<UserMaster>> GetListOfTerritoryManagersForTerritories(string territoryIds)
        {
            List<UserMaster> result = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                string query = $"SELECT * from UserMaster WHERE RoleID = 1 AND IsInActive=0 AND IsDeleted=0 AND TerritoryId IN ({territoryIds})";
                result = await db.QueryAsync<UserMaster>(query).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetListOfTerritoryManagersForTerritories), ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return result;
        }
        public async Task<List<UserMaster>> GetListOfTerritoryManagersForZone(int zoneId)
        {
            List<UserMaster> result = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                string query = string.Format("SELECT * from UserMaster WHERE RoleID = 1  AND IsInActive=0 AND IsDeleted=0 AND ZoneId = {0}", zoneId);
                result = await db.QueryAsync<UserMaster>(query).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetListOfTerritoryManagersForZone), ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return result;
        }

        //public async Task UpdateOrderGrandTotal(string query)
        //{
        //    try
        //    {
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
        //        var result = await db.ExecuteAsync(query);
        //        await db.CloseAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(UpdateOrderGrandTotal), ex.StackTrace);
        //    }
        //}
        public async Task UpdateOrderGrandTotal(string query)
        {
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                var result = await db.ExecuteAsync(query).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(UpdateOrderGrandTotal), ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
        }

        //public async Task<List<CustomerDocument>> GetCustomerDocumentsForUpload()
        //{
        //    try
        //    {
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);

        //        var table = await db.Table<CustomerDocument>().Where(x => x.IsExported == 0 && x.IsDelete.Equals("0")).ToListAsync();

        //        await db.CloseAsync();

        //        return table;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetCustomerDocumentsForUpload), ex.StackTrace);
        //        return null;
        //    }
        //}

        public async Task<List<CustomerDocument>> GetCustomerDocumentsForUpload()
        {
            List<CustomerDocument> table = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<CustomerDocument>().ConfigureAwait(false);
                table = await db.Table<CustomerDocument>()
                    .Where(x => x.IsExported == 0)
                    .ToListAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetCustomerDocumentsForUpload", ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false); db = null;
            }
            return table;
        }

        //public async Task<string> GetCustomerDeviceIdForDocumentUpload(int customerId)
        //{
        //    try
        //    {
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);

        //        var customer = await db.Table<CustomerMaster>().Where(x => x.CustomerID == customerId).FirstOrDefaultAsync();

        //        await db.CloseAsync();

        //        return (customer != null) ? customer.DeviceCustomerID : string.Empty;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "GetCustomerDeviceIdForDocumentUpload", ex.StackTrace);
        //        return null;
        //    }
        //}
        public async Task<string> GetCustomerDeviceIdForDocumentUpload(int customerId)
        {
            CustomerMaster customer = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<CustomerMaster>().ConfigureAwait(false);
                customer = await db.Table<CustomerMaster>()
                    .FirstOrDefaultAsync(x => x.CustomerID == customerId).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetCustomerDeviceIdForDocumentUpload), ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false); db = null;
            }
            return (customer != null) ? customer?.DeviceCustomerID : string.Empty;
        }

        //public async Task<bool> DeleteExistingCustomerDocumentData(CustomerDocument customerDocument)
        //{
        //    SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);

        //    await db.Table<CustomerDocument>().Where(a => a.DeviceDocID == customerDocument.DeviceDocID).DeleteAsync();

        //    await db.CloseAsync();

        //    return true;
        //}

        public async Task<bool> DeleteExistingCustomerDocumentData(CustomerDocument customerDocument)
        {
            bool success = true;

            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.Table<CustomerDocument>().DeleteAsync(a => a.DeviceDocID == customerDocument.DeviceDocID).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                success = false;
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(DeleteExistingCustomerDocumentData), ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }

            return success;
        }


        //public async Task<List<CustomerPageModel>> GetCustomerMasterDataForCustomerPage(string query)
        //{
        //    try
        //    {
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
        //        var result = await db.QueryAsync<CustomerPageModel>(query);
        //        await db.CloseAsync();
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetCustomerMasterDataForCustomerPage), ex.StackTrace);

        //        return null;
        //    }
        //}

        public async Task<List<CustomerPageModel>> GetCustomerMasterDataForCustomerPage(string query)
        {
            List<CustomerPageModel> result = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<CustomerMaster>().ConfigureAwait(false);
                result = await db.QueryAsync<CustomerPageModel>(query).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetCustomerMasterDataForCustomerPage), ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return result;
        }

        //public async Task<List<ProductDistribution>> GetProductDistributionsDataForSelectedCustomer(int customerId, bool isDirectCustomer)
        //{
        //    List<ProductDistribution> productDistributionsItems;

        //    try
        //    {
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);

        //        if (isDirectCustomer)
        //        {
        //            productDistributionsItems = await db.Table<ProductDistribution>().Where(x => x.isExported == 1 && x.IsDeleted == 0 && x.CustomerId == customerId).ToListAsync();
        //        }
        //        else
        //        {
        //            productDistributionsItems = await db.Table<ProductDistribution>().Where(x => x.IsDeleted == 0 && x.CustomerId == customerId).ToListAsync();
        //        }

        //        return productDistributionsItems;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetProductDistributionsDataForSelectedCustomer), ex.StackTrace);

        //        return null;
        //    }
        //}
        public async Task<List<ProductDistribution>> GetProductDistributionsDataForSelectedCustomer(int customerId, bool isDirectCustomer)
        {
            List<ProductDistribution> productDistributionsItems = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<ProductDistribution>();
                if (isDirectCustomer)
                {
                    productDistributionsItems = await db.Table<ProductDistribution>().Where(x => x.isExported == 1 && x.IsDeleted == 0 && x.CustomerId == customerId).ToListAsync();
                }
                else
                {
                    productDistributionsItems = await db.Table<ProductDistribution>().Where(x => x.IsDeleted == 0 && x.CustomerId == customerId).ToListAsync();
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetProductDistributionsDataForSelectedCustomer", ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false); db = null;
            }
            return productDistributionsItems;
        }

        public async Task<bool> DbExecuteAsync(string query)
        {
            int result;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                result = await db.ExecuteAsync(query).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(DbExecuteAsync), ex.StackTrace);
                result = 0;
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return result > 0;
        }

        //public async Task<bool> UpdateDatabaseTableAfterAPICall(string queryString)
        //{
        //    try
        //    {
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH);
        //        var result = await db.ExecuteAsync(queryString);
        //        await db.CloseAsync();
        //        return result > 0;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(UpdateDatabaseTableAfterAPICall), ex.StackTrace);
        //        return false;
        //    }
        //}

        public async Task<bool> UpdateDatabaseTableAfterAPICall(string queryString)
        {
            int result = 0;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                if (queryString.IndexOf("CustomerMaster") != -1)
                {
                    await db.CreateTableAsync<CustomerMaster>().ConfigureAwait(false);
                }
                else if (queryString.IndexOf("OrderMaster") != -1)
                {
                    await db.CreateTableAsync<OrderMaster>().ConfigureAwait(false);
                }
                else if (queryString.IndexOf("CallActivityList") != -1)
                {
                    await db.CreateTableAsync<CallActivityList>().ConfigureAwait(false);
                }
                else if (queryString.IndexOf("ScheduledRoutes") != -1)
                {
                    await db.CreateTableAsync<ScheduledRoutes>().ConfigureAwait(false);
                }
                else if (queryString.IndexOf("UserTaxStatement") != -1)
                {
                    await db.CreateTableAsync<UserTaxStatement>().ConfigureAwait(false);
                }
                else if (queryString.IndexOf("CustomerDistributor") != -1)
                {
                    await db.CreateTableAsync<CustomerDistributor>().ConfigureAwait(false);
                }
                else if (queryString.IndexOf("ProductDistribution") != -1)
                {
                    await db.CreateTableAsync<ProductDistribution>().ConfigureAwait(false);
                }
                result = await db.ExecuteAsync(queryString).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(UpdateDatabaseTableAfterAPICall), ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false); db = null;
            }
            return result > 0;
        }
        //public async Task<List<CustomerMaster>> GetCustomerDataCashSaleAsync(int? zoneId, int? regionId, string territoryId, string stateID, string cityName)
        //{
        //    try
        //    {
        //        var queryStringBuilder = new StringBuilder();

        //        queryStringBuilder.Append("SELECT * FROM CustomerMaster WHERE ");
        //        queryStringBuilder.Append(zoneId.HasValue ? string.Format("ZoneId = {0} AND ", zoneId.Value) : string.Empty);
        //        queryStringBuilder.Append(regionId.HasValue ? string.Format("RegionId = {0} AND ", regionId.Value) : string.Empty);
        //        queryStringBuilder.Append(!string.IsNullOrWhiteSpace(territoryId) ? string.Format("TerritoryId = '{0}' AND ", territoryId) : string.Empty);
        //        queryStringBuilder.Append(!string.IsNullOrEmpty(stateID) ? string.Format("PhysicalAddressStateID = '{0}' AND ", stateID) : string.Empty);
        //        queryStringBuilder.Append(!string.IsNullOrEmpty(cityName) ? string.Format("PhysicalAddressCityID = '{0}' AND ", cityName) : string.Empty);
        //        queryStringBuilder.Append("DeviceCustomerID != '0-0' AND isDeleted == 0 AND IsExported != 2 AND AccountType == 2");

        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);

        //        var customerMasterTabel = await db.QueryAsync<CustomerMaster>(queryStringBuilder.ToString());

        //        await db.CloseAsync();

        //        return customerMasterTabel;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "GetCustomerDataCashSaleAsync", ex.StackTrace);
        //        return null;
        //    }ol
        //}
        public async Task<List<CustomerMaster>> GetCustomerDataCashSaleAsync(int? zoneId, int? regionId, string territoryId, string stateID, string cityName)
        {
            List<CustomerMaster> customerMasterTabel = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                var queryStringBuilder = new StringBuilder();
                queryStringBuilder.Append("SELECT * FROM CustomerMaster WHERE ");
                queryStringBuilder.Append(zoneId.HasValue ? string.Format("ZoneId = {0} AND ", zoneId.Value) : string.Empty);
                queryStringBuilder.Append(regionId.HasValue ? string.Format("RegionId = {0} AND ", regionId.Value) : string.Empty);
                queryStringBuilder.Append(!string.IsNullOrWhiteSpace(territoryId) ? string.Format("TerritoryId = '{0}' AND ", territoryId) : string.Empty);
                queryStringBuilder.Append(!string.IsNullOrEmpty(stateID) ? string.Format("PhysicalAddressStateID = '{0}' AND ", stateID) : string.Empty);
                queryStringBuilder.Append(!string.IsNullOrEmpty(cityName) ? string.Format("PhysicalAddressCityID = '{0}' AND ", cityName) : string.Empty);
                queryStringBuilder.Append("DeviceCustomerID != '0-0' AND isDeleted == 0 AND IsExported != 2 AND AccountType == 2");

                await db.CreateTableAsync<CustomerMaster>().ConfigureAwait(false);
                customerMasterTabel = await db.QueryAsync<CustomerMaster>(queryStringBuilder.ToString()).ConfigureAwait(false);

            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "GetCustomerDataCashSaleAsync", ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return customerMasterTabel;
        }

        //public async Task<List<CustomerMaster>> GetCustomerDataForItemSearchAsync(int? zoneId, int? regionId, string territoryId, string stateID, string cityName)
        //{
        //    try
        //    {
        //        var queryStringBuilder = new StringBuilder();

        //        //queryStringBuilder.Append("SELECT * FROM CustomerMaster cm LEFT JOIN OrderMaster ordrmst on ordrmst.CustomerID = cm.CustomerID AND ordrmst.DeviceCustomerID !='' GROUP by cm.DeviceCustomerID");


        //        var queryString = string.Empty;

        //        queryStringBuilder.Append("SELECT CustomerMaster.devicecustomerid as DeviceCustomerID, " +
        //            "Classification.AccountClassificationName as AccountClassificationName" +
        //            "CustomerMaster.LastCallActivityDate, CustomerMaster.AccountClassification as AccountClassification, " +
        //            "CustomerMaster.Rank, CustomerMaster.AccountType, CustomerMaster.customerid as CustomerID, " +
        //            "CustomerMaster.Latitude, CustomerMaster.Longitude " +
        //            "FROM CustomerMaster, Classification LEFT JOIN OrderMaster on OrderMaster.CustomerID = CustomerMaster.CustomerID AND OrderMaster.DeviceCustomerID !='' " +
        //            "WHERE ");
        //        queryStringBuilder.Append(zoneId.HasValue ? string.Format("CustomerMaster.ZoneId = {0} AND ", zoneId.Value) : string.Empty);
        //        queryStringBuilder.Append(regionId.HasValue ? string.Format("CustomerMaster.RegionId = {0} AND ", regionId.Value) : string.Empty);
        //        queryStringBuilder.Append(!string.IsNullOrWhiteSpace(territoryId) ? string.Format("CustomerMaster.TerritoryId = '{0}' AND ", territoryId) : string.Empty);
        //        queryStringBuilder.Append(!string.IsNullOrEmpty(stateID) ? string.Format("CustomerMaster.PhysicalAddressStateID = '{0}' AND ", stateID) : string.Empty);
        //        queryStringBuilder.Append(!string.IsNullOrEmpty(cityName) ? string.Format("CustomerMaster.PhysicalAddressCityID = '{0}' AND ", cityName) : string.Empty);
        //        queryStringBuilder.Append("CustomerMaster.DeviceCustomerID != '0-0' AND CustomerMaster.AccountType == 2 ");
        //        queryStringBuilder.Append("CustomerMaster.AccountClassification = Classification.AccountClassificationId  ");
        //        queryStringBuilder.Append("Classification.AccountClassificationId NOT IN(3, 7, 20) ");
        //        queryStringBuilder.Append("GROUP by CustomerMaster.DeviceCustomerID");

        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);

        //        var stringQuery = queryStringBuilder.ToString();

        //        var customerMasterTabel1 = await db.QueryAsync<MapCustomerData>(stringQuery);

        //        var customerMasterTabel = await db.QueryAsync<CustomerMaster>(stringQuery);

        //        await db.CloseAsync();

        //        return customerMasterTabel;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "GetCustomerDataForItemSearchAsync", ex.StackTrace);
        //        return null;
        //    }
        //}

        public async Task<List<CustomerMaster>> GetCustomerDataForItemSearchAsync(int? zoneId, int? regionId, string territoryId, string stateID, string cityName)
        {
            List<CustomerMaster> customerMasterTabel = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                var queryStringBuilder = new StringBuilder();

                //queryStringBuilder.Append("SELECT * FROM CustomerMaster cm LEFT JOIN OrderMaster ordrmst on ordrmst.CustomerID = cm.CustomerID AND ordrmst.DeviceCustomerID !='' GROUP by cm.DeviceCustomerID");


                var queryString = string.Empty;

                queryStringBuilder.Append("SELECT CustomerMaster.devicecustomerid as DeviceCustomerID, " +
                    "Classification.AccountClassificationName as AccountClassificationName" +
                    "CustomerMaster.LastCallActivityDate, CustomerMaster.AccountClassification as AccountClassification, " +
                    "CustomerMaster.Rank, CustomerMaster.AccountType, CustomerMaster.customerid as CustomerID, " +
                    "CustomerMaster.Latitude, CustomerMaster.Longitude " +
                    "FROM CustomerMaster, Classification LEFT JOIN OrderMaster on OrderMaster.CustomerID = CustomerMaster.CustomerID AND OrderMaster.DeviceCustomerID !='' " +
                    "WHERE ");
                queryStringBuilder.Append(zoneId.HasValue ? string.Format("CustomerMaster.ZoneId = {0} AND ", zoneId.Value) : string.Empty);
                queryStringBuilder.Append(regionId.HasValue ? string.Format("CustomerMaster.RegionId = {0} AND ", regionId.Value) : string.Empty);
                queryStringBuilder.Append(!string.IsNullOrWhiteSpace(territoryId) ? string.Format("CustomerMaster.TerritoryId = '{0}' AND ", territoryId) : string.Empty);
                queryStringBuilder.Append(!string.IsNullOrEmpty(stateID) ? string.Format("CustomerMaster.PhysicalAddressStateID = '{0}' AND ", stateID) : string.Empty);
                queryStringBuilder.Append(!string.IsNullOrEmpty(cityName) ? string.Format("CustomerMaster.PhysicalAddressCityID = '{0}' AND ", cityName) : string.Empty);
                queryStringBuilder.Append("CustomerMaster.DeviceCustomerID != '0-0' AND CustomerMaster.AccountType == 2 ");
                queryStringBuilder.Append("CustomerMaster.AccountClassification = Classification.AccountClassificationId  ");
                queryStringBuilder.Append("Classification.AccountClassificationId NOT IN(3, 7, 20) ");
                queryStringBuilder.Append("GROUP by CustomerMaster.DeviceCustomerID");

                var stringQuery = queryStringBuilder.ToString();

                //var customerMasterTabel1 = await db.QueryAsync<MapCustomerData>(stringQuery);

                customerMasterTabel = await db.QueryAsync<CustomerMaster>(stringQuery).ConfigureAwait(false);

            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "GetCustomerDataForItemSearchAsync", ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return customerMasterTabel;
        }

        //public async Task<List<OrderMaster>> GetOrderMasterDataForCashSales()
        //{
        //    try
        //    {
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);

        //        var orderMasters = await db.Table<OrderMaster>().Where(x => x.SalesType == "1").ToListAsync();

        //        await db.CloseAsync();

        //        return orderMasters;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetOrderMasterDataForCashSales", ex);
        //        return null;
        //    }
        //}
        public string SqlLiteDateTimeFormat(string colName)
        {
            return string.Format("datetime( substr({0},7,4)||'-'||substr({0},1,2)||'-'||substr({0},4,2)||' '||substr({0},12) )", colName);
        }
        public string SqlLiteDateFormat(string colName)
        {
            return string.Format("date( substr({0},7,4)||'-'||substr({0},1,2)||'-'||substr({0},4,2) )", colName);
        }

        public async Task<List<OrderMaster>> GetOrderMasterDataForCashSales(DateTime? startDateTime, DateTime? endDateTime)
        {
            List<OrderMaster> orderMasters = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                string dateFilter = "";
                if (startDateTime.HasValue && endDateTime.HasValue)
                {
                    string formattedStartDate = DateTimeHelper.ConvertToDbInsertDateTimeFormat(startDateTime.Value);
                    string formattedEndDate = DateTimeHelper.ConvertToDbInsertDateTimeFormat(endDateTime.Value);
                    dateFilter = string.Format("AND ( {0} >= {1} AND {0} <= {2} )",
                  SqlLiteDateFormat("CreatedDate"),
                  SqlLiteDateFormat("'" + formattedStartDate + "'"),
                  SqlLiteDateFormat("'" + formattedEndDate + "'")
                  );
                }
                await db.CreateTableAsync<OrderMaster>().ConfigureAwait(false);

                //orderMasters = await db.Table<OrderMaster>().Where(x => x.SalesType == "1" ).ToListAsync().ConfigureAwait(false);
                string query = string.Format(@"select Sum(cast(GrandTotal as double)) as GrandTotal,devicecustomerid,1 as SalesType 
                                from OrderMaster Where devicecustomerid != '0-0' AND SalesType = 1 {0} GROUP by devicecustomerid", dateFilter);

                orderMasters = await db.QueryAsync<OrderMaster>(query).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(GetType().Name, "GetOrderMasterDataForCashSales", ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return orderMasters;
        }

        //public async Task<List<Classification>> GetClassificationAsync()
        //{
        //    try
        //    {
        //        string queryString = "SELECT * FROM Classification";

        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);

        //        var table = await db.QueryAsync<Classification>(queryString);

        //        await db.CloseAsync();

        //        return table;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "GetClassificationAsync", ex.Message);
        //        return null;
        //    }
        //}
        public async Task<List<Classification>> GetClassificationAsync()
        {
            List<Classification> table = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                string queryString = "SELECT * FROM Classification";
                table = await db.QueryAsync<Classification>(queryString).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "GetClassificationAsync", ex.Message);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return table;
        }

        //public async Task<List<string>> GetListOfDeviceCustomerIdsFromOrderDetailsAndOrderMaster(int productId)
        //{
        //    List<string> deviceCustomerIdsList = new List<string>();

        //    List<OrderDetail> orderDetailsList = new List<OrderDetail>();

        //    try
        //    {
        //        SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);

        //        orderDetailsList = await db.Table<OrderDetail>().Where(x => x.ProductId == productId).ToListAsync();

        //        var deviceOrderIds = orderDetailsList.Select(x => x.DeviceOrderID).ToList();

        //        var orderMasterList = await db.Table<OrderMaster>().Where(y => deviceOrderIds.Contains(y.DeviceOrderID)).ToListAsync();

        //        deviceCustomerIdsList = orderMasterList.Where(d => d.DeviceCustomerID != string.Empty).Select(s => s.DeviceCustomerID).ToList();

        //        await db.CloseAsync();

        //        return deviceCustomerIdsList;

        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "GetListOfDeviceCustomerIdsFromOrderDetailsAndOrderMaster", ex.StackTrace);

        //        return deviceCustomerIdsList;
        //    }
        //}
        public async Task<List<string>> GetListOfDeviceCustomerIdsFromOrderDetailsAndOrderMaster(int productId)
        {
            List<string> deviceCustomerIdsList = new List<string>();

            List<OrderDetail> orderDetailsList = new List<OrderDetail>();

            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                orderDetailsList = await db.Table<OrderDetail>().Where(x => x.ProductId == productId).ToListAsync().ConfigureAwait(false);

                var deviceOrderIds = orderDetailsList.Select(x => x.DeviceOrderID).ToList();

                var orderMasterList = await db.Table<OrderMaster>().Where(y => deviceOrderIds.Contains(y.DeviceOrderID)).ToListAsync().ConfigureAwait(false);

                deviceCustomerIdsList = orderMasterList.Where(d => d.DeviceCustomerID != string.Empty).Select(s => s.DeviceCustomerID).ToList();
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "GetListOfDeviceCustomerIdsFromOrderDetailsAndOrderMaster", ex.StackTrace);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return deviceCustomerIdsList;
        }

        public async Task<Dictionary<string, string>> GetConfiguration()
        {
            Dictionary<string, string> result = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                var itemCollection = await db.Table<Configuration>().ToListAsync().ConfigureAwait(false);
                if (itemCollection.Any())
                {
                    result = new Dictionary<string, string>();
                    foreach (var item in itemCollection)
                    {
                        result.Add(item.KEYName, item.KEYValue);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetConfiguration), ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return result;
        }
        public async Task<string> GetConfigurationValueAsync(string _keyName)
        {
            string result = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<Configuration>();
                Configuration config = await db.FindWithQueryAsync<Configuration>($"SELECT ConfigID,KEYName,KEYValue FROM Configuration WHERE KEYName = '{_keyName}'")
                    .ConfigureAwait(false);
                if (config != null) result = config.KEYValue;
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetConfigurationValueAsync), ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return result;
        }
        public async Task<ProductDetailUiModel> IsCustomerInSRCAsync(int _customerId)
        {
            ProductDetailUiModel productDetailUiModel = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                await db.CreateTableAsync<Configuration>();
                ProductMaster pm = await db.FindWithQueryAsync<ProductMaster>($@"SELECT pm.ProductId
                FROM ProductMaster pm
                INNER JOIN BrandData bd ON pm.CatId=bd.CatId AND pm.BrandId=bd.BrandId
                WHERE pm.CustomerId='{_customerId}' AND lower(bd.BrandName)=lower('Planogram') AND pm.isDeleted = 0")
                   .ConfigureAwait(false);
                if (pm != null) { productDetailUiModel = new ProductDetailUiModel { ProductId = pm.ProductID }; }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), nameof(GetConfigurationValueAsync), ex);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return productDetailUiModel;
        }

        public async Task<List<AVPMaster>> GetAVPMastersAsync()
        {
            List<AVPMaster> table = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                string queryString = "SELECT * FROM AVPMaster";
                table = await db.QueryAsync<AVPMaster>(queryString).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "GetAVPMastersAsync", ex.Message);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return table;
        }

        public async Task<AVPMaster> GetAVPMasterByIdAsync(int avpId)
        {
            AVPMaster avpMaster = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                string queryString = $"SELECT * FROM AVPMaster WHERE AVPID = {avpId}";
                avpMaster = await db.FindWithQueryAsync<AVPMaster>(queryString).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "GetAVPMastersAsync", ex.Message);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return avpMaster;
        }

        public async Task<List<ZoneMaster>> GetZoneMastersAsync()
        {
            List<ZoneMaster> table = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                string queryString = "SELECT * FROM ZoneMaster";
                table = await db.QueryAsync<ZoneMaster>(queryString).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "GetZoneMastersAsync", ex.Message);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return table;
        }

        public async Task<ZoneMaster> GetZoneMasterByIdAsync(int zoneId)
        {
            ZoneMaster zoneMaster = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                string queryString = $"SELECT * FROM ZoneMaster WHERE ZoneID = {zoneId}";
                zoneMaster = await db.FindWithQueryAsync<ZoneMaster>(queryString).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "GetZoneMastersAsync", ex.Message);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return zoneMaster;
        }

        public async Task<TerritoryMaster> GetTerritoryMasterByIdAsync(int id)
        {
            TerritoryMaster territoryMaster = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                string queryString = $"SELECT * FROM TerritoryMaster WHERE TerritoryID = {id}";
                territoryMaster = await db.FindWithQueryAsync<TerritoryMaster>(queryString).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "GetTerritoryMastersAsync", ex.Message);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return territoryMaster;
        }

        public async Task<BDMaster> GetBDMasterByIdAsync(int id)
        {
            BDMaster bdMaster = null;
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                string queryString = $"SELECT * FROM BDMaster WHERE BDID = {id}";
                bdMaster = await db.FindWithQueryAsync<BDMaster>(queryString).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "GetBDMastersAsync", ex.Message);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return bdMaster;
        }

        public async Task<int> GetRoleIdAsync(string roleName)
        {
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                string queryString = $"SELECT RoleId FROM RoleMaster WHERE RoleName = '{roleName}'";
                var roleMaster = await db.FindWithQueryAsync<RoleMaster>(queryString).ConfigureAwait(false);
                if (roleMaster != null) return roleMaster.RoleID;
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "GetRoleIdAsync", ex.Message);
            }
            finally
            {
                await db.CloseAsync().ConfigureAwait(false);
            }
            return 0;
        }

        public async Task<string> GetUserFullNameAsync(string defTerritoryId)
        {
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(ApplicationConstants.DATABASE_PATH, false);
            try
            {
                string queryString = $@"SELECT TRIM(IFNULL(FirstName, '') || ' ' || IFNULL(LastName, '')) AS FullName 
                    FROM UserMaster WHERE DefTerritoryId = {defTerritoryId} AND IsInActive=0 AND IsDeleted=0";
                string fullName = await db.ExecuteScalarAsync<string>(queryString).ConfigureAwait(false);
                if (!string.IsNullOrWhiteSpace(fullName)) return fullName;
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(DatabaseService), "GetUserFullNameAsync", ex.Message);
            }
            return null;
        }
    }
}