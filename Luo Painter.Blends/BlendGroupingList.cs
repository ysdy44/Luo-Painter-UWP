using Microsoft.Graphics.Canvas.Effects;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Luo_Painter.Blends
{
    public sealed class BlendGroupingList : List<IBlendGrouping>, IList<IBlendGrouping>
    {
        public int GetIndex(BlendEffectMode type)
        {
            int index = 0;
            foreach (IBlendGrouping item in this)
                foreach (BlendEffectMode item2 in item)
                    if (item2 == type)
                        return index;
                    else
                        index++;
            return -1;
        }
        public BlendGroupType GetGroupType(BlendEffectMode type)
        {
            foreach (IBlendGrouping item in this)
                foreach (BlendEffectMode item2 in item)
                    if (item2 == type)
                        return item.Key;
            return BlendGroupType.None;
        }
    }
    public interface IBlendGrouping : IGrouping<BlendGroupType, BlendEffectMode>
    {
    }

    public sealed class BlendGrouping : List<BlendEffectMode>, IList<BlendEffectMode>, IBlendGrouping
    {
        public BlendGroupType Key { set; get; }
    }
    public sealed class NoneBlendGrouping : IBlendGrouping
    {
        public BlendGroupType Key => BlendGroupType.None;
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
        public IEnumerator<BlendEffectMode> GetEnumerator()
        {
            yield return BlendExtensions.None;
        }
    }
}