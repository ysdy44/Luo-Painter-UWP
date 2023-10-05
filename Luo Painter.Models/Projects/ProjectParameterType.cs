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
}