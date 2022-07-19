using Luo_Painter.Projects.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Search;

namespace Luo_Painter.Projects
{
    public sealed partial class ProjectObservableCollection : ObservableCollection<Project>
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

        public void Enable()
        {
            foreach (Project item in this)
            {
                item.Enable();
            }
        }
        public void Enable(StorageItemTypes types)
        {
            foreach (Project item in this)
            {
                item.Enable(types);
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

    }
}