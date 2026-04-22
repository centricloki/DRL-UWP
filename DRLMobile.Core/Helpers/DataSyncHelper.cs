using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using DRLMobile.Core.Enums;
using DRLMobile.Core.Interface;
using DRLMobile.Core.Models.DataModels;
using DRLMobile.Core.Models.DataSyncRequestModels;
using DRLMobile.Core.Services;
using DRLMobile.ExceptionHandler;

using Newtonsoft.Json;

using RestSharp.Deserializers;

using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Controls;

namespace DRLMobile.Core.Helpers
{
    public static class DataSyncHelper
    {
        private static readonly string FILE_NAME = "DataSyncHelper";

        private static readonly IQueryService queryService = new QueryService();
        private static readonly IDatabaseService DbService = new DatabaseService();
        public static string LatestSyncDateTime { get; set; }
        public static async Task<string> SyncDataAfterUserLogin(string userName, string pin, string lastSyncDate, string userId)
        {
            string returnMsg = "";
            try
            {
                if (await queryService.UploadDataOnPartialSync(userName, pin, lastSyncDate))
                {
                    string downloadedData = null; SyncDataModel deserialized = null;
                    string oldterritoryid = await queryService.GetTerritoriesBeforeSyncOfUserAsync(userName, pin);
                    downloadedData = await InvokeWebService.DataDownloadService(userName, Convert.ToInt32(pin), lastSyncDate, oldterritoryid).ConfigureAwait(false);

                    if (!string.IsNullOrWhiteSpace(downloadedData))
                    {
                        deserialized = JsonConvert.DeserializeObject<SyncDataModel>(downloadedData);

                        if (deserialized != null && Convert.ToInt32(deserialized.responsestatus) == 200)
                        {
                            if ((!string.IsNullOrEmpty(deserialized.errormsg) && deserialized.errormsg.ToLower().Contains("exceeded")) || string.IsNullOrEmpty(deserialized.errormsg))
                            {
                                await queryService.DownloadDataOnPartialSync(deserialized, userId);
                                if (!string.IsNullOrWhiteSpace(deserialized.lastsyncutcdate))
                                    LatestSyncDateTime = deserialized.lastsyncutcdate;
                            }
                        }
                        returnMsg = deserialized.errormsg;
                    }
                    else
                    {
                        returnMsg = "Something went wrong.Please try again later.";
                    }
                }
                else
                {
                    returnMsg = "Something went wrong.Please try again later.";
                }

            }
            catch (Exception ex)
            {
                if (ex is WebException)
                {
                    throw ex;
                }
                else
                {
                    ErrorLogger.WriteToErrorLog(FILE_NAME, "SyncDataAfterUserLogin", ex);
                    returnMsg = "Something went wrong.Please try again later.";
                }
            }
            return returnMsg;
        }

        public static List<AddEditCustomerModel> GetCustomersListForUpload(List<CustomerMaster> customers)
        {
            List<AddEditCustomerModel> customerDataToUpload = new List<AddEditCustomerModel>();

            foreach (var customerItem in customers)
            {
                var customer = new AddEditCustomerModel()
                {
                    TaxStatement = customerItem.TaxStatement,
                    accountclassification = string.IsNullOrWhiteSpace(customerItem.AccountClassification) ? 0 : Convert.ToInt32(customerItem.AccountClassification),
                    accountresponsibility = customerItem.AccountResponsibility,
                    accounttype = customerItem.AccountType,
                    broker = customerItem.Broker,
                    buyer = customerItem.Buyer,
                    contactemail = customerItem.ContactEmail,
                    contactname = customerItem.ContactName,
                    contactphone = customerItem.ContactPhone,
                    contactrole = customerItem.ContactRole,
                    createdate = customerItem.CreatedDate,
                    customername = customerItem.CustomerName,
                    devicecustomerid = customerItem.DeviceCustomerID,
                    devicedistributorcustomerid = customerItem.DistributorID,
                    emailid = customerItem.EmailID,
                    fax = customerItem.Fax,
                    generalcomments = customerItem.GeneralComments,
                    iscreatepermanent = 0,
                    mailingaddress = customerItem.MailingAddress,
                    mailingaddresscityid = customerItem.MailingAddressCityID,
                    mailingaddressstateid = customerItem.MailingAddressStateID,
                    //mailingaddresszipcode = ZipCodeConversion(customerItem.MailingAddressZipID),
                    managername = customerItem.ManagerName,
                    phone = customerItem.Phone,
                    physicaladdress = customerItem.PhysicalAddress,
                    physicaladdresscityid = customerItem.PhysicalAddressCityID,
                    physicaladdressstateid = customerItem.PhysicalAddressStateID,
                    //physicaladdresszipcode = ZipCodeConversion(customerItem.PhysicalAddressZipCode),
                    ranking = customerItem.Rank,
                    retailerlicense = customerItem.RetailerLicense,
                    retailersalestaxcertificate = customerItem.RetailerSalesTaxCertificate,
                    shippingaddress = customerItem.ShippingAddress,
                    shippingaddresscityid = customerItem.ShippingAddressCityID,
                    shippingaddressstateid = customerItem.ShippingAddressStateID,
                    //shippingaddresszipcode = ZipCodeConversion(customerItem.ShippingAddressZipCode),
                    statetobaccolicense = customerItem.StateTobaccoLicense,
                    storecount = string.IsNullOrWhiteSpace(customerItem.StoreCount) ? 0 : Convert.ToInt32(customerItem.StoreCount),
                    territoryid = string.IsNullOrWhiteSpace(customerItem.TerritoryID) ? 0 : Convert.ToInt32(customerItem.TerritoryID),
                    isexported = customerItem.IsExported,
                    OrderDeliveryWeekDays = customerItem.OrderDeliveryWeekDays,
                    isdeleted = customerItem.isDeleted
                };
                customer.mailingaddresszipcode = ZipCodeConversion(customerItem.MailingAddressZipID);
                customer.physicaladdresszipcode = ZipCodeConversion(customerItem.PhysicalAddressZipCode);
                customer.shippingaddresszipcode = ZipCodeConversion(customerItem.ShippingAddressZipCode);
                customerDataToUpload.Add(customer);
            }

            return customerDataToUpload;
        }

        private static string ZipCodeConversion(string paramZipCode)
        {
            int validZipCode;
            string defaultZipCode = "0";

            if (string.IsNullOrWhiteSpace(paramZipCode)) return defaultZipCode;

            bool success = int.TryParse(paramZipCode, out validZipCode);
            if (success)
            {
                return (paramZipCode.Length > 5) ? defaultZipCode : paramZipCode;
            }
            else
            {
                return defaultZipCode;
            }
        }

        public static List<AddEditContactModel> GetContactsListForUpload(List<ContactMaster> contacts)
        {
            List<AddEditContactModel> contactDataToUpload = new List<AddEditContactModel>();

            foreach (var contactItem in contacts)
            {
                var contact = new AddEditContactModel()
                {
                    contactemail = contactItem.ContactEmail,
                    contactfax = contactItem.ContactFax,
                    contactid = contactItem.ContactID.Value,
                    contactname = contactItem.ContactName,
                    contactphone = contactItem.ContactPhone,
                    contactrole = contactItem.ContactRole,
                    createdate = contactItem.CreatedDate,
                    deleted = contactItem.IsDeleted,
                    devicecontactid = contactItem.DeviceContactID,
                    devicecustomerid = contactItem.DeviceCustomerID,
                    importedfrom = 5,
                    isexported = contactItem.IsExported,
                    positionid = contactItem.PositionID,
                    rankid = contactItem.RankID,
                    updatedate = contactItem.UpdatedDate,
                };

                contactDataToUpload.Add(contact);
            }

            return contactDataToUpload;
        }

        public async static Task<List<AddOrderModel>> GetOrdersListForUpload(List<OrderMaster> orders)
        {
            List<AddOrderModel> ordersDataToUpload = new List<AddOrderModel>();
            List<AddOrderDetails> addOrderDetails = null;

            try
            {
                foreach (var orderItem in orders)
                {
                    var orderDetails = await DbService.GetOrderDetailsDataAsync(orderItem.DeviceOrderID);

                    if (orderDetails != null && orderDetails.Count > 0)
                    {
                        addOrderDetails = new List<AddOrderDetails>();
                        orderDetails = orderDetails.OrderBy(x => DateTimeHelper.ConvertToDBDateTime(x.CreatedDate)).ToList();
                        foreach (var item in orderDetails)
                        {
                            var orderDetailItem = new AddOrderDetails
                            {
                                creditrequest = item.CreditRequest,
                                price = Convert.ToDecimal(item.Price),
                                productid = item.ProductId,
                                quantity = item.Quantity,
                                units = item.Unit
                            };
                            addOrderDetails.Add(orderDetailItem);
                        }
                    }

                    var order = new AddOrderModel()
                    {
                        IsVoided = orderItem.IsVoided,
                        createdate = orderItem.CreatedDate,
                        customercomment = string.IsNullOrEmpty(orderItem.CustomerComment) ? "" : orderItem.CustomerComment,
                        customername = orderItem.CustomerName,
                        customerparentid = orderItem.CustomerDistributorID,
                        customershippingcityid = orderItem.CustomerShippingCityID,
                        customershippingstateid = orderItem.CustomerShippingStateID,
                        customershippingzipcode = orderItem.CustomerShippingZipCode,
                        customstatement = orderItem.CustomStatement,
                        devicecustomerid = orderItem.DeviceCustomerID,
                        deviceorderid = orderItem.DeviceOrderID,
                        emailrecipients = string.IsNullOrEmpty(orderItem.EmailRecipients) ? "" : orderItem.EmailRecipients,
                        grandtotal = orderItem.GrandTotal,
                        invoicenumber = orderItem.InvoiceNumber,
                        isoderconfirmed = orderItem.IsOrderConfirmed,
                        isorderemailsent = orderItem.IsOrderEmailSent,
                        orderaddress = orderItem.OrderAddress,
                        ordercityid = orderItem.OrderCityId,
                        orderdate = orderItem.OrderDate,
                        ordernumber = orderItem.InvoiceNumber,
                        orderstateid = orderItem.OrderStateId,
                        orderzipcode = orderItem.OrderZipCode,
                        prebookshipdate = orderItem.PrebookShipDate,
                        printname = orderItem.PrintName,
                        purchaseordernumber = orderItem.PurchaseOrderNumber,
                        republicsalesrepository = orderItem.RepublicSalesRepository,
                        retailerlicense = orderItem.RetailerLicense,
                        retailersalestaxcertificate = orderItem.RetailerSalesTaxCertificate,
                        salestype = Convert.ToInt32(orderItem.SalesType),
                        sellername = orderItem.OrderMasterSellerName,
                        sellerreptobaccopermitno = orderItem.OrderMasterSellerRepTobacco,
                        statetobaccolicense = orderItem.StateTobaccoLicence,
                        orderdetails = addOrderDetails,
                        isexported = orderItem.IsExported,
                        retaildistributornumber = orderItem.RetailDistributorNumber
                    };

                    ordersDataToUpload.Add(order);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(FILE_NAME, "GetOrdersListForUpload", ex);
            }

            return ordersDataToUpload;
        }

        public static List<AddCallActivityModel> GetCallActivityListForUpload(List<CallActivityList> callActivities)
        {
            List<AddCallActivityModel> callActivitiesDataToUpload = new List<AddCallActivityModel>();

            foreach (var callActivityItem in callActivities)
            {
                var callActivity = new AddCallActivityModel()
                {
                    IsVoided = callActivityItem.IsVoided,
                    activitytype = callActivityItem.ActivityType,
                    grandtotal = Convert.ToInt32(callActivityItem.GrandTotal),
                    calldate = callActivityItem.CallDate,
                    comments = callActivityItem.Comments,
                    createdate = callActivityItem.CreatedDate,
                    devicecallactivityid = callActivityItem.CallActivityDeviceID,
                    devicecustomerid = callActivityItem.CustomerID,
                    deviceorderid = callActivityItem.OrderID,
                    gratisproductused = callActivityItem.GratisProduct,
                    hours = string.IsNullOrEmpty(callActivityItem.Hours) ? 0 : Convert.ToInt32(callActivityItem.Hours),
                    isthisaccountfromyourlist = 1,
                    objective = callActivityItem.Objective,
                    result = callActivityItem.Result,
                    orderdeviceid = callActivityItem.OrderID,
                    ConsumerActivationEngagement = !string.IsNullOrWhiteSpace(callActivityItem.ConsumerActivationEngagement) ? callActivityItem.ConsumerActivationEngagement : null
                };

                if (callActivity.activitytype.Equals("Narrative-Weekly Report", StringComparison.OrdinalIgnoreCase))
                {
                    callActivity.MarketsVisited = callActivityItem.MarketsVisited;
                    callActivity.CallsMadeVsGoal = callActivityItem.CallsMadeVsGoal;
                    callActivity.NewCustomerAcquisitions = callActivityItem.NewCustomerAcquisitions;
                    callActivity.KeyWinsSummary = callActivityItem.KeyWinsSummary;
                    callActivity.ChallengesAndFeedback = callActivityItem.ChallengesAndFeedback;
                    callActivity.NextCyclePlan = callActivityItem.NextCyclePlan;
                    callActivity.NextWeekPlan = callActivityItem.NextWeekPlan;
                }
                callActivitiesDataToUpload.Add(callActivity);
            }

            return callActivitiesDataToUpload;
        }

        public async static Task<List<AddRoutesModel>> GetScheduleRouteListForUpload(List<ScheduledRoutes> scheduledRoutes)
        {
            List<AddRoutesModel> scheduledRoutesDataToUpload = new List<AddRoutesModel>();


            foreach (var scheduledRouteItem in scheduledRoutes)
            {
                var query = string.Format("SELECT RouteStations.CustomerId from RouteStations WHERE RouteStations.RouteId in ({0})", scheduledRouteItem.RouteId);

                var customerIdsList = await DbService.GetCustomerIdsFromRouteStationTableByRouteId(query);

                var scheduleRoute = new AddRoutesModel()
                {
                    addressname = scheduledRouteItem.AddressName,
                    cityid = scheduledRouteItem.CityId,
                    // Ak comented
                    //createdate = scheduledRouteItem.CreatedDate,

                    createdate = scheduledRouteItem.CreatedDate,
                    devicerouteid = scheduledRouteItem.DeviceRouteId,
                    discription = scheduledRouteItem.RouteBrief,
                    enddate = scheduledRouteItem.EndDate,
                    housenumber = scheduledRouteItem.HouseNo,
                    idAssignToTSM = scheduledRouteItem.idAssignToTSM,
                    latitude = scheduledRouteItem.Latitude,
                    longitude = scheduledRouteItem.Longitude,
                    // commneted by AK teh createddate line/
                    // added datetime now as it required a non utc time.
                    planneddate = DateTime.Now.ToString("MM/dd/yyyy").Replace("-", "/"),
                    //  planneddate = scheduledRouteItem.CreatedDate.ToString()

                    routedescription = scheduledRouteItem.RouteBrief,
                    routename = scheduledRouteItem.RouteName,
                    routetype = scheduledRouteItem.RouteType,
                    //  startdate = scheduledRouteItem.StartDate,
                    startdate = scheduledRouteItem.StartDate,
                    streetname = scheduledRouteItem.StreetName,
                    userid = scheduledRouteItem.UserId,
                    zipcode = scheduledRouteItem.Zipcode,
                    customrdata = customerIdsList.Count() > 0 ? customerIdsList.ToList() : null,
                    isdeleted = scheduledRouteItem.IsDeleted,
                };

                scheduledRoutesDataToUpload.Add(scheduleRoute);
            }

            return scheduledRoutesDataToUpload;
        }

        public static List<AddUserTaxStatement> GetUserTaxStatementListForUpload(List<UserTaxStatement> userTaxStatements)
        {
            List<AddUserTaxStatement> userTaxStatementDataToUpload = new List<AddUserTaxStatement>();

            foreach (var userTaxStatementItem in userTaxStatements)
            {
                var usertaxstatement = new AddUserTaxStatement()
                {
                    CreatedBy = userTaxStatementItem.CreatedBy,
                    CreatedDate = userTaxStatementItem.CreatedDate,
                    Description = userTaxStatementItem.Description,
                    DeviceUserTaxStatementID = userTaxStatementItem.DeviceUserTaxStatementID,
                    IsDeleted = userTaxStatementItem.IsDeleted,
                    IsExported = userTaxStatementItem.IsExported,
                    Title = userTaxStatementItem.Title,
                    UpdatedBy = userTaxStatementItem.UpdatedBy,
                    UpdatedDate = userTaxStatementItem.UpdatedDate,
                    UserID = userTaxStatementItem.UserID,
                    UserTaxStatementID = userTaxStatementItem.UserTaxStatementID
                };

                userTaxStatementDataToUpload.Add(usertaxstatement);
            }

            return userTaxStatementDataToUpload;
        }

        public static List<AddCustomerDistributorModel> GetCustomerDistributorListForUpload(List<CustomerDistributor> customerDistributors)
        {
            List<AddCustomerDistributorModel> customerDistributorDataToUpload = new List<AddCustomerDistributorModel>();

            foreach (var custDistributor in customerDistributors)
            {
                var custDistriData = new AddCustomerDistributorModel()
                {
                    DistributorID = Convert.ToInt32(custDistributor.DistributorID),
                    DeviceCustomerID = custDistributor.DeviceCustomerID,
                    CreatedDate = custDistributor.CreatedDate,
                    CreatedBy = Convert.ToInt32(custDistributor.CreatedBy),
                    UpdatedBy = Convert.ToInt32(custDistributor.UpdatedBy),
                    UpdatedDate = custDistributor.UpdatedDate,
                    DistributorPriority = custDistributor.DistributorPriority,
                    IsExported = 0,
                    IsDeleted = custDistributor.IsDeleted
                };

                customerDistributorDataToUpload.Add(custDistriData);
            }

            return customerDistributorDataToUpload;
        }

        public static List<AddFavoriteModel> GetFavoriteListForUpload(List<Favorite> favorites)
        {
            List<AddFavoriteModel> favoriteDataToUpload = new List<AddFavoriteModel>();

            foreach (var favoriteItem in favorites)
            {
                var favoriteData = new AddFavoriteModel()
                {
                    userid = favoriteItem.UserId,
                    productid = favoriteItem.ProductId,
                    isdeleted = favoriteItem.isDeleted
                };

                favoriteDataToUpload.Add(favoriteData);
            }

            return favoriteDataToUpload;
        }

        public async static Task<List<AddProductDistributionModel>> GetProductDistributionDataForUpload(List<ProductDistribution> productDistributions)
        {
            List<AddProductDistributionModel> productDistributionDataToUpload = new List<AddProductDistributionModel>();

            foreach (var item in productDistributions)
            {
                var customer = await DbService.GetSavedCustomerInfoAsync(item.CustomerId).ConfigureAwait(false);

                var prodcutDistriData = new AddProductDistributionModel()
                {
                    devicecustomerproductid = string.IsNullOrEmpty(item.CustomerProductID) ? 0 : Convert.ToInt32(item.CustomerProductID),
                    productid = item.ProductId,
                    customerid = item.CustomerId,
                    devicecustomerid = customer?.DeviceCustomerID,
                    lastdistributionrecorddate = item.DistributionDate,
                    isdeleted = item.IsDeleted
                };

                productDistributionDataToUpload.Add(prodcutDistriData);
            }

            return productDistributionDataToUpload;
        }

        public async static Task<List<UploadCustomerDocumentsRequestModel>> GetCustomerDocumentsForUpload(List<CustomerDocument> customerDocuments, string userName, string pin, string lastSyncDate)
        {
            List<UploadCustomerDocumentsRequestModel> customerDocumentsList = new List<UploadCustomerDocumentsRequestModel>();

            foreach (var customerDocument in customerDocuments.OrderBy(x => x.CreatedDateFromServer))
            {

                var deviceCustomerId = await DbService
                    .GetCustomerDeviceIdForDocumentUpload(customerDocument.CustomerID).ConfigureAwait(false);

                byte[] FileBytes;
                string localpathName = Path.Combine(ApplicationConstants.APP_PATH, ApplicationConstants.CustomerDocumentsSubFolder,
                    HelperMethods.GetNameFromURL(customerDocument.CustomerDocumentName));
                if (customerDocument.IsDelete == "1" || customerDocument.IsDelete == "true")
                {
                    var customerDocToUpload = new UploadCustomerDocumentsRequestModel();

                    customerDocToUpload.customerid = customerDocument.CustomerID;
                    customerDocToUpload.devicedocumentid = customerDocument.DeviceDocID;
                    customerDocToUpload.description = customerDocument.CustomerDocDesc;
                    customerDocToUpload.devicecustomerid = deviceCustomerId;
                    customerDocToUpload.documenttype = customerDocument.CustomerDocType;
                    customerDocToUpload.IsPublishedToChild = !string.IsNullOrEmpty(customerDocument.IsPublishedToChild) && customerDocument.IsPublishedToChild.Equals("1");
                    customerDocToUpload.filename = HelperMethods.GetNameFromURL(customerDocument.CustomerDocumentName);
                    customerDocToUpload.filestream = null;
                    customerDocToUpload.filetype = 2;
                    customerDocToUpload.username = userName;
                    customerDocToUpload.pin = Convert.ToInt32(pin);
                    customerDocToUpload.updatedate = lastSyncDate;
                    customerDocToUpload.versionnumber = ApplicationConstants.APPLICATION_VERSION;
                    customerDocToUpload.isdelete = customerDocument.IsDelete;

                    customerDocumentsList.Add(customerDocToUpload);
                }
                else
                {
                    if (File.Exists(customerDocument.CustomerDocumentName))
                    {
                        using (FileStream fileStream = File.Open(customerDocument.CustomerDocumentName, FileMode.Open))
                        {
                            using (var memoryStream = new MemoryStream())
                            {
                                fileStream.CopyTo(memoryStream);
                                FileBytes = memoryStream.ToArray();
                            }
                        }
                        var customerDocToUpload = new UploadCustomerDocumentsRequestModel();

                        customerDocToUpload.customerid = customerDocument.CustomerID;
                        customerDocToUpload.devicedocumentid = customerDocument.DeviceDocID;
                        customerDocToUpload.description = customerDocument.CustomerDocDesc;
                        customerDocToUpload.devicecustomerid = deviceCustomerId;
                        customerDocToUpload.documenttype = customerDocument.CustomerDocType;
                        customerDocToUpload.IsPublishedToChild = !string.IsNullOrEmpty(customerDocument.IsPublishedToChild) && customerDocument.IsPublishedToChild.Equals("1");
                        customerDocToUpload.filename = customerDocument.CustomerDocumentName.Substring(customerDocument.CustomerDocumentName.IndexOf("CustomerDocuments") + 18);
                        customerDocToUpload.filestream = Convert.ToBase64String(FileBytes);
                        customerDocToUpload.filetype = 2;
                        customerDocToUpload.username = userName;
                        customerDocToUpload.pin = Convert.ToInt32(pin);
                        customerDocToUpload.updatedate = lastSyncDate;
                        customerDocToUpload.versionnumber = ApplicationConstants.APPLICATION_VERSION;
                        customerDocToUpload.isdelete = customerDocument.IsDelete;

                        customerDocumentsList.Add(customerDocToUpload);
                    }
                    else if (File.Exists(localpathName))
                    {

                        using (FileStream fileStream = File.Open(localpathName, FileMode.Open))
                        {
                            using (var memoryStream = new MemoryStream())
                            {
                                fileStream.CopyTo(memoryStream);
                                FileBytes = memoryStream.ToArray();
                            }
                        }
                        var customerDocToUpload = new UploadCustomerDocumentsRequestModel();

                        customerDocToUpload.customerid = customerDocument.CustomerID;
                        customerDocToUpload.devicedocumentid = customerDocument.DeviceDocID;
                        customerDocToUpload.description = customerDocument.CustomerDocDesc;
                        customerDocToUpload.devicecustomerid = deviceCustomerId;
                        customerDocToUpload.documenttype = customerDocument.CustomerDocType;
                        customerDocToUpload.IsPublishedToChild = !string.IsNullOrEmpty(customerDocument.IsPublishedToChild) && customerDocument.IsPublishedToChild.Equals("1");
                        customerDocToUpload.filename = localpathName.Substring(localpathName.IndexOf("CustomerDocuments") + 18);
                        customerDocToUpload.filestream = Convert.ToBase64String(FileBytes);
                        customerDocToUpload.filetype = 2;
                        customerDocToUpload.username = userName;
                        customerDocToUpload.pin = Convert.ToInt32(pin);
                        customerDocToUpload.updatedate = lastSyncDate;
                        customerDocToUpload.versionnumber = ApplicationConstants.APPLICATION_VERSION;
                        customerDocToUpload.isdelete = customerDocument.IsDelete;

                        customerDocumentsList.Add(customerDocToUpload);
                    }
                }
            }

            return customerDocumentsList;
        }

        public static List<UploadSignatureRequestModel> GetOrderSignaturesForUpload(List<OrderMaster> orderDataList, string userName, string pin, string lastSyncDate)
        {
            List<UploadSignatureRequestModel> orderSignatureList = new List<UploadSignatureRequestModel>();

            if (orderDataList != null && orderDataList.Count > 0)
            {
                foreach (var orderItem in orderDataList)
                {
                    byte[] FileBytes;

                    if (File.Exists(orderItem.CustomerSignatureFileName))
                    {
                        using (FileStream fileStream = File.Open(orderItem.CustomerSignatureFileName, FileMode.Open))
                        {
                            using (var memoryStream = new MemoryStream())
                            {
                                fileStream.CopyTo(memoryStream);
                                FileBytes = memoryStream.ToArray();
                            }
                        }
                        var signatureData = new UploadSignatureRequestModel();

                        signatureData.username = userName;
                        signatureData.pin = Convert.ToInt32(pin);
                        signatureData.updatedate = lastSyncDate;
                        signatureData.filename = orderItem.CustomerSignatureFileName.Substring(orderItem.CustomerSignatureFileName.IndexOf("LocalState") + 11);
                        signatureData.filestream = Convert.ToBase64String(FileBytes);
                        signatureData.filetype = 6;
                        signatureData.deviceorderid = orderItem.DeviceOrderID;
                        signatureData.versionnumber = ApplicationConstants.APPLICATION_VERSION;

                        orderSignatureList.Add(signatureData);
                    }
                }
            }

            return orderSignatureList;
        }

        public static async Task DownloadBrandImages()
        {
            try
            {
                var listOfUrl = await queryService.GetBrandImageURL();

                if (listOfUrl != null)
                {
                    foreach (var url in listOfUrl)
                    {
                        if (!string.IsNullOrWhiteSpace(url))
                        {
                            var fileType = (int)FileType.BrandStyle;
                            try
                            {
                                await InvokeWebService.DownloadDocumentAndFileFromServer(HelperMethods.GetNameFromURL(url), fileType.ToString());
                            }
                            catch (Exception ex)
                            {
                                // ErrorLogger.WriteToErrorLog(FILE_NAME, "DownloadBrandImages", ex.StackTrace + " - " + ex.Message);
                                ErrorLogger.WriteToErrorLog(nameof(DataSyncHelper), nameof(DownloadBrandImages), ex, $"url:{url}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(FILE_NAME, "DownloadBrandImages", ex.StackTrace + " - " + ex.Message);
            }
        }

    }
}
