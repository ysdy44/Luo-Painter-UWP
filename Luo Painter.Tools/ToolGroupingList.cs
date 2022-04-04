using System.Collections.Generic;
using System.Linq;

namespace Luo_Painter.Tools
{
    public sealed class ToolGroupingList : List<ToolGrouping>, IList<ToolGrouping>
    {
        public int GetIndex(ToolType type)
        {
            int index = 0;
            foreach (ToolGrouping item in this)
                foreach (ToolType item2 in item)
                    if (item2 == type)
                        return index;
                    else
                        index++;
            return -1;
        }
        public ToolGroupType GetGroupType(ToolType type)
        {
            foreach (ToolGrouping item in this)
                foreach (ToolType item2 in item)
                    if (item2 == type)
                        return item.Key;
            return ToolGroupType.None;
        }
    }
    public sealed class ToolGrouping : List<ToolType>, IList<ToolType>, IGrouping<ToolGroupType, ToolType>
    {
        public ToolGroupType Key { set; get; }
    }
}