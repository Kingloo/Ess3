using System;
using System.Collections.Generic;
using Amazon;
using Amazon.Runtime;
using Ess3.Library.Model;

namespace Ess3.Library.Interfaces
{
    public interface IAccount : IEquatable<IAccount>
    {
        string DisplayName { get; set; }
        string Id { get; set; }
        string AWSAccessKey { get; set; }
        string AWSSecretKey { get; set; }
        bool IsValidated { get; set; }
        IReadOnlyCollection<Ess3Bucket> Buckets { get; }

        AWSCredentials GetCredentials();
    }
}
