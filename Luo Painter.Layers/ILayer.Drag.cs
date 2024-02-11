namespace Luo_Painter.Layers
{
    partial interface ILayer
    {
        int Depth { get; set; }

        bool IsExist { get; set; }

        bool IsExpand { get; set; }

        void CacheIsExpand();
        void ApplyIsExpand();

        void Arrange(int depth);

        void Exist(bool isExist);

        //@Command
        /// <summary>
        /// Command fot IsExpand
        /// </summary>
        void RaiseCanExecuteChanged();
        //bool CanExecute(object parameter);
        //void Execute(object parameter);
    }
}