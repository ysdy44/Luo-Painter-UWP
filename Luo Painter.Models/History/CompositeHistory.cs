namespace Luo_Painter.Models
{
    public sealed class CompositeHistory : IHistory
    {
        public string Title { get; set; }
        public HistoryMode Mode => HistoryMode.Composite;
        public HistoryPropertyMode PropertyMode => HistoryPropertyMode.None;

        public IHistory[] Histories { get; }
        public CompositeHistory(params IHistory[] histories)
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

        public override string ToString() => this.Title;
    }
}