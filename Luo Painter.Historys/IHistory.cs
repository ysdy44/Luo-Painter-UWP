using System;

namespace Luo_Painter.Historys
{
    public interface IHistory : IDisposable
    {
        HistoryMode Mode { get; }
        HistoryType Type { get; }
    }
}