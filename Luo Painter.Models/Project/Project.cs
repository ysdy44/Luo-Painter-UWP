using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Media;

namespace Luo_Painter.Models
{
    public sealed partial class Project : ProjectBase, INotifyPropertyChanged
    {
        //@Content
        public ProjectType Type => ProjectType.Project;
        public bool IsEnabled { get; private set; } = true;
        public string Path { get; private set; }
        public string Name { get; private set; }
        public string DisplayName { get; private set; }
        public DateTimeOffset DateCreated { get; private set; }

        public string Thumbnail { get; private set; }

        public ImageSource ImageSource => this.FileImageSource.ImageSource;
        readonly FileImageSource FileImageSource;

        public Project(StorageFolder item)
        {
            this.Path = item.Path;
            this.Name = item.Name;
            this.DisplayName = item.DisplayName.Replace(".luo", string.Empty);
            this.DateCreated = item.DateCreated;

            string thumbnail = System.IO.Path.Combine(item.Path, "Thumbnail.png");
            this.Thumbnail = thumbnail;
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

        public async Task DeleteAsync()
        {
            StorageFolder item = await StorageFolder.GetFolderFromPathAsync(this.Path);
            await item.DeleteAsync();
        }

        public async Task RenameAsync(string name)
        {
            StorageFolder zipFolder = await StorageFolder.GetFolderFromPathAsync(this.Path);
            await zipFolder.RenameAsync($"{name}.luo", NameCollisionOption.ReplaceExisting);

            this.Path = zipFolder.Path;
            this.Name = zipFolder.Name;
            this.DisplayName = zipFolder.DisplayName.Replace(".luo", string.Empty);
            this.DateCreated = zipFolder.DateCreated;
            this.OnPropertyChanged(nameof(Name)); // Notify 
            this.OnPropertyChanged(nameof(DisplayName)); // Notify 
            this.OnPropertyChanged(nameof(DateCreated)); // Notify 

            string thumbnail = System.IO.Path.Combine(zipFolder.Path, "Thumbnail.png");
            this.Thumbnail = thumbnail;

            await this.FileImageSource.Refresh(thumbnail);
            this.OnPropertyChanged(nameof(ImageSource)); // Notify 
        }

        public async Task<ProjectBase> CopyAsync(StorageFolder folder, string suffix = "Dupliate")
        {
            switch (this.Type)
            {
                case ProjectType.Project:
                    StorageFolder zipFolder = await StorageFolder.GetFolderFromPathAsync(this.Path);
                    StorageFolder zipFolder2 = await folder.CreateFolderAsync($"{this.DisplayName} - {suffix}.luo", CreationCollisionOption.ReplaceExisting);
                    foreach (StorageFile item in await zipFolder.GetFilesAsync())
                    {
                        await item.CopyAsync(zipFolder2);
                    }

                    return new Project(zipFolder2);
                default:
                    return null;
            }
        }

        public void Enable(ProjectType types)
        {
            bool isEnabled = types.HasFlag(ProjectType.Project);
            if (this.IsEnabled == isEnabled) return;

            this.IsEnabled = isEnabled;
            this.OnPropertyChanged(nameof(IsEnabled)); // Notify 
        }

        //@Notify 
        /// <summary> Multicast event for property change notifications. </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Notifies listeners that a property value has changed.
        /// </summary>
        /// <param name="propertyName"> Name of the property used to notify listeners. </param>
        private void OnPropertyChanged(string propertyName) => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}