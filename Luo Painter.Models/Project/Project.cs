using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Media;

namespace Luo_Painter.Models
{
    public sealed partial class Project : ProjectBase
    {

        public ImageSource ImageSource => this.FileImageSource.ImageSource;
        readonly FileImageSource FileImageSource;

        public Project(StorageFolder item) : base(ProjectType.Project)
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

    }
}