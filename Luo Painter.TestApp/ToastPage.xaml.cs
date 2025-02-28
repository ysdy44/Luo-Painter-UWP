using Luo_Painter.Elements;
using System;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.TestApp
{
    public sealed partial class ToastPage : Page
    {

        //@Converter
        private ToastAudioType AudioTypeConverter(int value) => value < 0 ? ToastAudioType.Default : (ToastAudioType)value;

        //@Content
        string Title { get; } = "Luo Painter.TestApp";
        string Subtitle { get; } = "Show a Toast Notifier";
        string Image { get; } = "ms-appx:///Assets/LockScreenLogo.scale-200.png";
        ToastAudioType AudioType => this.AudioTypeConverter(this.ListBox.SelectedIndex);

        //@Construct
        public ToastPage()
        {
            this.InitializeComponent();
            this.ListBox.ItemsSource = System.Enum.GetValues(typeof(ToastAudioType));
            this.Button.Click += (s, e) =>
            {
                if (false)
                {
                    // Simple
                    ToastExtensions.Show(this.Subtitle, this.Subtitle, this.AudioType);
                }
                else
                {
                    // Complex
                    XmlDocument doc = ToastNotificationManager.GetTemplateContent(default);
                    doc.SetText(this.Subtitle);
                    doc.SetImage(this.Image);
                    doc.SetToast(this.AudioType);

                    ToastNotification toast = new ToastNotification(doc);
                    toast.Activated += this.Toast_Activated;

                    ToastNotificationManager.CreateToastNotifier().Show(toast);
                }
            };
        }

        private async void Toast_Activated(ToastNotification sender, object args)
        {
            await base.Dispatcher.RunAsync(default, this.ShowDialog);

            sender.Activated -= this.Toast_Activated;
        }

        private async void ShowDialog()
        {
            await new MessageDialog(this.Subtitle, this.Title).ShowAsync();
        }
    }
}