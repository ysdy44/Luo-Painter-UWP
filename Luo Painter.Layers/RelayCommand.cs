using System;
using System.Windows.Input;

namespace Luo_Painter.Layers
{
    /// <summary>
    /// Relay Command is used to separate UI objects and code logic.
    /// </summary>
    public class RelayCommand : ICommand
    {
        //@Delegate
        /// <summary>
        /// Occurs when clicked
        /// </summary>
        public event EventHandler<ILayer> Click;
        /// <summary>
        /// Occurs when there is a change that affects whether the command should be executed
        /// </summary>
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => true;
        public void Execute(object parameter)
        {
            if (parameter is ILayer item)
            {
                this.Click?.Invoke(this, item);//Delegate
            }
        }
    }
}