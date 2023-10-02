using Luo_Painter.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.UI
{
    public class ProjectDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate None { get; set; }
        public DataTemplate File { get; set; }
        public DataTemplate Folder { get; set; }
        protected override DataTemplate SelectTemplateCore(object item)
        {
            if (item is ProjectBase item2)
            {
                switch (item2.Type)
                {
                    case ProjectType.New: return this.None;
                    case ProjectType.Project: return this.File;
                    case ProjectType.Folder: return this.Folder;
                    default: return null;
                }
            }
            else return null;
        }
    }
}