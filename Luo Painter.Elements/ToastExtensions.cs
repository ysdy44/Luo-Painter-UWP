using Windows.Data.Xml.Dom;
using Windows.Foundation.Metadata;
using Windows.System.Profile;

namespace Luo_Painter.Elements
{
    public static partial class ToastExtensions
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

    }
}