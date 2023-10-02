﻿using Luo_Painter.Elements;
using Luo_Painter.Models;
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

                this.TB0.Text = App.Resource.GetString(UIType.Theme.ToString());
                this.TB1.Text = App.Resource.GetString(UIType.Theme_Light.ToString());
                this.TB2.Text = App.Resource.GetString(UIType.Theme_Dark.ToString());
                this.TB3.Text = App.Resource.GetString(UIType.Theme_UseSystem.ToString());

                this.TB4.Text = App.Resource.GetString(UIType.Language.ToString());
                this.TB5.Text = App.Resource.GetString(UIType.Language_Tip.ToString());
                base.PrimaryButtonText = App.Resource.GetString(UIType.Back.ToString());

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