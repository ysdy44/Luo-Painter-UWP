using System;
using System.Windows.Input;

namespace Luo_Painter.Elements
{
    /// <summary>
    /// Relay Command is used to separate UI objects and code logic.
    /// </summary>
    public class RelayCommand<T> : ICommand
    {
        //@Delegate
        /// <summary>
        /// Occurs when clicked.
        /// </summary>
        public event EventHandler<T> Click;
        /// <summary>
        /// Occurs when there is a change that affects whether the command should be executed
        /// </summary>
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => parameter != default;
        public void Execute(object parameter)
        {
            if (parameter is T item)
            {
                this.Click?.Invoke(this, item);//Delegate
            }
        }
    }
}