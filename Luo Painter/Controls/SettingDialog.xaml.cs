using Luo_Painter.Models;
using System;
using System.Globalization;
using Windows.Globalization;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Controls
{
    public sealed partial class SettingDialog : ContentDialog
    {

        //@String
        private string Back => App.Resource.GetString(UIType.Back.ToString());

        //@Converter
        private CultureInfo CultureInfoConverter(int value) => new CultureInfo(this.Lang[value]);
        readonly Lang Lang = new Lang();

        private ElementTheme Theme
        {
            get
            {
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey("Theme"))
                {
                    if (ApplicationData.Current.LocalSettings.Values["Theme"] is int item)
                    {
                        return (ElementTheme)item;
                    }
                }
                return base.RequestedTheme;
            }
            set
            {
                ApplicationData.Current.LocalSettings.Values["Theme"] = (int)value;
                if (Window.Current.Content is FrameworkElement frameworkElement)
                {
                    if (frameworkElement.RequestedTheme == value) return;
                    frameworkElement.RequestedTheme = value;
                }
            }
        }

        private string PrimaryLanguageOverride
        {
            get
            {
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey("Language"))
                {
                    if (ApplicationData.Current.LocalSettings.Values["Language"] is string item)
                    {
                        return item;
                    }
                }
                return ApplicationLanguages.PrimaryLanguageOverride;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    ApplicationData.Current.LocalSettings.Values["Language"] = string.Empty;

                    if (ApplicationLanguages.PrimaryLanguageOverride == string.Empty) return;
                    ApplicationLanguages.PrimaryLanguageOverride = string.Empty;

                    if (Window.Current.Content is FrameworkElement frameworkElement)
                    {
                        frameworkElement.Language = CultureInfo.CurrentCulture.Name;
                    }
                }
                else
                {
                    ApplicationData.Current.LocalSettings.Values["Language"] = value;

                    if (ApplicationLanguages.PrimaryLanguageOverride == value) return;
                    ApplicationLanguages.PrimaryLanguageOverride = value;

                    if (Window.Current.Content is FrameworkElement frameworkElement)
                    {
                        if (frameworkElement.Language == value) return;
                        frameworkElement.Language = value;
                    }
                }
            }
        }

        public SettingDialog()
        {
            this.InitializeComponent();

            string lang = this.PrimaryLanguageOverride;
            this.LanguageComboBox.SelectedIndex = this.Lang[lang];
            switch (this.Theme)
            {
                case ElementTheme.Light: this.ThemeComboBox.SelectedIndex = 0; break;
                case ElementTheme.Dark: this.ThemeComboBox.SelectedIndex = 1; break;
                default: this.ThemeComboBox.SelectedIndex = 2; break;
            }

            this.ThemeComboBox.SelectionChanged += (s, e) =>
            {
                switch (this.ThemeComboBox.SelectedIndex)
                {
                    case 0: this.Theme = ElementTheme.Light; break;
                    case 1: this.Theme = ElementTheme.Dark; break;
                    default: this.Theme = ElementTheme.Default; break;
                }
            };
            this.LanguageComboBox.SelectionChanged += (s, e) =>
            {
                int index = this.LanguageComboBox.SelectedIndex;
                this.PrimaryLanguageOverride = this.Lang[index];

                Res.SetUT(this.TB0, UIType.Theme);
                Res.SetUT(this.TB1, UIType.Theme_Light);
                Res.SetUT(this.TB2, UIType.Theme_Dark);
                Res.SetUT(this.TB3, UIType.Theme_UseSystem);

                Res.SetUT(this.TB4, UIType.Language);
                Res.SetUT(this.TB5, UIType.Language_Tip);
                Res.SetUT(this.TB6, UIType.Language_UseSystemSetting);
            };
            this.LanguageTipButton.Click += async (s, e) => await Windows.ApplicationModel.Core.CoreApplication.RequestRestartAsync(string.Empty);
            //this.LocalFolderButton.Click += async (s, e) => await Launcher.LaunchFolderAsync(ApplicationData.Current.LocalFolder);
        }
    }
}