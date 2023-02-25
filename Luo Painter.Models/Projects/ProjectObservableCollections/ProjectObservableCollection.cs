using Luo_Painter.Models.Projects;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Search;

namespace Luo_Painter.Models
{
    public sealed partial class ProjectObservableCollection : ObservableCollection<ProjectBase>
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

        public readonly IList<ProjectBase> Temp = new List<ProjectBase>();

        public void Insert(StorageFolder zipFolder)
        {
            base.Insert(1, new Project(zipFolder));
        }
        public void Insert(ProjectBase project)
        {
            base.Insert(1, project);
        }

        public void Enable()
        {
            foreach (ProjectBase item in this)
            {
                item.Enable();
            }
        }
        public void Enable(ProjectType types)
        {
            foreach (ProjectBase item in this)
            {
                item.Enable(types);
            }
        }

        public void Refresh(string path)
        {
            if (string.IsNullOrEmpty(path)) return;

            foreach (ProjectBase item in this)
            {
                switch (item.Type)
                {
                    case ProjectType.Project:
                        if (item.Path == path)
                        {
                            if (item is Project item2)
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

            foreach (ProjectBase item in this)
            {
                if (item.DisplayName == name) return false;
            }

            return true;
        }

    }
}