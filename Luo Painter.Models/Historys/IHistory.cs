using System;

namespace Luo_Painter.Models
{
    public interface IHistory : IDisposable
    {
        HistoryMode Mode { get; }
        HistoryType PropertyMode { get; }
    }
}