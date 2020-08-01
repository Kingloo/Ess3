using System;

namespace Ess3.Library
{
    public class Operation : IOperation
    {
        public OperationType OperationType { get; set; } = OperationType.None;

        public DateTime Created { get; } = DateTime.UtcNow;

        public DateTime Started { get; } = DateTime.MinValue;

        public DateTime Finished { get; } = DateTime.MaxValue;

        public Operation() { }

        public void Cancel() { }
    }
}
