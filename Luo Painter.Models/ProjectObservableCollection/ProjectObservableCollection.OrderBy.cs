using System.Collections.ObjectModel;
using System.Linq;

namespace Luo_Painter.Models
{
    public sealed partial class ProjectObservableCollection : ObservableCollection<ProjectBase>
    {

        public void OrderByType()
        {
            if (base.Count < 3) return;
            this.Order(this.OrderByDescending(c => c.Type).Reverse().ToArray());
        }
        public void OrderByTime()
        {
            if (base.Count < 3) return;
            //@Debug
            // Order by DateCreated
            this.Order(this.OrderByDescending(c => c.DateCreated).ToArray());
        }
        public void OrderByName()
        {
            if (base.Count < 3) return;
            this.Order(this.OrderByDescending(c => c.Name).Reverse().ToArray());
        }

        private void Order(ProjectBase[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                int index = base.IndexOf(array[i]);
                if (index == i) continue;
                base.Move(index, i);
            }
        }

    }
}