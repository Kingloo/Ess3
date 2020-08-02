using System.Collections.Generic;
using Ess3.Gui.Common;
using Ess3.Library.Interfaces;

namespace Ess3.Gui.Interfaces
{
    public interface IAccountControlViewModel
    {
        IReadOnlyCollection<IAccount> Accounts { get; }
        DelegateCommandAsync<IAccount> UpdateAccountCommand { get; }
        DelegateCommand AddAccountCommand { get; }
        DelegateCommand<IAccount>? RemoveAccountCommand { get; }

        void AddAccount(IAccount account);
        void RemoveAccount(IAccount account);
    }
}
