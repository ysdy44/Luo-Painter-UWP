using System;
using System.Collections.Generic;
using System.Linq;

namespace Luo_Painter.Elements
{
    public abstract class GroupingList<Grouping, GroupType, Type> : List<Grouping>, IList<Grouping>
        where Grouping : Grouping<GroupType, Type>
        where GroupType : Enum
        where Type : Enum
    {
        public GroupType this[Type type]
        {
            get
            {
                foreach (Grouping item in this)
                {
                    foreach (Type item2 in item)
                    {
                        if (item2.Equals(type))
                            return item.Key;
                    }
                }
                return default;
            }
        }
        public int IndexOf(Type type)
        {
            int index = 0;
            foreach (Grouping item in this)
            {
                foreach (Type item2 in item)
                {
                    if (item2.Equals(type))
                        return index;
                    else
                        index++;
                }
            }
            return -1;
        }
    }

    public abstract class Grouping<GroupType, Type> : List<Type>, IList<Type>, IGrouping<GroupType, Type>
        where GroupType : Enum
        where Type : Enum
    {
        public GroupType Key { set; get; }
    }
}