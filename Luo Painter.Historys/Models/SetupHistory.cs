using Windows.Graphics.Imaging;

namespace Luo_Painter.Historys.Models
{
    public class SetupHistory : IHistory
    {
        public HistoryMode Mode => HistoryMode.Setup;
        public HistoryType Type => HistoryType.None;
        public HistoryType PropertyType => HistoryType.None;

        public readonly BitmapSize UndoParameter;
        public readonly BitmapSize RedoParameter;
        public SetupHistory(BitmapSize undoParameter, BitmapSize redoParameter)
        {
            this.UndoParameter = undoParameter;
            this.RedoParameter = redoParameter;
        }

        public void Dispose()
        {
        }
    }
}