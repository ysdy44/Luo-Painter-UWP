namespace Luo_Painter.Historys.Models
{
    public interface IPropertyHistory : IHistory
    {
        string Id { get; }
        object UndoParameter { get; }
        object RedoParameter { get; }
    }

    public class PropertyHistory<T> : PropertyHistory<T, T>
    {
        public PropertyHistory(HistoryType type, string id, T undoParameter, T redoParameter) : base(type, id, undoParameter, redoParameter) { }
    }

    public class PropertyHistory<TUndo, TRedo> : IPropertyHistory
    {
        public HistoryMode Mode => HistoryMode.Property;
        public HistoryType Type => this.type;

        public string Id => this.id;
        public object UndoParameter => this.undoParameter;
        public object RedoParameter => this.redoParameter;

        readonly HistoryType type;
        readonly string id;
        readonly TUndo undoParameter;
        readonly TRedo redoParameter;

        public PropertyHistory(HistoryType type, string id, TUndo undoParameter, TRedo redoParameter)
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