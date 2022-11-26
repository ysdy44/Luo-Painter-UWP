using Windows.Data.Xml.Dom;
using Windows.Foundation.Metadata;
using Windows.System.Profile;
using Windows.UI.Notifications;

namespace Luo_Painter.Elements
{
    public enum ToastAudioType : int
    {
        Default,

        IM,
        Mail,
        Reminder,
        SMS,

        Looping_Alarm,
        Looping_Alarm2,
        Looping_Alarm3,
        Looping_Alarm4,
        Looping_Alarm5,
        Looping_Alarm6,
        Looping_Alarm7,
        Looping_Alarm8,
        Looping_Alarm9,
        Looping_Alarm10,

        Looping_Call,
        Looping_Call2,
        Looping_Call3,
        Looping_Call4,
        Looping_Call5,
        Looping_Call6,
        Looping_Call7,
        Looping_Call8,
        Looping_Call9,
        Looping_Call10,
    }

    public static class ToastExtensions
    {

        public static bool IsAdaptiveToastSupported()
        {
            switch (AnalyticsInfo.VersionInfo.DeviceFamily)
            {
                case "Windows.Mobile":
                case "Windows.Desktop":
                    return ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 3);
                default:
                    return false;
            }
        }

        public static void Show(string textString, string assetsImageFile, ToastAudioType audioName = default)
        {
            XmlDocument doc = ToastNotificationManager.GetTemplateContent(default);
            doc.SetText(textString);
            doc.SetImage(assetsImageFile);
            doc.SetToast(audioName);

            ToastNotification toast = new ToastNotification(doc);
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }

        public static void SetText(this XmlDocument doc, string textString)
        {
            XmlNodeList text = doc.GetElementsByTagName("text");
            XmlText textString1 = doc.CreateTextNode(textString);

            IXmlNode text1 = text[0];
            text1.AppendChild(textString1);
        }

        public static void SetImage(this XmlDocument doc, string assetsImageFile)
        {
            XmlNodeList image = doc.GetElementsByTagName("image");
            if (image[0] is XmlElement image1)
            {
                image1.SetAttribute("src", assetsImageFile);
                image1.SetAttribute("alt", "logo");
            }
        }

        public static void SetToast(this XmlDocument doc, ToastAudioType audioName, string launch = null)
        {
            IXmlNode toast = doc.SelectSingleNode("/toast");

            XmlElement audio = doc.CreateElement("audio");
            audio.SetAttribute("src", audioName.ToSource());
            toast.AppendChild(audio);

            if (toast is XmlElement toast1)
            {
                toast1.SetAttribute("duration", "short");

                // App Launch Parameter 
                if (string.IsNullOrEmpty(launch))
                {
                    // toast1.SetAttribute("launch", "{\"type\":\"toast\",\"param1\":\"12345\",\"param2\":\"67890\"}");
                }
                else
                {
                    toast1.SetAttribute("launch", launch);
                }
            }
        }

        public static string ToSource(this ToastAudioType audioName)
        {
            switch (audioName)
            {
                case ToastAudioType.IM: return "ms-winsoundevent:Notification.IM";
                case ToastAudioType.Mail: return "ms-winsoundevent:Notification.Mail";
                case ToastAudioType.Reminder: return "ms-winsoundevent:Notification.Reminder";
                case ToastAudioType.SMS: return "ms-winsoundevent:Notification.SMS";

                case ToastAudioType.Looping_Alarm: return "ms-winsoundevent:Notification.Looping.Alarm";
                case ToastAudioType.Looping_Alarm2: return "ms-winsoundevent:Notification.Looping.Alarm2";
                case ToastAudioType.Looping_Alarm3: return "ms-winsoundevent:Notification.Looping.Alarm3";
                case ToastAudioType.Looping_Alarm4: return "ms-winsoundevent:Notification.Looping.Alarm4";
                case ToastAudioType.Looping_Alarm5: return "ms-winsoundevent:Notification.Looping.Alarm5";
                case ToastAudioType.Looping_Alarm6: return "ms-winsoundevent:Notification.Looping.Alarm6";
                case ToastAudioType.Looping_Alarm7: return "ms-winsoundevent:Notification.Looping.Alarm7";
                case ToastAudioType.Looping_Alarm8: return "ms-winsoundevent:Notification.Looping.Alarm8";
                case ToastAudioType.Looping_Alarm9: return "ms-winsoundevent:Notification.Looping.Alarm9";
                case ToastAudioType.Looping_Alarm10: return "ms-winsoundevent:Notification.Looping.Alarm10";

                case ToastAudioType.Looping_Call: return "ms-winsoundevent:Notification.Looping.Call";
                case ToastAudioType.Looping_Call2: return "ms-winsoundevent:Notification.Looping.Call2";
                case ToastAudioType.Looping_Call3: return "ms-winsoundevent:Notification.Looping.Call3";
                case ToastAudioType.Looping_Call4: return "ms-winsoundevent:Notification.Looping.Call4";
                case ToastAudioType.Looping_Call5: return "ms-winsoundevent:Notification.Looping.Call5";
                case ToastAudioType.Looping_Call6: return "ms-winsoundevent:Notification.Looping.Call6";
                case ToastAudioType.Looping_Call7: return "ms-winsoundevent:Notification.Looping.Call7";
                case ToastAudioType.Looping_Call8: return "ms-winsoundevent:Notification.Looping.Call8";
                case ToastAudioType.Looping_Call9: return "ms-winsoundevent:Notification.Looping.Call9";
                case ToastAudioType.Looping_Call10: return "ms-winsoundevent:Notification.Looping.Call10";

                default: return "ms-winsoundevent:Notification.Default";
            }
        }

    }
}