using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Ess3.Gui.Common;
using Ess3.Gui.Interfaces;
using Ess3.Gui.Views;
using Ess3.Library;
using Ess3.Library.Interfaces;

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
            //AddFakeData();
        }

        public async Task UpdateAccountAsync(IAccount account)
        {
            if (!(account is null))
            {
                await Task.Delay(TimeSpan.FromSeconds(0.8d)).ConfigureAwait(false);

                account.Name = "fred.jones " + DateTime.Now.ToShortTimeString();
            }
        }

        private void OpenAddAccountWindow()
        {
            var addAccountWindow = new AddAccountWindow();

            var returned = addAccountWindow.ShowDialog();

            if (returned.HasValue && returned.Value)
            {
                var vm = (IAddAccountWindowViewModel)addAccountWindow.DataContext;

                IAccount account = new Account
                {
                    AWSAccessKey = vm.AWSAccessKey,
                    AWSSecretKey = vm.AWSSecretKey
                };

                _accounts.Add(account);
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

        private void AddFakeData()
        {
            _accounts.Add(new Account { Name = "fred.jones" });
            _accounts.Add(new Account { Name = "claudia.black" });
        }
    }
}
