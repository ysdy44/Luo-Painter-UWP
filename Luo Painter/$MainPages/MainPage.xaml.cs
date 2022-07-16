using Luo_Painter.Elements;
using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Search;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

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

    public class Project : INotifyPropertyChanged
    {

        //@Static
        public static readonly Project Add = new Project();

        public StorageItemTypes Type { get; }

        public string Path { get; private set; }
        public string Name { get; private set; }
        public string DisplayName { get; private set; }
        public DateTimeOffset DateCreated { get; private set; }

        public string Thumbnail { get; }
        public string ThumbnailLeft { get; }
        public string ThumbnailRight { get; }

        private Project() { }
        public Project(StorageItemTypes type, StorageFolder folder, string thumbnail) : this(type, folder, thumbnail, thumbnail, thumbnail) { }
        public Project(StorageItemTypes type, StorageFolder folder, string thumbnail, string thumbnail2) : this(type, folder, thumbnail, thumbnail2, thumbnail2) { }
        public Project(StorageItemTypes type, StorageFolder folder, string thumbnail, string thumbnailLeft, string thumbnailRight)
        {
            this.Type = type;

            this.Path = folder.Path;
            this.Name = folder.Name;
            this.DisplayName = folder.DisplayName;
            this.DateCreated = folder.DateCreated;

            this.Thumbnail = thumbnail;
            this.ThumbnailLeft = thumbnailLeft;
            this.ThumbnailRight = thumbnailRight;
        }

        public void Rename(string displayName)
        {
            this.DisplayName = displayName;
            this.OnPropertyChanged(nameof(DisplayName)); // Notify 
        }

        //@Notify 
        /// <summary> Multicast event for property change notifications. </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Notifies listeners that a property value has changed.
        /// </summary>
        /// <param name="propertyName"> Name of the property used to notify listeners. </param>
        protected void OnPropertyChanged(string propertyName) => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public sealed class ProjectObservableCollection : ObservableCollection<Project>
    {

        public bool IsLuo(string name) => name.EndsWith(".luo");
        public IAsyncOperation<IReadOnlyList<StorageFile>> GetFilesAsync(StorageFolder item)
            => item.CreateFileQueryWithOptions(this.QueryOptions).GetFilesAsync(0, 3);

        public readonly QueryOptions QueryOptions = new QueryOptions
        {
            FolderDepth = FolderDepth.Deep,
            FileTypeFilter =
            {
                ".png"
            }
        };
        public readonly IList<Project> Temp = new List<Project>();

        public async void Load(string path)
        {
            if (path is null)
            {
                foreach (StorageFolder item in await ApplicationData.Current.LocalFolder.GetFoldersAsync())
                {
                    if (this.IsLuo(item.Name)) this.AddFile(item);
                    else this.AddFolder(item, await this.GetFilesAsync(item));
                }
            }
            else
            {
                StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(path);
                if (folder is null) return;

                foreach (StorageFolder item in await folder.GetFoldersAsync())
                {
                    if (this.IsLuo(item.Name)) this.AddFile(item);
                    else this.AddFolder(item, await this.GetFilesAsync(item));
                }
            }

            base.Clear();
            base.Add(Project.Add);
            foreach (Project item in this.Temp)
            {
                base.Add(item);
            }
            this.Temp.Clear();
        }

        private void AddFile(StorageFolder item)
        {
            string thumbnail = System.IO.Path.Combine(item.Path, "Thumbnail.png");
            this.Temp.Add(new Project(StorageItemTypes.File, item, thumbnail));
        }
        private void AddFolder(StorageFolder item, IReadOnlyList<StorageFile> images)
        {
            switch (images.Count)
            {
                case 0:
                    break;
                case 1:
                    this.Temp.Add(new Project(StorageItemTypes.Folder, item, images[0].Path));
                    break;
                case 2:
                    this.Temp.Add(new Project(StorageItemTypes.Folder, item, images[0].Path, images[1].Path));
                    break;
                default:
                    this.Temp.Add(new Project(StorageItemTypes.Folder, item, images[0].Path, images[1].Path, images[2].Path));
                    break;
            }
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

        private void Load() => this.ObservableCollection.Load(this.Paths.GetPath());

        public MainPage()
        {
            this.InitializeComponent();
            this.Load();
            this.Canvas.SizeChanged += (s, e) =>
            {
                if (e.NewSize == Size.Empty) return;
                if (e.NewSize == e.PreviousSize) return;

                this.AlignmentGrid.RebuildWithInterpolation(e.NewSize);

                // Align Center
                int p = (int)e.NewSize.Width % 210;
                this.ListView.Padding = new Thickness(p / 2, 0, p / 2, 0);

                // Width
                int w = (int)e.NewSize.Width / 8;
                this.HomeButton.Width = w;
                this.BackButton.Width = w;
            };

            this.DocumentationButton.Click += (s, e) =>
            {
            };
            this.SettingButton.Click += (s, e) =>
            {
            };

            this.HomeButton.Click += (s, e) =>
            {
                if (this.Paths.GoHome())
                {
                    this.Load();
                }
            };
            this.BackButton.Click += (s, e) =>
            {
                if (this.Paths.GoBack())
                {
                    this.Load();
                }
            };

            this.PathListView.ItemClick += (s, e) =>
            {
                if (e.ClickedItem is Metadata item)
                {
                    int removes = this.Paths.Navigate(item.Path);
                    if (removes is 0) return;

                    this.Load();
                }
            };

            this.ListView.SelectionChanged += (s, e) =>
            {
                this.SelectedCount += e.AddedItems.Count;
                this.SelectedCount -= e.RemovedItems.Count;

                this.DupliateDocker.Count = this.SelectedCount;
                this.DeleteDocker.Count = this.SelectedCount;
                this.MoveDocker.Count = this.SelectedCount;
            };
            this.ListView.ItemClick += async (s, e) =>
            {
                if (e.ClickedItem is Project item2)
                {
                    switch (item2.Type)
                    {
                        case StorageItemTypes.File:
                            base.Frame.Navigate(typeof(DrawPage));
                            break;
                        case StorageItemTypes.Folder:
                            this.Paths.Add(new Metadata(item2.Path, item2.Name));
                            this.Load();
                            break;
                        case StorageItemTypes.None:
                            {
                                ContentDialogResult result = await this.AddDislog.ShowInstance();

                                switch (result)
                                {
                                    case ContentDialogResult.Primary:
                                        base.Frame.Navigate(typeof(DrawPage), this.SizePicker.Size);
                                        break;
                                    default:
                                        break;
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
            };

            this.AppBarListView.ItemClick += async (s, e) =>
            {
                if (e.ClickedItem is SymbolIcon item)
                {
                    switch (item.Symbol)
                    {
                        case Symbol.Add:
                            ContentDialogResult result1 = await this.AddDislog.ShowInstance();

                            switch (result1)
                            {
                                case ContentDialogResult.Primary:
                                    base.Frame.Navigate(typeof(DrawPage), this.SizePicker.Size);
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case Symbol.Pictures:
                            break;

                        case Symbol.Copy:
                            this.ObservableCollection.Remove(Project.Add);
                            this.ListView.IsItemClickEnabled = false;
                            this.DupliateDocker.IsShow = true;
                            break;
                        case Symbol.Delete:
                            this.ObservableCollection.Remove(Project.Add);
                            this.ListView.IsItemClickEnabled = false;
                            this.DeleteDocker.IsShow = true;
                            break;

                        case Symbol.MoveToFolder:
                            this.ObservableCollection.Remove(Project.Add);
                            this.ListView.IsItemClickEnabled = false;
                            this.MoveDocker.IsShow = true;
                            break;
                        case Symbol.NewFolder:
                            this.NewTextBox.Text = "New Folder";
                            this.NewTextBox.SelectAll();

                            ContentDialogResult result2 = await this.NewDislog.ShowInstance();
                            break;
                        default:
                            break;
                    }
                }
            };

            this.DupliateDocker.SecondaryButtonClick += (s, e) =>
            {
                this.ObservableCollection.Insert(0, Project.Add);
                this.ListView.IsItemClickEnabled = true;
                this.DupliateDocker.IsShow = false;
            };
            this.DupliateDocker.PrimaryButtonClick += (s, e) =>
            {
                this.ObservableCollection.Insert(0, Project.Add);
                this.ListView.IsItemClickEnabled = true;
                this.DupliateDocker.IsShow = false;
            };

            this.DeleteDocker.SecondaryButtonClick += (s, e) =>
            {
                this.ObservableCollection.Insert(0, Project.Add);
                this.ListView.IsItemClickEnabled = true;
                this.DeleteDocker.IsShow = false;
            };
            this.DeleteDocker.PrimaryButtonClick += (s, e) =>
            {
                this.ObservableCollection.Insert(0, Project.Add);
                this.ListView.IsItemClickEnabled = true;
                this.DeleteDocker.IsShow = false;
            };

            this.MoveDocker.SecondaryButtonClick += (s, e) =>
            {
                this.ObservableCollection.Insert(0, Project.Add);
                this.ListView.IsItemClickEnabled = true;
                this.MoveDocker.IsShow = false;
            };
            this.MoveDocker.PrimaryButtonClick += (s, e) =>
            {
                this.ObservableCollection.Insert(0, Project.Add);
                this.ListView.IsItemClickEnabled = true;
                this.MoveDocker.IsShow = false;
            };

            this.RenameCommand.Click += async (s, e) =>
            {
                this.RenameTextBox.Text = e.DisplayName;
                this.RenameTextBox.SelectAll();

                ContentDialogResult result = await this.RenameDislog.ShowInstance();
                switch (result)
                {
                    case ContentDialogResult.Primary:
                        string name = this.RenameTextBox.Text;
                        if (string.IsNullOrEmpty(name))
                        {
                            break;
                        }

                        e.Rename(name);
                        break;
                    default:
                        break;
                }
            };
        }

    }
}