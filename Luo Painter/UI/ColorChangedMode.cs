using Luo_Painter.Elements;
using Luo_Painter.Models;
using System;
using System.Linq.Expressions;

namespace Luo_Painter.Controls
{
    [Flags]
    public enum ColorChangedMode
    {
        WithPrimaryBrush = 1,
        WithSecondaryBrush = 2,
        WithColor = 4,
        All = WithPrimaryBrush | WithSecondaryBrush | WithColor
    }
}