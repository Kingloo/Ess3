using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Ess3.Library.Interfaces;

namespace Ess3.Library
{
    public class Account : IAccount
    {
        private string _name = string.Empty;
        public string Name
        {
            get
            {
                if (String.IsNullOrWhiteSpace(_name))
                {
                    return AWSAccessKey;
                }
                else
                {
                    return _name;
                }
            }
            set => _name = value;
        }

        public string Id { get; set; } = string.Empty;
        public string AWSAccessKey { get; set; } = string.Empty;
        public string AWSSecretKey { get; set; } = string.Empty;

        public Account() { }
    }
}
