using Windows.UI.Xaml;

namespace Luo_Painter.Historys.Models
{
    public class VisibilityHistory : IHistory<Visibility>
    {
        public HistoryType Type => HistoryType.Visibility;
        public string Id { get; set; }
        public Visibility UndoParameter { get; set; }
        public Visibility RedoParameter { get; set; }
        public void Dispose()
        {
        }
    }
}