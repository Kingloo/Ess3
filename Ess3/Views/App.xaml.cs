using System;
using System.Windows;
using System.Windows.Threading;
using Ess3.Common;
using Ess3.Model;
using Ess3.ViewModels;

namespace Ess3.Views
{
    public partial class App : Application
    {
        public App(Ess3Settings settings)
        {
            if (settings is null) { throw new ArgumentNullException(nameof(settings)); }

            InitializeComponent();

            MainWindow = new MainWindow(new MainWindowViewModel(settings));

            MainWindow.Show();
        }

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Log.Exception(e.Exception);
        }
    }
}
