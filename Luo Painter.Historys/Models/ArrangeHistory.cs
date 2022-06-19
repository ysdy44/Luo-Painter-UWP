using System;
using Windows.Graphics.Imaging;

namespace Luo_Painter.Historys.Models
{
    public sealed class SetupSizes
    {
        public BitmapSize UndoParameter;
        public BitmapSize RedoParameter;
    }

    public class ArrangeHistory : IHistory
    {
        public HistoryMode Mode => HistoryMode.Arrange;
        public HistoryType Type => HistoryType.None;
        public string[] UndoParameter { get; }
        public string[] RedoParameter { get; }
        public SetupSizes Sizes { get; private set; }
        public ArrangeHistory(string[] undoParameter, string[] redoParameter, SetupSizes sizes = null)
        {
            this.UndoParameter = undoParameter;
            this.RedoParameter = redoParameter;
            this.Sizes = sizes;
        }
        public void Dispose()
        {
            Array.Clear(this.UndoParameter, 0, this.UndoParameter.Length);
            Array.Clear(this.RedoParameter, 0, this.RedoParameter.Length);
            this.Sizes = null;
        }
    }
}