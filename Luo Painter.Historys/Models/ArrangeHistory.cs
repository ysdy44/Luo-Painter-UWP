using System.Collections.Generic;
using System.Linq;
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
        public Layerage[] UndoParameter { get; }
        public Layerage[] RedoParameter { get; }
        public SetupSizes Sizes { get; private set; }
        public ArrangeHistory(Layerage[] undoParameter, Layerage[] redoParameter, SetupSizes sizes = null)
        {
            this.UndoParameter = undoParameter;
            this.RedoParameter = redoParameter;
            this.Sizes = sizes;
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
            this.Sizes = null;
        }
    }
}