using System;

namespace Luo_Painter.Historys
{
    public interface IHistory : IDisposable
    {
        HistoryType Type { get; }
    }
}