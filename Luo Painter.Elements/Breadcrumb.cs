using Windows.Storage;

namespace Luo_Painter.Elements
{
    /// <summary>
    /// Represents label of <see cref="BreadcrumbObservableCollection"/>.
    /// </summary>
    public readonly struct Breadcrumb
    {
        /// <summary> <see cref="IStorageItem.Path"/> </summary>
        public readonly string Path;

        /// <summary> <see cref="IStorageItem.Name"/> </summary>
        public readonly string Name;

        //@Construct
        /// <summary>
        /// Constructs a Breadcrumb.
        /// </summary>
        /// <param name="path"> The path. </param>
        /// <param name="name"> The name. </param>
        public Breadcrumb(string path, string name)
        {
            this.Path = path;
            this.Name = name;
        }
        /// <summary>
        /// Constructs a Breadcrumb.
        /// </summary>
        /// <param name="item"> The item. </param>
        public Breadcrumb(IStorageItem item)
        {
            this.Path = item.Path;
            this.Name = item.Name;
        }
    }
}