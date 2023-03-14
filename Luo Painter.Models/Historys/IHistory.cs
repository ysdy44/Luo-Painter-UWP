using System;

namespace Luo_Painter.Models
{
    public interface IHistory : IDisposable
    {
        string Title { get; set; }
        HistoryMode Mode { get; }
        HistoryPropertyMode PropertyMode { get; }
    }
}