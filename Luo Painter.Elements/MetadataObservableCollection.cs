using System.Collections.ObjectModel;
using System.Linq;

namespace Luo_Painter.Elements
{
    /// <summary>
    /// Represents a list of labels separated by a bullet.
    /// It also generates an accessible string representing its path.
    /// </summary>
    public sealed class MetadataObservableCollection : ObservableCollection<string>
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
                case 1:
                    return this.Single();
                case 2:
                    return System.IO.Path.Combine(this.First(), this.Last());
                default:
                    return System.IO.Path.Combine(this.ToArray());
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

            string last = this.Last();
            base.Remove(last);
            return true;
        }

        /// <summary>
        /// Causes the list to be truncated to the specified item.
        /// </summary>
        /// <returns> The count of all removed items. </returns>
        public int Navigate(string source)
        {
            int removes = 0;
            while (true)
            {
                string last = this.Last();
                if (last == source)
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