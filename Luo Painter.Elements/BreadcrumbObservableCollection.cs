using System.Collections.ObjectModel;
using System.Linq;
using Windows.Storage;

namespace Luo_Painter.Elements
{
    /// <summary>
    /// Represents label of <see cref="BreadcrumbObservableCollection"/>.
    /// </summary>
    public struct Breadcrumb
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

    /// <summary>
    /// Represents a list of labels separated by a bullet.
    /// It also generates an accessible string representing its path.
    /// </summary>
    public sealed class BreadcrumbObservableCollection : ObservableCollection<Breadcrumb>
    {

        /// <summary>
        /// Generates an accessible string representing its path.
        /// </summary>
        /// <returns> The generated path. </returns>
        public string GetPath()
        {
            switch (base.Count)
            {
                case 0:
                    return null;
                default:
                    return this.Last().Path;
            }
        }

        /// <summary>
        /// Navigates to the root item in list.
        /// </summary>
        /// <returns>
        /// Return a value that indicates whether there is at least one entry in list.
        /// </returns>
        public bool GoHome()
        {
            if (base.Count is 0) return false;

            this.Clear();
            return true;
        }

        /// <summary>
        /// Navigates to the most recent item in list.
        /// </summary>
        /// <returns>
        /// Return a value that indicates whether there is at least one entry in list.
        /// </returns>
        public bool GoBack()
        {
            if (base.Count is 0) return false;

            Breadcrumb last = this.Last();
            base.Remove(last);
            return true;
        }

        /// <summary>
        /// Causes the list to be truncated to the specified item.
        /// </summary>
        /// <param name="sourcePath"> The target source path. </param>
        /// <returns> The count of all removed items. </returns>
        public int Navigate(string sourcePath)
        {
            int removes = 0;
            while (true)
            {
                Breadcrumb last = this.Last();
                if (last.Path == sourcePath)
                {
                    break;
                }
                else
                {
                    removes++;
                    base.Remove(last);
                }
            }
            return removes;
        }

    }
}