namespace Luo_Painter.Models
{
    public enum ProjectType
    {
        New = 1,
        Project = 2,
        Folder = 4,
        All = New | Project | Folder,
    }
}