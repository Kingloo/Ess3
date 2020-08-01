using System.Collections.Generic;
using Ess3.Library;

namespace Ess3.Gui.Interfaces
{
    public interface IOperationsControlViewModel
    {
        IReadOnlyCollection<IOperation> Operations { get; }

        public void Add(IOperation operation);
        public void Clear();
    }
}
