namespace Luo_Painter.Models.Historys
{
    public class ArrangeHistory : IHistory
    {
        public HistoryMode Mode => HistoryMode.Arrange;
        public HistoryType Type => HistoryType.None;

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
    }
}