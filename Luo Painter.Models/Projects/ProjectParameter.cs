using System.Collections.Generic;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;

namespace Luo_Painter.Models
{
    public enum ProjectParameterType
    {
        NewProject,

        /// <summary>
        /// <see cref="ProjectParameter.Bitmap"/>
        /// </summary>
        NewImage,

        /// <summary>
        /// <see cref="ProjectParameter.DocProject"/>
        /// <see cref="ProjectParameter.DocLayers"/>
        /// <see cref="ProjectParameter.Bitmaps"/>
        /// <see cref="ProjectParameter.Photos"/>
        /// </summary>
        OpenProject
    }

    public class ProjectParameter
    {
        public ProjectParameterType Type { get; internal set; }

        public string Path { get; internal set; }
        public string Name { get; internal set; }
        public string DisplayName { get; internal set; }

        public int Width { get; internal set; }
        public int Height { get; internal set; }

        public IBuffer Bitmap { get; internal set; }

        public string DocProject { get; internal set; }
        public string DocLayers { get; internal set; }
        public IDictionary<string, IBuffer> Bitmaps { get; internal set; }
        public IDictionary<string, SoftwareBitmap> Photos { get; internal set; }

        internal ProjectParameter()
        {
        }

    }
}