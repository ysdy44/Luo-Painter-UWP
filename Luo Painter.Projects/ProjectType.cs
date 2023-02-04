namespace Luo_Painter.Projects
{
    public enum ProjectType : int
    {
        Add = 1,
        File = 2,
        Folder = 4,
        All = Add | File | Folder,
    }
}