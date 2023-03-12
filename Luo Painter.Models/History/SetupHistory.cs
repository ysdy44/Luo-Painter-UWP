namespace Luo_Painter.Models
{
    public class SetupHistory : IHistory
    {
        public HistoryMode Mode => HistoryMode.Setup;
        public HistoryPropertyMode PropertyMode => HistoryPropertyMode.None;

        public System.Drawing.Size UndoParameter { get; }
        public System.Drawing.Size RedoParameter { get; }
        public SetupHistory(int undoWidth, int undoHeight, int redoWidth, int redoHeight)
        {
            this.UndoParameter = new System.Drawing.Size(undoWidth, undoHeight);
            this.RedoParameter = new System.Drawing.Size(redoWidth, redoHeight);
        }

        public void Dispose()
        {
        }
    }
}