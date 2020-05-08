using System;
using System.Diagnostics;
using System.Windows.Input;

namespace Technovert.WPF.GenericMVVM
{
    /// <summary>
    ///  Delegates the Command to another Method
    /// Reusable Implementation of ICommandInterface
    /// </summary>
    public class RelayCommand : ICommand {
        private readonly Func<bool> canExecute;
        private readonly Action execute;

        public RelayCommand(Action execute)
            : this(execute, null) {}
        /// <summary>
        /// Delegates the Command to another Method
        /// </summary>
        /// <param name="execute"> Method Name </param>
        /// <param name="canExecute"> Runs the Delegated Command only if returns True </param>
        public RelayCommand(Action execute, Func<bool> canExecute) {
            this.execute = execute ?? throw new ArgumentNullException("execute");
            this.canExecute = canExecute; 
        }

        public event EventHandler CanExecuteChanged {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        [DebuggerStepThrough]
        public bool CanExecute(object parameter) {
            return canExecute == null || canExecute();
        }

        public void Execute(object parameter) {
            execute();
        }
    }

    public class RelayCommand<T> : ICommand {
        private readonly Predicate<T> canExecute;
        private readonly Action<T> execute;

        public RelayCommand(Action<T> execute)
            : this(execute, null) {}

        public RelayCommand(Action<T> execute, Predicate<T> canExecute) {
            this.execute = execute ?? throw new ArgumentNullException("execute");
            this.canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        [DebuggerStepThrough]
        public bool CanExecute(object parameter) {
            return canExecute == null || canExecute((T) parameter);
        }

        public void Execute(object parameter) {
            execute((T) parameter);
        }
    }
}