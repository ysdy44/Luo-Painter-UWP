using System.Collections.ObjectModel;
using System.Linq;

namespace Luo_Painter.Layers
{
    public sealed class LayerObservableCollection : ObservableCollection<ILayer>
    {

        public void AddChild(ILayer layer)
        {
            base.Add(layer);
            this.AddChildren(layer);
        }
        private void AddChildren(ILayer layer)
        {
            foreach (ILayer child in layer.Children)
            {
                this.AddChild(child);
            }
        }

        private void RemoveChild(ILayer layer)
        {
            base.Remove(layer);
            this.RemoveChildren(layer);
        }
        private void RemoveChildren(ILayer layer)
        {
            foreach (ILayer child in layer.Children)
            {
                this.RemoveChild(child);
            }
        }

        private void InsertChild(int index, ILayer layer)
        {
            base.Insert(index, layer);
            this.InsertChildren(index, layer);
        }
        private void InsertChildren(int index, ILayer layer)
        {
            foreach (ILayer child in layer.Children)
            {
                index++;
                this.InsertChild(index, child);
            }
        }


        public void RemoveDragItems(ILayer layer)
        {
            layer.Arrange(0);
            this.RemoveChildren(layer);

            ILayer parent = this.FirstOrDefault(c => c.Children.Contains(layer));
            if (parent is null) return;

            parent.Children.Remove(layer);
        }

        public void AddDragItems(ILayer layer)
        {
            int index = this.IndexOfExist(layer);
            if (index < 0 || index >= base.Count - 1)
            {
                layer.Arrange(0);
                this.AddChildren(layer);
                return;
            }

            ILayer neighbor = base[index + 1];
            layer.Arrange(neighbor.Depth);
            this.InsertChildren(index, layer);

            ILayer parent = this.FirstOrDefault(c => c.Children.Contains(neighbor));
            if (parent is null) return;

            index = parent.Children.IndexOf(neighbor);
            parent.Children.Insert(index, layer);
        }

        private int IndexOfExist(ILayer layer)
        {
            for (int i = 0; i < base.Count; i++)
            {
                ILayer item = base[i];
                if (item.IsExist && item.Id == layer.Id) return i;
            }
            return -1;
        }

    }
}