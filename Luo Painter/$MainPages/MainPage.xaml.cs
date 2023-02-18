using Luo_Painter.Elements;
using Luo_Painter.Models;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Luo_Painter
{
    internal class ProjectCommand : RelayCommand<Project> { }

    internal class ProjectDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate None { get; set; }
        public DataTemplate File { get; set; }
        public DataTemplate Folder { get; set; }
        protected override DataTemplate SelectTemplateCore(object item)
        {
            if (item is Project item2)
            {
                switch (item2.Type)
                {
                    case ProjectType.Add: return this.None;
                    case ProjectType.File: return this.File;
                    case ProjectType.Folder: return this.Folder;
                    default: return null;
                }
            }
            else return null;
        }
    }

    public sealed partial class MainPage : Page
    {
        //@Converter
        private ListViewSelectionMode BooleanToSelectionModeConverter(bool value) => value ? ListViewSelectionMode.None : ListViewSelectionMode.Multiple;
        private ListViewReorderMode BooleanToReorderModeConverter(bool value) => value ? ListViewReorderMode.Enabled : ListViewReorderMode.Disabled;

        public int SelectedCount { get; private set; }
        public BreadcrumbObservableCollection Paths { get; } = new BreadcrumbObservableCollection();
        public ProjectObservableCollection ObservableCollection { get; } = new ProjectObservableCollection();

        public IList<string> ClipboardProjects { get; } = new List<string>();
        public string ClipboardPath { get; private set; }

        public bool Disabler
        {
            get => App.SourcePageType != SourcePageType.MainPage;
            set => App.SourcePageType = value ? SourcePageType.Invalid : SourcePageType.MainPage;
        }

        private void Load() => this.ObservableCollection.Load(this.Paths.GetPath());

        public MainPage()
        {
            this.InitializeComponent();
            this.ConstructListView();
            this.ConstructDialog();
            this.Load();

            base.SizeChanged += (s, e) =>
            {
                if (e.NewSize == Size.Empty) return;
                if (e.NewSize == e.PreviousSize) return;

                this.AlignmentGrid.RebuildWithInterpolation(e.NewSize);
                this.ListView.Resizing(e.NewSize);
            };

            this.DocumentationButton.Click += (s, e) =>
            {
            };
            this.SettingButton.Click += (s, e) =>
            {
                if (this.Disabler) return;

                base.Frame.Navigate(typeof(StylePage));
            };

            this.HomeButton.Click += (s, e) =>
            {
                if (this.Disabler) return;

                if (this.Paths.GoHome())
                {
                    this.Load();
                }
                this.UpdateBack();
            };
            this.BackButton.Click += (s, e) =>
            {
                if (this.Disabler) return;

                if (this.Paths.GoBack())
                {
                    this.Load();
                }
                this.UpdateBack();
            };

            this.PathListView.ItemClick += (s, e) =>
            {
                if (this.Disabler) return;

                if (e.ClickedItem is Breadcrumb item)
                {
                    int removes = this.Paths.Navigate(item.Path);
                    if (removes is 0) return;

                    this.Load();
                    this.UpdateBack();
                }
            };

            this.OrderListBox.SelectionChanged += (s, e) =>
            {
                if (this.Disabler) return;

                switch (this.OrderListBox.SelectedIndex)
                {
                    case 0:
                        this.ObservableCollection.OrderByType();
                        break;
                    case 1:
                        this.ObservableCollection.OrderByTime();
                        break;
                    case 2:
                        this.ObservableCollection.OrderByName();
                        break;
                    default:
                        break;
                }
            };


            // Drag and Drop 
            base.AllowDrop = true;
            base.Drop += async (s, e) =>
            {
                if (this.Disabler) return;
                if (e.DataView.Contains(StandardDataFormats.StorageItems) is false) return;

                foreach (IStorageItem item in await e.DataView.GetStorageItemsAsync())
                {
                    this.Click(ActionType.Image, item);
                    break;
                }
            };
            base.DragOver += (s, e) =>
            {
                e.AcceptedOperation = DataPackageOperation.Copy;
                //e.DragUIOverride.Caption = 
                e.DragUIOverride.IsCaptionVisible = e.DragUIOverride.IsContentVisible = e.DragUIOverride.IsGlyphVisible = true;
            };
        }

        //@BackRequested
        /// <summary> The current page no longer becomes an active page. </summary>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (SystemNavigationManager.GetForCurrentView() is SystemNavigationManager manager)
            {
                manager.BackRequested -= this.BackRequested;
            }
        }
        /// <summary> The current page becomes the active page. </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string path = this.ApplicationView.PersistedStateId;
            this.ObservableCollection.Refresh(path);

            this.ApplicationView.Title = string.Empty;
            this.ApplicationView.PersistedStateId = string.Empty;

            if (SystemNavigationManager.GetForCurrentView() is SystemNavigationManager manager)
            {
                manager.BackRequested += this.BackRequested;
            }

            this.Disabler = false;
        }
        private void BackRequested(object sender, BackRequestedEventArgs e)
        {
            e.Handled = true;
            if (this.Paths.GoBack())
            {
                this.Load();
            }
            this.UpdateBack();
        }

        private void UpdateBack()
        {
            if (this.Paths.Count is 0)
            {
                this.TitleTextBlock.Visibility = Visibility.Visible;
                this.DocumentationButton.Visibility = Visibility.Visible;
                this.SettingButton.Visibility = Visibility.Visible;

                this.PathListView.Visibility = Visibility.Collapsed;
                this.HomeButton.Visibility = Visibility.Collapsed;
                this.BackButton.Visibility = Visibility.Collapsed;

                if (SystemNavigationManager.GetForCurrentView() is SystemNavigationManager manager)
                {
                    manager.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
                }
            }
            else
            {
                this.TitleTextBlock.Visibility = Visibility.Collapsed;
                this.DocumentationButton.Visibility = Visibility.Collapsed;
                this.SettingButton.Visibility = Visibility.Collapsed;

                this.PathListView.Visibility = Visibility.Visible;
                this.HomeButton.Visibility = Visibility.Visible;
                this.BackButton.Visibility = Visibility.Visible;

                if (SystemNavigationManager.GetForCurrentView() is SystemNavigationManager manager)
                {
                    manager.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
                }
            }
        }

    }
}