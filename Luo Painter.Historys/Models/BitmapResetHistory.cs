using Windows.Storage.Streams;

namespace Luo_Painter.Historys.Models
{
    public class BitmapResetHistory : IHistory<IBuffer>
    {
        public HistoryType Type => HistoryType.BitmapReset;
        public string Id { get; set; }
        public IBuffer UndoParameter { get; set; }
        public IBuffer RedoParameter { get; set; }
        public void Dispose()
        {
        }
    }
}