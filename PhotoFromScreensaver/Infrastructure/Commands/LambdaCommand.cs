using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photos_Windows_spotlight.Infrastructure.Commands.Base;

namespace Photos_Windows_spotlight.Infrastructure.Commands
{
    class LambdaCommand : Command
    {
        private readonly Action<object> _executeAction;
        private readonly Func<object, bool> _canExecuteFunc;

        public LambdaCommand(Action<object> executeAction, Func<object, bool> canExecuteFunc = null)
        {
            _executeAction = executeAction ?? throw new ArgumentNullException(nameof(Execute));
            _canExecuteFunc = canExecuteFunc;
        }

        protected override void Execute(object p) => _executeAction(p);

        protected override bool CanExecute(object p) => _canExecuteFunc?.Invoke(p) ?? true;
    }
}
