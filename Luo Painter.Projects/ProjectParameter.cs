using System.Collections.Generic;
using System.Xml.Linq;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Luo_Painter.Projects
{
    public enum ProjectParameterType : byte
    {
        None,
        Image,
        File
    }

    public class ProjectParameter
    {
        public ProjectParameterType Type;

        public string Path;
        public string Name;
        public string DisplayName;

        public int Width;
        public int Height;

        public IBuffer Bitmap;

        public string DocProject;
        public string DocLayers;
        public IDictionary<string, IBuffer> Bitmaps;

        internal ProjectParameter()
        {
        }

    }
}