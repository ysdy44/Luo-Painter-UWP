using System.Collections.Generic;
using System.Linq;

namespace Luo_Painter.Tools
{
    public sealed class ToolGroupingList : List<ToolGrouping>, IList<ToolGrouping>
    {
        public ToolGroupType GetGroupType(ToolType type)
        {
            foreach (ToolGrouping item in this)
                foreach (ToolType item2 in item)
                    if (item2 == type)
                        return item.Key;
            return ToolGroupType.Pixel;
        }
    }
    public sealed class ToolGrouping : List<ToolType>, IList<ToolType>, IGrouping<ToolGroupType, ToolType>
    {
        public ToolGroupType Key { set; get; }
    }
}