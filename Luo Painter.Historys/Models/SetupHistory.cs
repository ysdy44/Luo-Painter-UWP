using Windows.Graphics.Imaging;

namespace Luo_Painter.Models.Historys
{
    public class SetupHistory : IHistory
    {
        public HistoryMode Mode => HistoryMode.Setup;
        public HistoryType Type => HistoryType.None;
        public HistoryType PropertyType => HistoryType.None;

        public readonly System.Drawing.Size UndoParameter;
        public readonly System.Drawing.Size RedoParameter;
        public SetupHistory(System.Drawing.Size undoParameter, System.Drawing.Size redoParameter)
        {
            this.UndoParameter = undoParameter;
            this.RedoParameter = redoParameter;
        }

        public void Dispose()
        {
        }
    }
}