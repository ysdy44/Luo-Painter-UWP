using System.Collections.Generic;
using Windows.Storage.Streams;

namespace Luo_Painter.Historys.Models
{
    public class BitmapHistory : IHistory<IDictionary<int, IBuffer>>
    {
        public HistoryType Type => HistoryType.Bitmap;
        public string Id { get; set; }
        public IDictionary<int, IBuffer> UndoParameter { get; set; }
        public IDictionary<int, IBuffer> RedoParameter { get; set; }
        public void Dispose()
        {
            this.UndoParameter?.Clear();
            this.RedoParameter?.Clear();
        }
    }
}