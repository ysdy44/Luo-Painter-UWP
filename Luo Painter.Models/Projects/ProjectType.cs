namespace Luo_Painter.Models
{
    public enum ProjectType : int
    {
        New = 1,
        Project = 2,
        Folder = 4,
        All = New | Project | Folder,
    }
}