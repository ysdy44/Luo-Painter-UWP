using Luo_Painter.Elements;
using Luo_Painter.Models;
using Luo_Painter.Strings;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Luo_Painter
{
    internal class ProjectCommand : RelayCommand<ProjectBase> { }

    internal class ProjectDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate None { get; set; }
        public DataTemplate File { get; set; }
        public DataTemplate Folder { get; set; }
        protected override DataTemplate SelectTemplateCore(object item)
        {
            if (item is ProjectBase item2)
            {
                switch (item2.Type)
                {
                    case ProjectType.New: return this.None;
                    case ProjectType.Project: return this.File;
                    case ProjectType.Folder: return this.Folder;
                    default: return null;
                }
            }
            else return null;
        }
    }

    public sealed partial class MainPage : Page
    {
        //@Strings
        private string Untitled => UIType.Untitled.GetString();
        private string New => UIType.UntitledFolder.GetString();
        FlowDirection Direction => CultureInfoCollection.FlowDirection;

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

        private async void Load()
        {
            string path = this.Paths.GetPath();
            if (path is null)
            {
                this.ObservableCollection.Load(await ApplicationData.Current.LocalFolder.GetFoldersAsync());
                return;
            }

            StorageFolder folder = null;

            try
            {
                folder = await StorageFolder.GetFolderFromPathAsync(path);
            }
            catch (Exception)
            {
            }

            if (folder is null)
            {
                await TipType.NoFolder.ToDialog(path).ShowAsync();
                return;
            }

            this.ObservableCollection.Load(await folder.GetFoldersAsync());
        }

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

            this.DocumentationButton.Click += async (s, e) => await this.AboutDialog.ShowInstance();
            this.SettingButton.Click += async (s, e) => await this.SettingDialog.ShowInstance();

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
                    this.Click(ActionType.NewImage, item);
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
                if (SystemNavigationManager.GetForCurrentView() is SystemNavigationManager manager)
                {
                    manager.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
                }
            }
            else
            {
                if (SystemNavigationManager.GetForCurrentView() is SystemNavigationManager manager)
                {
                    manager.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
                }
            }
        }

    }
}