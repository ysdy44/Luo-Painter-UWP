using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;

namespace Luo_Painter.Projects.Models
{
    public sealed class ProjectFolder : Project
    {

        //@Static
        const string LoadFaill = @"ms-appx:///Icons\LoadFaill.jpg";

        public string ThumbnailLeft { get; private set; }
        public string ThumbnailRight { get; private set; }

        public ProjectFolder(StorageFolder item, IReadOnlyList<StorageFile> images = null) : base(StorageItemTypes.Folder)
        {
            base.Path = item.Path;
            base.Name = item.Name;
            base.DisplayName = item.DisplayName;
            base.DateCreated = item.DateCreated;

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
            await zipFolder.RenameAsync(name, NameCollisionOption.GenerateUniqueName);

            base.Path = zipFolder.Path;
            base.Name = zipFolder.Name;
            base.DisplayName = zipFolder.DisplayName;
            base.DateCreated = zipFolder.DateCreated;
            this.OnPropertyChanged(nameof(Name)); // Notify 
            this.OnPropertyChanged(nameof(DisplayName)); // Notify 
            this.OnPropertyChanged(nameof(DateCreated)); // Notify 

            string thumbnail = System.IO.Path.Combine(zipFolder.Path, "Thumbnail.png");
            this.Thumbnail = thumbnail;
            this.ThumbnailLeft = thumbnail;
            this.ThumbnailRight = thumbnail;
        }

        public async Task DeleteAsync()
        {
            StorageFolder item = await StorageFolder.GetFolderFromPathAsync(this.Path);
            await item.DeleteAsync();
        }

    }
}