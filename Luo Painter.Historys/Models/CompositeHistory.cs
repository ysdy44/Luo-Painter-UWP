namespace Luo_Painter.Historys.Models
{
    public class CompositeHistory : IHistory
    {
        public HistoryMode Mode => HistoryMode.Composite;
        public HistoryType Type => HistoryType.None;

        public readonly IHistory[] Histories;
        public CompositeHistory(IHistory[] histories)
        {
            this.Histories = histories;
        }

        public void Dispose()
        {
            foreach (IHistory item in this.Histories)
            {
                item.Dispose();
            }
        }
    }
}