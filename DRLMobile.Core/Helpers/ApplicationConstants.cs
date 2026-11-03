namespace DRLMobile.Core.Helpers
{
    public static class ApplicationConstants
    {
        // Define all application level constants [Global Constants] in this file.
        // Please follow the syntax as per example below.
        // The constant will be defined as:
        public const string APP_NAME = "Honey";

        public static string APP_PATH;

        public static string DATABASE_PATH;

        public const string DATABASE_NAME = "DRL.sqlite";

        public static string APPLICATION_VERSION;

        public const string BrandImageBaseFolder = "BrandImages";

        #region Web service Constants

        public const int WEBSERVICE_TIMEOUT = 300000;

        //This is a Stag Url
        public const string UATBaseUrl = "https://service.drl-ent.com:14990/DRLServiceAPI.svc/";

        //This is a UAT Url
        //public const string UATBaseUrl = "https://service.drl-ent.com:14989/DRLServiceAPI.svc/";

        //This is a PROD url
        //public const string UATBaseUrl = "https://honeyapi.drl-ent.com/DRLServiceAPI.svc/";

        public const string LOGIN_WEB_METHOD = "GetDBFile";

        public const string DOWNLOAD_DATA_WEB_METHOD = "Login";

        public const string DOWNLOAD_SQLite_WEB_METHOD = "MultipartDownload";

        public const string UPLOAD_CUSTOMER_DATA_WEB_METHOD = "AddEditCustomer";

        public const string UPLOAD_CONTACT_DATA_WEB_METHOD = "AddEditContact";

        public const string UPLOAD_CALL_ACTIVITY_DATA_WEB_METHOD = "AddCallActivity";

        public const string UPLOAD_SCHEDULE_ROUTE_DATA_WEB_METHOD = "AddRoutes";

        public const string UPLOAD_USER_TAX_STATEMENT_DATA_WEB_METHOD = "AddUserTaxStatement";

        public const string UPLOAD_ORDER_DATA_WEB_METHOD = "AddOrder";

        public const string UPLOAD_CUSTOMER_DISTRIBUTOR_DATA = "AddCustomerDistributor";

        public const string UPLOAD_FAVORITE_DATA = "AddEditFavorite";

        public const string UPLOAD_PRODUCT_DISTRIBUTION_DATA = "AddCustomerProduct";

        public const string SYNC_DEVICE_INFOR_WEB_METHOD = "SyncDeviceInformation";

        public const string UPLOAD_USER_DOCUMENT = "WriteFile";
        public const string CHANGE_PIN = "ChangePin";
        public const string CUSTOMER_DOCUMENT_ZIP_FILE_WEB_METHOD = "GetCustomerDocumentZIPFile";
        public const string CustomerDocumentsSubFolder = "CustomerDocuments";
        public const string PartialSRC_ZIP_FILE_WEB_METHOD = "GetPartialSrcZIPFile";
        public const string PartialSRCSubFolder = "PartialSRC";

        #endregion

        #region HTTP Authentication Constants

        //This is a DEV Authentication Details
        //public const string ProdHttpAuthUserName = @"CENTRICCONSULTI\lokender.tiwari";
        //public const string ProdHttpAuthPassword = "Maddy@16May2022";

        //This is a UAT/PROD Authentication Details
        public const string ProdHttpAuthUserName = @"drl\honeydev";
        public const string ProdHttpAuthPassword = "SilicusDev01";

        #endregion

        #region Restsharp Request Constants

        public const string RestRequestAuthHeader = "Authorization";
        public const string RestRequestAuthHeaderType = "Basic";
        public const string RestRequestContentTypeHeaderKey = "Content-Type";
        public const string RestRequestContentTypeHeaderValue = "application/json";

        #endregion

        #region FedEx API Constants
        public const string FedExApiBaseUrl = "https://apis.fedex.com";
        public const string FedExApiClientId = "l7dd8938bcff32452eb95748738b8533ca";
        public const string FedExApiClientSecret = "885b1562413d40e1b11c3f3f3f762317";
        public const string FedExApiequestContentTypeHeaderValue = "application/json";
        #endregion

        #region SRC Zip
        public const string SrzZipFileName = "SrcZip.Zip";
        public const string SrzFileName = "SRCZip";
        public const string SRCZipProductFolder = "Product";
        public const string SRCZipSalesDocs = "SalesDocument";
        public const string SRCZipBrandStyle = "BrandStyle";

        // PROD
        //public const string SrzZipDownloadUrl = "https://honeyapi.drl-ent.com/Content/SRCZip.zip";
        //public const string customerDocZipUrl = "https://honeyapi.drl-ent.com/Content/CustomerZip/";

        // UAT 
        //public const string customerDocZipUrl = "https://service.drl-ent.com:14989/Content/CustomerZip/";
        //public const string SrzZipDownloadUrl = "https://service.drl-ent.com:14989/Content/SRCZip.zip";

        //Stag
        public const string customerDocZipUrl = "https://service.drl-ent.com:14990/Content/CustomerZip/";
        public const string SrzZipDownloadUrl = "https://service.drl-ent.com:14990/Content/SRCZip.zip";

        #endregion

        public const string ACTIVITY_BASE_FOLDER = "Activity";

        //Key and endpoints of graphhopper routing service
        public const string graphhopperKey = "f0f78ac4-dc82-47e6-a632-49a2246c4ab6";
        public const string graphhopperURL = "https://graphhopper.com/api/1/vrp?key=" + graphhopperKey;


        public const string BDRoleName = "BD Manager";
        public const string AVPRoleName = "AVP";

    }
}
