using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Photos_Windows_spotlight.Infrastructure.Commands.Base
{
    abstract class Command : ICommand
    {
        event EventHandler ICommand.CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        bool ICommand.CanExecute(object parameter) => CanExecute(parameter);
        
        void ICommand.Execute(object parameter) => Execute(parameter);

        protected virtual bool CanExecute(object p) => true;
        protected abstract void Execute(object p);
    }
}
