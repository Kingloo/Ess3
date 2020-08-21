using System.Globalization;
using System.Windows;
using System.Windows.Markup;
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

            Language = XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag);

            this.viewModel = viewModel;

            DataContext = this.viewModel;
        }

        private void AccountControl_AccountChanged(object sender, AccountChangedEventArgs e)
        {
            ((IFileControlViewModel)fileControl.DataContext).Account = e.Account;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            UnregisterName("fileControl");
        }
    }
}
