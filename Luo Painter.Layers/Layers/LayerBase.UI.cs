using Luo_Painter.Blends;
using Windows.UI;
using Windows.UI.Xaml;

namespace Luo_Painter.Layers
{
    public abstract partial class LayerBase
    {

        public double UIDepth => this.Depth * 20;

        public double UIVisibility => this.Visibility is Visibility.Visible ? 1d : 0.5d;

        public double UIIsExpand => this.IsExpand ? 90 : 0;

        public string UIBlendMode => this.BlendMode.GetIcon();

        public Color UITagType
        {
            get
            {
                switch (this.TagType)
                {
                    case TagType.None: return Colors.Transparent;
                    case TagType.Red: return Colors.LightCoral;
                    case TagType.Orange: return Colors.Orange;
                    case TagType.Yellow: return Colors.Yellow;
                    case TagType.Green: return Colors.YellowGreen;
                    case TagType.Blue: return Colors.SkyBlue;
                    case TagType.Purple: return Colors.Plum;
                    default: return Colors.Transparent;
                }
            }
        }

    }
}