using Luo_Painter.Brushes;
using Luo_Painter.Strings;
using System.Collections.Generic;
using System.Linq;

namespace Luo_Painter.Controls
{
    public class InkGrouping : List<InkAttributes>, IList<InkAttributes>, IGrouping<InkGroupingType, InkAttributes>
    {
        public InkGroupingType Key { set; get; }
        public string KeyString => this.Key.GetString();
    }
}