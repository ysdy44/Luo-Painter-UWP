using Luo_Painter.Brushes;
using Luo_Painter.Strings;
using System.Collections.Generic;
using System.Linq;

namespace Luo_Painter.UI
{
    public class InkGrouping : List<PaintBrush>, IList<PaintBrush>, IGrouping<InkGroupingType, PaintBrush>
    {
        public InkGroupingType Key { set; get; }
        public string KeyString => this.Key.GetString();
    }
}