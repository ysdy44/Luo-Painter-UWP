using System;
using System.ComponentModel;
using System.Windows.Input;

namespace Luo_Painter.Layers
{
    public partial interface ILayer : ILayerRender, INotifyPropertyChanged, ICommand, IDisposable
    {
        int Depth { get; set; }

        bool IsExist { get; set; }

        bool IsExpand { get; set; }

        void CacheIsExpand();
        void ApplyIsExpand();

        void Arrange(int depth);

        void Exist(bool isExist);

        /// <summary>
        /// Command fot IsExpand
        /// </summary>
        void RaiseCanExecuteChanged();
        //bool CanExecute(object parameter);
        //void Execute(object parameter);
    }
}