using System.Collections.Generic;
using System.Linq;

namespace Luo_Painter.Tools
{
    public sealed class ToolGroupingList : List<ToolGrouping>, IList<ToolGrouping> { }
    public sealed class ToolGrouping : List<ToolType>, IList<ToolType>, IGrouping<ToolGroupType, ToolType>
    {
        public ToolGroupType Key { set; get; }
    }
}