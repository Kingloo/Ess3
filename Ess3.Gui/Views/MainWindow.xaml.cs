using System.Diagnostics;
using System.Windows;
using Ess3.Gui.Interfaces;
using Ess3.Gui.ViewModels;

namespace Ess3.Gui.Views
{
    public partial class MainWindow : Window
    {
        private readonly IMainWindowViewModel viewModel;

        public MainWindow()
            : this(new MainWindowViewModel())
        { }

        public MainWindow(IMainWindowViewModel viewModel)
        {
            InitializeComponent();

            this.viewModel = viewModel;

            DataContext = this.viewModel;
        }

        private void AccountControl_AccountChanged(object sender, AccountChangedEventArgs e)
        {
            Debug.WriteLine(e.Account.DisplayName);

            ((IFileControlViewModel)fileControl.DataContext).Account = e.Account;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            UnregisterName("fileControl");
        }
    }
}
