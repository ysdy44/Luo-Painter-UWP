using Windows.Storage;

namespace Luo_Painter.Projects.Models
{
    public sealed class ProjectNone : Project
    {
        //@Static
        public static readonly ProjectNone Add = new ProjectNone();
        private ProjectNone() : base(StorageItemTypes.None) { }
    }
}