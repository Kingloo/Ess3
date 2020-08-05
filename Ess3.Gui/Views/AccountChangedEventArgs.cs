using System;
using Ess3.Library.Interfaces;

namespace Ess3.Gui.Views
{
    public class AccountChangedEventArgs : EventArgs
    {
        public IAccount Account { get; }

        public AccountChangedEventArgs(IAccount account) => Account = account;
    }
}
