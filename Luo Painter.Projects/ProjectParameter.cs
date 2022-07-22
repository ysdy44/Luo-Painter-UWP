using System.Collections.Generic;
using System.Xml.Linq;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Luo_Painter.Projects
{
    public class ProjectParameter
    {
        public StorageItemTypes Type;
        public string Path;
        public string Name;

        public int Width;
        public int Height;

        public IBuffer Bitmap;

        public string DocProject;
        public string DocLayers;
        public IDictionary<string, IBuffer> Bitmaps;

        public ProjectParameter(string path, string name, BitmapSize size)
        {
            this.Type = StorageItemTypes.None;
            this.Path = path;
            this.Name = name;

            this.Width = (int)size.Width;
            this.Height = (int)size.Height;
        }
        public ProjectParameter(string path, string name, uint width, uint height, IBuffer bitmap)
        {
            this.Type = StorageItemTypes.File;
            this.Path = path;
            this.Name = name;

            this.Width = (int)width;
            this.Height = (int)height;

            this.Bitmap = bitmap;
        }
        public ProjectParameter(string path, string name, int width, int height, string docProject, string docLayers, IDictionary<string, IBuffer> bitmaps)
        {
            this.Type = StorageItemTypes.Folder;
            this.Path = path;
            this.Name = name;

            this.Width = width;
            this.Height = height;

            this.DocProject = docProject;
            this.DocLayers = docLayers;
            this.Bitmaps = bitmaps;
        }
    }
}