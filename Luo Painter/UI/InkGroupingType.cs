using System;

namespace Luo_Painter.UI
{
    [Flags]
    public enum InkGroupingType
    {
        Erase = 8,
        Paint = 16,
        Fx = 32,
        Others = 64,

        Flash = 1 | Others,
        Splodge = 2 | Others,
        Scratch = 3 | Others,
    }
}