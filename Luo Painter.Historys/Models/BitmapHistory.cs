using System.Collections.Generic;
using Windows.Storage.Streams;

namespace Luo_Painter.Historys.Models
{
    public class BitmapHistory : IHistory
    {
        public HistoryType Type => HistoryType.Bitmap;
        public string Id => this.IdCore;
        public object UndoParameter => this.UndoParameterCore;
        public object RedoParameter => this.RedoParameterCore;

        readonly string IdCore;
        readonly IDictionary<int, IBuffer> UndoParameterCore;
        readonly IDictionary<int, IBuffer> RedoParameterCore;

        public BitmapHistory(string id, IDictionary<int, IBuffer> undo, IDictionary<int, IBuffer> redo)
        {
            this.IdCore = id;
            this.UndoParameterCore = undo;
            this.RedoParameterCore = redo;
        }
        public void Dispose()
        {
            this.UndoParameterCore.Clear();
            this.RedoParameterCore.Clear();
        }
    }
}