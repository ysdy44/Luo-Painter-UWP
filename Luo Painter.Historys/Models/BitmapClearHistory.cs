using Windows.Storage.Streams;

namespace Luo_Painter.Historys.Models
{
    public class BitmapClearHistory : IHistory
    {
        public HistoryType Type => HistoryType.BitmapClear;
        public string Id => this.IdCore;
        public object UndoParameter => this.UndoParameterCore;
        public object RedoParameter => null;

        readonly string IdCore;
        readonly IBuffer UndoParameterCore;

        public BitmapClearHistory(string id, IBuffer undo)
        {
            this.IdCore = id;
            this.UndoParameterCore = undo;
        }
        public void Dispose()
        {
        }
    }
}