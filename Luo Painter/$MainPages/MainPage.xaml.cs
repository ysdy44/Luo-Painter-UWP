using Luo_Painter.Elements;
using Luo_Painter.Layers;
using Luo_Painter.Projects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Search;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
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
                    case StorageItemTypes.None: return this.None;
                    case StorageItemTypes.File: return this.File;
                    case StorageItemTypes.Folder: return this.Folder;
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
        public MetadataObservableCollection Paths { get; } = new MetadataObservableCollection();
        public ProjectObservableCollection ObservableCollection { get; } = new ProjectObservableCollection();
       
        public IList<string> ClipboardProjects { get; } = new List<string>();
        public string ClipboardPath { get; private set; } 

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
                foreach (var item in ObservableCollection)
                {
                    item.IsEnabled = false;
                }
            };
            this.SettingButton.Click += (s, e) =>
            {
                foreach (var item in ObservableCollection)
                {
                    item.IsEnabled = true;
                }
            };

            this.HomeButton.Click += (s, e) =>
            {
                if (this.Paths.GoHome())
                {
                    this.Load();
                }
                this.UpdateBack();
            };
            this.BackButton.Click += (s, e) =>
            {
                if (this.Paths.GoBack())
                {
                    this.Load();
                }
                this.UpdateBack();
            };

            this.PathListView.ItemClick += (s, e) =>
            {
                if (e.ClickedItem is Metadata item)
                {
                    int removes = this.Paths.Navigate(item.Path);
                    if (removes is 0) return;

                    this.Load();
                    this.UpdateBack();
                }
            };

            this.OrderListBox.SelectionChanged += (s, e) =>
            {
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