namespace Luo_Painter.Models
{
    public sealed class ArrangeHistory : IHistory
    {
        public string Title { get; set; }
        public HistoryMode Mode => HistoryMode.Arrange;
        public HistoryPropertyMode PropertyMode => HistoryPropertyMode.None;

        public Layerage[] UndoParameter { get; }
        public Layerage[] RedoParameter { get; }
        public ArrangeHistory(Layerage[] undoParameter, Layerage[] redoParameter)
        {
            this.UndoParameter = undoParameter;
            this.RedoParameter = redoParameter;
        }

        public void Dispose()
        {
            foreach (Layerage item in this.UndoParameter)
            {
                item.Dispose();
            }
            foreach (Layerage item in this.RedoParameter)
            {
                item.Dispose();
            }
        }

        public override string ToString() => this.Title;
    }
}