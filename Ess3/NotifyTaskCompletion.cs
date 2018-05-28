using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ess3
{
    class NotifyTaskCompletion<TResult> : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Task<TResult> Task { get; private set; }
        public Task TaskCompletion { get; private set; }
        public TResult Result
        {
            get
            {
                return (this.Task.Status == TaskStatus.RanToCompletion) ? Task.Result : default(TResult);
            }
        }
        public TaskStatus Status { get { return this.Task.Status; } }
        public bool IsCompleted { get { return this.Task.IsCompleted; } }
        public bool IsNotCompleted { get { return !this.Task.IsCompleted; } }
        public bool IsSuccessfullyCompleted
        {
            get
            {
                if (this.Task.Status == TaskStatus.RanToCompletion)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public bool IsCanceled { get { return this.Task.IsCanceled; } }
        public bool IsFaulted { get { return this.Task.IsFaulted; } }
        public AggregateException Exception { get { return this.Task.Exception; } }
        public Exception InnerException
        {
            get
            {
                return (this.Exception == null) ? null : this.Exception.InnerException;
            }
        }
        public string ErrorMessage
        {
            get
            {
                return (this.InnerException == null) ? null : this.InnerException.Message;
            }
        }

        public NotifyTaskCompletion(Task<TResult> task)
        {
            this.Task = task;

            if (!task.IsCompleted)
            {
                this.TaskCompletion = WatchTaskAsync(task);
            }
        }

        private async Task WatchTaskAsync(Task task)
        {
            try
            {
                await task;
            }
            catch { }

            PropertyChangedEventHandler pceh = this.PropertyChanged;

            if (pceh == null)
            {
                return;
            }

            pceh(this, new PropertyChangedEventArgs("Status"));
            pceh(this, new PropertyChangedEventArgs("IsCompleted"));
            pceh(this, new PropertyChangedEventArgs("IsNotCompleted"));

            if (task.IsCanceled)
            {
                pceh(this, new PropertyChangedEventArgs("IsCanceled"));
            }
            else if (task.IsFaulted)
            {
                pceh(this, new PropertyChangedEventArgs("IsFaulted"));
                pceh(this, new PropertyChangedEventArgs("Exception"));
                pceh(this, new PropertyChangedEventArgs("InnerException"));
                pceh(this, new PropertyChangedEventArgs("ErrorMessage"));
            }
            else
            {
                pceh(this, new PropertyChangedEventArgs("IsSuccessfullyCompleted"));
                pceh(this, new PropertyChangedEventArgs("Result"));
            }
        }
    }
}
