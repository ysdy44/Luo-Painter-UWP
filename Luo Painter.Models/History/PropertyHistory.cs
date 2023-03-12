namespace Luo_Painter.Models
{
    public class PropertyHistory : IHistory
    {
        public HistoryMode Mode => HistoryMode.Property;
        public HistoryPropertyMode PropertyMode { get; }

        public string Id { get; }
        public object UndoParameter { get; }
        public object RedoParameter { get; }
        public PropertyHistory(HistoryPropertyMode propertyMode, string id, object undoParameter, object redoParameter)
        {
            this.PropertyMode = propertyMode;
            this.Id = id;
            this.UndoParameter = undoParameter;
            this.RedoParameter = redoParameter;
        }

        public void Dispose()
        {
        }
    }
}