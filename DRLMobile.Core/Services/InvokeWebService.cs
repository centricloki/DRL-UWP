using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

using DRLMobile.Core.Enums;
using DRLMobile.Core.Helpers;
using DRLMobile.Core.Models;
using DRLMobile.Core.Models.DataModels;
using DRLMobile.Core.Models.DataSyncRequestModels;
using DRLMobile.Core.Models.FedExAddressValidationModels;
using DRLMobile.Core.Models.UIModels;
using DRLMobile.ExceptionHandler;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using RestSharp;
using RestSharp.Authenticators;

using Windows.ApplicationModel.Contacts;
using Windows.Storage;

namespace DRLMobile.Core.Services
{
    public static class InvokeWebService
    {
        #region File Constants

        // Constant file name.
        private const string FILE_NAME = "InvokeWebService.cs";

        #endregion

        #region Public Methods

        /// <summary>
        /// Method to authenticate user during login
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns>Login information if login successful</returns>
        public async static Task<LoginUIModel> UserAuthenticateWebService(string userName, int password)
        {
            LoginUIModel loginUIModel = null;

            try
            {
                DataDownloadWebServiceRequestModel requestModel = new DataDownloadWebServiceRequestModel()
                {
                    username = userName,
                    pin = password,
                    versionnumber = ApplicationConstants.APPLICATION_VERSION
                };

                string serviceRequestModel = JsonConvert.SerializeObject(requestModel);

                string getDataDownloadInfo = await GetWebServiceResponse(ApplicationConstants.LOGIN_WEB_METHOD, serviceRequestModel);

                var responseObejct = JsonConvert.DeserializeObject<LoginUIModel>(getDataDownloadInfo);

                loginUIModel = responseObejct;
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(FILE_NAME, "UserAuthenticateWebService", ex);
            }

            return loginUIModel;
        }

        /// <summary>
        /// Method to download data during partial sync
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="updateDate"></param>
        /// <returns>Downloaded data string</returns>
        public async static Task<string> DataDownloadService(string userName, int password, string updateDate, string oldTerritoryId="")
        {
            string downloadedData = string.Empty;

            try
            {
                DataDownloadWebServiceRequestModel requestModel = new DataDownloadWebServiceRequestModel()
                {
                    username = userName,
                    pin = password,
                    versionnumber = ApplicationConstants.APPLICATION_VERSION,
                    updatedate = updateDate,
                    oldterritoryid= oldTerritoryId
                };

                string serviceRequestModel = JsonConvert.SerializeObject(requestModel);

                downloadedData = await GetWebServiceResponse(ApplicationConstants.DOWNLOAD_DATA_WEB_METHOD, serviceRequestModel).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(FILE_NAME, "DataDownloadService", ex);
            }

            return downloadedData;
        }

        public async static Task<DataSyncResponseModel> UploadCustomerDataDuringSyncProcess(string userName, int password, List<AddEditCustomerModel> customers)
        {
            UploadCustomerDataRequestModel requestModel = new UploadCustomerDataRequestModel()
            {
                username = userName,
                pin = password,
                customerdata = customers
            };

            string serviceRequestModel = JsonConvert.SerializeObject(requestModel);

            var customerSyncResponse = await GetWebServiceResponse(ApplicationConstants.UPLOAD_CUSTOMER_DATA_WEB_METHOD, serviceRequestModel).ConfigureAwait(false);

            var responseObejct = JsonConvert.DeserializeObject<DataSyncResponseModel>(customerSyncResponse);

            DataSyncResponseModel dataSyncResponseModel = responseObejct;

            return dataSyncResponseModel;
        }

        public async static Task<DataSyncResponseModel> UploadContactDataDuringSyncProcess(string userName, int password, List<AddEditContactModel> contacts)
        {
            UploadContactDataRequestModel requestModel = new UploadContactDataRequestModel()
            {
                username = userName,
                pin = password,
                contactdata = contacts
            };

            string serviceRequestModel = JsonConvert.SerializeObject(requestModel);

            var contactSyncResponse = await GetWebServiceResponse(ApplicationConstants.UPLOAD_CONTACT_DATA_WEB_METHOD, serviceRequestModel).ConfigureAwait(false);

            var responseObejct = JsonConvert.DeserializeObject<DataSyncResponseModel>(contactSyncResponse);

            DataSyncResponseModel dataSyncResponseModel = responseObejct;

            return dataSyncResponseModel;
        }

        public async static Task<DataSyncResponseModel> UploadCallActivityDataDuringSyncProcess(string userName, int password, List<AddCallActivityModel> callActivities)
        {
            UploadCallActivityDataRequestModel requestModel = new UploadCallActivityDataRequestModel()
            {
                username = userName,
                pin = password,
                callactivitydata = callActivities
            };

            string serviceRequestModel = JsonConvert.SerializeObject(requestModel);

            var callActivitySyncResponse = await GetWebServiceResponse(ApplicationConstants.UPLOAD_CALL_ACTIVITY_DATA_WEB_METHOD, serviceRequestModel).ConfigureAwait(false);

            var responseObejct = JsonConvert.DeserializeObject<DataSyncResponseModel>(callActivitySyncResponse);

            DataSyncResponseModel dataSyncResponseModel = responseObejct;

            return dataSyncResponseModel;
        }

        public async static Task<DataSyncResponseModel> UploadScheduledRouteDataDuringSyncProcess(string userName, int password, List<AddRoutesModel> scheduleRoute)
        {
            UploadRoutesDataRequestModel requestModel = new UploadRoutesDataRequestModel()
            {
                username = userName,
                pin = password,
                routedata = scheduleRoute
            };

            string serviceRequestModel = JsonConvert.SerializeObject(requestModel);

            var routeSyncResponse = await GetWebServiceResponse(ApplicationConstants.UPLOAD_SCHEDULE_ROUTE_DATA_WEB_METHOD, serviceRequestModel);

            var responseObejct = JsonConvert.DeserializeObject<DataSyncResponseModel>(routeSyncResponse);

            DataSyncResponseModel dataSyncResponseModel = responseObejct;

            return dataSyncResponseModel;
        }

        public async static Task<DataSyncResponseModel> UploadUserTaxStatementsDataDuringSyncProcess(string userName, int password, List<AddUserTaxStatement> userTaxStatementData)
        {
            UploadUserTaxStatementDataRequestModel requestModel = new UploadUserTaxStatementDataRequestModel()
            {
                username = userName,
                pin = password,
                versionnumber = ApplicationConstants.APPLICATION_VERSION,
                UserTaxStatement = userTaxStatementData
            };

            string serviceRequestModel = JsonConvert.SerializeObject(requestModel);

            var dataSyncResponse = await GetWebServiceResponse(ApplicationConstants.UPLOAD_USER_TAX_STATEMENT_DATA_WEB_METHOD, serviceRequestModel).ConfigureAwait(false);

            var responseObejct = JsonConvert.DeserializeObject<DataSyncResponseModel>(dataSyncResponse);

            DataSyncResponseModel dataSyncResponseModel = responseObejct;

            return dataSyncResponseModel;
        }

        public async static Task<DataSyncResponseModel> UploadOrderDataDuringSyncProcess(string userName, int password, List<AddOrderModel> addOrders)
        {
            try
            {
                UploadOrdersDataRequestModel requestModel = new UploadOrdersDataRequestModel()
                {
                    username = userName,
                    pin = password,
                    orderdata = addOrders
                };

                string serviceRequestModel = JsonConvert.SerializeObject(requestModel);

                var orderSyncResponse = await GetWebServiceResponse(ApplicationConstants.UPLOAD_ORDER_DATA_WEB_METHOD, serviceRequestModel).ConfigureAwait(false);

                var responseObejct = JsonConvert.DeserializeObject<DataSyncResponseModel>(orderSyncResponse);

                DataSyncResponseModel dataSyncResponseModel = responseObejct;

                return dataSyncResponseModel;
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException("InvokeWebService", "UploadOrderDataDuringSyncProcess", ex);
                return null;
            }
        }

        public async static Task<DataSyncResponseModel> UploadCustomerDistributorDuringSyncProcess(string userName, int password, List<AddCustomerDistributorModel> addCustomerDistributors)
        {
            UploadCustomerDistributorsDataRequestModel requestModel = new UploadCustomerDistributorsDataRequestModel()
            {
                username = userName,
                pin = password,
                customerdistributordata = addCustomerDistributors
            };

            string serviceRequestModel = JsonConvert.SerializeObject(requestModel);

            var data = await GetWebServiceResponse(ApplicationConstants.UPLOAD_CUSTOMER_DISTRIBUTOR_DATA, serviceRequestModel).ConfigureAwait(false);

            var responseObejct = JsonConvert.DeserializeObject<DataSyncResponseModel>(data);

            DataSyncResponseModel dataSyncResponseModel = responseObejct;

            return dataSyncResponseModel;
        }

        public async static Task UploadFavoriteDuringSyncProcess(List<AddFavoriteModel> addFavorites)
        {
            string serviceRequestModel = JsonConvert.SerializeObject(addFavorites);

            await GetWebServiceResponse(ApplicationConstants.UPLOAD_FAVORITE_DATA, serviceRequestModel).ConfigureAwait(false);
        }

        public async static Task<DataSyncResponseModel> UploadProductDistributionDuringSyncProcess(string userName, int password, List<AddProductDistributionModel> addProductDistributions)
        {
            UploadProductDistributionDataRequestModel requestModel = new UploadProductDistributionDataRequestModel()
            {
                username = userName,
                pin = password,
                customerproduct = addProductDistributions
            };

            string serviceRequestModel = JsonConvert.SerializeObject(requestModel);

            var responseData = await GetWebServiceResponse(ApplicationConstants.UPLOAD_PRODUCT_DISTRIBUTION_DATA, serviceRequestModel);

            var responseObejct = JsonConvert.DeserializeObject<DataSyncResponseModel>(responseData);

            DataSyncResponseModel dataSyncResponseModel = responseObejct;

            return dataSyncResponseModel;
        }

        /// <summary>
        /// Method to upload documents and files to server during partial sync
        /// </summary>
        public async static Task<DataSyncResponseModel> UploadUserDocumentToServer(UploadSignatureRequestModel signatureRequestModel, UploadCustomerDocumentsRequestModel customerDocumentsRequestModel)
        {
            string serviceRequestModel = string.Empty;

            if (signatureRequestModel != null)
            {
                serviceRequestModel = JsonConvert.SerializeObject(signatureRequestModel);
            }
            else if (customerDocumentsRequestModel != null)
            {
                serviceRequestModel = JsonConvert.SerializeObject(customerDocumentsRequestModel);
            }

            var responseData = await GetWebServiceResponse(ApplicationConstants.UPLOAD_USER_DOCUMENT, serviceRequestModel)
                .ConfigureAwait(false);

            var responseObejct = JsonConvert.DeserializeObject<DataSyncResponseModel>(responseData);

            DataSyncResponseModel dataSyncResponseModel = responseObejct;

            return dataSyncResponseModel;
        }
        /// <summary>
        /// Method to upload documents and files to server during partial sync
        /// </summary>
        public async static Task<DataSyncResponseModel> UploadActivityToServer(UploadSignatureRequestModel signatureRequestModel, UploadActivityRequestModel uploadActivityRequestModel)
        {
            string serviceRequestModel = string.Empty;

            if (signatureRequestModel != null)
            {
                serviceRequestModel = JsonConvert.SerializeObject(signatureRequestModel);
            }
            else if (uploadActivityRequestModel != null)
            {
                serviceRequestModel = JsonConvert.SerializeObject(uploadActivityRequestModel);
            }

            var responseData = await GetWebServiceResponse(ApplicationConstants.UPLOAD_USER_DOCUMENT, serviceRequestModel)
                .ConfigureAwait(false);

            var responseObejct = JsonConvert.DeserializeObject<DataSyncResponseModel>(responseData);

            DataSyncResponseModel dataSyncResponseModel = responseObejct;

            return dataSyncResponseModel;
        }

        /// <summary>
        /// Method used for downloading the documents and files for various entities
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="fileType"></param>
        /// <returns>Downloaded file's local path</returns>
        public async static Task<string> DownloadDocumentAndFileFromServer(string fileName, string fileType)
        {
            IRestResponse restResponse;

            WebServiceResponseModel webServiceResponse;

            ErrorLogger.ApplicationPath = ApplicationConstants.APP_PATH;

            string downloadedFilePath = string.Empty;

            try
            {
                FileDownloadRequestModel serviceRequestModel = new FileDownloadRequestModel()
                {
                    filename = fileName,
                    filetype = fileType
                };

                webServiceResponse = new WebServiceResponseModel();

                //EndpointAddress address = new EndpointAddress(ApplicationConstants.DevBaseUrl);

                EndpointAddress address = new EndpointAddress(ApplicationConstants.UATBaseUrl);

                string requestBody = JsonConvert.SerializeObject(serviceRequestModel);

                var client = new RestClient(address + ApplicationConstants.DOWNLOAD_SQLite_WEB_METHOD)
                {
                    //Authenticator = new HttpBasicAuthenticator(ApplicationConstants.DevHttpAuthUserName, ApplicationConstants.DevHttpAuthPassword),
                    Authenticator = new HttpBasicAuthenticator(ApplicationConstants.ProdHttpAuthUserName, ApplicationConstants.ProdHttpAuthPassword),
                    Timeout = -1
                };

                var serviceRequest = new RestRequest(Method.POST);

                serviceRequest.AddHeader(ApplicationConstants.RestRequestAuthHeader, ApplicationConstants.RestRequestAuthHeaderType);
                serviceRequest.AddHeader(ApplicationConstants.RestRequestContentTypeHeaderKey, ApplicationConstants.RestRequestContentTypeHeaderValue);
                serviceRequest.AddParameter(ApplicationConstants.RestRequestContentTypeHeaderValue, requestBody, ParameterType.RequestBody);

                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                restResponse = await client.ExecuteAsync(serviceRequest);

                if (restResponse.StatusCode == HttpStatusCode.OK)
                {
                    webServiceResponse.ServerResponse = restResponse.Content;
                    webServiceResponse.ResponseSatusCode = restResponse.StatusCode;

                    if (restResponse.Content.Length > 0)
                    {
                        downloadedFilePath = CreateAndSaveDocumentAndFile(restResponse.RawBytes, fileName, fileType);
                    }
                }
                else
                {
                    webServiceResponse.ServerResponse = restResponse.Content ?? "Response content not available";
                    webServiceResponse.ResponseSatusCode = restResponse.StatusCode;
                    webServiceResponse.HttpsResponseSatusDescription = restResponse.ErrorException != null ? restResponse.ErrorException.StackTrace : "Error Description not available";
                    webServiceResponse.ErrorMessage = restResponse.ErrorMessage;

                    ErrorLogger.WriteHttpErrorLog(FILE_NAME, "DownloadDocumentAndFileFromServer", webServiceResponse.ResponseSatusCode.ToString(),
                        webServiceResponse.ServerResponse, webServiceResponse.HttpsResponseSatusDescription, webServiceResponse.ErrorMessage);

                    downloadedFilePath = string.Empty;
                }
            }
            catch (Exception ex)
            {
                //ErrorHandler.LogAndThrowSpecifiedException(FILE_NAME, "DownloadDocumentAndFileFromServer", ex);
                ErrorLogger.WriteToErrorLog(nameof(InvokeWebService), nameof(DownloadDocumentAndFileFromServer), ex);
                throw ex;
            }

            return downloadedFilePath;
        }

        /// <summary>
        /// Method used for downloading the database file.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="updatedDate"></param>
        /// <returns>True, if database is downloaded, else False</returns>
        public async static Task<bool> DownloadDatabaseFileFromServer(string fileName, string updatedDate)
        {
            IRestResponse response;

            WebServiceResponseModel webServiceResponse;

            bool isDatabaseFileDownloadSuccessful = false;

            ErrorLogger.ApplicationPath = ApplicationConstants.APP_PATH;

            try
            {
                string actualFileName = fileName.Substring(fileName.IndexOf("UserFiles") + 10);

                FileDownloadRequestModel serviceRequestModel = new FileDownloadRequestModel()
                {
                    filename = actualFileName,
                    filetype = "8",
                    updatedate = updatedDate
                };

                webServiceResponse = new WebServiceResponseModel();

                //EndpointAddress address = new EndpointAddress(ApplicationConstants.DevBaseUrl);

                EndpointAddress address = new EndpointAddress(ApplicationConstants.UATBaseUrl);

                string requestBody = JsonConvert.SerializeObject(serviceRequestModel);

                var client = new RestClient(address + ApplicationConstants.DOWNLOAD_SQLite_WEB_METHOD)
                {
                    //Authenticator = new HttpBasicAuthenticator(ApplicationConstants.DevHttpAuthUserName, ApplicationConstants.DevHttpAuthPassword),
                    Authenticator = new HttpBasicAuthenticator(ApplicationConstants.ProdHttpAuthUserName, ApplicationConstants.ProdHttpAuthPassword),
                    Timeout = -1
                };

                var serviceRequest = new RestRequest(Method.POST);

                serviceRequest.AddHeader(ApplicationConstants.RestRequestAuthHeader, ApplicationConstants.RestRequestAuthHeaderType);
                serviceRequest.AddHeader(ApplicationConstants.RestRequestContentTypeHeaderKey, ApplicationConstants.RestRequestContentTypeHeaderValue);
                serviceRequest.AddParameter(ApplicationConstants.RestRequestContentTypeHeaderValue, requestBody, ParameterType.RequestBody);

                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                response = await client.ExecuteAsync(serviceRequest);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    webServiceResponse.ServerResponse = response.Content;
                    webServiceResponse.ResponseSatusCode = response.StatusCode;

                    if (response.Content.Length > 0)
                    {
                        CreateAndSaveDatabaseFile(response.RawBytes, actualFileName);

                        isDatabaseFileDownloadSuccessful = true;
                    }
                    else
                    {
                        isDatabaseFileDownloadSuccessful = false;
                    }
                }
                else
                {
                    webServiceResponse.ServerResponse = response.Content ?? "Response content not available";
                    webServiceResponse.ResponseSatusCode = response.StatusCode;
                    webServiceResponse.HttpsResponseSatusDescription = response.ErrorException != null ? response.ErrorException.StackTrace : "Error Description not available";
                    webServiceResponse.ErrorMessage = response.ErrorMessage ?? "Error Message not available";

                    ErrorLogger.WriteHttpErrorLog(FILE_NAME, "DownloadDatabaseFileFromServer", webServiceResponse.ResponseSatusCode.ToString(),
                       webServiceResponse.ServerResponse, webServiceResponse.HttpsResponseSatusDescription, webServiceResponse.ErrorMessage);

                    isDatabaseFileDownloadSuccessful = false;
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(FILE_NAME, "DownloadDatabaseFileFromServer", ex);
            }

            return isDatabaseFileDownloadSuccessful;
        }

        public async static Task<string> DownloadAppUpdateFile(string uri, string fileName)
        {
            try
            {
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

                var client = new RestClient(uri)
                {
                    //Authenticator = new HttpBasicAuthenticator(ApplicationConstants.DevHttpAuthUserName, ApplicationConstants.DevHttpAuthPassword),
                    Authenticator = new HttpBasicAuthenticator(ApplicationConstants.ProdHttpAuthUserName, ApplicationConstants.ProdHttpAuthPassword),
                    Timeout = -1
                };

                var serviceRequest = new RestRequest(Method.GET);
                serviceRequest.AddHeader("Accept", "*/*");
                var response = await client.ExecuteAsync(serviceRequest);
                string newFilePath = string.Empty;

                if (response.Content.Length > 0)
                {
                    var rawBites = response.RawBytes;
                    var folder = ApplicationData.Current.LocalFolder;
                    newFilePath = Path.Combine(folder.Path, fileName);

                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        using (FileStream file = new FileStream(newFilePath, FileMode.Create, FileAccess.Write))
                        {
                            memoryStream.Read(rawBites, 0, (int)memoryStream.Length);
                            file.Write(rawBites, 0, rawBites.Length);
                        }
                    }
                }
                return newFilePath;
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(FILE_NAME, "SrcZipTryOut", ex);
                return null;
            }
        }

        public async static Task<string> SendChannelUriToServer(string uri, string userId)
        {
            try
            {
                ErrorLogger.ApplicationPath = ApplicationConstants.APP_PATH;

                ChannelUriUpdateModel requestModel = new ChannelUriUpdateModel()
                {
                    devicetoken = uri,
                    deviceuniqueid = uri,
                    userid = userId.ToString(),
                    updatetype = "1",
                    applicationversion = HelperMethods.GetAppVersion()
                };

                var requestInJson = JsonConvert.SerializeObject(requestModel, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                // EndpointAddress address = new EndpointAddress(ApplicationConstants.DevBaseUrl);

                EndpointAddress address = new EndpointAddress(ApplicationConstants.UATBaseUrl);

                var client = new RestClient(address + ApplicationConstants.SYNC_DEVICE_INFOR_WEB_METHOD)
                {
                    // Authenticator = new HttpBasicAuthenticator(ApplicationConstants.DevHttpAuthUserName, ApplicationConstants.DevHttpAuthPassword),
                    Authenticator = new HttpBasicAuthenticator(ApplicationConstants.ProdHttpAuthUserName, ApplicationConstants.ProdHttpAuthPassword),
                    Timeout = -1
                };

                var serviceRequest = new RestRequest(Method.POST);

                serviceRequest.AddHeader(ApplicationConstants.RestRequestAuthHeader, ApplicationConstants.RestRequestAuthHeaderType);
                serviceRequest.AddHeader(ApplicationConstants.RestRequestContentTypeHeaderKey, ApplicationConstants.RestRequestContentTypeHeaderValue);
                serviceRequest.AddParameter(ApplicationConstants.RestRequestContentTypeHeaderValue, requestInJson, ParameterType.RequestBody);

                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                var response = await client.ExecuteAsync(serviceRequest);

                if (response == null || response.StatusCode != HttpStatusCode.OK)
                {
                    ErrorLogger.WriteToErrorLog(nameof(InvokeWebService), "SendChannelUriToServer", "Error sending channel URI to server");

                    return string.Empty;
                }

                return response.Content;
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(FILE_NAME, "SendChannelUriToServer", ex);

                return string.Empty;
            }
        }

        /// <summary>
        /// Method to get details of document zip
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns>document zip file details</returns>
        public async static Task<DataSyncCustomerDocumentZipResponseModel> GetCustomerDocumentZIPFile(string userName, int password)
        {
            try
            {
                CustomerDocumentDetailsModel requestModel = new CustomerDocumentDetailsModel()
                {
                    username = userName,
                    pin = password
                };

                string serviceRequestModel = JsonConvert.SerializeObject(requestModel);

                var responseData = await GetWebServiceResponse(ApplicationConstants.CUSTOMER_DOCUMENT_ZIP_FILE_WEB_METHOD, serviceRequestModel);

                var responseObejct = JsonConvert.DeserializeObject<DataSyncCustomerDocumentZipResponseModel>(responseData);

                DataSyncCustomerDocumentZipResponseModel dataSyncResponseModel = responseObejct;

                return dataSyncResponseModel;
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(FILE_NAME, "GetCustomerDocumentZIPFile", ex);
                return null;
            }
        }

        /// <summary>
        /// Method to get details of document zip
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="updateDate"></param>
        /// <returns>document zip file details</returns>
        public async static Task<DataSyncPartialSRCZipResponseModel> GetPartialSRCZIPFile(string userName, int password, string updateDate)
        {
            try
            {
                DataDownloadWebServiceRequestModel requestModel = new DataDownloadWebServiceRequestModel()
                {
                    username = userName,
                    pin = password,
                    versionnumber = ApplicationConstants.APPLICATION_VERSION,
                    updatedate = updateDate
                };

                string serviceRequestModel = JsonConvert.SerializeObject(requestModel);

                var responseData = await GetWebServiceResponse(ApplicationConstants.PartialSRC_ZIP_FILE_WEB_METHOD, serviceRequestModel);

                var responseObejct = JsonConvert.DeserializeObject<DataSyncPartialSRCZipResponseModel>(responseData);

                DataSyncPartialSRCZipResponseModel dataSyncResponseModel = responseObejct;

                return dataSyncResponseModel;
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(FILE_NAME, "GetCustomerDocumentZIPFile", ex);
                return null;
            }
        }
        public async static Task<string> ChangePinService(int userId, int newPin, string userName, int oldPin)
        {
            try
            {
                ChangePinRequestModel changedPinModel = new ChangePinRequestModel { newpin = newPin, userid = userId, username = userName, pin = oldPin };
                var serviceRequestModel = JsonConvert.SerializeObject(changedPinModel);
                var response = await GetWebServiceResponse(ApplicationConstants.CHANGE_PIN, serviceRequestModel);
                return response;
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(InvokeWebService), nameof(ChangePinService), ex.StackTrace + " - " + ex.Message);
                return null;
            }

        }

        public async static Task<List<RouteRespActivity>> GetOptmizedRoute(RouteAddress start_address, List<RouteService> services, RouteAddress end_address)
        {
            try
            {
                List<RouteVehicle> vehicles = new List<RouteVehicle>();
                RouteVehicle vehicle = new RouteVehicle();
                vehicle.vehicle_id = "custom_vehicle";
                vehicle.type_id = "custom_vehicle_type";
                vehicle.start_address = start_address;
                vehicle.end_address = end_address;
                vehicles.Add(vehicle);

                List<RoutVehicleType> vehicle_types = new List<RoutVehicleType>();
                RoutVehicleType vehicle_type = new RoutVehicleType();
                vehicle_type.type_id = "custom_vehicle_type";
                vehicle_type.profile = "car";
                vehicle_types.Add(vehicle_type);


                RoutingOptimizationRequestModel requestModel = new RoutingOptimizationRequestModel()
                {
                    vehicles = vehicles,
                    vehicle_types = vehicle_types,
                    services = services
                };

                string serviceRequestModel = JsonConvert.SerializeObject(requestModel);
                string webResponse = await GetWebServiceResponsefromGraphhopper(serviceRequestModel);
                if (!string.IsNullOrWhiteSpace(webResponse))
                {
                    if (webResponse.StartsWith("RouteError"))
                    {
                        var list = new List<RouteRespActivity>(1);
                        list.Add(new RouteRespActivity { type = "RouteError", location_id = webResponse });
                        return list;
                    }
                    else
                    {
                        var responseObejct = JsonConvert.DeserializeObject<RoutingOptimizationResponseModel>(webResponse);
                        if (responseObejct != null)
                        {
                            if (!string.IsNullOrEmpty(responseObejct.job_id))
                            {
                                if (responseObejct.solution.unassigned?.details?.Count > 0)
                                {
                                    List<RouteRespActivity> list = new List<RouteRespActivity>(responseObejct.solution.unassigned.details.Count);
                                    foreach (var item in responseObejct.solution.unassigned.details)
                                    {
                                        list.Add(new RouteRespActivity { type = "InvalidRoute", location_id = item.id });
                                    }
                                    return list;
                                }
                                if (responseObejct.solution.routes.Count > 0)
                                {
                                    var retobj = responseObejct.solution.routes[0].activities;
                                    return retobj;
                                }
                            }
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException("InvokeWebService", "GetOptmizedRoute", ex);
                return null;
            }
        }
        public async static Task<string> PostFedExAPIServiceResponseAsync(string routePath, string jsonPayload)
        {
            string bearerToken = await GetJSONTokenFedExAPIAuthorizationAsync();

            var client = new RestClient($"{ApplicationConstants.FedExApiBaseUrl}/{routePath}");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", $"Bearer {bearerToken}");
            request.AddHeader("X-locale", "en_US");
            request.AddHeader("Content-Type", ApplicationConstants.RestRequestContentTypeHeaderValue);
            // 'input' refers to JSON Payload
            request.AddParameter("application/x-www-form-urlencoded", jsonPayload, ParameterType.RequestBody);

            try
            {
                // Execute the request
                IRestResponse response = await client.ExecuteAsync(request);

                // Check the response status
                if (response.IsSuccessful)
                {
                    return response.Content; // Return 
                }
                else
                {
                    throw new Exception($@"Error: {response.ErrorMessage}
                    Status Code: {response.StatusCode}
                    {((response.Content != null) ? "" : "Response Content: " + response.Content)}
                    ");
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                throw new Exception($"Exception: {ex.Message}");
            }
        }
        #endregion

        #region Private Methods
        private async static Task<string> GetWebServiceResponse(string webMethodName, string serviceRequestModel)
        {
            IRestResponse response;
            WebServiceResponseModel webServiceResponse;
            ErrorLogger.ApplicationPath = ApplicationConstants.APP_PATH;

            string serverReponse = string.Empty;

            try
            {
                webServiceResponse = new WebServiceResponseModel();

                EndpointAddress address = new EndpointAddress(ApplicationConstants.UATBaseUrl);

                string requestBody = serviceRequestModel;


                var client = new RestClient(address + webMethodName)
                {
                    Authenticator = new HttpBasicAuthenticator(ApplicationConstants.ProdHttpAuthUserName, ApplicationConstants.ProdHttpAuthPassword),
                    Timeout = -1
                };
                client.ConfigureWebRequest((r) =>
                {
                    r.ServicePoint.Expect100Continue = false;
                    r.KeepAlive = true;
                });

                var serviceRequest = new RestRequest(Method.POST);
                serviceRequest.AddHeader("Connection", "Keep-Alive");
                serviceRequest.AddHeader(ApplicationConstants.RestRequestAuthHeader, ApplicationConstants.RestRequestAuthHeaderType);
                serviceRequest.AddHeader(ApplicationConstants.RestRequestContentTypeHeaderKey, ApplicationConstants.RestRequestContentTypeHeaderValue);
                serviceRequest.AddParameter(ApplicationConstants.RestRequestContentTypeHeaderValue, requestBody, ParameterType.RequestBody);

                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                response = await client.ExecuteAsync(serviceRequest);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    webServiceResponse.ServerResponse = response.Content;
                    webServiceResponse.ResponseSatusCode = response.StatusCode;

                    serverReponse = webServiceResponse.ServerResponse;
                }
                else if (response.StatusCode == 0 && (response.ErrorMessage != null && response.ErrorMessage.Contains("The server name or address could not be resolved"))) throw new WebException(response.ErrorMessage);
                else
                {
                    webServiceResponse.ServerResponse = response.Content ?? "Response content not available";
                    webServiceResponse.ResponseSatusCode = response.StatusCode;
                    webServiceResponse.HttpsResponseSatusDescription = (response.ErrorException != null ? response.ErrorException.StackTrace : "Error Description not available") + "\r\n";
                    webServiceResponse.HttpsResponseSatusDescription += $"ResponseStatus: {response.ResponseStatus}\r\n";
                    webServiceResponse.ErrorMessage = $"Request URL: {ApplicationConstants.UATBaseUrl}{webMethodName}\r\n";
                    webServiceResponse.ErrorMessage += $"Request Body: {requestBody}\r\n";
                    webServiceResponse.ErrorMessage += response.ErrorMessage ?? "Error Message not available";

                    ErrorLogger.WriteHttpErrorLog(FILE_NAME, "GetWebServiceResponse", webServiceResponse.ResponseSatusCode.ToString(),
                        webServiceResponse.ServerResponse, webServiceResponse.HttpsResponseSatusDescription, webServiceResponse.ErrorMessage);

                    serverReponse = string.Empty;
                }

            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(InvokeWebService), nameof(GetWebServiceResponse), ex);
                throw ex;
            }

            return serverReponse;
        }

        private async static Task<string> GetWebServiceResponsefromGraphhopper(string requestBody)
        {
            IRestResponse response;
            WebServiceResponseModel webServiceResponse;
            ErrorLogger.ApplicationPath = ApplicationConstants.APP_PATH;

            string serverReponse = string.Empty;

            try
            {
                //var client1 = new HttpClient();
                //var postData = new StringContent(reqbody, Encoding.UTF8, "application/json");
                //var request1 = await client1.PostAsync(ApplicationConstants.graphhopperURL, postData);
                //var response1 = await request1.Content.ReadAsStringAsync();

                webServiceResponse = new WebServiceResponseModel();
                var client = new RestClient(ApplicationConstants.graphhopperURL);
                var serviceRequest = new RestRequest(Method.POST);
                serviceRequest.AddHeader(ApplicationConstants.RestRequestContentTypeHeaderKey, ApplicationConstants.RestRequestContentTypeHeaderValue);
                serviceRequest.AddParameter(ApplicationConstants.RestRequestContentTypeHeaderValue, requestBody, ParameterType.RequestBody);
                //serviceRequest.AddJsonBody(reqbody);
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                response = await client.ExecuteAsync(serviceRequest);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    webServiceResponse.ServerResponse = response.Content;
                    webServiceResponse.ResponseSatusCode = response.StatusCode;
                    return serverReponse = webServiceResponse.ServerResponse;
                }
                else
                {
                    var problematicServiceIds = await GetProblematicServiceIdsAsync(response.Content);
                    if (problematicServiceIds.Count > 0) serverReponse = $"RouteError:{string.Join(",", problematicServiceIds)}";
                    else
                    {
                        webServiceResponse.ServerResponse = response.Content ?? "Response content not available";
                        webServiceResponse.ResponseSatusCode = response.StatusCode;
                        webServiceResponse.HttpsResponseSatusDescription = response.ErrorException != null ? response.ErrorException.StackTrace : "Error Description not available";
                        webServiceResponse.ErrorMessage = response.ErrorMessage ?? "Error Message not available";

                        ErrorLogger.WriteHttpErrorLog(FILE_NAME, "GetWebServiceResponsefromGraphhopper", webServiceResponse.ResponseSatusCode.ToString(),
                            webServiceResponse.ServerResponse, webServiceResponse.HttpsResponseSatusDescription, webServiceResponse.ErrorMessage);

                        return serverReponse = string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                //ErrorHandler.LogAndThrowSpecifiedException(FILE_NAME, "GetWebServiceResponsefromGraphhopper", ex);
                ErrorLogger.WriteToErrorLog(nameof(InvokeWebService), nameof(GetWebServiceResponsefromGraphhopper), ex);
            }

            return serverReponse;
        }

        private static async Task<List<string>> GetProblematicServiceIdsAsync(string apiResponse)
        {
            try
            {
                var errorInfo = GraphHopperErrorHandler.ParseErrorResponse(apiResponse);

                if (errorInfo.HasConnectionNotFoundErrors)
                {
                    // You can now use these service IDs to customize your error handling
                    foreach (var serviceId in errorInfo.ConnectionNotFoundServices)
                    {
                        // Log or handle each problematic service ID
                        System.Diagnostics.Debug.WriteLine($"Service ID {serviceId} has connection issues");
                    }

                    // Return the problematic service IDs
                    return errorInfo.ConnectionNotFoundServices;
                }

                return new List<string>(); // No errors found
            }
            catch (JsonException ex)
            {
                // Handle JSON parsing error
                System.Diagnostics.Debug.WriteLine($"JSON parsing error: {ex.Message}");
                return new List<string>();
            }
        }

        private static string CreateAndSaveDocumentAndFile(byte[] byteArrayContent, string docFileName, string fileType = "")
        {
            byte[] fileByteArray = byteArrayContent;

            MemoryStream memoryStream = new MemoryStream();

            string basePath;

            if (fileType == ((int)FileType.BrandStyle).ToString())
            {
                basePath = ApplicationConstants.APP_PATH + @"\BrandImages\";
            }
            else if (fileType == ((int)FileType.Customer).ToString())
            {
                basePath = $"{ApplicationConstants.APP_PATH}\\{ApplicationConstants.CustomerDocumentsSubFolder}";
            }
            else if (fileType == ((int)FileType.CallActivity).ToString())
            {
                basePath = $"{ApplicationConstants.APP_PATH}\\{ApplicationConstants.ACTIVITY_BASE_FOLDER}";
            }
            else
            {
                basePath = ApplicationConstants.APP_PATH + @"\UserDocuments\";
            }

            if (!Directory.Exists(basePath))
            {
                Directory.CreateDirectory(basePath);
            }

            string downloadedFilePath = Path.Combine(basePath, docFileName);

            if (File.Exists(downloadedFilePath))
            {
                File.Delete(downloadedFilePath);
            }

            using (FileStream file = new FileStream(downloadedFilePath, FileMode.Create, FileAccess.Write))
            {
                memoryStream.Read(fileByteArray, 0, (int)memoryStream.Length);

                file.Write(fileByteArray, 0, fileByteArray.Length);

                memoryStream.Close();

                memoryStream.Dispose();

                file.Close();

                file.Dispose();
            }

            return downloadedFilePath;
        }

        private static void CreateAndSaveDatabaseFile(byte[] byteArrayContent, string dbFileName)
        {
            if (byteArrayContent.Length > 0)
            {
                byte[] fileByteArray = byteArrayContent;

                MemoryStream memoryStream = new MemoryStream();

                string basePath = ApplicationConstants.APP_PATH;

                if (!Directory.Exists(basePath))
                {
                    Directory.CreateDirectory(basePath);
                }

                // Delete any existing files at basepath
                string[] files = Directory.GetFiles(basePath);

                foreach (string fileItem in files)
                {
                    File.Delete(fileItem);
                }

                string DbFilePath = Path.Combine(basePath, dbFileName);

                using (FileStream file = new FileStream(DbFilePath, FileMode.Create, FileAccess.Write))
                {
                    memoryStream.Read(fileByteArray, 0, (int)memoryStream.Length);

                    file.Write(fileByteArray, 0, fileByteArray.Length);

                    memoryStream.Close();

                    memoryStream.Dispose();

                    file.Close();

                    file.Dispose();
                }

                ZipFile.ExtractToDirectory(DbFilePath, ApplicationConstants.APP_PATH);
            }
        }

        private async static Task<string> GetJSONTokenFedExAPIAuthorizationAsync()
        {
            string oauthURL = "oauth/token";
            // Define the API endpoint
            var client = new RestClient($"{ApplicationConstants.FedExApiBaseUrl}/{oauthURL}");

            // Create a POST request
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");

            // Add form-encoded data (input is assumed to be in the correct format)
            string input = $"grant_type=client_credentials&client_id={ApplicationConstants.FedExApiClientId}&client_secret={ApplicationConstants.FedExApiClientSecret}";
            request.AddParameter("application/x-www-form-urlencoded", input, ParameterType.RequestBody);

            try
            {
                // Execute the request
                IRestResponse response = await client.ExecuteAsync(request);

                // Check the response status
                if (response.IsSuccessful)
                {
                    // Parse the JSON response to extract the token
                    FedExAPIAuthorizationResponse result = JsonConvert.DeserializeObject<FedExAPIAuthorizationResponse>(response.Content);
                    return result.AccessToken;// Return the token
                }
                else
                {
                    throw new Exception($@"Error: {response.ErrorMessage}
                    Status Code: {response.StatusCode}
                    {((response.Content != null) ? "" : "Response Content: " + response.Content)}
                    ");
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                throw new Exception($"Exception: {ex.Message}");
            }
        }

        #endregion
    }
}