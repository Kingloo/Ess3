using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ess3.Library.Operations
{
    public abstract class OperationBase
    {
        public event EventHandler<OperationStatusChangedEventArgs> StatusChanged = delegate { };
        private void OnStatusChanged(Status status)
            => StatusChanged.Invoke(this, new OperationStatusChangedEventArgs(status));

        public abstract OperationType OperationType { get; protected set; }
        public abstract Status Status { get; protected set; }

        protected OperationBase()
        {
            Status = Status.Created;
        }

        public void Run(Action action)
            => Run(action, CancellationToken.None);

        public void Run(Action action, CancellationToken token)
        {
            _ = Task.Run(() => MonitorOperation(action, token));
        }

        private async Task MonitorOperation(Action action, CancellationToken token)
        {
            using Task task = new Task(action, token, TaskCreationOptions.LongRunning);
            task.Start();

            OnStatusChanged(Status.Started);
            
            while (!token.IsCancellationRequested && IsTaskStillGoing(task.Status))
            {
                Status = task.Status switch
                {
                    TaskStatus.RanToCompletion => Status.Succeeded,
                    TaskStatus.Canceled => Status.Canceled,
                    TaskStatus.Faulted => Status.Failed,
                    TaskStatus.Running => Status.Running,
                    _ => Status.Unknown
                };

                OnStatusChanged(Status);

                await Task.Delay(TimeSpan.FromMilliseconds(750d)).ConfigureAwait(false);
            }

            OnStatusChanged(Status.Finished);

            await task.ConfigureAwait(false);
        }

        private static bool IsTaskStillGoing(TaskStatus status)
            => status switch
            {
                TaskStatus.RanToCompletion => false,
                TaskStatus.Faulted => false,
                TaskStatus.Canceled => false,
                _ => true
            };
    }
}
