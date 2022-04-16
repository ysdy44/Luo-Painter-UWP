using System;

namespace Luo_Painter.Historys.Models
{
    public class ArrangeHistory : IHistory
    {
        public HistoryMode Mode => HistoryMode.Arrange;
        public HistoryType Type => HistoryType.None;
        public string[] UndoParameter { get; }
        public string[] RedoParameter { get; }
        public ArrangeHistory(string[] undoParameter, string[] redoParameter)
        {
            this.UndoParameter = undoParameter;
            this.RedoParameter = redoParameter;
        }
        public void Dispose()
        {
            Array.Clear(this.UndoParameter, 0, this.UndoParameter.Length);
            Array.Clear(this.RedoParameter, 0, this.RedoParameter.Length);
        }
    }
}