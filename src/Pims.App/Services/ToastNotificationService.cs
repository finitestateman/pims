using Microsoft.Toolkit.Uwp.Notifications;

namespace Pims.App.Services
{
    public sealed class ToastNotificationService
    {
        private const string AppId = "Pims.UtilityCollections";

        public void Show(string title, string message)
        {
            var content = new ToastContentBuilder()
                .AddText(title)
                .AddText(message)
                .GetToastContent();

            var notifier = ToastNotificationManagerCompat.CreateToastNotifier(AppId);
            var notification = new Windows.UI.Notifications.ToastNotification(content.GetXml());
            notifier.Show(notification);
        }
    }
}
