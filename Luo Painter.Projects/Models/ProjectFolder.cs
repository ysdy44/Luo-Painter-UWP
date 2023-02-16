using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.System;

namespace Luo_Painter.Models.Projects
{
    public sealed class ProjectFolder : Project
    {

        //@Content
        public string ThumbnailLeft { get; private set; }
        public string ThumbnailRight { get; private set; }

        //@Construct
        public ProjectFolder(StorageFolder item, IReadOnlyList<StorageFile> images = null) : base(ProjectType.Folder)
        {
            //@Static
            const string fallbackImage = @"ms-appx:///Icons\LoadFaill.jpg";

            base.Path = item.Path;
            base.Name = item.Name;
            base.DisplayName = item.DisplayName;
            base.DateCreated = item.DateCreated;

            if (images is null)
            {
                base.Thumbnail = this.ThumbnailLeft = this.ThumbnailRight = fallbackImage;
                return;
            }

            switch (images.Count)
            {
                case 0:
                    base.Thumbnail = this.ThumbnailLeft = this.ThumbnailRight = fallbackImage;
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

        public async Task<bool> RenameAsync(string name)
        {
            string path = this.Path;

            StorageFolder item = null;
            try { item = await StorageFolder.GetFolderFromPathAsync(path); }
            catch (Exception) { }
            if (item is null) return false;

            await item.RenameAsync(name, NameCollisionOption.GenerateUniqueName);

            base.Path = item.Path;
            base.Name = item.Name;
            base.DisplayName = item.DisplayName;
            base.DateCreated = item.DateCreated;
            this.OnPropertyChanged(nameof(Name)); // Notify 
            this.OnPropertyChanged(nameof(DisplayName)); // Notify 
            this.OnPropertyChanged(nameof(DateCreated)); // Notify 

            string thumbnail = System.IO.Path.Combine(item.Path, "Thumbnail.png");
            base.Thumbnail = thumbnail;
            this.ThumbnailLeft = thumbnail;
            this.ThumbnailRight = thumbnail;
            this.OnPropertyChanged(nameof(Thumbnail)); // Notify 
            this.OnPropertyChanged(nameof(ThumbnailLeft)); // Notify 
            this.OnPropertyChanged(nameof(ThumbnailRight)); // Notify 

            return true;
        }

        public async Task<bool> DeleteAsync()
        {
            string path = this.Path;

            StorageFolder item = null;
            try { item = await StorageFolder.GetFolderFromPathAsync(path); }
            catch (Exception) { }
            if (item is null) return false;

            await item.DeleteAsync();

            return true;
        }

        public async void LocalAsync()
        {
            await Launcher.LaunchFolderPathAsync(base.Path);
        }

    }
}