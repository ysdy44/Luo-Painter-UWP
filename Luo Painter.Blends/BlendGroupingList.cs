using Microsoft.Graphics.Canvas.Effects;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Luo_Painter.Blends
{
    public sealed class BlendGroupingList : List<IBlendGrouping>, IList<IBlendGrouping> { }
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