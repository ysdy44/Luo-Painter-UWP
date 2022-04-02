using Windows.UI.Xaml;

namespace Luo_Painter.Historys.Models
{
    public class VisibilityHistory : IHistory
    {
        public HistoryType Type => HistoryType.Visibility;
        public string Id => this.IdCore;
        public object UndoParameter => this.UndoParameterCore;
        public object RedoParameter => this.RedoParameterCore;

        readonly string IdCore;
        readonly Visibility UndoParameterCore;
        readonly Visibility RedoParameterCore;

        public VisibilityHistory(string id, Visibility undo, Visibility redo)
        {
            this.IdCore = id;
            this.UndoParameterCore = undo;
            this.RedoParameterCore = redo;
        }
        public void Dispose()
        {
        }
    }
}