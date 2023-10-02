using Luo_Painter.Models;
using Luo_Painter.Strings;
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

        // Setting
        readonly ApplicationDataContainer LocalSettings = ApplicationData.Current.LocalSettings;

        //@Converter
        private CultureInfo CultureInfoConverter(int value) => new CultureInfo(this.Lang[value]);
        readonly Languages Lang = new Languages();

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
            switch (this.GetTheme())
            {
                case ElementTheme.Light: this.ThemeComboBox.SelectedIndex = 0; break;
                case ElementTheme.Dark: this.ThemeComboBox.SelectedIndex = 1; break;
                default: this.ThemeComboBox.SelectedIndex = 2; break;
            }

            this.ThemeComboBox.SelectionChanged += (s, e) =>
            {
                switch (this.ThemeComboBox.SelectedIndex)
                {
                    case 0: this.SetTheme(ElementTheme.Light); break;
                    case 1: this.SetTheme(ElementTheme.Dark); break;
                    default: this.SetTheme(ElementTheme.Default); break;
                }
            };

            this.LanguageComboBox.SelectionChanged += (s, e) =>
            {
                int index = this.LanguageComboBox.SelectedIndex;
                this.PrimaryLanguageOverride = this.Lang[index];

                this.TB0.Text = App.Resource.GetString(UIType.Theme.ToString());
                this.TB1.Text = App.Resource.GetString(UIType.Theme_Light.ToString());
                this.TB2.Text = App.Resource.GetString(UIType.Theme_Dark.ToString());
                this.TB3.Text = App.Resource.GetString(UIType.Theme_UseSystem.ToString());

                this.TB4.Text = App.Resource.GetString(UIType.Language.ToString());
                this.TB5.Text = App.Resource.GetString(UIType.Language_Tip.ToString());
                this.TB6.Text = App.Resource.GetString(UIType.Language_UseSystemSetting.ToString());

                this.TB7.Text = App.Resource.GetString(UIType.LocalFolder.ToString());
                this.TB8.Text = App.Resource.GetString(UIType.LocalFolder_Open.ToString());
            };
            this.LanguageTipButton.Click += async (s, e) => await Windows.ApplicationModel.Core.CoreApplication.RequestRestartAsync(string.Empty);
            this.LocalFolderButton.Click += async (s, e) => await Windows.System.Launcher.LaunchFolderAsync(ApplicationData.Current.LocalFolder);
        }

        public void SetTheme(ElementTheme value)
        {
            this.LocalSettings.Values["Theme"] = (int)value;
            if (Window.Current.Content is FrameworkElement frameworkElement)
            {
                if (frameworkElement.RequestedTheme == value) return;
                frameworkElement.RequestedTheme = value;
            }
        }
        public ElementTheme GetTheme()
        {
            if (this.LocalSettings.Values.ContainsKey("Theme"))
            {
                if (this.LocalSettings.Values["Theme"] is int item)
                {
                    return (ElementTheme)item;
                }
            }
            return ElementTheme.Dark;
        }
    }
}