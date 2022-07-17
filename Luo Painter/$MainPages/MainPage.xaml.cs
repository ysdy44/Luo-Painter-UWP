using Luo_Painter.Elements;
using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
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

    public abstract class Project
    {
        public StorageItemTypes Type { get; }

        public string Path { get; protected set; }
        public string Name { get; protected set; }
        public string DisplayName { get; protected set; }
        public DateTimeOffset DateCreated { get; protected set; }

        internal Project() { }
        protected Project(StorageItemTypes type, StorageFolder item)
        {
            this.Type = type;

            this.Path = item.Path;
            this.Name = item.Name;
            this.DisplayName = item.DisplayName.Replace(".luo", string.Empty);
            this.DateCreated = item.DateCreated;
        }

        protected void Rename(StorageFolder zipFolder)
        {
            this.Path = zipFolder.Path;
            this.Name = zipFolder.Name;
            this.DisplayName = zipFolder.DisplayName.Replace(".luo", string.Empty);
            this.DateCreated = zipFolder.DateCreated;
        }
    }
    public abstract class ProjectNotify : Project, INotifyPropertyChanged
    {
        public string Thumbnail { get; protected set; }

        public ProjectNotify(StorageItemTypes type, StorageFolder item) : base(type, item) { }

        public async Task DeleteAsync()
        {
            StorageFolder item = await StorageFolder.GetFolderFromPathAsync(base.Path);
            await item.DeleteAsync();
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

    public class ProjectNone : Project
    {
        //@Static
        public static readonly ProjectNone Add = new ProjectNone();

        public BitmapSize Size { get; private set; }

        private ProjectNone() { }
        public ProjectNone(BitmapSize size, StorageFolder item) : base(StorageItemTypes.None, item)
        {
            this.Size = size;
        }
    }

    public class ProjectFile : ProjectNotify
    {
        public ImageSource ImageSource => this.FileImageSource.ImageSource;
        readonly FileImageSource FileImageSource;

        public ProjectFile(StorageFolder item) : base(StorageItemTypes.File, item)
        {
            string thumbnail = System.IO.Path.Combine(item.Path, "Thumbnail.png");
            base.Thumbnail = thumbnail;
            this.FileImageSource = new FileImageSource(thumbnail);
        }

        public async void Refresh()
        {
            switch (await this.FileImageSource.Refresh(this.Thumbnail))
            {
                case FileImageSourceResult.Static:
                case FileImageSourceResult.Dynamic:
                    this.OnPropertyChanged(nameof(ImageSource)); // Notify 
                    break;
                default:
                    break;
            }
        }

        public async Task RenameAsync(string name)
        {
            StorageFolder zipFolder = await StorageFolder.GetFolderFromPathAsync(base.Path);
            await zipFolder.RenameAsync($"{name}.luo", NameCollisionOption.ReplaceExisting);

            base.Rename(zipFolder);
            this.OnPropertyChanged(nameof(Name)); // Notify 
            this.OnPropertyChanged(nameof(DisplayName)); // Notify 
            this.OnPropertyChanged(nameof(DateCreated)); // Notify 

            string thumbnail = System.IO.Path.Combine(zipFolder.Path, "Thumbnail.png");
            this.Thumbnail = thumbnail;

            await this.FileImageSource.Refresh(thumbnail);
            this.OnPropertyChanged(nameof(ImageSource)); // Notify 
        }
        public async Task<Project> CopyAsync(StorageFolder folder, string suffix = "Dupliate")
        {
            switch (this.Type)
            {
                case StorageItemTypes.File:
                    StorageFolder zipFolder = await StorageFolder.GetFolderFromPathAsync(base.Path);
                    StorageFolder zipFolder2 = await folder.CreateFolderAsync($"{this.DisplayName} - {suffix}.luo");
                    foreach (StorageFile item in await zipFolder.GetFilesAsync())
                    {
                        await item.CopyAsync(zipFolder2);
                    }

                    return new ProjectFile(zipFolder2);
                default:
                    return null;
            }
        }
    }

    public class ProjectFolder : ProjectNotify
    {
        //@Static
        const string LoadFaill = @"ms-appx:///Icons\LoadFaill.jpg";

        public string ThumbnailLeft { get; private set; }
        public string ThumbnailRight { get; private set; }

        public ProjectFolder(StorageFolder item, IReadOnlyList<StorageFile> images = null) : base(StorageItemTypes.Folder, item)
        {
            if (images is null)
            {
                base.Thumbnail = this.ThumbnailLeft = this.ThumbnailRight = ProjectFolder.LoadFaill;
                return;
            }

            switch (images.Count)
            {
                case 0:
                    base.Thumbnail = this.ThumbnailLeft = this.ThumbnailRight = ProjectFolder.LoadFaill;
                    break;
                case 1:
                    base.Thumbnail = this.ThumbnailLeft = this.ThumbnailRight = images[0].Path;
                    break;
                case 2:
                    base.Thumbnail = images[0].Path;
                    this.ThumbnailLeft = this.ThumbnailRight = images[1].Path;
                    break;
                default:
                    base.Thumbnail = images[0].Path;
                    this.ThumbnailLeft = images[1].Path;
                    this.ThumbnailRight = images[2].Path;
                    break;
            }
        }

        public async Task RenameAsync(string name)
        {
            StorageFolder zipFolder = await StorageFolder.GetFolderFromPathAsync(base.Path);
            await zipFolder.RenameAsync(name, NameCollisionOption.ReplaceExisting);

            base.Rename(zipFolder);
            this.OnPropertyChanged(nameof(Name)); // Notify 
            this.OnPropertyChanged(nameof(DisplayName)); // Notify 
            this.OnPropertyChanged(nameof(DateCreated)); // Notify 

            string thumbnail = System.IO.Path.Combine(zipFolder.Path, "Thumbnail.png");
            this.Thumbnail = thumbnail;
            this.ThumbnailLeft = thumbnail;
            this.ThumbnailRight = thumbnail;
        }
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

        public void Insert(StorageFolder zipFolder)
        {
            base.Insert(1, new ProjectFile(zipFolder));
        }
        public async Task<StorageFolder> Create(string path, string displayName)
        {
            if (path is null)
            {
                return await ApplicationData.Current.LocalFolder.CreateFolderAsync($"{displayName}.luo", CreationCollisionOption.GenerateUniqueName);
            }
            else
            {
                StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(path);
                if (folder is null) return null;

                return await folder.CreateFolderAsync($"{displayName}.luo", CreationCollisionOption.GenerateUniqueName);
            }
        }

        public async void Load(string path)
        {
            if (path is null)
            {
                foreach (StorageFolder item in await ApplicationData.Current.LocalFolder.GetFoldersAsync())
                {
                    if (this.IsLuo(item.Name)) this.Temp.Add(new ProjectFile(item));
                    else this.Temp.Add(new ProjectFolder(item, await this.GetFilesAsync(item)));
                }
            }
            else
            {
                StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(path);
                if (folder is null) return;

                foreach (StorageFolder item in await folder.GetFoldersAsync())
                {
                    if (this.IsLuo(item.Name)) this.Temp.Add(new ProjectFile(item));
                    else this.Temp.Add(new ProjectFolder(item, await this.GetFilesAsync(item)));
                }
            }

            base.Clear();
            base.Add(ProjectNone.Add);
            foreach (Project item in this.Temp)
            {
                base.Add(item);
            }
            this.Temp.Clear();
        }

        public async Task AddFolderAsync(string path, string name)
        {
            if (path is null)
            {
                StorageFolder item = await ApplicationData.Current.LocalFolder.CreateFolderAsync(name, CreationCollisionOption.ReplaceExisting);
                base.Add(new ProjectFolder(item));
            }
            else
            {
                StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(path);
                if (folder is null) return;

                StorageFolder item = await folder.CreateFolderAsync(name);
                base.Add(new ProjectFolder(item));
            }
        }

        public void Refresh(string path)
        {
            if (string.IsNullOrEmpty(path)) return;

            foreach (Project item in this)
            {
                switch (item.Type)
                {
                    case StorageItemTypes.File:
                        if (item.Path == path)
                        {
                            if (item is ProjectFile item2)
                            {
                                item2.Refresh();
                            }
                            return;
                        }
                        break;
                    default:
                        break;
                }
            }
        }
        public bool Match(string name)
        {
            if (string.IsNullOrEmpty(name)) return false;

            foreach (Project item in this)
            {
                if (item.DisplayName == name) return false;
            }

            return true;
        }

        public async Task DeleteAsync(IList<object> selectedItems)
        {
            foreach (Project item in selectedItems)
            {
                switch (item.Type)
                {
                    case StorageItemTypes.File:
                        if (item is ProjectFile item2)
                        {
                            await item2.DeleteAsync();
                            this.Temp.Add(item);
                        }
                        break;
                    case StorageItemTypes.Folder:
                        if (item is ProjectFolder item3)
                        {
                            await item3.DeleteAsync();
                            this.Temp.Add(item);
                        }
                        break;
                    default:
                        break;
                }
            }

            foreach (Project item in this.Temp)
            {
                base.Remove(item);
            }

            this.Temp.Clear();
        }

        public async Task CopyAsync(string path, IList<object> selectedItems, string suffix = "Dupliate")
        {
            StorageFolder folder;
            if (path is null)
            {
                folder = ApplicationData.Current.LocalFolder;
            }
            else
            {
                folder = await StorageFolder.GetFolderFromPathAsync(path);
                if (folder is null) return;
            }

            foreach (Project item in selectedItems)
            {
                switch (item.Type)
                {
                    case StorageItemTypes.File:
                        if (item is ProjectFile item2)
                        {
                            Project item3 = await item2.CopyAsync(folder, suffix);
                            if (item3 is null) continue;

                            this.Temp.Add(item3);
                        }
                        break;
                    default:
                        break;
                }
            }

            foreach (Project item in this.Temp)
            {
                base.Add(item);
            }

            this.Temp.Clear();
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
            this.ConstructListView();
            this.ConstructDialog();
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
                    this.UpdateBack();
                }
            };
            this.BackButton.Click += (s, e) =>
            {
                if (this.Paths.GoBack())
                {
                    this.Load();
                    this.UpdateBack();
                }
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
            string path = this.ApplicationView.Title;
            this.ObservableCollection.Refresh(path);
            this.ApplicationView.Title = string.Empty;

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
        }

        private void UpdateBack()
        {
            if (SystemNavigationManager.GetForCurrentView() is SystemNavigationManager manager)
            {
                manager.AppViewBackButtonVisibility = (this.Paths.Count is 0) ? AppViewBackButtonVisibility.Collapsed : AppViewBackButtonVisibility.Visible;
            }
        }

    }
}