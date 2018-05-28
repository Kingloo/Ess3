using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Ess3
{
    interface IAsyncCommand : ICommand
    {
        Task ExecuteAsync(object parameter);
    }

    abstract class AsyncCommandBase : IAsyncCommand
    {
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public async void Execute(object parameter)
        {
            await ExecuteAsync(parameter);
        }

        public abstract Task ExecuteAsync(object parameter);

        public abstract bool CanExecute(object parameter);

        protected void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
    
    class AsyncCommand<TResult> : AsyncCommandBase, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler pceh = this.PropertyChanged;
            if (pceh != null)
            {
                pceh(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private readonly Func<CancellationToken, Task<TResult>> _command = null;
        private readonly CancelAsyncCommand _cancelCommand = null;
        private NotifyTaskCompletion<TResult> _execution = null;
        public NotifyTaskCompletion<TResult> Execution
        {
            get { return this._execution; }
            private set
            {
                this._execution = value;
                this.OnPropertyChanged("Execution");
            }
        }
        public ICommand CancelCommand { get { return this._cancelCommand; } }
        
        public AsyncCommand(Func<CancellationToken, Task<TResult>> command)
        {
            this._command = command;
            this._cancelCommand = new CancelAsyncCommand();
        }

        public override async Task ExecuteAsync(object parameter)
        {
            this._cancelCommand.NotifyCommandStarting();

            this.Execution = new NotifyTaskCompletion<TResult>(_command(_cancelCommand.Token));

            RaiseCanExecuteChanged();

            await this.Execution.TaskCompletion;

            this._cancelCommand.NotifyCommandFinished();

            RaiseCanExecuteChanged();
        }

        public override bool CanExecute(object parameter)
        {
            return (this.Execution == null) || (this.Execution.IsCompleted);
        }

        private sealed class CancelAsyncCommand : ICommand
        {
            private CancellationTokenSource _cts = new CancellationTokenSource();
            public CancellationToken Token { get { return this._cts.Token; } }
            private bool _commandExecuting = false;

            public void NotifyCommandStarting()
            {
                this._commandExecuting = true;

                if (!this._cts.IsCancellationRequested)
                {
                    return;
                }

                this._cts = new CancellationTokenSource();

                RaiseCanExecuteChanged();
            }

            public void NotifyCommandFinished()
            {
                this._commandExecuting = false;
                RaiseCanExecuteChanged();
            }

            bool ICommand.CanExecute(object parameter)
            {
                return (this._commandExecuting) && (!this._cts.IsCancellationRequested);
            }

            void ICommand.Execute(object parameter)
            {
                this._cts.Cancel();

                RaiseCanExecuteChanged();
            }

            public event EventHandler CanExecuteChanged
            {
                add { CommandManager.RequerySuggested += value; }
                remove { CommandManager.RequerySuggested -= value; }
            }

            private void RaiseCanExecuteChanged()
            {
                CommandManager.InvalidateRequerySuggested();
            }
        }
    }
}
