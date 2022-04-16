using System.Collections.Generic;

namespace Luo_Painter.Historys.Models
{
    public interface IPropertysHistory : IHistory
    {
        HistoryType PropertyType { get; }
        object this[string id] { get; }
        IEnumerable<string> Ids { get; }
        object UndoParameter { get; }
        object RedoParameter { get; }
    }

    public class PropertysHistory<T> : IPropertysHistory
    {
        public HistoryType Type => this.type;
        public HistoryType PropertyType => this.propertyType;

        public object this[string id] => this.undoParameters[id];
        public IEnumerable<string> Ids => this.undoParameters.Keys;
        public object UndoParameter => this.undoParameter;
        public object RedoParameter => this.redoParameter;

        readonly HistoryType type;
        readonly HistoryType propertyType;
        readonly IDictionary<string, T> undoParameters;
        readonly T undoParameter;
        readonly T redoParameter;

        public PropertysHistory(HistoryType type, HistoryType propertyType, IDictionary<string, T> undoParameters, T undoParameter, T redoParameter)
        {
            this.type = type;
            this.propertyType = propertyType;
            this.undoParameters = undoParameters;
            this.undoParameter = undoParameter;
            this.redoParameter = redoParameter;
        }
        public void Dispose()
        {
            this.undoParameters.Clear();
        }
    }
}