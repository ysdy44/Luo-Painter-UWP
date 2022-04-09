using Windows.Storage.Streams;

namespace Luo_Painter.Historys.Models
{
    public class BitmapClearHistory : IHistory<IBuffer>
    {
        public HistoryType Type => HistoryType.BitmapClear;
        public string Id { get; set; }
        public IBuffer UndoParameter { get; set; }
        public IBuffer RedoParameter => null;
        public void Dispose()
        {
        }
    }
}