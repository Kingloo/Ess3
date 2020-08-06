using System;
using System.Collections.Generic;
using System.Text;

namespace Ess3.Library.Operations
{
    public class Download : OperationBase
    {
        public override OperationType OperationType { get; protected set; } = OperationType.Download;
        public override Status Status { get; protected set; }

        public Download() { }
    }
}
