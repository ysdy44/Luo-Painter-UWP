using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Luo_Painter.Layers
{
    public sealed class LayerObservableCollection : ObservableCollection<ILayer>
    {

        public int GetSelectedIndex(int index, int depth)
        {
            if (base.Count - 1 >= index)
            {
                ILayer item = base[index];
                if (item.Depth == depth) return index; // Next
            }

            index--;
            if (index < 0) return -1; // Empty

            {
                ILayer item = base[index];
                if (item.Depth + 1 == depth) return index; // Parent
            }

            for (int i = index; i >= 0; i--)
            {
                ILayer item = base[i];
                if (item.Depth == depth) return i; // Last
            }

            return 0; // ?
        }

        public ILayer GetParent(ILayer layer)
        {
            foreach (ILayer item in this)
            {
                if (item.Depth >= layer.Depth) continue;
                foreach (ILayer chil in item.Children)
                {
                    if (chil.Id == layer.Id) return item;
                }
            }
            return null;
        }

        public void AddChild(ILayer layer)
        {
            base.Add(layer);
            this.AddChildren(layer.Children);
        }
        private void AddChildren(IList<ILayer> layers)
        {
            foreach (ILayer child in layers)
            {
                this.AddChild(child);
            }
        }

        public void RemoveChild(ILayer layer)
        {
            base.Remove(layer);
            this.RemoveChildren(layer.Children);
        }
        private void RemoveChildren(IList<ILayer> layers)
        {
            foreach (ILayer child in layers)
            {
                this.RemoveChild(child);
            }
        }

        public void InsertChild(int index, ILayer layer)
        {
            base.Insert(index, layer);
            this.InsertChildren(index, layer.Children);
        }
        private void InsertChildren(int index, IList<ILayer> layers)
        {
            foreach (ILayer child in layers)
            {
                index++;
                this.InsertChild(index, child);
            }
        }


        public void RemoveDragItems(ILayer layer)
        {
            layer.Arrange(0);
            this.RemoveChildren(layer.Children);

            ILayer parent = this.GetParent(layer);
            if (parent is null) return;

            parent.Children.Remove(layer);
        }

        public void AddDragItems(ILayer layer)
        {
            int index = this.IndexOfExist(layer);
            if (index < 0 || index >= base.Count - 1)
            {
                layer.Arrange(0);
                this.AddChildren(layer.Children);
                return;
            }

            ILayer neighbor = base[index + 1];
            layer.Arrange(neighbor.Depth);
            this.InsertChildren(index, layer.Children);

            ILayer parent = this.GetParent(neighbor);
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