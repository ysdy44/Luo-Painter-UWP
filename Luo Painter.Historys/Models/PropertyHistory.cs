namespace Luo_Painter.Historys.Models
{
    public interface IPropertyHistory : IHistory
    {
        string Id { get; }
        object UndoParameter { get; }
        object RedoParameter { get; }
    }

    public class PropertyHistory<T> : IPropertyHistory
    {
        public HistoryType Type => this.type;

        public string Id => this.id;
        public object UndoParameter => this.undoParameter;
        public object RedoParameter => this.redoParameter;

        readonly HistoryType type;
        readonly string id;
        readonly T undoParameter;
        readonly T redoParameter;

        public PropertyHistory(HistoryType type, string id, T undoParameter, T redoParameter)
        {
            this.type = type;
            this.id = id;
            this.undoParameter = undoParameter;
            this.redoParameter = redoParameter;
        }

        public void Dispose()
        {
        }
    }
}