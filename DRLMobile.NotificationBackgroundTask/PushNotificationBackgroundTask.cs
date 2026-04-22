using Windows.ApplicationModel.Background;
using Windows.Networking.PushNotifications;
using Windows.Storage;

namespace DRLMobile.NotificationBackgroundTask
{

    public sealed class PushNotificationBackgroundTask : IBackgroundTask
    {
        private readonly ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();
            var notification = (RawNotification)taskInstance.TriggerDetails;
            var content = notification.Content;
            localSettings.Values["SHOW_USER_UPDATE_POPUP"] = true;
            localSettings.Values["IS_APPLICATION_UPDATE_AVAILABLE"] = true;
            localSettings.Values["NOTIFICATION_CONTENT"] = content;
            deferral.Complete();
        }
    }
}
