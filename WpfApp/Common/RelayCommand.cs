using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WpfApp.Common
{
    class RelayCommand<T> : ICommand
    {
        public event EventHandler CanExecuteChanged;
        public Action<T> Action { get; }

        public RelayCommand(Action<T> action)
        {
            Action = action;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            Action?.Invoke((T)parameter);
        }
    }
}
