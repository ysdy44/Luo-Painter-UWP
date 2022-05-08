using Luo_Painter.Blends;
using Luo_Painter.Elements;
using Luo_Painter.Tools;
using Microsoft.Graphics.Canvas.Effects;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.TestApp
{
    internal sealed class ToolIcon : TIcon<ToolType>
    {
        protected override void OnTypeChanged(ToolType value)
        {
            base.Content = new ContentControl
            {
                Content = value,
                Template = value.GetTemplate(out ResourceDictionary resource),
                Resources = resource,
            };
            ToolTipService.SetToolTip(this, new ToolTip
            {
                Content = value,
            });
        }
    }

    internal sealed class BlendIcon : TIcon<BlendEffectMode>
    {
        protected override void OnTypeChanged(BlendEffectMode value)
        {
            base.Content = TIconExtensions.GetStackPanel(new ContentControl
            {
                Content = value,
                Template = value.GetTemplate(out ResourceDictionary resource, out string title),
                Resources = resource,
            }, title);
        }
    }


    internal sealed class ToolGroupingList : GroupingList<ToolGrouping, ToolGroupType, ToolType> { }
    internal sealed class ToolGrouping : Grouping<ToolGroupType, ToolType> { }

    internal sealed class BlendGroupingList : GroupingList<BlendGrouping, BlendGroupType, BlendEffectMode> { }
    internal sealed class BlendGrouping : Grouping<BlendGroupType, BlendEffectMode> { }


    public sealed partial class ListViewPage : Page
    {
        public ListViewPage()
        {
            this.InitializeComponent();
        }
    }
}