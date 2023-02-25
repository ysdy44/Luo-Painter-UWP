using Luo_Painter.Models.Projects;
using System;
using System.Collections.ObjectModel;
using Windows.Storage;

namespace Luo_Painter.Models
{
    public sealed partial class ProjectObservableCollection : ObservableCollection<ProjectBase>
    {

        public async void Load(string path)
        {
            if (path is null)
            {
                foreach (StorageFolder item in await ApplicationData.Current.LocalFolder.GetFoldersAsync())
                {
                    if (this.IsLuo(item.Name)) this.Temp.Add(new Project(item));
                    else this.Temp.Add(new ProjectFolder(item, await this.GetFilesAsync(item)));
                }

                this.TempToSelf();
            }
            else
            {
                StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(path);
                if (folder is null) return;

                foreach (StorageFolder item in await folder.GetFoldersAsync())
                {
                    if (this.IsLuo(item.Name)) this.Temp.Add(new Project(item));
                    else this.Temp.Add(new ProjectFolder(item, await this.GetFilesAsync(item)));
                }

                this.TempToSelf();
            }
        }

        private void TempToSelf()
        {
            base.Clear();
            base.Add(ProjectNew.New);

            foreach (ProjectBase item in this.Temp)
            {
                switch (item.Type)
                {
                    case ProjectType.Folder:
                        base.Add(item);
                        break;
                }
            }

            foreach (ProjectBase item in this.Temp)
            {
                switch (item.Type)
                {
                    case ProjectType.Project:
                        base.Add(item);
                        break;
                }
            }

            this.Temp.Clear();
        }

    }
}