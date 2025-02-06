using Luo_Painter.Elements;
using Luo_Painter.Models;
using Luo_Painter.Strings;
using Luo_Painter.UI;
using System;
using System.Globalization;
using System.Linq;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Controls
{
    public sealed partial class SettingDialog : ContentDialog
    {

        // Setting
        readonly ApplicationDataContainer LocalSettings = ApplicationData.Current.LocalSettings;
        readonly CultureInfoCollection Cultures = new CultureInfoCollection();
        readonly ComboBoxItem[] Languages;
        private ComboBoxItem LanguageSelect(CultureInfo item) => new ComboBoxItem
        {
            ContentTemplate = string.IsNullOrEmpty(item.Name) ? this.LanguageUseSystemSettingTemplate : this.LanguageTemplate,
            Content = item
        };

        public SettingDialog()
        {
            this.InitializeComponent();
            this.Languages = this.Cultures.Select(this.LanguageSelect).ToArray();
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

            this.LanguageComboBox.ItemsSource = this.Languages;
            this.LanguageComboBox.SelectedIndex = this.Cultures.Index;
            this.LanguageComboBox.SelectionChanged += (s, e) =>
            {
                if (this.LanguageComboBox.SelectedItem is ComboBoxItem item && item.Content is CultureInfo info)
                    CultureInfoCollection.SetLanguage(info.Name);
                else
                    CultureInfoCollection.SetLanguageEmpty();

                this.TB0.Text = UIType.Theme.GetString();
                this.TB1.Text = UIType.Theme_Light.GetString();
                this.TB2.Text = UIType.Theme_Dark.GetString();
                this.TB3.Text = UIType.Theme_UseSystem.GetString();

                this.TB8.Text = UIType.Language.GetString();
                this.TB9.Text = UIType.Language_Tip.GetString();
                base.PrimaryButtonText = UIType.Back.GetString();

                this.TB11.Text = UIType.LocalFolder.GetString();
                this.TB12.Text = UIType.LocalFolder_Open.GetString();
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