using System;

namespace Luo_Painter.Historys
{
    public interface IHistory : IDisposable
    {
        HistoryType Type { get; }
    }

    public interface IHistory<T> : IHistory
    {
        T UndoParameter { get; }
        T RedoParameter { get; }
    }
}