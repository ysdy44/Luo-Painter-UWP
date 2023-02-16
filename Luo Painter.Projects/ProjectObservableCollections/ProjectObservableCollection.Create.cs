using Luo_Painter.Models.Projects;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Storage;

namespace Luo_Painter.Models
{
    public sealed partial class ProjectObservableCollection : ObservableCollection<Project>
    {

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

        public async Task AddFolderAsync(string path, string name)
        {
            if (path is null)
            {
                StorageFolder item = await ApplicationData.Current.LocalFolder.CreateFolderAsync(name, CreationCollisionOption.GenerateUniqueName);
                base.Add(new ProjectFolder(item));
            }
            else
            {
                StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(path);
                if (folder is null) return;

                StorageFolder item = await folder.CreateFolderAsync(name, CreationCollisionOption.GenerateUniqueName);
                base.Add(new ProjectFolder(item));
            }
        }

    }
}