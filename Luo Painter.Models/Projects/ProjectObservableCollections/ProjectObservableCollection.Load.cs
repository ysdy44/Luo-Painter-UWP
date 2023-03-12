using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.Storage;

namespace Luo_Painter.Models
{
    public sealed partial class ProjectObservableCollection : ObservableCollection<ProjectBase>
    {

        public async void Load(IReadOnlyList<StorageFolder> folder)
        {
            foreach (StorageFolder item in folder)
            {
                if (this.IsLuo(item.Name)) this.Temp.Add(new Project(item));
                else this.Temp.Add(new ProjectFolder(item, await this.GetFilesAsync(item)));
            }

            this.TempToSelf();
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