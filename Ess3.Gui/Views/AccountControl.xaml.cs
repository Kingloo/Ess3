using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using Ess3.Gui.Interfaces;
using Ess3.Gui.ViewModels;
using Ess3.Library.Interfaces;

namespace Ess3.Gui.Views
{
    public partial class AccountControl : UserControl
    {
        public event EventHandler<AccountChangedEventArgs> AccountChanged = delegate { };
        private void OnAccountChanged(IAccount account)
            => AccountChanged?.Invoke(this, new AccountChangedEventArgs(account));

        private readonly IAccountControlViewModel viewModel;

        public AccountControl()
            : this(new AccountControlViewModel())
        { }

        public AccountControl(IAccountControlViewModel viewModel)
        {
            InitializeComponent();

            this.viewModel = viewModel;

            DataContext = this.viewModel;
        }

        private void comboBox_Selected(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine($"selected: {e.Source.GetType().FullName}");
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 1 && e.AddedItems[0] is IAccount account)
            {
                OnAccountChanged(account);
            }
            else
            {
                Debug.WriteLine($"selection changed failed ({e.AddedItems.Count})");
            }
        }
    }
}
