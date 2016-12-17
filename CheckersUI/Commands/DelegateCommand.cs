using System;

namespace CheckersUI.Command
{
    public class DelegateCommand : CommandBase
    {
        private readonly Predicate<object> _canExecute;
        private readonly Action<object> _execute;

        public DelegateCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            _canExecute = canExecute;
            _execute = execute;
        }

        public override bool CanExecute(object parameter) =>
            _canExecute == null || _canExecute.Invoke(parameter);

        public override void Execute(object parameter) =>
            _execute.Invoke(parameter);
    }
}
