using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Core;
using Windows.Data.Xml.Dom;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.System;
using Windows.UI.Notifications;

namespace Luo_Painter.Elements
{
    public static partial class ToastExtensions
    {

        public static void Show(string textString, string assetsImageFile, ToastAudioType audioName = default)
        {
            XmlDocument doc = ToastNotificationManager.GetTemplateContent(default);
            doc.SetText(textString);
            doc.SetImage(assetsImageFile);
            doc.SetToast(audioName);

            ToastNotification toast = new ToastNotification(doc);
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }
        public static void Show(StorageFile item)
        {
            XmlDocument doc = ToastNotificationManager.GetTemplateContent(default);
            doc.SetText(item.Name);
            doc.SetImage(item.Path);
            doc.SetToast(default);

            ToastNotification toast = new ToastNotification(doc)
            {
                Data = new NotificationData(ToastExtensions.ToastValues(item))
            };

            toast.Activated += ToastExtensions.Toast_Activated;
            toast.Dismissed += ToastExtensions.Toast_Dismissed;
            toast.Failed += ToastExtensions.Toast_Failed;

            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }
        private static IEnumerable<KeyValuePair<string, string>> ToastValues(IStorageItem item)
        {
            yield return new KeyValuePair<string, string>("File", StorageApplicationPermissions.FutureAccessList.Add(item));
        }

        private static void Toast_Failed(ToastNotification sender, ToastFailedEventArgs args)
        {
            sender.Activated -= ToastExtensions.Toast_Activated;
            sender.Dismissed -= ToastExtensions.Toast_Dismissed;
            sender.Failed -= ToastExtensions.Toast_Failed;
        }
        private static void Toast_Dismissed(ToastNotification sender, ToastDismissedEventArgs args)
        {
            sender.Activated -= ToastExtensions.Toast_Activated;
            sender.Dismissed -= ToastExtensions.Toast_Dismissed;
            sender.Failed -= ToastExtensions.Toast_Failed;
        }
        private static async void Toast_Activated(ToastNotification sender, object args)
        {
            sender.Activated -= ToastExtensions.Toast_Activated;
            sender.Dismissed -= ToastExtensions.Toast_Dismissed;
            sender.Failed -= ToastExtensions.Toast_Failed;

            if (sender.Data.Values.ContainsKey("File"))
            {
                string token = sender.Data.Values["File"];

                StorageFile item = await StorageApplicationPermissions.FutureAccessList.GetFileAsync(token);
                StorageApplicationPermissions.FutureAccessList.Remove(token);

                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(default, () => ToastExtensions.LaunchFileAsync(item));
            }
        }

        private static async void LaunchFileAsync(StorageFile item)
        {
            try
            {
                await Launcher.LaunchFolderAsync(await item.GetParentAsync(), new FolderLauncherOptions
                {
                    ItemsToSelect = { item }
                });
            }
            catch (Exception)
            {
                try
                {
                    await Launcher.LaunchFileAsync(item);
                }
                catch (Exception)
                {
                }
            }
        }

    }
}