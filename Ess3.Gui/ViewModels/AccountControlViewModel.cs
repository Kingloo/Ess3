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
    public class AccountControlViewModel : IAccountControlViewModel
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

        private bool CanExecute(object _) => true;

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

            var returned = addAccountWindow.ShowDialog();

            if (returned.HasValue && returned.Value)
            {
                var vm = (IAddAccountWindowViewModel)addAccountWindow.DataContext;

                if (vm.Account is null)
                {
                    throw new Exception("validating account failed");
                }

                _accounts.Add(vm.Account);
            }
        }

        public void AddAccount(IAccount account)
        {
            _accounts.Add(account);
        }

        public void RemoveAccount(IAccount account)
        {
            _accounts.Remove(account);
        }

        private void AddFakeAccounts()
        {
            _accounts.Add(new Account { DisplayName = "fred.jones" });
            _accounts.Add(new Account { DisplayName = "claudia.black" });
        }
    }
}
