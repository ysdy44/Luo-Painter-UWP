namespace Luo_Painter.Layers
{
    public interface IDrag
    {
        int Depth { get; set; }

        bool IsExist { get; set; }

        bool IsExpand { get; set; }

        void CacheIsExpand();
        void ApplyIsExpand();

        void Arrange(int depth);

        void Exist(bool isExist);

        void RaiseCanExecuteChanged();
        bool CanExecute(object parameter);
        void Execute(object parameter);
    }
}