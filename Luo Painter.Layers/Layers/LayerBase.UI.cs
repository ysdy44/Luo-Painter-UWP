using System;
using Windows.UI.Xaml;

namespace Luo_Painter.Layers
{
    public abstract partial class LayerBase 
    {

        public double UIDepth => this.Depth * 20;

        public double UIVisibility => this.Visibility is Visibility.Visible ? 1d : 0.5d;

        public double UIIsExpand => this.IsExpand ? 90 : 0;

    }
}