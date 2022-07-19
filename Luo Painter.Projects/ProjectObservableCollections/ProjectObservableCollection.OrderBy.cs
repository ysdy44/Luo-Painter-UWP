using System.Collections.ObjectModel;
using System.Linq;

namespace Luo_Painter.Projects
{
    public sealed partial class ProjectObservableCollection : ObservableCollection<Project>
    {

        public void OrderByType()
        {
            if (base.Count < 3) return;
            this.Order(this.OrderByDescending(c => c.Type));
        }
        public void OrderByTime()
        {
            if (base.Count < 3) return;
            this.Order(this.OrderByDescending(c => c.DateCreated));
        }
        public void OrderByName()
        {
            if (base.Count < 3) return;
            this.Order(this.OrderByDescending(c => c.Name));
        }

        private void Order(IOrderedEnumerable<Project> ordered)
        {
            Project[] array = ordered.Reverse().ToArray();
            for (int i = 0; i < array.Length; i++)
            {
                int index = base.IndexOf(array[i]);
                if (index == i) continue;
                base.Move(index, i);
            }
        }

    }
}