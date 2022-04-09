namespace Luo_Painter.Historys.Models
{
    public class OpacityHistory : IHistory<float>
    {
        public HistoryType Type => HistoryType.Opacity;
        public string Id { get; set; }
        public float UndoParameter { get; set; }
        public float RedoParameter { get; set; }
        public void Dispose()
        {
        }
    }
}