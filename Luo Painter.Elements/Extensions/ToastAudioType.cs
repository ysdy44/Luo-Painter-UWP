namespace Luo_Painter.Elements
{
    public enum ToastAudioType
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

    public static partial class ToastExtensions
    {

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