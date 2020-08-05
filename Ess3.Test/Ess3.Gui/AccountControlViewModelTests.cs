using System;
using Ess3.Gui.Interfaces;
using Ess3.Gui.ViewModels;
using Ess3.Library;
using Ess3.Library.Interfaces;
using NUnit.Framework;

namespace Ess3.Test.Ess3.Gui
{
    [TestFixture]
    public class AccountControlViewModelTests
    {
        [Test]
        public void AddAccount_AddsAccount()
        {
            IAccountControlViewModel vm = new AccountControlViewModel();

            vm.AddAccount(new Account { DisplayName = "test account" });

            int expected = 1;
            int actual = vm.Accounts.Count;

            Assert.AreEqual(expected, actual, "count should have been {0}, but actually was {1}", expected, vm.Accounts.Count);
        }

        [Test]
        public void RemoveAccount_RemovesAccount()
        {
            IAccountControlViewModel vm = new AccountControlViewModel();

            IAccount account = new Account { DisplayName = "test account" };

            vm.AddAccount(account);

            if (vm.Accounts.Count != 1)
            {
                throw new Exception("adding account failed");
            }

            vm.RemoveAccount(account);

            Assert.Zero(vm.Accounts.Count, "vm.Accounts should have been empty, but it actually contained {0}", vm.Accounts.Count);
        }
    }
}
