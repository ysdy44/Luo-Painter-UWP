namespace Luo_Painter.Models.Historys
{
    public class PropertyHistory : IHistory
    {
        public HistoryMode Mode => HistoryMode.Property;
        public HistoryPropertyMode PropertyMode => this.type;

        public string Id => this.id;
        public object UndoParameter => this.undoParameter;
        public object RedoParameter => this.redoParameter;

        readonly HistoryPropertyMode type;
        readonly string id;
        readonly object undoParameter;
        readonly object redoParameter;

        public PropertyHistory(HistoryPropertyMode type, string id, object undoParameter, object redoParameter)
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