using System.Text.RegularExpressions;

namespace DRLMobile.Constants
{
    public static class Constants
    {
        public const string SELECTED_CUSTOMER_ID = "SELECTED_CUSTOMER_ID";
        public const string PHONE_FORMAT = "{0:(###)-###-####}";

        #region regex
        public static Regex PhoneNumbRegex = new Regex(@"^(\+\d{1,2}\s)?\(?\d{3}\)?[\s.-]\d{3}[\s.-]\d{4}$");
        public static Regex EmailRegex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
        #endregion

        public const string BADGE_COUNT = "BADGE_COUNT";
        public const string CURRENT_ORDER_ID = "CurrentOrderId";

        public const string CustomerDocumentsSubFolder = "CustomerDocuments";
        public const string UserDocumentsSubFolder = "UserDocuments";


        public const string BackgroundNotificationEntryPoint = "DRLMobile.NotificationBackgroundTask.PushNotificationBackgroundTask";
        ///"DRLMobile.NotificationBackgroundTask.PushNotificationBackgroundTask"
        ///

        public const string SHOW_USER_UPDATE_POPUP = "SHOW_USER_UPDATE_POPUP";
        public const string IS_APPLICATION_UPDATE_AVAILABLE = "IS_APPLICATION_UPDATE_AVAILABLE";
        public const string NOTIFICATION_CONTENT = "NOTIFICATION_CONTENT";




    }
}
