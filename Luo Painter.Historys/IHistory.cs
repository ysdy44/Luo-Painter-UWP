using System;

namespace Luo_Painter.Historys
{
    public interface IHistory: IDisposable
    {
        HistoryType Type { get; }
        object UndoParameter { get; }
        object RedoParameter { get; }
    }
}