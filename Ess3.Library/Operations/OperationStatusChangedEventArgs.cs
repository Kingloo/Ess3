using System;

namespace Ess3.Library.Operations
{
    public class OperationStatusChangedEventArgs : EventArgs
    {
        public Status Status { get; } = Status.None;

        public OperationStatusChangedEventArgs(Status status)
        {
            Status = status;
        }
    }
}