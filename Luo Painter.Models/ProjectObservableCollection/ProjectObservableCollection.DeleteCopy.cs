using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace Luo_Painter.Models
{
    partial class ProjectObservableCollection
    {

        public async Task DeleteAsync(ProjectBase selectedItem)
        {
            switch (selectedItem.Type)
            {
                case ProjectType.Project:
                    if (selectedItem is Project item2)
                    {
                        await item2.DeleteAsync();
                        base.Remove(selectedItem);
                    }
                    break;
                case ProjectType.Folder:
                    if (selectedItem is ProjectFolder item3)
                    {
                        await item3.DeleteAsync();
                        base.Remove(selectedItem);
                    }
                    break;
                default:
                    break;
            }
        }
        public async Task DeleteAsync(IEnumerable<object> selectedItems)
        {
            foreach (ProjectBase item in selectedItems.Cast<ProjectBase>())
            {
                switch (item.Type)
                {
                    case ProjectType.Project:
                        if (item is Project item2)
                        {
                            await item2.DeleteAsync();
                            this.Temp.Add(item);
                        }
                        break;
                    case ProjectType.Folder:
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

            foreach (ProjectBase item in this.Temp)
            {
                base.Remove(item);
            }

            this.Temp.Clear();
        }

        public async Task CopyAsync(string path, Project selectedItem, string suffix = "Dupliate")
        {
            if (path is null)
            {
                ProjectBase item = await selectedItem.CopyAsync(ApplicationData.Current.LocalFolder, suffix);
                if (item is null) return;

                base.Add(item);
            }
            else
            {
                StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(path);
                if (folder is null) return;

                ProjectBase item = await selectedItem.CopyAsync(folder, suffix);
                if (item is null) return;

                base.Add(item);
            }
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

            foreach (ProjectBase item in selectedItems.Cast<ProjectBase>())
            {
                switch (item.Type)
                {
                    case ProjectType.Project:
                        if (item is Project item2)
                        {
                            ProjectBase item3 = await item2.CopyAsync(folder, suffix);
                            if (item3 is null) continue;

                            this.Temp.Add(item3);
                        }
                        break;
                    default:
                        break;
                }
            }

            foreach (ProjectBase item in this.Temp)
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