﻿using Luo_Painter.Projects.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Storage;

namespace Luo_Painter.Projects
{
    public sealed partial class ProjectObservableCollection : ObservableCollection<Project>
    {

        public async Task DeleteAsync(IEnumerable<object> selectedItems)
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

        public async Task CopyAsync(string path, IEnumerable<object> selectedItems, string suffix = "Dupliate")
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

        public async Task Move(string path, IEnumerable<string> selectedItems)
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

            foreach (string item in selectedItems)
            {
                StorageFolder item2 = await StorageFolder.GetFolderFromPathAsync(item);
                {
                    StorageFolder item3 = await folder.CreateFolderAsync(item2.Name);
                    {
                        foreach (StorageFile item4 in await item2.GetFilesAsync())
                        {
                            await item4.CopyAsync(item3);
                        }
                    }
                    this.Insert(item3);
                }
                await item2.DeleteAsync();
            }
        }

    }
}