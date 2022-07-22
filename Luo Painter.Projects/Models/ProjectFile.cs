﻿using Luo_Painter.Elements;
using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Media;

namespace Luo_Painter.Projects.Models
{
    public sealed class ProjectFile : Project
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
                    StorageFolder zipFolder2 = await folder.CreateFolderAsync($"{this.DisplayName} - {suffix}.luo", CreationCollisionOption.ReplaceExisting);
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
}