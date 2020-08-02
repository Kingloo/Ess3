using System;

namespace Ess3.Library.Interfaces
{
    public interface IOperation
    {
        DateTime Created { get; }
        DateTime Started { get; }
        DateTime Finished { get; }

        void Cancel();
    }
}