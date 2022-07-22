using System.Collections.Generic;

namespace Luo_Painter.Layers
{
    public interface ILayerManager
    {
        LayerNodes Nodes { get; }
        LayerObservableCollection ObservableCollection { get; }
        IList<string> ClipboardLayers { get; }

        int LayerSelectedIndex { get; set; }
        object LayerSelectedItem { get; set; }
        IList<object> LayerSelectedItems { get; }
    }
}