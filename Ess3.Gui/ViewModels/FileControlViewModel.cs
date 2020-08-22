using System.Diagnostics;
using System.Threading.Tasks;
using Ess3.Gui.Common;
using Ess3.Gui.Interfaces;
using Ess3.Library.Interfaces;
using Ess3.Library.Model;
using Ess3.Library.S3;

namespace Ess3.Gui.ViewModels
{
    public class FileControlViewModel : BindableBase, IFileControlViewModel
    {
        private DelegateCommandAsync<Ess3Bucket>? _updateBucketCommand = null;
        public DelegateCommandAsync<Ess3Bucket> UpdateBucketCommand
        {
            get
            {
                if (_updateBucketCommand is null)
                {
                    _updateBucketCommand = new DelegateCommandAsync<Ess3Bucket>(UpdateBucketAsync, CanExecute);
                }

                return _updateBucketCommand;
            }
        }

        private DelegateCommandAsync<Ess3Directory>? _deleteDirectoryCommand = null;
        public DelegateCommandAsync<Ess3Directory> DeleteDirectoryCommand
        {
            get
            {
                if (_deleteDirectoryCommand is null)
                {
                    _deleteDirectoryCommand = new DelegateCommandAsync<Ess3Directory>(DeleteDirectoryAsync, CanExecute);
                }

                return _deleteDirectoryCommand;
            }
        }

        private IAccount? _account = null;
        public IAccount? Account
        {
            get => _account;
            set => SetProperty(ref _account, value, nameof(Account));
        }

        private Ess3Bucket? _selectedBucket = null;
        public Ess3Bucket? SelectedBucket
        {
            get => _selectedBucket;
            set => SetProperty(ref _selectedBucket, value, nameof(SelectedBucket));
        }

        public FileControlViewModel() { }

        public FileControlViewModel(IAccount account)
        {
            Account = account;
        }

        public async Task UpdateBucketAsync(Ess3Bucket bucket)
        {
            if (!(Account is null))
            {
                bucket.Clear();

                await Helpers.UpdateBucketAsync(Account, bucket);
            }
        }

        public async Task DeleteDirectoryAsync(Ess3Directory directory)
        {
            if (Account is null || SelectedBucket is null)
            {
                Debug.WriteLine("account or selected bucket was null");

                return;
            }

            bool success = await Delete.DirectoryAsync(Account, SelectedBucket, directory, failOnHasItems: true);

            Debug.WriteLine($"deleting directory succeeded: {success}");
        }

        private bool CanExecute(object _) => true;
    }
}
