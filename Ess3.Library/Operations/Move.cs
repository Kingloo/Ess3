using System;
using System.Collections.Generic;
using System.Text;

namespace Ess3.Library.Operations
{
    public class Move : OperationBase
    {
        public override OperationType OperationType { get; protected set; } = OperationType.Move;
        public override Status Status { get; protected set; }

        public Move() { }
    }
}
