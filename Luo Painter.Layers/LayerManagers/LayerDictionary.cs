using Luo_Painter.Historys;
using System;
using System.Collections.Generic;

namespace Luo_Painter.Layers
{
    public sealed class LayerDictionary : Dictionary<string, ILayer>, IDisposable
    {
        //@Static
        public static readonly LayerDictionary Instance = new LayerDictionary();

        private LayerDictionary() { }

        internal string NewGuid() => System.Guid.NewGuid().ToString();
        internal void Push(string id, ILayer layer) => base.Add(id, layer);

        public IEnumerable<ILayer> Permutation(Layerage[] layerages)
        {
            if (layerages is null) yield break;

            foreach (Layerage item in layerages)
            {
                yield return this.Permutation(item, 0);
            }
        }
        private ILayer Permutation(Layerage layerage, int depth)
        {
            ILayer layer = base[layerage.Id];
            layer.Depth = depth;
            layer.Children.Clear();

            if (layerage.Children is null) return layer;

            depth++;
            foreach (Layerage item in layerage.Children)
            {
                layer.Children.Add(this.Permutation(item, depth));
            }
            return layer;
        }

        public void Dispose()
        {
            foreach (KeyValuePair<string, ILayer> item in this)
            {
                item.Value.Dispose();
            }
        }
    }
}