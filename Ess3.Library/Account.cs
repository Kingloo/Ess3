using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Amazon.Runtime;
using Ess3.Library.Common;
using Ess3.Library.Interfaces;
using Ess3.Library.Model;

namespace Ess3.Library
{
    public class Account : BindableBase, IAccount
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
            set => SetProperty(ref _displayName, value, nameof(DisplayName));
        }

        private string _id = string.Empty;
        public string Id
        {
            get => _id;
            set => SetProperty(ref _id, value, nameof(Id));
        }

        private string _awsAccessKey = string.Empty;
        public string AWSAccessKey
        {
            get => _awsAccessKey;
            set => SetProperty(ref _awsAccessKey, value, nameof(AWSAccessKey));
        }

        private string _awsSecretKey = string.Empty;
        public string AWSSecretKey
        {
            get => _awsSecretKey;
            set => SetProperty(ref _awsSecretKey, value, nameof(AWSSecretKey));
        }

        private bool _isValidated = false;
        public bool IsValidated
        {
            get => _isValidated;
            set => SetProperty(ref _isValidated, value, nameof(IsValidated));
        }

        private readonly ObservableCollection<Ess3Bucket> _buckets = new ObservableCollection<Ess3Bucket>();
        public IReadOnlyCollection<Ess3Bucket> Buckets => _buckets;

        public Account() { }

        public AWSCredentials GetCredentials() => new BasicAWSCredentials(AWSAccessKey, AWSSecretKey);

        public void AddFile(Ess3File file)
        {
            _buckets.First().AddFile(file);
        }

        public void RemoveFile(Ess3File file)
        {
            throw new NotImplementedException();
        }

        public void AddDirectory(Ess3Directory directory)
        {
            _buckets.First().AddDirectory(directory);
        }

        public void RemoveDirectory(Ess3Directory directory)
        {
            throw new NotImplementedException();
        }

        public void ClearFiles()
        {
            throw new NotImplementedException();
        }

        public void ClearDirectories()
        {
            throw new NotImplementedException();
        }

        public void AddFakeBuckets()
        {
            _buckets.Add(new Ess3Bucket(DisplayName + "'s first bucket", DateTime.Now));
            _buckets.Add(new Ess3Bucket(DisplayName + "'s second bucket", DateTime.Now));
            _buckets.Add(new Ess3Bucket(DisplayName + "'s third bucket", DateTime.Now));
        }

        public void AddFakeFiles()
        {
            AddFile(
                new Ess3File
                {
                    BucketName = _buckets.First().BucketName,
                    Key = $"{DisplayName}'s first file",
                    Size = DisplayName.Length + 6
                });

            AddFile(
                new Ess3File
                {
                    BucketName = _buckets.First().BucketName,
                    Key = $"{DisplayName}'s second file",
                    Size = DisplayName.Length
                });
        }

        public bool Equals(IAccount other)
            => AWSAccessKey.Equals(other.AWSAccessKey, StringComparison.Ordinal)
            && AWSSecretKey.Equals(other.AWSSecretKey, StringComparison.Ordinal);        
    }
}
