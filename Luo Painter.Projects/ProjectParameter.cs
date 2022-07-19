using Windows.Graphics.Imaging;
using Windows.Storage;

namespace Luo_Painter.Projects
{
    public struct ProjectParameter
    {
        public StorageItemTypes Type;
        public string Path;
        public BitmapSize Size;
        public string Image;
        public ProjectParameter(string path, BitmapSize size)
        {
            this.Type = StorageItemTypes.None;
            this.Path = path;
            this.Size = size;
            this.Image = null;
        }
        public ProjectParameter(string path, string image)
        {
            this.Type = StorageItemTypes.File;
            this.Path = path;
            this.Size = default;
            this.Image = image;
        }
        public ProjectParameter(string path)
        {
            this.Type = StorageItemTypes.Folder;
            this.Path = path;
            this.Size = default;
            this.Image = null;
        }
    }
}