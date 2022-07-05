using Luo_Painter.Historys;
using System.Collections.Generic;
using System.Linq;

namespace Luo_Painter.Layers
{
    public sealed class LayerNodes : List<ILayer>
    {
        public Layerage[] Convert() => LayerNodes.Convert(this);
        private static Layerage[] Convert(IList<ILayer> layers) => (layers.Count is 0) ? null : layers.Select(LayerNodes.Convert).ToArray();
        private static Layerage Convert(ILayer layer) => new Layerage
        {
            Id = layer.Id,
            Children = LayerNodes.Convert(layer.Children)
        };
    }
}