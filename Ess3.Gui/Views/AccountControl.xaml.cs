using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Ess3.Gui.Interfaces;
using Ess3.Gui.ViewModels;

namespace Ess3.Gui.Views
{
    public partial class AccountControl : UserControl
    {
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
    }
}
