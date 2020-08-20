using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Ess3.Gui.Common;
using Ess3.Gui.Interfaces;
using Ess3.Gui.Views;
using Ess3.Library;
using Ess3.Library.Interfaces;
using Ess3.Library.S3;

namespace Ess3.Gui.ViewModels
{
    public class AccountControlViewModel : BindableBase, IAccountControlViewModel
    {
        private readonly ObservableCollection<IAccount> _accounts = new ObservableCollection<IAccount>();
        public IReadOnlyCollection<IAccount> Accounts => _accounts;

        private DelegateCommandAsync<IAccount>? _updateAccountCommand = null;
        public DelegateCommandAsync<IAccount> UpdateAccountCommand
        {
            get
            {
                if (_updateAccountCommand is null)
                {
                    _updateAccountCommand = new DelegateCommandAsync<IAccount>(UpdateAccountAsync, CanExecute);
                }

                return _updateAccountCommand;
            }
        }

        private DelegateCommand? _addAccountCommand = null;
        public DelegateCommand AddAccountCommand
        {
            get
            {
                if (_addAccountCommand is null)
                {
                    _addAccountCommand = new DelegateCommand(OpenAddAccountWindow, (_) => true);
                }

                return _addAccountCommand;
            }
        }

        private DelegateCommand<IAccount>? _removeAccountCommand = null;
        public DelegateCommand<IAccount> RemoveAccountCommand
        {
            get
            {
                if (_removeAccountCommand is null)
                {
                    _removeAccountCommand = new DelegateCommand<IAccount>(RemoveAccount, (_) => true);
                }

                return _removeAccountCommand;
            }
        }

        private DelegateCommand<IAccount?>? _setSelectedAccountCommand = null;
        public DelegateCommand<IAccount?> SetSelectedAccountCommand
        {
            get
            {
                if (_setSelectedAccountCommand is null)
                {
                    _setSelectedAccountCommand = new DelegateCommand<IAccount?>(SetSelectedAccount, (_) => true);
                }

                return _setSelectedAccountCommand;
            }
        }

        private bool CanExecute(object _) => true;

        private IAccount? _selectedAccount = null;
        public IAccount? SelectedAccount
        {
            get => _selectedAccount;
            set => SetProperty(ref _selectedAccount, value, nameof(SelectedAccount));
        }

        public AccountControlViewModel()
        {
            AddFakeAccounts();
        }

        public async Task UpdateAccountAsync(IAccount account)
        {
            if (account is null)
            {
                Debug.WriteLine("account was null");
                return;
            }

            if (!account.IsValidated)
            {
                Debug.WriteLine($"account has not been validated (displayName: {account.DisplayName}, accessKey: {account.AWSAccessKey})");
                return;
            }

            Int64 bucketSize = await S3Helpers.GetBucketSizeAsync(account, "kingloobackup");

            switch (bucketSize)
            {
                case -1L:
                    Debug.WriteLine("bucket name was null or empty");
                    break;
                case -2L:
                    Debug.WriteLine("bucket does not exist");
                    break;
                default:
                    Debug.WriteLine($"bucket size: {bucketSize}");
                    break;
            }
        }

        private void OpenAddAccountWindow()
        {
            var addAccountWindow = new AddAccountWindow();

            bool? returned = addAccountWindow.ShowDialog();

            if (returned.HasValue && returned.Value)
            {
                var addAccountWindowViewModel = (IAddAccountWindowViewModel)addAccountWindow.DataContext;

                if (addAccountWindowViewModel.Account is null)
                {
                    throw new Exception($"{nameof(addAccountWindow)} DialogResult was true but the account was null");
                }

                AddAccount(addAccountWindowViewModel.Account);
            }
        }

        public void AddAccount(IAccount account)
        {
            if (!_accounts.Contains(account))
            {
                _accounts.Add(account);
            }
        }

        public void RemoveAccount(IAccount account)
        {
            if (_accounts.Contains(account))
            {
                _accounts.Remove(account);
            }
        }

        public void SetSelectedAccount(IAccount? account)
        {
            SelectedAccount = account;
        }

        private void AddFakeAccounts()
        {
            var fred = new Account
            {
                DisplayName = "fred.jones",
                AWSAccessKey = "fredjonessaccesskey"
            };
            fred.AddFakeBuckets();
            fred.AddFakeFiles();

            var claudia = new Account
            {
                DisplayName = "claudia.black",
                AWSAccessKey = "claudiablackssecretkey"
            };
            claudia.AddFakeBuckets();
            claudia.AddFakeFiles();

            AddAccount(fred);
            AddAccount(claudia);
        }
    }
}
