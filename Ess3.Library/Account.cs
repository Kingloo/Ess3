using System;
using Ess3.Library.Interfaces;

namespace Ess3.Library
{
    public class Account : IAccount
    {
        private string _displayName = string.Empty;
        public string DisplayName
        {
            get
            {
                if (String.IsNullOrWhiteSpace(_displayName))
                {
                    return AWSAccessKey;
                }
                else
                {
                    return _displayName;
                }
            }
            set => _displayName = value;
        }

        public string Id { get; set; } = string.Empty;
        public string AWSAccessKey { get; set; } = string.Empty;
        public string AWSSecretKey { get; set; } = string.Empty;
        public bool IsValidated { get; set; } = false;

        public Account() { }
    }
}
