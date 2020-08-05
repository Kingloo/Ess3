using System;
using System.Threading;
using System.Threading.Tasks;
using Ess3.Library.Interfaces;

namespace Ess3.Library
{
    /**
     * should this class be abstract? then have DownloadOperation, UploadOperation etc.
     * this class would handle looping, cancellation, maybe other stuff
     */
    public class Operation : IOperation
    {
        private Task? task = null;

        public OperationType OperationType { get; set; } = OperationType.None;

        public DateTime Created { get; } = DateTime.UtcNow;

        public DateTime Started { get; } = DateTime.MinValue;

        public DateTime Finished { get; } = DateTime.MaxValue;

        public Operation() { }

        public void Start()
        {
            
        }

        public void Cancel()
        {

        }
    }
}
